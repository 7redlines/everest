﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Se7enRedLines.UI.Native;

namespace Se7enRedLines.UI
{
    public static class MouseHook
    {
        //======================================================
        #region _Public properties_

        public static event MouseButtonEventHandler _mouseDown;
        public static event MouseButtonEventHandler MouseDown
        {
            add
            {
                _mouseDown += value;
                EnsureHook();
            }
            remove
            {
                _mouseDown -= value;
                TryUnhook();
            }
        }

        public static event MouseEventHandler _mouseMove;
        public static event MouseEventHandler MouseMove
        {
            add
            {
                _mouseMove += value;
                EnsureHook();
            }
            remove
            {
                _mouseMove -= value;
                TryUnhook();
            }
        }

        public static event MouseButtonEventHandler _mouseUp;
        public static event MouseButtonEventHandler MouseUp
        {
            add
            {
                _mouseUp += value;
                EnsureHook();
            }
            remove
            {
                _mouseUp -= value;
                TryUnhook();
            }
        }

        public static event MouseButtonEventHandler _mouseClick;
        public static event MouseButtonEventHandler MouseClick
        {
            add
            {
                _mouseClick += value;
                EnsureHook();
            }
            remove
            {
                _mouseClick -= value;
                TryUnhook();
            }
        }

        private static event MouseButtonEventHandler _mouseDoubleClick;
        public static event MouseButtonEventHandler MouseDoubleClick
        {
            add
            {
                _mouseDoubleClick += value;
                EnsureHook();
            }
            remove
            {
                _mouseDoubleClick -= value;
                TryUnhook();
            }
        }

        private static event MouseWheelEventHandler _mouseWheel;
        public static event MouseWheelEventHandler MouseWheel
        {
            add
            {
                _mouseWheel += value;
                EnsureHook();
            }
            remove
            {
                _mouseWheel -= value;
                TryUnhook();
            }
        }

        #endregion

        //======================================================
        #region _Private, protected, internal methods_

        private static void EnsureHook()
        {
            if (_mouseHookHandle == IntPtr.Zero && IsAnythingSubscribed())
            {
                // Create an instance of HookProc.
                _hookDelegate = OnMouseProc;
                _mouseHookHandle = NativeMethods.SetHook(WinHook.WH_MOUSE_LL, _hookDelegate);

                //If the SetWindowsHookEx function fails.
                if (_mouseHookHandle == IntPtr.Zero)
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode, "The hook attachment during forced unsubscribe failed.");
                }

                GC.KeepAlive(_hookDelegate);
                GC.KeepAlive(_mouseHookHandle);
            }
        }

        private static IntPtr OnMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                //Marshall the data from callback.
                var hookData = (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof (MSLLHOOKSTRUCT));

                //detect button clicked
                MouseButton? button = null;
                MouseButtonState? buttonState = null;
                var clickCount = 0;

                switch (wParam.ToInt32())
                {
                    case MouseInputNotification.WM_LBUTTONDOWN:
                        buttonState = MouseButtonState.Pressed;
                        button = MouseButton.Left;
                        clickCount = 1;
                        break;
                    case MouseInputNotification.WM_LBUTTONUP:
                        buttonState = MouseButtonState.Released;
                        button = MouseButton.Left;
                        clickCount = 1;
                        break;
                    case MouseInputNotification.WM_RBUTTONDOWN:
                        buttonState = MouseButtonState.Pressed;
                        button = MouseButton.Right;
                        clickCount = 1;
                        break;
                    case MouseInputNotification.WM_RBUTTONUP:
                        buttonState = MouseButtonState.Released;
                        button = MouseButton.Right;
                        clickCount = 1;
                        break;
                    case MouseInputNotification.WM_LBUTTONDBLCLK:
                        buttonState = MouseButtonState.Released;
                        button = MouseButton.Left;
                        clickCount = 2;
                        break;
                    case MouseInputNotification.WM_RBUTTONDBLCLK:
                        buttonState = MouseButtonState.Released;
                        button = MouseButton.Right;
                        clickCount = 2;
                        break;
                    case MouseInputNotification.WM_MOUSEMOVE:
                        break;
                    case MouseInputNotification.WM_MOUSEWHEEL:
                        break;
                    default:
                        break;
                }

                if (button.HasValue && buttonState.HasValue)
                {
                    var e = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, button.Value);

                    switch (wParam.ToInt32())
                    {
                        case MouseInputNotification.WM_LBUTTONDOWN:
                            RaiseMouseDown(e);
                            break;
                        case MouseInputNotification.WM_LBUTTONUP:
                            RaiseMouseUp(e);
                            break;
                        case MouseInputNotification.WM_RBUTTONDOWN:
                            RaiseMouseDown(e);
                            break;
                        case MouseInputNotification.WM_RBUTTONUP:
                            RaiseMouseUp(e);
                            break;
                    }

                    if (e.Handled)
                        return new IntPtr(-1);

                    if (buttonState.Value == MouseButtonState.Released)
                    {
                        if (clickCount > 0)
                        {
                            RaiseMouseClick(e);
                        }

                        if (clickCount == 2)
                        {
                            RaiseMouseDoubleClick(e);
                        }
                    }

                    if (e.Handled)
                        return new IntPtr(-1);
                }

                if (wParam.ToInt32() == MouseInputNotification.WM_MOUSEMOVE)
                {
                    var e = new MouseEventArgs(Mouse.PrimaryDevice, 0);
                    RaiseMouseMove(e);

                    if (e.Handled)
                        return new IntPtr(-1);
                }

                if (wParam.ToInt32() == MouseInputNotification.WM_MOUSEWHEEL)
                {
                    var loword = BitConverter.ToInt16(BitConverter.GetBytes(hookData.MouseData), 0); ;
                    var hiword = BitConverter.ToInt16(BitConverter.GetBytes(hookData.MouseData), 2); ;

                    var e = new MouseWheelEventArgs(Mouse.PrimaryDevice, hookData.Time, loword < 0 ? -hiword : hiword);
                    RaiseMouseWheel(e);

                    if (e.Handled)
                        return new IntPtr(-1);
                }
            }

            //call next hook
            return NativeMethods.CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
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
            if (_mouseHookHandle != IntPtr.Zero)
            {
                var result = NativeMethods.UnhookWindowsHookEx(_mouseHookHandle);
                //reset invalid handle
                _mouseHookHandle = IntPtr.Zero;
                //Free up for GC
                _hookDelegate = null;

                //if failed and exception must be thrown
                if (!result)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode, "The hook attachment during forced unsubscribe failed.");
                }
            }
        }

        private static bool IsAnythingSubscribed()
        {
            var events = new Delegate[]
            {
                _mouseDown,
                _mouseMove,
                _mouseUp,
                _mouseClick,
                _mouseDoubleClick,
                _mouseWheel
            };

            return events.Any(e => e != null);
        }

        private static void RaiseMouseDown(MouseButtonEventArgs e)
        {
            Contract.Requires(e != null);

            e.RoutedEvent = Mouse.MouseDownEvent;

            try
            {
                if (_mouseDown != null)
                    _mouseDown(null, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        private static void RaiseMouseMove(MouseEventArgs e)
        {
            Contract.Requires(e != null);

            e.RoutedEvent = Mouse.MouseMoveEvent;

            try
            {
                if (_mouseMove != null)
                    _mouseMove(null, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        private static void RaiseMouseWheel(MouseWheelEventArgs e)
        {
            Contract.Requires(e != null);

            e.RoutedEvent = Mouse.MouseWheelEvent;

            try
            {
                if (_mouseWheel != null)
                    _mouseWheel(null, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        private static void RaiseMouseUp(MouseButtonEventArgs e)
        {
            Contract.Requires(e != null);

            e.RoutedEvent = Mouse.MouseUpEvent;

            try
            {
                if (_mouseUp != null)
                    _mouseUp(null, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        private static void RaiseMouseClick(MouseButtonEventArgs e)
        {
            Contract.Requires(e != null);

            e.RoutedEvent = Mouse.MouseUpEvent;

            try
            {
                if (_mouseClick != null)
                    _mouseClick(null, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        private static void RaiseMouseDoubleClick(MouseButtonEventArgs e)
        {
            Contract.Requires(e != null);

            e.RoutedEvent = Mouse.MouseUpEvent;

            try
            {
                if (_mouseDoubleClick != null)
                    _mouseDoubleClick(null, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        #endregion

        //======================================================
        #region _Fields_

        private static NativeMethods.HookProc _hookDelegate;
        private static IntPtr _mouseHookHandle;

        #endregion
    }
}