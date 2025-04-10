using Microsoft.Xna.Framework;
using Terraria;
using MahouShoujyo.Globals;

namespace MahouShoujyo
{
    public static partial class TileUtils
    {

        #region TileState
        public static bool IsTileSolidGround(this Tile tile) => tile != null && tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]);
        #endregion

    }
}
