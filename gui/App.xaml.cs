using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace dfbanka.gui
{
    public partial class App : Application
    {
        public readonly string DataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"dfbanka");
        public readonly string FileName = null;

        public App() : base()
        {
            this.FileName = Path.Combine(this.DataDir, @"config.xml");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MyWindow.Instance.Show();
        }

        public Configuration LoadConfiguration()
        {
            Configuration result = null;

            var xml = new XmlSerializer(typeof(Configuration));

            if (File.Exists(this.FileName))
            {
                var str = File.ReadAllText(this.FileName);
                using (var fe = new StringReader(str))
                    result = (Configuration)xml.Deserialize(fe);
            }

            return result;
        }

        public void SaveConfiguration(Configuration configration)
        {
            XmlSerializer xml = new XmlSerializer(typeof(Configuration));

            if (!Directory.Exists(this.DataDir))
                Directory.CreateDirectory(this.DataDir);

            using (var sw = new StringWriter())
            {
                xml.Serialize(sw, configration);
                File.WriteAllText(this.FileName, sw.ToString(), Encoding.UTF8);
            }
        }
    }
}