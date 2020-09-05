using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using allinthebox.Properties;

namespace allinthebox
{
    public partial class info_copy : Form
    {
        private int grabX, grabY;
        private readonly int iconStyle = 1;
        private Main main;
        private bool mousedown, maximized;

        private int mouseX, mouseY;

        public info_copy(Main m)
        {
            TopMost = true;
            InitializeComponent();

            //language
            linkLabel2.Text = strings.linkedIn;
            label1.Text = strings.infotext;

            main = m;
            label2.Text = "© Lukas Schreiber, 2017 - " + DateTime.Now.Year;
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

        private void info_copy_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(Name);
        }

        private void info_copy_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.currentOpenWindows.Remove(Name);
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
            if (iconStyle == 1)
                closeButton.Image = Resources.close;
            else
                closeButton.Image = Resources.close_white;
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.linkedin.com/in/lukas-schreiber-36b88b18b/");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:luke-schreiber@gmx.de");
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