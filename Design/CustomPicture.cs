using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Design
{
    public class CustomPicture : System.Windows.Forms.PictureBox
    {
        public CustomPicture() {

        }

        private Color _b;
        private int _bt;

        public Color BorderColor {
            get { return _b; }
            set { _b = value;Invalidate(); }
        }

        public int BorderThickness {
            get { return _bt; }
            set { _bt = value; Invalidate(); }
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
