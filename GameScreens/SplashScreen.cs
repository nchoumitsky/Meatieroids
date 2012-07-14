using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace MeatieroidsWindows
{
    // This screen implements the actual game logic.
    class SplashScreen : GameScreen
    {
        private ContentManager content;
        private SpriteFont gameFont;

        private String startMessage = "Press enter or A button to begin...";
        private String groupMessage = "A Pasta Adventure!";
        private Texture2D logo;

        private float textScale = 0.0f;

        public SplashScreen()
        {
            // we want the screen to take a bit longer to transition on and off
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = ScreenManager.GameFont;
            logo = content.Load<Texture2D>("images/meatlogo");
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void HandleInput(InputManager input)
        {
            if (input.IsMenuSelect())
            {
                ScreenManager.AddScreen(new BackgroundScreen());
                ScreenManager.AddScreen(new MainMenuScreen());
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            textScale += .1f;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            Vector2 viewSize = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
            Vector2 textPosition = (viewSize - gameFont.MeasureString(startMessage)) / 2 + new Vector2(0, 260);
            Vector2 groupPosition = (viewSize - gameFont.MeasureString(groupMessage)) / 2 + new Vector2(0, textScale);
            Rectangle logoPosition = new Rectangle(80, 80, (int)viewSize.X - 100, (int)viewSize.Y / 5);
            
            spriteBatch.Begin();
            spriteBatch.DrawString(gameFont, startMessage, textPosition, Color.White);
            spriteBatch.DrawString(gameFont, groupMessage, groupPosition, Color.Gold);
            spriteBatch.Draw(logo, logoPosition, Color.White);
            spriteBatch.End();

            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlphaValue);
        }
    }
}
