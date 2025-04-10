using Microsoft.Xna.Framework;
using Terraria;
using MahouShoujyo.Globals;
using System.Reflection;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace MahouShoujyo
{
    public static partial class DrawingUtils
    {
        internal static readonly FieldInfo UImageFieldMisc0 = typeof(MiscShaderData).GetField("_uImage0", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo UImageFieldMisc1 = typeof(MiscShaderData).GetField("_uImage1", BindingFlags.NonPublic | BindingFlags.Instance);
        internal static readonly FieldInfo UImageFieldArmor = typeof(ArmorShaderData).GetField("_uImage", BindingFlags.NonPublic | BindingFlags.Instance);
        public static MiscShaderData SetShaderTexture(this MiscShaderData shader, Asset<Texture2D> texture, int index = 1)
        {
            switch (index)
            {
                case 0:
                    UImageFieldMisc0.SetValue(shader, texture);
                    break;
                case 1:
                    UImageFieldMisc1.SetValue(shader, texture);
                    break;
            }
            return shader;
        }
    }
}
