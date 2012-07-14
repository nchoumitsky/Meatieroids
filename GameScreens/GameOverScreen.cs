using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace MeatieroidsWindows
{
    class GameOverScreen : GameScreen
    {
        protected string endGameMessage, score1Text, score2Text;
        int score, networkScore;
        SpriteFont screenFont;
        bool isNetworkGame;
        NetworkManager netManager;

        public GameOverScreen(int score)
        {
            isNetworkGame = false;
            this.score = score;
            IsPopupWindow = false;
            TransitionOnTime = TimeSpan.FromSeconds(0.6);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        public GameOverScreen(int score, int networkScore, NetworkManager nManager)
        {
            netManager = nManager;
            this.score = score;
            this.networkScore = networkScore;
            IsPopupWindow = false;
            TransitionOnTime = TimeSpan.FromSeconds(0.6);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;
            screenFont = ScreenManager.GameFont;

            if (isNetworkGame)
            {
                if(score > networkScore)
                {
                    endGameMessage = "YOU WIN!!";
                    score1Text = "Your score: " + score + "  Their score: " + networkScore;
                }
                else if(score < networkScore)
                {
                    endGameMessage = "YOU LOSE!!";
                    score1Text = "Your score: " + score + "  Their score: " + networkScore;
                }
                else
                {
                    endGameMessage = "Tie, only the meat win....";
                    score1Text = "Your score: " + score + "  Their score: " + networkScore;
                }
                netManager.CleanUpNetwork();
            }
            else
            {
                endGameMessage = "TOO MUCH TO EAT!";
                score1Text = "Your score : " + score;
            }
            score2Text = "Your Hi score: " + ScreenManager.CurrentHighScore;
        }

        public override void HandleInput(InputManager input)
        {
            if (input.IsMenuCancel())
            {
                if (isNetworkGame)
                {
                    foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
                    {
                        signedInGamer.Presence.PresenceMode = GamerPresenceMode.WaitingInLobby;
                    }
                }
                ScreenManager.AddScreen(new BackgroundScreen());
                ScreenManager.AddScreen(new MainMenuScreen());
                ExitScreen();
            }

            base.HandleInput(input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.GameFont;

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(endGameMessage);
            Vector2 score1Size = font.MeasureString(score1Text);
            Vector2 score2Size = font.MeasureString(score2Text);
            Vector2 textPosition = (viewportSize - textSize) / 2;
            Vector2 score1Position = (viewportSize - score1Size) / 2;
            Vector2 score2Position = (viewportSize - score2Size) / 2;
            Color color = new Color(255, 255, 255, TransitionAlphaValue);

            score1Position.Y += 50;
            score2Position.Y += 100;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlphaValue * 2 / 3);

            spriteBatch.Begin();
            spriteBatch.DrawString(screenFont, endGameMessage, textPosition, color);
            spriteBatch.DrawString(screenFont, score1Text, score1Position, color);
            spriteBatch.DrawString(screenFont, score2Text, score2Position, color);
            spriteBatch.End();
        }
    }
}
