using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace MahouShoujyo.AIs
{

    //是否主动攻击
    public enum ActiveType
    {
        wander,
        negative,
        active,
    }
    // 参数 detectRange ， 为 -1 无限距离， 为 0  不追踪
    #region ArmyEnemies
    public static class NormalAIs
    {
        public static void ArmyAI(NPC npc, Mod mod, ActiveType active, float detectRange = -1, float speedLimitX = 1.6f, float accSpeed = 0.05f , float speedLimitY = 3f, float maxJump = 90f, float gravity = 0.05f, float accRotation = 0f, float maxRotation = 0f, bool spriteFacesLeft = true)
        {
            npc.TargetClosest(true);
            if (npc.HasValidTarget)
            {
                Player player = Main.player[npc.target];
                if (detectRange < 0f || Vector2.Distance(player.Center, npc.Center)<= detectRange)
                {
                    if (player.Center.X < npc.Center.X) npc.velocity.X = MathHelper.Clamp(npc.velocity.X- accSpeed,-speedLimitX , speedLimitX);
                    else npc.velocity.X = MathHelper.Clamp(npc.velocity.X+ accSpeed, -speedLimitX, speedLimitX);
                }
                
                if (npc.collideX || npc.CheckStanding()) npc.velocity.Y -= (float)Math.Sqrt((maxJump * 2f * gravity)) ;
                if (npc.collideY) 
                if (npc.Hitbox.Bottom + 8 < player.Hitbox.Bottom) npc.stairFall = true;
                else npc.stairFall = false;    
            }
        }
    }
    #endregion
}
