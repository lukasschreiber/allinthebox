using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Design
{
    public class CustomComboBoxNoScroll : ComboBox
    {
        private const int WM_PAINT = 0xF;
        private readonly int buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;


        protected Image DropDownArrow;

        public CustomComboBoxNoScroll()
        {
            BorderColor = Color.DimGray;
            DrawMode = DrawMode.OwnerDrawVariable;
            FlatStyle = FlatStyle.Flat;

            if (SystemInformation.TerminalServerSession)
                return;

            var aProp =
                typeof(Control).GetProperty(
                    "DoubleBuffered",
                    BindingFlags.NonPublic |
                    BindingFlags.Instance);

            aProp.SetValue(this, true, null);

            DropDownClosed += dropDownClosed;
            DropDown += dropDown;
            SelectedIndexChanged += selectedIndexChanged;
        }

        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Design")]
        [Description("DropDown Arrow Graphic")]
        public Image DropDownArrowImage
        {
            get => DropDownArrow;
            set => DropDownArrow = value;
        }


        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "DimGray")]
        public Color BorderColor { get; set; }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT)
                using (var g = Graphics.FromHwnd(Handle))
                {
                    /*
                    using (var p = new Pen(this.BorderColor, 1))
                    {
                        g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                    }*/
                    using (var p = new Pen(BorderColor, 1))
                    {
                        g.DrawRectangle(p, 0, 0, Width - buttonWidth - 1, Height - 1);

                        if (DropDownArrow != null)
                            g.DrawImage(DropDownArrow,
                                new Rectangle(new Point(Width - buttonWidth - 1), new Size(17, Height)));

                        //g.DrawImageUnscaled(Resource.drop, new Point(Width - buttonWidth - 1));
                    }
                }
        }

        private void dropDownClosed(object sender, EventArgs e)
        {
            var _cb = (CustomComboBoxNoScroll) sender;
            BeginInvoke(new Action(() =>
            {
                _cb.Text = "";
                _cb.Select(_cb.Text.Length, _cb.Text.Length);
                _cb.Invalidate();
            }));
        }

        private void selectedIndexChanged(object sender, EventArgs e)
        {
            var _cb = (CustomComboBoxNoScroll) sender;
            BeginInvoke(new Action(() => { _cb.Text = ""; }));
        }

        private void dropDown(object sender, EventArgs e)
        {
        }
    }
}