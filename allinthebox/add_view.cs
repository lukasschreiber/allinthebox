using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace allinthebox
{
    public partial class add_view : Form
    {

        int iconStyle;
        public string currentStyle;

        public add_view(Main parent, String scannerData)
        {
            InitializeComponent();
            _parent = parent;
            barcode.Text = scannerData;

            //language
            this.label4.Text = Properties.strings.add;
            this.label1.Text = Properties.strings.barcode;
            this.label2.Text = Properties.strings.name;
            this.label3.Text = Properties.strings.comment;
            this.submit.Text = Properties.strings.itemAddButton;


            //load style
            currentStyle = _parent.loadSettingsDataBase().SelectSingleNode("/settings/style").InnerXml;

            ColorConverter cc = new ColorConverter();

            //load colors and images
            
            if (Style.iconStyle == Style.IconStyle.DARK)
            {
                this.closeButton.Image = allinthebox.Properties.Resources.close_white;
                iconStyle = 0;
            }
            else
            {
                this.closeButton.Image = allinthebox.Properties.Resources.close;
                iconStyle = 1;
            }
        }

        private Main _parent;

        private void submit_Click(object sender, EventArgs e)
        {
            save();
        }

        private void save() {
            String barcode_in = barcode.Text;
            String name_in = item_name.Text;
            String comment_in = "-";

            if (comment.Text == "")
            {
                comment_in = "-";
            }
            else
            {
                comment_in = comment.Text;
            }
            String spath = FileManager.GetDataBasePath(FileManager.NAMES.DATA);

            if (name_in != "" && barcode_in != "")
            {
                XDocument doc = XDocument.Load(spath);
                XElement root = new XElement("item");
                XElement image = new XElement("image", "");
                XElement image2 = new XElement("image2", "");
                XAttribute zoom = new XAttribute("zoom", "2");
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
                this.Hide();
                _parent.refresh(true);
                _parent.select(barcode_in);
            }
            else
            {
                MessageBox.Show(Properties.strings.itemAddErrorEmpty, Properties.strings.error + ":", MessageBoxButtons.OK);
            }
        }

        private void barcode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter) {
                save();
            }
        }

        private void item_name_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                save();
            }
        }

        private void comment_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                save();
            }
        }


        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left)
            {
                this.Close();
            }
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.closeButton, Properties.strings.exit);
            this.closeButton.BackColor = Color.FromArgb(223, 1, 1);
            this.closeButton.Image = allinthebox.Properties.Resources.close_white;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            this.closeButton.BackColor = Color.Transparent;
            if (iconStyle == 1)
            {
                this.closeButton.Image = allinthebox.Properties.Resources.close;
            }
            else
            {
                this.closeButton.Image = allinthebox.Properties.Resources.close_white;
            }
        }

        int mouseX = 0, mouseY = 0;
        bool mousedown, maximized;
        int grabX = 0, grabY = 0;

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row == 0) {
                using (SolidBrush brush = new SolidBrush(Color.Silver))
                    e.Graphics.FillRectangle(brush, e.CellBounds);
            }
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
                save();
        }

        private void bg_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            grabX = (MousePosition.X - this.DesktopLocation.X);
            grabY = (MousePosition.Y - this.DesktopLocation.Y);
        }

        private void bg_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown)
            {
                mouseX = MousePosition.X - grabX;
                mouseY = MousePosition.Y - grabY;
                this.SetDesktopLocation(mouseX, mouseY);
            }
        }

        private void bg_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;

        }
    }
}
