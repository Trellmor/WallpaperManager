using System;
using System.Runtime.InteropServices;

namespace WallpaperManager.WinAPI
{
    //Code from https://pulse.codeplex.com/
    class WinAPI
    {
        #region user32
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr result);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        #endregion
    }
}
