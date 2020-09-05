using System.Drawing;
using System.Windows.Forms;

namespace Design
{
    public class CustomPicture : PictureBox
    {
        private Color _b;
        private int _bt;

        public Color BorderColor
        {
            get => _b;
            set
            {
                _b = value;
                Invalidate();
            }
        }

        public int BorderThickness
        {
            get => _bt;
            set
            {
                _bt = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            ControlPaint.DrawBorder(pe.Graphics, pe.ClipRectangle,
                _b, _bt, ButtonBorderStyle.Solid,
                _b, _bt, ButtonBorderStyle.Solid,
                _b, _bt, ButtonBorderStyle.Solid,
                _b, _bt, ButtonBorderStyle.Solid
            );
        }
    }
}