using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing.QrCode;
using ZXing.Common;
using ZXing;
using System.Net;
using System.Net.Sockets;
using ZXing.QrCode.Internal;
using ZXing.Rendering;

namespace allinthebox
{
    public partial class remote_view : Form
    {
        String user;

        public remote_view(string user, Main main)
        {
            InitializeComponent();
            this.user = user;
            writeLabel = new WriteLabel(WriteLabelMethod);
            main.remoteServer.StartServer();
        }


  

        [Obsolete]
        private void Panel2_Paint(object sender, PaintEventArgs e)
        {
            if (onlyIntern)
            {
                string data = "aitb;" + RemoteServer.GetIP4Address() + ";" + RemoteServer.PORT + ";" + this.user + ";" + RemoteServer.CODE;
                data = Convert.ToBase64String(Security.Encrypt(data));
                e.Graphics.DrawImage(GenerateQR(300, 300, "aitb" + data), 0, 0, 300, 300);
            }
            else
            {
                string data = "aitb;" + RemoteServer.GetExtIP4Address() + ";" + RemoteServer.PORT + ";" + this.user + ";" + RemoteServer.CODE;
                data = Convert.ToBase64String(Security.Encrypt(data));
                e.Graphics.DrawImage(GenerateQR(300, 300, "aitb" + data), 0, 0, 300, 300);
            }
        }

        public void WriteLabelMethod(string text) {
            this.connection_label.Text = text;
        }

        public Bitmap GenerateQR(int width, int height, string text)
        {
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            EncodingOptions encodingOptions = new EncodingOptions() { Width = 300, Height = 300, Margin = 0, PureBarcode = false };
            encodingOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            barcodeWriter.Renderer = new BitmapRenderer();
            barcodeWriter.Options = encodingOptions;
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            Bitmap bitmap = barcodeWriter.Write(text);
            Bitmap logo = Properties.Resources.Logo_White;
            Graphics g = Graphics.FromImage(bitmap);
            g.DrawImage(logo,(bitmap.Width - 65) / 2,(bitmap.Height - 65) / 2,65,65);

            return bitmap;
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
                this.Hide();
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

        private void BackupManager_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(this.Name);
        }


        private void Refresh_MouseEnter(object sender, EventArgs e)
        {
            this.refresh.BackColor = Color.FromArgb(0, 96, 170);
            this.refresh.Image = Properties.Resources.i_white;
        }

        private void Refresh_MouseLeave(object sender, EventArgs e)
        {
            this.refresh.BackColor = Color.Transparent;
            this.refresh.Image = Properties.Resources.i_black;
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

        private void World_MouseEnter(object sender, EventArgs e)
        {
            hover = true;
            this.world.BackColor = Color.FromArgb(0, 96, 170);
            if (_lock % 2 != 0)
            {
                this.world.Image = Properties.Resources.unlock_white;
            }
            else
            {
                this.world.Image = Properties.Resources.lock_white;
            }
        }

        private int _lock = 0;
        private bool hover = false;
        public bool onlyIntern = true;

        private void World_Click(object sender, EventArgs e)
        {
            //toggle local, extern
            if (hover)
            {
                if (_lock % 2 != 0)
                {
                    this.world.Image = Properties.Resources.unlock_white;
                    onlyIntern = true;
                }
                else
                {
                    this.world.Image = Properties.Resources.lock_white;
                    onlyIntern = false;
                }
            }
            else {
                if (_lock % 2 != 0)
                {
                    this.world.Image = Properties.Resources.unlock_black;
                    onlyIntern = false;
                }
                else
                {
                    this.world.Image = Properties.Resources.lock_black;
                    onlyIntern = true;
                }
            }

            _lock++;
            this.panel2.Refresh();

        }

        private void World_MouseLeave(object sender, EventArgs e)
        {
            hover = false;
            this.world.BackColor = Color.Transparent;
            if (_lock % 2 != 0)
            {
                this.world.Image = Properties.Resources.unlock_black;
            }
            else
            {
                this.world.Image = Properties.Resources.lock_black;
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Deine interne IP Adresse lautet: " + RemoteServer.GetIP4Address() + "\nDie externe IP ist: " + RemoteServer.GetExtIP4Address() + "\nDas Port ist Port " + RemoteServer.PORT + "\nDas Passwort ist: "+ RemoteServer.CODE, "IP Info");
        }

        private void bg_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;

        }

        public delegate void WriteLabel(string text);
        public WriteLabel writeLabel;
    }
}
