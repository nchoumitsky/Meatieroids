using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Net;
namespace MeatieroidsWindows
{
    // This screen implements the actual game logic.
    class GameplayScreen : GameScreen
    {
        private ContentManager content;
        private SpriteFont screenFont;
        private SpriteManager spriteManager;
        private SoundEffect splatEffect;

        // gameplay variables
        private int level = 1;
        private int score;
        private int diffuculty;
        private bool spawnIsReady;

        //screen item variables and config
        private Vector2 scorePosition;
        private Vector2 levelPosition;
        private Vector2 opponentScorePosition;
        private Color textColor = Color.LightGreen;

        public GameplayScreen()
        {  
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(2.5);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            splatEffect = content.Load<SoundEffect>(@"splat");
            screenFont = ScreenManager.GameFont;

            // pause for a sec
            Thread.Sleep(800);
            opponentScorePosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + 40);
            scorePosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y);
            levelPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 200, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y);
            spriteManager = new SpriteManager(content,ScreenManager);

            //If we aren't on the XBOX the title safe region is the entire screen
            //Move the location of the strings so they are not on the edge of the screen
#if(!XBOX360)
            scorePosition.Y += 30;
            scorePosition.X += 30;
            levelPosition.Y += 30;
            levelPosition.X += 30;
            opponentScorePosition.Y += 30;
            opponentScorePosition.X += 30;
#endif
            base.LoadContent();

            diffuculty = ScreenManager.DiffucultyLevel;
            spawnIsReady = true;
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (isActive) //if this is the active screen
            {
                //We check to see if we should spawn meat.  This should only occur after the nextLevel screen
                //is exiting or the game is starting, in both multiplayer and single player.
                if (spawnIsReady && !IsExiting)
                {
                    spriteManager.SpawnMeat(3 + level * (diffuculty + 1));
                    spawnIsReady = false;
                }
                
                spriteManager.UpdateSprites(gameTime);
                collisionDetection();


                if (spriteManager.meatList.Count == 0 && spriteManager.explodingList.Count == 0 && spriteManager.forkList.Count == 0)
                    nextLevel();
            }
        }

        public override void HandleInput(InputManager input)
        {
            GamePadState gamePadState = input.CurrentGamePadState;
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected;

            if (input.IsPauseGame() || gamePadDisconnected)
            {

                ScreenManager.AddScreen(new PauseMenuScreen(this));
            }
            //If we hit the button necessary to fire a fork, we need to create the sprite and
            //if we are playing networked, let the other person know
            if (input.IsShootFork())
            {
                spriteManager.FireForks();
            }

        }

        public override void Draw(GameTime gameTime)
        {  
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.DrawString(screenFont, string.Concat("Score: ", score.ToString()), scorePosition, textColor);
            spriteBatch.DrawString(screenFont, string.Concat("Level: ", level.ToString()), levelPosition, textColor);
            spriteManager.Draw(gameTime, ref spriteBatch);
            spriteBatch.End();
           
            base.Draw(gameTime);

            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlphaValue);
        }

        //nextLevel() prepares us to enter the next level.  We add a screen that await user input to give them a chance
        //to relax for a moment and not suddenly have enemies pop onto their screen without warning.  We set the spawn
        //bool to true, however it will not spawn until the user returns from the next level screen because the game
        //also checks the isExiting bool, which is set to true upon the AddScreen function.  If we are hosting a network
        //game, we want to let the client know that it is time to end the level by sending them the NextLevel message
        private void nextLevel()
        {
            spawnIsReady = true;
            if (level > 0)
            {
                ScreenManager.AddScreen(new NextLevelScreen());  
            }
            level++;
        }

        //Collision detection tells the spriteManager run through the sprites and check for
        //collisions.  Depending on the result, the integer returned by the function, it
        //either adds to the scores(fork hits a meat) or it checks the amount of life and
        //either takes away one or if its ends the game
        private void collisionDetection()
        {
            MeatSprite oldMeat;
            if (spriteManager.CollisionDetect(out oldMeat))
            {
                if (score > ScreenManager.CurrentHighScore)
                    ScreenManager.CurrentHighScore = score;
                    ScreenManager.AddScreen(new GameOverScreen(score));
                    ExitScreen();
            }
            else if(oldMeat != null)
            {
                if(ScreenManager.SoundEnabled)
                     splatEffect.Play();
                score += (100 * oldMeat.Size);
                if (oldMeat.Size != 3)
                {
                    spriteManager.ExplosionSpawn(oldMeat);
                }
            }
        }

     }
}
