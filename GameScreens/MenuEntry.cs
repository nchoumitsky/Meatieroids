using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeatieroidsWindows
{
    // this class represents an item in a menu
    class MenuEntry
    {
        string displayText;

        /// Gets or sets the text of this menu entry.
        public string DisplayText
        {
            get { return displayText; }
            set { displayText = value; }
        }

        // Event raised when the menu entry is selected. set in the menuscreen calling it.
        public event EventHandler<PlayerIndexEventArgs> Selected;

        // Method for raising the Selected event.
        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs());
        }

        public MenuEntry(string text)
        {
            this.displayText = text;
        }

        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime) {}

        public virtual void Draw(MenuScreen screen, Vector2 position, bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Gold : Color.White;

            // Modify the alpha to fade text out during transitions.
            color = new Color(color.R, color.G, color.B, screen.TransitionAlphaValue);

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.GameFont;
            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, displayText, position, color, 0, origin, 1, SpriteEffects.None, 0);
        }

        // gets how much space this menu entry requires.
        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.GameFont.LineSpacing;
        }
    }
}
