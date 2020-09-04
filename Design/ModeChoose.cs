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
    public partial class ModeChoose : UserControl
    {
        private int _BorderThickness = 3;
        private Color _BorderColor = Color.Black;
        private Color _FocusColor = Color.Gray;
        private Color _SelectedColor = Color.Blue;

        private Rectangle OptOneRect, OptTwoRect, OptThreeRect;


        public ModeChoose()
        {
            DoubleBuffered = true;
            this.Height = 25;
            this.Width = 75;
            InitializeComponent();
            CalcSize();
            Invalidate();
        }

        private void CalcSize() {
            int rectWidth, rectHeight;
            rectWidth = rectHeight = (this.Width - 2 * _BorderThickness) / 3;
            this.Height = rectHeight + 2 * _BorderThickness;

            OptOneRect = new Rectangle(_BorderThickness, _BorderThickness, rectWidth, rectHeight);
            OptTwoRect = new Rectangle(_BorderThickness + rectWidth, _BorderThickness, rectWidth, rectHeight);
            OptThreeRect = new Rectangle(_BorderThickness + 2*rectWidth, _BorderThickness, rectWidth, rectHeight);
        }

        public Color BorderColor {
            get { return _BorderColor; }
            set { _BorderColor = value; }
        }

        public int BorderThickness{
            get { return _BorderThickness; }
            set { _BorderThickness = value; }
        }

        public Color FocusColor
        {
            get { return _FocusColor; }
            set { _FocusColor = value; }
        }

        public Color SelectedColor
        {
            get { return _SelectedColor; }
            set { _SelectedColor = value; }
        }

        public HoverState Selected
        {
            get { return selectedState; }
            set { selectedState = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Brush b = new SolidBrush(this.BackColor)) {
                e.Graphics.FillRectangle(b, new Rectangle(0, 0, this.Width, this.Height));
            }

            using (Brush b = new SolidBrush(_BorderColor)) {
                e.Graphics.DrawRectangle(new Pen(b, _BorderThickness), new Rectangle(_BorderThickness/2, _BorderThickness/2, this.Width - _BorderThickness, this.Height - _BorderThickness));
            }

            using (Brush b = new SolidBrush(_FocusColor)) {
                if (hoverState == HoverState.ONE)
                {
                    e.Graphics.FillRectangle(b, OptOneRect);
                }
                else if (hoverState == HoverState.TWO)
                {
                    e.Graphics.FillRectangle(b, OptTwoRect);
                }
                else if (hoverState == HoverState.THREE) {
                    e.Graphics.FillRectangle(b, OptThreeRect);
                }
            }

            using (Brush b = new SolidBrush(_SelectedColor))
            {
                if (selectedState == HoverState.ONE)
                {
                    e.Graphics.FillRectangle(b, OptOneRect);
                }
                else if (selectedState == HoverState.TWO)
                {
                    e.Graphics.FillRectangle(b, OptTwoRect);
                }
                else if (selectedState == HoverState.THREE)
                {
                    e.Graphics.FillRectangle(b, OptThreeRect);
                }
            }

            using (Image img = Resource.barcode) {
                e.Graphics.DrawImage(img, OptOneRect.X, OptOneRect.Y, OptOneRect.Width, OptOneRect.Height);
            }

            using (Image img = Resource.name)
            {
                int r = 12;
                e.Graphics.DrawImage(img, OptTwoRect.X+r/2, OptTwoRect.Y+r/2, OptTwoRect.Width-r, OptTwoRect.Height-r);
            }

            using (Image img = Resource.comment)
            {
                int r = 8;
                e.Graphics.DrawImage(img, OptThreeRect.X+r/2, OptThreeRect.Y+r/2, OptThreeRect.Width-r, OptThreeRect.Height-r);
            }
        }

        public enum HoverState {ONE,TWO,THREE,NONE};
        private HoverState hoverState = HoverState.NONE;
        private HoverState selectedState = HoverState.NONE;

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
            {
                hoverState = HoverState.ONE;
            }
            else if (OptTwoRect.Contains(e.Location))
            {
                hoverState = HoverState.TWO;
            }
            else if (OptThreeRect.Contains(e.Location))
            {
                hoverState = HoverState.THREE;
            }
            else {
                hoverState = HoverState.NONE;
            }

            if (hoverState != _hover) {
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnClick(e);
            HoverState _s;
            _s = selectedState;
            if (OptOneRect.Contains(e.Location))
            {
                selectedState = HoverState.ONE;
            }
            else if (OptTwoRect.Contains(e.Location))
            {
                selectedState = HoverState.TWO;
            }
            else if (OptThreeRect.Contains(e.Location))
            {
                selectedState = HoverState.THREE;
            }
            else
            {
                selectedState = HoverState.NONE;
            }

            if (selectedState != _s)
            {
                Invalidate();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CalcSize();
            Invalidate();
        }
    }
}
