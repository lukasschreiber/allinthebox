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

    public partial class loggerView : Form
    {
        int counter = 0;
        Boolean refreshing = false;
        int iconStyle = 1;
        public string currentStyle;


        List<LogItem> LogItems = new List<LogItem>();
        List<LogItem> Errors = new List<LogItem>();

        public loggerView()
        {
            InitializeComponent();
            this.listView1.Items.Clear();
            this.listView1.View = View.Details;
            this.listView1.Columns.Add("Name");
            this.listView1.Columns[0].Width = this.listView1.Width -4;
            this.listView1.HeaderStyle = ColumnHeaderStyle.None;
        }


        public void initialize() {
            XmlDocument Log = new XmlDocument();
            XmlDocument Err = new XmlDocument();

           
            Log.Load(FileManager.GetDataBasePath(FileManager.NAMES.LOG));

            foreach (XmlNode n in Log.SelectNodes("/log/l"))
            {
                Boolean b, i;
                string content;
                Color c;

                content = n.InnerXml;
                c = ColorTranslator.FromHtml(n.Attributes["c"].Value);
                b = Boolean.Parse(n.Attributes["b"].Value);
                i = Boolean.Parse(n.Attributes["i"].Value);

                LogItems.Add(new LogItem(content, c, b, i));
            }

            Err.Load(FileManager.GetDataBasePath(FileManager.NAMES.ERROR));

            foreach (XmlNode n in Err.SelectNodes("/log/l"))
            {
                LogItem e = new LogItem(n.InnerXml, Color.Red, false, false);

                Errors.Add(e);
            }
        }

        public void finish() {
            XDocument doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.LOG));
            doc.Element("log").RemoveNodes();

            int i = 0;

            foreach (LogItem l in LogItems)
            {
                i++;
                if (i > 500) {
                    break;
                }
                XElement e = new XElement("l", l.Content());
                e.SetAttributeValue("c", ColorTranslator.ToHtml(l.Color()));
                e.SetAttributeValue("b", l.isBold());
                e.SetAttributeValue("i", l.isItalic());
                doc.Element("log").Add(e);
            }
            doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.LOG));
        }

        private void refresh(Boolean refreshNew, Boolean error) {
            if (!refreshing)
            {
                listView1.BeginUpdate();
                refreshing = true;

              
                if (!error)
                {
                    this.title.Text = Properties.strings.logger + " - "  + Properties.strings.log;
                    if (refreshNew)
                    {
                        listView1.Items.Clear();
                    }

                    foreach (LogItem l in LogItems)
                    {

                        if (listView1.FindItemWithText(l.Content()) == null)
                        {
                            ListViewItem item = listView1.Items.Add(l.Content());
                            item.ForeColor = l.Color();
                            if (l.isBold())
                            {
                                item.Font = new Font(item.Font, FontStyle.Bold);
                            }
                            else
                            {
                                item.Font = new Font(item.Font, FontStyle.Regular);
                            }

                            if (l.isItalic())
                            {
                                item.Font = new Font(item.Font, FontStyle.Italic);
                            }
                            else
                            {
                                item.Font = new Font(item.Font, FontStyle.Regular);
                            }
                        }
                    }
                }
                else
                {
                    this.title.Text = Properties.strings.logger + " - " + Properties.strings.errorLog;

                    if (refreshNew)
                    {
                        listView1.Items.Clear();
                    }
                    

                    foreach (LogItem e in Errors)
                    {
                        if (listView1.FindItemWithText(e.Content()) == null)
                        {
                            ListViewItem item = listView1.Items.Insert(0, e.Content());
                            item.ForeColor = e.Color();
                        }
                    }
                }

                refreshing = false;
                listView1.EndUpdate();
            }
        }


        public void log(string user, string s, Color c, Boolean b, Boolean i) {
            string str = "[" + user + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")] " +s ;
            LogItems.Insert(0, new LogItem(str, c, b, i));
        }

        public void error(string user, string s,string stack)
        {
            string str = "[" + user + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")] " + s;
            XDocument doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.ERROR));
            XElement e = new XElement("l", str);
            e.SetAttributeValue("s", stack);
            doc.Element("log").Add(e);
            doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.ERROR));
            Program.logger.log(user, s, Color.Red);
        }

        public void error(string s, string stack) {
            error(Properties.strings.system, s,stack);
        }
        public void error(string s)
        {
            error(Properties.strings.system, s, "");
        }

        public void log(string user, string s, Color c, Boolean b) {
            log(user, s, c, b, false);
        }

        public void log( string s, Color c, Boolean b, Boolean i)
        {
            log(Properties.strings.system, s, c, b, false);
        }

        public void log(string user, string s, Color c)
        {
            log(user, s, c, false, false);
        }

        public void log(string s, Color c, Boolean b)
        {
            log(Properties.strings.system, s, c, b, false);
        }

        public void log(string s, Color c)
        {
            log(Properties.strings.system, s, c, false, false);
        }


        public void log(string user, string s)
        {
            log(user, s, Color.Black, false, false);
        }

        public void log( string s)
        {
            log(Properties.strings.system, s, Color.Black, false, false);
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (counter % 2 == 0)
            {
                if (e.IsSelected) e.Item.Selected = false;
                if (listView1.FocusedItem != null)
                {
                    listView1.FocusedItem.Focused = false;
                }
            }
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && e.Control) {
                if (counter % 2 == 0)
                {

                    if (DialogResult.Yes == MessageBox.Show(Properties.strings.deleteLog, Properties.strings.submit, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        XDocument doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.LOG));
                        doc.RemoveNodes();
                        LogItems.Clear();
                        doc.Add(new XElement("log", ""));
                        try
                        {
                            doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.LOG));
                        }
                        catch (Exception exc)
                        {
                            Program.logger.error(Properties.strings.logger, exc.Message, exc.StackTrace);
                        }
                        finally
                        {
                            Program.logger.log(Properties.strings.logger, Properties.strings.logDeleted, Color.Blue);
                            refresh(true, false);
                        }

                    }
                }
                else if (counter % 2 != 0) {
                    if (DialogResult.Yes == MessageBox.Show(Properties.strings.deleteError, Properties.strings.submit, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        XDocument doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.ERROR));
                        doc.RemoveNodes();
                        Errors.Clear();
                        doc.Add(new XElement("log", ""));
                        try
                        {
                            doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.ERROR));
                        }
                        catch (Exception exc)
                        {
                            Program.logger.error(Properties.strings.logger, exc.Message, exc.StackTrace);
                        }
                        finally
                        {
                            Program.logger.log(Properties.strings.logger, Properties.strings.logDeleted, Color.Blue);
                            refresh(true, false);
                            counter++;
                        }

                    }
                }


            }
            if (e.KeyCode == Keys.F2) {
                if (counter % 2 == 0)
                {
                    refresh(true, true);
                }
                else {
                    refresh(true, false);
                }
                counter++;
            }
        }

        private void loggerView_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
            Main.currentOpenWindows.Remove(this.Name);
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (counter % 2 != 0)
            {
                XmlDocument Log = new XmlDocument();
                Log.Load(FileManager.GetDataBasePath(FileManager.NAMES.ERROR));

                foreach (XmlNode n in Log.SelectNodes("/log/l"))
                {
                    if (listView1.SelectedItems[0].Text == n.InnerXml)
                    {
                        MessageBox.Show(n.Attributes["s"].Value);
                    }
                }
            }
        }

        private void loggerView_VisibleChanged(object sender, EventArgs e)
        {
            refresh(false, false);

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
            tt.SetToolTip(this.closeButton, Properties.strings.close);
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

        private void LoggerView_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(this.Name);
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


    class LogItem {

        private string s;
        private Color c;
        private Boolean b, i;

        public LogItem(string s, Color c, Boolean b, Boolean i) {
            this.s = s;
            this.c = c;
            this.b = b;
            this.i = i;
        }

        public Boolean isBold (){ return b; }
        public Boolean isItalic() { return i; }
        public string Content() { return s; }
        public Color Color() { return c; }

    }
}
