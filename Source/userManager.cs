using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using allinthebox.Properties;

namespace allinthebox
{
    public partial class userManager : Form
    {
        private readonly int iconStyle = 1;
        public Main main;

        public userManager(Main main)
        {
            InitializeComponent();

            //language
            label3.Text = strings.userManager;
            label1.Text = strings.username;
            label2.Text = strings.password;
            adminCheck.Text = strings.admin;
            userCheck.Text = strings.user;
            save.Text = strings.save;
            del.Text = strings.delete;
            add.Text = strings.newUser;
            refresh.Text = strings.refresh;

            loadUserData();
            this.main = main;

            var user = main.user_loggedin;
            user_list.SetSelected(user_list.FindStringExact(user), true);
        }


        #region load users to list on right

        private void loadUserData()
        {
            user_box.Enabled = false;

            var doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));

            foreach (var dm in doc.Descendants("user"))
            {
                var user_name = dm.Element("userName").Value;

                user_list.Items.Add(user_name);
            }

            var usbDevices = login_form.GetUSBDevices();

            pid_out.Items.Clear();

            pid_out.Items.Add(strings.noUsb);

            foreach (var usbDevice in usbDevices)
                if (usbDevice.Name.Contains(":") && usbDevice.Type >= 0x2 && usbDevice.Type <= 0x3 &&
                    !usbDevice.BootVolume)
                    if (usbDevice.Label != null && usbDevice.Label != "")
                        pid_out.Items.Add(usbDevice.Label);
        }

        #endregion

        #region load user details to view

        private void user_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cur_name = user_list.SelectedItem.ToString();

            user_box.Text = cur_name;

            user_box.Enabled = false;

            var document = new XmlDocument();
            document.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));
            var cur_pass = document.SelectSingleNode("/data/user[userName='" + cur_name + "']/pass").InnerText;
            var cur_pid = document.SelectSingleNode("data/user[userName='" + cur_name + "']/pid").InnerText;
            var cur_isAdmin = document.SelectSingleNode("data/user[userName='" + cur_name + "']/admin").InnerText;
            var cur_label = document.SelectSingleNode("data/user[userName='" + cur_name + "']/label").InnerText;

            pass_box.Text = "";

            if (cur_isAdmin.Equals("true"))
                adminCheck.Checked = true;
            else if (cur_isAdmin.Equals("false")) userCheck.Checked = true;


            var usbDevices = login_form.GetUSBDevices();

            pid_out.Items.Clear();

            pid_out.Items.Add(strings.noUsb);

            var cur_found = false;


            foreach (var usbDevice in usbDevices)
            {
                if (usbDevice.Name.Contains(":") && usbDevice.Type >= 0x2 && usbDevice.Type <= 0x3 &&
                    !usbDevice.BootVolume)
                    if (usbDevice.Label != "" || usbDevice.Label != null)
                        pid_out.Items.Add(usbDevice.Label);
                if (usbDevice.DeviceID == cur_pid) cur_found = true;
            }

            if (cur_found != true) pid_out.Items.Add(cur_label);

            if (cur_label != "")
            {
                var check_pid = cur_label;
                pid_out.Text = check_pid;
            }
            else
            {
                pid_out.Text = strings.noUsb;
            }
        }

        #endregion

        #region delete user

        private void del_Click(object sender, EventArgs e)
        {
            var delete_okay = true;
            var cur_name = user_list.SelectedItem.ToString();

            var testDoc = new XmlDocument();
            testDoc.Load(FileManager.GetDataBasePath(FileManager.NAMES.DATA));

            foreach (XmlNode n in testDoc.SelectNodes("//user"))
                if (n.InnerXml.ToLower().Equals(cur_name.ToLower()))
                    delete_okay = false;

            if (delete_okay)
            {
                var doc = new XmlDocument();
                doc.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));
                var XPath = "/data/user[userName='" + cur_name + "']";
                var node = doc.SelectSingleNode(XPath);
                node.ParentNode.RemoveChild(node);
                doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.USER));

                if (user_list.SelectedIndex >= 1)
                {
                    //select -1
                }

                uRefresh();
            }
            else
            {
                MessageBox.Show("Der Nutzer hat noch etwas entliehen!");
            }
        }

        #endregion

        #region save user

        private void save_Click(object sender, EventArgs e)
        {
            var duplicateError = false;

            var cur_name = user_list.SelectedItem.ToString();

            if (pass_box.Text != "" && user_box.Text != "")
            {
                var testDoc = new XmlDocument();
                testDoc.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));

                foreach (XmlNode node in testDoc.SelectNodes("/data/user"))
                    if (!node.SelectSingleNode("userName").InnerXml.ToLower().Equals(cur_name.ToLower()))
                    {
                        var existingLabel = node.SelectSingleNode("label").InnerXml;
                        if (!existingLabel.Equals(""))
                            if (existingLabel.ToLower().Equals(pid_out.Text.ToLower()))
                                duplicateError = true;
                    }

                if (!duplicateError)
                {
                    var doc = new XmlDocument();
                    doc.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));

                    var XPathName = "/data/user[userName='" + cur_name + "']/userName";
                    var node_name = doc.SelectSingleNode(XPathName);
                    node_name.InnerXml = user_box.Text;

                    var XPathPass = "/data/user[userName='" + cur_name + "']/pass";
                    var node_pass = doc.SelectSingleNode(XPathPass);
                    node_pass.InnerXml = Main.GetHashString(pass_box.Text);

                    var XPathPid = "/data/user[userName='" + cur_name + "']/pid";
                    var node_pid = doc.SelectSingleNode(XPathPid);

                    var usbDevices = login_form.GetUSBDevices();

                    foreach (var usbDevice in usbDevices)
                        if (!pid_out.Text.ToLower().Equals("kein usb stick"))
                        {
                            var tempLabel = usbDevice.Label;
                            if (tempLabel == null) tempLabel = "USB Stick";
                            if (pid_out.Text.Equals(tempLabel)) node_pid.InnerXml = usbDevice.DeviceID;
                        }
                        else
                        {
                            node_pid.InnerXml = "";
                        }


                    var XPathLabel = "/data/user[userName='" + cur_name + "']/label";
                    var node_label = doc.SelectSingleNode(XPathLabel);
                    if (pid_out.Text.ToLower().Equals("kein usb stick"))
                        node_label.InnerXml = "";
                    else
                        node_label.InnerXml = pid_out.Text;

                    var XPathAdmin = "/data/user[userName='" + cur_name + "']/admin";
                    var node_admin = doc.SelectSingleNode(XPathAdmin);

                    if (adminCheck.Checked)
                        node_admin.InnerXml = "true";
                    else if (adminCheck.Checked == false) node_admin.InnerXml = "false";

                    doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.USER));
                    uRefresh();
                }
                else
                {
                    MessageBox.Show("Dieser USB Stick wird bereits als Schlüssel verwendet!");
                }
            }
            else
            {
                MessageBox.Show("Beide Felder müssen ausgefüllt werden!");
            }
        }

        #endregion

        #region refresh and add user

        public void uRefresh()
        {
            user_list.Items.Clear();
            loadUserData();
            try
            {
                user_list.SetSelected(user_list.FindStringExact(user_box.Text), true);
            }
            catch
            {
                user_list.SetSelected(0, true);
            }
        }

        private void add_Click(object sender, EventArgs e)
        {
            var addUserView = new add_user(this);
            addUserView.Show();
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            var usbDevices = login_form.GetUSBDevices();

            pid_out.Items.Clear();

            pid_out.Items.Add(strings.noUsb);

            foreach (var usbDevice in usbDevices)
                if (usbDevice.Name.Contains(":") && usbDevice.Type >= 0x2 && usbDevice.Type <= 0x3 &&
                    !usbDevice.BootVolume)
                    if (usbDevice.Label != null && usbDevice.Label != "")
                        pid_out.Items.Add(usbDevice.Label);
            uRefresh();
        }

        #endregion

        #region listeners and essentials

        private void userManager_Load(object sender, EventArgs e)
        {
            /*pid_out.DrawMode = DrawMode.OwnerDrawFixed;
            pid_out.DrawItem += pid_out_DrawItem;
            pid_out.DropDownClosed += pid_out_DropDownClosed;*/
            Main.currentOpenWindows.Add(Name);
        }

        private void pid_out_DropDownClosed(object sender, EventArgs e)
        {
            //toolTip.Hide(pid_out);
        }

        private void pid_out_DrawItem(object sender, DrawItemEventArgs e)
        {
            /*if (e.Index < 0) { return; }

            string text = pid_out.GetItemText(pid_out.Items[e.Index]);

            var usbDevices = login_form.GetUSBDevices();
            String Description = "";

            foreach (var usbDevice in usbDevices)
            {
                String content = pid_out.GetItemText(pid_out.Items[e.Index]);
                if (usbDevice.DeviceID == content)
                {
                    Description = usbDevice.Description;
                }
                else {
                    Description = text;
                }
            }

            e.DrawBackground();
            using (SolidBrush br = new SolidBrush(e.ForeColor))
            { e.Graphics.DrawString(text, e.Font, br, e.Bounds); }
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            { toolTip.Show(Description, pid_out, e.Bounds.Right, e.Bounds.Bottom); }
            e.DrawFocusRectangle();*/
        }

        private void pid_out_Leave(object sender, EventArgs e)
        {
            toolTip.Hide(pid_out);
        }

        private void pid_out_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(pid_out);
        }

        private void reset_inputs()
        {
            user_box.Text = "";
            pass_box.Text = "";
            pid_out.Text = "";
        }

        private void user_list_Leave(object sender, EventArgs e)
        {
            user_box.Enabled = false;
            //reset_inputs();
        }

        private void flowLayoutPanel1_Click(object sender, EventArgs e)
        {
            user_box.Enabled = false;
            //reset_inputs();
        }

        private void userManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.currentOpenWindows.Remove(Name);
        }

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row == 0)
                using (var brush = new SolidBrush(Color.Silver))
                {
                    e.Graphics.FillRectangle(brush, e.CellBounds);
                }
        }

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

        private void closeButton_Click(object sender, EventArgs e)
        {
            var me = (MouseEventArgs) e;
            if (me.Button == MouseButtons.Left) Close();
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            var tt = new ToolTip();
            tt.SetToolTip(closeButton, strings.noUsb);
            closeButton.BackColor = Color.FromArgb(223, 1, 1);
            closeButton.Image = Resources.close_white;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            closeButton.BackColor = Color.Transparent;
            if (iconStyle == 1)
                closeButton.Image = Resources.close;
            else
                closeButton.Image = Resources.close_white;
        }

        private int mouseX, mouseY;
        private bool mousedown, maximized;
        private int grabX, grabY;


        private void bg_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            grabX = MousePosition.X - DesktopLocation.X;
            grabY = MousePosition.Y - DesktopLocation.Y;
        }

        private void bg_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown)
            {
                mouseX = MousePosition.X - grabX;
                mouseY = MousePosition.Y - grabY;
                SetDesktopLocation(mouseX, mouseY);
            }
        }

        private void bg_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
        }
    }

    #endregion
}