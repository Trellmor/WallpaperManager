using System;
using System.Drawing;
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
                ActiveDesktop.IActiveDesktop _activeDesktop = ActiveDesktop.ActiveDesktopWrapper.GetActiveDesktop();
                _activeDesktop.SetWallpaper(path, 0);
                _activeDesktop.ApplyChanges(ActiveDesktop.AD_Apply.ALL | ActiveDesktop.AD_Apply.FORCE);

                Marshal.ReleaseComObject(_activeDesktop);
            };
            Thread thread = new Thread(threadStarter);
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA (REQUIRED!!!!)
            thread.Start();
            thread.Join(2000);
        }

        public static void SetWallpaperUsingDesktopWallpaper(Rectangle rect, string path)
        {
            DesktopWallpaper.IDesktopWallpaper _desktopWallpaper = DesktopWallpaper.DesktopWallpaperWrapper.GetDesktopWallpaper();
            for (uint i = 0; i < _desktopWallpaper.GetMonitorDevicePathCount(); i++)
            {
                try
                {
                    string monitorId = _desktopWallpaper.GetMonitorDevicePathAt(i);
                    DesktopWallpaper.Rect monitorRect = _desktopWallpaper.GetMonitorRECT(monitorId);
                    if (monitorRect.Left == rect.Left && monitorRect.Top == rect.Top && monitorRect.Right == rect.Right && monitorRect.Bottom == rect.Bottom)
                    {
                        _desktopWallpaper.SetWallpaper(monitorId, path);
                        break;
                    }
                }
                catch (COMException e)
                {
                    System.Console.WriteLine(e.ErrorCode);
                }
            }
            Marshal.ReleaseComObject(_desktopWallpaper);
        }

        public static bool SupportsDesktopWallpaper()
        {
            try
            {
                DesktopWallpaper.IDesktopWallpaper _desktopWallpaper = DesktopWallpaper.DesktopWallpaperWrapper.GetDesktopWallpaper();
                Marshal.ReleaseComObject(_desktopWallpaper);
                return true;
            }
            catch (COMException e)
            {
                if ((uint)e.ErrorCode == 0x80040154)
                {
                    return false;
                }
                else
                {
                    throw e;
                }
            }
        }
    }
}
