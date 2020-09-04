using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Design
{
    public partial class CustomMultiLineText : UserControl
    {
        public CustomMultiLineText()
        {
            InitializeComponent();
            this.richTextBoxC1.ScrollBar = this.customScrollbar1;
            this.richTextBoxC1.parent = this;
            for (int i = 0; i < 100; i++)
            {
                this.richTextBoxC1.Text += Environment.NewLine;
            }
            calcSize();
            calcScrollBar();
            this.richTextBoxC1.GotFocus += RichTextBoxC1_GotFocus;
            this.richTextBoxC1.LostFocus += RichTextBoxC1_LostFocus;
            this.customScrollbar1.GotFocus += CustomScrollbar1_GotFocus;
            this.customScrollbar1.LostFocus += CustomScrollbar1_LostFocus;

        }

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

        private int borderThickness;
        private Color bc;
        private Color fbc;

        public Color BorderColor
        {
            get { return bc; }
            set { bc = value; Invalidate(); }
        }

        public Color FocusedBorderColor
        {
            get { return fbc; }
            set { fbc = value; Invalidate(); }
        }

        public int BorderThickness
        {
            get { return borderThickness; }
            set { borderThickness = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            calcSize();

            if (borderThickness > 0)
            {
                using (Pen bp = new Pen(new SolidBrush(this.richTextBoxC1.Focused ? fbc : bc), borderThickness))
                {
                    e.Graphics.DrawRectangle(bp, new Rectangle(borderThickness / 2, borderThickness / 2, Width - borderThickness, Height - borderThickness));

                }
            }

        }

        public override string Text {
            get { return this.richTextBoxC1.Text; }
            set {
                int a = richTextBoxC1.SelectionStart;
                this.richTextBoxC1.Text = value;
                this.richTextBoxC1.SelectionStart = a;
            }
        }

        private void calcSize() {

            int xa, ya, wa, ha;
            xa = borderThickness + this.Padding.Left;
            ya = borderThickness + this.Padding.Top;
            wa = this.Width - this.borderThickness - 15 - xa;
            ha = this.Height - ya - borderThickness - this.Padding.Bottom;
            this.richTextBoxC1.Size = new Size(wa, ha);
            this.richTextBoxC1.Location = new Point(xa, ya);

            this.customScrollbar1.Location = new Point(this.Width - borderThickness - this.customScrollbar1.Width, borderThickness);
            this.customScrollbar1.Size = new Size(15, this.Height - 2 * borderThickness);
        }

        public void calcScrollBar()
        {
            int textHeight;

            using (Graphics g = richTextBoxC1.CreateGraphics())
            {
                textHeight = TextRenderer.MeasureText(g, "Test", richTextBoxC1.Font).Height;
            }
            this.customScrollbar1.Maximum = (this.richTextBoxC1.Lines.Length - (this.Height / textHeight)) * textHeight;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);


        private void customScrollbar1_Scroll(object sender, EventArgs e)
        {
            int nPos = customScrollbar1.Value;
            nPos <<= 16;
            uint wParam = (uint)SB_THUMBPOSITION | (uint)nPos;
            try
            {
                SendMessage(richTextBoxC1.Handle, WM_VSCROLL, new IntPtr(wParam), IntPtr.Zero);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
            }
        }

        public void resetStyle() {
            int a = richTextBoxC1.SelectionStart;
            richTextBoxC1.SelectAll();
            richTextBoxC1.SelectionColor = Color.Black;
            richTextBoxC1.SelectAll();
            richTextBoxC1.SelectionFont = new Font(this.richTextBoxC1.Font.FontFamily,this.richTextBoxC1.Font.Size, FontStyle.Regular);
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

        public async void save(string url) {
            await Task.Factory.StartNew(() =>
            {
                this.BeginInvoke((Action)delegate {
                    richTextBoxC1.SaveFile(url, RichTextBoxStreamType.RichText);
                });
            });
            
        }

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

        private void richTextBoxC1_TextChanged(object sender, EventArgs e)
        {

            OnTextChanged(e);
        }

        public string FirstLine {
            get { return this.richTextBoxC1.Lines.FirstOrDefault(); }
        }

        static string GetRtfUnicodeEscapedString(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (c == '\\' || c == '{' || c == '}')
                    sb.Append(@"\" + c);
                else if (c <= 0x7f)
                    sb.Append(c);
                else
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
            }
            return sb.ToString();
        }

        private void richTextBoxC1_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
            int caret = richTextBoxC1.SelectionStart;

            if (this.richTextBoxC1.Rtf.Contains("-->"))
            {
                this.richTextBoxC1.Rtf = this.richTextBoxC1.Rtf.Replace("-->", GetRtfUnicodeEscapedString("→"));
            }

            if (e.KeyCode == Keys.Tab)
            {
                richTextBoxC1.SelectionTabs = new int[] { 10, 20 };
                richTextBoxC1.SelectionIndent = 10;
                richTextBoxC1.SelectionBullet = true;
            }
            if (e.KeyCode == Keys.Tab && e.Shift)
            {
                richTextBoxC1.SelectionIndent = 0;
                richTextBoxC1.SelectionBullet = false;

            }
            if (e.KeyCode == Keys.R && e.Control && e.Shift) {
                if (richTextBoxC1.SelectionColor == Color.Red)
                {
                    richTextBoxC1.SelectionColor = Color.Black;
                }
                else {
                    richTextBoxC1.SelectionColor = Color.Red;
                }
            }
            if (e.KeyCode == Keys.G && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionColor == Color.Green)
                {
                    richTextBoxC1.SelectionColor = Color.Black;
                }
                else
                {
                    richTextBoxC1.SelectionColor = Color.Green;
                }
            }
            if (e.KeyCode == Keys.B && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionColor == Color.Blue)
                {
                    richTextBoxC1.SelectionColor = Color.Black;
                }
                else
                {
                    richTextBoxC1.SelectionColor = Color.Blue;
                }
            }
            if (e.KeyCode == Keys.F && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionFont.Bold)
                {
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font,FontStyle.Regular);
                }
                else
                {
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Bold);
                }
            }
            if (e.KeyCode == Keys.K && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionFont.Italic)
                {
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Regular);
                }
                else
                {
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Italic);
                }
            }
            if (e.KeyCode == Keys.D && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionFont.Strikeout)
                {
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Regular);
                }
                else
                {
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Strikeout);
                }
            }
            if (e.KeyCode == Keys.U && e.Control && e.Shift)
            {
                if (richTextBoxC1.SelectionFont.Underline)
                {
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Regular);
                }
                else
                {
                    richTextBoxC1.SelectionFont = new Font(richTextBoxC1.Font, FontStyle.Underline);
                }
            }
            richTextBoxC1.SelectionStart = caret;

        }
    }


    public class RichTextBoxC : RichTextBox
    {

        private CustomScrollbar _s = null;
        private CustomMultiLineText _p = null;

        public RichTextBoxC()
        {
            this.Font = new Font("Century Gothic", 8.25f, FontStyle.Regular);
        }

        public CustomScrollbar ScrollBar
        {
            get { return _s; }
            set { _s = value; }
        }

        public CustomMultiLineText parent
        {
            get { return _p; }
            set { _p = value; }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, ref Point lParam);

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == CustomMultiLineText.WM_VSCROLL || m.Msg == CustomMultiLineText.WM_GETDLGCODE || m.Msg == CustomMultiLineText.WM_MOUSEFIRST)
            {
                Point p = new Point();
                SendMessage(this.Handle, CustomMultiLineText.EM_GETSCROLLPOS, IntPtr.Zero, ref p);
                if (_s != null && _p != null)
                {
                    if (p.Y > (_s.Maximum))
                    {
                        _s.Value = _s.Maximum - _s.GetThumbHeight();
                    }
                    else
                    {
                        _s.Value = p.Y;
                    }

                }
            }
        }
    }
}
