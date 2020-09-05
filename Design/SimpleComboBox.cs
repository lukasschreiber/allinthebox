using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Design
{
    public partial class SimpleComboBox : DropDownControl
    {
        public delegate void SelectedIndexChangedHandler(object sender, EventArgs e);

        private static readonly int dropItemHeight = 22;

        public List<string> Items = new List<string>();


        public SimpleComboBox()
        {
            InitializeComponent();

            InitializeDropDown(drop);

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty
                                                         | BindingFlags.Instance | BindingFlags.NonPublic, null,
                panel1, new object[] {true});
            SetDoubleBuffered(panel1);
            SetDoubleBuffered(drop);
            SetDoubleBuffered(customScrollbar1);

            BackColor = panel1.BackColor;

            CalculateScrollBar();
            panel1.MouseWheel += Panel1_MouseWheel;
            panel1.MouseHover += Panel1_MouseHover;
            OnDropDown += SimpleComboBox_OnDropDown;
            Load += SimpleComboBox_Load;
            ;
        }

        private void SimpleComboBox_Load(object sender, EventArgs e)
        {
            ClearAllItems();

            AddItem("");
        }

        public event SelectedIndexChangedHandler OnSelectedIndexChanged;


        private void SimpleComboBox_OnDropDown(object sender, EventArgs e)
        {
            ClearAllItems();
            foreach (var s in Items) AddItem(s);

            var wconst = 0;
            if (panel1.Controls.Count > 10)
                wconst = 10;
            else
                wconst = panel1.Controls.Count;

            DropDownHeight = wconst * dropItemHeight;
            DropDownWidth = Width;
            RefreshDropDown();
            CalculateScrollBar();
        }


        private void CalculateScrollBar()
        {
            var pt = new Point(panel1.AutoScrollPosition.X, panel1.AutoScrollPosition.Y);
            customScrollbar1.Minimum = 0;
            customScrollbar1.Maximum = (panel1.Controls.Count + 1) * dropItemHeight;
            customScrollbar1.LargeChange = customScrollbar1.Maximum / customScrollbar1.Height + panel1.Height;
            double sc = panel1.Controls.Count == 0
                ? customScrollbar1.Maximum
                : customScrollbar1.Maximum / panel1.Controls.Count + 1;
            customScrollbar1.SmallChange = (int) Math.Round(sc / 2);
            customScrollbar1.Value = Math.Abs(panel1.AutoScrollPosition.Y);
            checkScrollBarNeeded();
        }

        private void checkScrollBarNeeded()
        {
            if (panel1.Controls.Count * dropItemHeight <= panel1.Height) customScrollbar1.Hide();
            else customScrollbar1.Show();
        }

        public void AddItem(string s, Color c)
        {
            new DropDownItem(this, s, c);
        }

        public void AddItem(string s)
        {
            AddItem(s, ForeColor);
        }

        public void ClearAllItems()
        {
            SuspendLayout();
            panel1.Controls.Clear();
            RefreshDropDown();
            ResumeLayout();
        }


        public void ScrollTo(string s)
        {
            try
            {
                var last = (DropDownItem) panel1.Controls[panel1.Controls.Count - 1];
                var c = panel1.Controls.Find(last.TextHolder.Text, true).First();
                panel1.ScrollControlIntoView(c);
                customScrollbar1.Value = Math.Abs(panel1.AutoScrollPosition.Y);

                var d = panel1.Controls.Find(s, true).First();
                panel1.ScrollControlIntoView(d);
                customScrollbar1.Value = Math.Abs(panel1.AutoScrollPosition.Y);
            }
            catch
            {
                panel1.AutoScrollPosition = new Point(0, 0);
                customScrollbar1.Value = Math.Abs(panel1.AutoScrollPosition.Y);
            }
        }

        public void ScrollToBottom()
        {
            if (Controls.Count > 0)
            {
                panel1.ScrollControlIntoView(panel1.Controls[panel1.Controls.Count - 1]);
                customScrollbar1.Value = Math.Abs(panel1.AutoScrollPosition.Y);
            }
        }

        private void Panel1_MouseHover(object sender, EventArgs e)
        {
            panel1.Focus();
        }

        private void Panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            customScrollbar1.Value = Math.Abs(panel1.AutoScrollPosition.Y);
        }

        private void customScrollbar1_Scroll(object sender, EventArgs e)
        {
            panel1.AutoScrollPosition = new Point(0, customScrollbar1.Value);
            Application.DoEvents();
        }

        public static void SetDoubleBuffered(Control c)
        {
            if (SystemInformation.TerminalServerSession)
                return;

            var aProp =
                typeof(Control).GetProperty(
                    "DoubleBuffered",
                    BindingFlags.NonPublic |
                    BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }

        public class DropDownItem : Panel
        {
            private readonly Color c;
            private readonly SimpleComboBox p;
            private readonly string s;

            public Label TextHolder = new Label();

            public DropDownItem(SimpleComboBox _parent, string _s, Color _c)
            {
                p = _parent;
                c = _c;
                s = _s;

                init();
            }

            private void init()
            {
                var font = new Font("Century Gothic", 8.25f, FontStyle.Regular);

                Width = p.Right;
                Height = dropItemHeight;
                Margin = new Padding(0, 0, 0, 0);
                Padding = new Padding(0, 0, 0, 0);
                BorderStyle = BorderStyle.None;

                TextHolder.Margin = new Padding(5, 0, 3, 0);
                TextHolder.Text = s;
                TextHolder.Name = s;
                TextHolder.ForeColor = c;
                TextHolder.TextAlign = ContentAlignment.MiddleLeft;
                TextHolder.Size = new Size(Width, Height);
                TextHolder.Font = font;
                TextHolder.BorderStyle = BorderStyle.None;
                SetDoubleBuffered(this);
                TextHolder.MouseEnter += DropDownItem_MouseHover;
                TextHolder.MouseLeave += TextHolder_MouseLeave;
                TextHolder.Click += TextHolder_Click;
                TextHolder.GotFocus += TextHolder_GotFocus;
                TextHolder.LostFocus += TextHolder_LostFocus;
                Controls.Add(TextHolder);

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
                p.inputText.Text = TextHolder.Text;
                p.inputText.Select(p.inputText.Text.Length, 0);
                if (p.OnSelectedIndexChanged == null) return;
                p.OnSelectedIndexChanged(this, new EventArgs());
                p.CloseDropDown();
            }

            private void TextHolder_MouseLeave(object sender, EventArgs e)
            {
                TextHolder.BackColor = Color.FromArgb(255, 255, 255);
                TextHolder.ForeColor = Color.Black;
            }

            private void DropDownItem_MouseHover(object sender, EventArgs e)
            {
                TextHolder.BackColor = Color.FromArgb(0, 96, 170);
                TextHolder.ForeColor = Color.White;
            }
        }
    }
}