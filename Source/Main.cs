/*
©Lukas Schreiber 2017-2020
 */


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using allinthebox.Properties;
using Bunifu.Framework.UI;
using TaskbarHook;
using Rectangle = System.Drawing.Rectangle;

namespace allinthebox
{
    public partial class Main : Form
    {
        private const int
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17;

        private const int _ = 10;
        private Color buttonColorPrototype;

        private Font buttonFontPrototype;

        private int c;


        private readonly List<DataGridViewRow> DisplayedRows = new List<DataGridViewRow>();
        private int grabX, grabY;
        private Rectangle lastDim;
        private bool mousedown, maximized;

        private int mouseX, mouseY;
        private readonly ToolTip ttMax = new ToolTip();


        public Main()
        {
            DoubleBuffered = true;

            whiteCircuit = Invert(Resources.circuitBoardBackground);

            InitializeComponent();


            //load remoteServer
            remoteServer = new RemoteServer(this);
            remoteServer.RecievedMessage += RemoteServer_RecievedMessage;

            //language
            userDisplay.Text = strings.notLoggedIn;
            resDisplay.Text = strings.noResults;
            barcodeSearch.Text = strings.searchBarcode;
            nameSearch.Text = strings.searchNames;
            commentSearch.Text = strings.searchComments;
            sideViewButton1.ButtonText = strings.regalmanager;
            um.ButtonText = strings.userManager;
            sideViewButton4.ButtonText = strings.MultiAuswahlTool;
            backup.ButtonText = strings.BackupManager;
            sideViewButton6.ButtonText = strings.log;
            joke.Text = strings.joke;
            dataGrid.Columns["name"].HeaderText = strings.name;
            dataGrid.Columns["barcode"].HeaderText = strings.barcode;
            dataGrid.Columns["status"].HeaderText = strings.state;
            dataGrid.Columns["comment"].HeaderText = strings.comment;
            dataGrid.Columns["regal"].HeaderText = strings.regal;
            dataGrid.Columns["rateCol"].HeaderText = strings.rate;
            resultDisplay.Text = strings.noItem;
            barcode_label.Text = strings.barcode + ":";
            label2.Text = strings.name + ":";
            available.Text = strings.available;
            not_available.Text = strings.borrowed;
            label3.Text = strings.comment + ":";
            save_changes.Text = strings.save;
            button2.Text = strings.delete;
            label1.Text = strings.regal + ":";
            label4.Text = strings.rate + ":";
            button_refresh.Text = strings.refresh;
            add.Text = strings.add;
            delete_button.Text = strings.delete;
            button4.Text = strings.borrow;
            contextMenuDataGrid.Items[0].Text = strings.delete;
            contextMenuDataGrid.Items[1].Text = strings.add;
            contextMenuDataGrid.Items[2].Text = strings.refresh;
            contextMenuDataGrid.Items[3].Text = strings.borrow;


            pic_1.setParent(this);
            pic_2.setParent(this);

            um.Visible = false;
            backup.Visible = false;


            applyStyle();

            Maximize();

            BM = new backupManager(this);
            rv = new remote_view(user_loggedin, this);

            applyInitialSettings();

            filterIcon.Image = Resources.filter_stroke;
            currentMode = SearchMode.MARKUP;


            loadImagesToBuffer();
            applyTableColors();

            refresh(false);

            rowHeight = 40;

            long nextBackup = 0;
            var dt = DateTime.ParseExact(settings.SelectSingleNode("/settings/lastUpdate").InnerXml,
                "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            var lastBackup = dt.ToFileTimeUtc();


            if (backupSchedule == strings.annually)
                nextBackup = lastBackup + 31556952000000000;
            else if (backupSchedule == strings.monthly)
                nextBackup = lastBackup + 2592000000000000;
            else if (backupSchedule == strings.weekly)
                nextBackup = lastBackup + 604800000000000;
            else if (backupSchedule == strings.daily) nextBackup = lastBackup;

            if (backupSchedule != strings.never && nextBackup < DateTime
                .ParseExact(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss").Replace('.', '/'), "MM/dd/yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture).ToFileTimeUtc()) backupAsync();

            ActiveControl = dataGrid;

            KeyPreview = true;
            listener.normalize_listener();
            Focus();
            if (dataGrid.RowCount > 0) dataGrid.Rows[0].Selected = true;

            InitTimer();

            label6.Text = "© Lukas Schreiber, 2017-" + DateTime.Now.Year;


            for (var i = 0; i < 6; i++) dataGrid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            var searchInt = int.Parse(settings.SelectSingleNode("/settings/searchDefault").InnerXml);
            switch (searchInt)
            {
                case 0:
                    nameSearch.Select();
                    currentMatrix = SearchMatrix.NAME;
                    break;
                case 1:
                    barcodeSearch.Select();
                    currentMatrix = SearchMatrix.BARCODE;
                    break;
                case 2:
                    commentSearch.Select();
                    currentMatrix = SearchMatrix.REMARK;
                    break;
            }


            XmlNodeChangedEventHandler handler = (sender, e) => changed = true;
            document.NodeChanged += handler;
            document.NodeInserted += handler;
            document.NodeRemoved += handler;

            XmlNodeChangedEventHandler settingsHandler = (sender, e) => settingsChanged = true;
            settings.NodeChanged += settingsHandler;
            settings.NodeInserted += settingsHandler;
            settings.NodeRemoved += settingsHandler;

            XmlNodeChangedEventHandler rmHandler = (sender, e) => rmChanged = true;
            rmDoc.NodeChanged += rmHandler;
            rmDoc.NodeInserted += rmHandler;
            rmDoc.NodeRemoved += rmHandler;

            dataGrid.DoubleBuffered(true);
            loadJoke();

            if (dataGrid.RowCount <= 0)
            {
                showSideView(false);
            }
            else
            {
                showSideView(true);
                foreach (DataGridViewRow r in dataGrid.Rows) r.Height = rowHeight;
            }

            CalculateScrollBar();
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

        private new Rectangle Top => new Rectangle(0, 0, ClientSize.Width, _);
        private new Rectangle Left => new Rectangle(0, 0, _, ClientSize.Height);


        private new Rectangle Bottom => new Rectangle(0, ClientSize.Height - _, ClientSize.Width, _);
        private new Rectangle Right => new Rectangle(ClientSize.Width - _, 0, _, ClientSize.Height);

        private Rectangle TopLeft => new Rectangle(0, 0, _, _);
        private Rectangle TopRight => new Rectangle(ClientSize.Width - _, 0, _, _);
        private Rectangle BottomLeft => new Rectangle(0, ClientSize.Height - _, _, _);
        private Rectangle BottomRight => new Rectangle(ClientSize.Width - _, ClientSize.Height - _, _, _);

        //handle recieved Messages
        /*
         * every TCP request comes in here
         * handle answers here
         * request = e.Message
         */
        private void RemoteServer_RecievedMessage(object sender, ServerEventArgs e)
        {
            var transmissionData = "";

            //only async tasks here
            var msg = e.Message.Split(';');

            if (msg[0].Contains(RemoteServer.CODES.CONNECTED.ToString()))
            {
                /*
                 request: 0;name
                 answer: RemoteServer.CODE
                 */

                Program.logger.log("connected to " + msg[1], Color.Green);
                rv.Invoke(rv.writeLabel, "Verbunden mit " + msg[1]);

                transmissionData = RemoteServer.CODE;
                remoteServer.ANSWER = Encoding.ASCII.GetBytes(transmissionData);
            }

            if (msg[0].Contains(RemoteServer.CODES.BARCODE.ToString()))
            {
                /*
                 * request: 1;barcode
                 * answer: data
                 */


                var b = msg[1];

                if (ActiveBarcodes.Contains(b.Trim()))
                    transmissionData = RemoteServer.CODES.CURRENTLY_IN_USE.ToString();
                else
                    transmissionData = getDataFromXML(b);
                remoteServer.ANSWER = Encoding.UTF8.GetBytes(transmissionData);
            }

            if (msg[0].Contains(RemoteServer.CODES.CANCEL.ToString()))
            {
                /*
                 * request: 2, barcode
                 * answer: 2
                 */

                ActiveBarcodes.Remove(msg[1].Trim());
                Console.WriteLine(ActiveBarcodes.Count + " currently used barcodes");
                transmissionData = RemoteServer.CODES.CANCEL.ToString();
                remoteServer.ANSWER = Encoding.ASCII.GetBytes(transmissionData);
            }

            if (msg[0].Contains(RemoteServer.CODES.SAVE.ToString()))
            {
                /*
                 * request: 3, data
                 * answer: 3
                 */
                ActiveBarcodes.Remove(msg[1].Trim());

                saveDataToXML(msg[1], msg[2], msg[3], msg[4]);

                transmissionData = RemoteServer.CODES.SAVE.ToString();
                remoteServer.ANSWER = Encoding.ASCII.GetBytes(transmissionData);
            }

            if (msg[0].Contains(RemoteServer.CODES.ADD.ToString()))
            {
                /*
                 * request: 4, data
                 * answer 4
                 */

                addItemFromRemote(msg[1], msg[2], msg[3]);

                transmissionData = RemoteServer.CODES.SAVE.ToString();
                remoteServer.ANSWER = Encoding.ASCII.GetBytes(transmissionData);
            }
        }

        //add item from Remote
        private void addItemFromRemote(string barcode, string name, string rat)
        {
            BeginInvoke((Action) async delegate
            {
                document = loadDataBase();
                var doc = document.ToXDocument();

                if (name != "" && barcode != "")
                {
                    var root = new XElement("item");
                    var image = new XElement("image", "");
                    var image2 = new XElement("image2", "");
                    var zoom = new XAttribute("zoom", "2");
                    image.Add(zoom);
                    image2.Add(zoom);
                    root.Add(new XElement("itemName", name));
                    root.Add(new XElement("barcode", barcode));
                    root.Add(new XElement("status", "1"));
                    root.Add(new XElement("comment", "-"));
                    root.Add(image);
                    root.Add(image2);
                    root.Add(new XElement("regal", ""));
                    root.Add(new XElement("user", ""));
                    root.Add(new XElement("rate", rat));
                    doc.Element("data").Add(root);

                    document = doc.ToXmlDocument();
                    await SaveAsync(document, FileManager.NAMES.DATA);

                    saved();
                    refresh(false);
                }
            });
        }

        //save DATA from Remote to XML
        private void saveDataToXML(string barcode, string name, string stat, string rat)
        {
            document = loadDataBase();

            BeginInvoke((Action) async delegate
            {
                var XPathName = "/data/item[barcode='" + barcode + "']/itemName";
                var node_name = document.SelectSingleNode(XPathName);
                node_name.InnerXml = name;

                var XPathRate = "/data/item[barcode='" + barcode + "']/rate";
                var node_rate = document.SelectSingleNode(XPathRate);
                node_rate.InnerXml = rat;

                if (stat == "1")
                {
                    var XPath = "/data/item[barcode='" + barcode + "']/status";
                    var userPath = "/data/item[barcode='" + barcode + "']/user";
                    var node = document.SelectSingleNode(XPath);
                    var user_node = document.SelectSingleNode(userPath);
                    node.InnerXml = "1";
                    user_node.InnerXml = "";
                }
                else
                {
                    var XPath = "/data/item[barcode='" + barcode + "']/status";
                    var userPath = "/data/item[barcode='" + barcode + "']/user";
                    var node = document.SelectSingleNode(XPath);
                    var user_node = document.SelectSingleNode(userPath);
                    node.InnerXml = "0";
                    user_node.InnerXml = user_loggedin;
                }

                await SaveAsync(document, FileManager.NAMES.DATA);

                DisplayedRows.Clear();
                foreach (DataGridViewRow row in dataGrid.Rows)
                    if (row.Displayed)
                        if (!DisplayedRows.Contains(row))
                            DisplayedRows.Add(row);
                refresh(false, true, true);
            });
        }

        //get DATA for Remote from XML
        private string getDataFromXML(string code)
        {
            var a = "";
            document = loadDataBase();

            //handle that barcode is not present

            var node = document.SelectSingleNode("data/item[barcode='" + code.Trim() + "']");

            if (node != null)
            {
                ActiveBarcodes.Add(code.Trim());


                a += node.SelectSingleNode("itemName").InnerText.Trim() + ";";
                a += code.Trim() + ";";
                a += node.SelectSingleNode("status").InnerText.Trim() + ";";
                a += node.SelectSingleNode("comment").InnerText.Replace(Environment.NewLine, " ").Trim() + ";";
                a += node.SelectSingleNode("regal").InnerText.Trim() + ";";
                a += node.SelectSingleNode("rate").InnerText.Trim();
            }
            else
            {
                a = RemoteServer.CODES.NOT_IN_DB + ";" + code.Trim();
            }

            return a;
        }


        private void dataGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var hit = dataGrid.HitTest(e.X, e.Y);
                if (hit.Type == DataGridViewHitTestType.None)
                {
                    dataGrid.ClearSelection();
                    dataGrid.CurrentCell = null;
                }
            }
        }


        private void add_Click(object sender, EventArgs e)
        {
            var main = this;
            var addView = new add_view(main, "");
            addView.Show();
        }

        private void delete_button_Click(object sender, EventArgs e)
        {
            delete();
        }

        public async void delete()
        {
            var selectedRows = 0;
            foreach (DataGridViewRow row in dataGrid.SelectedRows) selectedRows++;

            if (selectedRows > 0)
            {
                if (DialogResult.Yes == MessageBox.Show(strings.deleteLine, strings.submit, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning))
                {
                    var rowNumber = dataGrid.Rows.Count;
                    var curRow = rowNumber;
                    foreach (DataGridViewRow row in dataGrid.SelectedRows)
                    {
                        var temp_barcode = dataGrid.Rows[row.Index].Cells[1].Value + string.Empty;
                        if (ActiveBarcodes.Contains(temp_barcode)) ActiveBarcodes.Remove(temp_barcode);
                        curRow = row.Index;
                        document = loadDataBase();


                        var XPath = "/data/item[barcode='" + temp_barcode + "']";
                        var node = document.SelectSingleNode(XPath);
                        node.ParentNode.RemoveChild(node);
                        await SaveAsync(document, FileManager.NAMES.DATA);
                        dataGrid.Rows.RemoveAt(row.Index);

                        if (File.Exists(FileManager.GetDataBasePath("Comments/" + temp_barcode + ".rtf")))
                            File.Delete(FileManager.GetDataBasePath("Comments/" + temp_barcode + ".rtf"));
                    }

                    if (curRow - 1 >= 0) dataGrid.Rows[curRow - 1].Selected = true;
                    refresh(false);
                }
            }
            else
            {
                MessageBox.Show(strings.noRowSelected, strings.error + ":", MessageBoxButtons.OK);
            }
        }

        public void select(string barcode_select)
        {
            dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (barcode_select != "")
                try
                {
                    foreach (DataGridViewRow row in dataGrid.Rows)
                        if (row.Cells[1].Value.ToString().Equals(barcode_select))
                        {
                            lastSelectedRowIndex = dataGrid.SelectedRows[0].Index;
                            dataGrid.ClearSelection();
                            row.Selected = true;
                            if (row.Index >= 10)
                                dataGrid.FirstDisplayedScrollingRowIndex = row.Index - 10;
                            else
                                dataGrid.FirstDisplayedScrollingRowIndex = 0;
                            loadDataToView(row.Index);
                        }
                }
                catch (Exception exc)
                {
                    Program.logger.error(exc.Message, exc.StackTrace);
                }
        }

        private void save_changes_Click(object sender, EventArgs e)
        {
            saveChanges();
        }

        public async void saveChanges()
        {
            c++;
            await Task.Delay(50).ContinueWith(t =>
            {
                if (barcode_box.Text != "" && name_box.Text != "")
                {
                    var barcode_send = barcode_box.Text;
                    var name_send = name_box.Text;
                    var image_1 = pic_1.FileName;
                    var image_2 = pic_2.FileName;
                    var zoom_1 = Math.Round(pic_1.ZoomValue) + "";
                    var zoom_2 = Math.Round(pic_2.ZoomValue) + "";
                    var comment = "-";
                    var regal = rOption.Text;

                    var user = user_loggedin;
                    var rate = rating.Value + "";
                    var commentUrl = FileManager.GetDataBasePath("Comments/" + barcode_send + ".rtf");

                    BeginInvoke((Action) delegate
                    {
                        if (!comment_box.FirstLine.Equals(""))
                        {
                            comment = comment_box.FirstLine;
                            comment_box.save(commentUrl);
                        }
                        else
                        {
                            comment = "-";
                        }
                    });

                    document = loadDataBase();

                    BeginInvoke((Action) async delegate
                    {
                        var XPathName = "/data/item[barcode='" + barcode_send + "']/itemName";
                        var node_name = document.SelectSingleNode(XPathName);
                        node_name.InnerXml = name_send;

                        if (pic_1.Visible && !pic_1.emptyImg)
                        {
                            var XPathImg = "/data/item[barcode='" + barcode_send + "']/image";
                            var node_img = document.SelectSingleNode(XPathImg);
                            var attr_zoom = node_img.Attributes[0];
                            node_img.InnerXml = image_1;
                            attr_zoom.Value = zoom_1;
                        }

                        if (pic_2.Visible && !pic_2.emptyImg)
                        {
                            var XPathImg2 = "/data/item[barcode='" + barcode_send + "']/image2";
                            var node_img2 = document.SelectSingleNode(XPathImg2);
                            var attr_zoom2 = node_img2.Attributes[0];
                            node_img2.InnerXml = image_2;
                            attr_zoom2.Value = zoom_2;
                        }


                        var XPathCom = "/data/item[barcode='" + barcode_send + "']/comment";
                        var node_com = document.SelectSingleNode(XPathCom);
                        node_com.InnerXml = comment;

                        var XPathReg = "/data/item[barcode='" + barcode_send + "']/regal";
                        var node_reg = document.SelectSingleNode(XPathReg);
                        node_reg.InnerXml = regal;

                        var XPathRate = "/data/item[barcode='" + barcode_send + "']/rate";
                        var node_rate = document.SelectSingleNode(XPathRate);
                        node_rate.InnerXml = rate;

                        if (available.Checked)
                        {
                            var barcode = barcode_box.Text;
                            var XPath = "/data/item[barcode='" + barcode + "']/status";
                            var userPath = "/data/item[barcode='" + barcode + "']/user";
                            var node = document.SelectSingleNode(XPath);
                            var user_node = document.SelectSingleNode(userPath);
                            node.InnerXml = "1";
                            user_node.InnerXml = "";
                        }
                        else
                        {
                            var barcode = barcode_box.Text;
                            var XPath = "/data/item[barcode='" + barcode + "']/status";
                            var userPath = "/data/item[barcode='" + barcode + "']/user";
                            var node = document.SelectSingleNode(XPath);
                            var user_node = document.SelectSingleNode(userPath);
                            node.InnerXml = "0";
                            user_node.InnerXml = user_loggedin;
                        }

                        await SaveAsync(document, FileManager.NAMES.DATA);


                        saved();
                        refresh(false);
                    });
                }
            });
        }


        private void checkSaved()
        {
            if (barcode_box.Text != "")
            {
                var sb = barcode_box.Text;

                document = loadDataBase();


                var regal = document.SelectSingleNode("/data/item[barcode='" + sb + "']/regal").InnerText;
                var name = document.SelectSingleNode("/data/item[barcode='" + sb + "']/itemName").InnerText;

                var regal_new = rOption.Text;
                var name_new = name_box.Text;

                if (regal == regal_new && name == name_new)
                    saved();
                else
                    unsaved();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            delete();
        }

        private void saved()
        {
            save_changes.Iconimage = Resources.check;
        }

        private void unsaved()
        {
            save_changes.Iconimage = Resources.red_cross;
        }

        private void name_box_KeyUp(object sender, KeyEventArgs e)
        {
            resultDisplay.Text = name_box.Text;
            checkSaved();
        }


        private void not_available_Click(object sender, EventArgs e)
        {
            refreshState(false, user_loggedin);
            saveChanges();
        }

        private void available_Click(object sender, EventArgs e)
        {
            refreshState(true, user_loggedin);
            saveChanges();
        }


        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (!customSearchBox1.Focused) listener.barcode_listener(sender, e, this);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (button4.Text.ToLower() == strings.borrow.ToLower())
            {
                refreshState(false, user_loggedin);
                saveChanges();
            }
            else
            {
                refreshState(true, user_loggedin);
                saveChanges();
            }
        }


        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void rm_Click(object sender, EventArgs e)
        {
            var Rm = new rm(this);
            if (!currentOpenWindows.Contains(Rm.Name) || winOpenOnce)
                Rm.Show();
            //roomManager2 manager2 = new roomManager2();
            //manager2.Show();
            else if (currentOpenWindows.Contains(Rm.Name)) Application.OpenForms[Rm.Name].BringToFront();
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == saveHotKey.key && (e.Control || !saveHotKey.controlNeeded) &&
                (e.Alt || !saveHotKey.altNeeded) && (e.Shift || !saveHotKey.shiftNeeded))
            {
                saveChanges();
                saved();
            }

            if (e.KeyCode == borrowHotKey.key && (e.Control || !borrowHotKey.controlNeeded) &&
                (e.Alt || !borrowHotKey.altNeeded) && (e.Shift || !borrowHotKey.shiftNeeded))
            {
                if (button4.Text.ToLower() == strings.borrow.ToLower())
                {
                    not_available.Checked = true;
                    refreshState(false, user_loggedin);
                    saveChanges();
                }
                else
                {
                    available.Checked = true;
                    refreshState(true, user_loggedin);
                    saveChanges();
                }
            }

            if (e.KeyCode == delHotKey.key && (e.Control || !delHotKey.controlNeeded) &&
                (e.Alt || !delHotKey.altNeeded) && (e.Shift || !delHotKey.shiftNeeded)) delete();

            if (e.KeyCode == reloadHotKey.key && (e.Control || !reloadHotKey.controlNeeded) &&
                (e.Alt || !reloadHotKey.altNeeded) && (e.Shift || !reloadHotKey.shiftNeeded)) refresh(true);

            if (e.KeyCode == versionHotKey.key && (e.Control || !versionHotKey.controlNeeded) &&
                (e.Alt || !versionHotKey.altNeeded) && (e.Shift || !versionHotKey.shiftNeeded))
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                MessageBox.Show(version.ToString());
            }

            if (e.KeyCode == helpHotKey.key && (e.Control || !helpHotKey.controlNeeded) &&
                (e.Alt || !helpHotKey.altNeeded) && (e.Shift || !helpHotKey.shiftNeeded))
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var h = new help($"{version}", this);
                if (!currentOpenWindows.Contains(h.Name) || winOpenOnce)
                    h.Show();
                else if (currentOpenWindows.Contains(h.Name)) Application.OpenForms[h.Name].BringToFront();
            }
        }

        private void um_Click(object sender, EventArgs e)
        {
            var um = new userManager(this);
            if (!currentOpenWindows.Contains(um.Name) || winOpenOnce)
                um.Show();
            else if (currentOpenWindows.Contains(um.Name)) Application.OpenForms[um.Name].BringToFront();
        }

        private void backup_Click(object sender, EventArgs e)
        {
            if (!currentOpenWindows.Contains(BM.Name) || winOpenOnce)
                BM.Show();
            else if (currentOpenWindows.Contains(BM.Name)) Application.OpenForms[BM.Name].BringToFront();
        }

        public async Task backupData()
        {
            await Task.Factory.StartNew(delegate
            {
                var folderName = DateTime.Now.ToString("dd.MM.yyyy (HH-mm-ss)");
                Directory.CreateDirectory(FileManager.GetDataBasePath("Backup\\" + folderName));
                File.Copy(FileManager.GetDataBasePath(FileManager.NAMES.DATA),
                    FileManager.GetDataBasePath("Backup\\" + folderName + "\\Data.xml"), true);
                File.Copy(FileManager.GetDataBasePath(FileManager.NAMES.RACKS),
                    FileManager.GetDataBasePath("Backup\\" + folderName + "\\Racks.xml"), true);
                File.Copy(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS),
                    FileManager.GetDataBasePath("Backup\\" + folderName + "\\Settings.xml"), true);

                settings = loadSettingsDataBase();

                var date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                settings.SelectSingleNode("/settings/lastUpdate").InnerXml = date.Replace('.', '/');
            });
            await SaveAsync(settings, FileManager.NAMES.SETTINGS);

            BM.refreshUI();
            Program.logger.log("BackupManager", "Backup saved", Color.Green);
        }

        private void logBut_Click(object sender, EventArgs e)
        {
            if (!currentOpenWindows.Contains(Program.logger.Name) || winOpenOnce)
                Program.logger.Show();
            else if (currentOpenWindows.Contains(Program.logger.Name))
                Application.OpenForms[Program.logger.Name].BringToFront();
        }

        private void dataGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) contextMenuDataGrid.Show(Cursor.Position);
        }


        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            remoteServer.StopServer();
            Program.logger.log(strings.systemClose + " [" + e.CloseReason + "]", Color.Green);
            Program.logger.log("******************************************", Color.Gray);
            Program.logger.finish();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            var info_Copy = new info_copy(this);
            if (!currentOpenWindows.Contains(info_Copy.Name) || winOpenOnce) info_Copy.Show();
        }


        private void sync_Click(object sender, EventArgs e)
        {
            var Client = new WebClient();
            Client.Headers.Add("Content-Type", "binary/octet-stream");

            var result = Client.UploadFile("https://polymation.tk/allinthebox/reciever.php", "POST",
                FileManager.GetDataBasePath(FileManager.NAMES.DATA));

            var s = Encoding.UTF8.GetString(result, 0, result.Length);
            MessageBox.Show(strings.refreshedFile);
        }

        private void contextMenuDataGrid_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var tsc = e;

            if (tsc.ClickedItem.Text.Equals(strings.menuDeleteRow))
            {
                contextMenuDataGrid.Hide();
                delete();
            }
            else if (tsc.ClickedItem.Text.Equals(strings.add))
            {
                var av = new add_view(this, "");
                av.Show();
            }
            else if (tsc.ClickedItem.Text.Equals(strings.refresh))
            {
                refresh(true);
            }
            else if (tsc.ClickedItem.Text.Equals(strings.menuBorrow))
            {
                if (button4.Text.ToLower() == "ausleihen")
                {
                    not_available.Checked = true;
                    refreshState(false, user_loggedin);
                    saveChanges();
                }
                else
                {
                    available.Checked = true;
                    refreshState(true, user_loggedin);
                    saveChanges();
                }
            }
        }


        private void settingsImg_Click(object sender, EventArgs e)
        {
            settings s = null;

            var isAminDOc = new XmlDocument();
            isAminDOc.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));

            var isAdminNode = isAminDOc.SelectSingleNode("/data/user[userName='" + user_loggedin + "']/admin");

            if (isAdminNode.InnerXml.ToLower().Equals("true"))
                s = new settings(true, this);
            else if (isAdminNode.InnerXml.ToLower().Equals("false")) s = new settings(false, this);
            if (!currentOpenWindows.Contains(s.Name) || winOpenOnce)
                s.Show();
            else if (currentOpenWindows.Contains(s.Name)) Application.OpenForms[s.Name].BringToFront();
        }

        private void multiBorrow_Click(object sender, EventArgs e)
        {
            var mb = new multiBorrow(this);
            if (!currentOpenWindows.Contains(mb.Name) || winOpenOnce)
                mb.Show();
            else if (currentOpenWindows.Contains(mb.Name)) Application.OpenForms[mb.Name].BringToFront();
        }

        private XmlDocument loadRoomManagerDataBase()
        {
            if (rmChanged)
            {
                rmDoc.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));
                Program.logger.log("DatabaseManager", "Lade Racks.xml", Color.DarkMagenta);
                rmChanged = false;
            }

            return rmDoc;
        }

        public XmlDocument loadDataBase()
        {
            if (changed)
            {
                document.Load(FileManager.GetDataBasePath(FileManager.NAMES.DATA));
                Program.logger.log("DatabaseManager", "Lade Data.xml", Color.DarkMagenta);
                changed = false;
            }

            return document;
        }

        private void header_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (maximized)
                Normalize();
            else
                Maximize();
        }

        private void header_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown && maximized)
            {
                grabX = MousePosition.X - DesktopLocation.X;
                grabY = MousePosition.Y - DesktopLocation.Y;
                Size = new Size(MinimumSize.Width, MinimumSize.Height);
                SetBounds(MousePosition.X - Width / 2, MousePosition.Y - 5, MinimumSize.Width, MinimumSize.Height);
                maximized = false;
                if (Style.iconStyle == Style.IconStyle.LIGHT)
                    maximizeButton.Image = Resources.maximize;
                else
                    maximizeButton.Image = Resources.maximize_white;
                maximizeButton.Padding = new Padding(4, 4, 4, 4);
            }

            if (mousedown && !maximized)
            {
                mouseX = MousePosition.X - grabX;
                mouseY = MousePosition.Y - grabY;
                SetDesktopLocation(mouseX, mouseY);
            }
        }

        private void header_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
        }

        private void header_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            grabX = MousePosition.X - DesktopLocation.X;
            grabY = MousePosition.Y - DesktopLocation.Y;
        }

        private void Normalize()
        {
            maximized = false;
            if (Style.iconStyle == Style.IconStyle.LIGHT)
                maximizeButton.Image = Resources.maximize;
            else
                maximizeButton.Image = Resources.maximize_white;
            maximizeButton.Padding = new Padding(4, 4, 4, 4);
            SetBounds(lastDim.X, lastDim.Y, lastDim.Width, lastDim.Height);
        }

        private void Maximize()
        {
            maximized = true;
            if (Style.iconStyle == Style.IconStyle.LIGHT)
                maximizeButton.Image = Resources.restore;
            else
                maximizeButton.Image = Resources.restore_white;
            maximizeButton.Padding = new Padding(5, 5, 5, 5);
            lastDim = new Rectangle(DesktopLocation.X, DesktopLocation.Y, Width, Height);

            var screenX = Screen.FromControl(this).Bounds.X;
            var screenY = Screen.FromControl(this).Bounds.Y;
            var screenWidth = Screen.FromControl(this).Bounds.Width;
            var screenHeight = Screen.FromControl(this).Bounds.Height;
            var TaskBarHeight = 0;
            var screen = new Rectangle();

            if (GetTaskBarLocation() == TaskBarLocation.BOTTOM)
            {
                TaskBarHeight = TaskBarFactory.GetTaskbar().Rectangle.Bottom -
                                TaskBarFactory.GetTaskbar().Rectangle.Top;
                screen = new Rectangle(screenX, screenY, screenWidth, screenHeight - TaskBarHeight);
            }
            else if (GetTaskBarLocation() == TaskBarLocation.LEFT)
            {
                TaskBarHeight = TaskBarFactory.GetTaskbar().Rectangle.Right;
                screen = new Rectangle(screenX, screenY, screenWidth - TaskBarHeight, screenHeight);
            }
            else if (GetTaskBarLocation() == TaskBarLocation.RIGHT)
            {
                TaskBarHeight = TaskBarFactory.GetTaskbar().Rectangle.Left -
                                TaskBarFactory.GetTaskbar().Rectangle.Right;
                screen = new Rectangle(screenX, screenY, screenWidth - TaskBarHeight, screenHeight);
            }
            else if (GetTaskBarLocation() == TaskBarLocation.TOP)
            {
                TaskBarHeight = TaskBarFactory.GetTaskbar().Rectangle.Bottom;
                screen = new Rectangle(screenX, screenY, screenWidth, screenHeight - TaskBarHeight);
            }

            SetBounds(screen.X, screen.Y, screen.Width, screen.Height);
        }

        private TaskBarLocation GetTaskBarLocation()
        {
            var taskBarLocation = TaskBarLocation.BOTTOM;
            var taskBarOnTopOrBottom = Screen.PrimaryScreen.WorkingArea.Width == Screen.PrimaryScreen.Bounds.Width;
            if (taskBarOnTopOrBottom)
            {
                if (Screen.PrimaryScreen.WorkingArea.Top > 0) taskBarLocation = TaskBarLocation.TOP;
            }
            else
            {
                if (Screen.PrimaryScreen.WorkingArea.Left > 0)
                    taskBarLocation = TaskBarLocation.LEFT;
                else
                    taskBarLocation = TaskBarLocation.RIGHT;
            }

            return taskBarLocation;
        }

        private void CalculateScrollBar()
        {
            int DisplayedRows;
            var rowCount = 0;
            foreach (DataGridViewRow row in dataGrid.Rows)
                if (row.Visible)
                    rowCount++;
            if (rowCount > 0)
            {
                double a = (dataGrid.Height - dataGrid.ColumnHeadersHeight) / rowHeight;
                DisplayedRows = dataGrid.DisplayedRowCount(false) > 0
                    ? dataGrid.DisplayedRowCount(false)
                    : (int) Math.Floor(a);
                if (DisplayedRows >= rowCount) DisplayedRows = rowCount;
            }
            else
            {
                DisplayedRows = 0;
            }

            customScrollbar1.Value = 0;
            customScrollbar1.Minimum = 0;
            customScrollbar1.LargeChange = 5;
            customScrollbar1.ExtraSmallChange = 5;
            customScrollbar1.SmallChange = customScrollbar1.ExtraSmallChange * 7;
            customScrollbar1.Maximum = rowCount - DisplayedRows + customScrollbar1.LargeChange;

            customScrollbar1.Value = Math.Abs(dataGrid.FirstDisplayedScrollingRowIndex);
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            header.Width = Width + 5;
            footer.Width = Width - sidebar.Width + 5;
            info.Location = new Point(Width - info.Width + 5, header.Height);
            info.Height = Height - (footer.Height + header.Height);

            dataGrid.Width = Width - (sidebar.Width + info.Width) + 5;
            dataGrid.Height = Height - (header.Height + footer.Height + 50);
            dataGrid.Location = new Point(sidebar.Width, header.Height);

            quickButton.Height = Height - (footer.Height + dataGrid.Height + header.Height);
            quickButton.Width = Width - (sidebar.Width + info.Width) + 5;
            quickButton.Location = new Point(sidebar.Width, header.Height + dataGrid.Height);

            footer.Location = new Point(sidebar.Width, header.Height + dataGrid.Height + quickButton.Height);

            scrollHolder.Width = SystemInformation.VerticalScrollBarWidth;
            scrollHolder.Height = dataGrid.Height;
            customScrollbar1.Height = scrollHolder.Height;
            scrollHolder.Location = new Point(sidebar.Width + dataGrid.Width - scrollHolder.Width, header.Height);

            barcode_box.Location = new Point(barcode_label.Location.X + barcode_label.Width, barcode_label.Location.Y);

            sidebar.Invalidate();
            dataGrid.Invalidate();

            var b = 0;
            closeButton.Location = new Point(Width - closeButton.Width + b, closeButton.Location.Y);
            maximizeButton.Location = new Point(Width - 2 * closeButton.Width + b, maximizeButton.Location.Y);
            minimizeButton.Location = new Point(Width - 3 * closeButton.Width + b, minimizeButton.Location.Y);
            helpButton.Location = new Point(Width - 4 * closeButton.Width + b, helpButton.Location.Y);

            separator1.StrokeSize = separator2.StrokeSize = new Size(customSearchBox1.Width, 1);
            separator3.StrokeSize = separator4.StrokeSize = separator8.StrokeSize = separator5.StrokeSize =
                separator6.StrokeSize = separator7.StrokeSize = new Size(name_box.Width, 1);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            var me = (MouseEventArgs) e;
            if (me.Button == MouseButtons.Left) Close();
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            var tt = new ToolTip();
            tt.ShowAlways = true;
            tt.ShowAlways = true;
            tt.SetToolTip(closeButton, strings.close);
            closeButton.BackColor = Color.FromArgb(223, 1, 1);
            closeButton.Image = Resources.close_white;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            closeButton.BackColor = Color.Transparent;
            if (Style.iconStyle == Style.IconStyle.LIGHT)
                closeButton.Image = Resources.close;
            else
                closeButton.Image = Resources.close_white;
        }

        private void maximizeButton_Click(object sender, EventArgs e)
        {
            var me = (MouseEventArgs) e;
            if (me.Button == MouseButtons.Left)
            {
                if (maximized)
                    Normalize();
                else
                    Maximize();
            }
        }

        private void maximizeButton_MouseEnter(object sender, EventArgs e)
        {
            if (maximized)
                ttMax.SetToolTip(maximizeButton, strings.windowNormal);
            else
                ttMax.SetToolTip(maximizeButton, strings.windowMaximize);

            maximizeButton.BackColor = highlightButton;
        }

        private void maximizeButton_MouseLeave(object sender, EventArgs e)
        {
            ttMax.RemoveAll();
            maximizeButton.BackColor = Color.Transparent;
        }

        private void minimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void minimizeButton_MouseEnter(object sender, EventArgs e)
        {
            var tt = new ToolTip();
            tt.ShowAlways = true;
            tt.SetToolTip(minimizeButton, strings.windowMinimize);
            minimizeButton.BackColor = highlightButton;
        }

        private void minimizeButton_MouseLeave(object sender, EventArgs e)
        {
            minimizeButton.BackColor = Color.Transparent;
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var h = new help($"{version}", this);
            if (!currentOpenWindows.Contains(h.Name) || winOpenOnce)
                h.Show();
            else if (currentOpenWindows.Contains(h.Name)) Application.OpenForms[h.Name].BringToFront();
        }

        private void helpButton_MouseEnter(object sender, EventArgs e)
        {
            var tt = new ToolTip();
            tt.ShowAlways = true;
            tt.ShowAlways = true;
            tt.SetToolTip(helpButton, strings.help);
            helpButton.BackColor = Color.FromArgb(0, 96, 170);
            helpButton.Image = Resources.help_white;
        }

        private void helpButton_MouseLeave(object sender, EventArgs e)
        {
            helpButton.BackColor = Color.Transparent;
            if (Style.iconStyle == Style.IconStyle.LIGHT)
                helpButton.Image = Resources.help_black;
            else
                helpButton.Image = Resources.help_white;
        }

        private void label6_MouseEnter(object sender, EventArgs e)
        {
            var cc = new ColorConverter();
            label6.ForeColor = (Color) cc.ConvertFromString("#0060AA");
            label6.Font = new Font(label6.Font, FontStyle.Underline);
        }

        private void label6_MouseLeave(object sender, EventArgs e)
        {
            label6.ForeColor = Color.Black;
            label6.Font = new Font(label6.Font, FontStyle.Regular);
        }

        private void header_Paint(object sender, PaintEventArgs e)
        {
            using (Brush b = new SolidBrush(Multiply(sidebar.BackColor, header.BackColor)))
            {
                var rect = new Rectangle(0, 0, sidebar.Width + 4, header.Height);
                e.Graphics.FillRectangle(b, rect);
            }
        }


        private void sidebar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(sidebar.BackColor), sidebar.Bounds);
            e.Graphics.Clear(sidebar.BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            var img = Style.iconStyle == Style.IconStyle.DARK ? whiteCircuit : Resources.circuitBoardBackground;
            try
            {
                float nWidth = sidebar.Width + 40;
                float ratio = img.Width / img.Height;
                var nHeight = nWidth * ratio;
                e.Graphics.DrawImage(img, -20, sidebar.Height - nHeight + 80, nWidth, nHeight);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
            }
        }

        private Image Invert(Image img)
        {
            var pic = new Bitmap(img);
            for (var y = 0; y <= pic.Height - 1; y++)
            for (var x = 0; x <= pic.Width - 1; x++)
            {
                var inv = pic.GetPixel(x, y);
                inv = Color.FromArgb(inv.A, 255 - inv.R, 255 - inv.G, 255 - inv.B);
                pic.SetPixel(x, y, inv);
            }

            return pic;
        }

        private void resort(bool keepViewport)
        {
            var distance = 0;

            var selectedRow = dataGrid.SelectedRows[0].Index;

            if (!keepViewport)
            {
                try
                {
                    if (DisplayedRows.Count == 0 || DisplayedRows.Contains(dataGrid.Rows[selectedRow]))
                        distance = lastSelectedRowIndex - dataGrid.FirstDisplayedCell.RowIndex;
                    else
                        distance = selectedRow < DisplayedRows.FirstOrDefault().Index ? DisplayedRows.Count - 2 :
                            selectedRow > DisplayedRows.LastOrDefault().Index ? 1 : 0;
                }
                catch (NullReferenceException exc)
                {
                    Program.logger.error(exc.Message);
                    distance = 0;
                }

                if (distance <= selectedRow && selectedRow - distance > 0)
                    try
                    {
                        dataGrid.FirstDisplayedScrollingRowIndex = selectedRow - distance;
                    }
                    catch (Exception e)
                    {
                        Program.logger.log(e.Message, e.StackTrace);
                        dataGrid.FirstDisplayedScrollingRowIndex = 0;
                    }
                else
                    dataGrid.FirstDisplayedScrollingRowIndex = 0;


                dataGrid.Rows[selectedRow].Selected = true;
                lastSelectedRowIndex = selectedRow;
            }
            else
            {
                dataGrid.FirstDisplayedScrollingRowIndex = lastFirstDisplayedRowIndex;
                dataGrid.Rows[selectedRow].Selected = true;
                lastSelectedRowIndex = selectedRow;
            }


            DisplayedRows.Clear();
            foreach (DataGridViewRow row in dataGrid.Rows)
                if (row.Displayed)
                    if (!DisplayedRows.Contains(row))
                        DisplayedRows.Add(row);


            curNum.Text = selectedRow + 1 + " / " + dataGrid.RowCount;
        }

        private void dataGrid_Sorted(object sender, EventArgs e)
        {
            applyTableColors();
            resort(false);
        }

        private void dataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            customScrollbar1.Value = e.NewValue;
            DisplayedRows.Clear();
            foreach (DataGridViewRow row in dataGrid.Rows)
                if (row.Displayed)
                    if (!DisplayedRows.Contains(row))
                        DisplayedRows.Add(row);
        }

        private void customScrollbar1_Scroll(object sender, EventArgs e)
        {
            if (dataGrid.RowCount > 0)
                dataGrid.FirstDisplayedScrollingRowIndex = customScrollbar1.Value < 0 ? 0 :
                    customScrollbar1.Value > customScrollbar1.Maximum ? customScrollbar1.Maximum :
                    customScrollbar1.Value;
        }

        private void dataGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGrid.Rows[e.RowIndex].Height = rowHeight;
            CalculateScrollBar();
            customScrollbar1.Invalidate();
        }

        private void dataGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CalculateScrollBar();
            customScrollbar1.Invalidate();
        }

        private void dataGrid_Paint(object sender, PaintEventArgs e)
        {
            var c = new ColorConverter();
            using (var p = new Pen(new SolidBrush((Color) c.ConvertFromString("#A6A6A6")), 1))
            {
                //e.Graphics.DrawLine(p, new Point(this.dataGrid.Width - SystemInformation.VerticalScrollBarWidth-1, 0), new Point(this.dataGrid.Width-SystemInformation.VerticalScrollBarWidth - 1, dataGrid.Height));
            }
        }


        private void rating_Click(object sender, MouseEventArgs e)
        {
            saveChanges();
        }

        private string ConvertRtfToText(string rtf)
        {
            using (var rtb = new RichTextBox())
            {
                rtb.Rtf = rtf;
                return rtb.Text;
            }
        }

        private void dataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (dataGrid.Columns["comment"].Index == e.ColumnIndex && e.RowIndex >= 0)
                if (File.Exists(FileManager.GetDataBasePath("Comments/" + dataGrid.Rows[e.RowIndex].Cells[1].Value +
                                                            string.Empty + ".rtf")))
                    if (ConvertRtfToText(File.ReadAllText(FileManager.GetDataBasePath("Comments/" +
                        dataGrid.Rows[e.RowIndex].Cells[1].Value + string.Empty + ".rtf"))) != e.Value + "")
                    {
                        using (
                            Brush gridBrush = new SolidBrush(dataGrid.GridColor),
                            backColorBrush = (dataGrid.SelectedRows.Count > 0
                                ? dataGrid.SelectedRows[0].Index == e.RowIndex
                                : false)
                                ? new SolidBrush(e.CellStyle.SelectionBackColor)
                                : new SolidBrush(e.CellStyle.BackColor),
                            textColorBrush = (dataGrid.SelectedRows.Count > 0
                                ? dataGrid.SelectedRows[0].Index == e.RowIndex
                                : false)
                                ? new SolidBrush(e.CellStyle.SelectionForeColor)
                                : new SolidBrush(e.CellStyle.ForeColor)
                        )
                        {
                            using (var gridLinePen = new Pen(gridBrush))
                            {
                                e.Graphics.FillRectangle(backColorBrush, e.CellBounds);

                                if (dataGrid.BorderStyle != BorderStyle.None)
                                {
                                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                        e.CellBounds.Bottom - 1, e.CellBounds.Right - 1,
                                        e.CellBounds.Bottom - 1);
                                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1,
                                        e.CellBounds.Top, e.CellBounds.Right - 1,
                                        e.CellBounds.Bottom);
                                }

                                if (e.Value != null)
                                {
                                    var str = e.Value.ToString().Trim() + "";
                                    str = e.Graphics.MeasureString(str, e.CellStyle.Font).Width <
                                          e.CellBounds.Width - 20
                                        ? str
                                        : str.Substring(0, 20) + "...";
                                    Image img = (dataGrid.SelectedRows.Count > 0
                                        ? dataGrid.SelectedRows[0].Index == e.RowIndex
                                        : false)
                                        ? Resources.help_white
                                        : Resources.help_black;
                                    e.Graphics.DrawImage(img, e.CellBounds.Right - 16,
                                        e.CellBounds.Top + e.CellBounds.Height / 2 - 5, 10, 10);
                                    e.Graphics.DrawString(str, e.CellStyle.Font, textColorBrush,
                                        new Point(e.CellBounds.Left,
                                            e.CellBounds.Top + e.CellBounds.Height / 2 -
                                            (int) e.Graphics.MeasureString(str, e.CellStyle.Font).Height / 2));
                                }
                            }
                        }

                        e.Handled = true;
                    }

            if (dataGrid.Columns["rateCol"].Index == e.ColumnIndex && e.RowIndex >= 0)
            {
                var newRect = new Rectangle(e.CellBounds.X + 1,
                    e.CellBounds.Y + 1, e.CellBounds.Width - 4,
                    e.CellBounds.Height - 4);

                using (
                    Brush gridBrush = new SolidBrush(dataGrid.GridColor),
                    backColorBrush = (dataGrid.SelectedRows.Count > 0
                        ? dataGrid.SelectedRows[0].Index == e.RowIndex
                        : false)
                        ? new SolidBrush(e.CellStyle.SelectionBackColor)
                        : new SolidBrush(e.CellStyle.BackColor)
                )
                {
                    using (var gridLinePen = new Pen(gridBrush))
                    {
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);

                        if (dataGrid.BorderStyle != BorderStyle.None)
                        {
                            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                e.CellBounds.Bottom - 1, e.CellBounds.Right - 1,
                                e.CellBounds.Bottom - 1);
                            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1,
                                e.CellBounds.Top, e.CellBounds.Right - 1,
                                e.CellBounds.Bottom);
                        }

                        if (e.Value != null)
                            using (Brush StarBrush =
                                (dataGrid.SelectedRows.Count > 0 ? dataGrid.SelectedRows[0].Index == e.RowIndex : false)
                                    ? new SolidBrush(e.CellStyle.SelectionForeColor)
                                    : new SolidBrush(Color.Gold))
                            {
                                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                                var numOfStars = e.Value.ToString() == "" ? 0 : int.Parse(e.Value.ToString());
                                if (numOfStars >= 5) numOfStars = 5;
                                if (numOfStars > 0)
                                {
                                    if (numOfStars >= 1)
                                        e.Graphics.FillPolygon(StarBrush,
                                            Calculate5StarPoints(
                                                new PointF(e.CellBounds.X + e.CellBounds.Height / 2,
                                                    e.CellBounds.Y + e.CellBounds.Height / 2), 6f, 3f));
                                    if (numOfStars >= 2)
                                        e.Graphics.FillPolygon(StarBrush,
                                            Calculate5StarPoints(
                                                new PointF(e.CellBounds.X + e.CellBounds.Height / 2 + 12f,
                                                    e.CellBounds.Y + e.CellBounds.Height / 2), 6f, 3f));
                                    if (numOfStars >= 3)
                                        e.Graphics.FillPolygon(StarBrush,
                                            Calculate5StarPoints(
                                                new PointF(e.CellBounds.X + e.CellBounds.Height / 2 + 24f,
                                                    e.CellBounds.Y + e.CellBounds.Height / 2), 6f, 3f));
                                    if (numOfStars >= 4)
                                        e.Graphics.FillPolygon(StarBrush,
                                            Calculate5StarPoints(
                                                new PointF(e.CellBounds.X + e.CellBounds.Height / 2 + 36f,
                                                    e.CellBounds.Y + e.CellBounds.Height / 2), 6f, 3f));
                                    if (numOfStars == 5)
                                        e.Graphics.FillPolygon(StarBrush,
                                            Calculate5StarPoints(
                                                new PointF(e.CellBounds.X + e.CellBounds.Height / 2 + 48f,
                                                    e.CellBounds.Y + e.CellBounds.Height / 2), 6f, 3f));
                                }

                                e.Graphics.SmoothingMode = SmoothingMode.Default;
                            }

                        e.Handled = true;
                    }
                }
            }
        }

        private PointF[] Calculate5StarPoints(PointF Orig, float outerradius, float innerradius)
        {
            var Ang36 = Math.PI / 5.0;
            var Ang72 = 2.0 * Ang36;

            var Sin36 = (float) Math.Sin(Ang36);
            var Sin72 = (float) Math.Sin(Ang72);
            var Cos36 = (float) Math.Cos(Ang36);
            var Cos72 = (float) Math.Cos(Ang72);

            PointF[] pnts = {Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig};
            pnts[0].Y -= outerradius;
            pnts[1].X += innerradius * Sin36;
            pnts[1].Y -= innerradius * Cos36;
            pnts[2].X += outerradius * Sin72;
            pnts[2].Y -= outerradius * Cos72;
            pnts[3].X += innerradius * Sin72;
            pnts[3].Y += innerradius * Cos72;
            pnts[4].X += outerradius * Sin36;
            pnts[4].Y += outerradius * Cos36;

            pnts[5].Y += innerradius;

            pnts[6].X += pnts[6].X - pnts[4].X;
            pnts[6].Y = pnts[4].Y;
            pnts[7].X += pnts[7].X - pnts[3].X;
            pnts[7].Y = pnts[3].Y;
            pnts[8].X += pnts[8].X - pnts[2].X;
            pnts[8].Y = pnts[2].Y;
            pnts[9].X += pnts[9].X - pnts[1].X;
            pnts[9].Y = pnts[1].Y;
            return pnts;
        }

        private void button_refresh_MouseEnter(object sender, EventArgs e)
        {
            buttonFontPrototype = button_refresh.Font;
            button_refresh.Font = new Font(buttonFontPrototype.FontFamily, buttonFontPrototype.Size - 1f,
                buttonFontPrototype.Style);
        }

        private void button_refresh_MosueLeave(object sender, EventArgs e)
        {
            button_refresh.Font = buttonFontPrototype;
        }

        private void button_refresh_MouseDown(object sender, MouseEventArgs e)
        {
            buttonColorPrototype = button_refresh.ForeColor;
            button_refresh.ForeColor = Color.FromArgb(0, 62, 170);
        }

        private void button_refresh_MouseUp(object sender, MouseEventArgs e)
        {
            button_refresh.ForeColor = buttonColorPrototype;
        }

        private void add_MouseEnter(object sender, EventArgs e)
        {
            buttonFontPrototype = add.Font;
            add.Font = new Font(buttonFontPrototype.FontFamily, buttonFontPrototype.Size - 1f,
                buttonFontPrototype.Style);
        }

        private void add_MouseLeave(object sender, EventArgs e)
        {
            add.Font = buttonFontPrototype;
        }

        private void add_MouseDown(object sender, MouseEventArgs e)
        {
            buttonColorPrototype = add.ForeColor;
            add.ForeColor = Color.FromArgb(0, 62, 170);
        }

        private void add_MouseUp(object sender, MouseEventArgs e)
        {
            add.ForeColor = buttonColorPrototype;
        }

        private void delete_button_MouseEnter(object sender, EventArgs e)
        {
            buttonFontPrototype = delete_button.Font;
            delete_button.Font = new Font(buttonFontPrototype.FontFamily, buttonFontPrototype.Size - 1f,
                buttonFontPrototype.Style);
        }

        private void delete_button_MouseLeave(object sender, EventArgs e)
        {
            delete_button.Font = buttonFontPrototype;
        }

        private void delete_button_MouseDown(object sender, MouseEventArgs e)
        {
            buttonColorPrototype = delete_button.ForeColor;
            delete_button.ForeColor = Color.FromArgb(0, 62, 170);
        }

        private void delete_button_MouseUp(object sender, MouseEventArgs e)
        {
            delete_button.ForeColor = buttonColorPrototype;
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            buttonFontPrototype = button4.Font;
            button4.Font = new Font(buttonFontPrototype.FontFamily, buttonFontPrototype.Size - 1f,
                buttonFontPrototype.Style);
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.Font = buttonFontPrototype;
        }

        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            buttonColorPrototype = button4.ForeColor;
            button4.ForeColor = Color.FromArgb(0, 62, 170);
        }

        private void button4_MouseUp(object sender, MouseEventArgs e)
        {
            button4.ForeColor = buttonColorPrototype;
        }


        private void rOption_OnTextChanged(object sender, EventArgs e)
        {
        }

        private void BarcodeSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (barcodeSearch.Checked) currentMatrix = SearchMatrix.BARCODE;
            if (customSearchBox1.Text != "")
            {
                resetSearch(false);
                search(currentMatrix, currentMode);
            }
        }

        private void NameSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (nameSearch.Checked) currentMatrix = SearchMatrix.NAME;
            if (customSearchBox1.Text != "")
            {
                resetSearch(false);
                search(currentMatrix, currentMode);
            }
        }

        private void CommentSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (commentSearch.Checked) currentMatrix = SearchMatrix.REMARK;
            if (customSearchBox1.Text != "")
            {
                resetSearch(false);
                search(currentMatrix, currentMode);
            }
        }

        private void FilterIcon_Click(object sender, EventArgs e)
        {
            if (currentMode == SearchMode.FILTER)
            {
                filterIcon.Image = Resources.filter_stroke;
                currentMode = SearchMode.MARKUP;
            }
            else
            {
                filterIcon.Image = Resources.filter;
                currentMode = SearchMode.FILTER;
            }

            if (customSearchBox1.Text != "")
            {
                resetSearch(false);
                search(currentMatrix, currentMode);
            }
        }

        private void rOption_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!refreshing) saveChanges();
        }

        private void Remote_Click(object sender, EventArgs e)
        {
            rv.Show();
        }


        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == 0x84 && !maximized)
            {
                var cursor = PointToClient(Cursor.Position);

                if (TopLeft.Contains(cursor)) message.Result = (IntPtr) HTTOPLEFT;
                else if (TopRight.Contains(cursor)) message.Result = (IntPtr) HTTOPRIGHT;
                else if (BottomLeft.Contains(cursor)) message.Result = (IntPtr) HTBOTTOMLEFT;
                else if (BottomRight.Contains(cursor)) message.Result = (IntPtr) HTBOTTOMRIGHT;

                else if (Top.Contains(cursor)) message.Result = (IntPtr) HTTOP;
                else if (Left.Contains(cursor)) message.Result = (IntPtr) HTLEFT;
                else if (Right.Contains(cursor)) message.Result = (IntPtr) HTRIGHT;
                else if (Bottom.Contains(cursor)) message.Result = (IntPtr) HTBOTTOM;
            }
        }

        public XmlDocument loadSettingsDataBase()
        {
            if (settingsChanged)
            {
                settings.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
                Program.logger.log("DatabaseManager", "Load Settings.xml", Color.DarkMagenta);
                settingsChanged = false;
            }

            return settings;
        }

        public async Task SaveAsync(XmlDocument xml, string filename)
        {
            await Task.Factory.StartNew(delegate
            {
                using (var fs = new FileStream(FileManager.GetDataBasePath(filename), FileMode.Create))
                {
                    xml.Save(fs);
                }
            });
            Program.logger.log("DatabaseManager", "Save asynchronous " + filename, Color.BlueViolet);
        }


        private void comment_box_TextChanged(object sender, EventArgs e)
        {
            unsaved();
        }

        public Color Multiply(Color c1, Color c2)
        {
            var ar = c1.R;
            var br = c2.R;
            var cr = (byte) ((ar * br + 0xFF) >> 8);

            var ag = c1.G;
            var bg = c2.G;
            var cg = (byte) ((ag * bg + 0xFF) >> 8);

            var ab = c1.B;
            var bb = c2.B;
            var cb = (byte) ((ab * bb + 0xFF) >> 8);

            return Color.FromArgb(255, cr, cg, cb);
        }

        public Image getImage(string str)
        {
            foreach (var img in images)
                if (img.name.Equals(str))
                    return img.image;

            try
            {
                return Image.FromFile(str);
            }
            catch (Exception exc)
            {
                Program.logger.error(exc.Message, exc.StackTrace);
                return null;
            }
        }

        private enum TaskBarLocation
        {
            TOP,
            BOTTOM,
            LEFT,
            RIGHT
        }


        #region initialisers

        public int result_number = 0;
        public int barcode_result_number = 0;
        public int numOfItems;
        private int row;
        public int numOfAvailable;
        public int commentResult = 0;
        public bool name_received = false;
        public string backupSchedule = "";
        private Timer timer1;
        private readonly int rowHeight = 40;
        public string user_loggedin = "System";
        public bool refreshing;
        private string tableFont;
        public int lastSelectedRowIndex = -1;
        public bool winOpenOnce;
        private readonly backupManager BM;
        private bool borrowedColorfull;
        private Color tableSelectionBorrowedColor;
        private Color tableSelectionColor;
        private XmlDocument rmDoc = new XmlDocument();
        public bool settingsOnTop, rmOnTop, imageOnTop, multiBorrowOnTop;
        public bool changed = true;
        public bool rmChanged = true;
        private readonly Image whiteCircuit;
        public RemoteServer remoteServer;
        private readonly remote_view rv;
        public List<string> ActiveBarcodes = new List<string>();

        public bool settingsChanged = true;

        private Color highlightButton;

        public XmlDocument document = new XmlDocument();
        public Color[] searchColors = {Color.Red, Color.Gold, Color.Green};
        private readonly HID_Listener listener = new HID_Listener();

        public KeyCombination saveHotKey, borrowHotKey, helpHotKey, versionHotKey, reloadHotKey, delHotKey;

        public XmlDocument settings = new XmlDocument();
        private readonly List<ItemImage> images = new List<ItemImage>();
        public static List<string> currentOpenWindows = new List<string>();

        #endregion


        #region apply settings and style

        private void applyStyle()
        {
            settings = loadSettingsDataBase();

            header.BackColor = footer.BackColor = Style.get("main/titlebar");
            userDisplay.ForeColor = Style.get("main/subTitleColor");
            title.ForeColor = Style.get("main/titleColor");
            sidebar.BackColor = sideViewButton1.BackColor = remote.BackColor = sideViewButton4.BackColor =
                sideViewButton6.BackColor =
                    um.BackColor = sync.BackColor = backup.BackColor = Style.get("main/sideColor");
            sideViewButton1.HoverColor = sideViewButton4.HoverColor = remote.HoverColor = sideViewButton6.HoverColor =
                um.HoverColor = sync.HoverColor = backup.HoverColor = Style.get("main/sideButtonsHover");
            barcodeSearch.ForeColor = button4.ForeColor = delete_button.ForeColor = add.ForeColor =
                button_refresh.ForeColor = nameSearch.ForeColor = label6.ForeColor = curNum.ForeColor =
                    commentSearch.ForeColor = joke.ForeColor = Style.get("main/sideBarTextDark");
            quickButton.BackColor = button_refresh.BackColor = add.BackColor = delete_button.BackColor =
                button4.BackColor = panel2.BackColor = scrollHolder.BackColor =
                    info.BackColor = dataGrid.BackgroundColor = Style.get("main/backColor");
            if (Style.iconStyle == Style.IconStyle.DARK)
            {
                closeButton.Image = Resources.close_white;
                maximizeButton.Image = Resources.maximize_white;
                minimizeButton.Image = Resources.minimize_white;
                helpButton.Image = Resources.help_white;
                customScrollbar1.ThumbBottomImage = Resources.ThumbBottom;
                customScrollbar1.ThumbBottomSpanImage = Resources.ThumbSpanBottom;
                customScrollbar1.ThumbMiddleImage = Resources.ThumbMiddle;
                customScrollbar1.ThumbTopImage = Resources.ThumbTop;
                customScrollbar1.ThumbTopSpanImage = Resources.ThumbSpanTop;
                customScrollbar1.DownArrowImage = Resources.downarrow;
                customScrollbar1.UpArrowImage = Resources.uparrow;
                customScrollbar1.ChannelColor = Color.FromArgb(31, 31, 31);
            }
            else
            {
                closeButton.Image = Resources.close;
                maximizeButton.Image = Resources.maximize;
                minimizeButton.Image = Resources.minimize;
                helpButton.Image = Resources.help_black;
                customScrollbar1.ThumbBottomImage = Resources.ThumbBottomLight;
                customScrollbar1.ThumbBottomSpanImage = Resources.ThumbSpanBottomLight;
                customScrollbar1.ThumbMiddleImage = Resources.ThumbMiddleLight;
                customScrollbar1.ThumbTopImage = Resources.ThumbTopLight;
                customScrollbar1.ThumbTopSpanImage = Resources.ThumbSpanTopLight;
                customScrollbar1.DownArrowImage = Resources.downarrowLight;
                customScrollbar1.UpArrowImage = Resources.uparrowLight;
                customScrollbar1.ChannelColor = Color.FromArgb(240, 240, 240);
            }

            highlightButton = Style.get("main/highlightButton");
        }


        public void applyInitialSettings()
        {
            settings = loadSettingsDataBase();
            document = loadDataBase();
            rmDoc = loadRoomManagerDataBase();

            backupSchedule = settings.SelectSingleNode("/settings/backupSchedule").InnerXml;

            borrowedColorfull = bool.Parse(settings.SelectSingleNode("/settings/borrowedColorful").InnerXml);
            tableSelectionBorrowedColor =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionBorrowedColor").InnerXml);
            tableSelectionColor =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionColor").InnerXml);

            dataGrid.Columns["regal"].Visible =
                bool.Parse(settings.SelectSingleNode("/settings/showRegalCol").InnerXml);
            dataGrid.Columns["name"].Visible = bool.Parse(settings.SelectSingleNode("/settings/showNameCol").InnerXml);
            dataGrid.Columns["barcode"].Visible =
                bool.Parse(settings.SelectSingleNode("/settings/showBarCol").InnerXml);
            dataGrid.Columns["status"].Visible =
                bool.Parse(settings.SelectSingleNode("/settings/showStatusCol").InnerXml);
            dataGrid.Columns["comment"].Visible =
                bool.Parse(settings.SelectSingleNode("/settings/showComCol").InnerXml);
            dataGrid.Columns["rateCol"].Visible =
                bool.Parse(settings.SelectSingleNode("/settings/showRateCol").InnerXml);

            dataGrid.GridColor =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableLineColor").InnerXml);

            dataGrid.DefaultCellStyle.SelectionBackColor =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionColor").InnerXml);

            var tableFontIn = settings.SelectSingleNode("/settings/tableFontFamily").InnerXml;
            if (checkFontInstalled(tableFontIn))
                tableFont = tableFontIn;
            else
                tableFont = SystemFonts.DefaultFont.Name;

            settingsOnTop = bool.Parse(settings.SelectSingleNode("/settings/settingsOntop").InnerXml);
            rmOnTop = bool.Parse(settings.SelectSingleNode("/settings/rmOnTop").InnerXml);
            imageOnTop = bool.Parse(settings.SelectSingleNode("/settings/imageOnTop").InnerXml);
            multiBorrowOnTop = bool.Parse(settings.SelectSingleNode("/settings/multiborrowOnTop").InnerXml);

            dataGrid.DefaultCellStyle.Font = new Font(tableFont,
                int.Parse(settings.SelectSingleNode("/settings/tableFontSize").InnerXml), GraphicsUnit.Point);
            dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font(tableFont,
                int.Parse(settings.SelectSingleNode("/settings/tableFontSize").InnerXml) + 1, FontStyle.Bold,
                GraphicsUnit.Point);

            searchColors[0] = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/nameSearchColor").InnerXml);
            searchColors[1] =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/barcodeSearchColor").InnerXml);
            searchColors[2] =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/commentSearchColor").InnerXml);

            saveHotKey = new KeyCombination(settings.SelectSingleNode("/settings/saveHotKey").InnerXml);
            borrowHotKey = new KeyCombination(settings.SelectSingleNode("/settings/borrowHotKey").InnerXml);
            helpHotKey = new KeyCombination(settings.SelectSingleNode("/settings/helpHotKey").InnerXml);
            versionHotKey = new KeyCombination(settings.SelectSingleNode("/settings/versionHotKey").InnerXml);
            reloadHotKey = new KeyCombination(settings.SelectSingleNode("/settings/refreshHotKey").InnerXml);
            delHotKey = new KeyCombination(settings.SelectSingleNode("/settings/delHotKey").InnerXml);

            Program.logger.log("Einstellungen anwenden");

            winOpenOnce = !bool.Parse(settings.SelectSingleNode("/settings/winOpenOnce").InnerXml);
        }

        private bool checkCells(DataGridViewRow row)
        {
            foreach (DataGridViewCell c in row.Cells)
                if (c == null || c.Value == null)
                {
                    dataGrid.Rows.Remove(row);
                    return false;
                }
                else
                {
                    return true;
                }

            return false;
        }

        public void applyTableColors()
        {
            Program.logger.log("format table");

            var i = 0;

            foreach (DataGridViewRow row in dataGrid.Rows)
                if (dataGrid.RowCount > 0 && checkCells(row))
                {
                    if (row.Cells["status"].Value.ToString().Contains(strings.borrowed) &&
                        bool.Parse(settings.SelectSingleNode("/settings/borrowedItalic").InnerXml))
                        row.DefaultCellStyle.Font = new Font(tableFont,
                            int.Parse(settings.SelectSingleNode("/settings/tableFontSize").InnerXml), FontStyle.Italic,
                            GraphicsUnit.Point);
                    else
                        row.DefaultCellStyle.Font = new Font(tableFont,
                            int.Parse(settings.SelectSingleNode("/settings/tableFontSize").InnerXml),
                            GraphicsUnit.Point);
                }

            foreach (DataGridViewRow row in dataGrid.Rows)
                if (row.Visible)
                {
                    i++;
                    if (dataGrid.RowCount > 0 && checkCells(row))
                    {
                        if (row.Cells["status"].Value.ToString().Contains(strings.borrowed) &&
                            bool.Parse(settings.SelectSingleNode("/settings/borrowedColorful").InnerXml))
                        {
                            row.DefaultCellStyle.BackColor =
                                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableBorrowedColor")
                                    .InnerXml);
                            row.DefaultCellStyle.SelectionBackColor =
                                ColorTranslator.FromHtml(settings
                                    .SelectSingleNode("/settings/tableSelectionBorrowedColor").InnerXml);
                        }
                        else
                        {
                            row.DefaultCellStyle.SelectionBackColor =
                                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionColor")
                                    .InnerXml);
                            if (i % 2 == 0)
                                row.DefaultCellStyle.BackColor =
                                    ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tablePrimeColor")
                                        .InnerXml);
                            else
                                row.DefaultCellStyle.BackColor =
                                    ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSecondColor")
                                        .InnerXml);
                        }
                    }
                }
        }

        #endregion

        #region essentials

        public async void loadJoke()
        {
            var Joke = await getJoke();
            joke.Text = Joke;
            oldJokes.Add(joke.Text);
        }

        public List<string> oldJokes = new List<string>();

        private void joke_Click(object sender, EventArgs e)
        {
            //freeze when end reached
            var me = (MouseEventArgs) e;
            if (me.Button == MouseButtons.Left)
                if (oldJokes.Contains(joke.Text))
                {
                    var p = oldJokes.IndexOf(joke.Text);
                    if (p < oldJokes.Count - 1)
                    {
                        joke.Text = oldJokes[p + 1];
                    }
                    else
                    {
                        if (oldJokes.Count >= jokeCount - 1)
                            joke.Text = oldJokes[0];
                        else
                            loadJoke();
                    }
                }

            if (me.Button == MouseButtons.Right)
                if (oldJokes.Count > 0)
                {
                    var p = oldJokes.IndexOf(joke.Text);
                    if (p - 1 < 0)
                        joke.Text = oldJokes[0];
                    else
                        joke.Text = oldJokes[p - 1];
                }
        }

        private int jokeCount;

        private async Task<string> getJoke()
        {
            var jokes = new List<string>();

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var reader =
                        new StreamReader(FileManager.GetDataBasePath("Files/" +
                                                                     settings.SelectSingleNode("/settings/csvFileName")
                                                                         .InnerXml)))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(settings.SelectSingleNode("/settings/csvChar").InnerXml[0]);

                            jokes.Add(values[0]);
                        }
                    }
                }
                catch (Exception exc)
                {
                    BeginInvoke((Action) delegate { Program.logger.error(exc.Message, exc.StackTrace); });
                }
            });
            var r = new Random();

            string joke;
            int j;
            if (jokes.Count > 0)
            {
                j = r.Next(1, jokes.Count);
                joke = jokes[j];
                while (oldJokes.Count > 0 && oldJokes.Contains(joke))
                {
                    j = r.Next(1, jokes.Count);
                    joke = jokes[j];
                }

                joke = joke.Trim('\'');

                jokeCount = jokes.Count;
            }
            else
            {
                joke = "";
            }

            return joke;
        }

        private async void backupAsync()
        {
            await backupData();
        }

        public void loadImagesToBuffer()
        {
            Program.logger.log("load images to buffer");
            images.Clear();
            document = loadDataBase();
            foreach (XmlNode node in document.SelectNodes("/data/item/image"))
                if (node.InnerXml != "")
                    images.Add(new ItemImage(Image.FromFile(FileManager.GetDataBasePath("Images\\" + node.InnerXml)),
                        "Images\\" + node.InnerXml));
        }

        public bool checkFontInstalled(string fontName)
        {
            var fontsCollection = new InstalledFontCollection();
            foreach (var fontFamiliy in fontsCollection.Families)
                if (fontFamiliy.Name == fontName)
                    return true;
            return false;
        }

        public bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    Program.logger.log("HTTPClient", "Check for Internet | Internet", Color.Green);
                    return true;
                }
            }
            catch
            {
                Program.logger.log("HTTPClient", "Check for Internet | no Internet", Color.Red);
                return false;
            }
        }

        public void InitTimer()
        {
            timer1 = new Timer();
            timer1.Tick += timer1_Tick;
            timer1.Interval = 2000; // in miliseconds
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            check_name();
        }

        public void check_name()
        {
            if (!name_received)
            {
                var session = new XmlDocument();
                session.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));
                var user_name = session.SelectSingleNode("/list/session/name").InnerText;
                user_loggedin = user_name;
                if (user_name != "") userDisplay.Text = strings.loggedInAs + " " + user_name;

                var user = new XmlDocument();
                session.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));
                var isAdmin = session.SelectSingleNode("/data/user[userName='" + user_name + "']/admin").InnerText;

                if (isAdmin.Equals("true"))
                {
                    um.Visible = true;
                    backup.Visible = true;
                }
                else if (isAdmin.Equals("false"))
                {
                    um.Visible = false;
                    backup.Visible = false;
                }
            }
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();
            return algorithm.ComputeHash(Encoding.ASCII.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            var sb = new StringBuilder();
            foreach (var b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString().ToLower();
        }

        #endregion

        #region load Data and refresh

        public void loadData()
        {
            numOfItems = 0;


            document = loadDataBase();

            if (document.SelectNodes("/data/item").Count > 0)
            {
                while (document.SelectNodes("/data/item").Count >= dataGrid.Rows.Count)
                    dataGrid.Rows.Add(new DataGridViewRow());
            }
            else
            {
                if (document.SelectNodes("data/item").Count == 0 && dataGrid.RowCount != 0) dataGrid.Rows.Clear();
            }

            row = 0;
            foreach (XmlNode dm in document.SelectNodes("/data/item"))
            {
                var state = "";
                if (dm.SelectSingleNode("status").InnerXml != "1")
                    state = strings.borrowed + " " + strings.by + " " + dm.SelectSingleNode("user").InnerXml;
                else
                    state = strings.available;

                string[] item =
                {
                    WebUtility.HtmlDecode(dm.SelectSingleNode("itemName").InnerXml),
                    WebUtility.HtmlDecode(dm.SelectSingleNode("barcode").InnerXml),
                    state,
                    WebUtility.HtmlDecode(dm.SelectSingleNode("comment").InnerXml),
                    WebUtility.HtmlDecode(dm.SelectSingleNode("regal").InnerXml),
                    WebUtility.HtmlDecode(dm.SelectSingleNode("rate").InnerXml)
                };
                dataGrid.Rows[row].Cells[0].Value = item[0];
                dataGrid.Rows[row].Cells[1].Value = item[1];
                dataGrid.Rows[row].Cells[2].Value = item[2];
                var firstLine = item[3].Substring(0,
                    item[3].Contains(Environment.NewLine) ? item[3].IndexOf(Environment.NewLine) : item[3].Length);
                dataGrid.Rows[row].Cells[3].Value = firstLine;
                dataGrid.Rows[row].Cells[4].Value = item[4];
                dataGrid.Rows[row].Cells[5].Value = item[5];

                numOfItems++;
                if (dm.SelectSingleNode("status").InnerXml != "0") numOfAvailable++;
                row++;
            }

            applyTableColors();
            CalculateScrollBar();
        }

        private bool pic2_Visible_Before;

        public void showSideView(bool w)
        {
            if (!w)
            {
                if (pic_2.Visible)
                {
                    pic_2.Visible = false;
                    pic2_Visible_Before = true;
                }
            }
            else
            {
                if (pic2_Visible_Before) pic_2.Visible = true;
                pic2_Visible_Before = false;
            }

            barcode_label.Visible = w;
            barcode_box.Visible = w;
            label2.Visible = w;
            name_box.Visible = w;
            separator3.Visible = w;
            available.Visible = w;
            not_available.Visible = w;
            separator4.Visible = w;
            label3.Visible = w;
            comment_box.Visible = w;
            separator6.Visible = w;
            save_changes.Visible = w;
            button2.Visible = w;
            separator5.Visible = w;
            label1.Visible = w;
            rOption.Visible = w;
            separator7.Visible = w;
            label4.Visible = w;
            rating.Visible = w;
            separator8.Visible = w;
            pic_1.Visible = w;
        }

        private List<int> loadingProcesses = new List<int>();

        public void loadDataToView(int index)
        {
            document = loadDataBase();

            var selectedName = dataGrid.Rows[index].Cells[0].Value + string.Empty;
            var selectedBarcode = dataGrid.Rows[index].Cells[1].Value + string.Empty;
            var selectedComment = document.SelectSingleNode("/data/item[barcode='" + selectedBarcode + "']/comment")
                .InnerText;
            var selectedState = dataGrid.Rows[index].Cells[2].Value + string.Empty;

            if (!ActiveBarcodes.Contains(selectedBarcode.Trim()))
            {
                ActiveBarcodes.Add(selectedBarcode.Trim());
                showSideView(true);

                bool State()
                {
                    if (selectedState.ToUpper() == strings.available.ToUpper())
                        return true;
                    return false;
                }

                /*loadingProcesses.Clear();
                int processId = Environment.TickCount;
                loadingProcesses.Add(processId);
                */
                var img_uri_1 = document.SelectSingleNode("/data/item[barcode='" + selectedBarcode + "']/image")
                    .InnerText;
                var img_zoom_1 = float.Parse(document
                    .SelectSingleNode("/data/item[barcode ='" + selectedBarcode + "']/image/@zoom").Value);
                var img_uri_2 = document.SelectSingleNode("/data/item[barcode='" + selectedBarcode + "']/image2")
                    .InnerText;
                var img_zoom_2 = float.Parse(document
                    .SelectSingleNode("/data/item[barcode ='" + selectedBarcode + "']/image2/@zoom").Value);

                var regal = document.SelectSingleNode("/data/item[barcode='" + selectedBarcode + "']/regal").InnerText;

                var borrow_user = document.SelectSingleNode("/data/item[barcode='" + selectedBarcode + "']/user")
                    .InnerText;

                var _rating = document.SelectSingleNode("/data/item[barcode='" + selectedBarcode + "']/rate").InnerXml;
                var rating = _rating == "" ? 0 : int.Parse(_rating);


                //error

                if (img_uri_1 != "")
                {
                    pic_1.Visible = true;
                    pic_1.Padding = new Padding(0, 0, 0, 0);
                    pic_1.Image = getImage("img\\" + img_uri_1);
                    pic_1.emptyImg = false;
                    pic_1.ZoomValue = img_zoom_1;
                    pic_1.FileName = img_uri_1;
                    pic_1.sendInfo(selectedBarcode, selectedName);
                    pic_1.Invalidate();
                }
                else
                {
                    if (!pic_1.emptyImg) pic_1.setEmpty();
                }

                if (img_uri_2 != "")
                {
                    pic_2.Visible = true;
                    pic_2.Padding = new Padding(0, 0, 0, 0);
                    pic_2.Image = getImage("img\\" + img_uri_2);
                    pic_2.emptyImg = false;
                    pic_2.ZoomValue = img_zoom_2;
                    pic_2.FileName = img_uri_2;
                    pic_2.sendInfo(selectedBarcode, selectedName);
                    pic_2.Invalidate();
                }
                else
                {
                    if (pic_1.emptyImg)
                        pic_2.Visible = false;
                    else
                        pic_2.Visible = true;

                    if (!pic_2.emptyImg) pic_2.setEmpty();
                }

                this.rating.Value = rating;


                rOption.Items.Clear();
                rOption.Items.Add("");

                foreach (XmlNode dm in rmDoc.SelectNodes("/list/regal"))
                {
                    var item = dm.SelectSingleNode("name").InnerXml;
                    rOption.Items.Add(item);
                }


                var selectedStateBool = State();
                barcode_box.Text = selectedBarcode;
                name_box.Text = selectedName;
                if (File.Exists(FileManager.GetDataBasePath("Comments/" + selectedBarcode + ".rtf")))
                {
                    comment_box.load(FileManager.GetDataBasePath("Comments/" + selectedBarcode + ".rtf"));
                }
                else
                {
                    comment_box.resetStyle();
                    comment_box.Text = selectedComment;
                }

                refreshState(selectedStateBool, borrow_user);

                rOption.Text = regal;

                resultDisplay.Text = selectedName;
                saved();

                /*Task.Delay(400).ContinueWith(t => {
                    if (loadingProcesses.Contains(processId)) {
                        this.BeginInvoke((Action)delegate
                        {

                        });
                    }
                });*/
            }
            else
            {
                showSideView(false);
                resultDisplay.Text =
                    "Ups, scheinbar wurde dieses Item schon in einer Remote geöffnet, veruche es später noch einmal.";
            }
        }

        public void refreshState(bool selectedStateBool, string borrow_user)
        {
            if (!selectedStateBool)
            {
                not_available.Text = strings.borrowed + " " + strings.by + " " + borrow_user;
                button4.Text = strings.back;
                not_available.Checked = true;
            }
            else
            {
                not_available.Text = strings.borrowed;
                button4.Text = strings.borrow;
                available.Checked = true;
            }
        }

        private void dataGrid_SelectionChanged(object sender, EventArgs e)
        {
            Task.Delay(100).ContinueWith(t =>
            {
                BeginInvoke(new Action(delegate
                {
                    var totalRows = 0;
                    var indexOfSelected = 0;
                    if (dataGrid.SelectedRows.Count > 0)
                    {
                        if (!checkCells(dataGrid.SelectedRows[0])) return;

                        if (dataGrid.SelectedRows[0].Cells["status"].Value.ToString().Contains(strings.borrowed) &&
                            borrowedColorfull)
                            dataGrid.SelectedRows[0].DefaultCellStyle.SelectionBackColor = tableSelectionBorrowedColor;
                        else
                            dataGrid.SelectedRows[0].DefaultCellStyle.SelectionBackColor = tableSelectionColor;

                        if (lastSelectedRowIndex >= 0)
                        {
                            if (lastSelectedRowIndex >= dataGrid.RowCount) lastSelectedRowIndex = dataGrid.RowCount - 1;
                            if (dataGrid.Rows[lastSelectedRowIndex].Cells["status"].Value.ToString()
                                .Contains(strings.borrowed) && borrowedColorfull)
                                dataGrid.Rows[lastSelectedRowIndex].DefaultCellStyle.SelectionBackColor =
                                    tableSelectionBorrowedColor;
                            else
                                dataGrid.Rows[lastSelectedRowIndex].DefaultCellStyle.SelectionBackColor =
                                    tableSelectionColor;

                            if (ActiveBarcodes.Contains(dataGrid.Rows[lastSelectedRowIndex].Cells["barcode"].Value
                                .ToString()))
                                ActiveBarcodes.Remove(dataGrid.Rows[lastSelectedRowIndex].Cells["barcode"].Value
                                    .ToString());
                        }

                        if ((lastSelectedRowIndex != dataGrid.SelectedRows[0].Index || !barcode_box.Visible) &&
                            !refreshing)
                        {
                            totalRows = dataGrid.RowCount;
                            indexOfSelected = dataGrid.SelectedRows[0].Index + 1;

                            try
                            {
                                loadDataToView(dataGrid.SelectedRows[0].Index);
                            }
                            catch (Exception exc)
                            {
                                Program.logger.error(exc.Message, exc.StackTrace);
                            }

                            curNum.Text = indexOfSelected + " / " + totalRows;

                            lastSelectedRowIndex = dataGrid.SelectedRows[0].Index;
                        }
                        else
                        {
                            totalRows = dataGrid.RowCount;
                        }
                    }
                    else
                    {
                        if (!refreshing)
                        {
                            showSideView(false);
                            resultDisplay.Text = strings.nothingSelected;
                            var total_rows = dataGrid.Rows.Count;
                            curNum.Text = "0 / " + total_rows;
                        }
                    }
                }));
            });
        }

        private void refreshImage(string img_uri)
        {
            var img = getImage("img\\noimage.png");

            if (img_uri != "")
                img = getImage("img\\" + img_uri);
            else
                img = getImage("img\\noimage.png");

            /*pic.Image = img;
            double differenz = (double)img.Width / (double)img.Height;
            double produkt = (double)differenz * 130;
            int width = (int)Math.Floor(produkt);
            pic.Width = width;
            pic.Height = 130;*/
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            refresh(true);
        }

        public void refresh(bool refreshAnyWay)
        {
            refresh(refreshAnyWay, true, false);
        }

        public void refresh(bool refreshAnyWay, bool refreshSearch)
        {
            refresh(refreshAnyWay, refreshSearch, false);
        }

        private int lastFirstDisplayedRowIndex;

        public void refresh(bool refreshAnyWay, bool refreshSearch, bool keepViewport)
        {
            document = loadDataBase();
            var root = document.DocumentElement;
            var ElementList = root.GetElementsByTagName("item");
            if (ElementList.Count <= 0) return;

            if (refreshAnyWay) changed = true;


            lastFirstDisplayedRowIndex = dataGrid.FirstDisplayedScrollingRowIndex;

            rmDoc = loadRoomManagerDataBase();

            refreshing = true;
            var selectedCount = 0;

            var oldSortOrder = dataGrid.SortOrder;
            var oldSortColumn = dataGrid.SortedColumn;

            foreach (DataGridViewRow row in dataGrid.SelectedRows) selectedCount++;

            var selectedRow = 0;

            if (selectedCount > 0)
            {
                if (!checkCells(dataGrid.SelectedRows[0])) return;
                var distance = 0;

                selectedRow = dataGrid.SelectedRows[0].Index;
                try
                {
                    distance = dataGrid.SelectedRows[0].Index - dataGrid.FirstDisplayedCell.RowIndex;
                }
                catch (Exception e)
                {
                    Program.logger.error(e.Message);
                    distance = 0;
                }

                //selectedRow equals 0 if called from add_view.cs

                var items = rmDoc.SelectNodes("/data/item").Count;
                var curRows = dataGrid.Rows.Count;

                for (var i = 0; i <= items - curRows; i++) dataGrid.Rows.Add(new DataGridViewRow());
                loadData();
            }
            else
            {
                var items = rmDoc.SelectNodes("/data/item").Count;
                var curRows = dataGrid.Rows.Count;
                foreach (DataGridViewRow row in dataGrid.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Value = "";
                for (var i = 0; i <= items - curRows; i++) dataGrid.Rows.Add(new DataGridViewRow());
                loadData();
            }


            if (oldSortColumn != null)
            {
                if (oldSortOrder == SortOrder.Ascending)
                    dataGrid.Sort(oldSortColumn, ListSortDirection.Ascending);
                else
                    dataGrid.Sort(oldSortColumn, ListSortDirection.Descending);
            }

            Console.WriteLine(selectedCount + " " + selectedRow);
            dataGrid.Rows[selectedRow].Selected = true;
            curNum.Text = (dataGrid.SelectedRows.Count > 0 ? dataGrid.SelectedRows[0].Index + 1 : 0) + " / " +
                          dataGrid.RowCount;

            if (refreshSearch)
            {
                if (currentMatrix == SearchMatrix.BARCODE &&
                    bool.Parse(settings.SelectSingleNode("/settings/searchExact").InnerXml))
                    resetSearch(true, keepViewport);
                else
                    search(currentMatrix, currentMode);
            }


            if (keepViewport)
                resort(true);
            else
                resort(false);
            refreshing = false;

            if (keepViewport)
            {
                dataGrid.FirstDisplayedScrollingRowIndex = DisplayedRows.FirstOrDefault().Index;

                DisplayedRows.Clear();
                foreach (DataGridViewRow row in dataGrid.Rows)
                    if (row.Displayed)
                        if (!DisplayedRows.Contains(row))
                            DisplayedRows.Add(row);
            }
        }

        #endregion

        #region search

        private SearchMatrix currentMatrix;
        private SearchMode currentMode;

        private enum SearchMatrix
        {
            BARCODE,
            NAME,
            REMARK
        }

        private enum SearchMode
        {
            FILTER,
            MARKUP
        }

        private string searchString;
        private int numOfResults;


        private void CustomSearchBox1_SearchClick(object sender, EventArgs e)
        {
            search(currentMatrix, currentMode);
        }

        private void CustomSearchBox1_ResetClick(object sender, EventArgs e)
        {
            resetSearch(true);
        }

        private void CustomSearchBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (bool.Parse(settings.SelectSingleNode("/settings/searchWhileType").InnerXml))
            {
                resetSearch(false);
                search(currentMatrix, currentMode);
            }

            if (e.KeyCode == Keys.Enter) search(currentMatrix, currentMode);
            if (customSearchBox1.Text == "") resetSearch(false);
        }

        private void resetSearch(bool emptyText)
        {
            resetSearch(emptyText, false);
        }

        private void resetSearch(bool emptyText, bool keepView)
        {
            if (emptyText) customSearchBox1.Text = "";
            customSearchBox1.searchInAction = false;
            customSearchBox1.Invalidate();
            refresh(false, false, keepView);
            foreach (DataGridViewRow row in dataGrid.Rows) row.DefaultCellStyle.ForeColor = Color.Black;
            resDisplay.Text = strings.noResults;
        }

        private readonly RichTextBox rtfBox = new RichTextBox();

        private void search(SearchMatrix matrix, SearchMode mode)
        {
            var caseSensitive = bool.Parse(settings.SelectSingleNode("/settings/searchCaseSensitive").InnerXml);

            if (customSearchBox1.Text == "") return;

            if (caseSensitive)
                searchString = customSearchBox1.Text;
            else
                searchString = customSearchBox1.Text.ToUpper();

            customSearchBox1.searchInAction = true;

            var col = "";

            if (matrix == SearchMatrix.NAME)
                col = "name";
            else if (matrix == SearchMatrix.BARCODE)
                col = "barcode";
            else
                col = "comment";

            if (mode == SearchMode.FILTER)
            {
                //FILTER
                dataGrid.SuspendLayout();

                var toBeDeleted = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    var comment = "";
                    if (matrix == SearchMatrix.REMARK &&
                        File.Exists(FileManager.GetDataBasePath("Comments/" + row.Cells["barcode"].Value + ".rtf")))
                    {
                        rtfBox.Rtf =
                            File.ReadAllText(
                                FileManager.GetDataBasePath("Comments/" + row.Cells["barcode"].Value + ".rtf"));
                        comment = rtfBox.Text;
                    }

                    if (matrix == SearchMatrix.BARCODE &&
                        bool.Parse(settings.SelectSingleNode("/settings/searchExact").InnerXml))
                    {
                        var comp = caseSensitive
                            ? row.Cells[col].Value.ToString()
                            : row.Cells[col].Value.ToString().ToUpper();
                        if (comp.Equals(searchString))
                        {
                            row.Selected = true;

                            numOfResults = 1;
                            if (dataGrid.SelectedRows[0].Index > 10)
                                dataGrid.FirstDisplayedScrollingRowIndex = dataGrid.SelectedRows[0].Index - 10;
                            else
                                dataGrid.FirstDisplayedScrollingRowIndex = 0;
                        }
                    }
                    else
                    {
                        var comp = caseSensitive
                            ? row.Cells[col].Value.ToString()
                            : row.Cells[col].Value.ToString().ToUpper();
                        var compcom = caseSensitive ? comment : comment.ToUpper();
                        if (!compcom.Contains(searchString) && !comp.Contains(searchString)) toBeDeleted.Add(row);
                    }
                }

                toBeDeleted.ForEach(d => dataGrid.Rows.Remove(d));
                if (toBeDeleted.Count > 0)
                    numOfResults = dataGrid.RowCount;
                else
                    numOfResults = 0;

                dataGrid.ResumeLayout();
                resDisplay.Width = 150;
                resDisplay.Text = numOfResults + " " + strings.found;
                applyTableColors();
                CalculateScrollBar();
                if (dataGrid.RowCount > 0) dataGrid.Rows[0].Selected = true;
            }
            else
            {
                //MARKUP
                var resultRowsIndex = new List<int>();
                numOfResults = 0;
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    var comment = "";
                    if (matrix == SearchMatrix.REMARK &&
                        File.Exists(FileManager.GetDataBasePath("Comments/" + row.Cells["barcode"].Value + ".rtf")))
                    {
                        rtfBox.Rtf =
                            File.ReadAllText(
                                FileManager.GetDataBasePath("Comments/" + row.Cells["barcode"].Value + ".rtf"));
                        comment = rtfBox.Text;
                    }

                    if (matrix == SearchMatrix.BARCODE &&
                        bool.Parse(settings.SelectSingleNode("/settings/searchExact").InnerXml))
                    {
                        var comp = caseSensitive
                            ? row.Cells[col].Value.ToString()
                            : row.Cells[col].Value.ToString().ToUpper();

                        if (comp.Equals(searchString))
                        {
                            row.Selected = true;
                            if (dataGrid.SelectedRows[0].Index > 10)
                                dataGrid.FirstDisplayedScrollingRowIndex = dataGrid.SelectedRows[0].Index - 10;
                            else
                                dataGrid.FirstDisplayedScrollingRowIndex = 0;
                        }
                    }
                    else
                    {
                        var comp = caseSensitive
                            ? row.Cells[col].Value.ToString()
                            : row.Cells[col].Value.ToString().ToUpper();
                        var compcom = caseSensitive ? comment : comment.ToUpper();
                        if (comp.Contains(searchString) || compcom.Contains(searchString))
                        {
                            row.DefaultCellStyle.BackColor =
                                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/" + col + "SearchColor")
                                    .InnerXml);
                            row.DefaultCellStyle.ForeColor = Color.White;
                            numOfResults++;
                            resultRowsIndex.Add(row.Index);
                        }
                    }
                }

                if (resultRowsIndex.Count > 0 &&
                    bool.Parse(settings.SelectSingleNode("/settings/scrollToSearch").InnerXml))
                {
                    if (resultRowsIndex[0] > 10)
                        dataGrid.FirstDisplayedScrollingRowIndex = resultRowsIndex[0] - 10;
                    else
                        dataGrid.FirstDisplayedScrollingRowIndex = 0;
                }

                resDisplay.Width = 150;
                resDisplay.Text = numOfResults + " " + strings.found;
            }

            if (numOfResults == 0) resetSearch(false);
        }

        #endregion
    }

    public class ItemImage
    {
        public Image image;
        public string name;

        public ItemImage(Image _image, string _name)
        {
            image = _image;
            name = _name;
        }
    }

    public class KeyCombination
    {
        public bool altNeeded;

        public bool controlNeeded;

        private readonly KeysConverter kc = new KeysConverter();
        public Keys key;
        public bool shiftNeeded;

        public KeyCombination(string comb)
        {
            if (comb.Contains("Shift")) shiftNeeded = true;
            if (comb.Contains("Ctrl")) controlNeeded = true;
            if (comb.Contains("Alt")) altNeeded = true;
            if (comb.Contains("+"))
            {
                var pieces = comb.Split('+');
                var nameRaw = pieces[pieces.Length - 1];
                key = (Keys) kc.ConvertFromString(nameRaw.Trim());
            }
            else
            {
                key = (Keys) kc.ConvertFromString(comb.Trim());
            }
        }

        public override string ToString()
        {
            return controlNeeded + "; " + altNeeded + "; " + shiftNeeded + "; " + key;
        }
    }


    public static class XmlHandler
    {
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }

        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }

            return xmlDocument;
        }
    }

    public static class ExtensionMethods
    {
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            var dgvType = dgv.GetType();
            var pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }

    public class Rating : BunifuRating
    {
        private Rectangle b;

        public Rating()
        {
            b = Bounds;
            foreach (Control c in Controls) c.MouseClick += C_MouseClick;
        }

        private void C_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(e);
        }
    }

    public class DataGridViewC : DataGridView
    {
        protected override bool ShowFocusCues => false;
    }
}