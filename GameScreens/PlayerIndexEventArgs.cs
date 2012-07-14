using System;
using Microsoft.Xna.Framework;

namespace MeatieroidsWindows
{
    /// Custom event argument which includes the index of the player who
    /// triggered the event. This is used by the MenuEntry.Selected event.
    class PlayerIndexEventArgs : EventArgs
    {
        public PlayerIndexEventArgs()
        {
        }

        // Gets the index of the player who triggered this event.
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }

        // this is always going to be once since we always only have one player
        PlayerIndex playerIndex = PlayerIndex.One;
    }
}
