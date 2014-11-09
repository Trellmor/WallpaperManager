using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace WallpaperManager.Settings
{
    [Serializable]
    [XmlType("Screen")]
    public class XmlScreen
    {
        private string _Wallpaper = null;
        private string _TempWallpaper = null;

        public XmlScreen()
        {

        }

        public XmlScreen(Screen screen)
        {
            //TODO: device name is not constant
            Bounds = screen.Bounds;
        }

        public void Save(string path)
        {
            if (!String.IsNullOrEmpty(_TempWallpaper))
            {
                DeleteFile(_Wallpaper);
                
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(_TempWallpaper));

                _Wallpaper = path + fileName;

                File.Move(_TempWallpaper, _Wallpaper);
                _TempWallpaper = null;
            }
        }

        public void Delete()
        {
            DeleteFile(_TempWallpaper);
            DeleteFile(_Wallpaper);
        }

        private void DeleteFile(string path)
        {

            if (!String.IsNullOrEmpty(path) && File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void SetWallpaper(string path)
        {
            DeleteFile(_TempWallpaper);

            _TempWallpaper = Path.GetTempPath() + Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(path));
            File.Copy(path, _TempWallpaper);
        }

        [XmlIgnore]
        public bool IsModified
        {
            get
            {
                return !String.IsNullOrEmpty(_TempWallpaper);
            }
        }

        public XmlRectangle Bounds { get; set; }
        public string Wallpaper
        {
            get
            {
                return (!String.IsNullOrEmpty(_TempWallpaper)) ? _TempWallpaper : _Wallpaper;
            }
            set
            {
                _Wallpaper = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is XmlScreen)
            {
                var screen = (XmlScreen)obj;

                return screen.Bounds.Equals(this.Bounds);
            }
            return false;
        }
    }
}
