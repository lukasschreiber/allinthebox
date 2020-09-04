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
            return System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + root + "\\" + file;
        }

        public static string GetDataBasePath()
        {
            return System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + root;
        }

        public static string GetBaseDirectoyPath(string file)
        {
            return System.AppDomain.CurrentDomain.BaseDirectory + file;
        }

        public static string GetBaseDirectoyPath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }

        public async static Task WriteXMLFile(XmlDocument xmlDocument, string name)
        {
            await Task.Factory.StartNew(() =>
            {
                xmlDocument.Save(GetDataBasePath(name));
            });
        }

        public async static Task WriteXMLFile(XDocument xmlDocument, string name)
        {
            await Task.Factory.StartNew(() =>
            {
                xmlDocument.Save(GetDataBasePath(name));
            });
        }

        public async static Task WriteXMLFile(string root, string name)
        {
            await Task.Factory.StartNew(() =>
            {
                XDocument xmlDocument = new XDocument();
                xmlDocument.Add(new XElement(root, "\n"));
                xmlDocument.Save(GetDataBasePath(name));
            });
        }


        public async static void CheckDataStruct()
        {
            if (!Directory.Exists(GetDataBasePath()))
            {
                Directory.CreateDirectory(GetDataBasePath());
            }

            if (!Directory.Exists(GetDataBasePath("Backup")))
            {
                Directory.CreateDirectory(GetDataBasePath("Backup"));
            }

            if (!Directory.Exists(GetDataBasePath("Comments")))
            {
                Directory.CreateDirectory(GetDataBasePath("Comments"));
            }

            if (!Directory.Exists(GetDataBasePath("Files")))
            {
                Directory.CreateDirectory(GetDataBasePath("Files"));
            }

            if (!Directory.Exists(GetDataBasePath("Images")))
            {
                Directory.CreateDirectory(GetDataBasePath("Images"));
            }

            if (!Directory.Exists(GetDataBasePath("Presets")))
            {
                Directory.CreateDirectory(GetDataBasePath("Presets"));
            }

            if (!Directory.Exists(GetDataBasePath("Styles")))
            {
                Directory.CreateDirectory(GetDataBasePath("Styles"));
            }

            if (!File.Exists(GetDataBasePath(FileManager.NAMES.DATA)))
            {
                await WriteXMLFile("data", FileManager.NAMES.DATA);
            }

            if (!File.Exists(GetDataBasePath(FileManager.NAMES.ERROR)))
            {
                await WriteXMLFile("log", FileManager.NAMES.ERROR);
            }

            if (!File.Exists(GetDataBasePath(FileManager.NAMES.LOG)))
            {
                await WriteXMLFile("log", FileManager.NAMES.LOG);
            }

            if (!File.Exists(GetDataBasePath(FileManager.NAMES.RACKS)))
            {
                await WriteXMLFile("list", FileManager.NAMES.RACKS);
            }

            if (!File.Exists(GetDataBasePath(FileManager.NAMES.SESSION)))
            {
                XDocument session = new XDocument();
                XElement root = new XElement("list");

                XElement s = new XElement("session");
                s.Add(new XElement("name", "admin"));
                s.Add(new XElement("preset", "raum_4.png"));
                root.Add(s);
                session.Add(root);
                await WriteXMLFile(session, FileManager.NAMES.SESSION);
            }

            if (!File.Exists(GetDataBasePath(FileManager.NAMES.USER)))
            {
                XDocument user = new XDocument();
                XElement root = new XElement("user");
                root.Add(new XElement("userName", "admin"));
                root.Add(new XElement("pass", Main.GetHashString("admin")));
                root.Add(new XElement("pid", ""));
                root.Add(new XElement("label", ""));
                root.Add(new XElement("admin", "true"));
                XElement data = new XElement("data");
                data.Add(root);
                user.Add(data);

                await WriteXMLFile(user, FileManager.NAMES.USER);
            }

            if (!File.Exists(GetDataBasePath(FileManager.NAMES.SETTINGS)))
            {
                File.Copy(GetBaseDirectoyPath("Presets\\Settings.xml"), GetDataBasePath(FileManager.NAMES.SETTINGS));
            }

            if (!File.Exists(GetDataBasePath("Styles\\StyleLight.xml")))
            {
                File.Copy(GetBaseDirectoyPath("Presets\\StyleLight.xml"), GetDataBasePath("Styles\\StyleLight.xml"));
            }

            if (!File.Exists(GetDataBasePath("Styles\\StyleDark.xml")))
            {
                File.Copy(GetBaseDirectoyPath("Presets\\StyleDark.xml"), GetDataBasePath("Styles\\StyleDark.xml"));
            }


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
