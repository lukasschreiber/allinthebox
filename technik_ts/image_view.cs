using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Xml;

namespace allinthebox
{
    public partial class image_view : Form
    {
        public Main main;
        int iconStyle;
        int maxWidth;
        public image_view(Image img, String title, Main m)
        {
            InitializeComponent();


            this.header.Text = Properties.strings.picture;
            this.header.Text = title;

            this.main = m;
            this.Text = title;
            this.MinimizeBox = false;
            this.MaximizeBox = false;


            //load style
            currentStyle = main.loadSettingsDataBase().SelectSingleNode("/settings/style").InnerXml;
            maxWidth = int.Parse(main.loadSettingsDataBase().SelectSingleNode("/settings/maxWidthImage").InnerXml);

            ColorConverter cc = new ColorConverter();

            //load colors and images
            if (Style.iconStyle == Style.IconStyle.DARK)
            {
                this.closeButton.Image = allinthebox.Properties.Resources.close_white;
                iconStyle = 0;
            }
            else
            {
                this.closeButton.Image = allinthebox.Properties.Resources.close;
                iconStyle = 1;
            }

            if (main.imageOnTop)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }

            picture.Image = img;

            int y = img.Height;
            int x = img.Width;

            if (x <= maxWidth && y <= x)
            {
                this.Width = img.Width;
                this.Height = img.Height+36;
                this.picture.Location = new Point(0, 36);
                this.picture.Size = new Size(img.Width, img.Height);
            }
            else if (y <= maxWidth && x <= y)
            {
                this.Width = img.Width;
                this.Height = img.Height+36;
                this.picture.Location = new Point(0, 36);
                this.picture.Size = new Size(img.Width, img.Height);
            }
            else
            {
                this.Width = maxWidth;
                double differenz = (double)img.Height / (double)img.Width;
                double produkt = (double)differenz * maxWidth;
                int height = (int)Math.Floor(produkt);
                this.Height = height + 36;
                this.picture.Location = new Point(0, 36);
                this.picture.Size = new Size(maxWidth, height);
            }
        }

        string name = "img";

        private void image_view_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.currentOpenWindows.Remove(name);

        }

        private void image_view_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(name);
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
            if (iconStyle == 1)
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
        private string currentStyle;

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
