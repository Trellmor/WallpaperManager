using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WallpaperManager.Settings;
using WallpaperManager.WinAPI;

namespace WallpaperManager
{
    class SysTrayApp : Form
    {
        private ContextMenu trayMenu;
        private NotifyIcon trayIcon;
        private FormWallpaperManager formManager;

        public SysTrayApp()
        {
            trayMenu = new ContextMenu();

            MenuItem item = trayMenu.MenuItems.Add("Settings", DoShowManager);
            item.DefaultItem = true;

            trayMenu.MenuItems.Add("Refresh Wallpaper", DoRefreshWallpaper);

            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Exit", DoExit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "WallpaperManager";
            trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
            trayIcon.DoubleClick += new EventHandler(DoShowManager);

            SystemEvents.DisplaySettingsChanged += new EventHandler(DisplaySettingsChanged);
        }

        private void DoShowManager(object sender, EventArgs e)
        {
            if (formManager == null)
            {
                formManager = new FormWallpaperManager();
                formManager.FormClosed += new FormClosedEventHandler(DoManagerClosed);
                formManager.Show();
            }
            else
            {
                formManager.BringToFront();
            }
        }

        private void DoManagerClosed(object sender, FormClosedEventArgs e) {
            formManager = null;
        }

        private void DoRefreshWallpaper(object sender, EventArgs e)
        {
            Manager.ChangeWallpaper();
        }

        private void DoExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;

            Manager.ChangeWallpaper();

            base.OnLoad(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SystemEvents.DisplaySettingsChanged -= new EventHandler(DisplaySettingsChanged);
                trayIcon.Dispose();
            }

            base.Dispose(disposing);
        }

        private void DisplaySettingsChanged(object sender, EventArgs e)
        {
            Manager.ChangeWallpaper();
        }
    }
}
