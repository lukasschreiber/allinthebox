using System;
using System.Drawing;
using System.Windows.Forms;
using allinthebox.Properties;

namespace allinthebox
{
    public partial class image_view : Form
    {
        private string currentStyle;
        private int grabX, grabY;
        private readonly int iconStyle;
        public Main main;
        private readonly int maxWidth;
        private bool mousedown, maximized;

        private int mouseX, mouseY;

        private readonly string name = "img";

        public image_view(Image img, string title, Main m)
        {
            InitializeComponent();


            header.Text = strings.picture;
            header.Text = title;

            main = m;
            Text = title;
            MinimizeBox = false;
            MaximizeBox = false;


            //load style
            currentStyle = main.loadSettingsDataBase().SelectSingleNode("/settings/style").InnerXml;
            maxWidth = int.Parse(main.loadSettingsDataBase().SelectSingleNode("/settings/maxWidthImage").InnerXml);

            var cc = new ColorConverter();

            //load colors and images
            if (Style.iconStyle == Style.IconStyle.DARK)
            {
                closeButton.Image = Resources.close_white;
                iconStyle = 0;
            }
            else
            {
                closeButton.Image = Resources.close;
                iconStyle = 1;
            }

            if (main.imageOnTop)
                TopMost = true;
            else
                TopMost = false;

            picture.Image = img;

            var y = img.Height;
            var x = img.Width;

            if (x <= maxWidth && y <= x)
            {
                Width = img.Width;
                Height = img.Height + 36;
                picture.Location = new Point(0, 36);
                picture.Size = new Size(img.Width, img.Height);
            }
            else if (y <= maxWidth && x <= y)
            {
                Width = img.Width;
                Height = img.Height + 36;
                picture.Location = new Point(0, 36);
                picture.Size = new Size(img.Width, img.Height);
            }
            else
            {
                Width = maxWidth;
                var differenz = img.Height / (double) img.Width;
                var produkt = differenz * maxWidth;
                var height = (int) Math.Floor(produkt);
                Height = height + 36;
                picture.Location = new Point(0, 36);
                picture.Size = new Size(maxWidth, height);
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

        private void image_view_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.currentOpenWindows.Remove(name);
        }

        private void image_view_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(name);
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