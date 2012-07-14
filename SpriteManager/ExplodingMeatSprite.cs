using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    class ExplodingMeatSprite : Sprite
    {
        private int lifeTime = 22;

        public ExplodingMeatSprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed)
        {
        }

        public ExplodingMeatSprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed, int millisecondsPerFrame) :
            base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, millisecondsPerFrame)
        {
        }

        public override Vector2 direction
        {
            get { return speed; }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            lifeTime--;
            base.Update(gameTime, clientBounds);
        }
        public int getLifeTime()
        {
            return lifeTime;
        }
        public int size
        {
            get { return 3; }
        }

    }
}
