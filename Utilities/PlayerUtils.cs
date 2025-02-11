using Microsoft.Xna.Framework;
using Terraria;
using MahouShoujyo.Globals;

namespace MahouShoujyo
{
    public static partial class MagicUtils
    {
        public static PlayerState input(this Player player) => player.GetModPlayer<PlayerState>();
        public static MGPlayer magic(this Player player) => player.GetModPlayer<MGPlayer>();
    }
}
