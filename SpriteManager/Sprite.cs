using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    abstract class Sprite
    {
        protected Texture2D textureImage;
        protected Vector2 position;
        protected Point frameSize;
        protected int collisionOffset;
        protected Point currentFrame;
        protected Point sheetSize;
        private int timeSinceLastFrame = 0;
        private int millisecondsPerFrame;
        protected Vector2 speed;
        private const int defaultMillisecondsPerFrame = 16;

        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed)
            : this(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, defaultMillisecondsPerFrame)
        { }

        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed, int millisecondsPerFrame)
        {
            this.textureImage = textureImage;
            this.position = position;
            this.frameSize = frameSize;
            this.collisionOffset = collisionOffset;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.speed = speed;
            this.millisecondsPerFrame = millisecondsPerFrame;
        }

        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    ++currentFrame.Y;
                    if (currentFrame.Y >= sheetSize.Y)
                        currentFrame.Y = 0;
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureImage, position,
                new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y),
                Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public abstract Vector2 direction
        {
            get;
        }

        public Vector2 getPosition
        {
            get { return position; }
        }

        public virtual Rectangle CollisionRect
        {
            get
            {
                return new Rectangle((int)position.X + collisionOffset, (int)position.Y + collisionOffset,
                    frameSize.X - (collisionOffset * 2), frameSize.Y - (collisionOffset * 2));
            }
        }

        public bool IsOutOfBounds(Rectangle clientRect)
        {
            if (position.X < -frameSize.X ||
                position.X > clientRect.Width ||
                position.Y < -frameSize.Y ||
                position.Y > clientRect.Height)
                return true;
            return false;
        }
    }
}
