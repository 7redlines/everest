using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Se7enRedLines.UI.Native;

namespace Se7enRedLines.UI
{
    public delegate void HotkeyEventHandler(HotkeyEventArgs args);

    /// <summary>
    /// Low level hotkey operations.
    /// </summary>
    public static class HotkeyHook
    {
        //======================================================
        #region _Constructors_

        static HotkeyHook()
        {
            HotkeyRepeatLimit = TimeSpan.FromSeconds(1);
            _hotKeyForm = new WinProcHandlerForm();
            Registered = new HotkeyInfo[0];
            _repeatLimitTimer = Stopwatch.StartNew();
        }

        #endregion

        //======================================================
        #region _Public properties_

        internal static HotkeyInfo[] Registered { get; set; }

        public static event HotkeyEventHandler HotkeyPressed;

        public static TimeSpan HotkeyRepeatLimit { get; set; }

        #endregion

        //======================================================
        #region _Public methods_

        public static bool Register(HotkeyInfo hotkey)
        {
            Contract.Requires(hotkey != null);

            if (Registered.Contains(hotkey))
                return true;

            if (hotkey.Status != HotkeyStatus.Registered)
            {
                if (hotkey.Id == 0)
                {
                    hotkey.Id = NativeMethods.GlobalAddAtom();

                    if (hotkey.Id == 0)
                    {
                        Trace.TraceError("Unable to generate unique hotkey ID: " + hotkey.KeyGesture);
                        hotkey.Status = HotkeyStatus.Failed;
                        return false;
                    }
                }

                var keyCode = KeyInterop.VirtualKeyFromKey(hotkey.KeyGesture.Key);

                if (!NativeMethods.RegisterHotKey(_hotKeyForm.Handle, hotkey.Id, (uint)hotkey.KeyGesture.Modifiers.ToVirtual(), (uint)keyCode))
                {
                    NativeMethods.GlobalDeleteAtom(hotkey.Id);
                    Trace.TraceError("Unable to register hotkey: " + hotkey.KeyGesture);
                    hotkey.Id = 0;
                    hotkey.Status = HotkeyStatus.Failed;
                    return false;
                }

                hotkey.Status = HotkeyStatus.Registered;

                Registered = Registered.Concat(new[] {hotkey}).ToArray();
            }

            CheckHook();

            return true;
        }

        public static bool Unregister(HotkeyInfo hotkey)
        {
            Contract.Requires(hotkey != null);

            if (!Registered.Contains(hotkey))
            {
                Trace.TraceWarning("Hotkey is not registed");
                return false;
            }

            if (hotkey.Id > 0)
            {
                bool result = NativeMethods.UnregisterHotKey(_hotKeyForm.Handle, hotkey.Id);

                if (result)
                {
                    NativeMethods.GlobalDeleteAtom(hotkey.Id);
                    hotkey.Id = 0;
                    hotkey.Status = HotkeyStatus.NotConfigured;

                    Registered = Registered.Except(new[] {hotkey}).ToArray();
                    CheckHook();

                    return true;
                }
            }

            hotkey.Status = HotkeyStatus.Failed;

            return false;
        }

        public static bool Unregister()
        {
            var result = true;
            foreach (var info in Registered)
            {
                result = result && Unregister(info);
            }

            return result;
        }

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        private static void CheckHook()
        {
            _hotKeyForm.WndProcHandler -= OnWndProc;

            if (Registered.Length != 0)
                _hotKeyForm.WndProcHandler += OnWndProc;
        }

        private static bool OnWndProc(object sender, Message msg)
        {
            if (msg.Msg == (int)WinMsg.WM_HOTKEY && CheckRepeatLimitTime())
            {
                var id = (ushort)msg.WParam;
                //var key = (Keys)(((int)msg.LParam >> 16) & 0xFFFF);
                var key = ((int)msg.LParam >> 16) & 0xFFFF;

                var keyModifiers = (KEYMODIFIERS)((int)msg.LParam & 0xFFFF);

                var gesture = new KeyGesture(KeyInterop.KeyFromVirtualKey(key), keyModifiers.FromVirtual());

                return OnWndHotkey(id, gesture);
            }

            return false;
        }

        private static bool OnWndHotkey(ushort id, KeyGesture keyGesture)
        {
            var hotkey = Registered.FirstOrDefault(h =>
                h.Id == id && h.KeyGesture.Key == keyGesture.Key &&
                h.KeyGesture.Modifiers == keyGesture.Modifiers);

            return hotkey != null && RaiseHotkeyPressed(hotkey);
        }

        private static bool RaiseHotkeyPressed(HotkeyInfo hotkey)
        {
            if (HotkeyPressed != null)
                HotkeyPressed(new HotkeyEventArgs(hotkey));

            return true;
        }

        private static bool CheckRepeatLimitTime()
        {
            if (HotkeyRepeatLimit != TimeSpan.Zero)
            {
                if (_repeatLimitTimer.Elapsed >= HotkeyRepeatLimit)
                {
                    _repeatLimitTimer.Reset();
                    _repeatLimitTimer.Start();
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private static ModifierKeys FromVirtual(this KEYMODIFIERS @virtual)
        {
            var result = ModifierKeys.None;

            if (@virtual.HasFlag(KEYMODIFIERS.MOD_ALT))
                result |= ModifierKeys.Alt;
            if (@virtual.HasFlag(KEYMODIFIERS.MOD_CONTROL))
                result |= ModifierKeys.Control;
            if (@virtual.HasFlag(KEYMODIFIERS.MOD_SHIFT))
                result |= ModifierKeys.Shift;
            if (@virtual.HasFlag(KEYMODIFIERS.MOD_WIN))
                result |= ModifierKeys.Windows;

            return result;
        }

        private static KEYMODIFIERS ToVirtual(this ModifierKeys modifiers)
        {
            var result = KEYMODIFIERS.NONE;

            if (modifiers.HasFlag(ModifierKeys.Alt))
                result |= KEYMODIFIERS.MOD_ALT;
            if (modifiers.HasFlag(ModifierKeys.Control))
                result |= KEYMODIFIERS.MOD_CONTROL;
            if (modifiers.HasFlag(ModifierKeys.Shift))
                result |= KEYMODIFIERS.MOD_SHIFT;
            if (modifiers.HasFlag(ModifierKeys.Windows))
                result |= KEYMODIFIERS.MOD_WIN;
            
            return result;
        }

        #endregion

        //======================================================
        #region _Fields_

        private static readonly WinProcHandlerForm _hotKeyForm;

        private static Stopwatch _repeatLimitTimer;


        #endregion
    }
}