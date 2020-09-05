using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using allinthebox.Properties;

namespace allinthebox
{
    public partial class add_user : Form
    {
        private int grabX, grabY;
        private bool mousedown, maximized;

        private int mouseX, mouseY;
        private readonly userManager um;

        public add_user(userManager userManager)
        {
            um = userManager;
            InitializeComponent();

            //language
            label4.Text = strings.add;
            add.Text = strings.addButton;

            user_box.ForeColor = SystemColors.GrayText;
            user_box.Text = strings.username;
            user_box.Leave += user_box_Leave;
            user_box.Enter += user_box_Enter;

            pass_box.ForeColor = SystemColors.GrayText;
            pass_box.Text = strings.password;
            pass_box.Leave += pass_box_Leave;
            pass_box.Enter += pass_box_Enter;
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

        private void user_box_Leave(object sender, EventArgs e)
        {
            if (user_box.Text.Length == 0)
            {
                user_box.Text = strings.username;
                user_box.ForeColor = SystemColors.GrayText;
            }
        }

        private void user_box_Enter(object sender, EventArgs e)
        {
            if (user_box.Text == strings.username)
            {
                user_box.Text = "";
                user_box.ForeColor = SystemColors.WindowText;
            }
        }

        private void pass_box_Leave(object sender, EventArgs e)
        {
            if (pass_box.Text.Length == 0) pass_box.Text = strings.password;
        }

        private void pass_box_Enter(object sender, EventArgs e)
        {
            if (pass_box.Text == strings.password)
            {
                pass_box.Text = "";
                pass_box.ForeColor = SystemColors.WindowText;
            }
        }

        private void add_Click(object sender, EventArgs e)
        {
            if (pass_box.Text != "" && user_box.Text != "" && pass_box.Text != strings.password &&
                user_box.Text != strings.username)
            {
                var namingViolation = false;
                var spath = FileManager.GetDataBasePath(FileManager.NAMES.USER);
                var doc = XDocument.Load(spath);
                foreach (var node in doc.Element("data").Elements("user"))
                    if (node.Element("userName").Value.ToLower() == user_box.Text.ToLower())
                        namingViolation = true;
                if (!namingViolation)
                {
                    var root = new XElement("user");
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
                else
                {
                    MessageBox.Show(strings.namingViolation);
                }
            }
            else
            {
                MessageBox.Show(strings.passwordAndUserFilled);
            }
        }

        private void add_user_FormClosing(object sender, FormClosingEventArgs e)
        {
            um.Focus();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            var me = (MouseEventArgs) e;
            if (me.Button == MouseButtons.Left) Close();
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            var tt = new ToolTip();
            tt.SetToolTip(closeButton, strings.close);
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

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row == 0)
                using (var brush = new SolidBrush(Color.Silver))
                {
                    e.Graphics.FillRectangle(brush, e.CellBounds);
                }
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
        }

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
}