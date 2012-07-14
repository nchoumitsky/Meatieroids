using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using System;

namespace MeatieroidsWindows
{
    enum SessionProperty { GameMode, WinningScore }
    enum GameMode { HeadToHead, Other }
    enum HighScore { FiftyThousand, Unlimited }

    class NetworkGameMenu : MenuScreen
    {
        private MenuEntry opt1;
        private PlayerIndex currentPlayerIndex;
        private SignedInGamer currentGamer;
        private NetworkSessionProperties netSessionProperties;

        public NetworkGameMenu(PlayerIndex enteringPlayer)
            : base("Start a network game")
        {
            currentPlayerIndex = enteringPlayer;
            currentGamer = SignedInGamer.SignedInGamers[currentPlayerIndex];
            // Create our menu entries.
            if (currentGamer != null)
            {
                opt1 = new MenuEntry("Currently signed in");
            }
            else
            {
                opt1 = new MenuEntry("Select to sign in");
                SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(gamerSignIn);
            }
            MenuEntry opt2 = new MenuEntry("Host local network game");
            MenuEntry opt3 = new MenuEntry("Join local newtwork game");
            MenuEntry opt4 = new MenuEntry("Go Back");


            opt1.Selected += LiveSignIn;
            opt2.Selected += HostLocalGame;
            opt3.Selected += FindLocalGame;
            opt4.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(opt1);
            MenuEntries.Add(opt2);
            MenuEntries.Add(opt3);
            MenuEntries.Add(opt4);
            SetMenuEntryText();
        }
        public override void LoadContent()
        {
            base.LoadContent();
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
         
        }

        void gamerSignIn(object sender, SignedInEventArgs e)
        {
            if(e.Gamer == SignedInGamer.SignedInGamers[currentPlayerIndex])
            {
                opt1.DisplayText = "Currently signed in";
                SetMenuEntryText();
                currentGamer = e.Gamer;
                netSessionProperties = new NetworkSessionProperties();
            }
            else
            {
                //doesn't matter
            }
        }

        void LiveSignIn(object sender, PlayerIndexEventArgs e)
        {
            currentGamer = SignedInGamer.SignedInGamers[currentPlayerIndex];
            if (currentGamer != null)
            {
                opt1.DisplayText = "Currently signed in";
                SetMenuEntryText();
            }
            else
            {
                Guide.ShowSignIn(1, false);
            }

        }

        void HostLocalGame(object sender, PlayerIndexEventArgs e)
        {
            if (currentGamer != null)
            {
                netSessionProperties[(int)SessionProperty.GameMode] = (int)GameMode.HeadToHead;
                netSessionProperties[(int)SessionProperty.WinningScore] = (int)HighScore.FiftyThousand;
                NetworkSession newSession = NetworkSession.Create(NetworkSessionType.Local, 1, 2, 0, netSessionProperties);
                ScreenManager.AddScreen(new LocalNetworkGameMenu(currentPlayerIndex, newSession));
            }
            else
            {
                Guide.ShowSignIn(1, false);
                if (currentGamer != null)
                {
                    netSessionProperties[(int)SessionProperty.GameMode] = (int)GameMode.HeadToHead;
                    netSessionProperties[(int)SessionProperty.WinningScore] = 50000;
                    NetworkSession newSession = NetworkSession.Create(NetworkSessionType.Local, 1, 2, 0, netSessionProperties);
                    ScreenManager.AddScreen(new LocalNetworkGameMenu(currentPlayerIndex, newSession));
                }
            }
            
        }

        void FindLocalGame(object sender, PlayerIndexEventArgs e)
        {
            //ADD: pop up a window with games and info on them
            if (currentGamer != null)
            {
                AvailableNetworkSessionCollection sessions = NetworkSession.Find(NetworkSessionType.SystemLink, 1, netSessionProperties);
                ScreenManager.AddScreen(new SearchLocalNetworkScreen(currentPlayerIndex, sessions));
            }
            else
            {
                Guide.ShowSignIn(1, false);
                if (currentGamer != null)
                {
                    AvailableNetworkSessionCollection sessions = NetworkSession.Find(NetworkSessionType.SystemLink, 1, netSessionProperties);
                    ScreenManager.AddScreen(new SearchLocalNetworkScreen(currentPlayerIndex, sessions));
                }
            }

        }

    }
}
