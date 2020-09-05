using System;
using System.Drawing;
using System.Windows.Forms;
using allinthebox.Properties;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace allinthebox
{
    public partial class help : Form
    {
        private int grabX, grabY;
        public Main main;
        private bool mousedown, maximized;

        private int mouseX, mouseY;

        public help(string version, Main m)
        {
            InitializeComponent();

            Text = strings.help;
            label2.Text = strings.help;

            main = m;
            var html = Resources.help;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            doc.GetElementbyId("version").InnerHtml = "<em>v." + version + "</em>";
            //doc.GetElementbyId("image").Attributes["src"].Value = AppDomain.CurrentDomain.BaseDirectory + "LogoSquare.png";

            html = doc.DocumentNode.OuterHtml;
            webBrowser1.DocumentText = html;
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

        private void help_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(Name);
        }

        private void help_FormClosing(object sender, FormClosingEventArgs e)
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
            if (Style.iconStyle == Style.IconStyle.LIGHT)
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