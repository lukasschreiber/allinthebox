using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace allinthebox
{
    public partial class rm : Form
    {
        public String curPreset;
        List<Label> labels = new List<Label>();
        public Main main;
        public rm(Main m)
        {
            InitializeComponent();

            //language
            this.Text = Properties.strings.regalmanager;
            this.add.Text = Properties.strings.add;
            this.delete.Text = Properties.strings.delete;
            this.refresh.Text = Properties.strings.refresh;
            this.label1.Text = Properties.strings.regalManagerDescribe;
            this.preset.Text = Properties.strings.preset;
            this.load_preset.Text = Properties.strings.loadPreset;


            this.main = m;
            loadData();
            if (main.rmOnTop)
            {
                this.TopMost = true;
            }
            else {
                this.TopMost = false;
            }
            this.MaximizeBox = false;
        }

        private Label selected = null;
        private int num = 0;

        public void loadData()
        {
            labels.Clear();
            XmlDocument session = new XmlDocument();
            session.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));
            curPreset = session.SelectSingleNode("/list/session/preset").InnerText;

            bg.BackgroundImage = Image.FromFile(FileManager.GetDataBasePath("Presets\\"+curPreset));

            XDocument doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));

            foreach (var dm in doc.Descendants("regal"))
            {
                String item = dm.Element("name").Value;

                create(item);


                System.Threading.Thread.Sleep(1);

  
            }
        }

        private void add_Click(object sender, EventArgs e)
        {
            if (nameText.Text != "") {
                String spath = FileManager.GetDataBasePath(FileManager.NAMES.RACKS);
                XDocument doc = XDocument.Load(spath);
                XElement root = new XElement("regal");
                root.Add(new XElement("name", nameText.Text));
                root.Add(new XElement("x", ""));
                root.Add(new XElement("y", ""));
                doc.Element("list").Add(root);

                clear();

                doc.Save(spath);

                
                System.Threading.Thread.Sleep(100);
                loadData();
            }
        }

        private void delete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private async void Delete() {
            if (selected != null)
            {
                if (DialogResult.Yes == MessageBox.Show(Properties.strings.regalDelete, Properties.strings.submit, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {

                    String temp_name = selected.Text;

                    XmlDocument doc = new XmlDocument();
                    doc.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));
                    String XPath = "/list/regal[name='" + temp_name + "']";
                    XmlNode node = doc.SelectSingleNode(XPath);
                    node.ParentNode.RemoveChild(node);
                    doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));


                    selected.Dispose();
                    selected = null;

                    clear();

                    loadData();

                    main.changed = true;
                    XmlDocument dataBase = main.loadDataBase();
                    foreach (XmlNode n in dataBase.SelectNodes("/data/item"))
                    {
                        if (n.SelectSingleNode("regal").InnerXml == temp_name)
                        {
                            n.SelectSingleNode("regal").InnerXml = "";
                        }
                    }
                    await SaveAsync(dataBase, FileManager.NAMES.DATA);
                }
            }
        }


        private void refresh_Click(object sender, EventArgs e)
        {
            clear();
            loadData();
        }

        private void clear()
        {

            XDocument doc = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));

            foreach (var dm in doc.Descendants("regal"))
            {
                String name = dm.Element("name").Value;

                Label lb = Controls.Find("label_"+name, true).FirstOrDefault() as Label;

                Controls.Remove(lb);
                lb.Dispose();

            }

            num = 0;
            number.Text = "0 / 32";

        }

        private void create(string name)
        {
            Label lbl = new Label();

            Random rnd = new Random();

            XmlDocument doc = new XmlDocument();
            doc.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));

            String x_xml = "/list/regal[name='" + name + "']/x";
            XmlNode x_node = doc.SelectSingleNode(x_xml);
            string pre_x = x_node.InnerXml;

            String y_xml = "/list/regal[name='" + name + "']/y";
            XmlNode y_node = doc.SelectSingleNode(y_xml);
            string pre_y = y_node.InnerXml;

            if (pre_y != "" && pre_x != "")
            {
                lbl.Location = new System.Drawing.Point(Int32.Parse(pre_x), Int32.Parse(pre_y));
            }
            else
            {
                int x = rnd.Next(10, bg.Width - 10);
                int y = rnd.Next(10, bg.Height - 10);
                lbl.Location = new System.Drawing.Point(x, y);
            }   
            lbl.Name = "label_" + name;
            lbl.Text = name;

            lbl.AutoSize = true;
            lbl.Size = new System.Drawing.Size(35, 13);
            lbl.TabIndex = 0;
            lbl.MouseDown += new System.Windows.Forms.MouseEventHandler(label_MouseDown);
            lbl.MouseMove += new System.Windows.Forms.MouseEventHandler(label_MouseMove);


            this.bg.Controls.Add(lbl);
            num++;
            number.Text = num + " / 32";

            if (num >= 32)
            {
                add.Enabled = false;
            }
            else {
                add.Enabled = true;
            }
            labels.Add(lbl);
        }


        private Point MouseDownLocation;

        private void label_MouseDown(object sender, MouseEventArgs e)
        {
            Label L = (Label)sender;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
            }
            XDocument document = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));

            foreach (var dm in document.Descendants("regal"))
            {
                String name = dm.Element("name").Value;

                Label lb = Controls.Find("label_" + name, true).FirstOrDefault() as Label;

                lb.ForeColor = Color.Black;
                lb.BackColor = Color.White;

            }
            selected = L;
            selected.ForeColor = Color.White;
            selected.BackColor = Color.Blue;
        }

        private void label_MouseMove(object sender, MouseEventArgs e)
        {
            Label L = (Label)sender;
            Rectangle PR = L.Parent.ClientRectangle;
            if (e.Button == MouseButtons.Left)
            {
                Transitions.Transition t = new Transitions.Transition(new Transitions.TransitionType_Linear(3));
                t.add(L, "Left", Math.Min(Math.Max(0, e.X + L.Left - 10), PR.Right - L.Width));
                t.add(L, "Top", Math.Min(Math.Max(0, e.Y + L.Top - 10), PR.Bottom - L.Height));
                t.run();
            }
        }

        private void bg_Click(object sender, EventArgs e)
        {
            XDocument document = XDocument.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));

            foreach (var dm in document.Descendants("regal"))
            {
                String name = dm.Element("name").Value;

                Label lb = Controls.Find("label_" + name, true).FirstOrDefault() as Label;

                lb.ForeColor = Color.Black;
                lb.BackColor = Color.White;

               // System.Threading.Thread.Sleep(50);


            }

            selected = null;
        }

        private void preset_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Title = Properties.strings.savePreset;
            SFD.InitialDirectory = @Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            SFD.Filter = Properties.strings.presetType + " (*.png)|*.png";
            SFD.ShowDialog();

            if (!File.Exists(SFD.FileName))
            {

                Bitmap bmp = new Bitmap(bg.Width, bg.Height);
                Graphics g = Graphics.FromImage(bmp);

                g.Clear(Color.White);

                g.Flush();
                bmp.Save(SFD.FileName, System.Drawing.Imaging.ImageFormat.Png);

                MessageBox.Show(Properties.strings.fileCreated);

            }
            else {
                MessageBox.Show(Properties.strings.fileAlreadyExists);
            }

        }

        private void load_preset_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();

            OFD.Title = Properties.strings.choosePreset;
            OFD.Multiselect = false;
            OFD.ReadOnlyChecked = true;
            OFD.InitialDirectory = FileManager.GetDataBasePath("Presets\\");
            OFD.Filter = Properties.strings.presetType+" (*.png)|*.png|" + Properties.strings.allFilesType + " (*.*)|*.*";
            OFD.FilterIndex = 1;
            OFD.RestoreDirectory = true;

            if (OFD.ShowDialog() == DialogResult.OK)
            {
                String fullPath = OFD.FileName;

                String fileName = System.IO.Path.GetFileName(fullPath);

                String filePath = System.IO.Path.GetDirectoryName(fullPath);

                int numOfOldPresets = Directory.GetFiles(FileManager.GetDataBasePath("Presets"), "*", SearchOption.AllDirectories).Length;

                String newName = fileName;

                if (filePath != FileManager.GetDataBasePath("Presets"))
                {
                    System.IO.File.Copy(fullPath, FileManager.GetDataBasePath("Presets\\" + newName));

                    XmlDocument document = new XmlDocument();
                    document.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));

                    String XPathName = "/list/session/preset";
                    XmlNode node_name = document.SelectSingleNode(XPathName);
                    node_name.InnerXml = newName;
                    document.Save(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));

                    bg.BackgroundImage = Image.FromFile(FileManager.GetDataBasePath("Presets\\" + newName));
                }
                else {
                    XmlDocument document = new XmlDocument();
                    document.Load(FileManager.GetDataBasePath(FileManager.NAMES.SESSION));

                    String XPathName = "/list/session/preset";
                    XmlNode node_name = document.SelectSingleNode(XPathName);
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
                using (var fs = new FileStream(FileManager.GetDataBasePath("//"+filename), FileMode.Create))
                {
                    xml.Save(fs);
                }
            });
        }

        private void rm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.currentOpenWindows.Remove(this.Name);

            XmlDocument doc = new XmlDocument();
            doc.Load(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));

            foreach (Label L in labels)
            {
                String x_xml = "/list/regal[name='" + L.Text + "']/x";
                XmlNode x_node = doc.SelectSingleNode(x_xml);
                x_node.InnerXml = L.Location.X.ToString();

                String y_xml = "/list/regal[name='" + L.Text + "']/y";
                XmlNode y_node = doc.SelectSingleNode(y_xml);
                y_node.InnerXml = L.Location.Y.ToString();
            }
            doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.RACKS));


            main.rmChanged = true;
            main.refresh(false);
            if (main.dataGrid.RowCount > 0)
            {
                main.loadDataToView(main.dataGrid.SelectedRows[0].Index);
            }
        }

        private void rm_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(this.Name);
        }
    }
}
