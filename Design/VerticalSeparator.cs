using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Design
{
    public partial class VerticalSeparator : UserControl
    {
        Color _Color = Color.White;
        Size _StrokeSize;
        public VerticalSeparator()
        {
            this.Height = 100;
            this.Width = 3;
            StrokeSize = new Size(1, 70);
            InitializeComponent();
            Invalidate();
        }

        public Color Color {
            get { return _Color; }
            set { _Color = value; }
        }

        public Size StrokeSize {
            get { return _StrokeSize; }
            set { _StrokeSize = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Brush b = new SolidBrush(_Color)) {
                int x = (this.Width - _StrokeSize.Width) / 2;
                int y = (this.Height - _StrokeSize.Height) / 2;
                int width = _StrokeSize.Width;
                int height = _StrokeSize.Height;
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
