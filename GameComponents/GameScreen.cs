using System;
using Microsoft.Xna.Framework;

namespace MeatieroidsWindows
{
    // this class defines the base screen that all of the screens the user
    // sees in the game inherit from. 

    public enum ScreenState
    {
        TransitionOn,
        TransitionOff,
        Active,
        Hidden,
    }

    public abstract class GameScreen
    {
        private bool otherScreenHasFocus;
        private float transitionPosition = 1;
        private float transitionDelta = 1;

        private TimeSpan transitionOnTime = TimeSpan.Zero;
        private TimeSpan transitionOffTime = TimeSpan.Zero;
        private ScreenState screenState = ScreenState.TransitionOn;

        private ScreenManager screenManager;

        public bool IsPopupWindow { get; set; }

        // gets the current state of this screen
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }
    
        // how far into transitioning in or out the screen is
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        // gets the alpha value (how faded away) of the screen
        public byte TransitionAlphaValue
        {
            get { return (byte)(255 - TransitionPosition * 255); }
        }

        // checks if this screen is active
        public bool isActive
        {
            get 
            { 
                return !otherScreenHasFocus && 
                (screenState == ScreenState.TransitionOn || screenState == ScreenState.Active); 
            }
        }

        // property representing if a screen is going away, if so the screen will remove itself
        // after its last draw/update has been called
        public bool IsExiting { get; set; }

        // returns which ScreenManager this belongs to
        public ScreenManager ScreenManager { get; set; }

        // load and unload methods to be overridden by children instances 
        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }

        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (IsExiting) // the current screen is going away
            {
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    ScreenManager.RemoveScreen(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                // UpdateTransition will return something if its still transitioning
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                    screenState = ScreenState.TransitionOff;
                else
                    screenState = ScreenState.Hidden;
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                    screenState = ScreenState.TransitionOn;
                else
                    screenState = ScreenState.Active;
            }
        }

        // Helper for updating the screen transition position.
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            // Update the transition position.
            transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) || ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }
            else
                return true;
        }

        // Child instance input logic will be handled here
        public virtual void HandleInput(InputManager input) { }

        // This is called when the screen should draw itself.
        public virtual void Draw(GameTime gameTime) { }

        // removes the screen nicely, instead of just making it go away
        public void ExitScreen()
        {
                IsExiting = true;
        }
    }
}
