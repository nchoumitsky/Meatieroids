using System;
using System.Threading;//figure this out and remove
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

    class MainMenuScreen : MenuScreen
    {
        private string exitPrompt = "Are you sure you want to quit Meatieroids?";
        private string acceptPrompt = "Press Enter or A to exit the game";

        public MainMenuScreen() : base("Main Menu")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play Local Game");
            MenuEntry playNetworkGame = new MenuEntry("Play Network Game");
            MenuEntry optionMenuEntry = new MenuEntry("Options");
            MenuEntry creditsEntry = new MenuEntry("Credits");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameSelected;
            playNetworkGame.Selected += OnNetworkGameSelected;
            optionMenuEntry.Selected += OnOptionsSelected;
            creditsEntry.Selected += OnCreditsSelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(playNetworkGame);
            MenuEntries.Add(optionMenuEntry);
            MenuEntries.Add(creditsEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        // Event handler for when the Play Game menu entry is selected.
        void PlayGameSelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, new BackgroundScreenGame(),new GameplayScreen());
        }

        void OnOptionsSelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsScreen());
            ExitScreen();
        }

        void OnCreditsSelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new CreditsScreen());
            ExitScreen();
        }

        void OnNetworkGameSelected(object sender, PlayerIndexEventArgs e)
        {
            /*
            ScreenManager.netManager = new NetworkManager();
            ScreenManager.isNetworkGame = true;
            //if (ScreenManager.netManager.currentState == NetworkState.SignIn)
                ScreenManager.netManager.LogInUser();

            LoadingScreen.Load(ScreenManager, new BackgroundScreenGame(), new GameplayScreen());
             * */
            
            ScreenManager.AddScreen(new NetworkGameMenu(e.PlayerIndex));
            ExitScreen();
        }

        void ConfirmExitSelected(object sender, PlayerIndexEventArgs e)
        {

            ScreenManager.Game.Exit();
        }

        protected override void OnCancel()
        {
            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(exitPrompt, acceptPrompt);
            confirmExitMessageBox.Accepted += ConfirmExitSelected;
            ScreenManager.AddScreen(confirmExitMessageBox);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlphaValue * 2 / 5);
            base.Draw(gameTime);
        }
    }
}
