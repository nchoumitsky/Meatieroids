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
    class NetworkGameScreen : GameScreen
    {
        private ContentManager content;
        private SpriteFont screenFont;
        private SpriteManager spriteManager;
        private SoundEffect splatEffect;

        // gameplay variables
        private int level = 1;
        private int score;
        private int opponentScore;
        private int diffuculty;
        private bool spawnIsReady;
        private bool isHost;
        //screen item variables and config
        private Vector2 scorePosition;
        private Vector2 levelPosition;
        private Vector2 opponentScorePosition;
        private Color textColor = Color.LightGreen;
        private NetworkManager netManager;

        public NetworkGameScreen(bool host, NetworkManager nManager)
        {
            isHost = host;
            netManager = nManager;
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
            spriteManager = new SpriteManager(content,ScreenManager, netManager);

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
                    if (isHost)         
                        spriteManager.SpawnMeat(3 + level * (diffuculty + 1));

                    spawnIsReady = false;
                }
                
                spriteManager.UpdateSprites(gameTime);
                CollisionDetection();

                //Do everything related to updating network states only if we're a network game

                UpdateNetworkGameplay();
                if (spriteManager.meatList.Count == 0 && spriteManager.explodingList.Count == 0 && spriteManager.forkList.Count == 0 
                    && spriteManager.networkForkList.Count == 0 && netManager.netSession.IsHost)
                    NextLevel();

                else if (spriteManager.meatList.Count == 0 && spriteManager.explodingList.Count == 0 && spriteManager.forkList.Count == 0)
                    NextLevel();
            }
        }

        //The additional updates that have to do specifically with network play.  This consists
        //mainly of checking to see if there is a packet waiting for us and then, depending on
        //the packet header, doing the necessary action
        private void UpdateNetworkGameplay()
        {
            Vector2 location = new Vector2();
            Vector2 speed = new Vector2();

            switch (netManager.GetMessageType())
            {
                //The packet header would tell us that the host called spawn meat
                //Then we would read the next member of the packet, an int, telling us how much meat
                //We loop through the rest of the packet to check the coordinates and
                //the motion vectors of the meat and set them as we add them to the sprite manager
                case MessageType.SpawnMeat:
                    int i = netManager.GetNumberOfMeat();
                    for (int k = 0; k < i; k++)
                    {
                        netManager.GetMeat(ref location, ref speed);
                        spriteManager.meatList.Add(new MeatSprite(content.Load<Texture2D>(@"images\meat"), 1, location,
                                new Point(100, 100), 0, new Point(0, 0), new Point(0, 0), speed));
                    }
                    break;
                case MessageType.Score:
                    netManager.ReadScoreMessgae(ref opponentScore);
                    break;
                case MessageType.Nothing:
                    break;
                case MessageType.FireFork:
                    spriteManager.FireNetworkForks();
                    break;
                case MessageType.PauseGame:
                    netManager.RemoteReady = false;
                    ScreenManager.AddScreen(new PauseMenuScreen(this, netManager));
                    break;
                case MessageType.EndGame:
                    if (score > ScreenManager.CurrentHighScore)
                        ScreenManager.CurrentHighScore = score;
                    ScreenManager.AddScreen(new GameOverScreen(score, opponentScore, netManager));
                    ExitScreen();
                    break;
                //When we receive a player location header for a packet, the packet would
                //contain the position and rotation data for the player
                //We would read the next two data items in the class, the location and rotation vectors
                case MessageType.PlayerLocation:
                    spriteManager.netPlayer.location = netManager.ReadLocation();
                    spriteManager.netPlayer.Rotation = netManager.ReadRotation();
                    break;
                //The only person that receives this packet is the client, so this is how they know
                //that they need to skip to the nextLevel() method - the only way they could possibly
                //reach it
                case MessageType.NextLevel:
                    if (spriteManager.meatList.Count > 0)       //error checking - if any messages got
                        spriteManager.meatList.Clear();         //messed up, due to network lag make
                    if (spriteManager.networkForkList.Count > 0)  //sure we are synced for the next level
                        spriteManager.networkForkList.Clear();
                    NextLevel();                                
                    break;
            }
        }

        public override void HandleInput(InputManager input)
        {
            GamePadState gamePadState = input.CurrentGamePadState;
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected;

            if (input.IsPauseGame() || gamePadDisconnected)
            {

                netManager.RemoteReady = false;
                netManager.SendPauseMessage();


                ScreenManager.AddScreen(new PauseMenuScreen(this, netManager));
            }
            //If we hit the button necessary to fire a fork, we need to create the sprite and
            //if we are playing networked, let the other person know
            if (input.IsShootFork())
            {
                spriteManager.FireForks();
                netManager.SendFireForkMessage();
            }

            //We want to send the user our current position and rotation values however we do
            //not want to send too often or we clog up the network.  To accomplish this, we only
            //send our current location/position when it is changing - the other player will
            //know our present state if we do not move
            netManager.SendLocation(spriteManager.player.getPosition, spriteManager.player.getRotation);
        }

        public override void Draw(GameTime gameTime)
        {  
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.DrawString(screenFont, string.Concat("Score: ", score.ToString()), scorePosition, textColor);
            spriteBatch.DrawString(screenFont, string.Concat("Level: ", level.ToString()), levelPosition, textColor);
            spriteBatch.DrawString(screenFont, string.Concat("Opponent Score: ", opponentScore.ToString()), opponentScorePosition, textColor);
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
        private void NextLevel()
        {
            spawnIsReady = true;
            if (level > 0)
            {
                if(isHost)
                    netManager.SendNextLevelMessage();
                ScreenManager.AddScreen(new NextLevelScreen(netManager));  
            }
            level++;
        }

        //Collision detection tells the spriteManager run through the sprites and check for
        //collisions.  Depending on the result, the integer returned by the function, it
        //either adds to the scores(fork hits a meat) or it checks the amount of life and
        //either takes away one or if its ends the game
        private void CollisionDetection()
        {
            MeatSprite oldMeat;
            if (spriteManager.CollisionDetect(out oldMeat))
            {
                if (score > ScreenManager.CurrentHighScore)
                    ScreenManager.CurrentHighScore = score;


                    netManager.SendEndGameMessage();
                    ScreenManager.AddScreen(new GameOverScreen(score, opponentScore, netManager));
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

                netManager.SendScoreMessage(score);
            }
        }

     }
}
