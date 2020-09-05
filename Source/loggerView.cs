using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using allinthebox.Properties;

namespace allinthebox
{
    public partial class loggerView : Form
    {
        private int counter;
        public string currentStyle;
        private readonly List<LogItem> Errors = new List<LogItem>();
        private int grabX, grabY;
        private readonly int iconStyle = 1;


        private readonly List<LogItem> LogItems = new List<LogItem>();
        private bool mousedown, maximized;

        private int mouseX, mouseY;
        private bool refreshing;

        public loggerView()
        {
            InitializeComponent();
            listView1.Items.Clear();
            listView1.View = View.Details;
            listView1.Columns.Add("Name");
            listView1.Columns[0].Width = listView1.Width - 4;
            listView1.HeaderStyle = ColumnHeaderStyle.None;
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


        public void initialize()
        {
            var Log = new XmlDocument();
            var Err = new XmlDocument();


            Log.Load(FileManager.GetDataBasePath(FileManager.NAMES.LOG));

            foreach (XmlNode n in Log.SelectNodes("/log/l"))
            {
                bool b, i;
                string content;
                Color c;

                content = n.InnerXml;
                c = ColorTranslator.FromHtml(n.Attributes["c"].Value);
                b = bool.Parse(n.Attributes["b"].Value);
                i = bool.Parse(n.Attributes["i"].Value);

                LogItems.Add(new LogItem(content, c, b, i));
            }

            Err.Load(FileManager.GetDataBasePath(FileManager.NAMES.ERROR));

            foreach (XmlNode n in Err.SelectNodes("/log/l"))
            {
                var e = new LogItem(n.InnerXml, Color.Red, false, false);

                Errors.Add(e);
            }
        }

        public void finish()
        {
            var doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.LOG));
            doc.Element("log").RemoveNodes();

            var i = 0;

            foreach (var l in LogItems)
            {
                i++;
                if (i > 500) break;
                var e = new XElement("l", l.Content());
                e.SetAttributeValue("c", ColorTranslator.ToHtml(l.Color()));
                e.SetAttributeValue("b", l.isBold());
                e.SetAttributeValue("i", l.isItalic());
                doc.Element("log").Add(e);
            }

            doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.LOG));
        }

        private void refresh(bool refreshNew, bool error)
        {
            if (!refreshing)
            {
                listView1.BeginUpdate();
                refreshing = true;


                if (!error)
                {
                    title.Text = strings.logger + " - " + strings.log;
                    if (refreshNew) listView1.Items.Clear();

                    foreach (var l in LogItems)
                        if (listView1.FindItemWithText(l.Content()) == null)
                        {
                            var item = listView1.Items.Add(l.Content());
                            item.ForeColor = l.Color();
                            if (l.isBold())
                                item.Font = new Font(item.Font, FontStyle.Bold);
                            else
                                item.Font = new Font(item.Font, FontStyle.Regular);

                            if (l.isItalic())
                                item.Font = new Font(item.Font, FontStyle.Italic);
                            else
                                item.Font = new Font(item.Font, FontStyle.Regular);
                        }
                }
                else
                {
                    title.Text = strings.logger + " - " + strings.errorLog;

                    if (refreshNew) listView1.Items.Clear();


                    foreach (var e in Errors)
                        if (listView1.FindItemWithText(e.Content()) == null)
                        {
                            var item = listView1.Items.Insert(0, e.Content());
                            item.ForeColor = e.Color();
                        }
                }

                refreshing = false;
                listView1.EndUpdate();
            }
        }


        public void log(string user, string s, Color c, bool b, bool i)
        {
            var str = "[" + user + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")] " + s;
            LogItems.Insert(0, new LogItem(str, c, b, i));
        }

        public void error(string user, string s, string stack)
        {
            var str = "[" + user + " (" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ")] " + s;
            var doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.ERROR));
            var e = new XElement("l", str);
            e.SetAttributeValue("s", stack);
            doc.Element("log").Add(e);
            doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.ERROR));
            Program.logger.log(user, s, Color.Red);
        }

        public void error(string s, string stack)
        {
            error(strings.system, s, stack);
        }

        public void error(string s)
        {
            error(strings.system, s, "");
        }

        public void log(string user, string s, Color c, bool b)
        {
            log(user, s, c, b, false);
        }

        public void log(string s, Color c, bool b, bool i)
        {
            log(strings.system, s, c, b, false);
        }

        public void log(string user, string s, Color c)
        {
            log(user, s, c, false, false);
        }

        public void log(string s, Color c, bool b)
        {
            log(strings.system, s, c, b, false);
        }

        public void log(string s, Color c)
        {
            log(strings.system, s, c, false, false);
        }


        public void log(string user, string s)
        {
            log(user, s, Color.Black, false, false);
        }

        public void log(string s)
        {
            log(strings.system, s, Color.Black, false, false);
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (counter % 2 == 0)
            {
                if (e.IsSelected) e.Item.Selected = false;
                if (listView1.FocusedItem != null) listView1.FocusedItem.Focused = false;
            }
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && e.Control)
            {
                if (counter % 2 == 0)
                {
                    if (DialogResult.Yes == MessageBox.Show(strings.deleteLog, strings.submit, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning))
                    {
                        var doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.LOG));
                        doc.RemoveNodes();
                        LogItems.Clear();
                        doc.Add(new XElement("log", ""));
                        try
                        {
                            doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.LOG));
                        }
                        catch (Exception exc)
                        {
                            Program.logger.error(strings.logger, exc.Message, exc.StackTrace);
                        }
                        finally
                        {
                            Program.logger.log(strings.logger, strings.logDeleted, Color.Blue);
                            refresh(true, false);
                        }
                    }
                }
                else if (counter % 2 != 0)
                {
                    if (DialogResult.Yes == MessageBox.Show(strings.deleteError, strings.submit,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        var doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.ERROR));
                        doc.RemoveNodes();
                        Errors.Clear();
                        doc.Add(new XElement("log", ""));
                        try
                        {
                            doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.ERROR));
                        }
                        catch (Exception exc)
                        {
                            Program.logger.error(strings.logger, exc.Message, exc.StackTrace);
                        }
                        finally
                        {
                            Program.logger.log(strings.logger, strings.logDeleted, Color.Blue);
                            refresh(true, false);
                            counter++;
                        }
                    }
                }
            }

            if (e.KeyCode == Keys.F2)
            {
                if (counter % 2 == 0)
                    refresh(true, true);
                else
                    refresh(true, false);
                counter++;
            }
        }

        private void loggerView_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
            Main.currentOpenWindows.Remove(Name);
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (counter % 2 != 0)
            {
                var Log = new XmlDocument();
                Log.Load(FileManager.GetDataBasePath(FileManager.NAMES.ERROR));

                foreach (XmlNode n in Log.SelectNodes("/log/l"))
                    if (listView1.SelectedItems[0].Text == n.InnerXml)
                        MessageBox.Show(n.Attributes["s"].Value);
            }
        }

        private void loggerView_VisibleChanged(object sender, EventArgs e)
        {
            refresh(false, false);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            var me = (MouseEventArgs) e;
            if (me.Button == MouseButtons.Left) Close();
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            var tt = new ToolTip();
            tt.SetToolTip(closeButton, strings.close);
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

        private void LoggerView_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(Name);
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


    internal class LogItem
    {
        private readonly bool b;
        private readonly bool i;
        private readonly Color c;

        private readonly string s;

        public LogItem(string s, Color c, bool b, bool i)
        {
            this.s = s;
            this.c = c;
            this.b = b;
            this.i = i;
        }

        public bool isBold()
        {
            return b;
        }

        public bool isItalic()
        {
            return i;
        }

        public string Content()
        {
            return s;
        }

        public Color Color()
        {
            return c;
        }
    }
}