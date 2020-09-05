using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using allinthebox.Properties;

namespace allinthebox
{
    public class PictureHolder : PictureBox
    {
        public string barcode, name;
        private bool dragHandle;

        public bool emptyImg = true;
        private RectangleF HandleBounds;
        private bool mouseInBounds;
        private Main parent;
        public int PICTURE_COUNT = 3;

        private readonly float unit;


        public PictureHolder()
        {
            Size = new Size(77, 77);
            SizeMode = PictureBoxSizeMode.StretchImage;
            Padding = new Padding(40, 40, 40, 40);
            BorderStyle = BorderStyle.None;
            Image = Resources.more;
            BackColor = Color.White;
            MouseLeave += pic_MouseLeave;
            MouseEnter += pic_MouseEnter;
            MouseDown += PictureHolder_MouseDown;
            MouseUp += PictureHolder_MouseUp;
            DoubleClick += pic_MouseDoubleClick;
            Paint += pic_Paint;
            Click += PictureHolder_Click;
            Width = Height = 115;
            MouseMove += PictureHolder_MouseMove;

            unit = Width / 5f;
            HandleBounds = new RectangleF(unit * 2f - unit, 0, unit, 10);
        }

        public int Index { get; set; }

        public float ZoomValue { get; set; }

        public string FileName { get; set; }

        private void PictureHolder_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragHandle)
            {
                HandleBounds.X = e.X;

                if (HandleBounds.Right > Width) HandleBounds.X = Width - HandleBounds.Width;
                if (HandleBounds.Top < 0) HandleBounds.Y = 0;
                if (HandleBounds.Left < 0) HandleBounds.X = 0;
                if (HandleBounds.Bottom > Height) HandleBounds.Y = Height - HandleBounds.Height;
                if (HandleBounds.X < Width / 5)
                    ZoomValue = 1;
                else if (HandleBounds.X < Width / 5 * 2)
                    ZoomValue = 2;
                else if (HandleBounds.X < Width / 5 * 3)
                    ZoomValue = 3;
                else if (HandleBounds.X < Width / 5 * 4)
                    ZoomValue = 4;
                else if (HandleBounds.X < Width) ZoomValue = 5;
                Invalidate();
            }
        }

        public void sendInfo(string barcode, string name)
        {
            this.name = name;
            this.barcode = barcode;
        }

        private void PictureHolder_Click(object sender, EventArgs e)
        {
            var me = (MouseEventArgs) e;
            if (!emptyImg && me.Button == MouseButtons.Right)
            {
                if (parent.winOpenOnce || !Main.currentOpenWindows.Contains("img"))
                {
                    var img = new image_view(Image, name, parent);
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
            if (Image == null)
            {
                setEmpty();
                Image = Resources.none;
                return;
            }

            if (emptyImg == false)
            {
                var scaleFactor = ZoomValue;

                var startPoint = new Point(-(int) ((Image.Width / scaleFactor - Width) / 2),
                    -(int) ((Image.Height / scaleFactor - Height) / 2));

                e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, Width, Height);
                e.Graphics.DrawImage(Image, startPoint.X, startPoint.Y, Image.Width / scaleFactor,
                    Image.Height / scaleFactor);


                if (mouseInBounds)
                {
                    //scale for zoom

                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(220, 192, 192, 192)),
                        new RectangleF(0, 0, Width, 10));

                    if (dragHandle)
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 96, 170)), HandleBounds);
                    else
                        e.Graphics.FillRectangle(new SolidBrush(Color.White), HandleBounds);

                    var text = FileName;
                    var font = new Font("Century Gothic", 8, FontStyle.Regular, GraphicsUnit.Point);

                    var MaxWidth = Width - 6;
                    var sfFmt = new StringFormat(StringFormatFlags.LineLimit);
                    var g = Graphics.FromImage(new Bitmap(1, 1));
                    var sHeight = (int) g.MeasureString(text, font, MaxWidth, sfFmt).Height;
                    //int sOneLineHeight = (int)g.MeasureString("Z", objFont, MaxWidth, sfFmt).Height;
                    //int sNumLines = (int)(sHeight / sOneLineHeight);

                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(150, 100, 100, 100)),
                        new Rectangle(0, Height - sHeight - 6, Width, Height));
                    if (FileName != null || FileName != "")
                        e.Graphics.DrawString(
                            text,
                            font,
                            new SolidBrush(Color.White),
                            new Rectangle(3, Height - sHeight - 3, Width - 6, Height - 6)
                        );
                }
            }
        }

        public void setParent(Main main)
        {
            parent = main;
        }

        private async void pic_MouseDoubleClick(object sender, EventArgs e)
        {
            var me = (MouseEventArgs) e;

            if (me.Button == MouseButtons.Left)
            {
                //Left Click

                var openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = FileManager.GetDataBasePath("Images");
                openFileDialog.Filter = strings.imageFiles +
                                        "(*.jpg; *.png; *.jpeg; *.gif; *.bmp)|*.jpg; *.png; *.jpeg; *.gif; *.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Padding = new Padding(0, 0, 0, 0);
                    SizeMode = PictureBoxSizeMode.CenterImage;
                    FileName = openFileDialog.FileName.Split('\\').Last();

                    await Task.Factory.StartNew(delegate
                    {
                        if (!openFileDialog.FileName.Contains(FileManager.GetDataBasePath("Images")))
                            File.Copy(openFileDialog.FileName, FileManager.GetDataBasePath("Images\\" + FileName),
                                true);
                    });

                    Image = new Bitmap(FileManager.GetDataBasePath("Images\\" + FileName));
                    emptyImg = false;

                    switch (Index)
                    {
                        case 0:
                        {
                            parent.pic_2.Visible = true;
                            parent.pic_2.Invalidate();
                        }
                            break;
                    }

                    parent.changed = true;
                    parent.saveChanges();
                    parent.loadImagesToBuffer();
                }
            }
        }

        private void pic_MouseLeave(object sender, EventArgs e)
        {
            mouseInBounds = false;
            if (!emptyImg) ZoomValue += .2f;
            Invalidate();
        }

        private void pic_MouseEnter(object sender, EventArgs e)
        {
            HandleBounds = new RectangleF(unit * ZoomValue - unit, 0, unit, 10);
            Invalidate();
            mouseInBounds = true;
            if (!emptyImg) ZoomValue -= .2f;
            Invalidate();
        }

        public void setEmpty()
        {
            Image = Resources.more;
            Padding = new Padding(40, 40, 40, 40);
            SizeMode = PictureBoxSizeMode.StretchImage;
            emptyImg = true;
        }

        private void PictureHolder_MouseUp(object sender, MouseEventArgs e)
        {
            parent.changed = true;
            parent.saveChanges();

            dragHandle = false;
            Padding = new Padding(40, 40, 40, 40);
            Invalidate();
        }

        private void PictureHolder_MouseDown(object sender, MouseEventArgs e)
        {
            Padding = new Padding(38, 38, 38, 38);
            Invalidate();

            if (emptyImg == false)
                if (HandleBounds.Contains(e.Location))
                    dragHandle = true;
        }

        public static Image RotateImage(Image img, float rotationAngle)
        {
            var bmp = new Bitmap(img.Width, img.Height);

            var gfx = Graphics.FromImage(bmp);

            gfx.TranslateTransform((float) bmp.Width / 2, (float) bmp.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float) bmp.Width / 2, -(float) bmp.Height / 2);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(img, new Point(0, 0));
            gfx.Dispose();

            return bmp;
        }
    }
}