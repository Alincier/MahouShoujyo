using Microsoft.Xna.Framework;
using Terraria;
using MahouShoujyo.Globals;

namespace MahouShoujyo
{
    public static partial class MagicUtils
    {
        #region ModPlayer
        public static PlayerState input(this Player player) => player.GetModPlayer<PlayerState>();
        public static MGPlayer magic(this Player player) => player.GetModPlayer<MGPlayer>();
        #endregion

        #region Movement and Controls
        public static bool ControlsEnabled(this Player player, bool allowWoFTongue = false)
        {
            if (player.CCed) // Covers frozen (player.frozen), webs (player.webbed), and Medusa (player.stoned)
                return false;
            if (player.tongued && !allowWoFTongue)
                return false;
            return true;
        }

        // See also: Player.IsStandingStillForSpecialEffects (Vanilla shiny stone + standing still mana regen)
        // That is more or less equivalent to this with the default value of 0.05
        public static bool StandingStill(this Player player, float velocity = 0.05f) => player.velocity.Length() < velocity;

        /// Checks if the player is ontop of solid ground. May also check for solid ground for X tiles in front of them
        /// <param name="player">The Player whose position is being checked</param>
        /// <param name="solidGroundAhead">How many tiles in front of the player to check</param>
        /// <param name="airExposureNeeded">How many tiles above every checked tile are checked for non-solid ground</param>
        public static bool CheckStandingGround(this Player player, int solidGroundAhead = 0, int solidGroundBehind = 0, int airExposureNeeded = 0, bool fullSolid = false)
        {
            if (player.velocity.Y != 0) // Player gotta be standing still in any case.
                return false;

            Tile checkedTile;
            bool ConditionMet = true;

            int playerCenterX = (int)player.Center.X / 16;
            int playerCenterY = (int)(player.position.Y + (float)player.height - 1f) / 16 + 1;
            for (int i = solidGroundBehind; i <= solidGroundAhead; i++) // Check i tiles in front of the player.
            {

                ConditionMet = Main.tile[playerCenterX + player.direction * i, playerCenterY].IsTileSolidGround();
                if ((fullSolid) ? !ConditionMet : ConditionMet)
                    return ConditionMet;

                for (int j = 1; j <= airExposureNeeded; j++) // Check j tiles ontop of each checked tiles for non-solid ground.
                {
                    checkedTile = Main.tile[playerCenterX + player.direction * i, playerCenterY - j];

                    ConditionMet = !(checkedTile != null && checkedTile.HasUnactuatedTile && Main.tileSolid[checkedTile.TileType]); // IsTileSolidGround minus the ground part, to avoid platforms and other half solid tiles messing it up.
                    if (!ConditionMet)
                        return ConditionMet;
                }
            }
            return ConditionMet;
        }
        public static bool CheckStanding(this Player player, bool fullsolid = false)
        {
            int width = player.width / 16;
            return player.CheckStandingGround(solidGroundAhead: (width/ 2), solidGroundBehind: 1-(width-width/ 2), fullSolid: fullsolid);
        }
        #endregion
    }
}
