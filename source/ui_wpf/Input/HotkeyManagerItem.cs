using GalaSoft.MvvmLight.Helpers;

namespace Se7enRedLines.UI
{
    internal class HotkeyManagerItem
    {
        //======================================================
        #region _Constructors_

        public HotkeyManagerItem(ushort hotkeyId, WeakAction<HotkeyEventArgs> action)
        {
            HotkeyId = hotkeyId;
            Action = action;
        }

        #endregion

        //======================================================
        #region _Public properties_

        public ushort HotkeyId { get; set; }
        public WeakAction<HotkeyEventArgs> Action { get; protected set; }

        #endregion 
    }
}