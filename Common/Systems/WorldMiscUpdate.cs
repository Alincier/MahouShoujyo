﻿using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Chat;
using Terraria.Localization;

namespace MahouShoujyo.Common.Systems
{
    public class WorldMiscUpdate : ModSystem
    {
        public override void PostUpdateWorld()
        {



            #region WitchNight
            if (Main.dayTime)
            {
                WitchNightSystem.TryEnd();
            }
            else
            {
                WitchNightSystem.TryBegin();
            }
            WitchNightSystem.Update();
            #endregion


        }

    }

}
