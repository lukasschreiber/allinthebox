using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace allinthebox
{
    class Style
    {
        static XmlDocument styleSet;
        static string currentStyle;

        public static void init() {
            styleSet = new XmlDocument();
            styleSet.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            currentStyle = styleSet.SelectSingleNode("/settings/style").InnerXml;
            styleSet.Load(FileManager.GetDataBasePath("Styles//" + currentStyle + ".xml"));
            iconStyle = int.Parse(styleSet.SelectSingleNode("/" + currentStyle + "/iconStyle").InnerXml) == 0 ? IconStyle.DARK : IconStyle.LIGHT;
        }

        public enum IconStyle { LIGHT, DARK };
        public static IconStyle iconStyle;

        public static Color get(string name) {
            ColorConverter cc = new ColorConverter();
            Console.WriteLine("/" + currentStyle + "/" + name);
            try
            {
                return (Color)cc.ConvertFromString(styleSet.SelectSingleNode("/" + currentStyle + "/" + name).InnerXml);
            }
            catch (Exception e) {
                return Color.Silver;
            }
        }
    }
}
