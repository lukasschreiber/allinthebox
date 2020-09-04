using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace allinthebox
{
    public partial class help : Form
    {
        public Main main;
        public help(String version, Main m)
        {
            InitializeComponent();

            this.Text = Properties.strings.help;
            this.label2.Text = Properties.strings.help;

            this.main = m; 
            string html = Properties.Resources.help;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            doc.GetElementbyId("version").InnerHtml = "<em>v." + version+"</em>";
            //doc.GetElementbyId("image").Attributes["src"].Value = AppDomain.CurrentDomain.BaseDirectory + "LogoSquare.png";

            html = doc.DocumentNode.OuterHtml;
            webBrowser1.DocumentText = html;
        }

        private void help_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(this.Name);
        }

        private void help_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.currentOpenWindows.Remove(this.Name);

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
