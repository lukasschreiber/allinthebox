using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace allinthebox
{
    public class Program
    {
        //set variables for USB Authentification
        public static Boolean usb_detected = false;
        static List<USBDeviceInfo> usbDevices = login_form.GetUSBDevices();

        public static loggerView logger;


        [STAThread]
        static void Main()
        {
            //error handling
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //make App DPI aware
            if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //init FileManager
            FileManager.CheckDataStruct();

            //init Log Program
            logger = new loggerView();
            logger.initialize();

            Program.logger.log("System Start", Color.Green);

            Style.init();

            Program.logger.log("Style initialized", Color.Green);

            //set Language

            XmlDocument doc = new XmlDocument();
            doc.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            string culture = doc.SelectSingleNode("/settings/lang").InnerXml;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(culture);
            Program.logger.log("set language to " + culture, Color.Green);

            //read through all USB Devices found and load data
            Program.logger.log("Scan USB Devices");
            foreach (var usbDevice in usbDevices)
            {
                loadUSBData(usbDevice.DeviceID);
            }

            //handle USB response
            if (usb_detected)
            {
                Application.Run(mainForm: new Main());
            }
            else
            {
                Program.logger.log("No USB Key Found", Color.Red);
                Application.Run(mainForm: new login_form());
            }

        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
            logger.error(e.ExceptionObject.ToString());
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();


        //load usbData from file
        private async static void loadUSBData(String pid)
        {
            XDocument doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));
            Program.logger.log("Load User Information", Color.Purple);

            foreach (var user in doc.Descendants("data"))
            {
                foreach (var dm in doc.Descendants("user"))
                {

                    String tempUser = dm.Element("userName").Value;
                    String tempPass = dm.Element("pass").Value;

                    if (pid.Equals(dm.Element("pid").Value))
                    {
                        if (dm.Element("label").Value != utils.usb.getLabel(pid, usbDevices)) {
                            dm.Element("label").Value = utils.usb.getLabel(pid,usbDevices);
                            doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.USER));
                        }
                        usb_detected = true;
                        Program.logger.log("Valid USB Key Found", Color.Green);
                        Program.logger.log("login as " + tempUser, Color.Green);
                        Program.logger.log("Write session File");
                        await WriteSessionAsync(tempUser);
                       
                    }
                }
            }
        }

        private static async Task WriteSessionAsync(string tempUser) {

            await Task.Factory.StartNew(delegate
            {
                XmlDocument document = new XmlDocument();

                document.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));

                String XPathName = "/list/session/name";
                XmlNode node_name = document.SelectSingleNode(XPathName);
                node_name.InnerXml = tempUser;
                document.Save(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));
            });
        }
    }
}
