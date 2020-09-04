using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;

namespace allinthebox
{
    using MetroFramework.Forms;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Management;
    using System.Reflection;

    public partial class login_form : Form
    {
        public string currentStyle;

        public login_form()
        {
            //setup window, center
            InitializeComponent();

            //language
            this.label1.Text = Properties.strings.username + ":";
            this.label2.Text = Properties.strings.password + ":";
            this.label3.Text = Properties.strings.useUSB;
            this.exit.ButtonText = Properties.strings.exit;
            this.open.ButtonText = Properties.strings.login;

            //version
            /*System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;*/
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.VersionLabel.Text = "v" + version;

            this.CenterToScreen();
            this.errorText.Text = "";
            main = new Main();

            //load style
            currentStyle = main.loadSettingsDataBase().SelectSingleNode("/settings/style").InnerXml;

            ColorConverter cc = new ColorConverter();

            #region style
            //load colors and images

            if (Style.iconStyle == Style.IconStyle.DARK)
            {
                this.bunifuImageButton1.Image = allinthebox.Properties.Resources.eye_white;
                this.closeButton.Image = allinthebox.Properties.Resources.close_white;
            }
            else {
                this.bunifuImageButton1.Image = allinthebox.Properties.Resources.eye_grey;
                this.closeButton.Image = allinthebox.Properties.Resources.close;
            }

            this.BackColor = Style.get("login/backColor");
            this.password.BackColor = this.userName.BackColor = this.BackColor;
            this.exit.BackColor = BackColor;
            this.open.BackColor = BackColor;
            this.exit.IdleLineColor = this.open.IdleLineColor = Style.get("login/buttonBorder");
            this.exit.ActiveForecolor = this.open.ActiveForecolor = Style.get("login/activeText");
            this.exit.IdleForecolor = this.open.IdleForecolor = Style.get("login/buttonText");
            this.exit.IdleFillColor = this.open.IdleFillColor = Style.get("login/buttonBack");
            this.password.LineFocusedColor = this.userName.LineFocusedColor = Style.get("login/lineActiveColor");
            this.exit.ActiveFillColor = this.open.ActiveFillColor = Style.get("login/buttonActiveColor");

            this.open.Refresh();
            this.exit.Refresh();

            this.userName.ForeColor =this.userName.HintForeColor = Style.get("login/textColorInput");
            this.password.ForeColor =this.password.HintForeColor= Style.get("login/textColorInput");
            this.password.LineIdleColor =this.password.LineMouseHoverColor = this.userName.LineMouseHoverColor = this.userName.LineIdleColor = Style.get("login/lineColor");
            this.userName.Refresh();
            this.password.Refresh();

            this.label1.ForeColor =this.label2.ForeColor = this.label3.ForeColor = Style.get("login/textColor");
            this.label1.Refresh();
            this.label2.Refresh();
            this.label3.Refresh();

            #endregion

        }

        //create frame shadow fpr better contrast
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        public const int WM_KEYDOWN = 0x0100;

        public String shouldPass;
        public Main main;
        public ManagementEventWatcher watcher = new ManagementEventWatcher();
        public Boolean usb_detected = false;
        public Boolean loginOK = false;
        public String tempUser;

        //check if credentials are correct
        public void checkLogIn()
        {
            //ignore case
            String user = userName.Text.ToLower();

            //encrypt with MD5
            String pass = Main.GetHashString(password.Text);

            loadUserData();


            if (user != "" && pass != "")
            {
                if (loginOK)
                {
                    open_main();
                  
                }
                else
                {
                    this.errorText.Text = Properties.strings.errorCredentialsWrong;
                }
            }
            else
            {
                this.errorText.Text = Properties.strings.errorEmpty;
            }
        }

        public string getUserName() {
            return userName.Text;
        }

        //check if USB is inserted
        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var usbDevices = GetUSBDevices();

            foreach (var usbDevice in usbDevices)
            {
                loadUSBData(usbDevice.DeviceID);
            }

            if (usb_detected)
            {

                open_main();
                
            }
        }

        private void loadUSBData(String pid) {
            XDocument doc;
            
            doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));

            foreach (var user in doc.Descendants("data"))
            {
                foreach (var dm in doc.Descendants("user"))
                {

                    tempUser = dm.Element("userName").Value;
                    String tempPass = dm.Element("pass").Value;
                    

                    if (pid == dm.Element("pid").Value)
                    {
                        XmlDocument document = new XmlDocument();
                        document.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));

                        String XPathName = "/list/session/name";
                        XmlNode node_name = document.SelectSingleNode(XPathName);
                        node_name.InnerXml = tempUser;
                        document.Save(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));
                        usb_detected = true;
                    }
                }
            }
        }

        private void loadUserData()
        {

            XDocument doc;
            doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));

            foreach (var user in doc.Descendants("data"))
            {
                foreach (var dm in doc.Descendants("user"))
                {

                    tempUser = dm.Element("userName").Value;
                    String tempPass = dm.Element("pass").Value;

                    if (tempUser.ToLower() == userName.Text.ToLower() && tempPass == Main.GetHashString(password.Text))
                    {
                        loginOK = true;
                        XmlDocument document = new XmlDocument();
                        document.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));


                        String XPathName = "/list/session/name";
                        XmlNode node_name = document.SelectSingleNode(XPathName);
                        node_name.InnerXml = tempUser;
                        document.Save(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));
                    }
                }
            }
        }

        private void login_form_Load(object sender, EventArgs e)
        {
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            watcher.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
            watcher.Query = query;
            watcher.Start();
        }

        private void open_main() {

            Delegate d = new MethodInvoker(this.Hide);
            this.Invoke(d);

            Delegate ms = new MethodInvoker(main.Show);
            this.Invoke(ms);
        }

        public static List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_Volume"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                if ((UInt32)device.GetPropertyValue("DriveType") >= 0x2 && (UInt32)device.GetPropertyValue("DriveType") <= 0x3 && (string)device.GetPropertyValue("Label") != null)
                {
                    devices.Add(new USBDeviceInfo(
                    (string)device.GetPropertyValue("DeviceID"),
                    (string)device.GetPropertyValue("Label"),
                    (string)device.GetPropertyValue("Name"),
                    (UInt32)device.GetPropertyValue("DriveType"),
                    (Boolean)device.GetPropertyValue("BootVolume")
                    ));
                }
                
            }

            collection.Dispose();
            return devices;
        }

        private void login_form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                checkLogIn();
            }
        }

        private void password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                checkLogIn();
            }
        }

        private void password_OnValueChanged(object sender, EventArgs e)
        {
            this.errorText.Text = "";
            password.isPassword = true;
            String user = userName.Text;
            String passField = password.Text;
            String pass = Main.GetHashString(password.Text);
           
        }

        private void userName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                checkLogIn();
            }
        }

        private void userName_OnValueChanged(object sender, EventArgs e)
        {
            this.errorText.Text = "";
            String user = userName.Text;
            String passField = password.Text;
            String pass = Main.GetHashString(password.Text);
      
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
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
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left) {
                this.Close();
            }
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.closeButton, "Schließen");
            this.closeButton.BackColor = Color.FromArgb(223, 1, 1);
            this.closeButton.Image = allinthebox.Properties.Resources.close_white;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            this.closeButton.BackColor = Color.Transparent;
            if (Style.iconStyle == Style.IconStyle.LIGHT)
            {
                this.closeButton.Image = allinthebox.Properties.Resources.close;
            }
            else
            {
                this.closeButton.Image = allinthebox.Properties.Resources.close_white;
            }
        }

        int mouseX = 0, mouseY = 0;
        bool mousedown, maximized;
        int grabX = 0, grabY = 0;

        private void login_form_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            grabX = (MousePosition.X - this.DesktopLocation.X);
            grabY = (MousePosition.Y - this.DesktopLocation.Y);
        }

        private void login_form_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown)
            {
                mouseX = MousePosition.X - grabX;
                mouseY = MousePosition.Y - grabY;
                this.SetDesktopLocation(mouseX, mouseY);
            }
        }

        private void login_form_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;

        }
    }

    public class USBDeviceInfo
    {
        public USBDeviceInfo(string deviceID, string label, string name, UInt32 type, Boolean bootVolume)
        {
            this.DeviceID = deviceID;
            this.Label = label;
            this.Name = name;
            this.Type = type;
            this.BootVolume = bootVolume;
        }
        public string DeviceID { get; private set; }
        public string Label { get; private set; }
        public string Name { get; private set; }
        public UInt32 Type { get; private set; }
        public Boolean BootVolume { get; private set; }

    }
}
