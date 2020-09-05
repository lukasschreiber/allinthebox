using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using allinthebox.Properties;

namespace allinthebox
{
    public partial class settings : Form
    {
        private bool changesMade;

        private bool jokesDbChanged;

        public string[] keys = {"F1", "F2", "F3", "F4", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12"};

        public Main main;

        public settings(bool admin, Main m)
        {
            InitializeComponent();


            //language


            groupBox2.Text = strings.tableView;
            nameCheck.Text = strings.name;
            barCheck.Text = strings.barcode;
            statCheck.Text = strings.state;
            ComCheck.Text = strings.comment;
            regalCheckBox.Text = strings.regal;
            showRate.Text = strings.rate;
            groupBox3.Text = strings.tableDesign;
            label2.Text = strings.primeColor;
            label3.Text = strings.secondColor;
            label6.Text = strings.selectColor;
            label5.Text = strings.fontSize;
            borrowItalics.Text = strings.borrowedItalic;
            borrowColorCheck.Text = strings.borrowedMarked;
            label7.Text = strings.borrowMarkedSelectedColor;
            label8.Text = strings.fontColor;
            groupBox4.Text = strings.scanner;
            label12.Text = strings.minLength;
            label13.Text = strings.suffixSendMin;
            label14.Text = strings.suffix;
            label15.Text = strings.noteScanner;
            groupBox1.Text = strings.searchSettings;
            label1.Text = strings.searchStandard;
            nameRadio.Text = strings.searchNames;
            barcodeRadio.Text = strings.searchBarcode;
            commentRadio.Text = strings.searchComments;
            exactSearchCheck.Text = strings.exactBarcode;
            label9.Text = strings.nameColor;
            label10.Text = strings.barcodeColor;
            label11.Text = strings.commentColor;
            searchWhileType.Text = strings.searchWhileTyping;
            scrollToSearch.Text = strings.scrollToFirst;
            groupBox5.Text = strings.windowSerttings;
            winOpenOnce.Text = strings.openOnce;
            settingsOntop.Text = strings.settingsOnTop;
            rmOnTop.Text = strings.rmOnTop;
            imageOnTop.Text = strings.imageOnTop;
            multiborrowOnTop.Text = strings.selectOnTop;
            hotKeyHolder.Text = strings.hotKeys;
            label16.Text = strings.save;
            label17.Text = strings.borrow;
            label18.Text = strings.help;
            label19.Text = strings.version;
            label21.Text = strings.refresh;
            label22.Text = strings.delete;
            resetHot.Text = strings.resetHotkeys;

            main = m;

            //get language and styles
            foreach (var d in Directory.GetDirectories(FileManager.GetBaseDirectoyPath()))
                if (d.Split('\\').Last().Length <= 2)
                    language.Items.Add(new CultureInfo(d.Split('\\').Last()).DisplayName + " (" + d.Split('\\').Last() +
                                       ")");

            //get Style
            foreach (var f in Directory.GetFiles(FileManager.GetDataBasePath("Styles"), "*.xml"))
                styles.Items.Add(f.Split('\\').Last().Split('.').First());


            if (main.settingsOnTop)
                TopMost = true;
            else
                TopMost = false;

            MaximizeBox = false;

            changesMade = false;

            if (admin)
                Text += " | Admin";
            else
                Text += " | Nutzer";

            if (!admin)
            {
                groupBox4.Enabled = false;
                hotKeyHolder.Enabled = false;
            }
            else if (admin)
            {
                groupBox4.Enabled = true;
                hotKeyHolder.Enabled = true;
            }

            var fontsCollection = new InstalledFontCollection();
            fontCombo.Items.Add("(Standardschrift)");
            foreach (var fontFamiliy in fontsCollection.Families) fontCombo.Items.Add(fontFamiliy.Name);

            suffixCombo.Items.AddRange(keys);

            var settings = new XmlDocument();
            settings.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));

            switch (int.Parse(settings.SelectSingleNode("/settings/searchDefault").InnerXml))
            {
                case 0:
                    nameRadio.Select();
                    break;
                case 1:
                    barcodeRadio.Select();
                    break;
                case 2:
                    commentRadio.Select();
                    break;
            }

            exactSearchCheck.Checked = bool.Parse(settings.SelectSingleNode("/settings/searchExact").InnerXml);

            nameCheck.Checked = bool.Parse(settings.SelectSingleNode("/settings/showNameCol").InnerXml);
            barCheck.Checked = bool.Parse(settings.SelectSingleNode("/settings/showBarCol").InnerXml);
            statCheck.Checked = bool.Parse(settings.SelectSingleNode("/settings/showStatusCol").InnerXml);
            ComCheck.Checked = bool.Parse(settings.SelectSingleNode("/settings/showComCol").InnerXml);
            regalCheckBox.Checked = bool.Parse(settings.SelectSingleNode("/settings/showRegalCol").InnerXml);
            showRate.Checked = bool.Parse(settings.SelectSingleNode("/settings/showRateCol").InnerXml);


            borrowItalics.Checked = bool.Parse(settings.SelectSingleNode("/settings/borrowedItalic").InnerXml);

            borrowColorCheck.Checked = bool.Parse(settings.SelectSingleNode("/settings/borrowedColorful").InnerXml);

            searchWhileType.Checked = bool.Parse(settings.SelectSingleNode("/settings/searchWhileType").InnerXml);
            scrollToSearch.Checked = bool.Parse(settings.SelectSingleNode("/settings/scrollToSearch").InnerXml);
            caseSensitive.Checked = bool.Parse(settings.SelectSingleNode("/settings/searchCaseSensitive").InnerXml);

            winOpenOnce.Checked = bool.Parse(settings.SelectSingleNode("/settings/winOpenOnce").InnerXml);

            settingsOntop.Checked = bool.Parse(settings.SelectSingleNode("/settings/settingsOntop").InnerXml);
            rmOnTop.Checked = bool.Parse(settings.SelectSingleNode("/settings/rmOnTop").InnerXml);
            imageOnTop.Checked = bool.Parse(settings.SelectSingleNode("/settings/imageOnTop").InnerXml);
            multiborrowOnTop.Checked = bool.Parse(settings.SelectSingleNode("/settings/multiborrowOnTop").InnerXml);

            saveHotKey.Text = settings.SelectSingleNode("/settings/saveHotKey").InnerXml;
            borrowHotKey.Text = settings.SelectSingleNode("/settings/borrowHotKey").InnerXml;
            helpHotKey.Text = settings.SelectSingleNode("/settings/helpHotKey").InnerXml;
            versionHotKey.Text = settings.SelectSingleNode("/settings/versionHotKey").InnerXml;
            refreshHotKey.Text = settings.SelectSingleNode("/settings/refreshHotKey").InnerXml;
            delHotKey.Text = settings.SelectSingleNode("/settings/delHotKey").InnerXml;

            csvFileName.Text = settings.SelectSingleNode("/settings/csvFileName").InnerXml;
            csvChar.Text = settings.SelectSingleNode("/settings/csvChar").InnerXml;

            primecolor.BackColor =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tablePrimeColor").InnerXml);
            secondcolor.BackColor =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSecondColor").InnerXml);
            selectionColor.BackColor =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionColor").InnerXml);
            borrowedColor.BackColor =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableBorrowedColor").InnerXml);
            borrowedSelectionColor.BackColor =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/tableSelectionBorrowedColor").InnerXml);

            commentSearchColor.BackColor =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/commentSearchColor").InnerXml);
            barcodeSearchColor.BackColor =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/barcodeSearchColor").InnerXml);
            nameSearchColor.BackColor =
                ColorTranslator.FromHtml(settings.SelectSingleNode("/settings/nameSearchColor").InnerXml);


            fontCombo.Text = settings.SelectSingleNode("/settings/tableFontFamily").InnerXml;

            language.Text = new CultureInfo(settings.SelectSingleNode("/settings/lang").InnerXml).DisplayName + " (" +
                            settings.SelectSingleNode("/settings/lang").InnerXml + ")";
            styles.Text = settings.SelectSingleNode("/settings/style").InnerXml;

            textBox1.Text = settings.SelectSingleNode("/settings/maxWidthImage").InnerXml;

            fontSize.Value = int.Parse(settings.SelectSingleNode("/settings/tableFontSize").InnerXml) - 7;

            minLengthBarcode.Text = settings.SelectSingleNode("/settings/scannerMinKeyPress").InnerXml;
            scannerEnterpress.Text = settings.SelectSingleNode("/settings/scannerMinEnterPress").InnerXml;
            suffixCombo.Text = settings.SelectSingleNode("/settings/scannerSuffix").InnerXml;

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

        public async Task saveExtraSettings()
        {
            await Task.Factory.StartNew(delegate
            {
                var settingsSaveDoc = new XmlDocument();
                settingsSaveDoc.Load(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));


                settingsSaveDoc.SelectSingleNode("/settings/scannerMinKeyPress").InnerXml = minLengthBarcode.Text;
                settingsSaveDoc.SelectSingleNode("/settings/scannerMinEnterPress").InnerXml = scannerEnterpress.Text;
                settingsSaveDoc.SelectSingleNode("/settings/scannerSuffix").InnerXml = suffixCombo.Text;

                settingsSaveDoc.Save(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            });
        }

        public async Task saveSettings(string sender)
        {
            Program.logger.log("Settingsmanager", "saved setting " + sender, Color.Green);

            var settingsSaveDoc = main.loadSettingsDataBase();

            if (nameRadio.Checked)
                settingsSaveDoc.SelectSingleNode("/settings/searchDefault").InnerXml = "0";
            else if (barcodeRadio.Checked)
                settingsSaveDoc.SelectSingleNode("/settings/searchDefault").InnerXml = "1";
            else
                settingsSaveDoc.SelectSingleNode("/settings/searchDefault").InnerXml = "2";

            if (exactSearchCheck.Checked)
                settingsSaveDoc.SelectSingleNode("/settings/searchExact").InnerXml = "true";
            else
                settingsSaveDoc.SelectSingleNode("/settings/searchExact").InnerXml = "false";

            settingsSaveDoc.SelectSingleNode("/settings/showNameCol").InnerXml = nameCheck.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/showBarCol").InnerXml = barCheck.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/showStatusCol").InnerXml = statCheck.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/showComCol").InnerXml = ComCheck.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/showRegalCol").InnerXml = regalCheckBox.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/showRateCol").InnerXml = showRate.Checked.ToString();

            settingsSaveDoc.SelectSingleNode("/settings/borrowedItalic").InnerXml = borrowItalics.Checked.ToString();

            settingsSaveDoc.SelectSingleNode("/settings/borrowedColorful").InnerXml =
                borrowColorCheck.Checked.ToString();

            settingsSaveDoc.SelectSingleNode("/settings/searchWhileType").InnerXml = searchWhileType.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/scrollToSearch").InnerXml = scrollToSearch.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/searchCaseSensitive").InnerXml =
                caseSensitive.Checked.ToString();

            settingsSaveDoc.SelectSingleNode("/settings/winOpenOnce").InnerXml = winOpenOnce.Checked.ToString();

            settingsSaveDoc.SelectSingleNode("/settings/settingsOntop").InnerXml = settingsOntop.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/rmOnTop").InnerXml = rmOnTop.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/imageOnTop").InnerXml = imageOnTop.Checked.ToString();
            settingsSaveDoc.SelectSingleNode("/settings/multiborrowOnTop").InnerXml =
                multiborrowOnTop.Checked.ToString();

            settingsSaveDoc.SelectSingleNode("/settings/saveHotKey").InnerXml = saveHotKey.Text;
            settingsSaveDoc.SelectSingleNode("/settings/borrowHotKey").InnerXml = borrowHotKey.Text;
            settingsSaveDoc.SelectSingleNode("/settings/helpHotKey").InnerXml = helpHotKey.Text;
            settingsSaveDoc.SelectSingleNode("/settings/versionHotKey").InnerXml = versionHotKey.Text;
            settingsSaveDoc.SelectSingleNode("/settings/refreshHotKey").InnerXml = refreshHotKey.Text;
            settingsSaveDoc.SelectSingleNode("/settings/delHotKey").InnerXml = delHotKey.Text;


            settingsSaveDoc.SelectSingleNode("/settings/tablePrimeColor").InnerXml =
                ColorTranslator.ToHtml(primecolor.BackColor);
            settingsSaveDoc.SelectSingleNode("/settings/tableSecondColor").InnerXml =
                ColorTranslator.ToHtml(secondcolor.BackColor);
            settingsSaveDoc.SelectSingleNode("/settings/tableSelectionColor").InnerXml =
                ColorTranslator.ToHtml(selectionColor.BackColor);
            settingsSaveDoc.SelectSingleNode("/settings/tableBorrowedColor").InnerXml =
                ColorTranslator.ToHtml(borrowedColor.BackColor);
            settingsSaveDoc.SelectSingleNode("/settings/tableSelectionBorrowedColor").InnerXml =
                ColorTranslator.ToHtml(borrowedSelectionColor.BackColor);

            settingsSaveDoc.SelectSingleNode("/settings/csvFileName").InnerXml = csvFileName.Text;
            settingsSaveDoc.SelectSingleNode("/settings/csvChar").InnerXml = csvChar.Text;

            settingsSaveDoc.SelectSingleNode("/settings/commentSearchColor").InnerXml =
                ColorTranslator.ToHtml(commentSearchColor.BackColor);
            settingsSaveDoc.SelectSingleNode("/settings/barcodeSearchColor").InnerXml =
                ColorTranslator.ToHtml(barcodeSearchColor.BackColor);
            settingsSaveDoc.SelectSingleNode("/settings/nameSearchColor").InnerXml =
                ColorTranslator.ToHtml(nameSearchColor.BackColor);

            settingsSaveDoc.SelectSingleNode("/settings/tableFontSize").InnerXml = (7 + fontSize.Value).ToString();

            settingsSaveDoc.SelectSingleNode("/settings/tableFontFamily").InnerXml = fontCombo.Text;

            settingsSaveDoc.SelectSingleNode("/settings/lang").InnerXml =
                language.Text.Split('(').Last().Substring(0, 2);
            settingsSaveDoc.SelectSingleNode("/settings/style").InnerXml = styles.Text;


            settingsSaveDoc.SelectSingleNode("/settings/maxWidthImage").InnerXml = textBox1.Text;


            await Task.Factory.StartNew(delegate
            {
                settingsSaveDoc.Save(FileManager.GetDataBasePath(FileManager.NAMES.SETTINGS));
            });

            if (jokesDbChanged) jokesDbChanged = false;


            main.settingsChanged = true;
            main.applyInitialSettings();
            main.applyTableColors();
        }

        private async void nameRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (nameRadio.Checked && nameRadio.Focused) await saveSettings("nameRadio");
        }

        private async void barcodeRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (barcodeRadio.Checked && barcodeRadio.Focused) await saveSettings("barcodeRadio");
        }

        private async void commentRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (commentRadio.Checked && commentRadio.Focused) await saveSettings("commentRadio");
        }

        private async void exactSearchCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (exactSearchCheck.Focused) await saveSettings("exactSearch");
        }

        private async void nameCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (nameCheck.Focused) await saveSettings("nameCheck");
        }

        private async void barCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (barCheck.Focused) await saveSettings("barCheck");
        }

        private async void statCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (statCheck.Focused) await saveSettings("statCheck");
        }

        private async void ComCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (ComCheck.Focused) await saveSettings("ComCheck");
        }

        private async void regalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (regalCheckBox.Focused) await saveSettings("regalCheck");
        }

        private async void primecolor_Click(object sender, EventArgs e)
        {
            var primeDialog = new ColorDialog();

            primeDialog.CustomColors = new[] {ColorTranslator.ToOle(primecolor.BackColor)};
            primeDialog.Color = primecolor.BackColor;
            primeDialog.ShowDialog();

            var primeColor = primeDialog.Color;

            primecolor.BackColor = primeColor;

            await saveSettings("primeColor");
        }

        private async void secondcolor_Click(object sender, EventArgs e)
        {
            var secondDialog = new ColorDialog();

            secondDialog.CustomColors = new[] {ColorTranslator.ToOle(secondcolor.BackColor)};
            secondDialog.Color = secondcolor.BackColor;
            secondDialog.ShowDialog();

            var secondColor = secondDialog.Color;

            secondcolor.BackColor = secondColor;

            await saveSettings("secondColor");
        }


        private async void borrowItalics_CheckedChanged(object sender, EventArgs e)
        {
            if (borrowItalics.Focused) await saveSettings("borrowItalics");
        }

        private async void selectionColor_Click(object sender, EventArgs e)
        {
            var selectColorDialog = new ColorDialog();

            selectColorDialog.CustomColors = new[] {ColorTranslator.ToOle(selectionColor.BackColor)};
            selectColorDialog.Color = selectionColor.BackColor;
            selectColorDialog.ShowDialog();

            var selectColor = selectColorDialog.Color;

            selectionColor.BackColor = selectColor;

            await saveSettings("selectionColor");
        }

        private async void borrowedColor_Click(object sender, EventArgs e)
        {
            var borrowedColorDialog = new ColorDialog();

            borrowedColorDialog.CustomColors = new[] {ColorTranslator.ToOle(this.borrowedColor.BackColor)};
            borrowedColorDialog.Color = this.borrowedColor.BackColor;
            borrowedColorDialog.ShowDialog();

            var borrowedColor = borrowedColorDialog.Color;

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

            if (borrowColorCheck.Focused) await saveSettings("borrowColorCheck");
        }

        private async void borrowedSelectionColor_Click(object sender, EventArgs e)
        {
            var borrowedSelectionColorDialog = new ColorDialog();

            borrowedSelectionColorDialog.CustomColors =
                new[] {ColorTranslator.ToOle(this.borrowedSelectionColor.BackColor)};
            borrowedSelectionColorDialog.Color = this.borrowedSelectionColor.BackColor;
            borrowedSelectionColorDialog.ShowDialog();

            var borrowedSelectionColor = borrowedSelectionColorDialog.Color;

            this.borrowedSelectionColor.BackColor = borrowedSelectionColor;

            await saveSettings("borrwedSelecetionColor");
        }

        private async void commentSearchColor_Click(object sender, EventArgs e)
        {
            var commentSearchColorDialog = new ColorDialog();

            commentSearchColorDialog.CustomColors = new[] {ColorTranslator.ToOle(this.commentSearchColor.BackColor)};
            commentSearchColorDialog.Color = this.commentSearchColor.BackColor;
            commentSearchColorDialog.ShowDialog();

            var commentSearchColor = commentSearchColorDialog.Color;

            this.commentSearchColor.BackColor = commentSearchColor;

            await saveSettings("commentSearchColor");
        }

        private async void barcodeSearchColor_Click(object sender, EventArgs e)
        {
            var barcodeSearchColorDialog = new ColorDialog();

            barcodeSearchColorDialog.CustomColors = new[] {ColorTranslator.ToOle(this.barcodeSearchColor.BackColor)};
            barcodeSearchColorDialog.Color = this.barcodeSearchColor.BackColor;
            barcodeSearchColorDialog.ShowDialog();

            var barcodeSearchColor = barcodeSearchColorDialog.Color;

            this.barcodeSearchColor.BackColor = barcodeSearchColor;

            await saveSettings("barcodeSearchColor");
        }

        private async void nameSearchColor_Click(object sender, EventArgs e)
        {
            var nameSearchColorDialog = new ColorDialog();

            nameSearchColorDialog.CustomColors = new[] {ColorTranslator.ToOle(this.nameSearchColor.BackColor)};
            nameSearchColorDialog.Color = this.nameSearchColor.BackColor;
            nameSearchColorDialog.ShowDialog();

            var nameSearchColor = nameSearchColorDialog.Color;

            this.nameSearchColor.BackColor = nameSearchColor;

            await saveSettings("nameSearchColor");
        }

        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fontCombo.Focused) await saveSettings("font");
        }


        private async void searchWhileType_CheckedChanged(object sender, EventArgs e)
        {
            if (searchWhileType.Focused) await saveSettings("searchWhileType");
        }

        private async void scrollToSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (scrollToSearch.Focused) await saveSettings("scrollToSearch");
        }

        private void minLengthBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void scannerEnterpress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void fontCombo_DrawItem(object sender, DrawItemEventArgs e)
        {
            var font = fontCombo.Font;
            var brush = Brushes.Black;
            var text = fontCombo.Items[e.Index].ToString();

            if (!text.Equals("") || !text.Equals("(Standardschrift)")) font = new Font(text, font.Size);

            e.Graphics.DrawString(text, font, brush, e.Bounds);
        }

        private async void settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (changesMade)
            {
                DialogResult reallyDelete;
                reallyDelete = MessageBox.Show("Möchtest du die Änderungen wirklich Speichern?", "Delete Confirmation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (reallyDelete == DialogResult.Yes) await saveExtraSettings();
            }

            Main.currentOpenWindows.Remove(Name);
        }

        private void minLengthBarcode_TextChanged(object sender, EventArgs e)
        {
            if (minLengthBarcode.Focused) changesMade = true;
        }

        private void scannerEnterpress_TextChanged(object sender, EventArgs e)
        {
            if (scannerEnterpress.Focused) changesMade = true;
        }

        private void suffixCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suffixCombo.Focused) changesMade = true;
        }

        private void settings_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(Name);
        }

        private async void winOpenOnce_CheckedChanged(object sender, EventArgs e)
        {
            if (winOpenOnce.Focused) await saveSettings("winOpenOnce");
        }

        private async void settingsOntop_CheckedChanged(object sender, EventArgs e)
        {
            if (settingsOntop.Focused) await saveSettings("settingsOnTop");
        }

        private async void rmOnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (rmOnTop.Focused) await saveSettings("rmOnTop");
        }

        private async void imageOnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (imageOnTop.Focused) await saveSettings("imagenOnTop");
        }

        private async void multiborrowOnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (multiborrowOnTop.Focused) await saveSettings("multiborrowOnTop");
        }

        private async void saveHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (saveHotKey.Focused)
            {
                var keyOut = "";
                if (e.Control) keyOut += "Ctrl + ";
                if (e.Alt) keyOut += "Alt + ";
                if (e.Shift) keyOut += "Shift + ";

                keyOut += e.KeyData.ToString().Split(',')[0];

                saveHotKey.Text = keyOut;
                hotKeyHolder.Focus();
                await saveSettings("saveHotKey");
            }
        }

        private async void borrowHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (borrowHotKey.Focused)
            {
                var keyOut = "";
                if (e.Control) keyOut += "Ctrl + ";
                if (e.Alt) keyOut += "Alt + ";
                if (e.Shift) keyOut += "Shift + ";

                keyOut += e.KeyData.ToString().Split(',')[0];

                borrowHotKey.Text = keyOut;
                hotKeyHolder.Focus();
                await saveSettings("borrowHotKey");
            }
        }

        private async void helpHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (helpHotKey.Focused)
            {
                var keyOut = "";
                if (e.Control) keyOut += "Ctrl + ";
                if (e.Alt) keyOut += "Alt + ";
                if (e.Shift) keyOut += "Shift + ";

                keyOut += e.KeyData.ToString().Split(',')[0];

                helpHotKey.Text = keyOut;
                hotKeyHolder.Focus();
                await saveSettings("helpHotKey");
            }
        }

        private async void versionHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (versionHotKey.Focused)
            {
                var keyOut = "";
                if (e.Control) keyOut += "Ctrl + ";
                if (e.Alt) keyOut += "Alt + ";
                if (e.Shift) keyOut += "Shift + ";

                keyOut += e.KeyData.ToString().Split(',')[0];

                versionHotKey.Text = keyOut;
                hotKeyHolder.Focus();
                await saveSettings("versionHotKey");
            }
        }


        private async void refreshHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (refreshHotKey.Focused)
            {
                var keyOut = "";
                if (e.Control) keyOut += "Ctrl + ";
                if (e.Alt) keyOut += "Alt + ";
                if (e.Shift) keyOut += "Shift + ";

                keyOut += e.KeyData.ToString().Split(',')[0];

                refreshHotKey.Text = keyOut;
                hotKeyHolder.Focus();
                await saveSettings("refreshHotKey");
            }
        }

        private async void delHotKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (delHotKey.Focused)
            {
                var keyOut = "";
                if (e.Control) keyOut += "Ctrl + ";
                if (e.Alt) keyOut += "Alt + ";
                if (e.Shift) keyOut += "Shift + ";

                keyOut += e.KeyData.ToString().Split(',')[0];

                delHotKey.Text = keyOut;
                hotKeyHolder.Focus();
                await saveSettings("delHotKey");
            }
        }

        private async void resetHot_Click(object sender, EventArgs e)
        {
            saveHotKey.Text = "Ctrl + S";
            borrowHotKey.Text = "Ctrl + E";
            helpHotKey.Text = "F1";
            versionHotKey.Text = "Ctrl + Alt + V";
            refreshHotKey.Text = "F5";
            delHotKey.Text = "Ctrl + Delete";
            await saveSettings("refreshHotKey");
        }

        private async void fontCombo_KeyUp(object sender, KeyEventArgs e)
        {
            if (fontCombo.Focused && e.KeyCode == Keys.Enter) await saveSettings("fontCombo");
        }

        private async void fontSize_MouseUp(object sender, MouseEventArgs e)
        {
            if (fontSize.Focused) await saveSettings("fontSize");
        }

        private async void showRate_CheckedChanged(object sender, EventArgs e)
        {
            if (showRate.Focused) await saveSettings("showRate");
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private async void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Focused) await saveSettings("picWidth");
        }

        private async void CaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            if (caseSensitive.Focused) await saveSettings("caseSensitive");
        }

        private async void CsvFileName_TextChanged(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = FileManager.GetDataBasePath("Files");
            ofd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                await Task.Factory.StartNew(delegate
                {
                    if (!ofd.FileName.Contains(FileManager.GetDataBasePath("Files")))
                        File.Copy(ofd.FileName,
                            FileManager.GetDataBasePath("Files\\" + ofd.FileName.Split('\\').Last()), true);
                });

                csvFileName.Text = ofd.FileName.Split('\\').Last();
            }

            jokesDbChanged = true;

            await saveSettings("csv Name");
        }

        private async void CsvChar_TextChanged(object sender, EventArgs e)
        {
            if (csvChar.Focused) await saveSettings("csvChar");
        }

        private async void Language_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (language.Focused) await saveSettings("lang");
        }

        private async void Styles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (styles.Focused) await saveSettings("style");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //clean obsolete files
            var doc = main.loadDataBase();
            var actualBarcodes = new List<string>();

            foreach (XmlNode node in doc.SelectNodes("/data/item/barcode")) actualBarcodes.Add(node.InnerText.Trim());


            var files = Directory.GetFiles(FileManager.GetDataBasePath("Comments/"));

            var i = 0;
            foreach (var file in files)
            {
                var split1 = file.Split('\\');
                var split2 = split1[split1.Length - 1].Split('/');
                var fileName = split2[split2.Length - 1].Split('.')[0];
                Console.WriteLine(fileName + " " + actualBarcodes.Count());
                if (!actualBarcodes.Contains(fileName.Trim()))
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                        i++;
                    }
            }

            MessageBox.Show(i + " obsolete *.rtf Comment files cleaned", "Info");
        }
    }
}