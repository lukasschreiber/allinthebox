using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace allinthebox
{
    public class PictureHolder : PictureBox
    {

        private float _ZoomValue;
        private int _Index;
        private string _FileName;
        private Main parent;
        public int PICTURE_COUNT = 3;
        private bool mouseInBounds = false;
        public string barcode, name;

        float unit;
        RectangleF HandleBounds;
        bool dragHandle = false;


        public PictureHolder() {
            this.Size = new System.Drawing.Size(77, 77);
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Padding = new Padding(40, 40, 40, 40);
            this.BorderStyle = BorderStyle.None;
            this.Image = allinthebox.Properties.Resources.more;
            this.BackColor = Color.White;
            this.MouseLeave += pic_MouseLeave;
            this.MouseEnter += pic_MouseEnter;
            this.MouseDown += PictureHolder_MouseDown;
            this.MouseUp += PictureHolder_MouseUp;
            this.DoubleClick += pic_MouseDoubleClick;
            this.Paint += pic_Paint;
            this.Click += PictureHolder_Click;
            this.Width = this.Height = 115;
            this.MouseMove += PictureHolder_MouseMove;

            unit = Width / 5f;
            HandleBounds = new RectangleF(unit*2f-unit, 0, unit, 10);
        }

        private void PictureHolder_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragHandle == true)
            {
                HandleBounds.X = e.X;

                if (HandleBounds.Right > this.Width)
                {
                    HandleBounds.X = this.Width - HandleBounds.Width;
                }
                if (HandleBounds.Top < 0)
                {
                    HandleBounds.Y = 0;
                }
                if (HandleBounds.Left < 0)
                {
                    HandleBounds.X = 0;
                }
                if (HandleBounds.Bottom > this.Height)
                {
                    HandleBounds.Y = this.Height - HandleBounds.Height;
                }
                if (HandleBounds.X < Width / 5)
                {
                    ZoomValue = 1;
                }
                else if (HandleBounds.X < (Width / 5) * 2)
                {
                    ZoomValue = 2;
                }
                else if (HandleBounds.X < (Width / 5) * 3)
                {
                    ZoomValue = 3;
                }
                else if (HandleBounds.X < (Width / 5) * 4)
                {
                    ZoomValue = 4;
                }
                else if (HandleBounds.X < Width)
                {
                    ZoomValue = 5;
                }
                Invalidate();
            }
        }

        public void sendInfo(string barcode, string name) {
            this.name = name;
            this.barcode = barcode;
        }

        private void PictureHolder_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (!emptyImg && me.Button == MouseButtons.Right)
            {
                if (parent.winOpenOnce || !Main.currentOpenWindows.Contains("img"))
                {
                    image_view img = new image_view(this.Image, this.name, this.parent);
                    img.Show();
                }
                else if (Main.currentOpenWindows.Contains("img"))
                {
                    Application.OpenForms["img"].BringToFront();
                }

            }

        }


        private void pic_Paint(object sender, PaintEventArgs e)
        {

            if (this.Image == null) {
                setEmpty();
                this.Image = Properties.Resources.none;
                return;
            }

            if (emptyImg == false)
            {

                float scaleFactor = ZoomValue;

                Point startPoint = new Point(-(int)((this.Image.Width / scaleFactor - this.Width) / 2), -(int)((this.Image.Height / scaleFactor - this.Height) / 2));

                e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, Width, Height);
                e.Graphics.DrawImage(this.Image, startPoint.X, startPoint.Y, this.Image.Width / scaleFactor, this.Image.Height / scaleFactor);


                if (this.mouseInBounds)
                {

                    //scale for zoom

                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(220, 192, 192, 192)), new RectangleF(0, 0, Width, 10));

                    if (this.dragHandle)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255,0,96,170)), HandleBounds);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.White), HandleBounds);
                    }

                    string text = this.FileName;
                    Font font = new Font("Century Gothic", 8, FontStyle.Regular, GraphicsUnit.Point);

                    int MaxWidth = Width -6 ; 
                    StringFormat sfFmt = new StringFormat(StringFormatFlags.LineLimit);
                    Graphics g = Graphics.FromImage(new Bitmap(1, 1));
                    int sHeight = (int)g.MeasureString(text, font, MaxWidth, sfFmt).Height;
                    //int sOneLineHeight = (int)g.MeasureString("Z", objFont, MaxWidth, sfFmt).Height;
                    //int sNumLines = (int)(sHeight / sOneLineHeight);

                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(150, 100, 100, 100)), new Rectangle(0, Height - sHeight - 6, Width, Height));
                    if (this.FileName != null || this.FileName != "") {
                        e.Graphics.DrawString(
                            text,
                            font,
                            new SolidBrush(Color.White),
                            new Rectangle(3, Height-sHeight-3, Width-6, Height-6)
                        );
                    }
                }
            }
        }

        public void setParent(Main main) {
            this.parent = main;
        }

        public int Index {
            get { return _Index; }
            set { _Index = value; }
        }

        public float ZoomValue {
            get { return _ZoomValue; }
            set {
                _ZoomValue = value;
            }
        }

        public String FileName {
            get { return _FileName; }
            set { _FileName = value; }
        }

        public bool emptyImg = true;

        private async void pic_MouseDoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;

            if (me.Button == MouseButtons.Left)
            {
                //Left Click

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = FileManager.GetDataBasePath("Images");
                openFileDialog.Filter = Properties.strings.imageFiles + "(*.jpg; *.png; *.jpeg; *.gif; *.bmp)|*.jpg; *.png; *.jpeg; *.gif; *.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Padding = new Padding(0, 0, 0, 0);
                    this.SizeMode = PictureBoxSizeMode.CenterImage;
                    this.FileName = openFileDialog.FileName.Split('\\').Last();

                    await Task.Factory.StartNew(delegate
                    {
                        if (!openFileDialog.FileName.Contains(FileManager.GetDataBasePath("Images")))
                        {
                            File.Copy(openFileDialog.FileName, FileManager.GetDataBasePath("Images\\" + this.FileName), true);

                        }

                    });

                    this.Image = new Bitmap(FileManager.GetDataBasePath("Images\\" + this.FileName));
                    emptyImg = false;

                    switch (Index)
                    {
                        case 0: { parent.pic_2.Visible = true; parent.pic_2.Invalidate(); } break;
                    }

                    parent.changed = true;
                    parent.saveChanges();
                    parent.loadImagesToBuffer();
                }

            }
           
        }

        private void pic_MouseLeave(object sender, EventArgs e)
        {
            this.mouseInBounds = false;
            if (!emptyImg)
            {
                this.ZoomValue += .2f;
            }
            this.Invalidate();
        }

        private void pic_MouseEnter(object sender, EventArgs e)
        {
             HandleBounds = new RectangleF(unit * ZoomValue - unit, 0, unit, 10);
            Invalidate();
            this.mouseInBounds = true;
            if (!emptyImg) {
                this.ZoomValue -= .2f;
            }
            this.Invalidate();
        }

        public void setEmpty() {
            this.Image = Properties.Resources.more;
            this.Padding = new Padding(40, 40, 40, 40);
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            emptyImg = true;
        }

        private void PictureHolder_MouseUp(object sender, MouseEventArgs e)
        {
            parent.changed = true;
            parent.saveChanges();

            dragHandle = false;
            this.Padding = new Padding(40, 40, 40, 40);
            this.Invalidate();
        }

        private void PictureHolder_MouseDown(object sender, MouseEventArgs e)
        {
            this.Padding = new Padding(38, 38, 38, 38);
            this.Invalidate();

            if (emptyImg == false)
            {
                if (HandleBounds.Contains(e.Location))
                {
                    dragHandle = true;
                }
            }
        }

        public static Image RotateImage(Image img, float rotationAngle)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);

            Graphics gfx = Graphics.FromImage(bmp);

            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(img, new Point(0, 0));
            gfx.Dispose();

            return bmp;
        }

    }
}
