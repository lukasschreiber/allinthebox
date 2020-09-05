using System;
using System.Drawing;
using System.Windows.Forms;

namespace Design
{
    public partial class CustomSearchBox : UserControl
    {
        private Rectangle buttonRect;
        private bool hover;

        public bool searchInAction = false;

        public CustomSearchBox()
        {
            InitializeComponent();
            DoubleBuffered = true;
            textBox1.BackColor = BackColor;
            Cursor = Cursors.IBeam;
            textBox1.GotFocus += TextBox1_GotFocus;
            textBox1.LostFocus += TextBox1_LostFocus;
            textBox1.TextChanged += TextBox1_TextChanged;
            textBox1.KeyUp += TextBox1_KeyUp;
        }

        public Color BorderColor { get; set; } = Color.Black;

        public Color FocusBorderColor { get; set; } = Color.Blue;

        public Color HoverColor { get; set; } = Color.Gray;

        public int BorderThickness { get; set; } = 3;

        public Image FocusImage { get; set; } = null;

        public Image Image { get; set; } = null;

        public int ResizeConst { get; set; } = 0;

        public int FocusResizeConst { get; set; } = 0;

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            Text = textBox1.Text;
            Invalidate();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            textBox1.Text = Text;
        }

        private void TextBox1_LostFocus(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void TextBox1_GotFocus(object sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (buttonRect.Contains(e.Location))
            {
                if (searchInAction)
                    OnResetClick();
                else
                    OnSearchClick();
            }
        }


        public event EventHandler SearchClick;

        private void OnSearchClick()
        {
            if (SearchClick != null) SearchClick(this, EventArgs.Empty);
        }

        public event EventHandler ResetClick;

        private void OnResetClick()
        {
            if (ResetClick != null) ResetClick(this, EventArgs.Empty);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (buttonRect != null)
            {
                if (buttonRect.Contains(e.Location))
                {
                    var _hover = hover;
                    hover = true;
                    Cursor = Cursors.Default;
                    if (_hover != hover) Invalidate();
                }
                else
                {
                    var _hover = hover;
                    hover = false;
                    Cursor = Cursors.Default;
                    if (_hover != hover) Invalidate();
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            var _hover = hover;
            hover = false;
            if (_hover != hover) Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Brush b = new SolidBrush(textBox1.Focused ? FocusBorderColor : BorderColor))
            {
                e.Graphics.DrawRectangle(new Pen(b, BorderThickness), new Rectangle(0, 0, Width - 1, Height - 1));
            }

            if ((searchInAction ? FocusImage : Image) != null)
            {
                float resizeConst = searchInAction ? FocusResizeConst : ResizeConst;
                var x = textBox1.Width + BorderThickness + Padding.Left + 2 + resizeConst / 2;
                var y = Padding.Top + BorderThickness + resizeConst / 2;
                var width = Height - 2 * BorderThickness - Padding.Top - Padding.Bottom - resizeConst;
                var height = Height - 2 * BorderThickness - Padding.Top - Padding.Bottom - resizeConst;
                buttonRect = new Rectangle((int) (x - resizeConst / 2), (int) (y - resizeConst / 2),
                    (int) (width + resizeConst), (int) (height + resizeConst));
                if (hover) e.Graphics.FillRectangle(new SolidBrush(HoverColor), buttonRect);
                e.Graphics.DrawImage(searchInAction ? FocusImage : Image, x, y, width, height);
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            textBox1.BackColor = BackColor;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CalcSize();
        }

        private void CalcSize()
        {
            textBox1.Height = Height - 2 * BorderThickness - Padding.Top - Padding.Bottom;
            textBox1.Width = Width - 2 * BorderThickness - Padding.Left - Padding.Right -
                             (Height - 2 * BorderThickness - Padding.Top - Padding.Bottom + 2);
            var s = (Padding.Bottom + Padding.Top + textBox1.Height) / 2;
            var t = Padding.Left + BorderThickness;

            textBox1.Location = new Point(t, s);
        }
    }
}