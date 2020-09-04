using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace allinthebox
{
    public partial class settings : Form
    {

        public Main main;

        public string[] keys = { "F1", "F2", "F3", "F4", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12" };
        private Boolean changesMade;

        public settings(Boolean admin, Main m)
        {
            InitializeComponent();


            //language

        

            this.groupBox2.Text = Properties.strings.tableView;
            this.nameCheck.Text = Properties.strings.name;
            this.barCheck.Text = Properties.strings.barcode;
            this.statCheck.Text = Properties.strings.state;
            this.ComCheck.Text = Properties.strings.comment;
            this.regalCheckBox.Text = Properties.strings.regal;
            this.showRate.Text = Properties.strings.rate;
            this.groupBox3.Text = Properties.strings.tableDesign;
            this.label2.Text = Properties.strings.primeColor;
            this.label3.Text = Properties.strings.secondColor;
            this.label6.Text = Properties.strings.selectColor;
            this.label5.Text = Properties.strings.fontSize;
            this.borrowItalics.Text = Properties.strings.borrowedItalic;
            this.borrowColorCheck.Text = Properties.strings.borrowedMarked;
            this.label7.Text = Properties.strings.borrowMarkedSelectedColor;
            this.label8.Text = Properties.strings.fontColor;
            this.groupBox4.Text = Properties.strings.scanner;
            this.label12.Text = Properties.strings.minLength;
            this.label13.Text = Properties.strings.suffixSendMin;
            this.label14.Text = Properties.strings.suffix;
            this.label15.Text = Properties.strings.noteScanner;
            this.groupBox1.Text = Properties.strings.searchSettings;
            this.label1.Text = Properties.strings.searchStandard;
            this.nameRadio.Text = Properties.strings.searchNames;
            this.barcodeRadio.Text = Properties.strings.searchBarcode;
            this.commentRadio.Text = Properties.strings.searchComments;
            this.exactSearchCheck.Text = Properties.strings.exactBarcode;
            this.label9.Text = Properties.strings.nameColor;
            this.label10.Text = Properties.strings.barcodeColor;
            this.label11.Text = Properties.strings.commentColor;
            this.searchWhileType.Text = Properties.strings.searchWhileTyping;
            this.scrollToSearch.Text = Properties.strings.scrollToFirst;
            this.groupBox5.Text = Properties.strings.windowSerttings;
            this.winOpenOnce.Text = Properties.strings.openOnce;
            this.settingsOntop.Text = Properties.strings.settingsOnTop;
            this.rmOnTop.Text = Properties.strings.rmOnTop;
            this.imageOnTop.Text = Properties.strings.imageOnTop;
            this.multiborrowOnTop.Text = Properties.strings.selectOnTop;
            this.hotKeyHolder.Text = Properties.strings.hotKeys;
            this.label16.Text = Properties.strings.save;
            this.label17.Text = Properties.strings.borrow;
            this.label18.Text = Properties.strings.help;
            this.label19.Text = Properties.strings.version;
            this.label21.Text = Properties.strings.refresh;
            this.label22.Text = Properties.strings.delete;
            this.resetHot.Text = Properties.strings.resetHotkeys;

            this.main = m;

            //get language and styles
            foreach (string d in Directory.GetDirectories(FileManager.GetBaseDirectoyPath()))
            {
                if (d.Split('\\').Last().Length <= 2)
                {
                    language.Items.Add(new CultureInfo(d.Split('\\').Last()).DisplayName + " ("+d.Split('\\').Last()+")");
                }

            }

            //get Style
            foreach (string f in Directory.GetFiles(FileManager.GetDataBasePath("Styles"), "*.xml")) {
                styles.Items.Add(f.Split('\\').Last().Split('.').First());
            }


            if (main.settingsOnTop)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }

            this.MaximizeBox = false;

            changesMade = false;

            if (admin)
            {
                this.Text += " | Admin";
            }
            else
            {
                this.Text += " | Nutzer";
            }

            if (!admin)
            {
                this.groupBox4.Enabled = false;
                this.hotKeyHolder.Enabled = false;
            }
            else if (admin) {
                this.groupBox4.Enabled = true;
                this.hotKeyHolder.Enabled = true;
            }

            var fontsCollection = new InstalledFontCollection();
            this.fontCombo.Items.Add("(Standardschrift)");
            foreach (var fontFamiliy in fontsCollection.Families)
            {
                this.fontCombo.Items.Add(fontFamiliy.Name);
            }

            this.suffixCombo.Items.AddRange(keys);

            XmlDocument settings = new XmlDocument();
            settings.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));

            switch (int.Parse(settings.SelectSingleNode("/settings/searchDefault").InnerXml)) {
                case 0: this.nameRadio.Select();
                    break;
                case 1: this.barcodeRadio.Select();
                    break;
                case 2: this.commentRadio.Select();
                    break;
            }

            this.exactSearchCheck.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/searchExact").InnerXml);

            this.nameCheck.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/showNameCol").InnerXml);
            this.barCheck.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/showBarCol").InnerXml);
            this.statCheck.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/showStatusCol").InnerXml);
            this.ComCheck.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/showComCol").InnerXml);
            this.regalCheckBox.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/showRegalCol").InnerXml);
            this.showRate.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/showRateCol").InnerXml);


            this.borrowItalics.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/borrowedItalic").InnerXml);

            this.borrowColorCheck.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/borrowedColorful").InnerXml);

            this.searchWhileType.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/searchWhileType").InnerXml);
            this.scrollToSearch.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/scrollToSearch").InnerXml);
            this.caseSensitive.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/searchCaseSensitive").InnerXml);

            this.winOpenOnce.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/winOpenOnce").InnerXml);

            this.settingsOntop.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/settingsOntop").InnerXml);
            this.rmOnTop.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/rmOnTop").InnerXml);
            this.imageOnTop.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/imageOnTop").InnerXml);
            this.multiborrowOnTop.Checked = Boolean.Parse(settings.SelectSingleNode("/settings/multiborrowOnTop").InnerXml);

            this.saveHotKey.Text = settings.SelectSingleNode("/settings/saveHotKey").InnerXml;
            this.borrowHotKey.Text = settings.SelectSingleNode("/settings/borrowHotKey").InnerXml;
            this.helpHotKey.Text = settings.SelectSingleNode("/settings/helpHotKey").InnerXml;
            this.versionHotKey.Text = settings.SelectSingleNode("/settings/versionHotKey").InnerXml;
            this.refreshHotKey.Text = settings.SelectSingleNode("/settings/refreshHotKey").InnerXml;
            this.delHotKey.Text = settings.SelectSingleNode("/settings/delHotKey").InnerXml;

            this.csvFileName.Text = settings.SelectSingleNode("/settings/csvFileName").InnerXml;
            this.csvChar.Text = settings.SelectSingleNode("/settings/csvChar").InnerXml;

            this.primecolor.BackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tablePrimeColor").InnerXml);
            this.secondcolor.BackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSecondColor").InnerXml);
            this.selectionColor.BackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionColor").InnerXml);
            this.borrowedColor.BackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableBorrowedColor").InnerXml);
            this.borrowedSelectionColor.BackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionBorrowedColor").InnerXml);

            this.commentSearchColor.BackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/commentSearchColor").InnerXml);
            this.barcodeSearchColor.BackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/barcodeSearchColor").InnerXml);
            this.nameSearchColor.BackColor = ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/nameSearchColor").InnerXml);


            this.fontCombo.Text = settings.SelectSingleNode("/settings/tableFontFamily").InnerXml;

            this.language.Text = new CultureInfo(settings.SelectSingleNode("/settings/lang").InnerXml).DisplayName + " (" + settings.SelectSingleNode("/settings/lang").InnerXml + ")";
            this.styles.Text = settings.SelectSingleNode("/settings/style").InnerXml;

            this.textBox1.Text = settings.SelectSingleNode("/settings/maxWidthImage").InnerXml;

            this.fontSize.Value = int.Parse(settings.SelectSingleNode("/settings/tableFontSize").InnerXml) - 7;

            this.minLengthBarcode.Text = settings.SelectSingleNode("/settings/scannerMinKeyPress").InnerXml;
            this.scannerEnterpress.Text = settings.SelectSingleNode("/settings/scannerMinEnterPress").InnerXml;
            this.suffixCombo.Text = settings.SelectSingleNode("/settings/scannerSuffix").InnerXml;

            if (borrowColorCheck.Checked)
            {
                borrowedColor.Enabled = true;
                borrowedSelectionColor.Enabled = true;
            }
            else
            {
                borrowedColor.Enabled = false;
                borrowedSelectionColor.Enabled = false;
            }

        }

        public async Task saveExtraSettings() {
            await Task.Factory.StartNew(delegate
            {
                XmlDocument settingsSaveDoc = new XmlDocument();
                settingsSaveDoc.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));


                settingsSaveDoc.SelectSingleNode("/settings/scannerMinKeyPress").InnerXml = this.minLengthBarcode.Text;
                settingsSaveDoc.SelectSingleNode("/settings/scannerMinEnterPress").InnerXml = this.scannerEnterpress.Text;
                settingsSaveDoc.SelectSingleNode("/settings/scannerSuffix").InnerXml = this.suffixCombo.Text;

                settingsSaveDoc.Save(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            });
        }

        public async Task saveSettings(string sender) {

            Program.logger.log("Settingsmanager", "saved setting " + sender, Color.Green);

            XmlDocument settingsSaveDoc = main.loadSettingsDataBase();

            if (this.nameRadio.Checked)
            {
                settingsSaveDoc.SelectSingleNode("/settings/searchDefault").InnerXml = "0";
            }
            else if (this.barcodeRadio.Checked)
            {
                settingsSaveDoc.SelectSingleNode("/settings/searchDefault").InnerXml = "1";
            }
            else
            {
                settingsSaveDoc.SelectSingleNode("/settings/searchDefault").InnerXml = "2";
            }

            if (this.exactSearchCheck.Checked)
            {
                settingsSaveDoc.SelectSingleNode("/settings/searchExact").InnerXml = "true";
            }
            else
            {
                settingsSaveDoc.SelectSingleNode("/settings/searchExact").InnerXml = "false";
            }

            settingsSaveDoc.SelectSingleNode("/settings/showNameCol").InnerXml = this.nameCheck.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/showBarCol").InnerXml = this.barCheck.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/showStatusCol").InnerXml = this.statCheck.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/showComCol").InnerXml = this.ComCheck.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/showRegalCol").InnerXml = this.regalCheckBox.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/showRateCol").InnerXml = this.showRate.Checked.ToString();

            settingsSaveDoc.SelectSingleNode("/settings/borrowedItalic").InnerXml = this.borrowItalics.Checked.ToString();

            settingsSaveDoc.SelectSingleNode("/settings/borrowedColorful").InnerXml = this.borrowColorCheck.Checked.ToString();

            settingsSaveDoc.SelectSingleNode("/settings/searchWhileType").InnerXml = this.searchWhileType.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/scrollToSearch").InnerXml = this.scrollToSearch.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/searchCaseSensitive").InnerXml = this.caseSensitive.Checked.ToString();

            settingsSaveDoc.SelectSingleNode("/settings/winOpenOnce").InnerXml = this.winOpenOnce.Checked.ToString();

            settingsSaveDoc.SelectSingleNode("/settings/settingsOntop").InnerXml = this.settingsOntop.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/rmOnTop").InnerXml = this.rmOnTop.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/imageOnTop").InnerXml = this.imageOnTop.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/multiborrowOnTop").InnerXml = this.multiborrowOnTop.Checked.ToString();

            settingsSaveDoc.SelectSingleNode("/settings/saveHotKey").InnerXml = this.saveHotKey.Text;
            settingsSaveDoc.SelectSingleNode("/settings/borrowHotKey").InnerXml = this.borrowHotKey.Text;
            settingsSaveDoc.SelectSingleNode("/settings/helpHotKey").InnerXml = this.helpHotKey.Text;
            settingsSaveDoc.SelectSingleNode("/settings/versionHotKey").InnerXml = this.versionHotKey.Text;
            settingsSaveDoc.SelectSingleNode("/settings/refreshHotKey").InnerXml = this.refreshHotKey.Text;
            settingsSaveDoc.SelectSingleNode("/settings/delHotKey").InnerXml = this.delHotKey.Text;


            settingsSaveDoc.SelectSingleNode("/settings/tablePrimeColor").InnerXml = ColorTranslator.ToHtml(this.primecolor.BackColor);
            settingsSaveDoc.SelectSingleNode("/settings/tableSecondColor").InnerXml = ColorTranslator.ToHtml(this.secondcolor.BackColor);
            settingsSaveDoc.SelectSingleNode("/settings/tableSelectionColor").InnerXml = ColorTranslator.ToHtml(this.selectionColor.BackColor);
            settingsSaveDoc.SelectSingleNode("/settings/tableBorrowedColor").InnerXml = ColorTranslator.ToHtml(this.borrowedColor.BackColor);
            settingsSaveDoc.SelectSingleNode("/settings/tableSelectionBorrowedColor").InnerXml = ColorTranslator.ToHtml(this.borrowedSelectionColor.BackColor);

            settingsSaveDoc.SelectSingleNode("/settings/csvFileName").InnerXml = this.csvFileName.Text;
            settingsSaveDoc.SelectSingleNode("/settings/csvChar").InnerXml = this.csvChar.Text;

            settingsSaveDoc.SelectSingleNode("/settings/commentSearchColor").InnerXml = ColorTranslator.ToHtml(this.commentSearchColor.BackColor);
            settingsSaveDoc.SelectSingleNode("/settings/barcodeSearchColor").InnerXml = ColorTranslator.ToHtml(this.barcodeSearchColor.BackColor);
            settingsSaveDoc.SelectSingleNode("/settings/nameSearchColor").InnerXml = ColorTranslator.ToHtml(this.nameSearchColor.BackColor);

            settingsSaveDoc.SelectSingleNode("/settings/tableFontSize").InnerXml = (7 + fontSize.Value).ToString();

            settingsSaveDoc.SelectSingleNode("/settings/tableFontFamily").InnerXml = this.fontCombo.Text;

            settingsSaveDoc.SelectSingleNode("/settings/lang").InnerXml = this.language.Text.Split('(').Last().Substring(0, 2);
            settingsSaveDoc.SelectSingleNode("/settings/style").InnerXml = this.styles.Text;


            settingsSaveDoc.SelectSingleNode("/settings/maxWidthImage").InnerXml = this.textBox1.Text;


            await Task.Factory.StartNew(delegate
            {
                settingsSaveDoc.Save(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            });

            if (jokesDbChanged)
            {
                jokesDbChanged = false;
            }


            this.main.settingsChanged = true;
            this.main.applyInitialSettings();
            this.main.applyTableColors();
        }

        private async void nameRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (this.nameRadio.Checked && this.nameRadio.Focused)
            {
                await saveSettings("nameRadio");
            }
        }

        private async void barcodeRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (this.barcodeRadio.Checked && this.barcodeRadio.Focused)
            {
                await saveSettings("barcodeRadio");
            }
        }

        private async void commentRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (this.commentRadio.Checked && this.commentRadio.Focused)
            {
                await saveSettings("commentRadio");
            }
        }

        private async void exactSearchCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (this.exactSearchCheck.Focused)
            {
                await saveSettings("exactSearch");
            }
        }

        private async void nameCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (this.nameCheck.Focused)
            {
                await saveSettings("nameCheck");
            }
        }

        private async void barCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (this.barCheck.Focused)
            {
               await saveSettings("barCheck");
            }
        }

        private async void statCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (this.statCheck.Focused)
            {
                await saveSettings("statCheck");
            }
        }

        private async void ComCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ComCheck.Focused)
            {
                await saveSettings("ComCheck");
            }
        }

        private async void regalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.regalCheckBox.Focused)
            {
                await saveSettings("regalCheck");
            }
        }

        private async void primecolor_Click(object sender, EventArgs e)
        {
            ColorDialog primeDialog = new ColorDialog();

            primeDialog.CustomColors = new int[] { ColorTranslator.ToOle(this.primecolor.BackColor) };
            primeDialog.Color = this.primecolor.BackColor;
            primeDialog.ShowDialog();

            Color primeColor = primeDialog.Color;

            this.primecolor.BackColor = primeColor;

            await saveSettings("primeColor");
        }

        private async void secondcolor_Click(object sender, EventArgs e)
        {
            ColorDialog secondDialog = new ColorDialog();

            secondDialog.CustomColors = new int[] { ColorTranslator.ToOle(this.secondcolor.BackColor) };
            secondDialog.Color = this.secondcolor.BackColor;
            secondDialog.ShowDialog();

            Color secondColor = secondDialog.Color;

            this.secondcolor.BackColor = secondColor;

            await saveSettings("secondColor");
        }

       

        private async void borrowItalics_CheckedChanged(object sender, EventArgs e)
        {
            if (borrowItalics.Focused)
            {
                await saveSettings("borrowItalics");
            }
        }

        private async void selectionColor_Click(object sender, EventArgs e)
        {
            ColorDialog selectColorDialog = new ColorDialog();

            selectColorDialog.CustomColors = new int[] { ColorTranslator.ToOle(this.selectionColor.BackColor) };
            selectColorDialog.Color = this.selectionColor.BackColor;
            selectColorDialog.ShowDialog();

            Color selectColor = selectColorDialog.Color;

            this.selectionColor.BackColor = selectColor;

            await saveSettings("selectionColor");
        }

        private async void borrowedColor_Click(object sender, EventArgs e)
        {
            ColorDialog borrowedColorDialog = new ColorDialog();

            borrowedColorDialog.CustomColors = new int[] { ColorTranslator.ToOle(this.borrowedColor.BackColor) };
            borrowedColorDialog.Color = this.borrowedColor.BackColor;
            borrowedColorDialog.ShowDialog();

            Color borrowedColor = borrowedColorDialog.Color;

            this.borrowedColor.BackColor = borrowedColor;

            await saveSettings("borrowedColor");
        }

        private async void borrowColorCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (borrowColorCheck.Checked)
            {
                borrowedColor.Enabled = true;
                borrowedSelectionColor.Enabled = true;
            }
            else
            {
                borrowedColor.Enabled = false;
                borrowedSelectionColor.Enabled = false;
            }

            if (this.borrowColorCheck.Focused)
            {
                await saveSettings("borrowColorCheck");
            }
        }

        private async void borrowedSelectionColor_Click(object sender, EventArgs e)
        {
            ColorDialog borrowedSelectionColorDialog = new ColorDialog();

            borrowedSelectionColorDialog.CustomColors = new int[] { ColorTranslator.ToOle(this.borrowedSelectionColor.BackColor) };
            borrowedSelectionColorDialog.Color = this.borrowedSelectionColor.BackColor;
            borrowedSelectionColorDialog.ShowDialog();

            Color borrowedSelectionColor = borrowedSelectionColorDialog.Color;

            this.borrowedSelectionColor.BackColor = borrowedSelectionColor;

            await saveSettings("borrwedSelecetionColor");
        }

        private async void commentSearchColor_Click(object sender, EventArgs e)
        {
            ColorDialog commentSearchColorDialog = new ColorDialog();

            commentSearchColorDialog.CustomColors = new int[] { ColorTranslator.ToOle(this.commentSearchColor.BackColor) };
            commentSearchColorDialog.Color = this.commentSearchColor.BackColor;
            commentSearchColorDialog.ShowDialog();

            Color commentSearchColor = commentSearchColorDialog.Color;

            this.commentSearchColor.BackColor = commentSearchColor;

            await saveSettings("commentSearchColor");
        }

        private async void barcodeSearchColor_Click(object sender, EventArgs e)
        {
            ColorDialog barcodeSearchColorDialog = new ColorDialog();

            barcodeSearchColorDialog.CustomColors = new int[] { ColorTranslator.ToOle(this.barcodeSearchColor.BackColor) };
            barcodeSearchColorDialog.Color = this.barcodeSearchColor.BackColor;
            barcodeSearchColorDialog.ShowDialog();

            Color barcodeSearchColor = barcodeSearchColorDialog.Color;

            this.barcodeSearchColor.BackColor = barcodeSearchColor;

            await saveSettings("barcodeSearchColor");
        }

        private async void nameSearchColor_Click(object sender, EventArgs e)
        {
            ColorDialog nameSearchColorDialog = new ColorDialog();

            nameSearchColorDialog.CustomColors = new int[] { ColorTranslator.ToOle(this.nameSearchColor.BackColor) };
            nameSearchColorDialog.Color = this.nameSearchColor.BackColor;
            nameSearchColorDialog.ShowDialog();

            Color nameSearchColor = nameSearchColorDialog.Color;

            this.nameSearchColor.BackColor = nameSearchColor;

            await saveSettings("nameSearchColor");
        }

        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.fontCombo.Focused == true) {
                await saveSettings("font");
            }
        }


        private async void searchWhileType_CheckedChanged(object sender, EventArgs e)
        {
            if (this.searchWhileType.Focused)
            {
                await saveSettings("searchWhileType");
            }
        }

        private async void scrollToSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (this.scrollToSearch.Focused)
            {
                await saveSettings("scrollToSearch");
            }
        }

        private void minLengthBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void scannerEnterpress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void fontCombo_DrawItem(object sender, DrawItemEventArgs e)
        {
            Font font = fontCombo.Font;
            Brush brush = Brushes.Black;
            string text = fontCombo.Items[e.Index].ToString();

            if (!text.Equals("") || !text.Equals("(Standardschrift)")) {
                font = new Font(text, font.Size);
            }

            e.Graphics.DrawString(text, font, brush, e.Bounds);
        }

        private async void settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (changesMade)
            {
                DialogResult reallyDelete;
                reallyDelete = MessageBox.Show("Möchtest du die Änderungen wirklich Speichern?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (reallyDelete == DialogResult.Yes)
                {
                    await saveExtraSettings();
                }
            }
            Main.currentOpenWindows.Remove(this.Name);
        }

        private void minLengthBarcode_TextChanged(object sender, EventArgs e)
        {
            if (minLengthBarcode.Focused) {
                changesMade = true;
            }
        }

        private void scannerEnterpress_TextChanged(object sender, EventArgs e)
        {
            if (scannerEnterpress.Focused) {
                changesMade = true;
            }
        }

        private void suffixCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suffixCombo.Focused) {
                changesMade = true;
            }
        }

        private void settings_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(this.Name);
        }

        private async void winOpenOnce_CheckedChanged(object sender, EventArgs e)
        {
            if (this.winOpenOnce.Focused) {
                await saveSettings("winOpenOnce");
            }
        }

        private async void settingsOntop_CheckedChanged(object sender, EventArgs e)
        {
            if (settingsOntop.Focused) {
                await saveSettings("settingsOnTop");
            }
        }

        private async void rmOnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (rmOnTop.Focused) {
                await saveSettings("rmOnTop");
            }
        }

        private async void imageOnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (imageOnTop.Focused) {
                await saveSettings("imagenOnTop");
            }
        }

        private async void multiborrowOnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (multiborrowOnTop.Focused) {
                await saveSettings("multiborrowOnTop");
            }
        }

        private async void saveHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            
            if (saveHotKey.Focused)
            {
                string keyOut = "";
                if (e.Control)
                {
                    keyOut += "Ctrl + ";
                }
                if (e.Alt)
                {
                    keyOut += "Alt + ";
                }
                if (e.Shift)
                {
                    keyOut += "Shift + ";
                }

                keyOut += e.KeyData.ToString().Split(',')[0];

                saveHotKey.Text = keyOut;
                this.hotKeyHolder.Focus();
                await saveSettings("saveHotKey");
            }
        }

        private async void borrowHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (borrowHotKey.Focused)
            {
                string keyOut = "";
                if (e.Control)
                {
                    keyOut += "Ctrl + ";
                }
                if (e.Alt)
                {
                    keyOut += "Alt + ";
                }
                if (e.Shift)
                {
                    keyOut += "Shift + ";
                }

                keyOut += e.KeyData.ToString().Split(',')[0];

                borrowHotKey.Text = keyOut;
                this.hotKeyHolder.Focus();
                await saveSettings("borrowHotKey");
            }
        }

        private async void helpHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (helpHotKey.Focused)
            {
                string keyOut = "";
                if (e.Control)
                {
                    keyOut += "Ctrl + ";
                }
                if (e.Alt)
                {
                    keyOut += "Alt + ";
                }
                if (e.Shift)
                {
                    keyOut += "Shift + ";
                }

                keyOut += e.KeyData.ToString().Split(',')[0];

                helpHotKey.Text = keyOut;
                this.hotKeyHolder.Focus();
                await saveSettings("helpHotKey");
            }
        }

        private async void versionHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (versionHotKey.Focused)
            {
                string keyOut = "";
                if (e.Control)
                {
                    keyOut += "Ctrl + ";
                }
                if (e.Alt)
                {
                    keyOut += "Alt + ";
                }
                if (e.Shift)
                {
                    keyOut += "Shift + ";
                }

                keyOut += e.KeyData.ToString().Split(',')[0];

                versionHotKey.Text = keyOut;
                this.hotKeyHolder.Focus();
                await saveSettings("versionHotKey");
            }
        }

        

        private async void refreshHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (refreshHotKey.Focused)
            {
                string keyOut = "";
                if (e.Control)
                {
                    keyOut += "Ctrl + ";
                }
                if (e.Alt)
                {
                    keyOut += "Alt + ";
                }
                if (e.Shift)
                {
                    keyOut += "Shift + ";
                }

                keyOut += e.KeyData.ToString().Split(',')[0];

                refreshHotKey.Text = keyOut;
                this.hotKeyHolder.Focus();
                await saveSettings("refreshHotKey");
            }
        }

        private async void delHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (delHotKey.Focused)
            {
                string keyOut = "";
                if (e.Control)
                {
                    keyOut += "Ctrl + ";
                }
                if (e.Alt)
                {
                    keyOut += "Alt + ";
                }
                if (e.Shift)
                {
                    keyOut += "Shift + ";
                }

                keyOut += e.KeyData.ToString().Split(',')[0];

                delHotKey.Text = keyOut;
                this.hotKeyHolder.Focus();
                await saveSettings("delHotKey");
            }
        }

        private async void resetHot_Click(object sender, EventArgs e)
        {
            this.saveHotKey.Text = "Ctrl + S";
            this.borrowHotKey.Text = "Ctrl + E";
            this.helpHotKey.Text = "F1";
            this.versionHotKey.Text = "Ctrl + Alt + V";
            this.refreshHotKey.Text = "F5";
            this.delHotKey.Text = "Ctrl + Delete";
            await saveSettings("refreshHotKey");
        }

        private async void fontCombo_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.fontCombo.Focused == true && e.KeyCode==Keys.Enter)
            {
                await saveSettings("fontCombo");
            }
        }

        private async void fontSize_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.fontSize.Focused)
            {
                await saveSettings("fontSize");
            }
        }

        private async void showRate_CheckedChanged(object sender, EventArgs e)
        {
            if (this.showRate.Focused)
            {
                await saveSettings("showRate");
            }
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private async void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Focused)
            {
                await saveSettings("picWidth");
            }
        }

        private async void CaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            if (caseSensitive.Focused) {
                await saveSettings("caseSensitive");
            }
        }

        private async void CsvFileName_TextChanged(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = FileManager.GetDataBasePath("Files");
            ofd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK) {
                await Task.Factory.StartNew(delegate
                {
                    if (!ofd.FileName.Contains(FileManager.GetDataBasePath("Files")))
                    {
                        File.Copy(ofd.FileName, FileManager.GetDataBasePath("Files\\" + ofd.FileName.Split('\\').Last()), true);

                    }

                });

                this.csvFileName.Text = ofd.FileName.Split('\\').Last();
            }
            jokesDbChanged = true;

            await saveSettings("csv Name");
        }

        private bool jokesDbChanged = false;

        private async void CsvChar_TextChanged(object sender, EventArgs e)
        {
            if (csvChar.Focused) {
                await saveSettings("csvChar");
            }
        }

        private async void Language_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (language.Focused) {
                await saveSettings("lang");
            }
        }

        private async void Styles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (styles.Focused) {
                await saveSettings("style");
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //clean obsolete files
            XmlDocument doc = main.loadDataBase();
            List<string> actualBarcodes = new List<string>();

            foreach (XmlNode node in doc.SelectNodes("/data/item/barcode")) {
                actualBarcodes.Add(node.InnerText.Trim());
            }


            string[] files = Directory.GetFiles(FileManager.GetDataBasePath("Comments/"));

            int i = 0;
            foreach (string file in files) {
                string[] split1 = file.Split('\\');
                string[] split2 = split1[split1.Length - 1].Split('/');
                string fileName = split2[split2.Length-1].Split('.')[0];
                Console.WriteLine(fileName + " " + actualBarcodes.Count());
                if (!actualBarcodes.Contains(fileName.Trim()))
                {
                    if (File.Exists(file)) {
                        File.Delete(file);
                        i++;
                    }
                    
                }
            }

            MessageBox.Show(i + " obsolete *.rtf Comment files cleaned", "Info");
        }
    }
}
