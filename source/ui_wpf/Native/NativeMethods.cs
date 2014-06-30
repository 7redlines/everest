using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Se7enRedLines.UI.Native
{
    public static class NativeMethods
    {
        //======================================================
        #region _Unmanaged_

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll", EntryPoint = "GetDoubleClickTime")]
        private static extern uint GetDoubleClickTimeNative();

        #endregion

        //======================================================
        #region _Managed_

        public static IntPtr SetHook(int hookType, HookProc hookProc)
        {
            using (var currentProcess = Process.GetCurrentProcess())
            using (var currentModule = currentProcess.MainModule)
            {
                return SetWindowsHookEx(hookType, hookProc, GetModuleHandle(currentModule.ModuleName), 0);
            }
        }

        public static TimeSpan GetDoubleClickTime()
        {
            return TimeSpan.FromMilliseconds(GetDoubleClickTimeNative());
        }

        #endregion
    }
}