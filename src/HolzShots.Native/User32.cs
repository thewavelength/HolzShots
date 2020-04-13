using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace HolzShots.Native
{
    public static class User32
    {
        private const string DllName = "user32.dll";

        #region SendMessage

        [DllImport(DllName)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, StringBuilder lParam);

        [DllImport(DllName)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        #endregion
        #region Window Text

        [DllImport(DllName, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int cch);

        [DllImport(DllName, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hwnd);

        #endregion
    }
}