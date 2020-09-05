using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using allinthebox.Properties;

namespace allinthebox
{
    public partial class backupManager : Form
    {
        public string currentStyle;
        private int grabX, grabY;
        private readonly int iconStyle = 1;
        private readonly Main main;
        private bool mousedown, maximized;

        private int mouseX, mouseY;

        public backupManager(Main m)
        {
            main = m;
            InitializeComponent();

            //language
            label2.Text = strings.backup;
            bunifuFlatButton1.Text = strings.backupManually;
            label1.Text = strings.autoBackup;
            lastUpdate.Text = strings.lastBackup;
            nextBackupLabel.Text = strings.nextBackup;
            bunifuFlatButton2.Text = strings.restore;


            var set = new XmlDocument();
            set.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            options.Items.Add(strings.never);
            options.Items.Add(strings.daily);
            options.Items.Add(strings.weekly);
            options.Items.Add(strings.monthly);
            options.Items.Add(strings.annually);
            options.Text = set.SelectSingleNode("/settings/backupSchedule").InnerXml;
            refreshUI();

            //load style
            currentStyle = main.loadSettingsDataBase().SelectSingleNode("/settings/style").InnerXml;

            var cc = new ColorConverter();
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

        public void refreshUI()
        {
            var set = new XmlDocument();
            set.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            if (set.SelectSingleNode("/settings/backupSchedule").InnerXml != "")
            {
                var formattedDateArr = set.SelectSingleNode("/settings/lastUpdate").InnerXml.Split('/');
                var formattedDate = formattedDateArr[1] + "." + formattedDateArr[0] + "." + formattedDateArr[2];
                lastUpdate.Text = strings.lastBackup + ": " + formattedDate + " " + strings.timeFormat;
            }
            else
            {
                lastUpdate.Text = strings.lastBackup + ": " + strings.never;
            }

            var backupSchedule = set.SelectSingleNode("/settings/backupSchedule").InnerXml;
            long nextBackup = 0;
            var dt = DateTime.ParseExact(set.SelectSingleNode("/settings/lastUpdate").InnerXml, "MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture);
            var lastBackup = dt.ToFileTimeUtc();


            if (backupSchedule == strings.annually)
            {
                nextBackup = lastBackup + 31556952000 * 10000;
                nextBackupLabel.Text = strings.nextBackup + ": " +
                                       DateTime.FromFileTimeUtc(nextBackup).ToString("dd.MM.yyyy");
            }
            else if (backupSchedule == strings.monthly)
            {
                nextBackup = lastBackup + 2592000000 * (long) 10000;
                nextBackupLabel.Text = strings.nextBackup + ": " +
                                       DateTime.FromFileTimeUtc(nextBackup).ToString("dd.MM.yyyy");
            }
            else if (backupSchedule == strings.weekly)
            {
                nextBackup = lastBackup + 604800000 * (long) 10000;
                nextBackupLabel.Text = strings.nextBackup + ": " +
                                       DateTime.FromFileTimeUtc(nextBackup).ToString("dd.MM.yyyy");
            }
            else if (backupSchedule == strings.daily)
            {
                nextBackupLabel.Text = strings.nextBackup + ": " + strings.atNextStart;
            }
            else if (backupSchedule == strings.never)
            {
                nextBackupLabel.Text = strings.nextBackup + ": " + strings.never;
            }
        }

        private void backupManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
            Main.currentOpenWindows.Remove(Name);
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
                var result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    var files = Directory.GetFiles(fbd.SelectedPath);

                    foreach (var file in files)
                    {
                        File.Copy(file, FileManager.GetDataBasePath() + "\\" + file.Split('\\').Last(), true);
                        Program.logger.log("BackupManager",
                            "copy from " + file + " to " + FileManager.GetDataBasePath() + file.Split('\\').Last(),
                            Color.Blue);
                    }
                }
            }

            //main.dataGrid.Rows.Clear();
            main.loadImagesToBuffer();
            main.changed = true;
            main.refresh(false);
            MessageBox.Show(strings.restoreOk);
        }

        private void options_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var set = new XmlDocument();
            set.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            set.SelectSingleNode("/settings/backupSchedule").InnerXml = options.Text;
            set.Save(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));

            refreshUI();
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

        private void BackupManager_Load(object sender, EventArgs e)
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
}