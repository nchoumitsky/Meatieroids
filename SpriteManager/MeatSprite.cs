using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    class MeatSprite : Sprite
    {
        private int size;

        public MeatSprite(Texture2D textureImage, int theSize, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed)
        {
            size = theSize;
        }
        public MeatSprite(Texture2D textureImage, int theSize, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed,int millisecondsPerFrame) :
            base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, millisecondsPerFrame)
        {
            size = theSize;
        }
        public override Vector2 direction
        {
            get { return speed; }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            
            position += direction;
            if (position.X < 0 - frameSize.X) // at the left of the screen, draw at the right
                position.X += clientBounds.Width + frameSize.X;
            if (position.Y < 0 - frameSize.Y) // at the top of the screen, draw at the bottom
                position.Y += clientBounds.Height + frameSize.Y;
            if (position.X > clientBounds.Width)// at the right of the screen, draw at the left
                position.X -= (clientBounds.Width + frameSize.X);
            if (position.Y > clientBounds.Height)//at the bottom of the screen, draw at the top
                position.Y -= (clientBounds.Height + frameSize.Y);
            base.Update(gameTime, clientBounds);
        }
        public int Size
        {
            get { return size; }
            set { size = value; }
        }

    }
}
