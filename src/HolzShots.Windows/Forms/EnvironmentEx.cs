using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace HolzShots.Windows.Forms
{
    public static class EnvironmentEx
    {
        public static bool IsVistaOrHigher => Environment.OSVersion.Version.Major >= 6;
        public static bool IsSevenOrHigher => (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1) || Environment.OSVersion.Version.Major > 6;
        public static bool IsTenOrHigher => Environment.OSVersion.Version.Major >= 10;

        public static bool AppsUseLightTheme()
        {
            if (!IsTenOrHigher)
                return false;

            var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            return (int)key.GetValue("AppsUseLightTheme", 1) != 0;
        }

        /// <summary> It's a function instead of a property to singal that this call might be expensive (it involves a p/invoke) </summary>
        public static bool IsAeroEnabled() => IsVistaOrHigher && Native.DwmApi.DwmIsCompositionEnabled();

        public static bool IsFullscreenAppRunning()
        {
            var foregroundWindowHandle = Native.User32.GetForegroundWindow();

            var sb = new System.Text.StringBuilder();
            var charsWritten = Native.User32.GetClassName(foregroundWindowHandle, sb, sb.Capacity);
            if (charsWritten == 0)
                return false; // Call to GetClassName failed. Just assume that there is no app running in fullscreen

            var className = sb.ToString();
            if (className == "WorkerW")
                return false;

            if (!Native.User32.GetWindowRect(foregroundWindowHandle, out var fgWindowRect))
                return false; // Call to GetWindowRect failed. Just assume that there is no app running in fullscreen

            // This is pretty unreliable. But it works at least a little bit.
            // It doesn't work if the application is running full screen on a different monitor (or over all monitors).
            return Screen.PrimaryScreen.Bounds.Height == fgWindowRect.Bottom
                && Screen.PrimaryScreen.Bounds.Width == fgWindowRect.Right;
        }
    }
}
