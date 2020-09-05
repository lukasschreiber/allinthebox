using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace allinthebox
{
    public class FileManager
    {
        private const string root = "All in the Box";

        public static string GetDataBasePath(string file)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + root + "\\" + file;
        }

        public static string GetDataBasePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + root;
        }

        public static string GetBaseDirectoyPath(string file)
        {
            return AppDomain.CurrentDomain.BaseDirectory + file;
        }

        public static string GetBaseDirectoyPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static async Task WriteXMLFile(XmlDocument xmlDocument, string name)
        {
            await Task.Factory.StartNew(() => { xmlDocument.Save(GetDataBasePath(name)); });
        }

        public static async Task WriteXMLFile(XDocument xmlDocument, string name)
        {
            await Task.Factory.StartNew(() => { xmlDocument.Save(GetDataBasePath(name)); });
        }

        public static async Task WriteXMLFile(string root, string name)
        {
            await Task.Factory.StartNew(() =>
            {
                var xmlDocument = new XDocument();
                xmlDocument.Add(new XElement(root, "\n"));
                xmlDocument.Save(GetDataBasePath(name));
            });
        }


        public static async void CheckDataStruct()
        {
            if (!Directory.Exists(GetDataBasePath())) Directory.CreateDirectory(GetDataBasePath());

            if (!Directory.Exists(GetDataBasePath("Backup"))) Directory.CreateDirectory(GetDataBasePath("Backup"));

            if (!Directory.Exists(GetDataBasePath("Comments"))) Directory.CreateDirectory(GetDataBasePath("Comments"));

            if (!Directory.Exists(GetDataBasePath("Files"))) Directory.CreateDirectory(GetDataBasePath("Files"));

            if (!Directory.Exists(GetDataBasePath("Images"))) Directory.CreateDirectory(GetDataBasePath("Images"));

            if (!Directory.Exists(GetDataBasePath("Presets"))) Directory.CreateDirectory(GetDataBasePath("Presets"));

            if (!Directory.Exists(GetDataBasePath("Styles"))) Directory.CreateDirectory(GetDataBasePath("Styles"));

            if (!File.Exists(GetDataBasePath(NAMES.DATA))) await WriteXMLFile("data", NAMES.DATA);

            if (!File.Exists(GetDataBasePath(NAMES.ERROR))) await WriteXMLFile("log", NAMES.ERROR);

            if (!File.Exists(GetDataBasePath(NAMES.LOG))) await WriteXMLFile("log", NAMES.LOG);

            if (!File.Exists(GetDataBasePath(NAMES.RACKS))) await WriteXMLFile("list", NAMES.RACKS);

            if (!File.Exists(GetDataBasePath(NAMES.SESSION)))
            {
                var session = new XDocument();
                var root = new XElement("list");

                var s = new XElement("session");
                s.Add(new XElement("name", "admin"));
                s.Add(new XElement("preset", "raum_4.png"));
                root.Add(s);
                session.Add(root);
                await WriteXMLFile(session, NAMES.SESSION);
            }

            if (!File.Exists(GetDataBasePath(NAMES.USER)))
            {
                var user = new XDocument();
                var root = new XElement("user");
                root.Add(new XElement("userName", "admin"));
                root.Add(new XElement("pass", Main.GetHashString("admin")));
                root.Add(new XElement("pid", ""));
                root.Add(new XElement("label", ""));
                root.Add(new XElement("admin", "true"));
                var data = new XElement("data");
                data.Add(root);
                user.Add(data);

                await WriteXMLFile(user, NAMES.USER);
            }

            if (!File.Exists(GetDataBasePath(NAMES.SETTINGS)))
                File.Copy(GetBaseDirectoyPath("Presets\\Settings.xml"), GetDataBasePath(NAMES.SETTINGS));

            if (!File.Exists(GetDataBasePath("Styles\\StyleLight.xml")))
                File.Copy(GetBaseDirectoyPath("Presets\\StyleLight.xml"), GetDataBasePath("Styles\\StyleLight.xml"));

            if (!File.Exists(GetDataBasePath("Styles\\StyleDark.xml")))
                File.Copy(GetBaseDirectoyPath("Presets\\StyleDark.xml"), GetDataBasePath("Styles\\StyleDark.xml"));


            //Presets to Document Folder
            //copy csv and Documentation
        }

        //Filenames
        public static class NAMES
        {
            public const string DATA = "Data.xml";
            public const string ERROR = "Error.xml";
            public const string LOG = "Log.xml";
            public const string RACKS = "Racks.xml";
            public const string SESSION = "Session.xml";
            public const string SETTINGS = "Settings.xml";
            public const string USER = "User.xml";
        }
    }
}