using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace MeatieroidsWindows
{
    class SpriteManager
    {
        ContentManager content;
        public UserControlledSprite player; 
        public NetworkUserSprite netPlayer;
        public List<MeatSprite> meatList = new List<MeatSprite>();
        public List<ForkSprite> forkList = new List<ForkSprite>();
        public List<ForkSprite> networkForkList = new List<ForkSprite>();
        public List<ExplodingMeatSprite> explodingList = new List<ExplodingMeatSprite>();
        ScreenManager screenManager;
        public List<Sprite> lifeList = new List<Sprite>();

        int screenWidth;
        int screenHeight;
        int titleSafeLeft;
        int titleSafeRight;
        int titleSafeBottom;
        int titleSafeTop;
        int enemyMinSpeed = 1;
        int enemyMaxSpeed = 3;
        bool isNetworkGame;
        NetworkManager netManager;
        Random random = new Random();

        public SpriteManager(ContentManager c, ScreenManager s)
        {
            isNetworkGame = false;
            screenManager = s;
            content = c;
            screenWidth = screenManager.GraphicsDevice.Viewport.Width;
            screenHeight = screenManager.GraphicsDevice.Viewport.Height;
            titleSafeLeft = screenManager.GraphicsDevice.Viewport.TitleSafeArea.Left;
            titleSafeTop = screenManager.GraphicsDevice.Viewport.TitleSafeArea.Top;
            titleSafeBottom = screenManager.GraphicsDevice.Viewport.TitleSafeArea.Bottom;
            titleSafeRight = screenManager.GraphicsDevice.Viewport.TitleSafeArea.Right;

            player = new UserControlledSprite(content.Load<Texture2D>(@"images\player1"),
            new Vector2(screenWidth / 2, screenHeight / 2),
            new Point(156, 128), 10, new Point(1, 1), new Point(4, 1), new Vector2(6, 6), 120);

#if(XBOX360)           
            lifeList.Add(new MeatSprite(content.Load<Texture2D>(@"images\parm"), new Vector2(titleSafeRight - 65, titleSafeTop + 10),
                                new Point(65, 65), 0, new Point(0, 0), new Point(0, 0), new Vector2(0, 0)));
            lifeList.Add(new MeatSprite(content.Load<Texture2D>(@"images\parm"), new Vector2(titleSafeRight - 65, titleSafeTop + 80),
                                new Point(65, 65), 0, new Point(0, 0), new Point(0, 0), new Vector2(0, 0)));
            lifeList.Add(new MeatSprite(content.Load<Texture2D>(@"images\parm"), new Vector2(titleSafeRight - 65, titleSafeTop + 150),
                                new Point(65, 65), 0, new Point(0, 0), new Point(0, 0), new Vector2(0, 0)));
#else
            lifeList.Add(new MeatSprite(content.Load<Texture2D>(@"images\parm"), 0, new Vector2(titleSafeRight - 100, titleSafeTop + 30),
                                new Point(65, 65), 0, new Point(0, 0), new Point(0, 0), new Vector2(0, 0)));
            lifeList.Add(new MeatSprite(content.Load<Texture2D>(@"images\parm"), 0, new Vector2(titleSafeRight - 100, titleSafeTop + 100),
                                new Point(65, 65), 0, new Point(0, 0), new Point(0, 0), new Vector2(0, 0)));
            lifeList.Add(new MeatSprite(content.Load<Texture2D>(@"images\parm"), 0, new Vector2(titleSafeRight - 100, titleSafeTop + 170),
                                new Point(65, 65), 0, new Point(0, 0), new Point(0, 0), new Vector2(0, 0)));
#endif

        }
        public SpriteManager(ContentManager c, ScreenManager s, NetworkManager nManager)
        {
            isNetworkGame = true;
            netManager = nManager;
            screenManager = s;
            content = c;
            screenWidth = screenManager.GraphicsDevice.Viewport.Width;
            screenHeight = screenManager.GraphicsDevice.Viewport.Height;
            titleSafeLeft = screenManager.GraphicsDevice.Viewport.TitleSafeArea.Left;
            titleSafeTop = screenManager.GraphicsDevice.Viewport.TitleSafeArea.Top;
            titleSafeBottom = screenManager.GraphicsDevice.Viewport.TitleSafeArea.Bottom;
            titleSafeRight = screenManager.GraphicsDevice.Viewport.TitleSafeArea.Right;

            if (isNetworkGame)
            {
                //if host, load your sprite on the left side, if not load it on the right
                if (netManager.netSession.IsHost)
                {
                    player = new UserControlledSprite(content.Load<Texture2D>(@"images\player1"),
                new Vector2(screenWidth / 4, screenHeight / 2),
                new Point(156, 128), 10, new Point(1, 1), new Point(4, 1), new Vector2(6, 6), 120);
                    netPlayer = new NetworkUserSprite(content.Load<Texture2D>(@"images\player2"),
                new Vector2(screenWidth * 3 / 4, screenHeight / 2),
                new Point(156, 128), 10, new Point(1, 1), new Point(1, 1), new Vector2(6, 6), 120);
                }
                else
                {
                    player = new UserControlledSprite(content.Load<Texture2D>(@"images\player1"),
                new Vector2(screenWidth * 3 / 4, screenHeight / 2),
                new Point(156, 128), 10, new Point(1, 1), new Point(4, 1), new Vector2(6, 6), 120);
                    netPlayer = new NetworkUserSprite(content.Load<Texture2D>(@"images\player2"),
                new Vector2(screenWidth / 4, screenHeight / 2),
                new Point(156, 128), 10, new Point(1, 1), new Point(1, 1), new Vector2(6, 6), 120);
                }

            }
            else
                player = new UserControlledSprite(content.Load<Texture2D>(@"images\player1"),
                new Vector2(screenWidth / 2, screenHeight / 2),
                new Point(156, 128), 10, new Point(1, 1), new Point(4, 1), new Vector2(6, 6), 120);

#if(XBOX360)           
            lifeList.Add(new MeatSprite(content.Load<Texture2D>(@"images\parm"), new Vector2(titleSafeRight - 65, titleSafeTop + 10),
                                new Point(65, 65), 0, new Point(0, 0), new Point(0, 0), new Vector2(0, 0)));
            lifeList.Add(new MeatSprite(content.Load<Texture2D>(@"images\parm"), new Vector2(titleSafeRight - 65, titleSafeTop + 80),
                                new Point(65, 65), 0, new Point(0, 0), new Point(0, 0), new Vector2(0, 0)));
            lifeList.Add(new MeatSprite(content.Load<Texture2D>(@"images\parm"), new Vector2(titleSafeRight - 65, titleSafeTop + 150),
                                new Point(65, 65), 0, new Point(0, 0), new Point(0, 0), new Vector2(0, 0)));
#else
            lifeList.Add(new MeatSprite(content.Load<Texture2D>(@"images\parm"), 0, new Vector2(titleSafeRight - 100, titleSafeTop + 30),
                                new Point(65, 65), 0, new Point(0, 0), new Point(0, 0), new Vector2(0, 0)));
            lifeList.Add(new MeatSprite(content.Load<Texture2D>(@"images\parm"), 0, new Vector2(titleSafeRight - 100, titleSafeTop + 100),
                                new Point(65, 65), 0, new Point(0, 0), new Point(0, 0), new Vector2(0, 0)));
            lifeList.Add(new MeatSprite(content.Load<Texture2D>(@"images\parm"), 0, new Vector2(titleSafeRight - 100, titleSafeTop + 170),
                                new Point(65, 65), 0, new Point(0, 0), new Point(0, 0), new Vector2(0, 0)));
#endif

        }


        public void fireForks()
        {
            forkList.Add(new ForkSprite(content.Load<Texture2D>(@"images\fork"), new Vector2(player.getPosition.X + 9, player.getPosition.Y - 10),
                 new Point(40, 75), 0, new Point(0, 0), new Point(0, 0), new Vector2(-12, -12), (player.getRotation)));

        }
        public void fireNetworkForks()
        {
            networkForkList.Add(new ForkSprite(content.Load<Texture2D>(@"images\fork"), new Vector2(netPlayer.getPosition.X + 9, netPlayer.getPosition.Y - 10),
                 new Point(40, 75), 0, new Point(0, 0), new Point(0, 0), new Vector2(-12, -12), (netPlayer.Rotation)));
        }

        public void UpdateSprites(GameTime gameTime)
        {
            // Update player
            player.Update(gameTime, new Rectangle(0, 0, screenWidth, screenHeight));

            // Update all non-player sprites
            foreach (MeatSprite m in meatList)
                m.Update(gameTime, new Rectangle(0, 0, screenWidth, screenHeight));
            foreach (ForkSprite f in forkList)
                f.Update(gameTime, new Rectangle(0, 0, screenWidth, screenHeight));
            foreach (ExplodingMeatSprite e in explodingList)
                e.Update(gameTime, new Rectangle(0, 0, screenWidth, screenHeight));


            //Go through the fork & exploding sprites to see if they need to be removed
            for (int i = 0; i < forkList.Count; i++)
            {
                ForkSprite f = forkList[i];
                if (f.getLifeTime() == 0) //check to see if it has been fired recently or needs to be removed from the screen
                {
                    forkList.RemoveAt(i);
                    --i;
                }
            }
            for (int i = 0; i < explodingList.Count; i++)
            {
                ExplodingMeatSprite e = explodingList[i];
                if (e.getLifeTime() == 0) //check to see if it explodingsprite is in last frame, if so, REMOVE
                {
                    explodingList.RemoveAt(i);
                    --i;
                }
            }


            // Update network sprites
            if (isNetworkGame)
            {
                netPlayer.Update(gameTime, new Rectangle(0, 0, screenWidth, screenHeight));
                foreach (ForkSprite f in networkForkList)
                    f.Update(gameTime, new Rectangle(0, 0, screenWidth, screenHeight));
                
                for (int i = 0; i < networkForkList.Count; ++i)
                {
                    ForkSprite f = networkForkList[i];
                    if (f.getLifeTime() == 0) //check to see if it has been fired recently or needs to be removed from the screen
                    {
                        networkForkList.RemoveAt(i);
                        --i;
                    }
                }
            }

        }

        public bool collisionDetect(out MeatSprite oldMeat)
        {
            oldMeat = null;
            for (int i = 0; i < meatList.Count; i++)
            {
                //first check if the player is being hit
                MeatSprite m = meatList[i];
                if (m.collisionRect.Intersects(player.collisionRect))
                {
                    oldMeat = meatList[i];
                    if (lifeList.Count > 0)
                    {
                        lifeList.RemoveAt(lifeList.Count - 1);
                        explodingList.Add(new ExplodingMeatSprite(content.Load<Texture2D>(@"images\explodingsheet"), new Vector2(m.getPosition.X - 50, m.getPosition.Y - 50),
                              new Point(200, 200), 0, new Point(2, 1), new Point(7, 1), new Vector2(0, 0)));
                        meatList.RemoveAt(i);
                        break;
                    }
                    else
                        return true;
                }
                //then check if a fork has hit one of the meat
                for (int j = 0; j < forkList.Count; j++)
                {
                    ForkSprite f = forkList[j];
                    if (m.collisionRect.Intersects(f.collisionRect))
                    {
                        explodingList.Add(new ExplodingMeatSprite(content.Load<Texture2D>(@"images\explodingsheet"), new Vector2(m.getPosition.X - 50, m.getPosition.Y - 50),
                               new Point(200, 200), 0, new Point(2, 1), new Point(7, 1), new Vector2(0, 0)));
                        oldMeat = meatList[i];
                        meatList.RemoveAt(i);
                        forkList.RemoveAt(j);
                        i--;
                        break;
                    }
                }
                //Do the same for the networked player
                if(isNetworkGame)
                {
                    if (m.collisionRect.Intersects(netPlayer.collisionRect))
                      {
                        explodingList.Add(new ExplodingMeatSprite(content.Load<Texture2D>(@"images\explodingsheet"), new Vector2(m.getPosition.X - 50, m.getPosition.Y - 50),
                             new Point(200, 200), 0, new Point(2, 1), new Point(7, 1), new Vector2(0, 0)));
                        meatList.RemoveAt(i);
                        break;
                      }
                    for (int j = 0; j < networkForkList.Count; ++j)
                    {
                        ForkSprite f = networkForkList[j];
                        if(m.collisionRect.Intersects(f.collisionRect))
                        {
                            explodingList.Add(new ExplodingMeatSprite(content.Load<Texture2D>(@"images\explodingsheet"), new Vector2(m.getPosition.X - 50, m.getPosition.Y - 50),
                             new Point(200, 200), 0, new Point(2, 1), new Point(7, 1), new Vector2(0, 0)));
                            meatList.RemoveAt(i);
                            networkForkList.RemoveAt(j);
                            break;
                            
                        }
                    }
                }               
            }

            return false;
        }

        public void SpawnMeat(int number) 
        {
            Vector2 speed = Vector2.Zero;
            Vector2 position = Vector2.Zero;
            Point frameSize = new Point(100, 100);

            if (isNetworkGame)
                netManager.StartMeatSend(number);
            
            for (int i = 0; i < number; i++)
            {
                switch (random.Next(4)) //spawn in a somewhat random location
                {
                    case 0: //spawns on left side
                        position = new Vector2(titleSafeLeft, (random.Next(titleSafeTop, titleSafeBottom)));
                        speed = new Vector2(random.Next(enemyMinSpeed, enemyMaxSpeed), random.Next(enemyMinSpeed, enemyMaxSpeed) / 2);
                        break;
                    case 1: // spawns on right side
                        position = new Vector2(titleSafeRight, random.Next(titleSafeTop,titleSafeBottom));
                        speed = new Vector2(-random.Next(enemyMinSpeed, enemyMaxSpeed), random.Next(enemyMinSpeed, enemyMaxSpeed) / 2);
                        break;
                    case 2: //spawns on bottom
                        position = new Vector2(random.Next(titleSafeLeft, titleSafeRight), titleSafeBottom);
                        speed = new Vector2(random.Next(enemyMinSpeed, enemyMaxSpeed) / 2, -random.Next(enemyMinSpeed, enemyMaxSpeed));
                        break;
                    case 3: //spawns on top
                        position = new Vector2(random.Next(titleSafeLeft, titleSafeRight), titleSafeTop);
                        speed = new Vector2(-random.Next(enemyMinSpeed, enemyMaxSpeed) / 2, random.Next(enemyMinSpeed, enemyMaxSpeed));
                        break;
                }
                meatList.Add(new MeatSprite(content.Load<Texture2D>(@"images\meat"), 1, position,
                                frameSize, 25, new Point(0, 0), new Point(0, 0), speed));
                if(isNetworkGame)
                    if((i-1)<number)
                        netManager.SendMeat(position, speed);
                    else
                        netManager.SendLastMeat(position, speed);
            }
        }
        public void ExplosionSpawn(MeatSprite oldMeat)
        {
            int size = oldMeat.Size + 1;
            Vector2 oldDirection = oldMeat.direction;
            Vector2 newSpeed1, newSpeed2, speed1, speed2;
            
            //rotation for the new meat
            float r = (2 * (float) Math.PI) / 8;

            newSpeed1.X = (float) (Math.Cos(r) * oldMeat.direction.X) - (float) (Math.Sin(r) * oldMeat.direction.Y);
            newSpeed1.Y = (float) (Math.Cos(r) *  oldMeat.direction.Y) + (float) (Math.Sin(r) * oldMeat.direction.X);
            Vector2.Multiply(ref newSpeed1, (float)1.4, out speed1);

            newSpeed2.X = (float) (Math.Cos(-r) * oldMeat.direction.X) - (float) (Math.Sin(-r) * oldMeat.direction.Y);
            newSpeed2.Y = (float) (Math.Cos(-r) * oldMeat.direction.Y) + (float) (Math.Sin(-r) * oldMeat.direction.X);
            Vector2.Multiply(ref newSpeed2, (float)1.4, out speed2);

            if (size == 2 )
            {
                meatList.Add(new MeatSprite(content.Load<Texture2D>(@"images\meat2"), 2, oldMeat.getPosition,
                                new Point(50, 50), 10, new Point(0, 0), new Point(0, 0), speed1));
                meatList.Add(new MeatSprite(content.Load<Texture2D>(@"images\meat2"), 2, oldMeat.getPosition,
                                new Point(50, 50), 10, new Point(0, 0), new Point(0, 0), speed2));
             
            }
            else if (size == 3)
            {
                meatList.Add(new MeatSprite(content.Load<Texture2D>(@"images\meat3"), 3, oldMeat.getPosition,
                                new Point(25, 25), 5, new Point(0, 0), new Point(0, 0), speed1));
                meatList.Add(new MeatSprite(content.Load<Texture2D>(@"images\meat3"), 3, oldMeat.getPosition,
                                new Point(25, 25), 5, new Point(0, 0), new Point(0, 0), speed2));
            }

            
        }

        public void Draw(GameTime gameTime, ref SpriteBatch spriteBatch)
        {
            player.Draw(gameTime, spriteBatch);
            if (isNetworkGame)
            {
                netPlayer.Draw(gameTime, spriteBatch);
                foreach (ForkSprite f in networkForkList)
                    f.Draw(gameTime, spriteBatch);
            }

            foreach (Sprite m in meatList)
                m.Draw(gameTime, spriteBatch);
            foreach (ForkSprite f in forkList)
                f.Draw(gameTime, spriteBatch);
            foreach (ExplodingMeatSprite e in explodingList)
                e.Draw(gameTime, spriteBatch);
            foreach (Sprite s in lifeList)
                s.Draw(gameTime, spriteBatch);
        }
    }
}
