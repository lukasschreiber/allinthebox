using System;
using System.Drawing;
using System.Windows.Forms;

namespace Design
{
    public partial class ModeChoose : UserControl
    {
        public enum HoverState
        {
            ONE,
            TWO,
            THREE,
            NONE
        }

        private HoverState hoverState = HoverState.NONE;

        private Rectangle OptOneRect, OptTwoRect, OptThreeRect;


        public ModeChoose()
        {
            DoubleBuffered = true;
            Height = 25;
            Width = 75;
            InitializeComponent();
            CalcSize();
            Invalidate();
        }

        public Color BorderColor { get; set; } = Color.Black;

        public int BorderThickness { get; set; } = 3;

        public Color FocusColor { get; set; } = Color.Gray;

        public Color SelectedColor { get; set; } = Color.Blue;

        public HoverState Selected { get; set; } = HoverState.NONE;

        private void CalcSize()
        {
            int rectWidth, rectHeight;
            rectWidth = rectHeight = (Width - 2 * BorderThickness) / 3;
            Height = rectHeight + 2 * BorderThickness;

            OptOneRect = new Rectangle(BorderThickness, BorderThickness, rectWidth, rectHeight);
            OptTwoRect = new Rectangle(BorderThickness + rectWidth, BorderThickness, rectWidth, rectHeight);
            OptThreeRect = new Rectangle(BorderThickness + 2 * rectWidth, BorderThickness, rectWidth, rectHeight);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Brush b = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(b, new Rectangle(0, 0, Width, Height));
            }

            using (Brush b = new SolidBrush(BorderColor))
            {
                e.Graphics.DrawRectangle(new Pen(b, BorderThickness),
                    new Rectangle(BorderThickness / 2, BorderThickness / 2, Width - BorderThickness,
                        Height - BorderThickness));
            }

            using (Brush b = new SolidBrush(FocusColor))
            {
                if (hoverState == HoverState.ONE)
                    e.Graphics.FillRectangle(b, OptOneRect);
                else if (hoverState == HoverState.TWO)
                    e.Graphics.FillRectangle(b, OptTwoRect);
                else if (hoverState == HoverState.THREE) e.Graphics.FillRectangle(b, OptThreeRect);
            }

            using (Brush b = new SolidBrush(SelectedColor))
            {
                if (Selected == HoverState.ONE)
                    e.Graphics.FillRectangle(b, OptOneRect);
                else if (Selected == HoverState.TWO)
                    e.Graphics.FillRectangle(b, OptTwoRect);
                else if (Selected == HoverState.THREE) e.Graphics.FillRectangle(b, OptThreeRect);
            }

            using (Image img = Resource.barcode)
            {
                e.Graphics.DrawImage(img, OptOneRect.X, OptOneRect.Y, OptOneRect.Width, OptOneRect.Height);
            }

            using (Image img = Resource.name)
            {
                var r = 12;
                e.Graphics.DrawImage(img, OptTwoRect.X + r / 2, OptTwoRect.Y + r / 2, OptTwoRect.Width - r,
                    OptTwoRect.Height - r);
            }

            using (Image img = Resource.comment)
            {
                var r = 8;
                e.Graphics.DrawImage(img, OptThreeRect.X + r / 2, OptThreeRect.Y + r / 2, OptThreeRect.Width - r,
                    OptThreeRect.Height - r);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hoverState = HoverState.NONE;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            HoverState _hover;
            _hover = hoverState;
            if (OptOneRect.Contains(e.Location))
                hoverState = HoverState.ONE;
            else if (OptTwoRect.Contains(e.Location))
                hoverState = HoverState.TWO;
            else if (OptThreeRect.Contains(e.Location))
                hoverState = HoverState.THREE;
            else
                hoverState = HoverState.NONE;

            if (hoverState != _hover) Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnClick(e);
            HoverState _s;
            _s = Selected;
            if (OptOneRect.Contains(e.Location))
                Selected = HoverState.ONE;
            else if (OptTwoRect.Contains(e.Location))
                Selected = HoverState.TWO;
            else if (OptThreeRect.Contains(e.Location))
                Selected = HoverState.THREE;
            else
                Selected = HoverState.NONE;

            if (Selected != _s) Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CalcSize();
            Invalidate();
        }
    }
}