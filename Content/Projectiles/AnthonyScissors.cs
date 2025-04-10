using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using System.IO;
using MahouShoujyo.Content.NPCs.Enemies;

namespace MahouShoujyo.Content.Projectiles
{
    public class AnthonyScissors : ModProjectile
    {
        //材质
        private Texture2D tex = ModContent.Request<Texture2D>("MahouShoujyo/Content/Projectiles/AnthonyScissors").Value;
        bool initial = true;
        //拖尾绘制
        int frame_tail;
        Vector2[] pos_old;
        Vector2[] vel_old;
        ProjectileAudioTracker tracker;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            Main.projFrames[this.Type]=3;
        }
        //持有的NPC index，为-1时则为扔出的剪刀；
        public int held = -1;
        // Setting the default parameters of the projectile
        // You can check most of Fields and Properties here https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation
        public override void SetDefaults()
        {
            Projectile.width = 48; // The width of projectile hitbox
            Projectile.height = 32; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
            Projectile.DamageType = DamageClass.Magic; // What type of damage does this projectile affect?
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.penetrate = -1; //How many enemies could be hit and gone through.
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 0;//透明度，越大越透明，0-255.
            Projectile.frame=0;
            tracker = new ProjectileAudioTracker(Projectile);

            //轨迹记录
            frame_tail = 15;
            pos_old = new Vector2[frame_tail];
            vel_old = new Vector2[frame_tail];
            
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent)
            {
                if ( parent.Entity is NPC npc && npc.ModNPC is Anthony && parent.Context != null)
                {
                    held = npc.whoAmI;
                    if (Main.dedServ)
                    Projectile.netUpdate = true;
                    return;
                }
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)held);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            held = reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }
        public override void AI()
        {
            //记录轨迹
            if (Main.time % 3 == 0) MahouShoujyo.push(Projectile.Center, Projectile.velocity, frame_tail, ref pos_old, ref vel_old);
            Projectile.frame =((int)Projectile.ai[0] % 30) / 10;

            if (held >= 0)
            {
                if (Main.npc[held].active
                    && Main.npc[held].ModNPC is Anthony anthony
                    && anthony.state == 1)
                {
                    Projectile.Center = Main.npc[held].Center;
                    if ((int)Projectile.ai[0] % 60 < 30)
                        Projectile.velocity.Y = 10f;
                    else
                        Projectile.velocity.Y = -10f;
                }
                else
                {
                    Projectile.active = false;
                    if (Main.dedServ)
                        Projectile.netUpdate = true;
                }
                if (!Main.dedServ && (int)Projectile.ai[0] % 60 == 0)
                {
                    SoundEngine.PlaySound(new SoundStyle($"MahouShoujyo/Radio/Sound/scissorsCutting")
                    { PitchRange = (-0.1f, 0.1f), MaxInstances = 0 },
                        Projectile.Center, soundInstance =>
                    {
                        // The SoundUpdateCallback can be inlined if desired, such as in this example.
                        soundInstance.Position = Projectile.Center;
                        return tracker.IsActiveAndInGame();
                    });
                }

            }
            else
            {
                if (Projectile.ai[0] > 60)
                {
                    Projectile.velocity.Y += 1f;
                    Projectile.tileCollide = true;
                }
                if (Projectile.velocity.Y > 20f) Projectile.velocity.Y = 20f;
                if (!Main.dedServ && Projectile.ai[0] == 0)
                {
                    SoundEngine.PlaySound(new SoundStyle($"MahouShoujyo/Radio/Sound/scissorsCutting")
                    { PitchRange = (-0.1f, 0.1f), MaxInstances = 0 },
                    Projectile.Center);
                }
            }
            Projectile.ai[0]++;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float length = projHitbox.Width;
            float width = projHitbox.Height;
            float rotation = Projectile.velocity.ToRotation();
            Vector2 halfVector2 = (new Vector2(length / 2f , 0f)).RotatedBy(rotation);
            float collidingPoint = 0;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(),
                targetHitbox.Size(),
                projHitbox.Center()-halfVector2,
                projHitbox.Center()+halfVector2,
                lineWidth: width,
                ref collidingPoint))
                return true;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = frame_tail-1; i>0; i--)
                MahouShoujyo.draw_Center(
                    tex: tex,
                    frame_num: 3, frame: Projectile.frame,
                    pos: pos_old[i], color: Color.White*(0.5f-0.02f*i),
                    rot: (pos_old[i-1]-pos_old[i]).ToRotation(),
                    scale_X: 1-0.05f*i, scale_Y: 1-0.05f*i);
            MahouShoujyo.draw_Center(
                tex: tex,
                frame_num: 3, frame: Projectile.frame,
                pos: Projectile.Center, color: Color.White*1f,
                rot: Projectile.velocity.ToRotation(),
                scale_X: 1f, scale_Y: 1f);
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage > 0)
                if (Main.rand.NextBool(2)) target.AddBuff(BuffID.Cursed, 120);
            if (Main.netMode != NetmodeID.Server)
            for (int i = 0; i<10; i++)
            {
                Vector2 vel_dust = Projectile.velocity.SafeNormalize(Vector2.Zero)
                    .RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-5,5))) * 4f;
                Dust.NewDustDirect(target.position, target.width, target.height, DustID.Blood, vel_dust.X, vel_dust.Y);
            }
            
            
        }
        public override void OnKill(int timeLeft)
        {
            
            
        }
    }
}