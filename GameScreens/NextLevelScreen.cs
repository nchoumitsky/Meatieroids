using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    class NextLevelScreen : GameScreen
    {
        protected string message;
        protected string enterText;
        private SpriteFont screenFont;
        private bool ready;
        private bool isNetworkGame;
        private NetworkManager netManager;

        public NextLevelScreen()
        {
            isNetworkGame = false;
            IsPopupWindow = true;
            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        public NextLevelScreen(NetworkManager nManager)
        {
            isNetworkGame = true;
            IsPopupWindow = true;
            netManager = nManager;
            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        public override void LoadContent()
        {
            message = "You made it!";
            enterText = "Ready for some more meat??";
            if(isNetworkGame)
                netManager.RemoteReady = false;
            ready = false;
            ContentManager content = ScreenManager.Game.Content;
            screenFont = ScreenManager.GameFont;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void HandleInput(InputManager input)
        {
            if (input.IsMenuSelect())
            {
                if (isNetworkGame)
                {
                    if (!ready)
                    {
                        ready = true;
                        enterText = "Ready to go... waiting for other pasta";
                        netManager.SendReadyMessage();
                    }
                }
                else
                    ExitScreen();
            }
            base.HandleInput(input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (isNetworkGame)
            {
                UpdateNetworkGameplay();

                if (ready && netManager.RemoteReady && netManager.netSession.IsHost)
                {
                    netManager.SendStartLevelMessage();
                    ExitScreen();
                }
            }
            else
            {
                if (ready)
                {
                    ExitScreen();
                }
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void UpdateNetworkGameplay()
        {
            switch (netManager.GetMessageType())
            {
                case MessageType.StartLevel:
                    ExitScreen();
                    break;
                case MessageType.Ready:
                    netManager.RemoteReady = true;
                    if (!ready)
                        enterText = "Remote Pasta ready for meat... are you?";
                    break;
                case MessageType.EndGame:
                    ScreenManager.AddScreen(new BackgroundScreen());
                    ScreenManager.AddScreen(new MainMenuScreen());
                    netManager.CleanUpNetwork();
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.GameFont;

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 enterSize = font.MeasureString(enterText);
            Vector2 textPosition = (viewportSize - textSize) / 2;
            Vector2 enterPosition = (viewportSize - enterSize) / 2;
            Color color = new Color(255, 255, 255, TransitionAlphaValue);

            enterPosition.Y += 50;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlphaValue * 2 / 3);

            spriteBatch.Begin();
            spriteBatch.DrawString(screenFont, message, textPosition, color);
            spriteBatch.DrawString(screenFont, enterText, enterPosition, color);
            spriteBatch.End();
        }
    }
}
