using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using allinthebox.Properties;

namespace allinthebox
{
    public partial class add_view : Form
    {
        private readonly Main _parent;
        public string currentStyle;
        private int grabX, grabY;

        private readonly int iconStyle;
        private bool mousedown, maximized;

        private int mouseX, mouseY;

        public add_view(Main parent, string scannerData)
        {
            InitializeComponent();
            _parent = parent;
            barcode.Text = scannerData;

            //language
            label4.Text = strings.add;
            label1.Text = strings.barcode;
            label2.Text = strings.name;
            label3.Text = strings.comment;
            submit.Text = strings.itemAddButton;


            //load style
            currentStyle = _parent.loadSettingsDataBase().SelectSingleNode("/settings/style").InnerXml;

            var cc = new ColorConverter();

            //load colors and images

            if (Style.iconStyle == Style.IconStyle.DARK)
            {
                closeButton.Image = Resources.close_white;
                iconStyle = 0;
            }
            else
            {
                closeButton.Image = Resources.close;
                iconStyle = 1;
            }
        }


        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                var cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        private void submit_Click(object sender, EventArgs e)
        {
            save();
        }

        private void save()
        {
            var barcode_in = barcode.Text;
            var name_in = item_name.Text;
            var comment_in = "-";

            if (comment.Text == "")
                comment_in = "-";
            else
                comment_in = comment.Text;
            var spath = FileManager.GetDataBasePath(FileManager.NAMES.DATA);

            if (name_in != "" && barcode_in != "")
            {
                var doc = XDocument.Load(spath);
                var root = new XElement("item");
                var image = new XElement("image", "");
                var image2 = new XElement("image2", "");
                var zoom = new XAttribute("zoom", "2");
                image.Add(zoom);
                image2.Add(zoom);
                root.Add(new XElement("itemName", name_in));
                root.Add(new XElement("barcode", barcode_in));
                root.Add(new XElement("status", "1"));
                root.Add(new XElement("comment", comment_in));
                root.Add(image);
                root.Add(image2);
                root.Add(new XElement("regal", ""));
                root.Add(new XElement("user", ""));
                root.Add(new XElement("rate", "5"));
                doc.Element("data").Add(root);
                doc.Save(spath);
                _parent.changed = true;

                //Main aktualisieren
                Hide();
                _parent.refresh(true);
                _parent.select(barcode_in);
            }
            else
            {
                MessageBox.Show(strings.itemAddErrorEmpty, strings.error + ":", MessageBoxButtons.OK);
            }
        }

        private void barcode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter) save();
        }

        private void item_name_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter) save();
        }

        private void comment_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter) save();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            var me = (MouseEventArgs) e;
            if (me.Button == MouseButtons.Left) Close();
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            var tt = new ToolTip();
            tt.SetToolTip(closeButton, strings.exit);
            closeButton.BackColor = Color.FromArgb(223, 1, 1);
            closeButton.Image = Resources.close_white;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            closeButton.BackColor = Color.Transparent;
            if (iconStyle == 1)
                closeButton.Image = Resources.close;
            else
                closeButton.Image = Resources.close_white;
        }

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row == 0)
                using (var brush = new SolidBrush(Color.Silver))
                {
                    e.Graphics.FillRectangle(brush, e.CellBounds);
                }
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                save();
        }

        private void bg_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            grabX = MousePosition.X - DesktopLocation.X;
            grabY = MousePosition.Y - DesktopLocation.Y;
        }

        private void bg_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown)
            {
                mouseX = MousePosition.X - grabX;
                mouseY = MousePosition.Y - grabY;
                SetDesktopLocation(mouseX, mouseY);
            }
        }

        private void bg_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
        }
    }
}