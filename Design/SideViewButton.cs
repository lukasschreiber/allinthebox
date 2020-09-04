using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Transitions;

namespace Design
{
    public partial class SideViewButton : UserControl
    {
        private Color _HoverColor = Color.Blue;
        private string _ButtonText;
        public SideViewButton()
        {
            this.Width = 120;
            this.Height = 20;
            DoubleBuffered = true;
            InitializeComponent();
            CalcSize();
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CalcSize();
        }

        private bool hover = false;
        Rectangle boundsRect;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Brush b = new SolidBrush(hover ? _HoverColor : BackColor)) {
                e.Graphics.FillRectangle(b, boundsRect);
                Font f = new Font(this.Font.Name, hover ? this.Font.Size + .25f : this.Font.Size, this.Font.Style);
                SizeF stringSize = new SizeF();
                stringSize = e.Graphics.MeasureString(this.ButtonText, f);
                e.Graphics.DrawString(this.ButtonText,this.Font,new SolidBrush(this.ForeColor), new PointF((this.Width - stringSize.Width) / 2, (this.Height - stringSize.Height) / 2));

            }

        }

        public string ButtonText {
            get { return _ButtonText; }
            set { _ButtonText = value; }
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
            if (boundsRect.Contains(e.Location) && Form.ActiveForm==this.ParentForm)
            {
                hover = true;
                Invalidate();

            }
            else {
                hover = false;
                Invalidate();

            }
        }

        public Color HoverColor {
            get { return _HoverColor; }
            set { _HoverColor = value; }
        }

        private void CalcSize() {
            boundsRect = new Rectangle(0, 0, Width, Height);
            Invalidate();
        }
    }
}
