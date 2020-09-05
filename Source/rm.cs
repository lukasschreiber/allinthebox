using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using allinthebox.Properties;
using Transitions;

namespace allinthebox
{
    public partial class rm : Form
    {
        public string curPreset;
        private readonly List<Label> labels = new List<Label>();
        public Main main;


        private Point MouseDownLocation;
        private int num;

        private Label selected;

        public rm(Main m)
        {
            InitializeComponent();

            //language
            Text = strings.regalmanager;
            add.Text = strings.add;
            delete.Text = strings.delete;
            refresh.Text = strings.refresh;
            label1.Text = strings.regalManagerDescribe;
            preset.Text = strings.preset;
            load_preset.Text = strings.loadPreset;


            main = m;
            loadData();
            if (main.rmOnTop)
                TopMost = true;
            else
                TopMost = false;
            MaximizeBox = false;
        }

        public void loadData()
        {
            labels.Clear();
            var session = new XmlDocument();
            session.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));
            curPreset = session.SelectSingleNode("/list/session/preset").InnerText;

            bg.BackgroundImage = Image.FromFile(FileManager.GetDataBasePath("Presets\\" + curPreset));

            var doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));

            foreach (var dm in doc.Descendants("regal"))
            {
                var item = dm.Element("name").Value;

                create(item);


                Thread.Sleep(1);
            }
        }

        private void add_Click(object sender, EventArgs e)
        {
            if (nameText.Text != "")
            {
                var spath = FileManager.GetDataBasePath(FileManager.NAMES.RACKS);
                var doc = XDocument.Load(spath);
                var root = new XElement("regal");
                root.Add(new XElement("name", nameText.Text));
                root.Add(new XElement("x", ""));
                root.Add(new XElement("y", ""));
                doc.Element("list").Add(root);

                clear();

                doc.Save(spath);


                Thread.Sleep(100);
                loadData();
            }
        }

        private void delete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private async void Delete()
        {
            if (selected != null)
                if (DialogResult.Yes == MessageBox.Show(strings.regalDelete, strings.submit, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning))
                {
                    var temp_name = selected.Text;

                    var doc = new XmlDocument();
                    doc.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));
                    var XPath = "/list/regal[name='" + temp_name + "']";
                    var node = doc.SelectSingleNode(XPath);
                    node.ParentNode.RemoveChild(node);
                    doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));


                    selected.Dispose();
                    selected = null;

                    clear();

                    loadData();

                    main.changed = true;
                    var dataBase = main.loadDataBase();
                    foreach (XmlNode n in dataBase.SelectNodes("/data/item"))
                        if (n.SelectSingleNode("regal").InnerXml == temp_name)
                            n.SelectSingleNode("regal").InnerXml = "";
                    await SaveAsync(dataBase, FileManager.NAMES.DATA);
                }
        }


        private void refresh_Click(object sender, EventArgs e)
        {
            clear();
            loadData();
        }

        private void clear()
        {
            var doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));

            foreach (var dm in doc.Descendants("regal"))
            {
                var name = dm.Element("name").Value;

                var lb = Controls.Find("label_" + name, true).FirstOrDefault() as Label;

                Controls.Remove(lb);
                lb.Dispose();
            }

            num = 0;
            number.Text = "0 / 32";
        }

        private void create(string name)
        {
            var lbl = new Label();

            var rnd = new Random();

            var doc = new XmlDocument();
            doc.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));

            var x_xml = "/list/regal[name='" + name + "']/x";
            var x_node = doc.SelectSingleNode(x_xml);
            var pre_x = x_node.InnerXml;

            var y_xml = "/list/regal[name='" + name + "']/y";
            var y_node = doc.SelectSingleNode(y_xml);
            var pre_y = y_node.InnerXml;

            if (pre_y != "" && pre_x != "")
            {
                lbl.Location = new Point(int.Parse(pre_x), int.Parse(pre_y));
            }
            else
            {
                var x = rnd.Next(10, bg.Width - 10);
                var y = rnd.Next(10, bg.Height - 10);
                lbl.Location = new Point(x, y);
            }

            lbl.Name = "label_" + name;
            lbl.Text = name;

            lbl.AutoSize = true;
            lbl.Size = new Size(35, 13);
            lbl.TabIndex = 0;
            lbl.MouseDown += label_MouseDown;
            lbl.MouseMove += label_MouseMove;


            bg.Controls.Add(lbl);
            num++;
            number.Text = num + " / 32";

            if (num >= 32)
                add.Enabled = false;
            else
                add.Enabled = true;
            labels.Add(lbl);
        }

        private void label_MouseDown(object sender, MouseEventArgs e)
        {
            var L = (Label) sender;
            if (e.Button == MouseButtons.Left) MouseDownLocation = e.Location;
            var document = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));

            foreach (var dm in document.Descendants("regal"))
            {
                var name = dm.Element("name").Value;

                var lb = Controls.Find("label_" + name, true).FirstOrDefault() as Label;

                lb.ForeColor = Color.Black;
                lb.BackColor = Color.White;
            }

            selected = L;
            selected.ForeColor = Color.White;
            selected.BackColor = Color.Blue;
        }

        private void label_MouseMove(object sender, MouseEventArgs e)
        {
            var L = (Label) sender;
            var PR = L.Parent.ClientRectangle;
            if (e.Button == MouseButtons.Left)
            {
                var t = new Transition(new TransitionType_Linear(3));
                t.add(L, "Left", Math.Min(Math.Max(0, e.X + L.Left - 10), PR.Right - L.Width));
                t.add(L, "Top", Math.Min(Math.Max(0, e.Y + L.Top - 10), PR.Bottom - L.Height));
                t.run();
            }
        }

        private void bg_Click(object sender, EventArgs e)
        {
            var document = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));

            foreach (var dm in document.Descendants("regal"))
            {
                var name = dm.Element("name").Value;

                var lb = Controls.Find("label_" + name, true).FirstOrDefault() as Label;

                lb.ForeColor = Color.Black;
                lb.BackColor = Color.White;

                // System.Threading.Thread.Sleep(50);
            }

            selected = null;
        }

        private void preset_Click(object sender, EventArgs e)
        {
            var SFD = new SaveFileDialog();
            SFD.Title = strings.savePreset;
            SFD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            SFD.Filter = strings.presetType + " (*.png)|*.png";
            SFD.ShowDialog();

            if (!File.Exists(SFD.FileName))
            {
                var bmp = new Bitmap(bg.Width, bg.Height);
                var g = Graphics.FromImage(bmp);

                g.Clear(Color.White);

                g.Flush();
                bmp.Save(SFD.FileName, ImageFormat.Png);

                MessageBox.Show(strings.fileCreated);
            }
            else
            {
                MessageBox.Show(strings.fileAlreadyExists);
            }
        }

        private void load_preset_Click(object sender, EventArgs e)
        {
            var OFD = new OpenFileDialog();

            OFD.Title = strings.choosePreset;
            OFD.Multiselect = false;
            OFD.ReadOnlyChecked = true;
            OFD.InitialDirectory = FileManager.GetDataBasePath("Presets\\");
            OFD.Filter = strings.presetType + " (*.png)|*.png|" + strings.allFilesType + " (*.*)|*.*";
            OFD.FilterIndex = 1;
            OFD.RestoreDirectory = true;

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                var fullPath = OFD.FileName;

                var fileName = Path.GetFileName(fullPath);

                var filePath = Path.GetDirectoryName(fullPath);

                var numOfOldPresets = Directory
                    .GetFiles(FileManager.GetDataBasePath("Presets"), "*", SearchOption.AllDirectories).Length;

                var newName = fileName;

                if (filePath != FileManager.GetDataBasePath("Presets"))
                {
                    File.Copy(fullPath, FileManager.GetDataBasePath("Presets\\" + newName));

                    var document = new XmlDocument();
                    document.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));

                    var XPathName = "/list/session/preset";
                    var node_name = document.SelectSingleNode(XPathName);
                    node_name.InnerXml = newName;
                    document.Save(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));

                    bg.BackgroundImage = Image.FromFile(FileManager.GetDataBasePath("Presets\\" + newName));
                }
                else
                {
                    var document = new XmlDocument();
                    document.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));

                    var XPathName = "/list/session/preset";
                    var node_name = document.SelectSingleNode(XPathName);
                    node_name.InnerXml = fileName;
                    document.Save(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));
                    bg.BackgroundImage = Image.FromFile(FileManager.GetDataBasePath("Presets\\" + fileName));
                }
            }
        }

        public async Task SaveAsync(XmlDocument xml, string filename)
        {
            await Task.Factory.StartNew(delegate
            {
                using (var fs = new FileStream(FileManager.GetDataBasePath("//" + filename), FileMode.Create))
                {
                    xml.Save(fs);
                }
            });
        }

        private void rm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.currentOpenWindows.Remove(Name);

            var doc = new XmlDocument();
            doc.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));

            foreach (var L in labels)
            {
                var x_xml = "/list/regal[name='" + L.Text + "']/x";
                var x_node = doc.SelectSingleNode(x_xml);
                x_node.InnerXml = L.Location.X.ToString();

                var y_xml = "/list/regal[name='" + L.Text + "']/y";
                var y_node = doc.SelectSingleNode(y_xml);
                y_node.InnerXml = L.Location.Y.ToString();
            }

            doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));


            main.rmChanged = true;
            main.refresh(false);
            if (main.dataGrid.RowCount > 0) main.loadDataToView(main.dataGrid.SelectedRows[0].Index);
        }

        private void rm_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(Name);
        }
    }
}