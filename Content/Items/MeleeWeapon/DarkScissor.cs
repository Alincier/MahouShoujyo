using MahouShoujyo.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace MahouShoujyo.Content.Items.MeleeWeapon;

public class DarkScissor : ModItem
{
    Player player => Main.LocalPlayer;
    private int reset = 0;
    // The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.MahouShoujyo.hjson' file.
    public override void SetDefaults()
    {
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        Item.damage = 40;
        Item.DamageType = DamageClass.Melee;
        Item.width = 48;
        Item.height = 48;
        Item.useTime = 15;
        Item.useAnimation =30;
        //关闭使用时的贴图绘制
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.holdStyle = ItemHoldStyleID.None;
        Item.knockBack = 10f;
        Item.value = Item.buyPrice(platinum: 0, gold: 2, silver: 0, copper: 0);
        Item.rare = ItemRarityID.LightRed;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;
        Item.useTurn = false;
        Item.useAmmo =AmmoID.None;
        Item.shootSpeed = 10f;
        Item.shoot = ModContent.ProjectileType<AnthonyScissorsFriendly>();
        Item.channel = true;
        Item.useTurn = false;
        Item.autoReuse = true;
        Item.scale = 1f;
        // 假如这是一个法杖类型，不写默认false，这里就用到物品Type了
        //Item.staff[Type] = false;
        // 一般来说，法杖类武器会使用Shoot的那个使用方式，但它的贴图不像枪一样是水平朝向而是向右上倾斜
        // 让它变成true就会导致使用时贴图再转45度，变成法杖尖端朝着射击方向
    }
    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
    public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
    {
        if (!Main.dedServ) Lighting.AddLight(hitbox.Center.ToVector2(), Color.White.ToVector3()* 1f);
        base.UseItemHitbox(player, ref hitbox, ref noHitbox);
    }
    public override bool CanUseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }
        else
        {
            Item.noUseGraphic = false;
            Item.noMelee = false;
        }
        return base.CanUseItem(player);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse != 2) return false;
        Projectile.NewProjectile(source, position, velocity, type, damage / 4, knockback / 2, player.whoAmI);
        return false;
    }
    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(5)) target.AddBuff(BuffID.Confused, 60);
        for (int i = 0; i<10; i++)
        {
            Vector2 vel_dust = new Vector2(player.direction, 0f)
                .RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-5, 5))) * 4f;
            Dust.NewDustDirect(target.position, target.width, target.height, DustID.Blood, vel_dust.X, vel_dust.Y, Scale: 1);
        }
        base.OnHitNPC(player, target, hit, damageDone);
    }



}

