using System;
using System.Collections.Generic;
using System.Drawing;
using System.Management;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using allinthebox.Properties;

namespace allinthebox
{
    public partial class login_form : Form
    {
        public const int WM_KEYDOWN = 0x0100;
        public string currentStyle;
        private int grabX, grabY;
        public bool loginOK;
        public Main main;
        private bool mousedown, maximized;

        private int mouseX, mouseY;

        public string shouldPass;
        public string tempUser;
        public bool usb_detected;
        public ManagementEventWatcher watcher = new ManagementEventWatcher();

        public login_form()
        {
            //setup window, center
            InitializeComponent();

            //language
            label1.Text = strings.username + ":";
            label2.Text = strings.password + ":";
            label3.Text = strings.useUSB;
            exit.ButtonText = strings.exit;
            open.ButtonText = strings.login;

            //version
            /*System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;*/
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionLabel.Text = "v" + version;

            CenterToScreen();
            errorText.Text = "";
            main = new Main();

            //load style
            currentStyle = main.loadSettingsDataBase().SelectSingleNode("/settings/style").InnerXml;

            var cc = new ColorConverter();

            #region style

            //load colors and images

            if (Style.iconStyle == Style.IconStyle.DARK)
            {
                bunifuImageButton1.Image = Resources.eye_white;
                closeButton.Image = Resources.close_white;
            }
            else
            {
                bunifuImageButton1.Image = Resources.eye_grey;
                closeButton.Image = Resources.close;
            }

            BackColor = Style.get("login/backColor");
            password.BackColor = userName.BackColor = BackColor;
            exit.BackColor = BackColor;
            open.BackColor = BackColor;
            exit.IdleLineColor = open.IdleLineColor = Style.get("login/buttonBorder");
            exit.ActiveForecolor = open.ActiveForecolor = Style.get("login/activeText");
            exit.IdleForecolor = open.IdleForecolor = Style.get("login/buttonText");
            exit.IdleFillColor = open.IdleFillColor = Style.get("login/buttonBack");
            password.LineFocusedColor = userName.LineFocusedColor = Style.get("login/lineActiveColor");
            exit.ActiveFillColor = open.ActiveFillColor = Style.get("login/buttonActiveColor");

            open.Refresh();
            exit.Refresh();

            userName.ForeColor = userName.HintForeColor = Style.get("login/textColorInput");
            password.ForeColor = password.HintForeColor = Style.get("login/textColorInput");
            password.LineIdleColor = password.LineMouseHoverColor =
                userName.LineMouseHoverColor = userName.LineIdleColor = Style.get("login/lineColor");
            userName.Refresh();
            password.Refresh();

            label1.ForeColor = label2.ForeColor = label3.ForeColor = Style.get("login/textColor");
            label1.Refresh();
            label2.Refresh();
            label3.Refresh();

            #endregion
        }

        //create frame shadow fpr better contrast
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                var cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        //check if credentials are correct
        public void checkLogIn()
        {
            //ignore case
            var user = userName.Text.ToLower();

            //encrypt with MD5
            var pass = Main.GetHashString(password.Text);

            loadUserData();


            if (user != "" && pass != "")
            {
                if (loginOK)
                    open_main();
                else
                    errorText.Text = strings.errorCredentialsWrong;
            }
            else
            {
                errorText.Text = strings.errorEmpty;
            }
        }

        public string getUserName()
        {
            return userName.Text;
        }

        //check if USB is inserted
        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var usbDevices = GetUSBDevices();

            foreach (var usbDevice in usbDevices) loadUSBData(usbDevice.DeviceID);

            if (usb_detected) open_main();
        }

        private void loadUSBData(string pid)
        {
            XDocument doc;

            doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));

            foreach (var user in doc.Descendants("data"))
            foreach (var dm in doc.Descendants("user"))
            {
                tempUser = dm.Element("userName").Value;
                var tempPass = dm.Element("pass").Value;


                if (pid == dm.Element("pid").Value)
                {
                    var document = new XmlDocument();
                    document.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));

                    var XPathName = "/list/session/name";
                    var node_name = document.SelectSingleNode(XPathName);
                    node_name.InnerXml = tempUser;
                    document.Save(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));
                    usb_detected = true;
                }
            }
        }

        private void loadUserData()
        {
            XDocument doc;
            doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));

            foreach (var user in doc.Descendants("data"))
            foreach (var dm in doc.Descendants("user"))
            {
                tempUser = dm.Element("userName").Value;
                var tempPass = dm.Element("pass").Value;

                if (tempUser.ToLower() == userName.Text.ToLower() && tempPass == Main.GetHashString(password.Text))
                {
                    loginOK = true;
                    var document = new XmlDocument();
                    document.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));


                    var XPathName = "/list/session/name";
                    var node_name = document.SelectSingleNode(XPathName);
                    node_name.InnerXml = tempUser;
                    document.Save(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));
                }
            }
        }

        private void login_form_Load(object sender, EventArgs e)
        {
            var query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            watcher.EventArrived += watcher_EventArrived;
            watcher.Query = query;
            watcher.Start();
        }

        private void open_main()
        {
            Delegate d = new MethodInvoker(Hide);
            Invoke(d);

            Delegate ms = new MethodInvoker(main.Show);
            Invoke(ms);
        }

        public static List<USBDeviceInfo> GetUSBDevices()
        {
            var devices = new List<USBDeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_Volume"))
            {
                collection = searcher.Get();
            }

            foreach (var device in collection)
                if ((uint) device.GetPropertyValue("DriveType") >= 0x2 &&
                    (uint) device.GetPropertyValue("DriveType") <= 0x3 &&
                    (string) device.GetPropertyValue("Label") != null)
                    devices.Add(new USBDeviceInfo(
                        (string) device.GetPropertyValue("DeviceID"),
                        (string) device.GetPropertyValue("Label"),
                        (string) device.GetPropertyValue("Name"),
                        (uint) device.GetPropertyValue("DriveType"),
                        (bool) device.GetPropertyValue("BootVolume")
                    ));

            collection.Dispose();
            return devices;
        }

        private void login_form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) checkLogIn();
        }

        private void password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) checkLogIn();
        }

        private void password_OnValueChanged(object sender, EventArgs e)
        {
            errorText.Text = "";
            password.isPassword = true;
            var user = userName.Text;
            var passField = password.Text;
            var pass = Main.GetHashString(password.Text);
        }

        private void userName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) checkLogIn();
        }

        private void userName_OnValueChanged(object sender, EventArgs e)
        {
            errorText.Text = "";
            var user = userName.Text;
            var passField = password.Text;
            var pass = Main.GetHashString(password.Text);
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void open_Click(object sender, EventArgs e)
        {
            checkLogIn();
        }

        private void bunifuImageButton1_MouseEnter(object sender, EventArgs e)
        {
            password.isPassword = false;
        }

        private void bunifuImageButton1_MouseLeave(object sender, EventArgs e)
        {
            password.isPassword = true;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            var me = (MouseEventArgs) e;
            if (me.Button == MouseButtons.Left) Close();
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            var tt = new ToolTip();
            tt.SetToolTip(closeButton, "Schließen");
            closeButton.BackColor = Color.FromArgb(223, 1, 1);
            closeButton.Image = Resources.close_white;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            closeButton.BackColor = Color.Transparent;
            if (Style.iconStyle == Style.IconStyle.LIGHT)
                closeButton.Image = Resources.close;
            else
                closeButton.Image = Resources.close_white;
        }

        private void login_form_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            grabX = MousePosition.X - DesktopLocation.X;
            grabY = MousePosition.Y - DesktopLocation.Y;
        }

        private void login_form_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown)
            {
                mouseX = MousePosition.X - grabX;
                mouseY = MousePosition.Y - grabY;
                SetDesktopLocation(mouseX, mouseY);
            }
        }

        private void login_form_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
        }
    }

    public class USBDeviceInfo
    {
        public USBDeviceInfo(string deviceID, string label, string name, uint type, bool bootVolume)
        {
            DeviceID = deviceID;
            Label = label;
            Name = name;
            Type = type;
            BootVolume = bootVolume;
        }

        public string DeviceID { get; }
        public string Label { get; }
        public string Name { get; }
        public uint Type { get; }
        public bool BootVolume { get; }
    }
}