using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Design
{
    public partial class CustomSearchBox : UserControl
    {
        private Color _BorderColor = Color.Black;
        private Color _FocusBorderColor = Color.Blue;
        private Color _HoverColor = Color.Gray;

        private Image _Image = null;
        private Image _FocusImage = null;
        private int _ResizeConst = 0;
        private int _FocusResizeConst = 0;
        private int _BorderThickness = 3;

        public CustomSearchBox()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.textBox1.BackColor = this.BackColor;
            this.Cursor = Cursors.IBeam;
            this.textBox1.GotFocus += TextBox1_GotFocus;
            this.textBox1.LostFocus += TextBox1_LostFocus;
            this.textBox1.TextChanged += TextBox1_TextChanged;
            this.textBox1.KeyUp += TextBox1_KeyUp;
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            this.Text = this.textBox1.Text;
            Invalidate();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.textBox1.Text = this.Text;
        }

        private void TextBox1_LostFocus(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void TextBox1_GotFocus(object sender, EventArgs e)
        {
            Invalidate();
        }

        public Color BorderColor
        {
            get { return _BorderColor; }
            set { _BorderColor = value; }
        }

        public Color FocusBorderColor
        {
            get { return _FocusBorderColor; }
            set { _FocusBorderColor = value; }
        }

        public Color HoverColor
        {
            get { return _HoverColor; }
            set { _HoverColor = value; }
        }

        public int BorderThickness
        {
            get { return _BorderThickness; }
            set { _BorderThickness = value; }
        }

        public Image FocusImage
        {
            get { return _FocusImage; }
            set { _FocusImage = value; }
        }

        public Image Image
        {
            get { return _Image; }
            set { _Image = value; }
        }

        public int ResizeConst
        {
            get { return _ResizeConst; }
            set { _ResizeConst = value; }
        }

        public int FocusResizeConst
        {
            get { return _FocusResizeConst; }
            set { _FocusResizeConst = value; }
        }

        public bool searchInAction = false;

        private Rectangle buttonRect;
        private bool hover = false;

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (buttonRect.Contains(e.Location)) {
                if (searchInAction)
                {
                    OnResetClick();
                }
                else {
                    OnSearchClick();
                }
            }
        }


        public event EventHandler SearchClick;

        private void OnSearchClick()
        {
            if (SearchClick != null)
            {
                SearchClick(this, EventArgs.Empty);
            }
        }

        public event EventHandler ResetClick;

        private void OnResetClick()
        {
            if (ResetClick != null)
            {
                ResetClick(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (buttonRect != null) {
                if (buttonRect.Contains(e.Location))
                {
                    bool _hover = hover;
                    hover = true;
                    this.Cursor = Cursors.Default;
                    if (_hover != hover){
                        Invalidate();
                    }

                }
                else {
                    bool _hover = hover;
                    hover = false;
                    this.Cursor = Cursors.Default; 
                    if (_hover != hover) {
                        Invalidate();
                    }
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            bool _hover = hover;
            hover = false;
            if (_hover != hover)
            {
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            using (Brush b = new SolidBrush(this.textBox1.Focused ? _FocusBorderColor : _BorderColor))
            {
                e.Graphics.DrawRectangle(new Pen(b,_BorderThickness), new Rectangle(0, 0, this.Width - 1, this.Height - 1));
            }

            if ((searchInAction ? _FocusImage : _Image) != null) {
                float resizeConst = searchInAction ? _FocusResizeConst : _ResizeConst;
                float x = this.textBox1.Width + _BorderThickness + this.Padding.Left + 2 + resizeConst/2;
                float y = this.Padding.Top + _BorderThickness + resizeConst/2;
                float width = this.Height - 2 * BorderThickness - this.Padding.Top - this.Padding.Bottom - resizeConst;
                float height = this.Height - 2 * BorderThickness - this.Padding.Top - this.Padding.Bottom - resizeConst;
                buttonRect = new Rectangle((int)(x-resizeConst/2), (int)(y-resizeConst/2), (int)(width+resizeConst), (int)(height+resizeConst));
                if (hover) {
                    e.Graphics.FillRectangle(new SolidBrush(_HoverColor), buttonRect);
                }
                e.Graphics.DrawImage(searchInAction ? _FocusImage : _Image, x, y, width, height);
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            this.textBox1.BackColor = this.BackColor;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CalcSize();
        }

        private void CalcSize() {
            this.textBox1.Height = this.Height - 2 * _BorderThickness - this.Padding.Top - this.Padding.Bottom;
            this.textBox1.Width= this.Width - 2*_BorderThickness-this.Padding.Left-this.Padding.Right - (this.Height-2*_BorderThickness-this.Padding.Top-this.Padding.Bottom+2);
            int s = (this.Padding.Bottom + this.Padding.Top + this.textBox1.Height)/2;
            int t = this.Padding.Left+this.BorderThickness;

            this.textBox1.Location = new Point(t, s);

        }
    }
}
