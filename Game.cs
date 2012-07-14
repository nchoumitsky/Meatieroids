using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;

namespace MeatieroidsWindows
{
    public class MeatieroidsGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private ScreenManager screenManager;
        private GamerServicesComponent gamerServices;

        // The main game constructor.
        public MeatieroidsGame()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            // Register required components with the game - ScreenManager and Networking

            // Gamer services for live integration
            gamerServices = new GamerServicesComponent(this);
            Components.Add(gamerServices);

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // add the background and main menu screens to start the game
            screenManager.AddScreen(new BackgroundScreen());
            screenManager.AddScreen(new SplashScreen());
        }

        // This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.RosyBrown);
            base.Draw(gameTime);
        }
    }

    static class Program
    {
        static void Main()
        {
            using (MeatieroidsGame game = new MeatieroidsGame())
            {
                game.Run();
            }
        }
    }
}
