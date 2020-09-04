/*
©Lukas Schreiber 2017-2020
 */


using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace allinthebox
{


    public partial class Main : Form
    {


        #region initialisers   
        public int result_number = 0;
        public int barcode_result_number = 0;
        public int numOfItems = 0;
        private int row = 0;
        public int numOfAvailable = 0;
        public int commentResult = 0;
        public bool name_received = false;
        public string backupSchedule = "";
        private System.Windows.Forms.Timer timer1;
        private int rowHeight = 40;
        public string user_loggedin = "System";
        public bool refreshing;
        private String tableFont;
        public int lastSelectedRowIndex = -1;
        public Boolean winOpenOnce;
        backupManager BM;
        private Boolean borrowedColorfull;
        private Color tableSelectionBorrowedColor;
        private Color tableSelectionColor;
        private XmlDocument rmDoc = new XmlDocument();
        public Boolean settingsOnTop, rmOnTop, imageOnTop, multiBorrowOnTop;
        public bool changed = true;
        public bool rmChanged = true;
        private Image whiteCircuit;
        public RemoteServer remoteServer;
        private remote_view rv;
        public List<string> ActiveBarcodes = new List<string>();

        public bool settingsChanged = true;

        private Color highlightButton;

        public XmlDocument document = new XmlDocument();
        public Color[] searchColors = { Color.Red, Color.Gold, Color.Green };
        HID_Listener listener = new HID_Listener();

        public KeyCombination saveHotKey, borrowHotKey, helpHotKey, versionHotKey, reloadHotKey, delHotKey;

        public XmlDocument settings = new XmlDocument();
        List<ItemImage> images = new List<ItemImage>();
        public static List<String> currentOpenWindows = new List<String>();

        #endregion


        public Main()
        {
            DoubleBuffered = true;

            whiteCircuit = Invert(allinthebox.Properties.Resources.circuitBoardBackground);

            InitializeComponent();


            //load remoteServer
            remoteServer = new RemoteServer(this);
            remoteServer.RecievedMessage += RemoteServer_RecievedMessage;

            //language
            this.userDisplay.Text = Properties.strings.notLoggedIn;
            this.resDisplay.Text = Properties.strings.noResults;
            this.barcodeSearch.Text = Properties.strings.searchBarcode;
            this.nameSearch.Text = Properties.strings.searchNames;
            this.commentSearch.Text = Properties.strings.searchComments;
            this.sideViewButton1.ButtonText = Properties.strings.regalmanager;
            this.um.ButtonText = Properties.strings.userManager;
            this.sideViewButton4.ButtonText = Properties.strings.MultiAuswahlTool;
            this.backup.ButtonText = Properties.strings.BackupManager;
            this.sideViewButton6.ButtonText = Properties.strings.log;
            this.joke.Text = Properties.strings.joke;
            this.dataGrid.Columns["name"].HeaderText = Properties.strings.name;
            this.dataGrid.Columns["barcode"].HeaderText = Properties.strings.barcode;
            this.dataGrid.Columns["status"].HeaderText = Properties.strings.state;
            this.dataGrid.Columns["comment"].HeaderText = Properties.strings.comment;
            this.dataGrid.Columns["regal"].HeaderText = Properties.strings.regal;
            this.dataGrid.Columns["rateCol"].HeaderText = Properties.strings.rate;
            this.resultDisplay.Text = Properties.strings.noItem;
            this.barcode_label.Text = Properties.strings.barcode + ":";
            this.label2.Text = Properties.strings.name + ":";
            this.available.Text = Properties.strings.available;
            this.not_available.Text = Properties.strings.borrowed;
            this.label3.Text = Properties.strings.comment + ":";
            this.save_changes.Text = Properties.strings.save;
            this.button2.Text = Properties.strings.delete;
            this.label1.Text = Properties.strings.regal + ":";
            this.label4.Text = Properties.strings.rate + ":";
            this.button_refresh.Text = Properties.strings.refresh;
            this.add.Text = Properties.strings.add;
            this.delete_button.Text = Properties.strings.delete;
            this.button4.Text = Properties.strings.borrow;
            this.contextMenuDataGrid.Items[0].Text = Properties.strings.delete;
            this.contextMenuDataGrid.Items[1].Text = Properties.strings.add;
            this.contextMenuDataGrid.Items[2].Text = Properties.strings.refresh;
            this.contextMenuDataGrid.Items[3].Text = Properties.strings.borrow;


            this.pic_1.setParent(this);
            this.pic_2.setParent(this);

            um.Visible = false;
            backup.Visible = false;


            applyStyle();

            Maximize();

            BM = new backupManager(this);
            rv = new remote_view(user_loggedin, this);

            applyInitialSettings();

            this.filterIcon.Image = Properties.Resources.filter_stroke;
            currentMode = SearchMode.MARKUP;


            loadImagesToBuffer();
            applyTableColors();

            refresh(false);

            rowHeight = 40;

            long nextBackup = 0;
            var dt = DateTime.ParseExact(settings.SelectSingleNode("/settings/lastUpdate").InnerXml, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            long lastBackup = dt.ToFileTimeUtc();


            if (backupSchedule == Properties.strings.annually)
            {
                nextBackup = lastBackup + 31556952000000000;
            }
            else if (backupSchedule == Properties.strings.monthly)
            {
                nextBackup = lastBackup + 2592000000000000;
            }
            else if (backupSchedule == Properties.strings.weekly)
            {
                nextBackup = lastBackup + 604800000000000;
            }
            else if (backupSchedule == Properties.strings.daily) {
                nextBackup = lastBackup;

            }

            if (backupSchedule != Properties.strings.never && nextBackup < DateTime.ParseExact(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss").Replace('.', '/'), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToFileTimeUtc())
            {

                backupAsync();
            }

            ActiveControl = dataGrid;

            this.KeyPreview = true;
            listener.normalize_listener();
            this.Focus();
            if (dataGrid.RowCount > 0)
            {
                dataGrid.Rows[0].Selected = true;
            }

            InitTimer();

            this.label6.Text = "© Lukas Schreiber, 2017-" + DateTime.Now.Year.ToString();


            for (int i = 0; i < 6; i++)
            {
                dataGrid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }


            int searchInt = int.Parse(settings.SelectSingleNode("/settings/searchDefault").InnerXml);
            switch (searchInt)
            {
                case 0:
                    this.nameSearch.Select();
                    currentMatrix = SearchMatrix.NAME;
                    break;
                case 1:
                    this.barcodeSearch.Select();
                    currentMatrix = SearchMatrix.BARCODE;
                    break;
                case 2:
                    this.commentSearch.Select();
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

            if (dataGrid.RowCount <= 0) {
                showSideView(false);
            }
            else {
                showSideView(true);
                foreach (DataGridViewRow r in dataGrid.Rows)
                {
                    r.Height = rowHeight;
                }
            }

            CalculateScrollBar();
        }

        //handle recieved Messages
        /*
         * every TCP request comes in here
         * handle answers here
         * request = e.Message
         */
        private void RemoteServer_RecievedMessage(object sender, ServerEventArgs e)
        {
            string transmissionData = "";

            //only async tasks here
            string[] msg = e.Message.Split(';');

            if (msg[0].Contains(RemoteServer.CODES.CONNECTED.ToString()))
            {
                /*
                 request: 0;name
                 answer: RemoteServer.CODE
                 */

                Program.logger.log("connected to " + msg[1], Color.Green);
                rv.Invoke(rv.writeLabel, new object[] { "Verbunden mit " + msg[1] });

                transmissionData = RemoteServer.CODE;
                remoteServer.ANSWER = UTF8Encoding.ASCII.GetBytes(transmissionData);
            }

            if (msg[0].Contains(RemoteServer.CODES.BARCODE.ToString())) {
                /*
                 * request: 1;barcode
                 * answer: data
                 */


                string b = msg[1];

                if (ActiveBarcodes.Contains(b.Trim()))
                {
                    transmissionData = RemoteServer.CODES.CURRENTLY_IN_USE.ToString();

                }
                else {
                    transmissionData = getDataFromXML(b);
                }
                remoteServer.ANSWER = UTF8Encoding.UTF8.GetBytes(transmissionData);

            }

            if (msg[0].Contains(RemoteServer.CODES.CANCEL.ToString())) {

                /*
                 * request: 2, barcode
                 * answer: 2
                 */

                ActiveBarcodes.Remove(msg[1].Trim());
                Console.WriteLine(ActiveBarcodes.Count + " currently used barcodes");
                transmissionData = RemoteServer.CODES.CANCEL.ToString();
                remoteServer.ANSWER = UTF8Encoding.ASCII.GetBytes(transmissionData);

            }

            if (msg[0].Contains(RemoteServer.CODES.SAVE.ToString())) {
                /*
                 * request: 3, data
                 * answer: 3
                 */
                ActiveBarcodes.Remove(msg[1].Trim());

                saveDataToXML(msg[1], msg[2], msg[3], msg[4]);

                transmissionData = RemoteServer.CODES.SAVE.ToString();
                remoteServer.ANSWER = UTF8Encoding.ASCII.GetBytes(transmissionData);
            }

            if (msg[0].Contains(RemoteServer.CODES.ADD.ToString())) {
                /*
                 * request: 4, data
                 * answer 4
                 */

                addItemFromRemote(msg[1], msg[2], msg[3]);

                transmissionData = RemoteServer.CODES.SAVE.ToString();
                remoteServer.ANSWER = UTF8Encoding.ASCII.GetBytes(transmissionData);
            }

        }

        //add item from Remote
        private void addItemFromRemote(string barcode, string name, string rat) {

            this.BeginInvoke((Action)async delegate {
                document = loadDataBase();
                XDocument doc = XmlHandler.ToXDocument(document);

                if (name != "" && barcode != "")
                {
                    XElement root = new XElement("item");
                    XElement image = new XElement("image", "");
                    XElement image2 = new XElement("image2", "");
                    XAttribute zoom = new XAttribute("zoom", "2");
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

                    document = XmlHandler.ToXmlDocument(doc);
                    await SaveAsync(document, FileManager.NAMES.DATA);

                    saved();
                    refresh(false);
                }
            });
        }

        //save DATA from Remote to XML
        private void saveDataToXML(string barcode, string name, string stat, string rat) {
            document = loadDataBase();

            this.BeginInvoke((Action)async delegate {

                String XPathName = "/data/item[barcode='" + barcode + "']/itemName";
                XmlNode node_name = document.SelectSingleNode(XPathName);
                node_name.InnerXml = name;

                String XPathRate = "/data/item[barcode='" + barcode + "']/rate";
                XmlNode node_rate = document.SelectSingleNode(XPathRate);
                node_rate.InnerXml = rat;

                if (stat == "1")
                {
                    String XPath = "/data/item[barcode='" + barcode + "']/status";
                    String userPath = "/data/item[barcode='" + barcode + "']/user";
                    XmlNode node = document.SelectSingleNode(XPath);
                    XmlNode user_node = document.SelectSingleNode(userPath);
                    node.InnerXml = "1";
                    user_node.InnerXml = "";
                }
                else
                {
                    String XPath = "/data/item[barcode='" + barcode + "']/status";
                    String userPath = "/data/item[barcode='" + barcode + "']/user";
                    XmlNode node = document.SelectSingleNode(XPath);
                    XmlNode user_node = document.SelectSingleNode(userPath);
                    node.InnerXml = "0";
                    user_node.InnerXml = user_loggedin;

                }

                await SaveAsync(document, FileManager.NAMES.DATA);

                DisplayedRows.Clear();
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    if (row.Displayed)
                    {
                        if (!DisplayedRows.Contains(row))
                        {
                            DisplayedRows.Add(row);
                        }

                    }


                }
                refresh(false, true, true);
            });
        }

        //get DATA for Remote from XML
        private string getDataFromXML(string code) {


            String a = "";
            document = loadDataBase();

            //handle that barcode is not present

            XmlNode node = document.SelectSingleNode("data/item[barcode='" + code.Trim() + "']");

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
            else {
                a = RemoteServer.CODES.NOT_IN_DB.ToString() + ";" + code.Trim();
            }

            return a;
        }




        #region apply settings and style

        private void applyStyle()
        {
            settings = loadSettingsDataBase();

            this.header.BackColor = this.footer.BackColor = Style.get("main/titlebar");
            this.userDisplay.ForeColor = Style.get("main/subTitleColor");
            this.title.ForeColor = Style.get("main/titleColor");
            this.sidebar.BackColor = this.sideViewButton1.BackColor = this.remote.BackColor = this.sideViewButton4.BackColor = this.sideViewButton6.BackColor = this.um.BackColor = this.sync.BackColor = this.backup.BackColor = Style.get("main/sideColor");
            this.sideViewButton1.HoverColor = this.sideViewButton4.HoverColor = this.remote.HoverColor = this.sideViewButton6.HoverColor = this.um.HoverColor = this.sync.HoverColor = this.backup.HoverColor = Style.get("main/sideButtonsHover");
            this.barcodeSearch.ForeColor = this.button4.ForeColor = this.delete_button.ForeColor = this.add.ForeColor = this.button_refresh.ForeColor = this.nameSearch.ForeColor = this.label6.ForeColor = this.curNum.ForeColor = this.commentSearch.ForeColor = this.joke.ForeColor = Style.get("main/sideBarTextDark");
            this.quickButton.BackColor = this.button_refresh.BackColor = this.add.BackColor = this.delete_button.BackColor = this.button4.BackColor = this.panel2.BackColor = this.scrollHolder.BackColor = this.info.BackColor = this.dataGrid.BackgroundColor = Style.get("main/backColor");
            if (Style.iconStyle == Style.IconStyle.DARK)
            {
                this.closeButton.Image = allinthebox.Properties.Resources.close_white;
                this.maximizeButton.Image = allinthebox.Properties.Resources.maximize_white;
                this.minimizeButton.Image = allinthebox.Properties.Resources.minimize_white;
                this.helpButton.Image = allinthebox.Properties.Resources.help_white;
                this.customScrollbar1.ThumbBottomImage = allinthebox.Properties.Resources.ThumbBottom;
                this.customScrollbar1.ThumbBottomSpanImage = allinthebox.Properties.Resources.ThumbSpanBottom;
                this.customScrollbar1.ThumbMiddleImage = allinthebox.Properties.Resources.ThumbMiddle;
                this.customScrollbar1.ThumbTopImage = allinthebox.Properties.Resources.ThumbTop;
                this.customScrollbar1.ThumbTopSpanImage = allinthebox.Properties.Resources.ThumbSpanTop;
                this.customScrollbar1.DownArrowImage = allinthebox.Properties.Resources.downarrow;
                this.customScrollbar1.UpArrowImage = allinthebox.Properties.Resources.uparrow;
                this.customScrollbar1.ChannelColor = Color.FromArgb(31, 31, 31);
            }
            else {
                this.closeButton.Image = allinthebox.Properties.Resources.close;
                this.maximizeButton.Image = allinthebox.Properties.Resources.maximize;
                this.minimizeButton.Image = allinthebox.Properties.Resources.minimize;
                this.helpButton.Image = allinthebox.Properties.Resources.help_black;
                this.customScrollbar1.ThumbBottomImage = allinthebox.Properties.Resources.ThumbBottomLight;
                this.customScrollbar1.ThumbBottomSpanImage = allinthebox.Properties.Resources.ThumbSpanBottomLight;
                this.customScrollbar1.ThumbMiddleImage = allinthebox.Properties.Resources.ThumbMiddleLight;
                this.customScrollbar1.ThumbTopImage = allinthebox.Properties.Resources.ThumbTopLight;
                this.customScrollbar1.ThumbTopSpanImage = allinthebox.Properties.Resources.ThumbSpanTopLight;
                this.customScrollbar1.DownArrowImage = allinthebox.Properties.Resources.downarrowLight;
                this.customScrollbar1.UpArrowImage = allinthebox.Properties.Resources.uparrowLight;
                this.customScrollbar1.ChannelColor = Color.FromArgb(240, 240, 240);
            }
            this.highlightButton = Style.get("main/highlightButton");
        }


        public void applyInitialSettings() {

            settings = loadSettingsDataBase();
            document = loadDataBase();
            rmDoc = loadRoomManagerDataBase();

            backupSchedule = settings.SelectSingleNode("/settings/backupSchedule").InnerXml;

            this.borrowedColorfull = Boolean.Parse(settings.SelectSingleNode("/settings/borrowedColorful").InnerXml);
            this.tableSelectionBorrowedColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionBorrowedColor").InnerXml);
            this.tableSelectionColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionColor").InnerXml);

            this.dataGrid.Columns["regal"].Visible = Boolean.Parse(settings.SelectSingleNode("/settings/showRegalCol").InnerXml);
            this.dataGrid.Columns["name"].Visible = Boolean.Parse(settings.SelectSingleNode("/settings/showNameCol").InnerXml);
            this.dataGrid.Columns["barcode"].Visible = Boolean.Parse(settings.SelectSingleNode("/settings/showBarCol").InnerXml);
            this.dataGrid.Columns["status"].Visible = Boolean.Parse(settings.SelectSingleNode("/settings/showStatusCol").InnerXml);
            this.dataGrid.Columns["comment"].Visible = Boolean.Parse(settings.SelectSingleNode("/settings/showComCol").InnerXml);
            this.dataGrid.Columns["rateCol"].Visible = Boolean.Parse(settings.SelectSingleNode("/settings/showRateCol").InnerXml);

            this.dataGrid.GridColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableLineColor").InnerXml);

            this.dataGrid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionColor").InnerXml);

            String tableFontIn = settings.SelectSingleNode("/settings/tableFontFamily").InnerXml;
            if (checkFontInstalled(tableFontIn))
            {
                tableFont = tableFontIn;
            }
            else {
                tableFont = SystemFonts.DefaultFont.Name;
            }

            this.settingsOnTop = Boolean.Parse(settings.SelectSingleNode("/settings/settingsOntop").InnerXml);
            this.rmOnTop = Boolean.Parse(settings.SelectSingleNode("/settings/rmOnTop").InnerXml);
            this.imageOnTop = Boolean.Parse(settings.SelectSingleNode("/settings/imageOnTop").InnerXml);
            this.multiBorrowOnTop = Boolean.Parse(settings.SelectSingleNode("/settings/multiborrowOnTop").InnerXml);

            this.dataGrid.DefaultCellStyle.Font = new Font(tableFont, int.Parse(settings.SelectSingleNode("/settings/tableFontSize").InnerXml), GraphicsUnit.Point);
            this.dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font(tableFont, int.Parse(settings.SelectSingleNode("/settings/tableFontSize").InnerXml) + 1, FontStyle.Bold, GraphicsUnit.Point);

            this.searchColors[0] = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/nameSearchColor").InnerXml);
            this.searchColors[1] = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/barcodeSearchColor").InnerXml);
            this.searchColors[2] = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/commentSearchColor").InnerXml);

            this.saveHotKey = new KeyCombination(settings.SelectSingleNode("/settings/saveHotKey").InnerXml);
            this.borrowHotKey = new KeyCombination(settings.SelectSingleNode("/settings/borrowHotKey").InnerXml);
            this.helpHotKey = new KeyCombination(settings.SelectSingleNode("/settings/helpHotKey").InnerXml);
            this.versionHotKey = new KeyCombination(settings.SelectSingleNode("/settings/versionHotKey").InnerXml);
            this.reloadHotKey = new KeyCombination(settings.SelectSingleNode("/settings/refreshHotKey").InnerXml);
            this.delHotKey = new KeyCombination(settings.SelectSingleNode("/settings/delHotKey").InnerXml);

            Program.logger.log("Einstellungen anwenden");

            this.winOpenOnce = !Boolean.Parse(settings.SelectSingleNode("/settings/winOpenOnce").InnerXml);
        }

        private Boolean checkCells(DataGridViewRow row) {
            foreach (DataGridViewCell c in row.Cells) {
                if (c == null || c.Value == null)
                {
                    dataGrid.Rows.Remove(row);
                    return false;
                }
                else {
                    return true;
                }
            }
            return false;
        }

        public void applyTableColors() {
            Program.logger.log("format table");

            int i = 0;

            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                if (dataGrid.RowCount > 0 && checkCells(row))
                {
                    if (row.Cells["status"].Value.ToString().Contains(Properties.strings.borrowed) && Boolean.Parse(settings.SelectSingleNode("/settings/borrowedItalic").InnerXml))
                    {
                        row.DefaultCellStyle.Font = new Font(tableFont, int.Parse(settings.SelectSingleNode("/settings/tableFontSize").InnerXml), FontStyle.Italic, GraphicsUnit.Point);
                    }
                    else
                    {
                        row.DefaultCellStyle.Font = new Font(tableFont, int.Parse(settings.SelectSingleNode("/settings/tableFontSize").InnerXml), GraphicsUnit.Point);
                    }
                }
            }

            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                if (row.Visible == true)
                {
                    i++;
                    if (dataGrid.RowCount > 0 && checkCells(row))
                    {
                        if (row.Cells["status"].Value.ToString().Contains(Properties.strings.borrowed) && Boolean.Parse(settings.SelectSingleNode("/settings/borrowedColorful").InnerXml))
                        {
                            row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableBorrowedColor").InnerXml);
                            row.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionBorrowedColor").InnerXml);
                        }
                        else
                        {
                            row.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionColor").InnerXml);
                            if (i % 2 == 0)
                            {
                                row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tablePrimeColor").InnerXml);
                            }
                            else
                            {
                                row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSecondColor").InnerXml);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region essentials

        public async void loadJoke() {
            string Joke = await getJoke();
            this.joke.Text = Joke;
            oldJokes.Add(this.joke.Text);

        }

        public List<string> oldJokes = new List<string>();
        private void joke_Click(object sender, EventArgs e)
        {
            //freeze when end reached
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left) {
                if (oldJokes.Contains(this.joke.Text))
                {
                    int p = oldJokes.IndexOf(this.joke.Text);
                    if (p < oldJokes.Count - 1)
                    {
                        this.joke.Text = oldJokes[p + 1];
                    }
                    else
                    {
                        if (oldJokes.Count >= jokeCount - 1)
                        {
                            this.joke.Text = oldJokes[0];
                        }
                        else
                        {
                            loadJoke();
                        }
                    }
                }

            }

            if (me.Button == MouseButtons.Right)
            {
                if (oldJokes.Count > 0)
                {
                    int p = oldJokes.IndexOf(this.joke.Text);
                    if (p - 1 < 0)
                    {
                        this.joke.Text = oldJokes[0];
                    }
                    else
                    {
                        this.joke.Text = oldJokes[p - 1];
                    }
                }
            }

        }

        private int jokeCount = 0;

        private async Task<string> getJoke() {
            List<string> jokes = new List<string>();

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var reader = new StreamReader(FileManager.GetDataBasePath("Files/" + settings.SelectSingleNode("/settings/csvFileName").InnerXml)))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(settings.SelectSingleNode("/settings/csvChar").InnerXml[0]);

                            jokes.Add(values[0]);
                        }
                    }
                }
                catch (Exception exc) {
                    this.BeginInvoke((Action)delegate
                    {
                        Program.logger.error(exc.Message, exc.StackTrace);
                    });
                }
            });
            Random r = new Random();

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
            else {
                joke = "";
            }
            return joke;
        }

        private async void backupAsync()
        {
            await backupData();
        }

        public void loadImagesToBuffer() {
            Program.logger.log("load images to buffer");
            images.Clear();
            document = loadDataBase();
            foreach (XmlNode node in document.SelectNodes("/data/item/image")) {
                if (node.InnerXml != "") {
                    images.Add(new ItemImage(Image.FromFile(FileManager.GetDataBasePath("Images\\" + node.InnerXml)), "Images\\" + node.InnerXml));
                }
            }
        }

        public bool checkFontInstalled(String fontName) {
            var fontsCollection = new InstalledFontCollection();
            foreach (var fontFamiliy in fontsCollection.Families)
            {
                if (fontFamiliy.Name == fontName) {
                    return true;
                }
            }
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
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 2000; // in miliseconds
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            check_name();
        }

        public void check_name() {
            if (!name_received) {
                XmlDocument session = new XmlDocument();
                session.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));
                String user_name = session.SelectSingleNode("/list/session/name").InnerText;
                user_loggedin = user_name;
                if (user_name != "")
                {
                    this.userDisplay.Text = Properties.strings.loggedInAs + " " + user_name;
                }

                XmlDocument user = new XmlDocument();
                session.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));
                String isAdmin = session.SelectSingleNode("/data/user[userName='" + user_name + "']/admin").InnerText;

                if (isAdmin.Equals("true"))
                {
                    um.Visible = true;
                    backup.Visible = true;
                }
                else if (isAdmin.Equals("false")) {
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
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
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
                {
                    dataGrid.Rows.Add(new DataGridViewRow());
                }
            }
            else {
                if (document.SelectNodes("data/item").Count == 0 && dataGrid.RowCount != 0) {
                    dataGrid.Rows.Clear();
                }
            }

            row = 0;
            foreach (XmlNode dm in document.SelectNodes("/data/item"))
            {
                String state = "";
                if (dm.SelectSingleNode("status").InnerXml != "1")
                {
                    state = Properties.strings.borrowed + " " + Properties.strings.by + " " + dm.SelectSingleNode("user").InnerXml;
                }
                else
                {
                    state = Properties.strings.available;
                }

                string[] item =
                    {
                        WebUtility.HtmlDecode(dm.SelectSingleNode("itemName").InnerXml),
                        WebUtility.HtmlDecode(dm.SelectSingleNode("barcode").InnerXml),
                        state,
                        WebUtility.HtmlDecode(dm.SelectSingleNode("comment").InnerXml),
                        System.Net.WebUtility.HtmlDecode(dm.SelectSingleNode("regal").InnerXml),
                        WebUtility.HtmlDecode(dm.SelectSingleNode("rate").InnerXml)
                    };
                dataGrid.Rows[row].Cells[0].Value = item[0];
                dataGrid.Rows[row].Cells[1].Value = item[1];
                dataGrid.Rows[row].Cells[2].Value = item[2];
                string firstLine = item[3].Substring(0, item[3].Contains(Environment.NewLine) ? item[3].IndexOf(Environment.NewLine) : item[3].Length);
                dataGrid.Rows[row].Cells[3].Value = firstLine;
                dataGrid.Rows[row].Cells[4].Value = item[4];
                dataGrid.Rows[row].Cells[5].Value = item[5];

                numOfItems++;
                if (dm.SelectSingleNode("status").InnerXml != "0")
                {
                    numOfAvailable++;
                }
                row++;
            }

            applyTableColors();
            CalculateScrollBar();
        }

        bool pic2_Visible_Before = false;

        public void showSideView(bool w) {

            if (!w)
            {
                if (pic_2.Visible)
                {
                    pic_2.Visible = false;
                    pic2_Visible_Before = true;
                }
            }
            else {
                if (pic2_Visible_Before) {
                    pic_2.Visible = true;
                }
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
        List<int> loadingProcesses = new List<int>();

        public void loadDataToView(int index) {

            document = loadDataBase();

            string selectedName = dataGrid.Rows[index].Cells[0].Value + string.Empty;
            string selectedBarcode = dataGrid.Rows[index].Cells[1].Value + string.Empty;
            string selectedComment = document.SelectSingleNode("/data/item[barcode='" + selectedBarcode + "']/comment").InnerText;
            string selectedState = dataGrid.Rows[index].Cells[2].Value + string.Empty;

            if (!ActiveBarcodes.Contains(selectedBarcode.Trim()))
            {
                ActiveBarcodes.Add(selectedBarcode.Trim());
                showSideView(true);

                Boolean State()
                {
                    if (selectedState.ToUpper() == Properties.strings.available.ToUpper())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                /*loadingProcesses.Clear();
                int processId = Environment.TickCount;
                loadingProcesses.Add(processId);
                */
                string img_uri_1 = document.SelectSingleNode("/data/item[barcode='" + selectedBarcode + "']/image").InnerText;
                float img_zoom_1 = float.Parse(document.SelectSingleNode("/data/item[barcode ='" + selectedBarcode + "']/image/@zoom").Value);
                string img_uri_2 = document.SelectSingleNode("/data/item[barcode='" + selectedBarcode + "']/image2").InnerText;
                float img_zoom_2 = float.Parse(document.SelectSingleNode("/data/item[barcode ='" + selectedBarcode + "']/image2/@zoom").Value);

                string regal = document.SelectSingleNode("/data/item[barcode='" + selectedBarcode + "']/regal").InnerText;

                string borrow_user = document.SelectSingleNode("/data/item[barcode='" + selectedBarcode + "']/user").InnerText;

                string _rating = document.SelectSingleNode("/data/item[barcode='" + selectedBarcode + "']/rate").InnerXml;
                int rating = _rating == "" ? 0 : int.Parse(_rating);


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
                    if (!pic_1.emptyImg)
                    {
                        pic_1.setEmpty();
                    }
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
                    {
                        pic_2.Visible = false;
                    }
                    else
                    {
                        pic_2.Visible = true;
                    }

                    if (!pic_2.emptyImg)
                    {
                        pic_2.setEmpty();
                    }
                }

                this.rating.Value = rating;


                rOption.Items.Clear();
                rOption.Items.Add("");

                foreach (XmlNode dm in rmDoc.SelectNodes("/list/regal"))
                {
                    String item = dm.SelectSingleNode("name").InnerXml;
                    rOption.Items.Add(item);

                }


                Boolean selectedStateBool = State();
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
            else {
                showSideView(false);
                resultDisplay.Text = "Ups, scheinbar wurde dieses Item schon in einer Remote geöffnet, veruche es später noch einmal.";
            }

        }

        public void refreshState(Boolean selectedStateBool, String borrow_user) {
            if (!selectedStateBool)
            {
                not_available.Text = Properties.strings.borrowed + " " + Properties.strings.by + " " + borrow_user;
                button4.Text = Properties.strings.back;
                not_available.Checked = true;
            }
            else
            {
                not_available.Text = Properties.strings.borrowed;
                button4.Text = Properties.strings.borrow;
                available.Checked = true;
            }
        }

        private void dataGrid_SelectionChanged(object sender, EventArgs e)
        {
         
            Task.Delay(100).ContinueWith(t =>
            {
                this.BeginInvoke(new Action(delegate
                {
                    
                    int totalRows = 0;
                    int indexOfSelected = 0;
                    if (dataGrid.SelectedRows.Count > 0)
                    {
                        if (!checkCells(dataGrid.SelectedRows[0])) {
                            return;
                        }

                        if (dataGrid.SelectedRows[0].Cells["status"].Value.ToString().Contains(Properties.strings.borrowed) && borrowedColorfull)
                        {
                            dataGrid.SelectedRows[0].DefaultCellStyle.SelectionBackColor = tableSelectionBorrowedColor;
                        }
                        else
                        {
                            dataGrid.SelectedRows[0].DefaultCellStyle.SelectionBackColor = tableSelectionColor;
                        }

                        if (lastSelectedRowIndex >= 0)
                        {
                            if (lastSelectedRowIndex >= dataGrid.RowCount) {
                                lastSelectedRowIndex = dataGrid.RowCount - 1;
                            }
                            if (dataGrid.Rows[lastSelectedRowIndex].Cells["status"].Value.ToString().Contains(Properties.strings.borrowed) && borrowedColorfull)
                            {
                                dataGrid.Rows[lastSelectedRowIndex].DefaultCellStyle.SelectionBackColor = tableSelectionBorrowedColor;
                            }
                            else
                            {
                                dataGrid.Rows[lastSelectedRowIndex].DefaultCellStyle.SelectionBackColor = tableSelectionColor;
                            }

                            if (ActiveBarcodes.Contains(dataGrid.Rows[lastSelectedRowIndex].Cells["barcode"].Value.ToString()))
                            {
                                ActiveBarcodes.Remove(dataGrid.Rows[lastSelectedRowIndex].Cells["barcode"].Value.ToString());
                            }
                        }

                        if ((lastSelectedRowIndex != dataGrid.SelectedRows[0].Index || !barcode_box.Visible) && !refreshing)
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
                        else {
                            totalRows = dataGrid.RowCount;
                        }

                    }
                    else
                    {
                        if (!refreshing)
                        {
                            showSideView(false);
                            resultDisplay.Text = Properties.strings.nothingSelected;
                            int total_rows = dataGrid.Rows.Count;
                            curNum.Text = "0 / " + total_rows;
                        }
                    }
                }));
            });
        }

        private void refreshImage(String img_uri) {
            Image img = getImage("img\\noimage.png");

            if (img_uri != "")
            {
                img = getImage("img\\" + img_uri);
            }
            else
            {
                img = getImage("img\\noimage.png");
            }

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

        public void refresh(bool refreshAnyWay) {
            refresh(refreshAnyWay, true, false);
        }

        public void refresh(bool refreshAnyWay, bool refreshSearch) {
            refresh(refreshAnyWay, refreshSearch, false);
        }

        int lastFirstDisplayedRowIndex = 0;
        public void refresh(bool refreshAnyWay, bool refreshSearch, bool keepViewport)
        {
            document = loadDataBase();
            XmlElement root = document.DocumentElement;
            XmlNodeList ElementList = root.GetElementsByTagName("item");
            if (ElementList.Count <= 0) {
                return;
            }

            if (refreshAnyWay) {
                changed = true;
            }


            lastFirstDisplayedRowIndex = dataGrid.FirstDisplayedScrollingRowIndex;

            rmDoc = loadRoomManagerDataBase();

            refreshing = true;
            int selectedCount = 0;

            SortOrder oldSortOrder = dataGrid.SortOrder;
            DataGridViewColumn oldSortColumn = dataGrid.SortedColumn;

            foreach (DataGridViewRow row in dataGrid.SelectedRows)
            {
                selectedCount++;
            }

            int selectedRow = 0;

            if (selectedCount > 0)
            {

                if (!checkCells(dataGrid.SelectedRows[0])) {
                    return;
                }
                int distance = 0;

                selectedRow = dataGrid.SelectedRows[0].Index;
                try
                {
                    distance = dataGrid.SelectedRows[0].Index - dataGrid.FirstDisplayedCell.RowIndex;
                }
                catch(Exception e)
                {
                    Program.logger.error(e.Message);
                    distance = 0;

                }

                //selectedRow equals 0 if called from add_view.cs

                int items = rmDoc.SelectNodes("/data/item").Count;
                int curRows = dataGrid.Rows.Count;

                for (int i = 0; i <= items - curRows; i++)
                {
                    dataGrid.Rows.Add(new DataGridViewRow());
                }
                loadData();

            }
            else {
                int items = rmDoc.SelectNodes("/data/item").Count;
                int curRows = dataGrid.Rows.Count;
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Value = "";
                    }
                }
                for (int i = 0; i <= items - curRows; i++)
                {
                    dataGrid.Rows.Add(new DataGridViewRow());
                }
                loadData();
            }


            if (oldSortColumn != null)
            {
                if (oldSortOrder == SortOrder.Ascending)
                {
                    dataGrid.Sort(oldSortColumn, System.ComponentModel.ListSortDirection.Ascending);
                }
                else
                {
                    dataGrid.Sort(oldSortColumn, System.ComponentModel.ListSortDirection.Descending);
                }
            }

            Console.WriteLine(selectedCount + " " + selectedRow);
            this.dataGrid.Rows[selectedRow].Selected = true;
            this.curNum.Text = (dataGrid.SelectedRows.Count > 0 ? dataGrid.SelectedRows[0].Index + 1 : 0) + " / " + dataGrid.RowCount;

            if (refreshSearch) {
                if (currentMatrix == SearchMatrix.BARCODE && Boolean.Parse(settings.SelectSingleNode("/settings/searchExact").InnerXml))
                {
                    resetSearch(true, keepViewport);
                }
                else {
                    search(currentMatrix, currentMode);
                }

            }


            if (keepViewport)
            {
                resort(true);
            }
            else {
                resort(false);
            }
            refreshing = false;

            if (keepViewport) {
                dataGrid.FirstDisplayedScrollingRowIndex = DisplayedRows.FirstOrDefault().Index;

                DisplayedRows.Clear();
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    if (row.Displayed)
                    {
                        if (!DisplayedRows.Contains(row))
                        {
                            DisplayedRows.Add(row);
                        }

                    }


                }
            }
        }

        #endregion

        #region search

        private SearchMatrix currentMatrix;
        private SearchMode currentMode;

        private enum SearchMatrix { BARCODE, NAME, REMARK };
        private enum SearchMode { FILTER, MARKUP };

        string searchString;
        int numOfResults = 0;


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
            if (Boolean.Parse(settings.SelectSingleNode("/settings/searchWhileType").InnerXml)) {
                resetSearch(false);
                search(currentMatrix, currentMode);
            }

            if (e.KeyCode == Keys.Enter) {
                search(currentMatrix, currentMode);
            }
            if (customSearchBox1.Text == "") {
                resetSearch(false);
            }
        }

        private void resetSearch(bool emptyText) {
            resetSearch(emptyText, false);
        }
        private void resetSearch(bool emptyText, bool keepView) {
            if (emptyText) customSearchBox1.Text = "";
            customSearchBox1.searchInAction = false;
            customSearchBox1.Invalidate();
            refresh(false, false, keepView);
            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                row.DefaultCellStyle.ForeColor = Color.Black;
            }
            this.resDisplay.Text = Properties.strings.noResults;
        }

        private RichTextBox rtfBox = new RichTextBox();

        private void search(SearchMatrix matrix, SearchMode mode) {

            bool caseSensitive = Boolean.Parse(settings.SelectSingleNode("/settings/searchCaseSensitive").InnerXml);

            if (customSearchBox1.Text == "") {
                return;
            }
            else
            {
                if (caseSensitive)
                {
                    searchString = customSearchBox1.Text;
                }
                else {
                    searchString = customSearchBox1.Text.ToUpper();
                }
            }

            customSearchBox1.searchInAction = true;

            string col = "";

            if (matrix == SearchMatrix.NAME)
            {
                col = "name";
            }
            else if (matrix == SearchMatrix.BARCODE)
            {
                col = "barcode";
            }
            else
            {
                col = "comment";
            }

            if (mode == SearchMode.FILTER)
            {
                //FILTER
                dataGrid.SuspendLayout();

                var toBeDeleted = new List<DataGridViewRow>();
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    string comment = "";
                    if (matrix == SearchMatrix.REMARK && File.Exists(FileManager.GetDataBasePath("Comments/" + row.Cells["barcode"].Value.ToString() + ".rtf"))) {
                        rtfBox.Rtf = File.ReadAllText(FileManager.GetDataBasePath("Comments/" + row.Cells["barcode"].Value.ToString() + ".rtf"));
                        comment = rtfBox.Text;
                    }
                    if (matrix == SearchMatrix.BARCODE && Boolean.Parse(settings.SelectSingleNode("/settings/searchExact").InnerXml))
                    {
                        string comp = caseSensitive ? row.Cells[col].Value.ToString() : row.Cells[col].Value.ToString().ToUpper();
                        if (comp.Equals(searchString))
                        {
                            row.Selected = true;
                            
                            numOfResults = 1;
                            if (dataGrid.SelectedRows[0].Index > 10)
                            {
                                dataGrid.FirstDisplayedScrollingRowIndex = dataGrid.SelectedRows[0].Index - 10;
                            }
                            else
                            {
                                dataGrid.FirstDisplayedScrollingRowIndex = 0;
                            }
                        }
                    }
                    else {
                        string comp = caseSensitive ? row.Cells[col].Value.ToString() : row.Cells[col].Value.ToString().ToUpper();
                        string compcom = caseSensitive ? comment : comment.ToUpper();
                        if (!compcom.Contains(searchString) && !comp.Contains(searchString))
                        {
                            toBeDeleted.Add(row);
                        }
                    }


                }

                toBeDeleted.ForEach(d => dataGrid.Rows.Remove(d));
                if (toBeDeleted.Count > 0)
                {
                    numOfResults = dataGrid.RowCount;
                }
                else {
                    numOfResults = 0;
                }

                dataGrid.ResumeLayout();
                this.resDisplay.Width = 150;
                this.resDisplay.Text = numOfResults + " " + Properties.strings.found;
                applyTableColors();
                CalculateScrollBar();
                if (dataGrid.RowCount > 0)
                {
                    dataGrid.Rows[0].Selected = true;
                }

            }
            else
            {

                //MARKUP
                List<int> resultRowsIndex = new List<int>();
                numOfResults = 0;
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    string comment = "";
                    if (matrix == SearchMatrix.REMARK && File.Exists(FileManager.GetDataBasePath("Comments/" + row.Cells["barcode"].Value.ToString() + ".rtf")))
                    {
                        rtfBox.Rtf = File.ReadAllText(FileManager.GetDataBasePath("Comments/" + row.Cells["barcode"].Value.ToString() + ".rtf"));
                        comment = rtfBox.Text;
                    }

                    if (matrix == SearchMatrix.BARCODE && Boolean.Parse(settings.SelectSingleNode("/settings/searchExact").InnerXml))
                    {
                        string comp = caseSensitive ? row.Cells[col].Value.ToString() : row.Cells[col].Value.ToString().ToUpper();

                        if (comp.Equals(searchString))
                        {
                            row.Selected = true;
                            if (dataGrid.SelectedRows[0].Index > 10)
                            {
                                dataGrid.FirstDisplayedScrollingRowIndex = dataGrid.SelectedRows[0].Index - 10;
                            }
                            else
                            {
                                dataGrid.FirstDisplayedScrollingRowIndex = 0;
                            }
                        }
                    }
                    else {
                        string comp = caseSensitive ? row.Cells[col].Value.ToString() : row.Cells[col].Value.ToString().ToUpper();
                        string compcom = caseSensitive ? comment : comment.ToUpper();
                        if (comp.Contains(searchString) || compcom.Contains(searchString))
                        {
                            row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/" + col + "SearchColor").InnerXml);
                            row.DefaultCellStyle.ForeColor = Color.White;
                            numOfResults++;
                            resultRowsIndex.Add(row.Index);

                        }
                    }

                }
                if (resultRowsIndex.Count > 0 && Boolean.Parse(settings.SelectSingleNode("/settings/scrollToSearch").InnerXml))
                {
                    if (resultRowsIndex[0] > 10)
                    {
                        dataGrid.FirstDisplayedScrollingRowIndex = resultRowsIndex[0] - 10;
                    }
                    else
                    {
                        dataGrid.FirstDisplayedScrollingRowIndex = 0;
                    }
                }
                this.resDisplay.Width = 150;
                this.resDisplay.Text = numOfResults + " " + Properties.strings.found;

            }

            if (numOfResults == 0)
            {
                resetSearch(false);
            }

        }




        #endregion


        private void dataGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                DataGridView.HitTestInfo hit = dataGrid.HitTest(e.X, e.Y);
                if (hit.Type == DataGridViewHitTestType.None)
                {
                    dataGrid.ClearSelection();
                    dataGrid.CurrentCell = null;
                }
            }
        }



        private void add_Click(object sender, EventArgs e)
        {
            Main main = this;
            var addView = new add_view(main, "");
            addView.Show();
        }

        private void delete_button_Click(object sender, EventArgs e)
        {
            delete();
        }

        public async void delete() {

            int selectedRows = 0;
            foreach (DataGridViewRow row in dataGrid.SelectedRows)
            {
                selectedRows++;
            }

            if (selectedRows > 0)
            {
                if (DialogResult.Yes == MessageBox.Show(Properties.strings.deleteLine, Properties.strings.submit, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    int rowNumber = dataGrid.Rows.Count;
                    int curRow = rowNumber;
                    foreach (DataGridViewRow row in dataGrid.SelectedRows)
                    {
                        String temp_barcode = dataGrid.Rows[row.Index].Cells[1].Value + String.Empty;
                        if (ActiveBarcodes.Contains(temp_barcode)) {
                            ActiveBarcodes.Remove(temp_barcode);
                        }
                        curRow = row.Index;
                        document = loadDataBase();


                        String XPath = "/data/item[barcode='" + temp_barcode + "']";
                        XmlNode node = document.SelectSingleNode(XPath);
                        node.ParentNode.RemoveChild(node);
                        await SaveAsync(document, FileManager.NAMES.DATA);
                        dataGrid.Rows.RemoveAt(row.Index);

                        if (File.Exists(FileManager.GetDataBasePath("Comments/" + temp_barcode + ".rtf"))) {
                            File.Delete(FileManager.GetDataBasePath("Comments/" + temp_barcode + ".rtf"));
                        }
                    }
                    if (curRow - 1 >= 0)
                    {
                        dataGrid.Rows[curRow - 1].Selected = true;
                    }
                    refresh(false);
                }
            }
            else
            {
                MessageBox.Show(Properties.strings.noRowSelected, Properties.strings.error + ":", MessageBoxButtons.OK);
            }
        }

        public void select(string barcode_select) {
            dataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            if (barcode_select != "")
            {
                try
                {
                    foreach (DataGridViewRow row in dataGrid.Rows)
                    {
                        if (row.Cells[1].Value.ToString().Equals(barcode_select))
                        {
                            lastSelectedRowIndex = dataGrid.SelectedRows[0].Index;
                            dataGrid.ClearSelection();
                            row.Selected = true;
                            if (row.Index >= 10)
                            {
                                dataGrid.FirstDisplayedScrollingRowIndex = row.Index - 10;
                            }
                            else
                            {
                                dataGrid.FirstDisplayedScrollingRowIndex = 0;
                            }
                            loadDataToView(row.Index);
                        }
                    }
                }
                catch (Exception exc)
                {
                    Program.logger.error(exc.Message, exc.StackTrace);
                }
            }
        }

        private void save_changes_Click(object sender, EventArgs e)
        {
            saveChanges();
        }

        int c;
        public async void saveChanges() {
            c++;
            await Task.Delay(50).ContinueWith(t =>
            {
                if (barcode_box.Text != "" && name_box.Text != "")
                {

                    string barcode_send = barcode_box.Text;
                    string name_send = name_box.Text;
                    string image_1 = pic_1.FileName;
                    string image_2 = pic_2.FileName;
                    string zoom_1 = Math.Round(pic_1.ZoomValue) + "";
                    string zoom_2 = Math.Round(pic_2.ZoomValue) + "";
                    string comment = "-";
                    string regal = rOption.Text;

                    string user = user_loggedin;
                    string rate = this.rating.Value + "";
                    string commentUrl = FileManager.GetDataBasePath("Comments/" + barcode_send + ".rtf");

                    this.BeginInvoke((Action)delegate {
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

                    this.BeginInvoke((Action)async delegate {

                        String XPathName = "/data/item[barcode='" + barcode_send + "']/itemName";
                        XmlNode node_name = document.SelectSingleNode(XPathName);
                        node_name.InnerXml = name_send;

                        if (pic_1.Visible && !pic_1.emptyImg)
                        {
                            String XPathImg = "/data/item[barcode='" + barcode_send + "']/image";
                            XmlNode node_img = document.SelectSingleNode(XPathImg);
                            XmlAttribute attr_zoom = node_img.Attributes[0];
                            node_img.InnerXml = image_1;
                            attr_zoom.Value = zoom_1;
                        }

                        if (pic_2.Visible && !pic_2.emptyImg)
                        {
                            String XPathImg2 = "/data/item[barcode='" + barcode_send + "']/image2";
                            XmlNode node_img2 = document.SelectSingleNode(XPathImg2);
                            XmlAttribute attr_zoom2 = node_img2.Attributes[0];
                            node_img2.InnerXml = image_2;
                            attr_zoom2.Value = zoom_2;
                        }


                        String XPathCom = "/data/item[barcode='" + barcode_send + "']/comment";
                        XmlNode node_com = document.SelectSingleNode(XPathCom);
                        node_com.InnerXml = comment;

                        String XPathReg = "/data/item[barcode='" + barcode_send + "']/regal";
                        XmlNode node_reg = document.SelectSingleNode(XPathReg);
                        node_reg.InnerXml = regal;

                        String XPathRate = "/data/item[barcode='" + barcode_send + "']/rate";
                        XmlNode node_rate = document.SelectSingleNode(XPathRate);
                        node_rate.InnerXml = rate;

                        if (available.Checked)
                        {
                            String barcode = barcode_box.Text;
                            String XPath = "/data/item[barcode='" + barcode + "']/status";
                            String userPath = "/data/item[barcode='" + barcode + "']/user";
                            XmlNode node = document.SelectSingleNode(XPath);
                            XmlNode user_node = document.SelectSingleNode(userPath);
                            node.InnerXml = "1";
                            user_node.InnerXml = "";
                        }
                        else
                        {
                            String barcode = barcode_box.Text;
                            String XPath = "/data/item[barcode='" + barcode + "']/status";
                            String userPath = "/data/item[barcode='" + barcode + "']/user";
                            XmlNode node = document.SelectSingleNode(XPath);
                            XmlNode user_node = document.SelectSingleNode(userPath);
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


        private void checkSaved() {

            if (barcode_box.Text != "")
            {
                String sb = barcode_box.Text;

                document = loadDataBase();


                string regal = document.SelectSingleNode("/data/item[barcode='" + sb + "']/regal").InnerText;
                string name = document.SelectSingleNode("/data/item[barcode='" + sb + "']/itemName").InnerText;

                string regal_new = rOption.Text;
                string name_new = name_box.Text;

                if (regal == regal_new && name == name_new)
                {
                    saved();
                }
                else {
                    unsaved();
                }

            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            delete();
        }

        private void saved() {
            save_changes.Iconimage = allinthebox.Properties.Resources.check;
        }
        private void unsaved() {
            save_changes.Iconimage = allinthebox.Properties.Resources.red_cross;
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
            if (!customSearchBox1.Focused)
            {
                listener.barcode_listener(sender, e, this);
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (button4.Text.ToLower() == Properties.strings.borrow.ToLower())
            {
                refreshState(false, user_loggedin);
                saveChanges();
            }
            else {
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
            var Rm = new allinthebox.rm(this);
            if (!currentOpenWindows.Contains(Rm.Name) || winOpenOnce)
            {
                Rm.Show();
                //roomManager2 manager2 = new roomManager2();
                //manager2.Show();
            }
            else if (currentOpenWindows.Contains(Rm.Name))
            {
                Application.OpenForms[Rm.Name].BringToFront();
            }
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == saveHotKey.key && (e.Control || !saveHotKey.controlNeeded) && (e.Alt || !saveHotKey.altNeeded) && (e.Shift || !saveHotKey.shiftNeeded)) {
                saveChanges();
                saved();
            }

            if (e.KeyCode == borrowHotKey.key && (e.Control || !borrowHotKey.controlNeeded) && (e.Alt || !borrowHotKey.altNeeded) && (e.Shift || !borrowHotKey.shiftNeeded)) {
                if (button4.Text.ToLower() == Properties.strings.borrow.ToLower())
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

            if (e.KeyCode == delHotKey.key && (e.Control || !delHotKey.controlNeeded) && (e.Alt || !delHotKey.altNeeded) && (e.Shift || !delHotKey.shiftNeeded)) {
                delete();
            }

            if (e.KeyCode == reloadHotKey.key && (e.Control || !reloadHotKey.controlNeeded) && (e.Alt || !reloadHotKey.altNeeded) && (e.Shift || !reloadHotKey.shiftNeeded)) {
                refresh(true);
            }

            if (e.KeyCode == versionHotKey.key && (e.Control || !versionHotKey.controlNeeded) && (e.Alt || !versionHotKey.altNeeded) && (e.Shift || !versionHotKey.shiftNeeded)) {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                MessageBox.Show(version.ToString());
            }

            if (e.KeyCode == helpHotKey.key && (e.Control || !helpHotKey.controlNeeded) && (e.Alt || !helpHotKey.altNeeded) && (e.Shift || !helpHotKey.shiftNeeded)) {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                help h = new help($"{version}", this);
                if (!currentOpenWindows.Contains(h.Name) || winOpenOnce)
                {
                    h.Show();
                }
                else if (currentOpenWindows.Contains(h.Name))
                {
                    Application.OpenForms[h.Name].BringToFront();
                }
            }
        }

        private void um_Click(object sender, EventArgs e)
        {
            userManager um = new userManager(this);
            if (!currentOpenWindows.Contains(um.Name) || winOpenOnce)
            {
                um.Show();
            }
            else if (currentOpenWindows.Contains(um.Name)) {
                Application.OpenForms[um.Name].BringToFront();
            }

        }

        private void backup_Click(object sender, EventArgs e)
        {
            if (!currentOpenWindows.Contains(BM.Name) || winOpenOnce)
            {
                BM.Show();
            }
            else if (currentOpenWindows.Contains(BM.Name))
            {
                Application.OpenForms[BM.Name].BringToFront();
            }
        }

        public async Task backupData() {
            await Task.Factory.StartNew(delegate
            {

                string folderName = DateTime.Now.ToString("dd.MM.yyyy (HH-mm-ss)");
                Directory.CreateDirectory(FileManager.GetDataBasePath("Backup\\" + folderName));
                System.IO.File.Copy(FileManager.GetDataBasePath(FileManager.NAMES.DATA), FileManager.GetDataBasePath("Backup\\" + folderName + "\\Data.xml"), true);
                System.IO.File.Copy(FileManager.GetDataBasePath(FileManager.NAMES.RACKS), FileManager.GetDataBasePath("Backup\\" + folderName + "\\Racks.xml"), true);
                System.IO.File.Copy(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS), FileManager.GetDataBasePath("Backup\\" + folderName + "\\Settings.xml"), true);

                settings = loadSettingsDataBase();

                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                settings.SelectSingleNode("/settings/lastUpdate").InnerXml = date.Replace('.', '/');
            });
            await SaveAsync(settings, FileManager.NAMES.SETTINGS);

            BM.refreshUI();
            Program.logger.log("BackupManager", "Backup saved", Color.Green);

        }

        private void logBut_Click(object sender, EventArgs e)
        {
            if (!currentOpenWindows.Contains(Program.logger.Name) || winOpenOnce)
            {
                Program.logger.Show();
            }
            else if (currentOpenWindows.Contains(Program.logger.Name))
            {
                Application.OpenForms[Program.logger.Name].BringToFront();
            }
        }

        private void dataGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {


                contextMenuDataGrid.Show(Cursor.Position);
            }
        }


        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            remoteServer.StopServer();
            Program.logger.log(Properties.strings.systemClose + " [" + e.CloseReason + "]", Color.Green);
            Program.logger.log("******************************************", Color.Gray);
            Program.logger.finish();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            info_copy info_Copy = new info_copy(this);
            if (!currentOpenWindows.Contains(info_Copy.Name) || winOpenOnce)
            {
                info_Copy.Show();
            }
        }





        private void sync_Click(object sender, EventArgs e)
        {
            System.Net.WebClient Client = new System.Net.WebClient();
            Client.Headers.Add("Content-Type", "binary/octet-stream");

            byte[] result = Client.UploadFile("https://polymation.tk/allinthebox/reciever.php", "POST", FileManager.GetDataBasePath(FileManager.NAMES.DATA));

            String s = UTF8Encoding.UTF8.GetString(result, 0, result.Length);
            MessageBox.Show(Properties.strings.refreshedFile);
        }

        private void contextMenuDataGrid_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            ToolStripItemClickedEventArgs tsc = (ToolStripItemClickedEventArgs)e;

            if (tsc.ClickedItem.Text.Equals(Properties.strings.menuDeleteRow))
            {
                contextMenuDataGrid.Hide();
                delete();
            }
            else if (tsc.ClickedItem.Text.Equals(Properties.strings.add))
            {
                add_view av = new add_view(this, "");
                av.Show();
            }
            else if (tsc.ClickedItem.Text.Equals(Properties.strings.refresh))
            {
                refresh(true);
            }
            else if (tsc.ClickedItem.Text.Equals(Properties.strings.menuBorrow))
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

            XmlDocument isAminDOc = new XmlDocument();
            isAminDOc.Load(FileManager.GetDataBasePath(FileManager.NAMES.USER));

            XmlNode isAdminNode = isAminDOc.SelectSingleNode("/data/user[userName='" + user_loggedin + "']/admin");

            if (isAdminNode.InnerXml.ToLower().Equals("true"))
            {
                s = new settings(true, this);
            }
            else if (isAdminNode.InnerXml.ToLower().Equals("false"))
            {
                s = new settings(false, this);
            }
            if (!currentOpenWindows.Contains(s.Name) || winOpenOnce)
            {
                s.Show();
            }
            else if (currentOpenWindows.Contains(s.Name))
            {
                Application.OpenForms[s.Name].BringToFront();
            }
        }

        private void multiBorrow_Click(object sender, EventArgs e)
        {
            multiBorrow mb = new multiBorrow(this);
            if (!currentOpenWindows.Contains(mb.Name) || winOpenOnce)
            {
                mb.Show();
            }
            else if (currentOpenWindows.Contains(mb.Name))
            {
                Application.OpenForms[mb.Name].BringToFront();
            }
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

        public XmlDocument loadDataBase() {
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
            {
                Normalize();
            }
            else
            {
                Maximize();
            }
        }

        private void header_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown && maximized)
            {
                grabX = (MousePosition.X - this.DesktopLocation.X);
                grabY = (MousePosition.Y - this.DesktopLocation.Y);
                this.Size = new Size(this.MinimumSize.Width, this.MinimumSize.Height);
                this.SetBounds(MousePosition.X - this.Width / 2, MousePosition.Y - 5, this.MinimumSize.Width, this.MinimumSize.Height);
                maximized = false;
                if (Style.iconStyle == Style.IconStyle.LIGHT)
                {
                    this.maximizeButton.Image = allinthebox.Properties.Resources.maximize;
                }
                else {
                    this.maximizeButton.Image = allinthebox.Properties.Resources.maximize_white;
                }
                this.maximizeButton.Padding = new Padding(4, 4, 4, 4);
            }
            if (mousedown && !maximized)
            {
                mouseX = MousePosition.X - grabX;
                mouseY = MousePosition.Y - grabY;
                this.SetDesktopLocation(mouseX, mouseY);
            }
        }

        private void header_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
        }

        private void header_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            grabX = (MousePosition.X - this.DesktopLocation.X);
            grabY = (MousePosition.Y - this.DesktopLocation.Y);
        }

        private void Normalize()
        {
            maximized = false;
            if (Style.iconStyle == Style.IconStyle.LIGHT)
            {
                this.maximizeButton.Image = allinthebox.Properties.Resources.maximize;
            }
            else
            {
                this.maximizeButton.Image = allinthebox.Properties.Resources.maximize_white;
            }
            this.maximizeButton.Padding = new Padding(4, 4, 4, 4);
            this.SetBounds(lastDim.X, lastDim.Y, lastDim.Width, lastDim.Height);
        }

        private void Maximize()
        {
            maximized = true;
            if (Style.iconStyle == Style.IconStyle.LIGHT)
            {
                this.maximizeButton.Image = allinthebox.Properties.Resources.restore;
            }
            else
            {
                this.maximizeButton.Image = allinthebox.Properties.Resources.restore_white;
            }
            this.maximizeButton.Padding = new Padding(5, 5, 5, 5);
            lastDim = new Rectangle(this.DesktopLocation.X, this.DesktopLocation.Y, this.Width, this.Height);

            int screenX = Screen.FromControl(this).Bounds.X;
            int screenY = Screen.FromControl(this).Bounds.Y;
            int screenWidth = Screen.FromControl(this).Bounds.Width;
            int screenHeight = Screen.FromControl(this).Bounds.Height;
            int TaskBarHeight = 0;
            Rectangle screen = new Rectangle();

            if (GetTaskBarLocation() == TaskBarLocation.BOTTOM)
            {
                TaskBarHeight = TaskbarHook.TaskBarFactory.GetTaskbar().Rectangle.Bottom - TaskbarHook.TaskBarFactory.GetTaskbar().Rectangle.Top;
                screen = new Rectangle(screenX, screenY, screenWidth, screenHeight - TaskBarHeight);
            }
            else if (GetTaskBarLocation() == TaskBarLocation.LEFT)
            {
                TaskBarHeight = TaskbarHook.TaskBarFactory.GetTaskbar().Rectangle.Right;
                screen = new Rectangle(screenX, screenY, screenWidth - TaskBarHeight, screenHeight);
            }
            else if (GetTaskBarLocation() == TaskBarLocation.RIGHT)
            {
                TaskBarHeight = TaskbarHook.TaskBarFactory.GetTaskbar().Rectangle.Left - TaskbarHook.TaskBarFactory.GetTaskbar().Rectangle.Right;
                screen = new Rectangle(screenX, screenY, screenWidth - TaskBarHeight, screenHeight);

            }
            else if (GetTaskBarLocation() == TaskBarLocation.TOP)
            {
                TaskBarHeight = TaskbarHook.TaskBarFactory.GetTaskbar().Rectangle.Bottom;
                screen = new Rectangle(screenX, screenY, screenWidth, screenHeight - TaskBarHeight);

            }

            this.SetBounds(screen.X, screen.Y, screen.Width, screen.Height);


        }

        private enum TaskBarLocation { TOP, BOTTOM, LEFT, RIGHT }

        private TaskBarLocation GetTaskBarLocation()
        {
            TaskBarLocation taskBarLocation = TaskBarLocation.BOTTOM;
            bool taskBarOnTopOrBottom = (Screen.PrimaryScreen.WorkingArea.Width == Screen.PrimaryScreen.Bounds.Width);
            if (taskBarOnTopOrBottom)
            {
                if (Screen.PrimaryScreen.WorkingArea.Top > 0) taskBarLocation = TaskBarLocation.TOP;
            }
            else
            {
                if (Screen.PrimaryScreen.WorkingArea.Left > 0)
                {
                    taskBarLocation = TaskBarLocation.LEFT;
                }
                else
                {
                    taskBarLocation = TaskBarLocation.RIGHT;
                }
            }
            return taskBarLocation;
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

        private void CalculateScrollBar()
        {
            int DisplayedRows;
            int rowCount = 0;
            foreach (DataGridViewRow row in dataGrid.Rows) {
                if (row.Visible) rowCount++;
            }
            if (rowCount > 0)
            {
                double a = (this.dataGrid.Height - this.dataGrid.ColumnHeadersHeight) / rowHeight;
                DisplayedRows = this.dataGrid.DisplayedRowCount(false) > 0 ? this.dataGrid.DisplayedRowCount(false) : (int)Math.Floor(a);
                if (DisplayedRows >= rowCount) {
                    DisplayedRows = rowCount;
                }

            }
            else {
                DisplayedRows = 0;
            }
            this.customScrollbar1.Value = 0;
            this.customScrollbar1.Minimum = 0;
            this.customScrollbar1.LargeChange = 5;
            this.customScrollbar1.ExtraSmallChange = 5;
            this.customScrollbar1.SmallChange = this.customScrollbar1.ExtraSmallChange * 7;
            this.customScrollbar1.Maximum = rowCount - DisplayedRows + this.customScrollbar1.LargeChange;

            this.customScrollbar1.Value = Math.Abs(this.dataGrid.FirstDisplayedScrollingRowIndex);
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            this.header.Width = this.Width + 5;
            this.footer.Width = this.Width - this.sidebar.Width + 5;
            this.info.Location = new Point(this.Width - this.info.Width + 5, this.header.Height);
            this.info.Height = this.Height - (this.footer.Height + this.header.Height);

            this.dataGrid.Width = this.Width - (this.sidebar.Width + this.info.Width) + 5;
            this.dataGrid.Height = this.Height - (this.header.Height + this.footer.Height + 50);
            this.dataGrid.Location = new Point(this.sidebar.Width, this.header.Height);

            this.quickButton.Height = this.Height - (footer.Height + dataGrid.Height + header.Height);
            this.quickButton.Width = this.Width - (this.sidebar.Width + this.info.Width) + 5;
            this.quickButton.Location = new Point(this.sidebar.Width, this.header.Height + this.dataGrid.Height);

            this.footer.Location = new Point(this.sidebar.Width, this.header.Height + this.dataGrid.Height + this.quickButton.Height);

            this.scrollHolder.Width = SystemInformation.VerticalScrollBarWidth;
            this.scrollHolder.Height = this.dataGrid.Height;
            this.customScrollbar1.Height = this.scrollHolder.Height;
            this.scrollHolder.Location = new Point(this.sidebar.Width + this.dataGrid.Width - this.scrollHolder.Width, this.header.Height);

            this.barcode_box.Location = new Point(this.barcode_label.Location.X + this.barcode_label.Width, this.barcode_label.Location.Y);

            this.sidebar.Invalidate();
            this.dataGrid.Invalidate();

            int b = 0;
            this.closeButton.Location = new Point(this.Width - this.closeButton.Width + b, this.closeButton.Location.Y);
            this.maximizeButton.Location = new Point(this.Width - (2 * this.closeButton.Width) + b, this.maximizeButton.Location.Y);
            this.minimizeButton.Location = new Point(this.Width - (3 * this.closeButton.Width) + b, this.minimizeButton.Location.Y);
            this.helpButton.Location = new Point(this.Width - (4 * this.closeButton.Width) + b, this.helpButton.Location.Y);

            this.separator1.StrokeSize = this.separator2.StrokeSize = new Size(this.customSearchBox1.Width, 1);
            this.separator3.StrokeSize = this.separator4.StrokeSize = this.separator8.StrokeSize = this.separator5.StrokeSize = this.separator6.StrokeSize = this.separator7.StrokeSize = new Size(this.name_box.Width, 1);
        }

        private const int
        HTLEFT = 10,
        HTRIGHT = 11,
        HTTOP = 12,
        HTTOPLEFT = 13,
        HTTOPRIGHT = 14,
        HTBOTTOM = 15,
        HTBOTTOMLEFT = 16,
        HTBOTTOMRIGHT = 17;

        const int _ = 10;

        new Rectangle Top { get { return new Rectangle(0, 0, this.ClientSize.Width, _); } }
        new Rectangle Left { get { return new Rectangle(0, 0, _, this.ClientSize.Height); } }

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
            tt.ShowAlways = true;
            tt.ShowAlways = true;
            tt.SetToolTip(this.closeButton, Properties.strings.close);
            this.closeButton.BackColor = Color.FromArgb(223, 1, 1);
            this.closeButton.Image = allinthebox.Properties.Resources.close_white;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            this.closeButton.BackColor = Color.Transparent;
            if (Style.iconStyle == Style.IconStyle.LIGHT)
            {
                this.closeButton.Image = allinthebox.Properties.Resources.close;
            }
            else
            {
                this.closeButton.Image = allinthebox.Properties.Resources.close_white;
            }

        }

        private void maximizeButton_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left)
            {
                if (maximized)
                {
                    Normalize();
                }
                else
                {
                    Maximize();
                }
            }
        }
        ToolTip ttMax = new ToolTip();

        private void maximizeButton_MouseEnter(object sender, EventArgs e)
        {
            if (maximized)
            {
                ttMax.SetToolTip(this.maximizeButton, Properties.strings.windowNormal);
            }
            else {
                ttMax.SetToolTip(this.maximizeButton, Properties.strings.windowMaximize);
            }

            this.maximizeButton.BackColor = highlightButton;
        }

        private void maximizeButton_MouseLeave(object sender, EventArgs e)
        {
            ttMax.RemoveAll();
            this.maximizeButton.BackColor = Color.Transparent;
        }

        private void minimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void minimizeButton_MouseEnter(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.ShowAlways = true;
            tt.SetToolTip(this.minimizeButton, Properties.strings.windowMinimize);
            this.minimizeButton.BackColor = highlightButton;
        }

        private void minimizeButton_MouseLeave(object sender, EventArgs e)
        {
            this.minimizeButton.BackColor = Color.Transparent;
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            help h = new help($"{version}", this);
            if (!currentOpenWindows.Contains(h.Name) || winOpenOnce)
            {
                h.Show();
            }
            else if (currentOpenWindows.Contains(h.Name))
            {
                Application.OpenForms[h.Name].BringToFront();
            }
        }

        private void helpButton_MouseEnter(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.ShowAlways = true;
            tt.ShowAlways = true;
            tt.SetToolTip(this.helpButton, Properties.strings.help);
            this.helpButton.BackColor = Color.FromArgb(0, 96, 170);
            this.helpButton.Image = allinthebox.Properties.Resources.help_white;
        }

        private void helpButton_MouseLeave(object sender, EventArgs e)
        {
            this.helpButton.BackColor = Color.Transparent;
            if (Style.iconStyle == Style.IconStyle.LIGHT)
            {
                this.helpButton.Image = allinthebox.Properties.Resources.help_black;
            }
            else
            {
                this.helpButton.Image = allinthebox.Properties.Resources.help_white;
            }
        }

        private void label6_MouseEnter(object sender, EventArgs e)
        {
            ColorConverter cc = new ColorConverter();
            this.label6.ForeColor = (Color)cc.ConvertFromString("#0060AA");
            this.label6.Font = new Font(this.label6.Font, FontStyle.Underline);

        }

        private void label6_MouseLeave(object sender, EventArgs e)
        {
            this.label6.ForeColor = Color.Black;
            this.label6.Font = new Font(this.label6.Font, FontStyle.Regular);
        }

        private void header_Paint(object sender, PaintEventArgs e)
        {
            using (Brush b = new SolidBrush(Multiply(this.sidebar.BackColor, this.header.BackColor))) {
                Rectangle rect = new Rectangle(0, 0, this.sidebar.Width + 4, this.header.Height);
                e.Graphics.FillRectangle(b, rect);
            }
        }


        private void sidebar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(this.sidebar.BackColor), this.sidebar.Bounds);
            e.Graphics.Clear(this.sidebar.BackColor);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Image img = (Style.iconStyle == Style.IconStyle.DARK) ? whiteCircuit : allinthebox.Properties.Resources.circuitBoardBackground;
            try
            {
                float nWidth = this.sidebar.Width + 40;
                float ratio = img.Width / img.Height;
                float nHeight = nWidth * ratio;
                e.Graphics.DrawImage(img, -20, this.sidebar.Height - nHeight + 80, nWidth, nHeight);
            }
            catch (Exception exc) {
                Console.WriteLine(exc.StackTrace);
            }
        }

        private Image Invert(Image img) {
            Bitmap pic = new Bitmap(img);
            for (int y = 0; (y <= (pic.Height - 1)); y++)
            {
                for (int x = 0; (x <= (pic.Width - 1)); x++)
                {
                    Color inv = pic.GetPixel(x, y);
                    inv = Color.FromArgb(inv.A, (255 - inv.R), (255 - inv.G), (255 - inv.B));
                    pic.SetPixel(x, y, inv);
                }
            }
            return pic;
        }

        private void resort(bool keepViewport) {
            int distance = 0;

            int selectedRow = dataGrid.SelectedRows[0].Index;

            if (!keepViewport)
            {
                try
                {

                    if (DisplayedRows.Count == 0 || DisplayedRows.Contains(dataGrid.Rows[selectedRow]))
                    {
                        distance = lastSelectedRowIndex - dataGrid.FirstDisplayedCell.RowIndex;

                    }
                    else
                    {
                        distance = selectedRow < DisplayedRows.FirstOrDefault().Index ? DisplayedRows.Count - 2 : selectedRow > DisplayedRows.LastOrDefault().Index ? 1 : 0;
                    }
                }
                catch (NullReferenceException exc)
                {
                    Program.logger.error(exc.Message);
                    distance = 0;

                }

                if (distance <= selectedRow && selectedRow - distance > 0)
                {
                    try
                    {
                        dataGrid.FirstDisplayedScrollingRowIndex = selectedRow - distance;
                    }
                    catch (Exception e) {
                        Program.logger.log(e.Message, e.StackTrace);
                        dataGrid.FirstDisplayedScrollingRowIndex = 0;
                    }
                }
                else
                {
                    dataGrid.FirstDisplayedScrollingRowIndex = 0;
                }


                dataGrid.Rows[selectedRow].Selected = true;
                lastSelectedRowIndex = selectedRow;

            }
            else {

                dataGrid.FirstDisplayedScrollingRowIndex = lastFirstDisplayedRowIndex;
                dataGrid.Rows[selectedRow].Selected = true;
                lastSelectedRowIndex = selectedRow;
            }



            DisplayedRows.Clear();
            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                if (row.Displayed)
                {
                    if (!DisplayedRows.Contains(row))
                    {
                        DisplayedRows.Add(row);
                    }

                }


            }


            curNum.Text = (selectedRow + 1) + " / " + dataGrid.RowCount;

        }

        private void dataGrid_Sorted(object sender, EventArgs e)
        {
            applyTableColors();
            resort(false);

        }


        private List<DataGridViewRow> DisplayedRows = new List<DataGridViewRow>();

        private void dataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            this.customScrollbar1.Value = e.NewValue;
            DisplayedRows.Clear();
            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                if (row.Displayed)
                {
                    if (!DisplayedRows.Contains(row)) {
                        DisplayedRows.Add(row);
                    }

                }

                
            }

        }

        private void customScrollbar1_Scroll(object sender, EventArgs e)
        {
            if (dataGrid.RowCount > 0)
            {
                this.dataGrid.FirstDisplayedScrollingRowIndex = this.customScrollbar1.Value < 0 ? 0 : (this.customScrollbar1.Value > this.customScrollbar1.Maximum) ? this.customScrollbar1.Maximum : this.customScrollbar1.Value;
            }
        }

        private void dataGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGrid.Rows[e.RowIndex].Height = rowHeight;
            CalculateScrollBar();
            this.customScrollbar1.Invalidate();
        }

        private void dataGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CalculateScrollBar();
            this.customScrollbar1.Invalidate();

        }

        private void dataGrid_Paint(object sender, PaintEventArgs e)
        {
            ColorConverter c = new ColorConverter();
            using (Pen p = new Pen(new SolidBrush((Color)c.ConvertFromString("#A6A6A6")), 1)) {
                //e.Graphics.DrawLine(p, new Point(this.dataGrid.Width - SystemInformation.VerticalScrollBarWidth-1, 0), new Point(this.dataGrid.Width-SystemInformation.VerticalScrollBarWidth - 1, dataGrid.Height));
            }
        }


        private void rating_Click(object sender, MouseEventArgs e)
        {
            saveChanges();
        }

        private string ConvertRtfToText(string rtf)
        {
            using (RichTextBox rtb = new RichTextBox())
            {
                rtb.Rtf = rtf;
                return rtb.Text;
            }
        }

        private void dataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

            if (this.dataGrid.Columns["comment"].Index == e.ColumnIndex && e.RowIndex >= 0) {
                if (File.Exists(FileManager.GetDataBasePath("Comments/" + this.dataGrid.Rows[e.RowIndex].Cells[1].Value + String.Empty + ".rtf")))
                    if (ConvertRtfToText(File.ReadAllText(FileManager.GetDataBasePath("Comments/" + this.dataGrid.Rows[e.RowIndex].Cells[1].Value + String.Empty + ".rtf"))) != e.Value + "")
                    {

                        using (
                            Brush gridBrush = new SolidBrush(this.dataGrid.GridColor),
                            backColorBrush = (this.dataGrid.SelectedRows.Count > 0 ? this.dataGrid.SelectedRows[0].Index == e.RowIndex : false)
                            ? new SolidBrush(e.CellStyle.SelectionBackColor)
                            : new SolidBrush(e.CellStyle.BackColor),
                            textColorBrush = (this.dataGrid.SelectedRows.Count > 0 ? this.dataGrid.SelectedRows[0].Index == e.RowIndex : false)
                            ? new SolidBrush(e.CellStyle.SelectionForeColor)
                            : new SolidBrush(e.CellStyle.ForeColor)
                        )
                        {
                            using (Pen gridLinePen = new Pen(gridBrush))
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
                                    string str = e.Value.ToString().Trim() + "";
                                    str = e.Graphics.MeasureString(str, e.CellStyle.Font).Width < e.CellBounds.Width - 20 ? str : str.Substring(0, 20) + "...";
                                    Image img = (this.dataGrid.SelectedRows.Count > 0 ? this.dataGrid.SelectedRows[0].Index == e.RowIndex : false)
                                    ? Properties.Resources.help_white
                                    : Properties.Resources.help_black;
                                    e.Graphics.DrawImage(img, e.CellBounds.Right - 16, e.CellBounds.Top + e.CellBounds.Height / 2 - 5, 10, 10);
                                    e.Graphics.DrawString(str, e.CellStyle.Font, textColorBrush, new Point(e.CellBounds.Left, e.CellBounds.Top + e.CellBounds.Height / 2 - (int)e.Graphics.MeasureString(str, e.CellStyle.Font).Height / 2));
                                }
                            }
                        }
                        e.Handled = true;

                    }
            }

            if (this.dataGrid.Columns["rateCol"].Index == e.ColumnIndex && e.RowIndex >= 0)
            {
                Rectangle newRect = new Rectangle(e.CellBounds.X + 1,
                    e.CellBounds.Y + 1, e.CellBounds.Width - 4,
                    e.CellBounds.Height - 4);

                using (
                    Brush gridBrush = new SolidBrush(this.dataGrid.GridColor),
                    backColorBrush = (this.dataGrid.SelectedRows.Count > 0 ? this.dataGrid.SelectedRows[0].Index == e.RowIndex : false)
                    ? new SolidBrush(e.CellStyle.SelectionBackColor)
                    : new SolidBrush(e.CellStyle.BackColor)
                )
                {
                    using (Pen gridLinePen = new Pen(gridBrush))
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

                            using (Brush StarBrush = (this.dataGrid.SelectedRows.Count > 0 ? this.dataGrid.SelectedRows[0].Index == e.RowIndex : false) ? new SolidBrush(e.CellStyle.SelectionForeColor) : new SolidBrush(Color.Gold))
                            {
                                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                                int numOfStars = e.Value.ToString() == "" ? 0 : int.Parse(e.Value.ToString());
                                if (numOfStars >= 5) numOfStars = 5;
                                if (numOfStars > 0)
                                {
                                    if (numOfStars >= 1)
                                    {
                                        e.Graphics.FillPolygon(StarBrush, Calculate5StarPoints(new PointF(e.CellBounds.X + e.CellBounds.Height / 2, e.CellBounds.Y + e.CellBounds.Height / 2), 6f, 3f));
                                    }
                                    if (numOfStars >= 2) {
                                        e.Graphics.FillPolygon(StarBrush, Calculate5StarPoints(new PointF(e.CellBounds.X + e.CellBounds.Height / 2 + 12f, e.CellBounds.Y + e.CellBounds.Height / 2), 6f, 3f));
                                    }
                                    if (numOfStars >= 3) {
                                        e.Graphics.FillPolygon(StarBrush, Calculate5StarPoints(new PointF(e.CellBounds.X + e.CellBounds.Height / 2 + 24f, e.CellBounds.Y + e.CellBounds.Height / 2), 6f, 3f));
                                    }
                                    if (numOfStars >= 4) {
                                        e.Graphics.FillPolygon(StarBrush, Calculate5StarPoints(new PointF(e.CellBounds.X + e.CellBounds.Height / 2 + 36f, e.CellBounds.Y + e.CellBounds.Height / 2), 6f, 3f));
                                    }
                                    if (numOfStars == 5) {
                                        e.Graphics.FillPolygon(StarBrush, Calculate5StarPoints(new PointF(e.CellBounds.X + e.CellBounds.Height / 2 + 48f, e.CellBounds.Y + e.CellBounds.Height / 2), 6f, 3f));
                                    }
                                }

                                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                            }

                        }
                        e.Handled = true;
                    }
                }
            }
        }

        private PointF[] Calculate5StarPoints(PointF Orig, float outerradius, float innerradius)
        {

            double Ang36 = Math.PI / 5.0;
            double Ang72 = 2.0 * Ang36;

            float Sin36 = (float)Math.Sin(Ang36);
            float Sin72 = (float)Math.Sin(Ang72);
            float Cos36 = (float)Math.Cos(Ang36);
            float Cos72 = (float)Math.Cos(Ang72);

            PointF[] pnts = { Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig };
            pnts[0].Y -= outerradius;
            pnts[1].X += innerradius * Sin36; pnts[1].Y -= innerradius * Cos36;
            pnts[2].X += outerradius * Sin72; pnts[2].Y -= outerradius * Cos72;
            pnts[3].X += innerradius * Sin72; pnts[3].Y += innerradius * Cos72;
            pnts[4].X += outerradius * Sin36; pnts[4].Y += outerradius * Cos36;

            pnts[5].Y += innerradius;

            pnts[6].X += pnts[6].X - pnts[4].X; pnts[6].Y = pnts[4].Y;
            pnts[7].X += pnts[7].X - pnts[3].X; pnts[7].Y = pnts[3].Y;
            pnts[8].X += pnts[8].X - pnts[2].X; pnts[8].Y = pnts[2].Y;
            pnts[9].X += pnts[9].X - pnts[1].X; pnts[9].Y = pnts[1].Y;
            return pnts;
        }

        private Font buttonFontPrototype;
        private Color buttonColorPrototype;

        private void button_refresh_MouseEnter(object sender, EventArgs e)
        {
            buttonFontPrototype = this.button_refresh.Font;
            this.button_refresh.Font = new Font(buttonFontPrototype.FontFamily, buttonFontPrototype.Size - 1f, buttonFontPrototype.Style);
        }

        private void button_refresh_MosueLeave(object sender, EventArgs e)
        {
            this.button_refresh.Font = this.buttonFontPrototype;
        }

        private void button_refresh_MouseDown(object sender, MouseEventArgs e)
        {
            this.buttonColorPrototype = this.button_refresh.ForeColor;
            this.button_refresh.ForeColor = Color.FromArgb(0, 62, 170);
        }

        private void button_refresh_MouseUp(object sender, MouseEventArgs e)
        {
            this.button_refresh.ForeColor = this.buttonColorPrototype;
        }

        private void add_MouseEnter(object sender, EventArgs e)
        {
            buttonFontPrototype = this.add.Font;
            this.add.Font = new Font(buttonFontPrototype.FontFamily, buttonFontPrototype.Size - 1f, buttonFontPrototype.Style);
        }

        private void add_MouseLeave(object sender, EventArgs e)
        {
            this.add.Font = this.buttonFontPrototype;
        }

        private void add_MouseDown(object sender, MouseEventArgs e)
        {
            this.buttonColorPrototype = this.add.ForeColor;
            this.add.ForeColor = Color.FromArgb(0, 62, 170);
        }

        private void add_MouseUp(object sender, MouseEventArgs e)
        {
            this.add.ForeColor = this.buttonColorPrototype;
        }

        private void delete_button_MouseEnter(object sender, EventArgs e)
        {
            buttonFontPrototype = this.delete_button.Font;
            this.delete_button.Font = new Font(buttonFontPrototype.FontFamily, buttonFontPrototype.Size - 1f, buttonFontPrototype.Style);
        }

        private void delete_button_MouseLeave(object sender, EventArgs e)
        {
            this.delete_button.Font = this.buttonFontPrototype;
        }

        private void delete_button_MouseDown(object sender, MouseEventArgs e)
        {
            this.buttonColorPrototype = this.delete_button.ForeColor;
            this.delete_button.ForeColor = Color.FromArgb(0, 62, 170);
        }

        private void delete_button_MouseUp(object sender, MouseEventArgs e)
        {
            this.delete_button.ForeColor = this.buttonColorPrototype;
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            buttonFontPrototype = this.button4.Font;
            this.button4.Font = new Font(buttonFontPrototype.FontFamily, buttonFontPrototype.Size - 1f, buttonFontPrototype.Style);
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            this.button4.Font = this.buttonFontPrototype;
        }

        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            this.buttonColorPrototype = this.button4.ForeColor;
            this.button4.ForeColor = Color.FromArgb(0, 62, 170);
        }

        private void button4_MouseUp(object sender, MouseEventArgs e)
        {
            this.button4.ForeColor = this.buttonColorPrototype;
        }



        private void rOption_OnTextChanged(object sender, EventArgs e)
        {

        }

        private void BarcodeSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (barcodeSearch.Checked)
            {
                currentMatrix = SearchMatrix.BARCODE;
            }
            if (customSearchBox1.Text != "")
            {
                resetSearch(false);
                search(currentMatrix, currentMode);
            }

        }

        private void NameSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (nameSearch.Checked) {
                currentMatrix = SearchMatrix.NAME;
            }
            if (customSearchBox1.Text != "") {
                resetSearch(false);
                search(currentMatrix, currentMode);
            }
        }

        private void CommentSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (commentSearch.Checked) {
                currentMatrix = SearchMatrix.REMARK;
            }
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
                this.filterIcon.Image = Properties.Resources.filter_stroke;
                currentMode = SearchMode.MARKUP;
            }
            else {
                this.filterIcon.Image = Properties.Resources.filter;
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
            if (!refreshing) {
                saveChanges();
            }
        }

        private void Remote_Click(object sender, EventArgs e)
        {
            rv.Show();
        }


        new Rectangle Bottom { get { return new Rectangle(0, this.ClientSize.Height - _, this.ClientSize.Width, _); } }
        new Rectangle Right { get { return new Rectangle(this.ClientSize.Width - _, 0, _, this.ClientSize.Height); } }

        Rectangle TopLeft { get { return new Rectangle(0, 0, _, _); } }
        Rectangle TopRight { get { return new Rectangle(this.ClientSize.Width - _, 0, _, _); } }
        Rectangle BottomLeft { get { return new Rectangle(0, this.ClientSize.Height - _, _, _); } }
        Rectangle BottomRight { get { return new Rectangle(this.ClientSize.Width - _, this.ClientSize.Height - _, _, _); } }


        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == 0x84 && !maximized)
            {
                var cursor = this.PointToClient(Cursor.Position);

                if (TopLeft.Contains(cursor)) message.Result = (IntPtr)HTTOPLEFT;
                else if (TopRight.Contains(cursor)) message.Result = (IntPtr)HTTOPRIGHT;
                else if (BottomLeft.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (BottomRight.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (Top.Contains(cursor)) message.Result = (IntPtr)HTTOP;
                else if (Left.Contains(cursor)) message.Result = (IntPtr)HTLEFT;
                else if (Right.Contains(cursor)) message.Result = (IntPtr)HTRIGHT;
                else if (Bottom.Contains(cursor)) message.Result = (IntPtr)HTBOTTOM;
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

        int mouseX = 0, mouseY = 0;
        bool mousedown, maximized;
        int grabX = 0, grabY = 0;
        Rectangle lastDim;

        public Color Multiply(Color c1, Color c2) {
            byte ar = c1.R;
            byte br = c2.R;
            byte cr = (byte)((ar * br + 0xFF) >> 8);

            byte ag = c1.G;
            byte bg = c2.G;
            byte cg = (byte)((ag * bg + 0xFF) >> 8);

            byte ab = c1.B;
            byte bb = c2.B;
            byte cb = (byte)((ab * bb + 0xFF) >> 8);

            return Color.FromArgb(255, cr, cg, cb);
        }

        public Image getImage(String str) {
            foreach (ItemImage img in images) {

                if (img.name.Equals(str)) {
                    return img.image;
                }
            }

            try
            {
                return Image.FromFile(str);
            }
            catch (Exception exc) {
                Program.logger.error(exc.Message, exc.StackTrace);
                return null;
            }
        }

    }

    public class ItemImage {

        public Image image;
        public String name;

        public ItemImage(Image _image, String _name) {
            this.image = _image;
            this.name = _name;
        }
    }

    public class KeyCombination {

        public Boolean controlNeeded = false;
        public Boolean altNeeded = false;
        public Boolean shiftNeeded = false;
        public Keys key;

        private KeysConverter kc = new KeysConverter();

        public KeyCombination(string comb) {
            if (comb.Contains("Shift")) {
                this.shiftNeeded = true;
            }
            if (comb.Contains("Ctrl")) {
                this.controlNeeded = true;
            }
            if (comb.Contains("Alt")) {
                this.altNeeded = true;
            }
            if (comb.Contains("+"))
            {
                string[] pieces = comb.Split('+');
                string nameRaw = pieces[pieces.Length - 1];
                this.key = (Keys)kc.ConvertFromString(nameRaw.Trim());
            }
            else {
                this.key = (Keys)kc.ConvertFromString(comb.Trim());
            }
        }

        public override string ToString() {
            return controlNeeded + "; " + altNeeded + "; " + shiftNeeded + "; " + key.ToString();
        }



    }


    public static class XmlHandler{
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
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }

    public class Rating : Bunifu.Framework.UI.BunifuRating {
        private Rectangle b;

        public Rating() {
            b = this.Bounds;
            foreach (Control c in this.Controls)
            {
                c.MouseClick += C_MouseClick;
            }
        }

        private void C_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(e);
        }
    }

    public class DataGridViewC : DataGridView {

        public DataGridViewC(){

        }

        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
    }
}
