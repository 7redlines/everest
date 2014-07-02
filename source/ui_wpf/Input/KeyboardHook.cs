using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Se7enRedLines.UI.Native;

namespace Se7enRedLines.UI
{
    public static class KeyboardHook
    {
        //======================================================
        #region _Public properties_

        private static event KeyEventHandler _keyUp;
        public static event KeyEventHandler KeyUp
        {
            add
            {
                _keyUp += value;
                EnsureHook();
            }
            remove
            {
                _keyUp -= value;
                TryUnhook();
            }
        }

        private static event KeyEventHandler _keyDown;
        public static event KeyEventHandler KeyDown
        {
            add
            {
                _keyDown += value;
                EnsureHook();
            }
            remove
            {
                _keyDown -= value;
                TryUnhook();
            }
        }

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        private static void EnsureHook()
        {
            if (_keyboardHookHandle == IntPtr.Zero && IsAnythingSubscribed())
            {
                // Create an instance of HookProc.
                _hookDelegate = OnKeyboardProc;
                _keyboardHookHandle = NativeMethods.SetHook(WinHook.WH_KEYBOARD_LL, _hookDelegate);

                //If the SetWindowsHookEx function fails.
                if (_keyboardHookHandle == IntPtr.Zero)
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode, "The hook attachment failed while subscribing to keyboard events.");
                }

                GC.KeepAlive(_hookDelegate);
                GC.KeepAlive(_keyboardHookHandle);
            }
        }

        private static IntPtr OnKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var handled = false;

                var hookData = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

                var keyCode = wParam.ToInt32();
                if (_keyDown != null &&
                    (keyCode == KeyboardInpuNotification.WM_KEYDOWN ||
                     keyCode == KeyboardInpuNotification.WM_SYSKEYDOWN))
                {
                    var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0,
                        KeyInterop.KeyFromVirtualKey(hookData.VirtualKeyCode));
                    RaiseKeyDown(e);
                    handled = e.Handled;
                }

                if (_keyUp != null &&
                    (keyCode == KeyboardInpuNotification.WM_KEYUP ||
                     keyCode == KeyboardInpuNotification.WM_SYSKEYUP))
                {
                    var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0,
                       KeyInterop.KeyFromVirtualKey(hookData.VirtualKeyCode));
                    RaiseKeyUp(e);
                    handled = handled || e.Handled;
                }

                if (handled)
                    return new IntPtr(-1);
            }

            //call next hook
            return NativeMethods.CallNextHookEx(_keyboardHookHandle, nCode, wParam, lParam);
        }

        private static void TryUnhook()
        {
            if (!IsAnythingSubscribed())
            {
                ForceUnhook();
            }
        }

        private static void ForceUnhook()
        {
            if (_keyboardHookHandle == IntPtr.Zero)
                return;

            var result = NativeMethods.UnhookWindowsHookEx(_keyboardHookHandle);
            //reset invalid handle
            _keyboardHookHandle = IntPtr.Zero;
            //Free up for GC
            _hookDelegate = null;

            //if failed and exception must be thrown
            if (!result)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, "The hook attachment during forced unsubscribe failed.");
            }
        }

        private static bool IsAnythingSubscribed()
        {
            var events = new Delegate[]
            {
                _keyDown,
                _keyUp
            };

            return events.Any(e => e != null);
        }

        private static void RaiseKeyDown(KeyEventArgs e)
        {
            Contract.Requires(e != null);

            if (_keyDown != null)
                _keyDown(null, e);
        }

        private static void RaiseKeyUp(KeyEventArgs e)
        {
            Contract.Requires(e != null);

            if (_keyUp != null)
                _keyUp(null, e);
        }

        #endregion

        //======================================================
        #region _Fields_

        private static NativeMethods.HookProc _hookDelegate;
        private static IntPtr _keyboardHookHandle;


        #endregion
    }
}