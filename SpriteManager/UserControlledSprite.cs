using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace MeatieroidsWindows
{
    class UserControlledSprite : Sprite
    {
        private float rotation = 0;
        private float pointer;
        private Vector2 Directing = new Vector2(0, -1);
        private InputManager inputManager = new InputManager();

        public UserControlledSprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed)
        {
        }

        public UserControlledSprite(Texture2D textureImage, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame,
            Point sheetSize, Vector2 speed, int millisecondsPerFrame) :
            base(textureImage, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, millisecondsPerFrame)
        {

        }

        public float getRotation
        {
            get { return rotation; }
        }

        public override Vector2 direction
        {
            get
            {
                Vector2 inputDirection = Vector2.Zero;
#if(!XBOX360)
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    Rotation -= 1f / 10;
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    Rotation += 1f / 10;
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    return -Directing * speed;
#endif
               
                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft)||
                    GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickLeft))
                    Rotation -= 1f / 10;
                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight)||
                    GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.LeftThumbstickRight))
                    Rotation += 1f / 10;
                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightTrigger))
                    return -Directing * speed;
                return inputDirection * speed;
            }
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

        public float Rotation
        {
            get { return rotation; }

            set
            {

                //Maintain the angle between 0 and TwoPi
                rotation = MathHelper.Clamp(value,0,MathHelper.TwoPi);
                if (rotation == 0)
                    rotation = MathHelper.TwoPi;
                else if (rotation == MathHelper.TwoPi)
                    rotation = 0;
                pointer = rotation + MathHelper.PiOver2;

                Directing.X = (float)Math.Cos(pointer);
                Directing.Y = (float)Math.Sin(pointer);
            }
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
                return new Rectangle((int)position.X - (int)(.5 * frameSize.X) + collisionOffset, (int)position.Y -  (int)(.5 * frameSize.Y) + collisionOffset,
                    frameSize.X - (collisionOffset * 2), frameSize.Y - (collisionOffset * 2));
            }
        }
    }
}
