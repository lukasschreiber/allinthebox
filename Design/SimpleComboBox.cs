using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Design
{
    public partial class SimpleComboBox : DropDownControl
    {

        public List<string> Items = new List<string>();

        private static int dropItemHeight = 22;


        public SimpleComboBox()
        {

            InitializeComponent();

            InitializeDropDown(drop);

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty
            | BindingFlags.Instance | BindingFlags.NonPublic, null,
            panel1, new object[] { true });
            SetDoubleBuffered(panel1);
            SetDoubleBuffered(drop);
            SetDoubleBuffered(customScrollbar1);

            this.BackColor = this.panel1.BackColor;

            CalculateScrollBar();
            this.panel1.MouseWheel += Panel1_MouseWheel;
            this.panel1.MouseHover += Panel1_MouseHover;
            this.OnDropDown += SimpleComboBox_OnDropDown;
            this.Load += SimpleComboBox_Load; ;
        }

        private void SimpleComboBox_Load(object sender, EventArgs e)
        {
            this.ClearAllItems();
            
            this.AddItem("");
        }


        public delegate void SelectedIndexChangedHandler(object sender, EventArgs e);
        public event SelectedIndexChangedHandler OnSelectedIndexChanged;

        
        private void SimpleComboBox_OnDropDown(object sender, EventArgs e)
        {
            this.ClearAllItems();
            foreach (String s in Items) {
                this.AddItem(s);
            }

            int wconst = 0;
            if (panel1.Controls.Count > 10)
            {
                wconst = 10;
            }
            else
            {
                wconst = panel1.Controls.Count;
            }

            DropDownHeight = wconst * dropItemHeight;
            DropDownWidth = Width;
            RefreshDropDown();
            CalculateScrollBar();
        }


        private void CalculateScrollBar() {
            Point pt = new Point(this.panel1.AutoScrollPosition.X, this.panel1.AutoScrollPosition.Y);
            this.customScrollbar1.Minimum = 0;
            this.customScrollbar1.Maximum = (this.panel1.Controls.Count+1) * dropItemHeight;
            this.customScrollbar1.LargeChange = customScrollbar1.Maximum / customScrollbar1.Height + this.panel1.Height;
            double sc = this.panel1.Controls.Count == 0 ? customScrollbar1.Maximum : customScrollbar1.Maximum / this.panel1.Controls.Count+1;
            this.customScrollbar1.SmallChange = (int)Math.Round(sc/2);
            this.customScrollbar1.Value = Math.Abs(this.panel1.AutoScrollPosition.Y);
            checkScrollBarNeeded();
        }

        private void checkScrollBarNeeded() {
            if (panel1.Controls.Count * dropItemHeight <= panel1.Height) this.customScrollbar1.Hide();
            else this.customScrollbar1.Show();
        }

        public void AddItem(string s, Color c) {
            new DropDownItem(this, s, c);
          
        }

        public void AddItem(string s) {
            AddItem(s, this.ForeColor);
        }

        public void ClearAllItems() {
            SuspendLayout();
            this.panel1.Controls.Clear();
            this.RefreshDropDown();
            ResumeLayout();
        }


        public void ScrollTo(String s)
        {
            try
            {
                DropDownItem last = (DropDownItem)panel1.Controls[panel1.Controls.Count - 1];
                Control c = panel1.Controls.Find(last.TextHolder.Text, true).First();
                panel1.ScrollControlIntoView(c);
                this.customScrollbar1.Value = Math.Abs(this.panel1.AutoScrollPosition.Y);

                Control d = panel1.Controls.Find(s, true).First();
                panel1.ScrollControlIntoView(d);
                this.customScrollbar1.Value = Math.Abs(this.panel1.AutoScrollPosition.Y);

            }
            catch {
                panel1.AutoScrollPosition = new Point(0, 0);
                this.customScrollbar1.Value = Math.Abs(this.panel1.AutoScrollPosition.Y);
            }
        }

        public void ScrollToBottom() {
            if (Controls.Count > 0) {
                panel1.ScrollControlIntoView(panel1.Controls[panel1.Controls.Count - 1]);
                this.customScrollbar1.Value = Math.Abs(this.panel1.AutoScrollPosition.Y);
            }
        }

        private void Panel1_MouseHover(object sender, EventArgs e)
        {
            this.panel1.Focus();
        }

        private void Panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            this.customScrollbar1.Value = Math.Abs(this.panel1.AutoScrollPosition.Y);
        }

        private void customScrollbar1_Scroll(object sender, EventArgs e)
        {
            
            this.panel1.AutoScrollPosition = new Point(0, this.customScrollbar1.Value);
            Application.DoEvents();
        }

        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }

        public class DropDownItem : Panel {

            public Label TextHolder = new Label();
            SimpleComboBox p;
            string s;
            Color c;

            public DropDownItem(SimpleComboBox _parent, string _s, Color _c) {

                p = _parent;
                c = _c;
                s = _s;

                init();
            }

            private void init() {
                Font font = new Font("Century Gothic", 8.25f, FontStyle.Regular);

                this.Width = p.Right;
                this.Height = dropItemHeight;
                this.Margin = new Padding(0, 0, 0, 0);
                this.Padding = new Padding(0, 0, 0, 0);
                this.BorderStyle = BorderStyle.None;

                TextHolder.Margin = new Padding(5, 0, 3, 0);
                TextHolder.Text = s;
                TextHolder.Name = s;
                TextHolder.ForeColor = c;
                TextHolder.TextAlign = ContentAlignment.MiddleLeft;
                TextHolder.Size = new Size(Width, Height);
                TextHolder.Font = font;
                TextHolder.BorderStyle = BorderStyle.None;
                SimpleComboBox.SetDoubleBuffered(this);
                this.TextHolder.MouseEnter += DropDownItem_MouseHover;
                this.TextHolder.MouseLeave += TextHolder_MouseLeave;
                this.TextHolder.Click += TextHolder_Click;
                this.TextHolder.GotFocus += TextHolder_GotFocus;
                this.TextHolder.LostFocus += TextHolder_LostFocus;
                this.Controls.Add(TextHolder);

                p.panel1.Controls.Add(this);
                
            }

            private void TextHolder_LostFocus(object sender, EventArgs e)
            {
                p.CFocused = false;
            }

            private void TextHolder_GotFocus(object sender, EventArgs e)
            {
                p.CFocused = true;
            }

            private void TextHolder_Click(object sender, EventArgs e)
            {
                p.inputText.Text = this.TextHolder.Text;
                p.inputText.Select(p.inputText.Text.Length, 0);
                if (p.OnSelectedIndexChanged == null) return;
                p.OnSelectedIndexChanged(this, new EventArgs());
                p.CloseDropDown();
            }

            private void TextHolder_MouseLeave(object sender, EventArgs e)
            {
                this.TextHolder.BackColor = Color.FromArgb(255, 255, 255);
                this.TextHolder.ForeColor = Color.Black;
            }

            private void DropDownItem_MouseHover(object sender, EventArgs e)
            {
                this.TextHolder.BackColor = Color.FromArgb(0,96,170);
                this.TextHolder.ForeColor = Color.White;
            }
        }
    }
}
