using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    // This screen provides a loading screen between two screens
    class LoadingScreen : GameScreen
    {
        private GameScreen[] screensToLoad;
        private bool otherScreensAreGone;
        private string LoadingMessage;
        private bool isNetworkGame;
        private NetworkManager netManager;

        private LoadingScreen(ScreenManager screenManager, GameScreen[] screens)
        {
            isNetworkGame = false;
            screensToLoad = screens;
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
        }

        private LoadingScreen(ScreenManager screenManager, NetworkManager nManager, GameScreen[] screens)
        {
            isNetworkGame = true;
            netManager = nManager;
            screensToLoad = screens;
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
        }

        public static void Load(ScreenManager screenManager, params GameScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // create the loading screen and add it to the manager
            LoadingScreen loadingScreen = new LoadingScreen(screenManager, screensToLoad);
            screenManager.AddScreen(loadingScreen);
        }

        public static void Load(ScreenManager screenManager, NetworkManager nManager, params GameScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // create the loading screen and add it to the manager
            LoadingScreen loadingScreen = new LoadingScreen(screenManager, nManager,  screensToLoad);
            screenManager.AddScreen(loadingScreen);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (isNetworkGame)
            {
                if (netManager.CurrentState == NetworkState.LoggedIn)
                    netManager.StartNetworkGame();

                else if (netManager.CurrentState == NetworkState.PlayerJoined)
                {
                    // once the menu has transitioned away, draw the new screens being loaded
                    if (otherScreensAreGone)
                    {
                        ScreenManager.RemoveScreen(this);
                        ScreenManager.Game.ResetElapsedTime();

                        foreach (GameScreen screen in screensToLoad)
                        {
                            if (screen != null)
                                ScreenManager.AddScreen(screen);
                        }
                    }
                }

            }
            else
            {
                if (otherScreensAreGone)
                {
                    ScreenManager.RemoveScreen(this);
                    ScreenManager.Game.ResetElapsedTime();

                    foreach (GameScreen screen in screensToLoad)
                    {
                        if (screen != null)
                            ScreenManager.AddScreen(screen);
                    }
                }
            }
        }

        public override void HandleInput(InputManager input)
        {
            if (input.IsPauseGame())
            {
                otherScreensAreGone = false;

                if (netManager.CurrentState == NetworkState.CreatedSession)
                    netManager.CleanUpNetwork();

                ScreenManager.AddScreen(new BackgroundScreen());
                ScreenManager.AddScreen(new MainMenuScreen());
            }

            base.HandleInput(input);
        }

        public override void Draw(GameTime gameTime)
        {
            if ((ScreenState == ScreenState.Active) && (ScreenManager.GetScreens().Length == 1))
                otherScreensAreGone = true;

            if (isNetworkGame)
            {
                if (netManager.CurrentState == NetworkState.CreatedSession)
                    LoadingMessage = "Waiting for opponent...";
                else
                    LoadingMessage = "Signing in...";
            }
            else
                LoadingMessage = "Making Meatballs...";

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.GameFont;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(LoadingMessage);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            // make the screen black so when the game loads it fades in nicely
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            Color strColor = Color.DarkRed;

            spriteBatch.Begin();
            spriteBatch.DrawString(font, LoadingMessage, textPosition, strColor);
            spriteBatch.End();
        }
    }
}
