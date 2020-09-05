using System;
using System.Drawing;
using System.Windows.Forms;
using allinthebox.Properties;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using ZXing.Rendering;

namespace allinthebox
{
    public partial class remote_view : Form
    {
        public delegate void WriteLabel(string text);

        private int _lock;
        private int grabX, grabY;
        private bool hover;
        private bool mousedown, maximized;

        private int mouseX, mouseY;
        public bool onlyIntern = true;
        private readonly string user;
        public WriteLabel writeLabel;

        public remote_view(string user, Main main)
        {
            InitializeComponent();
            this.user = user;
            writeLabel = WriteLabelMethod;
            main.remoteServer.StartServer();
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


        [Obsolete]
        private void Panel2_Paint(object sender, PaintEventArgs e)
        {
            if (onlyIntern)
            {
                var data = "aitb;" + RemoteServer.GetIP4Address() + ";" + RemoteServer.PORT + ";" + user + ";" +
                           RemoteServer.CODE;
                data = Convert.ToBase64String(Security.Encrypt(data));
                e.Graphics.DrawImage(GenerateQR(300, 300, "aitb" + data), 0, 0, 300, 300);
            }
            else
            {
                var data = "aitb;" + RemoteServer.GetExtIP4Address() + ";" + RemoteServer.PORT + ";" + user + ";" +
                           RemoteServer.CODE;
                data = Convert.ToBase64String(Security.Encrypt(data));
                e.Graphics.DrawImage(GenerateQR(300, 300, "aitb" + data), 0, 0, 300, 300);
            }
        }

        public void WriteLabelMethod(string text)
        {
            connection_label.Text = text;
        }

        public Bitmap GenerateQR(int width, int height, string text)
        {
            var barcodeWriter = new BarcodeWriter();
            var encodingOptions = new EncodingOptions {Width = 300, Height = 300, Margin = 0, PureBarcode = false};
            encodingOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            barcodeWriter.Renderer = new BitmapRenderer();
            barcodeWriter.Options = encodingOptions;
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            var bitmap = barcodeWriter.Write(text);
            var logo = Resources.Logo_White;
            var g = Graphics.FromImage(bitmap);
            g.DrawImage(logo, (bitmap.Width - 65) / 2, (bitmap.Height - 65) / 2, 65, 65);

            return bitmap;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            var me = (MouseEventArgs) e;
            if (me.Button == MouseButtons.Left) Hide();
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

        private void BackupManager_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(Name);
        }


        private void Refresh_MouseEnter(object sender, EventArgs e)
        {
            refresh.BackColor = Color.FromArgb(0, 96, 170);
            refresh.Image = Resources.i_white;
        }

        private void Refresh_MouseLeave(object sender, EventArgs e)
        {
            refresh.BackColor = Color.Transparent;
            refresh.Image = Resources.i_black;
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

        private void World_MouseEnter(object sender, EventArgs e)
        {
            hover = true;
            world.BackColor = Color.FromArgb(0, 96, 170);
            if (_lock % 2 != 0)
                world.Image = Resources.unlock_white;
            else
                world.Image = Resources.lock_white;
        }

        private void World_Click(object sender, EventArgs e)
        {
            //toggle local, extern
            if (hover)
            {
                if (_lock % 2 != 0)
                {
                    world.Image = Resources.unlock_white;
                    onlyIntern = true;
                }
                else
                {
                    world.Image = Resources.lock_white;
                    onlyIntern = false;
                }
            }
            else
            {
                if (_lock % 2 != 0)
                {
                    world.Image = Resources.unlock_black;
                    onlyIntern = false;
                }
                else
                {
                    world.Image = Resources.lock_black;
                    onlyIntern = true;
                }
            }

            _lock++;
            panel2.Refresh();
        }

        private void World_MouseLeave(object sender, EventArgs e)
        {
            hover = false;
            world.BackColor = Color.Transparent;
            if (_lock % 2 != 0)
                world.Image = Resources.unlock_black;
            else
                world.Image = Resources.lock_black;
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Deine interne IP Adresse lautet: " + RemoteServer.GetIP4Address() + "\nDie externe IP ist: " +
                RemoteServer.GetExtIP4Address() + "\nDas Port ist Port " + RemoteServer.PORT + "\nDas Passwort ist: " +
                RemoteServer.CODE, "IP Info");
        }

        private void bg_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
        }
    }
}