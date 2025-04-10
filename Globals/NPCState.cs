using MahouShoujyo.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Security.Policy;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace MahouShoujyo.Globals
{
    public class NPCState : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        //是否为使魔的字段
        public bool familiar = false;
        //是否为魔女的字段
        public bool majyo = false;
        private NPC me = null;
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            me = npc;
        }
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (WitchNightSystem.EventActive)
            {
                spawnRate = (int)(spawnRate / 1.5f);
                maxSpawns *= 2;
            }
        }
        #region Modify Familiar/Majyo
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (familiar) modifiers.ModifyHitInfo +=Modifiers_ModifyFamiliarHitInfo;
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (familiar) modifiers.ModifyHitInfo +=Modifiers_ModifyFamiliarHitInfo;
        }
        private void Modifiers_ModifyFamiliarHitInfo(ref NPC.HitInfo info)
        {
            try
            {
                if (me == null) return;
                if (info.Damage <=me.lifeMax / 40f)
                info.Knockback = 0;
            }
            catch { }
        }
        #endregion
        #region Adjust Master/Expert 
        public static void AdjustMaster(NPC npc, float multify)
        {
            if (Main.masterMode)
            {
                npc.lifeMax = (int)Math.Round(npc.lifeMax * (multify / 3f));
                npc.damage = (int)Math.Round(npc.damage * (multify / 3f));
            }
               
        }
        public static void AdjustExpert(NPC npc, float multify)
        {
            if (Main.expertMode && !Main.masterMode)
            {
                npc.lifeMax = (int)Math.Round(npc.lifeMax * (multify / 2f));
                npc.damage = (int)Math.Round(npc.damage * (multify / 2f));
            }

            
        }
        #endregion
    }
}
