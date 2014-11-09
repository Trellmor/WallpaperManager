using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Serialization;
using System.Windows;

namespace WallpaperManager.Settings
{
    [Serializable]
    public struct XmlRectangle
    {
        [XmlAttribute("x")]
        public int X { get; set; }
        [XmlAttribute("y")]
        public int Y { get; set; }
        [XmlAttribute("width")]
        public int Width { get; set; }
        [XmlAttribute("height")]
        public int Height { get; set; }

        public XmlRectangle(XmlRectangle rectangle) : this()
        {
            this.X = rectangle.X;
            this.Y = rectangle.Y;
            this.Height = rectangle.Height;
            this.Width = rectangle.Width;
        }

        public XmlRectangle Scale(double factor)
        {
            var rect = new XmlRectangle();
            rect.X = (int)(this.X * factor);
            rect.Y = (int)(this.Y * factor);
            rect.Width = (int)(this.Width * factor);
            rect.Height = (int)(this.Height * factor);
            return rect;
        }

        public XmlRectangle Expand(int by)
        {
            var rect = new XmlRectangle(this);            
            rect.X -= by;
            rect.Y -= by;
            rect.Width += by * 2;
            rect.Height += by * 2;
            return rect;
        }

        public override bool Equals(object obj)
        {
            if (obj is XmlRectangle)
            {
                var rect = (XmlRectangle)obj;
                return rect.X == this.X &&
                    rect.Y == this.Y &&
                    rect.Width == this.Width &&
                    rect.Height == this.Height;
            }
            return false;
        }

        #region Implicit Conversion Operators

        public static implicit operator Rectangle(XmlRectangle xmlRectangle)
        {
            return new Rectangle(xmlRectangle.X, xmlRectangle.Y, xmlRectangle.Width, xmlRectangle.Height);
        }

        public static implicit operator XmlRectangle(Rectangle rectangle)
        {
            return new XmlRectangle() { X = rectangle.X, Y = rectangle.Y, Height = rectangle.Height, Width = rectangle.Width };
        }

        public static implicit operator Rect(XmlRectangle xmlRectangle)
        {
            return new Rect(xmlRectangle.X, xmlRectangle.Y, xmlRectangle.Width, xmlRectangle.Height);
        }

        public static implicit operator XmlRectangle(Rect rect)
        {
            return new XmlRectangle() { X = (int)rect.X, Y = (int)rect.Y, Height = (int)rect.Height, Width = (int)rect.Width };
        }

        #endregion Implicit Conversion Operators
    }
}
