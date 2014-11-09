using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperManager
{
    class WallpaperManagerException : Exception
    {
        public WallpaperManagerException(string message)
            : base(message)
        {
        }

        public WallpaperManagerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    class ImageLoadException : WallpaperManagerException
    {
        public ImageLoadException(string message)
            : base(message)
        {
        }

        public ImageLoadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
