using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    // this screen forms the background for the menu screens
    class BackgroundScreenGame : GameScreen
    {
        private ContentManager content;
        private Texture2D backgroundTexture;

        // screen variables
        private Color overlayColor = Color.Firebrick;

        public BackgroundScreenGame()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("images/tomatoes");
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Color drawColor = overlayColor * TransitionAlphaValue;

            // draw the background
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), drawColor);
            spriteBatch.End();
        }
    }
}
