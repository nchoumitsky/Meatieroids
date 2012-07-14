using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;

namespace MeatieroidsWindows
{
    // The screen manager manages instances of GameScreen. It maintains a list of screens
    // which will have their update and draw methods called if they are active, and sends
    // user input to whatever the active screens are
    public class ScreenManager : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        SpriteFont gameFont;
        Texture2D fadeTexture;
        //IAsyncResult loadResult;

        // lists of screens and screens waiting to be updated by the draw and update methods
        List<GameScreen> screens = new List<GameScreen>();
        List<GameScreen> screensToUpdate = new List<GameScreen>();

        // user input handler for the game
        InputManager userInput = new InputManager();

        // initialization flags for the screen manager
        bool isInitialized;
        bool isSoundEnabled;

        // gamplay variables
        int diffucultyLevel = 0;
        int highScore = 0;

        // public properties and game settings
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public SpriteFont GameFont
        {
            get { return gameFont; }
        }

        public int DiffucultyLevel
        {
            get { return diffucultyLevel; }
            set { diffucultyLevel = value; }
        }

        public int CurrentHighScore
        {
            get { return highScore; }
            set { highScore = value; }
        }

        public bool SoundEnabled
        {
            get { return isSoundEnabled; }
            set { isSoundEnabled = value; }
        }

        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }

        //Constructs a new screen manager component.
        public ScreenManager(Game game) : base(game) { }

        // Initializes the screen manager component.
        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
            isSoundEnabled = true;
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            //netSession = new NetworkManager();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameFont = content.Load<SpriteFont>("menufont");
            fadeTexture = content.Load<Texture2D>("images/blankBG");

            Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, StorageLoadCompletedCallback, null);

            // Tell each of the screens to load their content.
            foreach (GameScreen Screen in screens)
                Screen.LoadContent();
        }

        void StorageLoadCompletedCallback(IAsyncResult result)
        {
            StorageDevice device = Guide.EndShowStorageDeviceSelector(result);
            int score = new int();
            if (device.IsConnected)
            {
                ScreenManager.DoLoadGame(device, ref score);
                CurrentHighScore = score;
            }
        }

        protected override void UnloadContent()
        {
            Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, StorageSaveCompletedCallback, null);
            foreach (GameScreen screen in screens)
                screen.UnloadContent();
        }

        void StorageSaveCompletedCallback(IAsyncResult result)
        {
            StorageDevice device = Guide.EndShowStorageDeviceSelector(result);
            if (device.IsConnected)
            {
                ScreenManager.DoSaveGame(device, CurrentHighScore);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad input and check for messages on the network
            userInput.Update();

            screensToUpdate.Clear();
            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen CurrentScreen = screensToUpdate[screensToUpdate.Count - 1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Update the screen.
                CurrentScreen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                //if the screen is about to be shown or is active
                if (CurrentScreen.ScreenState == ScreenState.TransitionOn || CurrentScreen.ScreenState == ScreenState.Active)
                {
                    // if this is the active screen
                    if (!otherScreenHasFocus)
                    {
                        CurrentScreen.HandleInput(userInput);
                        otherScreenHasFocus = true;
                    }

                    if (!CurrentScreen.IsPopupWindow)
                        coveredByOtherScreen = true;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen CurrentScreen in screens)
            {
                if (CurrentScreen.ScreenState == ScreenState.Hidden)
                    continue;

                CurrentScreen.Draw(gameTime);
            }
        }

        public void AddScreen(GameScreen screen)
        {
            // set the player that is controlling this screen 
            screen.ScreenManager = this;
            screen.IsExiting = false;

            if (isInitialized)
                screen.LoadContent();

            screens.Add(screen);
        }

        // pops a screen off the screens list. 
        public void RemoveScreen(GameScreen screen)
        {
            // remove the screen from the list and delete everything it loaded
            if (isInitialized)
                screen.UnloadContent();

            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }

        // use to make grayish background for popups
        public void FadeBackBufferToBlack(int alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            spriteBatch.Begin();
            spriteBatch.Draw(fadeTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), new Color(0, 0, 0, (byte)alpha));
            spriteBatch.End();
        }

        [Serializable]
        public struct SaveGameData
        {
            public int Score;
        }

        public static void DoSaveGame(StorageDevice device, int scoreToSave)
        {
            // Create the data to save.
            SaveGameData data = new SaveGameData();
            data.Score = scoreToSave;

            // Open a storage container.
            StorageContainer container =
                device.OpenContainer("Meatieroids");

            // Get the path of the save game.
            string filename = Path.Combine(container.Path, "hiscore.sav");

            // Open the file, creating it if necessary.
            FileStream stream = File.Open(filename, FileMode.OpenOrCreate);
            if (device.FreeSpace > 0)
            {
                // Convert the object to XML data and put it in the stream.
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                serializer.Serialize(stream, data);
            }
            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();
        }

        private static void DoLoadGame(StorageDevice device, ref int score)
        {
            // Open a storage container.
            StorageContainer container =
                device.OpenContainer("Meatieroids");

            // Get the path of the save game.
            string filename = Path.Combine(container.Path, "hiscore.sav");

            // Check to see whether the save exists.
            if (!File.Exists(filename))
                // Notify the user there is no save.
                return;

            // Open the file.
            FileStream stream = File.Open(filename, FileMode.OpenOrCreate,
                FileAccess.Read);

             //Read the data from the file.
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
            SaveGameData data = (SaveGameData)serializer.Deserialize(stream);
            score = data.Score;

            stream.Close();

            // Dispose the container.
            container.Dispose();
        }
    }
}
