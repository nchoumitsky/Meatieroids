using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    abstract class MenuScreen : GameScreen
    {
        private List<MenuEntry> menuEntries = new List<MenuEntry>();
        private int selectedEntry = 0;
        private string menuTitle;

        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        public MenuScreen(string menuTitle)
        {
            this.MenuTitle = menuTitle;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public string MenuTitle { get; set; }

        public override void HandleInput(InputManager input)
        {
            // Move to the previous menu entry
            if (input.IsMenuUp())
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            // Move to the next menu entry
            if (input.IsMenuDown())
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            if (input.IsMenuSelect())
            {
                OnSelectEntry(selectedEntry);
            }
            else if (input.IsMenuCancel())
            {
                OnCancel();
            }
        }

        protected virtual void OnSelectEntry(int entryIndex)
        {
            // call whatever we specified to be the action handler asociated with that menu entry
            menuEntries[selectedEntry].OnSelectEntry();
        }

        protected virtual void OnCancel()
        {
            ExitScreen();
        }

        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            Viewport view = ScreenManager.GraphicsDevice.Viewport;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.GameFont;

            // make the menu nicely slide into the screen
            float transitionDelta = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title and entries
            Vector2 menuItemPosition = new Vector2(100, 150);
            Vector2 titlePosition = new Vector2(view.Width / 2, view.Height / 9);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = Color.White * TransitionAlphaValue;
            float titleScale = 2f;
            float menuDistance = 20;

            // slide the title on screen instead of just plopping it in there
            titlePosition.X -= transitionDelta * 100;

            if (ScreenState == ScreenState.TransitionOn || ScreenState == ScreenState.TransitionOff)
                menuItemPosition.X -= transitionDelta * 256;

            spriteBatch.Begin();
            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];
                bool isSelected = isActive && (i == selectedEntry);
                menuEntry.Draw(this, menuItemPosition, isSelected, gameTime);
                // increment the position vector for the next menu entry
                menuItemPosition.Y += menuEntry.GetHeight(this) + menuDistance;
            }

            spriteBatch.End();
        }
    }
}
