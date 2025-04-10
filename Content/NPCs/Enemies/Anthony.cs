using MahouShoujyo.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using MahouShoujyo.Content.Items;
using MahouShoujyo.Content.Biomes;
using Microsoft.Xna.Framework;
using MahouShoujyo.Globals;
using Terraria.GameContent.ItemDropRules;
using System.IO;
using System;
using Humanizer;
using MahouShoujyo.Content.Projectiles;
using Terraria.ModLoader.Utilities;

namespace MahouShoujyo.Content.NPCs.Enemies
{
    public class Anthony : ModNPC
    {
        public int state = 0;
        private int coolDown = 0;
        private int skillTimer = 0;
        private const int loadingTime = 120;
        private const int cuttingTime = 180;
        private const int cuttingCD = 600;
        private const int throwingTime = 60;
        private int oldDirection = 1;

        //public static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 17;
            //if (!Main.dedServ)
            //    glowmask = ModContent.Request<Texture2D>("MahouShoujyo/Content/NPCs/Enemies/Anthony");
        }
        public override void SetDefaults()
        {
            NPC.damage = 50;
            NPC.width = 32;
            NPC.height = 48;
            NPC.aiStyle = NPCAIStyleID.Fighter;
            NPC.defense = 10;
            NPC.lifeMax = 200;
            NPC.friendly = false;
            NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(0, 0, 20, 0);
            NPC.DeathSound = SoundID.NPCDeath6;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<GriefSeed>();
            if (Main.hardMode)
            {
                NPC.damage = 100;
                NPC.defense = 20;
                NPC.lifeMax = 400;
            }
            if (DownedBossSystem.downedConciousBoss)
            {
                NPC.damage = (int)(NPC.damage * 1.5f);
                NPC.lifeMax = (int)(NPC.lifeMax * 2f);
            }
            SpawnModBiomes = new int[1] { ModContent.GetInstance<WitchNightBiome>().Type };

            NPC.npcSlots = 0.5f;
            NPC.AdjustExpert(1.5f);
            NPC.AdjustMaster(2.25f);
            NPC.Familiar(true);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            SpawnConditionBestiaryInfoElement ui = new SpawnConditionBestiaryInfoElement("Bestiary_Biomes.TheCorruption", 0);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface ,
                new FlavorTextBestiaryInfoElement(this.GetLocalizedValue("description"))
            });
        }
        //0站立，1空中，2-5移动，6-7前摇，8-11切，12-15扔，16僵直
        public override void FindFrame(int frameHeight)
        {
            int frame = 0;
            switch (state)
            {
                case -1:
                    frame= 16;

                    break;
                case 0:
                    if (Math.Abs(NPC.velocity.Y) < 1f)
                    {
                        if (Math.Abs(NPC.velocity.X) < 0.05f) frame = 0;
                        else frame = (((int)Main.time / 10) % 4) +2;
                    }
                    else frame = 1;
                    break;
                case 1:
                    //cutting
                    frame = (((int)Main.time / 30) % 4) +8;
                    break;
                case 2:
                    //throwing
                    frame = (((int)Main.time / 15) % 4) +12;
                    break;
                case 3:
                    //loading
                    frame = (((int)Main.time / 10) % 2) +6;
                    break;
                default:
                    break;
            }
            NPC.frame.Y = frame*frameHeight;
            if (state != -1) NPC.spriteDirection = NPC.direction * 1;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                SoundEngine.PlaySound(SoundID.NPCHit11, NPC.Center);
            }

        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw glowmask
            //spriteBatch.Draw(glowmask.Value, NPC.Center - screenPos, NPC.frame, Color.White * 0.6f, NPC.rotation, new Vector2(33, 31), 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (PlayerState.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (PlayerState.ZoneWitchNight(spawnInfo.Player))
            {
                return SpawnCondition.OverworldNightMonster.Chance* (DownedBossSystem.downedConciousBoss ? 0.25f  : 0.5f);
            }
            return 0f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((int)state);
            writer.Write((int)coolDown);
            writer.Write((int)skillTimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            state = reader.ReadInt32();
            coolDown = reader.ReadInt32();
            skillTimer = reader.ReadInt32();
        }

        public override bool PreAI()
        {
            checkState();
            switch (state)
            {
                case 0: 
                    return true;
                    break;
                case 1:
                    DoCutting();
                    if (skillTimer > cuttingTime-loadingTime) return false;
                    else return true;
                    break;
                case 2:
                    DoThrowing();
                    return false;
                    break;
                case 3:
                    DoLoading();
                    return true;
                    break;
                default:
                    return true;
                    break;
            }
            return state == 0;
        }
        public void checkState()
        {
            if (coolDown > 0) coolDown--;
            if (skillTimer > 0 ) skillTimer --;
            //完成前摇就使用技能，否则恢复常态
            if (skillTimer <=0)
            {
                if (state == 3)
                {
                    if (!Main.rand.NextBool(DownedBossSystem.downedConciousBoss ? 3 : 6))
                    {
                        state = 1;
                        skillTimer = cuttingTime;
                        oldDirection = NPC.direction;
                    }
                    else
                    {
                        state = 2;
                        skillTimer = throwingTime;
                        oldDirection = NPC.direction;
                    }
                }
                else state = 0;
            }
                
            if (coolDown <= 0 && NPC.HasValidTarget && Math.Abs(Main.player[NPC.target].Center.X - NPC.Center.X) <= 160f)
            {
                state = 3;
                coolDown = cuttingCD;
                skillTimer = loadingTime;
                oldDirection = NPC.direction;
            }
        }
        public void DoLoading()
        {
            //NPC.velocity = new Vector2(1f, 0f) * NPC.direction * 0.01f;
        }
        public void DoCutting()
        {
            NPC.direction = oldDirection;
            NPC.velocity = new Vector2(1f, 0f) * NPC.direction * 2f;
            if (Main.netMode == NetmodeID.MultiplayerClient || !NPC.HasValidTarget) return;
            if (skillTimer ==cuttingTime)
            {
                Projectile p = Projectile.NewProjectileDirect(NPC.GetSource_FromAI("cutting"), 
                    NPC.Center, 
                    new Vector2(NPC.direction * 48f, 0), 
                    ModContent.ProjectileType<AnthonyScissors>(), 
                    60, 
                    5);
                p.netUpdate = true;
            }
        }
        private Vector2 vel_throw = Vector2.Zero;
        public void DoThrowing()
        {
            NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * 0.01f;
            if (Main.netMode == NetmodeID.MultiplayerClient || !NPC.HasValidTarget) return;
            if (skillTimer ==throwingTime ) vel_throw = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 5f;
            if (skillTimer ==throwingTime / 2 )
            {
                Projectile p= Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), 
                    NPC.Center, 
                    vel_throw,
                    ModContent.ProjectileType<AnthonyScissors>(), 
                    60, 
                    5);
                p.netUpdate = true;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Knockback > 0)
            {
                state = -1;
                skillTimer = 6;
                NPC.spriteDirection = hit.HitDirection * -1;
            }
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Knockback > 0)
            {
                state = -1;
                skillTimer = 6;
                NPC.spriteDirection = hit.HitDirection * -1;
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(new CommonDrop(ModContent.ItemType<PieceofGrief>(), 2, 1));
        }
    }
}
