using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    // this screen has two options that can be set by the user
    // diffuculty .. how much meat is spawned, and if sound is enabled

    class OptionsScreen : MenuScreen
    {
        private enum Difficulties
        {
            Easy,
            Normal,
            Hard,
        }

        private bool sound;
        private Difficulties currentDifficulty;
        private MenuEntry opt1, opt2, opt3;

        public OptionsScreen()
            : base("Game Options")
        {
            // Create our menu entries.
            opt1 = new MenuEntry(string.Empty);
            opt2 = new MenuEntry(string.Empty);
            opt3 = new MenuEntry("Go Back");
            
            opt1.Selected += SoundMenuEntrySelected;
            opt2.Selected += DifficultyMenuEntrySelected;
            opt3.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(opt1);
            MenuEntries.Add(opt2);
            MenuEntries.Add(opt3);

            SetMenuEntryText();
        }
        public override void LoadContent()
        {
            base.LoadContent();
            sound = ScreenManager.SoundEnabled;
            currentDifficulty = (Difficulties)ScreenManager.DiffucultyLevel;
            SetMenuEntryText();
        }
        protected override void OnCancel()
        {
            ScreenManager.AddScreen(new MainMenuScreen());
            ExitScreen();
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlphaValue * 2 / 5);
            base.Draw(gameTime);
        }

        void SetMenuEntryText()
        {
            opt1.DisplayText = "Sound : " + (sound ? "On" : "Off");
            opt2.DisplayText = "Diffuculty : " + currentDifficulty;
        }

        void SoundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            sound = !sound;
            ScreenManager.SoundEnabled = sound;
            SetMenuEntryText();
        }

        void DifficultyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentDifficulty++;

            if (currentDifficulty > Difficulties.Hard)
                currentDifficulty = 0;

            ScreenManager.DiffucultyLevel = (int)currentDifficulty;
            SetMenuEntryText();
        }

    }
}
