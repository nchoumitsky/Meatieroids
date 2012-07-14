using Microsoft.Xna.Framework;

namespace MeatieroidsWindows
{
    class PauseMenuScreen : MenuScreen
    {
        string promptMessage = "Are you sure you want to end the current game?";
        string acceptMessage = "Press Enter or A to quit";
        bool ready;
        MenuEntry resumeGameMenuEntry;
        MenuEntry quitGameMenuEntry;
        GameScreen screen;
        bool isNetworkGame;
        NetworkManager netManager;

        public PauseMenuScreen(GameScreen callingScreen) : base("No More Meatballs?")
        {
            screen = callingScreen;
            IsPopupWindow = true;
            ready = false;
            isNetworkGame = false;
            resumeGameMenuEntry = new MenuEntry("Keep Eating");
            quitGameMenuEntry = new MenuEntry("I'll take this to go");
            
            // hook up event handlers
            resumeGameMenuEntry.Selected += UnpauseGame;
            quitGameMenuEntry.Selected += QuitGameSelected;

            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }

        public PauseMenuScreen(GameScreen callingScreen, NetworkManager nManager)
            : base("No More Meatballs?")
        {
            screen = callingScreen;
            IsPopupWindow = true;
            ready = false;
            isNetworkGame = true;
            resumeGameMenuEntry = new MenuEntry("Keep Eating");
            quitGameMenuEntry = new MenuEntry("I'll take this to go");
            netManager = nManager;

            // hook up event handlers
            resumeGameMenuEntry.Selected += UnpauseGame;
            quitGameMenuEntry.Selected += QuitGameSelected;

            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }


        // Event handler for when the Quit Game menu entry is selected.
        void QuitGameSelected(object sender, PlayerIndexEventArgs e)
        {
            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(promptMessage, acceptMessage);
            confirmQuitMessageBox.Accepted += ConfirmQuitAccepted;
            ScreenManager.AddScreen(confirmQuitMessageBox);
        }

        void UnpauseGame(object sender, PlayerIndexEventArgs e)
        {
            if (isNetworkGame)
            {
                ready = true;
               netManager.SendUnPauseMessage();
                if (!netManager.RemoteReady)
                {
                    resumeGameMenuEntry.DisplayText = "Ready to eat, awaiting opposing pasta...";
                    return;
                }
            }
            ExitScreen();
        }

        void ConfirmQuitAccepted(object sender, PlayerIndexEventArgs e)
        {
            // notify the other player that the game is ending
            if (isNetworkGame)
            {
                netManager.SendEndGameMessage();
                netManager.CleanUpNetwork();
                isNetworkGame = false;
            }

            // reload the main menu since all other screens have quit
            ScreenManager.AddScreen(new BackgroundScreen());
            ScreenManager.AddScreen(new MainMenuScreen());
            ExitScreen();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (isNetworkGame)
            {
                UpdateNetworkGameplay();

                if (netManager.RemoteReady && ready)
                    ExitScreen();
            }
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        private void UpdateNetworkGameplay()
        {
            switch (netManager.GetMessageType())
            {
                case MessageType.UnPauseGame:
                    netManager.RemoteReady = true;
                    resumeGameMenuEntry.DisplayText = "Keep Eating... Remote Pasta is boiling and ready to go";
                    break;
                case MessageType.EndGame:
                    ScreenManager.AddScreen(new BackgroundScreen());
                    ScreenManager.AddScreen(new MainMenuScreen());
                    netManager.CleanUpNetwork();
                    ExitScreen();
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlphaValue * 2 / 3);
            base.Draw(gameTime);
        }
    }
}
