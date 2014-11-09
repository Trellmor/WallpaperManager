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
                if (!String.IsNullOrEmpty(current.Wallpaper) && File.Exists(current.Wallpaper))
                {
                    Desktop.SetWallpaperUsingActiveDesktop(current.Wallpaper);
                }
            }
        }
    }
}
