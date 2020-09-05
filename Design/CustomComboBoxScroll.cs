using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Design
{
    public class CustomComboBoxScroll : UserControl
    {
        private const int WM_PAINT = 0xF;
        private Color backColor = Color.White;
        public Color borderColor = Color.Black;
        public int dropDownIconWidth = 17;
        private Color foreColor = Color.Black;
        private textBox textBox;


        public CustomComboBoxScroll()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            Size = new Size(textBox.Width + dropDownIconWidth, textBox.Height);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Appearence")]
        [Description("BackColor")]
        public override Color BackColor
        {
            get => backColor;
            set
            {
                backColor = value;
                textBox.BackColor = backColor;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Appearence")]
        [Description("Width of Icon")]
        public int DropDownIconWidth
        {
            get => dropDownIconWidth;
            set
            {
                dropDownIconWidth = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Appearence")]
        [Description("ForeColor")]
        public override Color ForeColor
        {
            get => foreColor;
            set
            {
                foreColor = value;
                textBox.ForeColor = foreColor;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DefaultValue(false)]
        [Category("Appearence")]
        [Description("BorderColor")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                textBox.BorderColor = borderColor;
                Invalidate();
            }
        }

        private void InitializeComponent()
        {
            textBox = new textBox();
            SuspendLayout();
            MouseClick += (o, a) => ClickRegistered(o, a);
            MouseMove += (o, a) => MoveRegistered(o, a);
            // 
            // textBox
            // 
            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left
                                              | AnchorStyles.Right;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Location = new Point(0, 0);
            textBox.Margin = new Padding(0);
            textBox.Name = "textBox";
            textBox.Size = new Size(127, 20);
            textBox.TabIndex = 0;
            // 
            // CustomComboBoxScroll
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(textBox);
            Name = "CustomComboBoxScroll";
            Size = new Size(144, 118);
            ResumeLayout(false);
            PerformLayout();
        }

        private void MoveRegistered(object o, MouseEventArgs a)
        {
            if (a.Location.X < Width && a.Location.X > Width - dropDownIconWidth)
                if (a.Location.Y > 0 && a.Location.Y < textBox.Height)
                    Cursor = Cursors.Default;
        }

        private void ClickRegistered(object o, MouseEventArgs a)
        {
            if (a.Button == MouseButtons.Left)
                if (a.Location.X < Width && a.Location.X > Width - dropDownIconWidth)
                    if (a.Location.Y > 0 && a.Location.Y < textBox.Height)
                        MessageBox.Show("Drop");
        }

        private void CustomComboBoxScroll_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void dropDownClicked(object sender, EventArgs e)
        {
            MessageBox.Show("");
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT)
                using (var g = Graphics.FromHwnd(Handle))
                {
                    if (Resource.drop != null)
                        //g.DrawImage(Resource.drop, new Rectangle(new Point(Width - dropDownIconWidth - 1), new Size(dropDownIconWidth, this.textBox.Height)));
                        g.DrawImageUnscaled(Resource.drop, new Point(Width - dropDownIconWidth - 1));
                }
        }
    }


    internal class textBox : TextBox
    {
        private const int WM_PAINT = 0xF;

        public Color BorderColor = Color.Black;

        public textBox()
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Left
                                      | AnchorStyles.Right;
            BorderStyle = BorderStyle.FixedSingle;
            Location = new Point(0, 0);
            Margin = new Padding(0);
            Name = "textBox";
            Size = new Size(130, 20);
            TabIndex = 0;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT)
                using (var g = Graphics.FromHwnd(Handle))
                {
                    using (var p = new Pen(BorderColor, 1))
                    {
                        g.DrawRectangle(p, 0, 0, Width - 1, Height - 1);
                    }
                }
        }
    }
}