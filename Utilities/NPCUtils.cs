using Microsoft.Xna.Framework;
using Terraria;
using MahouShoujyo.Globals;

namespace MahouShoujyo
{
    public static partial class NPCUtils
    {
        #region find NPCState
        //whether the NPC is a familiar.Default to false.
        public static bool Familiar(this NPC npc, bool value = false) => npc.GetGlobalNPC<NPCState>().familiar = value;
        //whether the NPC is a majyo.Default to false.
        public static bool Majyo(this NPC npc, bool value = false) => npc.GetGlobalNPC<NPCState>().majyo = value;
        #endregion
        #region Movement

        // See also: Player.IsStandingStillForSpecialEffects (Vanilla shiny stone + standing still mana regen)
        // That is more or less equivalent to this with the default value of 0.05
        public static bool StandingStill(this NPC npc, float velocity = 0.05f) => npc.velocity.Length() < velocity;

        /// Checks if the player is ontop of solid ground. May also check for solid ground for X tiles in front of them
        /// <param name="player">The Player whose position is being checked</param>
        /// <param name="solidGroundAhead">How many tiles in front of the player to check</param>
        /// <param name="solidGroundBehind">How many tiles behind the player to check</param>
        /// <param name="airExposureNeeded">How many tiles above every checked tile are checked for non-solid ground</param>
        /// <param name="fullSolid"> is it necessary that the ground all solid</param>
        public static bool CheckStandingGround(this NPC npc, int solidGroundAhead = 0, int solidGroundBehind = 0, int airExposureNeeded = 0, bool fullSolid = false)
        {
            if (npc.velocity.Y != 0) // Player gotta be standing still in any case.
                return false;

            Tile checkedTile;
            bool ConditionMet = true;

            int npcCenterX = (int)npc.Center.X / 16;
            int npcCenterY = (int)(npc.position.Y + (float)npc.height - 1f) / 16 + 1;
            for (int i = solidGroundBehind; i <= solidGroundAhead; i++) // Check i tiles in front of the player.
            {

                ConditionMet = Main.tile[npcCenterX + npc.direction * i, npcCenterY].IsTileSolidGround();
                if ((fullSolid)?!ConditionMet:ConditionMet)
                    return ConditionMet;

                for (int j = 1; j <= airExposureNeeded; j++) // Check j tiles ontop of each checked tiles for non-solid ground.
                {
                    checkedTile = Main.tile[npcCenterX + npc.direction * i, npcCenterY - j];

                    ConditionMet = !(checkedTile != null && checkedTile.HasUnactuatedTile && Main.tileSolid[checkedTile.TileType]); // IsTileSolidGround minus the ground part, to avoid platforms and other half solid tiles messing it up.
                    if (!ConditionMet)
                        return ConditionMet;
                }
            }
            return ConditionMet;
        }
        public static bool CheckStanding(this NPC npc, bool fullsolid = false)
        {
            int width = npc.width / 16;
            return npc.CheckStandingGround(solidGroundAhead: (width/ 2), solidGroundBehind: 1-(width-width/ 2),fullSolid: fullsolid );
        }

        #endregion
        #region AdjustExpert&Master
        public static void AdjustExpert(this NPC npc, float m) => NPCState.AdjustExpert(npc, m);
        public static void AdjustMaster(this NPC npc, float m) => NPCState.AdjustMaster(npc, m);
        #endregion
    }
}
