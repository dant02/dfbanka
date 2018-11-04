using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace dfbanka.gui.api
{
    internal class Files
    {
        public static class Paths
        {
            public static readonly string DataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"dfbanka");
            public static readonly string ConfigXml = null;
            public static readonly string OrdersJson = null;
            public static readonly string BankXml = null;

            static Paths()
            {
                ConfigXml = Path.Combine(DataDir, @"config.xml");
                OrdersJson = Path.Combine(DataDir, @"orders.json");
                BankXml = Path.Combine(DataDir, @"bank.xml");
            }
        }

        public static string Load(string fileName)
        {
            if (File.Exists(fileName))
                return File.ReadAllText(fileName);

            return null;
        }

        public static T Load<T>(string fileName)
        {
            var str = Load(fileName);

            if (!string.IsNullOrWhiteSpace(str))
            {
                var xml = new XmlSerializer(typeof(T));

                using (var fe = new StringReader(str))
                    return (T)xml.Deserialize(fe);
            }

            return default(T);
        }

        public static void Save(string fileName, string data)
        {
            Save(Paths.DataDir, fileName, data);
        }

        public static void Save(string directory, string fileName, string data)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(fileName, data, Encoding.UTF8);
        }

        public static void Save<T>(string directory, string fileName, T data)
        {
            var xml = new XmlSerializer(typeof(T));

            using (var sw = new StringWriter())
            {
                xml.Serialize(sw, data);
                Save(directory, fileName, sw.ToString());
            }
        }

        public static void Save<T>(string fileName, T data)
        {
            Save(Paths.DataDir, fileName, data);
        }
    }
}