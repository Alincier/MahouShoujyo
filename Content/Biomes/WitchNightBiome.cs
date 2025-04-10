using MahouShoujyo.Common.Configs;
using MahouShoujyo.Common.Systems;
using MahouShoujyo.Content.Assets.Backgrounds;
using MahouShoujyo.Content.NPCs.BOSSes.Majo_Consciousness;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MahouShoujyo.Content.Biomes
{
    public class WitchNightBiome : ModBiome
    {
        //    public override ModWaterStyle WaterStyle => ModContent.GetInstance<ExampleWaterStyle>(); // Sets a water style for when inside this biome
        //public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<ConsciousBackgroundStyle>();
        //    public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;
        public override int Music => Getmusic();//MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery");
        private int Getmusic()
        {
            int majoindex = NPC.FindFirstNPC(ModContent.NPCType<Majo_Consciousness>());
            if (majoindex >= 0 && Main.npc[majoindex].active)
            {
                return ((Majo_Consciousness)Main.npc[majoindex].ModNPC).Music;
            }
            return MusicLoader.GetMusicSlot(Mod, "Radio/Music/Majo_consciousness_p1"); ;
        }
        public const float changeTime = 180;
        public static float shaderDegree = 0;
        //    public override int BiomeTorchItemType => ModContent.ItemType<ExampleTorch>();
        //    public override int BiomeCampfireItemType => ModContent.ItemType<ExampleCampfire>();

        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => BackgroundPath; // Re-uses Bestiary Background for Map Background

        public override bool IsBiomeActive(Player player)
        {
            return WitchNightSystem.EventActive;
        }
        public override void SpecialVisuals(Player player, bool isActive)
        {
            DrawShader();
        }
        public static void DrawShader()
        {
            if (Main.netMode == NetmodeID.Server) return;
            if (WitchNightSystem.EventActive) shaderDegree++;
            else shaderDegree--;
            shaderDegree = Math.Clamp(shaderDegree, 0, changeTime);
            if (shaderDegree > 0 && Main.netMode != NetmodeID.Server && (Main.LocalPlayer.ZoneDirtLayerHeight || Main.LocalPlayer.ZoneOverworldHeight || Main.LocalPlayer.ZoneSkyHeight))
            {
                Color color = new Color(120, 60, 180);
                MahouShoujyo.SceneShader("ColorScaleDynamic", shaderDegree / changeTime, factor: 2, r0: color.R, r1: color.G, r2: color.B);

            }
            else
            {
                MahouShoujyo.DelSceneShader("ColorScaleDynamic");
            }
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
    }
}
