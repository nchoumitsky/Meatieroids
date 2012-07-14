using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    // this screen forms the background for the menu screens
    class BackgroundScreen : GameScreen
    {
        ContentManager content;
        Texture2D backgroundTexture;

        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("images/tablecloth");
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        // override the update method of gamescreen because unlike other screens,
        // the background should never be faded away, its always there.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            byte fade = TransitionAlphaValue;

            // draw the background
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), new Color(fade, fade, fade));
            spriteBatch.End();
        }
    }
}
