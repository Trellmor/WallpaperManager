using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using ImageMagick;
using System.Windows.Forms;

namespace WallpaperManager.Settings
{
    [Serializable]
    [XmlType("Profile")]
    public class XmlProfile
    {
        public XmlProfile()
        {
            Screens = new List<XmlScreen>();
            Folder = Path.GetRandomFileName();
        }

        public XmlProfile(Screen[] screens) : this()
        {
            foreach (Screen screen in screens)
            {
                Screens.Add(new XmlScreen(screen));
            }
        }

        private string GetPath()
        {
            return XmlSettings.GetPath() + Folder + "\\";
        }

        public void Save()
        {
            bool modified = false;
            foreach (XmlScreen screen in Screens)
            {
                modified |= screen.IsModified;
                screen.Save(GetPath());
            }

            if (modified)
            {
                //Generate new Wallpaper image
                Rect rect = new Rect();
                foreach (XmlScreen screen in Screens)
                {
                    rect = Rect.Union(rect, screen.Bounds);
                }

                
                int xOffset = (int)rect.Left * -1;
                int yOffset = (int)rect.Top * -1;

                //New image
                using (MagickImageCollection images = new MagickImageCollection())
                {
                    MagickImage background = new MagickImage(new MagickColor(0, 0, 0), (int)rect.Width, (int)rect.Height);
                    images.Add(background);

                    foreach (XmlScreen screen in Screens)
                    {
                        if (!String.IsNullOrEmpty(screen.Wallpaper) && File.Exists(screen.Wallpaper))
                        {
                            MagickImage image = new MagickImage(screen.Wallpaper);
                            image.Resize(screen.Bounds.Width, screen.Bounds.Height);
                            image.Extent(screen.Bounds.Width, screen.Bounds.Height, Gravity.Center, MagickColor.Transparent);
                            image.Page = new MagickGeometry(screen.Bounds.X + xOffset, screen.Bounds.Y + yOffset, screen.Bounds.Width, screen.Bounds.Height);
                            images.Add(image);
                        }
                    }

                    if (!String.IsNullOrEmpty(Wallpaper) && File.Exists(Wallpaper)) {
                        File.Delete(Wallpaper);
                    }
                    Wallpaper = GetPath() + Path.ChangeExtension(Path.GetRandomFileName(), ".png");

                    using (MagickImage result = images.Merge())
                    {
                        result.Write(Wallpaper);
                    }
                }
            }
        }

        public void Delete()
        {
            foreach (XmlScreen screen in Screens)
            {
                screen.Delete();
            }

            Directory.Delete(GetPath(), true);
        }

        [XmlAttribute]
        public string Name { get; set; }
        public List<XmlScreen> Screens { get; set; }
        public string Folder { get; set; }
        public string Wallpaper { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is XmlProfile)
            {
                var profile = (XmlProfile)obj;

                if (profile.Screens.Count != this.Screens.Count) return false;

                foreach (var screen in this.Screens)
                {
                    if (!profile.Screens.Contains(screen))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
