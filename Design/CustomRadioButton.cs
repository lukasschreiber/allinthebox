using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Design
{
    public class CustomRadioButton : RadioButton
    {
        public CustomRadioButton()
        {
            _TextFont = this.Font;
            _n = _f = Color.White;
            this.CheckedChanged += CustomRadioButton_CheckedChanged;
            this.MouseEnter += CustomRadioButton_MouseEnter;
            this.MouseLeave += CustomRadioButton_MouseLeave;
        }

        bool hover = false;
        private void CustomRadioButton_MouseLeave(object sender, EventArgs e)
        {
            hover = false;
        }

        private void CustomRadioButton_MouseEnter(object sender, EventArgs e)
        {
            hover = true;
        }

        private void CustomRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        Font _TextFont;
        public Font TextFont{
            get { return _TextFont; }
            set { _TextFont = value;Invalidate(); }
        }

        Color _n;
        public Color RadioColor {
            get { return _n; }
            set { _n = value;Invalidate(); }
        }

        Color _f;
        public Color HoverRadioColor
        {
            get { return _f; }
            set { _f = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            using (Brush b = new SolidBrush(this.BackColor), a = new SolidBrush(hover?_f:_n))
            {
                pevent.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                pevent.Graphics.FillRectangle(b, new Rectangle(-1, -1, this.Width+2, this.Height+2));
                pevent.Graphics.DrawEllipse(new Pen(a, 1.6f), new Rectangle(3, (this.Height - 12) / 2, 12, 12));

                if (this.Checked)
                    pevent.Graphics.FillEllipse(a, new Rectangle(6, (this.Height - 6) / 2, 6, 6));
                else
                    pevent.Graphics.FillEllipse(b, new Rectangle(6, (this.Height - 6) / 2, 6, 6));


                float s = pevent.Graphics.MeasureString(this.Text, this._TextFont).Height;
                pevent.Graphics.DrawString(this.Text, this._TextFont, new SolidBrush(this.ForeColor), new PointF(18,(this.Height-s)/2));

                pevent.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
        }



        
    }
}
