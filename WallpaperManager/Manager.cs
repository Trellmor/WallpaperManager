using System;
using System.IO;
using System.Windows.Forms;
using WallpaperManager.Settings;
using WallpaperManager.WinAPI;

namespace WallpaperManager
{
    class Manager
    {
        public static void ChangeWallpaper()
        {
            XmlSettings settings = XmlSettings.Load();

            XmlProfile current = new XmlProfile(Screen.AllScreens);

            if (settings.Profiles.Contains(current))
            {
                
                current = settings.Profiles[settings.Profiles.IndexOf(current)];
                if (Desktop.SupportsDesktopWallpaper())
                {
                    foreach (XmlScreen screen in current.Screens)
                    {
                        if (!String.IsNullOrEmpty(screen.Wallpaper) && File.Exists(screen.Wallpaper))
                        {
                            Desktop.SetWallpaperUsingDesktopWallpaper(screen.Bounds, screen.Wallpaper);
                        }
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(current.Wallpaper) && File.Exists(current.Wallpaper))
                    {
                        Desktop.SetWallpaperUsingActiveDesktop(current.Wallpaper);
                    }
                }
            }
        }
    }
}
