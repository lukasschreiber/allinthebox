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

namespace allinthebox
{
    public partial class add_user : Form
    {
        private userManager um;

        public add_user(userManager userManager)
        {
            um = userManager;
            InitializeComponent();

            //language
            this.label4.Text = Properties.strings.add;
            this.add.Text = Properties.strings.addButton;

            user_box.ForeColor = SystemColors.GrayText;
            user_box.Text = Properties.strings.username;
            this.user_box.Leave += new System.EventHandler(this.user_box_Leave);
            this.user_box.Enter += new System.EventHandler(this.user_box_Enter);

            pass_box.ForeColor = SystemColors.GrayText;
            pass_box.Text = Properties.strings.password;
            this.pass_box.Leave += new System.EventHandler(this.pass_box_Leave);
            this.pass_box.Enter += new System.EventHandler(this.pass_box_Enter);
        }

        private void user_box_Leave(object sender, EventArgs e)
        {
            if (user_box.Text.Length == 0)
            {
                user_box.Text = Properties.strings.username;
                user_box.ForeColor = SystemColors.GrayText;
            }
        }

        private void user_box_Enter(object sender, EventArgs e)
        {
            if (user_box.Text == Properties.strings.username)
            {
                user_box.Text = "";
                user_box.ForeColor = SystemColors.WindowText;
            }
        }

        private void pass_box_Leave(object sender, EventArgs e)
        {
            if (pass_box.Text.Length == 0) {
                pass_box.Text = Properties.strings.password;
            }
        }

        private void pass_box_Enter(object sender, EventArgs e)
        {
            if (pass_box.Text == Properties.strings.password)
            {
                pass_box.Text = "";
                pass_box.ForeColor = SystemColors.WindowText;
            }
        }

        private void add_Click(object sender, EventArgs e)
        {
            if (pass_box.Text != "" && user_box.Text != "" && pass_box.Text != Properties.strings.password && user_box.Text != Properties.strings.username)
            {
                bool namingViolation = false;
                String spath = FileManager.GetDataBasePath(FileManager.NAMES.USER);
                XDocument doc = XDocument.Load(spath);
                foreach (XElement node in doc.Element("data").Elements("user")) {
                    if (node.Element("userName").Value.ToLower() ==user_box.Text.ToLower())
                    {
                        namingViolation = true;
                    }
                }
                if (!namingViolation)
                {
                    XElement root = new XElement("user");
                    root.Add(new XElement("userName", user_box.Text));
                    root.Add(new XElement("pass", Main.GetHashString(pass_box.Text)));
                    root.Add(new XElement("pid", ""));
                    root.Add(new XElement("label", ""));
                    root.Add(new XElement("admin", "false"));
                    doc.Element("data").Add(root);
                    doc.Save(spath);
                    um.uRefresh();
                    um.Focus();
                }
                else {
                    MessageBox.Show(Properties.strings.namingViolation);
                }
            }
            else
            {
                MessageBox.Show(Properties.strings.passwordAndUserFilled);
            }
        }

        private void add_user_FormClosing(object sender, FormClosingEventArgs e)
        {
            um.Focus();
        }

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

        private void closeButton_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left)
            {
                this.Close();
            }
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.closeButton, Properties.strings.close);
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

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row == 0)
            {
                using (SolidBrush brush = new SolidBrush(Color.Silver))
                    e.Graphics.FillRectangle(brush, e.CellBounds);
            }
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void bg_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            grabX = (MousePosition.X - this.DesktopLocation.X);
            grabY = (MousePosition.Y - this.DesktopLocation.Y);
        }

        private void bg_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown)
            {
                mouseX = MousePosition.X - grabX;
                mouseY = MousePosition.Y - grabY;
                this.SetDesktopLocation(mouseX, mouseY);
            }
        }

        private void bg_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;

        }
    }
}
