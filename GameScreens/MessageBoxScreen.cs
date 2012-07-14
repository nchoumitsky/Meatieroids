using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    class MessageBoxScreen : GameScreen
    {
        protected string message;
        protected string enterText;

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        public MessageBoxScreen(string message, string acceptMessage)
        {
            this.message = message;
            this.enterText = acceptMessage;

            IsPopupWindow = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
        }

        public override void HandleInput(InputManager input)
        {
            if (input.IsMenuSelect())
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs());

                ExitScreen();
            }
            else if (input.IsMenuCancel())
            {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, new PlayerIndexEventArgs());

                ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.GameFont;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlphaValue * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 enterSize = font.MeasureString(enterText);
            Vector2 textPosition = (viewportSize - textSize) / 2;
            Vector2 enterPosition = (viewportSize - enterSize) / 2;
            Color color = new Color(255, 255, 255, TransitionAlphaValue);

            enterPosition.Y += 50;

            spriteBatch.Begin();
            spriteBatch.DrawString(font, message, textPosition, color);
            spriteBatch.DrawString(font, enterText, enterPosition, color);
            spriteBatch.End();
        }
    }
}
