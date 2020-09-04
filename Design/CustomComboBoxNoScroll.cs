using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Diagnostics;


namespace Design {

    public class CustomComboBoxNoScroll : ComboBox {


        protected Image DropDownArrow = null;

        private const int WM_PAINT = 0xF;
        private int buttonWidth = SystemInformation.HorizontalScrollBarArrowWidth;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT)
            {
                using (var g = Graphics.FromHwnd(Handle))
                {
                    /*
                    using (var p = new Pen(this.BorderColor, 1))
                    {
                        g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                    }*/
                    using (var p = new Pen(this.BorderColor, 1))
                    {
                        g.DrawRectangle(p, 0, 0, Width - buttonWidth - 1, Height - 1);

                        if (DropDownArrow != null)
                        {
                            g.DrawImage(DropDownArrow, new Rectangle(new Point(Width - buttonWidth - 1), new Size(17, this.Height)));
                        }

                        //g.DrawImageUnscaled(Resource.drop, new Point(Width - buttonWidth - 1));
                    }
                }
            }
        }

        [Browsable(true), DefaultValue(false), Category("Design"), Description("DropDown Arrow Graphic")]
        public Image DropDownArrowImage
        {
            get { return DropDownArrow; }
            set { DropDownArrow = value; }
        }

        private void dropDownClosed(object sender, EventArgs e)
        {
            CustomComboBoxNoScroll _cb = (CustomComboBoxNoScroll)sender;
            this.BeginInvoke(new Action(() => {
                _cb.Text = "";
                _cb.Select(_cb.Text.Length, _cb.Text.Length);
                _cb.Invalidate();
            }));
        }

        public CustomComboBoxNoScroll()
        {
            BorderColor = Color.DimGray;
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.FlatStyle = FlatStyle.Flat;

            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(this, true, null);

            this.DropDownClosed += new System.EventHandler(this.dropDownClosed);
            this.DropDown += new System.EventHandler(this.dropDown);
            this.SelectedIndexChanged += new System.EventHandler(this.selectedIndexChanged);
        }

        private void selectedIndexChanged(object sender, EventArgs e)
        {
            CustomComboBoxNoScroll _cb = (CustomComboBoxNoScroll)sender;
            this.BeginInvoke(new Action(() => {
                _cb.Text = "";
            }));
        }

        private void dropDown(object sender, EventArgs e)
        {
        }


        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Color), "DimGray")]
        public Color BorderColor { get; set; }
    }
}
