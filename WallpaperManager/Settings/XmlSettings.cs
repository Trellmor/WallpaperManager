using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WallpaperManager.Settings
{
    
    [Serializable]
    [XmlRoot("Settings")]
    public class XmlSettings
    {
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(XmlSettings));

        public XmlSettings()
        {
            Profiles = new List<XmlProfile>();
        }

        public static string GetPath()
        {
            string name = AppDomain.CurrentDomain.FriendlyName;
            name = name.Substring(0, name.IndexOf('.'));
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + name + "\\";
        }

        public void Save()
        {
            string path = GetPath() + "settings.xml"; ;
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            foreach (XmlProfile profile in Profiles) {
                profile.Save();
            }

            using (TextWriter writer = File.CreateText(path))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static XmlSettings Load()
        {
            string path = GetPath() + "settings.xml";

            if (File.Exists(path))
            {
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    return (XmlSettings)serializer.Deserialize(stream);
                }
            }
            else
            {
                return new XmlSettings();
            }
        }

        public List<XmlProfile> Profiles { get; set; }
    }
}
