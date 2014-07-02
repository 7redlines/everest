using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace Se7enRedLines.UI
{
    public class HotkeyInfo
    {
        //======================================================
        #region _Constructors_

        public HotkeyInfo(KeyGesture keyGesture)
            : this(0, string.Empty, keyGesture, HotkeyStatus.NotConfigured)
        {
        }

        public HotkeyInfo(string name, KeyGesture keyGesture)
            : this(0, name, keyGesture, HotkeyStatus.NotConfigured)
        {
        }

        internal HotkeyInfo(ushort id, string name, KeyGesture keyGesture, HotkeyStatus status)
        {
            Contract.Requires(keyGesture != null);
            Contract.Requires(keyGesture.Key != Key.None);

            Id = id;
            Name = name;
            KeyGesture = keyGesture;
            Status = status;
        }

        #endregion

        //======================================================
        #region _Public properties_

        public ushort Id { get; internal protected set; }
        public string Name { get; internal protected set; }

        public KeyGesture KeyGesture { get; protected set; }
        public HotkeyStatus Status { get; internal protected set; }

        #endregion 

        //======================================================
        #region _Private, protected, internal methods_

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
                return string.Format("{0}: {1}", Name, KeyGesture);

            return KeyGesture.ToString();
        }

        #endregion
    }
}