using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace MeatieroidsWindows
{
    class NetworkUserSprite : Sprite
    {
        private float rotation = 0;
        private Vector2 Directing = new Vector2(0, -1);
        private InputManager inputManager = new InputManager();

        public NetworkUserSprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed)
        {
        }

        public NetworkUserSprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed, int millisecondsPerFrame) :
            base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, millisecondsPerFrame)
        {

        }

        public override Vector2 direction
        {
            get {return position;}
        }

        public  Vector2 location
        {
            get { return position; }
            set { position = value; }
        }

        public float Rotation
        {
            get { return rotation; }

            set { rotation = value; }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureImage, position,
                new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y),
                Color.White, rotation, new Vector2(frameSize.X / 2, frameSize.Y / 2), 1f, SpriteEffects.None, 0);
        }

        public override Rectangle CollisionRect
        {
            get
            {
                return new Rectangle((int)position.X - (int)(.5 * frameSize.X) + collisionOffset, (int)position.Y - (int)(.5 * frameSize.Y) + collisionOffset,
                    frameSize.X - (collisionOffset * 2), frameSize.Y - (collisionOffset * 2));
            }
        }
    }
}
