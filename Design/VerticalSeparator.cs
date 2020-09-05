using System;
using System.Drawing;
using System.Windows.Forms;

namespace Design
{
    public partial class VerticalSeparator : UserControl
    {
        private Size _StrokeSize;

        public VerticalSeparator()
        {
            Height = 100;
            Width = 3;
            StrokeSize = new Size(1, 70);
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