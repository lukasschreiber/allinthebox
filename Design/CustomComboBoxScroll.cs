using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Design
{
    public class CustomComboBoxScroll : UserControl
    {
        private textBox textBox;
        private Color backColor = Color.White;
        private Color foreColor = Color.Black;
        public Color borderColor = Color.Black;
        public int dropDownIconWidth = 17;


        public CustomComboBoxScroll()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            this.Size = new Size(this.textBox.Width + this.dropDownIconWidth, this.textBox.Height);
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Appearence"), Description("BackColor")]
        public override Color BackColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value;
                this.textBox.BackColor = backColor;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Appearence"), Description("Width of Icon")]
        public int DropDownIconWidth
        {
            get { return this.dropDownIconWidth; }
            set
            {
                this.dropDownIconWidth = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Appearence"), Description("ForeColor")]
        public override Color ForeColor
        {
            get { return this.foreColor; }
            set
            {
                this.foreColor = value;
                this.textBox.ForeColor = foreColor;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Appearence"), Description("BorderColor")]
        public Color BorderColor
        {
            get { return this.borderColor; }
            set
            {
                this.borderColor = value;
                this.textBox.BorderColor = this.borderColor;
                Invalidate();
            }
        }

        private void InitializeComponent()
        {
            this.textBox = new Design.textBox();
            this.SuspendLayout();
            this.MouseClick += new MouseEventHandler((o, a) => ClickRegistered(o,a));
            this.MouseMove += new MouseEventHandler((o, a) => MoveRegistered(o, a));
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox.Location = new System.Drawing.Point(0, 0);
            this.textBox.Margin = new System.Windows.Forms.Padding(0);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(127, 20);
            this.textBox.TabIndex = 0;
            // 
            // CustomComboBoxScroll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox);
            this.Name = "CustomComboBoxScroll";
            this.Size = new System.Drawing.Size(144, 118);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void MoveRegistered(object o, MouseEventArgs a)
        {
            if (a.Location.X < Width && a.Location.X > Width - dropDownIconWidth)
            {
                if (a.Location.Y > 0 && a.Location.Y < this.textBox.Height)
                {
                    this.Cursor = System.Windows.Forms.Cursors.Default;
                }
            }
        }

        private void ClickRegistered(object o, MouseEventArgs a)
        {
            if (a.Button == MouseButtons.Left) {
                if (a.Location.X < Width && a.Location.X > Width - dropDownIconWidth) {
                    if (a.Location.Y > 0 && a.Location.Y < this.textBox.Height) {
                        MessageBox.Show("Drop");
                    }
                }
            }
        }

        private void CustomComboBoxScroll_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void dropDownClicked(object sender, EventArgs e)
        {
            MessageBox.Show("");
        }

        private const int WM_PAINT = 0xF;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT)
            {
                using (var g = Graphics.FromHwnd(Handle))
                {
                    if (Resource.drop != null)
                    {
                        //g.DrawImage(Resource.drop, new Rectangle(new Point(Width - dropDownIconWidth - 1), new Size(dropDownIconWidth, this.textBox.Height)));
                        g.DrawImageUnscaled(Resource.drop, new Point(Width - dropDownIconWidth - 1));

                    }
                }
            }
        }
    }


    class textBox : TextBox {
        public textBox() {
            this.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "textBox";
            this.Size = new System.Drawing.Size(130, 20);
            this.TabIndex = 0;
        }

        public Color BorderColor=Color.Black;
        private const int WM_PAINT = 0xF;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT)
            {
                using (var g = Graphics.FromHwnd(Handle))
                {
                    using (var p = new Pen(this.BorderColor, 1))
                    {
                        g.DrawRectangle(p, 0, 0, Width-1, Height-1);
                    }
                }
            }
        }
    }
}
