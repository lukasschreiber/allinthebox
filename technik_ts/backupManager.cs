using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace allinthebox
{
    public partial class backupManager : Form
    {
        Main main;
        int iconStyle = 1;
        public string currentStyle;

        public backupManager(Main m)
        {
            main = m;
            InitializeComponent();

            //language
            this.label2.Text = Properties.strings.backup;
            this.bunifuFlatButton1.Text = Properties.strings.backupManually;
            this.label1.Text = Properties.strings.autoBackup;
            this.lastUpdate.Text = Properties.strings.lastBackup;
            this.nextBackupLabel.Text = Properties.strings.nextBackup;
            this.bunifuFlatButton2.Text = Properties.strings.restore;


            XmlDocument set = new XmlDocument();
            set.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            options.Items.Add(Properties.strings.never);
            options.Items.Add(Properties.strings.daily);
            options.Items.Add(Properties.strings.weekly);
            options.Items.Add(Properties.strings.monthly);
            options.Items.Add(Properties.strings.annually);
            options.Text = set.SelectSingleNode("/settings/backupSchedule").InnerXml;
            refreshUI();

            //load style
            currentStyle = main.loadSettingsDataBase().SelectSingleNode("/settings/style").InnerXml;

            ColorConverter cc = new ColorConverter();

          
        }

        public void refreshUI() {
            XmlDocument set = new XmlDocument();
            set.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            if (set.SelectSingleNode("/settings/backupSchedule").InnerXml != "")
            {
                string[] formattedDateArr = set.SelectSingleNode("/settings/lastUpdate").InnerXml.Split('/');
                string formattedDate = formattedDateArr[1] + "." + formattedDateArr[0] + "." + formattedDateArr[2];
                lastUpdate.Text = Properties.strings.lastBackup + ": " + formattedDate + " " + Properties.strings.timeFormat;
            }
            else
            {
                lastUpdate.Text = Properties.strings.lastBackup + ": " + Properties.strings.never;
            }

            var backupSchedule = set.SelectSingleNode("/settings/backupSchedule").InnerXml;
            long nextBackup = 0;
            var dt = DateTime.ParseExact(set.SelectSingleNode("/settings/lastUpdate").InnerXml, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            long lastBackup = dt.ToFileTimeUtc();


            if (backupSchedule == Properties.strings.annually)
            {
                nextBackup = lastBackup + 31556952000 * (Int64)10000;
                nextBackupLabel.Text = Properties.strings.nextBackup + ": " + DateTime.FromFileTimeUtc(nextBackup).ToString("dd.MM.yyyy");
            }
            else if (backupSchedule == Properties.strings.monthly)
            {
                nextBackup = lastBackup + 2592000000 * (Int64)10000;
                nextBackupLabel.Text = Properties.strings.nextBackup + ": " + DateTime.FromFileTimeUtc(nextBackup).ToString("dd.MM.yyyy");
            }
            else if (backupSchedule == Properties.strings.weekly)
            {
                nextBackup = lastBackup + 604800000 * (Int64)10000;
                nextBackupLabel.Text = Properties.strings.nextBackup + ": " + DateTime.FromFileTimeUtc(nextBackup).ToString("dd.MM.yyyy");
            }
            else if (backupSchedule == Properties.strings.daily)
            {
                nextBackupLabel.Text = Properties.strings.nextBackup + ": " + Properties.strings.atNextStart;
            }
            else if (backupSchedule == Properties.strings.never) {
                nextBackupLabel.Text = Properties.strings.nextBackup + ": " + Properties.strings.never;
            }
        }

        private void backupManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
            Main.currentOpenWindows.Remove(this.Name);
        }

        private async void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            await main.backupData();
        }

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = FileManager.GetDataBasePath();
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);

                    foreach (var file in files)
                    {
                        System.IO.File.Copy(file, FileManager.GetDataBasePath() + "\\" + file.Split('\\').Last(), true);
                        Program.logger.log("BackupManager", "copy from " + file + " to " + FileManager.GetDataBasePath() + file.Split('\\').Last(), Color.Blue);
                    }
                }
            }
            //main.dataGrid.Rows.Clear();
            main.loadImagesToBuffer();
            main.changed = true;
            main.refresh(false);
            MessageBox.Show(Properties.strings.restoreOk);
        }

        private void options_OnSelectedIndexChanged(object sender, EventArgs e)
        {

            XmlDocument set = new XmlDocument();
            set.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            set.SelectSingleNode("/settings/backupSchedule").InnerXml = options.Text;
            set.Save(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));

            refreshUI();
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

        private void BackupManager_Load(object sender, EventArgs e)
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
}