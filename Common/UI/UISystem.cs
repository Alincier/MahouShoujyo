using MahouShoujyo.Globals;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace MahouShoujyo.Common.UI
{
    public class UISystem : ModSystem
    {

        private UserInterface magiaUserInterface;
        internal MagiaUI magiaUI;
        public override void Load()
        {
            // Create custom interface which can swap between different UIStates
            magiaUserInterface = new UserInterface();
            // Creating custom UIState
            magiaUI = new MagiaUI();
            // Activate calls Initialize() on the UIState if not initialized, then calls OnActivate and then calls Activate on every child element
            //magiaUI.Activate();
            magiaUserInterface?.SetState(magiaUI);
        }
        public override void UpdateUI(GameTime gameTime)
        {
            // Here we call .Update on our custom UI and propagate it to its state and underlying elements
            if (magiaUserInterface?.CurrentState != null)
            {
                magiaUserInterface?.Update(gameTime);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "MahouShoujyo: MagiaUI",
                    delegate
                    {
                        if (magiaUserInterface?.CurrentState != null && !Main.dedServ && Main.LocalPlayer.active && Main.LocalPlayer.magic().magia)
                        {
                            magiaUserInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
            try
            {
                //Main.NewText(magiaUserInterface?.CurrentState);
            }
            catch
            {

            }
            
        }
    }
}
