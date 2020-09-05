using System;
using System.Drawing;
using System.Windows.Forms;

namespace Design
{
    public partial class Separator : UserControl
    {
        private Size _StrokeSize;

        public Separator()
        {
            Height = 3;
            Width = 100;
            StrokeSize = new Size(70, 1);
            InitializeComponent();
            Invalidate();
        }

        public Color Color { get; set; } = Color.White;

        public Size StrokeSize
        {
            get => _StrokeSize;
            set => _StrokeSize = value;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Brush b = new SolidBrush(Color))
            {
                var x = (Width - _StrokeSize.Width) / 2;
                var y = (Height - _StrokeSize.Height) / 2;
                var width = _StrokeSize.Width;
                var height = _StrokeSize.Height;
                e.Graphics.FillRectangle(b, new Rectangle(x, y, width, height));
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}