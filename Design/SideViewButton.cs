using System;
using System.Drawing;
using System.Windows.Forms;

namespace Design
{
    public partial class SideViewButton : UserControl
    {
        private Rectangle boundsRect;

        private bool hover;

        public SideViewButton()
        {
            Width = 120;
            Height = 20;
            DoubleBuffered = true;
            InitializeComponent();
            CalcSize();
        }

        public string ButtonText { get; set; }

        public Color HoverColor { get; set; } = Color.Blue;


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CalcSize();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Brush b = new SolidBrush(hover ? HoverColor : BackColor))
            {
                e.Graphics.FillRectangle(b, boundsRect);
                var f = new Font(Font.Name, hover ? Font.Size + .25f : Font.Size, Font.Style);
                var stringSize = new SizeF();
                stringSize = e.Graphics.MeasureString(ButtonText, f);
                e.Graphics.DrawString(ButtonText, Font, new SolidBrush(ForeColor),
                    new PointF((Width - stringSize.Width) / 2, (Height - stringSize.Height) / 2));
            }
        }


        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hover = false;
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            hover = false;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (boundsRect.Contains(e.Location) && Form.ActiveForm == ParentForm)
            {
                hover = true;
                Invalidate();
            }
            else
            {
                hover = false;
                Invalidate();
            }
        }

        private void CalcSize()
        {
            boundsRect = new Rectangle(0, 0, Width, Height);
            Invalidate();
        }
    }
}