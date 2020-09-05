using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Design
{
    public partial class CustomMultiLineText : UserControl
    {
        public const int WM_VSCROLL = 0x115;
        public const int WM_HSCROLL = 0x114;
        public const int SB_LINEUP = 0;
        public const int SB_LINEDOWN = 1;
        public const int SB_PAGEUP = 2;
        public const int SB_PAGEDOWN = 3;
        public const int SB_THUMBPOSITION = 4;
        public const int SB_THUMBTRACK = 5;
        public const int SB_TOP = 6;
        public const int SB_BOTTOM = 7;
        public const int SB_ENDSCROLL = 8;
        public const int WM_GETDLGCODE = 0x87;
        public const int WM_MOUSEFIRST = 0x200;
        public const int EM_GETSCROLLPOS = 0x4DD;
        public const int EM_SETSCROLLPOS = 0x4DE;
        private Color bc;

        private int borderThickness;
        private Color fbc;

        public CustomMultiLineText()
        {
            InitializeComponent();
            richTextBoxC1.ScrollBar = customScrollbar1;
            richTextBoxC1.parent = this;
            for (var i = 0; i < 100; i++) richTextBoxC1.Text += Environment.NewLine;
            calcSize();
            calcScrollBar();
            richTextBoxC1.GotFocus += RichTextBoxC1_GotFocus;
            richTextBoxC1.LostFocus += RichTextBoxC1_LostFocus;
            customScrollbar1.GotFocus += CustomScrollbar1_GotFocus;
            customScrollbar1.LostFocus += CustomScrollbar1_LostFocus;
        }

        public Color BorderColor
        {
            get => bc;
            set
            {
                bc = value;
                Invalidate();
            }
        }

        public Color FocusedBorderColor
        {
            get => fbc;
            set
            {
                fbc = value;
                Invalidate();
            }
        }

        public int BorderThickness
        {
            get => borderThickness;
            set
            {
                borderThickness = value;
                Invalidate();
            }
        }

        public override string Text
        {
            get => richTextBoxC1.Text;
            set
            {
                var a = richTextBoxC1.SelectionStart;
                richTextBoxC1.Text = value;
                richTextBoxC1.SelectionStart = a;
            }
        }

        public string FirstLine => richTextBoxC1.Lines.FirstOrDefault();

        private void CustomScrollbar1_LostFocus(object sender, EventArgs e)
        {
            OnLostFocus(e);
        }

        private void CustomScrollbar1_GotFocus(object sender, EventArgs e)
        {
            OnGotFocus(e);
        }

        private void RichTextBoxC1_LostFocus(object sender, EventArgs e)
        {
            OnLostFocus(e);
        }

        private void RichTextBoxC1_GotFocus(object sender, EventArgs e)
        {
            OnGotFocus(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            calcSize();

            if (borderThickness > 0)
                using (var bp = new Pen(new SolidBrush(richTextBoxC1.Focused ? fbc : bc), borderThickness))
                {
                    e.Graphics.DrawRectangle(bp,
                        new Rectangle(borderThickness / 2, borderThickness / 2, Width - borderThickness,
                            Height - borderThickness));
                }
        }

        private void calcSize()
        {
            int xa, ya, wa, ha;
            xa = borderThickness + Padding.Left;
            ya = borderThickness + Padding.Top;
            wa = Width - borderThickness - 15 - xa;
            ha = Height - ya - borderThickness - Padding.Bottom;
            richTextBoxC1.Size = new Size(wa, ha);
            richTextBoxC1.Location = new Point(xa, ya);

            customScrollbar1.Location = new Point(Width - borderThickness - customScrollbar1.Width, borderThickness);
            customScrollbar1.Size = new Size(15, Height - 2 * borderThickness);
        }

        public void calcScrollBar()
        {
            int textHeight;

            using (var g = richTextBoxC1.CreateGraphics())
            {
                textHeight = TextRenderer.MeasureText(g, "Test", richTextBoxC1.Font).Height;
            }

            customScrollbar1.Maximum = (richTextBoxC1.Lines.Length - Height / textHeight) * textHeight;
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);


        private void customScrollbar1_Scroll(object sender, EventArgs e)
        {
            var nPos = customScrollbar1.Value;
            nPos <<= 16;
            var wParam = SB_THUMBPOSITION | (uint) nPos;
            try
            {
                SendMessage(richTextBoxC1.Handle, WM_VSCROLL, new IntPtr(wParam), IntPtr.Zero);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
            }
        }

        public void resetStyle()
        {
            var a = richTextBoxC1.SelectionStart;
            richTextBoxC1.SelectAll();
            richTextBoxC1.SelectionColor = Color.Black;
            richTextBoxC1.SelectAll();
            richTextBoxC1.SelectionFont =
                new Font(richTextBoxC1.Font.FontFamily, richTextBoxC1.Font.Size, FontStyle.Regular);
            richTextBoxC1.SelectAll();
            richTextBoxC1.SelectionIndent = 0;
            richTextBoxC1.SelectAll();
            richTextBoxC1.SelectionBullet = false;
            richTextBoxC1.SelectionStart = a;
        }

        public void load(string url)
        {
            resetStyle();
            richTextBoxC1.LoadFile(url, RichTextBoxStreamType.RichText);
        }

        public async void save(string url)
        {
            await Task.Factory.StartNew(() =>
            {
                BeginInvoke((Action) delegate { richTextBoxC1.SaveFile(url, RichTextBoxStreamType.RichText); });
            });
        }

        private void richTextBoxC1_TextChanged(object sender, EventArgs e)
        {
            OnTextChanged(e);
        }

        private static string GetRtfUnicodeEscapedString(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
                if (c == '\\' || c == '{' || c == '}')
                    sb.Append(@"\" + c);
                else if (c <= 0x7f)
                    sb.Append(c);
                else
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
            return sb.ToString();
        }

        private void richTextBoxC1_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
            var caret = richTextBoxC1.SelectionStart;

            if (richTextBoxC1.Rtf.Contains("-->"))
                richTextBoxC1.Rtf = richTextBoxC1.Rtf.Replace("-->", GetRtfUnicodeEscapedString("→"));

            if (e.KeyCode == Keys.Tab)
            {
                richTextBoxC1.SelectionTabs = new[] {10, 20};
                richTextBoxC1.SelectionIndent = 10;
                richTextBoxC1.SelectionBullet = true;
            }

            if (e.KeyCode == Keys.Tab && e.Shift)
            {
                richTextBoxC1.SelectionIndent = 0;
                richTextBoxC1.SelectionBullet = false;
            }

            if (e.KeyCode == Keys.R && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionColor == Color.Red)
                    richTextBoxC1.SelectionColor = Color.Black;
                else
                    richTextBoxC1.SelectionColor = Color.Red;
            }

            if (e.KeyCode == Keys.G && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionColor == Color.Green)
                    richTextBoxC1.SelectionColor = Color.Black;
                else
                    richTextBoxC1.SelectionColor = Color.Green;
            }

            if (e.KeyCode == Keys.B && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionColor == Color.Blue)
                    richTextBoxC1.SelectionColor = Color.Black;
                else
                    richTextBoxC1.SelectionColor = Color.Blue;
            }

            if (e.KeyCode == Keys.F && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionFont.Bold)
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Regular);
                else
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Bold);
            }

            if (e.KeyCode == Keys.K && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionFont.Italic)
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Regular);
                else
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Italic);
            }

            if (e.KeyCode == Keys.D && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionFont.Strikeout)
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Regular);
                else
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Strikeout);
            }

            if (e.KeyCode == Keys.U && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionFont.Underline)
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Regular);
                else
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Underline);
            }

            richTextBoxC1.SelectionStart = caret;
        }
    }


    public class RichTextBoxC : RichTextBox
    {
        public RichTextBoxC()
        {
            Font = new Font("Century Gothic", 8.25f, FontStyle.Regular);
        }

        public CustomScrollbar ScrollBar { get; set; }

        public CustomMultiLineText parent { get; set; }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, ref Point lParam);

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == CustomMultiLineText.WM_VSCROLL || m.Msg == CustomMultiLineText.WM_GETDLGCODE ||
                m.Msg == CustomMultiLineText.WM_MOUSEFIRST)
            {
                var p = new Point();
                SendMessage(Handle, CustomMultiLineText.EM_GETSCROLLPOS, IntPtr.Zero, ref p);
                if (ScrollBar != null && parent != null)
                {
                    if (p.Y > ScrollBar.Maximum)
                        ScrollBar.Value = ScrollBar.Maximum - ScrollBar.GetThumbHeight();
                    else
                        ScrollBar.Value = p.Y;
                }
            }
        }
    }
}