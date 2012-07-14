using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    class ForkSprite : Sprite
    {
        int lifeTime = 50;
        float rotation;

        public ForkSprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed, float rotation) :
            base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed)
        {
            this.rotation = rotation;
        }
        public override Vector2 direction
        {
            get
            {
                Vector2 dir = Vector2.Zero;
                dir.X = (float)Math.Cos(rotation + MathHelper.PiOver2);
                dir.Y = (float)Math.Sin(rotation + MathHelper.PiOver2);
                return dir * speed;
            }
        }
        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            position += direction;
            lifeTime--;

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
        public int getLifeTime()
        {
            return lifeTime;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureImage, position,
                new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y),
                Color.White, rotation, new Vector2(frameSize.X / 2, frameSize.Y / 2), 1f, SpriteEffects.None, 0);
        }
    }
}
