using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using allinthebox.utils;

namespace allinthebox
{
    public class Program
    {
        //set variables for USB Authentification
        public static bool usb_detected;
        private static readonly List<USBDeviceInfo> usbDevices = login_form.GetUSBDevices();

        public static loggerView logger;


        [STAThread]
        private static void Main()
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

            logger.log("System Start", Color.Green);

            Style.init();

            logger.log("Style initialized", Color.Green);

            //set Language

            var doc = new XmlDocument();
            doc.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            var culture = doc.SelectSingleNode("/settings/lang").InnerXml;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
            logger.log("set language to " + culture, Color.Green);

            //read through all USB Devices found and load data
            logger.log("Scan USB Devices");
            foreach (var usbDevice in usbDevices) loadUSBData(usbDevice.DeviceID);

            //handle USB response
            if (usb_detected)
            {
                Application.Run(new Main());
            }
            else
            {
                logger.log("No USB Key Found", Color.Red);
                Application.Run(new login_form());
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
            logger.error(e.ExceptionObject.ToString());
        }

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();


        //load usbData from file
        private static async void loadUSBData(string pid)
        {
            var doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));
            logger.log("Load User Information", Color.Purple);

            foreach (var user in doc.Descendants("data"))
            foreach (var dm in doc.Descendants("user"))
            {
                var tempUser = dm.Element("userName").Value;
                var tempPass = dm.Element("pass").Value;

                if (pid.Equals(dm.Element("pid").Value))
                {
                    if (dm.Element("label").Value != usb.getLabel(pid, usbDevices))
                    {
                        dm.Element("label").Value = usb.getLabel(pid, usbDevices);
                        doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.USER));
                    }

                    usb_detected = true;
                    logger.log("Valid USB Key Found", Color.Green);
                    logger.log("login as " + tempUser, Color.Green);
                    logger.log("Write session File");
                    await WriteSessionAsync(tempUser);
                }
            }
        }

        private static async Task WriteSessionAsync(string tempUser)
        {
            await Task.Factory.StartNew(delegate
            {
                var document = new XmlDocument();

                document.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));

                var XPathName = "/list/session/name";
                var node_name = document.SelectSingleNode(XPathName);
                node_name.InnerXml = tempUser;
                document.Save(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));
            });
        }
    }
}