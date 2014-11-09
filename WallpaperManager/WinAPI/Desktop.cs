using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace WallpaperManager.WinAPI
{
    //Code from https://pulse.codeplex.com/
    public class Desktop
    {

        public static void EnableActiveDesktop()
        {
            IntPtr result = IntPtr.Zero;
            WinAPI.SendMessageTimeout(WinAPI.FindWindow("Progman", null), 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 500, out result);
        }

        public static void SetWallpaperUsingActiveDesktop(string path)
        {
            EnableActiveDesktop();

            ThreadStart threadStarter = () =>
            {
                WinAPI.IActiveDesktop _activeDesktop = WinAPI.ActiveDesktopWrapper.GetActiveDesktop();
                _activeDesktop.SetWallpaper(path, 0);
                _activeDesktop.ApplyChanges(WinAPI.AD_Apply.ALL | WinAPI.AD_Apply.FORCE);

                Marshal.ReleaseComObject(_activeDesktop);
            };
            Thread thread = new Thread(threadStarter);
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA (REQUIRED!!!!)
            thread.Start();
            thread.Join(2000);

        }
    }
}
