using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Design
{
    public class CustomRadioButton : RadioButton
    {
        private Color _f;

        private Color _n;

        private Font _TextFont;

        private bool hover;

        public CustomRadioButton()
        {
            _TextFont = Font;
            _n = _f = Color.White;
            CheckedChanged += CustomRadioButton_CheckedChanged;
            MouseEnter += CustomRadioButton_MouseEnter;
            MouseLeave += CustomRadioButton_MouseLeave;
        }

        public Font TextFont
        {
            get => _TextFont;
            set
            {
                _TextFont = value;
                Invalidate();
            }
        }

        public Color RadioColor
        {
            get => _n;
            set
            {
                _n = value;
                Invalidate();
            }
        }

        public Color HoverRadioColor
        {
            get => _f;
            set
            {
                _f = value;
                Invalidate();
            }
        }

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

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            using (Brush b = new SolidBrush(BackColor), a = new SolidBrush(hover ? _f : _n))
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                pevent.Graphics.FillRectangle(b, new Rectangle(-1, -1, Width + 2, Height + 2));
                pevent.Graphics.DrawEllipse(new Pen(a, 1.6f), new Rectangle(3, (Height - 12) / 2, 12, 12));

                if (Checked)
                    pevent.Graphics.FillEllipse(a, new Rectangle(6, (Height - 6) / 2, 6, 6));
                else
                    pevent.Graphics.FillEllipse(b, new Rectangle(6, (Height - 6) / 2, 6, 6));


                var s = pevent.Graphics.MeasureString(Text, _TextFont).Height;
                pevent.Graphics.DrawString(Text, _TextFont, new SolidBrush(ForeColor),
                    new PointF(18, (Height - s) / 2));

                pevent.Graphics.SmoothingMode = SmoothingMode.Default;
            }
        }
    }
}