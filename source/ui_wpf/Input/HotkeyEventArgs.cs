using System;

namespace Se7enRedLines.UI
{
    public class HotkeyEventArgs : EventArgs
    {
        //======================================================
        #region _Constructors_

        public HotkeyEventArgs(HotkeyInfo hotkey)
        {
            Hotkey = hotkey;
        }

        #endregion

        //======================================================
        #region _Public properties_

        public HotkeyInfo Hotkey { get; protected set; }

        #endregion
    }
}