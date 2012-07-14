 using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MeatieroidsWindows
{
    // main class for handling user input

    public class InputManager
    {
        public KeyboardState CurrentKeyboardState;
        public GamePadState CurrentGamePadState;
        public KeyboardState LastKeyboardState;
        public GamePadState LastGamePadState;
        public bool GamePadWasConnected;

        private PlayerIndex playerIndex;

        // Constructs a new input state.
        public InputManager()
        {
            CurrentKeyboardState = new KeyboardState();
            CurrentGamePadState = new GamePadState();
            LastKeyboardState = new KeyboardState();
            LastGamePadState = new GamePadState();
            playerIndex = PlayerIndex.One;
            GamePadWasConnected = false;
        }

        // get the current state of a keyboard or controller
        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            LastGamePadState = CurrentGamePadState;
            CurrentKeyboardState = Keyboard.GetState();
            CurrentGamePadState = GamePad.GetState(playerIndex);

            // Keep track of whether a gamepad has ever been
            // connected, so we can detect if it is unplugged.
            if (CurrentGamePadState.IsConnected)
                GamePadWasConnected = true;
        }

        // Helper for checking if a key was newly pressed during this update. The
        public bool IsNewKeyPress(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyUp(key));
        }
        public bool IsKeyPress(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key));
        }
        // Helper for checking if a button was newly pressed during this update.
        public bool IsNewButtonPress(Buttons button)
        {
            // check to make sure the player isnt just holding the buttons down
            return (CurrentGamePadState.IsButtonDown(button) && LastGamePadState.IsButtonUp(button));
        }
        public bool IsButtonPress(Buttons button)
        {
            return (CurrentGamePadState.IsButtonDown(button));
        }

        // Checks for a "menu select" input action.
        public bool IsMenuSelect()
        {
            return IsNewKeyPress(Keys.Space) ||
                   IsNewKeyPress(Keys.Enter) ||
                   IsNewButtonPress(Buttons.A) ||
                   IsNewButtonPress(Buttons.Start);
        }

        // Checks for a "menu cancel" input action.
        public bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape) ||
                   IsNewButtonPress(Buttons.B) ||
                   IsNewButtonPress(Buttons.Back);
        }

        // Checks for a "menu up" input action.
        public bool IsMenuUp()
        {
            return IsNewKeyPress(Keys.Up) ||
                   IsNewButtonPress(Buttons.DPadUp) ||
                   IsNewButtonPress(Buttons.LeftThumbstickUp);
        }

        // Checks for a "menu down" input action.
        public bool IsMenuDown()
        {
            return IsNewKeyPress(Keys.Down) ||
                   IsNewButtonPress(Buttons.DPadDown) ||
                   IsNewButtonPress(Buttons.LeftThumbstickDown);
        }

        // Checks for a "pause the game" input action.
        public bool IsPauseGame()
        {
            return IsNewKeyPress(Keys.Escape) ||
                   IsNewButtonPress(Buttons.Back) ||
                   IsNewButtonPress(Buttons.Start);
        }

        // sees if the player shot a fork
        public bool IsShootFork()
        {
            return IsNewKeyPress(Keys.Space) ||
                   IsNewButtonPress(Buttons.A);
        }  

        // This code allowed us to check if the player is still moving
        public bool IsMoving()
        {
#if(!XBOX360)
            return IsKeyPress(Keys.Up) || IsKeyPress(Keys.Down)||
                IsKeyPress(Keys.Left) || IsKeyPress(Keys.Right)||
                IsButtonPress(Buttons.LeftThumbstickLeft) || IsButtonPress(Buttons.LeftThumbstickRight) ||
                IsButtonPress(Buttons.RightThumbstickLeft) || IsButtonPress(Buttons.RightThumbstickRight) ||
                IsButtonPress(Buttons.RightTrigger) || IsButtonPress(Buttons.DPadRight) ||
                IsButtonPress(Buttons.DPadLeft);
#endif

            return IsButtonPress(Buttons.LeftThumbstickLeft) || IsButtonPress(Buttons.LeftThumbstickRight) ||
                IsButtonPress(Buttons.RightThumbstickLeft) || IsButtonPress(Buttons.RightThumbstickRight) ||
                IsButtonPress(Buttons.RightTrigger) || IsButtonPress(Buttons.DPadRight) ||
                IsButtonPress(Buttons.DPadLeft);
        }
    }
}
