using System;
using System.Drawing;
using System.Xml;

namespace allinthebox
{
    internal class Style
    {
        public enum IconStyle
        {
            LIGHT,
            DARK
        }

        private static XmlDocument styleSet;
        private static string currentStyle;
        public static IconStyle iconStyle;

        public static void init()
        {
            styleSet = new XmlDocument();
            styleSet.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            currentStyle = styleSet.SelectSingleNode("/settings/style").InnerXml;
            styleSet.Load(FileManager.GetDataBasePath("Styles//" + currentStyle + ".xml"));
            iconStyle = int.Parse(styleSet.SelectSingleNode("/" + currentStyle + "/iconStyle").InnerXml) == 0
                ? IconStyle.DARK
                : IconStyle.LIGHT;
        }

        public static Color get(string name)
        {
            var cc = new ColorConverter();
            Console.WriteLine("/" + currentStyle + "/" + name);
            try
            {
                return (Color) cc.ConvertFromString(styleSet.SelectSingleNode("/" + currentStyle + "/" + name)
                    .InnerXml);
            }
            catch (Exception e)
            {
                return Color.Silver;
            }
        }
    }
}