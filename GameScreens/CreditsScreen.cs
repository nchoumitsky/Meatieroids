using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    class CreditsScreen : MenuScreen
    {
        private MenuEntry createdBy;
        private MenuEntry nick;
        private MenuEntry lauren;
        private MenuEntry dan;
        private MenuEntry specialThanks;
        private MenuEntry tom;
        private MenuEntry exit;

        public CreditsScreen()
            : base("Credits")
        {
            // Create our menu entries.
            createdBy = new MenuEntry("This meaty game was created by:");
            nick = new MenuEntry("Nick Choumitsky - did all the work");
            lauren = new MenuEntry("Lauren Domingo - drew everything");
            dan = new MenuEntry("Dan Aronds - Our lead.rar");
            specialThanks = new MenuEntry("Special thanks to:");
            tom = new MenuEntry("Tom Gehr - the inspiration for Meatieroids");
            exit = new MenuEntry("Go Back");

            exit.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(createdBy);
            MenuEntries.Add(nick);
            MenuEntries.Add(lauren);
            MenuEntries.Add(dan);
            MenuEntries.Add(specialThanks);
            MenuEntries.Add(tom);
            MenuEntries.Add(exit);
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
    }
}
