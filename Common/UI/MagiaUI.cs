using MahouShoujyo.Content;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MahouShoujyo;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using MahouShoujyo.Globals;

namespace MahouShoujyo.Common.UI
{
    public class MagiaUI : UIState
    {
        internal MagiaUIFrame frame;
        public override void OnInitialize()
        {
            /*UIPanel panel = new UIPanel();
            panel.Width.Set(80f, 0f);
            panel.Height.Set(140f, 0f);
            Reset();
            Append(panel);*/
            frame = new MagiaUIFrame();
            frame.Width.Set(80f, 0f);
            frame.Height.Set(140f, 0f);
            Append(frame);

        }
        public override void Update(GameTime gameTime)
        {
            frame.Update(gameTime);
        }
    }

    public class MagiaUIFrame : UIElement
    {
        private bool reset = false;
        private bool mouseHover = false;
        private bool dragging = false;
        private Vector2 offset = Vector2.Zero;
        private Texture2D tex;
        private float pX = 0;
        private float pY = 0;
        private float emo = 0;
        private static Player player => Main.LocalPlayer;
        private static MGPlayer mgplayer => Main.LocalPlayer.magic();

        public override void OnInitialize()
        {
            tex = ModContent.Request<Texture2D>("MahouShoujyo/Common/UI/Images/SoulGemFrame").Value;
            reset = true;
            
        }
        public void Reset()
        {
            Left.Set(Main.miniMapWidth-80f, 0f);
            Top.Set(Main.miniMapWidth, 0f);
        }
        public override void MouseOver(UIMouseEvent evt)
        {
            if (ContainsPoint(evt.MousePosition)) mouseHover = true;
        }
        public override void MouseOut(UIMouseEvent evt)
        {
            mouseHover = false;
        }
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (ContainsPoint(evt.MousePosition))
            {
                dragging = true;
                offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            }
        }
        public override void LeftMouseUp(UIMouseEvent evt)
        {
            dragging = true;
        }
        Asset<Texture2D> ContentTex = ModContent.Request<Texture2D>("MahouShoujyo/Common/UI/Images/SourBarTexture");
        Asset<Texture2D> MaskTex = ModContent.Request<Texture2D>("MahouShoujyo/Common/UI/Images/SoulGemMask");
        public override void Draw(SpriteBatch sb)
        {
            //Effect maskEffect = ModContent.Request<Effect>("MahouShoujyo/Effects/Mask").Value;
            tex = ModContent.Request<Texture2D>("MahouShoujyo/Common/UI/Images/SoulGemFrame").Value;
            int width = tex.Width;
            int height = tex.Height;
            Texture2D contentTex = ContentTex.Value;
            Texture2D maskTex = MaskTex.Value;
            int texWidth = contentTex.Width;
            int texHeight = contentTex.Height;
            int useWidth = texWidth / 3;
            int useHeight = texHeight / 2;
            Rectangle rect = new Rectangle(0, 0, width, height);
            float PollutionDegree = mgplayer.getPollutionPercent();
            Rectangle soulRect = new Rectangle((int)emo, (int)(PollutionDegree * useHeight), useWidth, useHeight);
            sb.End();
            GraphicsDevice gd = Main.graphics.GraphicsDevice;

            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, GameShaders.Misc["Mask"].Shader, Main.UIScaleMatrix);

            //gd.Textures[0] = contentTex;
            gd.Textures[1] = maskTex;
            GameShaders.Misc["Mask"].UseColor( Color.Red);
            GameShaders.Misc["Mask"].Shader.Parameters["uScreenResolution"].SetValue(new Vector2(texWidth, texHeight));
            //GameShaders.Misc["Mask"].UseShaderSpecificData(new Vector4( Left.Pixels, Top.Pixels, maskTex.Width, maskTex.Height));
            GameShaders.Misc["Mask"].UseShaderSpecificData(new Vector4(emo, PollutionDegree * useHeight, useWidth, useHeight));
            GameShaders.Misc["Mask"].Apply();
            sb.Draw(contentTex, new Rectangle((int)Left.Pixels, (int)Top.Pixels, width /2 , height /2), soulRect, Color.White);
            /*sb.Draw(
                contentTex, new Vector2(Left.Pixels, Top.Pixels),
                soulRect, Color.White, 0,
                new Vector2(0, 0),
                new Vector2(.5f, .5f),
                 SpriteEffects.None, 0);
            //Main.NewText(GameShaders.Misc["Mask"]);*/
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            sb.Draw(
                tex, new Vector2(Left.Pixels, Top.Pixels),
                rect, Color.Lerp(Color.White, new Color(30,24,36), mgplayer.getPollutionPercent()) , 0,
                new Vector2(0, 0),
                new Vector2(.5f, .5f),
                 SpriteEffects.None, 0);
            if (mouseHover)
            {
                Utils.DrawInvBG(sb,
                new Rectangle((int)Main.MouseScreen.X + 10, (int)Main.MouseScreen.Y + 10, 180+((player.magic().relief) ? 80 : 0), 30),
                new Color(23, 25, 81, 200));
                Utils.DrawBorderString(
                sb,
                Language.GetText("Mods.MahouShoujyo.Commons.GemUITips"+((player.magic().relief) ? 1 : 0)).Value+" "+player.magic().getPollution()+"/"+player.magic().getLimit(),
                Main.MouseScreen+ new Vector2(20f, 20f),
                Color.White,
                0.8f);
            }

        }
        public void SetPosition(float X, float Y)
        {
            Left.Set(X, 0f);
            Top.Set(Y, 0f);

        }
        public override void Update(GameTime gameTime)
        {
            emo += 1f;
            if (emo>= ContentTex.Width() * 2f / 3f) emo-= ContentTex.Width() * 2f / 3f;
            if (!player.input().mouseLeft ) dragging = false;
            //Main.NewText("dragging:"+dragging+" "+player.input().mouseLeft);
            if (!Main.dedServ && player.active && player.magic().magia)
            {
                if (mouseHover)
                {
                    player.mouseInterface = true;
                }
                if (reset)
                {
                    SetPosition(Main.miniMapX-80, Main.miniMapY);
                    reset = false;
                    dragging = false;
                    return;
                }
                if (dragging)
                {
                    SetPosition(Main.MouseScreen.X - offset.X, Main.MouseScreen.Y - offset.Y);
                    CalculatedStyle dimensions = GetDimensions();
                    SetPosition(MathHelper.Clamp(Left.Pixels, 0, Main.screenWidth - dimensions.Width),
                        MathHelper.Clamp(Top.Pixels, 0, Main.screenHeight - dimensions.Height));
                }
            }
            
            base.Update(gameTime);
        }
    }
}
