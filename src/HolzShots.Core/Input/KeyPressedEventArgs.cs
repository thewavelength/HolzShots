using System;
using System.Windows.Forms;

namespace HolzShots.Input
{
    /// <summary>Event Args for the event that is fired after the hotkey has been pressed.</summary>
    public class KeyPressedEventArgs : EventArgs
    {
        public ModifierKeys Modifier { get; }
        public Keys Key { get; }

        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public int GetIdentifier() => (int)Key << 16 | (int)Modifier;
    }
}
