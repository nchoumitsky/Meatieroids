using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using System;

namespace MeatieroidsWindows
{
    

    class LocalNetworkGameMenu : MenuScreen
    {
        MenuEntry gameTypeOption, opt2, opt3, opt4, opt5, highScoreOption;
        PlayerIndex currentPlayerIndex;
        SignedInGamer currentGamer;
        NetworkSession netSession;

        public LocalNetworkGameMenu(PlayerIndex enteringPlayer, NetworkSession nSession)
            : base("Local Network Game Lobby")
        {
            netSession = nSession;
            currentPlayerIndex = enteringPlayer;
            currentGamer = SignedInGamer.SignedInGamers[currentPlayerIndex];
            netSession.GameStarted += new EventHandler<GameStartedEventArgs>(loadNetworkGameScreen);
            
            // Create our menu entries.
            gameTypeOption = new MenuEntry("Game Type: " + GameType());
            highScoreOption = new MenuEntry("Score to Win: " + WinningScore());
            opt2 = new MenuEntry("Host: " + nSession.Host.ToString());
            opt3 = new MenuEntry("Ready?");
            opt4 = new MenuEntry("Waiting for opponent");
            opt5 = new MenuEntry("Go Back");

            netSession.GameStarted += new EventHandler<GameStartedEventArgs>(StartGame);

            gameTypeOption.Selected += changeGameType;
            highScoreOption.Selected += changeWinningScore;
            opt3.Selected += setReady;
            opt4.Selected += startGame;
            opt5.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(gameTypeOption);
            MenuEntries.Add(highScoreOption);
            MenuEntries.Add(opt2);
            MenuEntries.Add(opt3);
            MenuEntries.Add(opt4);
            MenuEntries.Add(opt5);

            SetMenuEntryText();
        }

        void StartGame(object sender, GameStartedEventArgs e)
        {
            NetworkManager netManager = new NetworkManager(netSession);
            ScreenManager.AddScreen(new NetworkGameScreen(netSession.IsHost, netManager));
        }
        public override void LoadContent()
        {
            base.LoadContent();
            SetMenuEntryText();
        }
        protected override void OnCancel()
        {
            //leave networksession
            netSession.Dispose();
            ScreenManager.AddScreen(new MainMenuScreen());
            ExitScreen();
        }

        public override void Draw(GameTime gameTime)
        {
            if (netSession.IsHost && netSession.AllGamers.Count == 21 )
            {
                if (netSession.IsEveryoneReady)
                {
                    opt4.DisplayText = "Oponent ready - start!";
                }
                else
                {
                    opt4.DisplayText = "Waiting for opponent";
                }
            }
            ScreenManager.FadeBackBufferToBlack(TransitionAlphaValue * 2 / 5);
            base.Draw(gameTime);
        }

        void SetMenuEntryText()
        {
           

        }

        void changeGameType(object sender, PlayerIndexEventArgs e)
        {
            //implement different network gameplay
        }

        void changeWinningScore(object sender, PlayerIndexEventArgs e)
        {
            //go through enumeration
        }

        void setReady(object sender, PlayerIndexEventArgs e)
        {
            foreach (LocalNetworkGamer g in netSession.LocalGamers)
            {
                g.IsReady = true;
                opt3.DisplayText = "Ready!  Waiting...";
            }
            //set the ready flag for 
        }

        void startGame(object sender, PlayerIndexEventArgs e)
        {
            if (netSession.IsHost)
            {
                if (netSession.IsEveryoneReady)
                {
                    netSession.StartGame();
                    foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
                    {
                        signedInGamer.Presence.PresenceMode = GamerPresenceMode.InCombat;
                    }
                }
            }

        }

        void loadNetworkGameScreen(object sender, GameStartedEventArgs e)
        {
            NetworkManager nManager = new NetworkManager(netSession);
            ScreenManager.AddScreen(new NetworkGameScreen(netSession.IsHost, nManager));

        }

        protected String GameType()
        {
            String result = "ERROR";
            switch ((int) netSession.SessionProperties[(int)SessionProperty.GameMode])
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
            switch ((int)netSession.SessionProperties[(int)SessionProperty.WinningScore])
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
