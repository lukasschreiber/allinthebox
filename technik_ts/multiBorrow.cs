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
using Xceed.Words.NET;

namespace allinthebox
{
    public partial class multiBorrow : Form
    {
        XmlDocument doc;
        Main main;
        int iconStyle = 1;

        public multiBorrow(Main m)
        {
            InitializeComponent();

            //language
            this.label2.Text = Properties.strings.MultiAuswahlTool;
            this.borrow.Text = Properties.strings.borrow;
            this.back.Text = Properties.strings.back;
            this.saveToWord.Text = Properties.strings.saveAsDoc;
            this.reset.Text = Properties.strings.reset;
            this.dataGridView1.Columns["names"].HeaderText = Properties.strings.name;
            this.dataGridView1.Columns["barcode"].HeaderText = Properties.strings.barcode;


            main = m;

            this.MaximizeBox = false;
            if (main.multiBorrowOnTop)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }

            doc = new XmlDocument();
            doc.Load(FileManager.GetDataBasePath(FileManager.NAMES.DATA));
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex.Equals(0))
            {
                try
                {
                    foreach (XmlNode n in doc.SelectNodes("/data/item/barcode"))
                    {
                        if (n.InnerXml.Equals(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                        {
                            dataGridView1.Rows[e.RowIndex].Cells[1].Value = doc.SelectSingleNode("/data/item[barcode='" + n.InnerXml + "']/itemName").InnerXml;

                        }
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.StackTrace);
                }
            }
        }


        private async void borrow_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                XmlNode userNode = doc.SelectSingleNode("/data/item[barcode='" + dataGridView1.Rows[row.Index].Cells[0].Value + "']/user");
                XmlNode statNode = doc.SelectSingleNode("/data/item[barcode='" + dataGridView1.Rows[row.Index].Cells[0].Value + "']/status");

                if (userNode != null && statNode != null)
                {
                    userNode.InnerXml = main.user_loggedin;
                    statNode.InnerXml = "0";
                    doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.DATA));
                    main.changed = true;
                }

            }

            DialogResult reallyDelete;
            reallyDelete = MessageBox.Show(Properties.strings.wantToSave, Properties.strings.saveQuestion, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (reallyDelete == DialogResult.Yes)
            {
                await saveFile();
            }

            main.refresh(false);
        }

        private void back_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                XmlNode userNode = doc.SelectSingleNode("/data/item[barcode='" + dataGridView1.Rows[row.Index].Cells[0].Value + "']/user");
                XmlNode statNode = doc.SelectSingleNode("/data/item[barcode='" + dataGridView1.Rows[row.Index].Cells[0].Value + "']/status");

                if (userNode != null && statNode != null)
                {
                    userNode.InnerXml = "";
                    statNode.InnerXml = "1";
                    doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.DATA));
                    main.changed = true;
                }

            }

            main.refresh(false);

        }

        private void multiBorrow_Load(object sender, EventArgs e)
        {
            Main.currentOpenWindows.Add(this.Name);
        }

        private void multiBorrow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.currentOpenWindows.Remove(this.Name);
        }

        private async void saveToWord_Click(object sender, EventArgs e)
        {
            await saveFile();

        }

        private async Task saveFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Properties.strings.doc + "|*.docx|" + Properties.strings.excel + "|*.xlsx";
            sfd.ShowDialog();

            string fileName = sfd.FileName;


            if (fileName != "" || fileName != null)
            {
                if (sfd.FilterIndex == 1)
                {
                    await Task.Factory.StartNew(delegate
                    {
                        try
                        {
                            DocX doc = DocX.Create(fileName);

                            Xceed.Words.NET.Formatting titleFormat = new Xceed.Words.NET.Formatting();
                            titleFormat.FontFamily = new Xceed.Words.NET.Font("Calibri");
                            titleFormat.Size = 18;
                            titleFormat.Position = 40;
                            titleFormat.Bold = true;

                            doc.InsertParagraph(Properties.strings.listSentence_1 + " " + main.user_loggedin + " " + Properties.strings.listSentence_2 + " " + System.DateTime.Today.Date.ToString("dd.MM.yyyy"), false, titleFormat);



                            if (dataGridView1.Rows.Count > 0)
                            {

                                Table t = doc.AddTable(dataGridView1.Rows.Count, 4);
                                t.Alignment = Alignment.left;
                                t.Design = TableDesign.LightGrid;
                                t.SetBorder(TableBorderType.InsideH, new Border());
                                t.SetBorder(TableBorderType.InsideV, new Border());
                                t.SetBorder(TableBorderType.Left, new Border());
                                t.SetBorder(TableBorderType.Right, new Border());
                                t.SetBorder(TableBorderType.Top, new Border());
                                t.SetBorder(TableBorderType.Bottom, new Border());

                                t.Rows[0].Cells[0].Paragraphs.First().Append(Properties.strings.name);
                                t.Rows[0].Cells[1].Paragraphs.First().Append(Properties.strings.barcode);
                                t.Rows[0].Cells[2].Paragraphs.First().Append(Properties.strings.comment);
                                t.Rows[0].Cells[3].Paragraphs.First().Append(Properties.strings.tick);


                                for (int i = 0; i < t.RowCount - 1; i++)
                                {
                                    t.Rows[i + 1].Cells[0].Paragraphs.First().Append(dataGridView1.Rows[i].Cells[1].Value.ToString());
                                    t.Rows[i + 1].Cells[1].Paragraphs.First().Append(dataGridView1.Rows[i].Cells[0].Value.ToString());
                                    t.Rows[i + 1].Cells[2].Paragraphs.First().Append(this.doc.SelectSingleNode("/data/item[barcode='" + dataGridView1.Rows[i].Cells[0].Value + "']/comment").InnerXml.Trim());

                                }

                                doc.InsertTable(t);

                            }

                            doc.Save();
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.Message);
                        }
                        finally
                        {
                            string[] fileNameSplit = fileName.Split('\\');
                            string fileNameOnly = fileNameSplit.Last();
                            if (File.Exists(fileName))
                            {
                                MessageBox.Show(Properties.strings.file + " \"" + fileNameOnly + "\" " + Properties.strings.fileSavedSentence_2);
                            }
                        }

                    });

                }

                if (sfd.FilterIndex == 2)
                {
                    await Task.Factory.StartNew(delegate {
                        // Creating a Excel object. 
                        Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
                        Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
                        Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

                        try
                        {

                            worksheet = workbook.ActiveSheet;

                            worksheet.Name = Properties.strings.list;

                            int cellRowIndex = 1;
                            int cellColumnIndex = 1;

                            worksheet.Cells[1, 1].EntireRow.Font.Bold = true;

                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    worksheet.Cells[cellRowIndex, cellColumnIndex].Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                    if (cellRowIndex == 1)
                                    {
                                        switch (j)
                                        {
                                            case 0: worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Columns[0].HeaderText; break;
                                            case 1: worksheet.Cells[cellRowIndex, cellColumnIndex] = dataGridView1.Columns[1].HeaderText; break;
                                            case 2: worksheet.Cells[cellRowIndex, cellColumnIndex] = Properties.strings.comment; break;
                                            case 3: worksheet.Cells[cellRowIndex, cellColumnIndex] = Properties.strings.tick; break;
                                        }

                                    }
                                    else
                                    {
                                        switch (j)
                                        {
                                            case 0: worksheet.Cells[cellRowIndex, cellColumnIndex] = "\t" + dataGridView1.Rows[i - 1].Cells[0].Value.ToString(); break;
                                            case 1: worksheet.Cells[cellRowIndex, cellColumnIndex] = "\t" + dataGridView1.Rows[i - 1].Cells[1].Value.ToString(); break;
                                            case 2: worksheet.Cells[cellRowIndex, cellColumnIndex] = this.doc.SelectSingleNode("/data/item[barcode='" + dataGridView1.Rows[i - 1].Cells[0].Value + "']/comment").InnerXml.Trim(); break;
                                            case 3: worksheet.Cells[cellRowIndex, cellColumnIndex] = ""; break;
                                        }
                                    }
                                    cellColumnIndex++;
                                }
                                cellColumnIndex = 1;
                                cellRowIndex++;
                            }

                            worksheet.Cells[dataGridView1.Rows.Count, 1].Columns.EntireColumn.AutoFit();
                            worksheet.Cells[dataGridView1.Rows.Count, 2].Columns.EntireColumn.AutoFit();
                            worksheet.Cells[dataGridView1.Rows.Count, 3].Columns.EntireColumn.AutoFit();
                            worksheet.Cells[dataGridView1.Rows.Count,4].Columns.EntireColumn.AutoFit();

                            workbook.SaveAs(fileName);

                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            string[] fileNameSplit = fileName.Split('\\');
                            string fileNameOnly = fileNameSplit.Last();
                            MessageBox.Show(Properties.strings.file + " \"" + fileNameOnly + "\" " + Properties.strings.fileSavedSentence_2);
                            excel.Quit();
                            workbook = null;
                            excel = null;
                        }
                    });

                }
            }
        }

        private void reset_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == main.delHotKey.key && (e.Control || !main.delHotKey.controlNeeded) && (e.Alt || !main.delHotKey.altNeeded) && (e.Shift || !main.delHotKey.shiftNeeded))
            {

                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    MessageBox.Show("");

                    dataGridView1.Rows.RemoveAt(row.Index);
                }
            }

            if (e.KeyCode == main.saveHotKey.key && (e.Control || !main.saveHotKey.controlNeeded) && (e.Alt || !main.saveHotKey.altNeeded) && (e.Shift || !main.saveHotKey.shiftNeeded))
            {
                saveFile();
            }
        }

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row == 0) {
                using (SolidBrush brush = new SolidBrush(Color.Silver))
                    e.Graphics.FillRectangle(brush, e.CellBounds);
            }
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

