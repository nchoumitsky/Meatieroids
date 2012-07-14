using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework;

namespace MeatieroidsWindows
{
    class SearchLocalNetworkScreen : MenuScreen
    {
        private AvailableNetworkSessionCollection availableSessions;
        private MenuEntry returnEntry;
        private MenuEntry Games;
        private MenuEntry gameMode;
        private MenuEntry highScore;
        private MenuEntry joinGame;
        private int gameCount;
        private int totalGames;

        private AvailableNetworkSession currentSessionSelected;
        private PlayerIndex currentPlayer;

        public SearchLocalNetworkScreen(PlayerIndex playerIndex, AvailableNetworkSessionCollection availableSessions)
            : base("Local Network Game Search")
        {
            currentPlayer = playerIndex;
            IsPopupWindow = true;
            this.availableSessions = availableSessions;
            if (availableSessions.Count == 0)
            {
                Games = new MenuEntry("No Games found");
                returnEntry = new MenuEntry("Return");
                MenuEntries.Add(Games);
            }
            else
            {
                gameCount = 0;
                totalGames = availableSessions.Count;
                currentSessionSelected = availableSessions[0];
                Games = new MenuEntry("Game: 0 / " + gameCount);
                gameMode = new MenuEntry("Game mode: " + GameType());
                highScore = new MenuEntry("Hi score: " + WinningScore());
                joinGame = new MenuEntry("Join this game");

                joinGame.Selected += JoinSession;
                MenuEntries.Add(Games);
                MenuEntries.Add(gameMode);
                MenuEntries.Add(highScore);
                MenuEntries.Add(joinGame);

            }
            
            // hook up event handlers
            returnEntry.Selected += GoBack;

            MenuEntries.Add(returnEntry);
        }

        void GoBack(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }

        void JoinSession(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new LocalNetworkGameMenu(currentPlayer, NetworkSession.Join(currentSessionSelected)));
            ScreenManager.RemoveScreen(this);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlphaValue * 2 / 3);
            base.Draw(gameTime);
        }

        protected String GameType()
        {
            String result = "ERROR";
            switch ((int)availableSessions[gameCount].SessionProperties[(int)SessionProperty.GameMode])
            {
                case (int)GameMode.HeadToHead:
                    result = "Head to Head";
                    break;
            }

            return result;
        }

        protected String WinningScore()
        {
            String result = "ERROR";
            switch ((int)availableSessions[gameCount].SessionProperties[(int)SessionProperty.WinningScore])
            {
                case (int)HighScore.FiftyThousand:
                    result = "50000";
                    break;
                case (int)HighScore.Unlimited:
                    result = "Unlimited";
                    break;
            }
            return result;
        }
    }
}
