using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using allinthebox.Properties;
using Microsoft.Office.Interop.Excel;
using Xceed.Words.NET;
using Application = Microsoft.Office.Interop.Excel.Application;
using Border = Xceed.Words.NET.Border;
using Font = Xceed.Words.NET.Font;
using Formatting = Xceed.Words.NET.Formatting;

namespace allinthebox
{
    public partial class multiBorrow : Form
    {
        private readonly XmlDocument doc;
        private int grabX, grabY;
        private readonly int iconStyle = 1;
        private readonly Main main;
        private bool mousedown, maximized;

        private int mouseX, mouseY;

        public multiBorrow(Main m)
        {
            InitializeComponent();

            //language
            label2.Text = strings.MultiAuswahlTool;
            borrow.Text = strings.borrow;
            back.Text = strings.back;
            saveToWord.Text = strings.saveAsDoc;
            reset.Text = strings.reset;
            dataGridView1.Columns["names"].HeaderText = strings.name;
            dataGridView1.Columns["barcode"].HeaderText = strings.barcode;


            main = m;

            MaximizeBox = false;
            if (main.multiBorrowOnTop)
                TopMost = true;
            else
                TopMost = false;

            doc = new XmlDocument();
            doc.Load(FileManager.GetDataBasePath(FileManager.NAMES.DATA));
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

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex.Equals(0))
                try
                {
                    foreach (XmlNode n in doc.SelectNodes("/data/item/barcode"))
                        if (n.InnerXml.Equals(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                            dataGridView1.Rows[e.RowIndex].Cells[1].Value = doc
                                .SelectSingleNode("/data/item[barcode='" + n.InnerXml + "']/itemName").InnerXml;
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.StackTrace);
                }
        }


        private async void borrow_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var userNode = doc.SelectSingleNode("/data/item[barcode='" +
                                                    dataGridView1.Rows[row.Index].Cells[0].Value + "']/user");
                var statNode = doc.SelectSingleNode("/data/item[barcode='" +
                                                    dataGridView1.Rows[row.Index].Cells[0].Value + "']/status");

                if (userNode != null && statNode != null)
                {
                    userNode.InnerXml = main.user_loggedin;
                    statNode.InnerXml = "0";
                    doc.Save(FileManager.GetDataBasePath(FileManager.NAMES.DATA));
                    main.changed = true;
                }
            }

            DialogResult reallyDelete;
            reallyDelete = MessageBox.Show(strings.wantToSave, strings.saveQuestion, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (reallyDelete == DialogResult.Yes) await saveFile();

            main.refresh(false);
        }

        private void back_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var userNode = doc.SelectSingleNode("/data/item[barcode='" +
                                                    dataGridView1.Rows[row.Index].Cells[0].Value + "']/user");
                var statNode = doc.SelectSingleNode("/data/item[barcode='" +
                                                    dataGridView1.Rows[row.Index].Cells[0].Value + "']/status");

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
            Main.currentOpenWindows.Add(Name);
        }

        private void multiBorrow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.currentOpenWindows.Remove(Name);
        }

        private async void saveToWord_Click(object sender, EventArgs e)
        {
            await saveFile();
        }

        private async Task saveFile()
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = strings.doc + "|*.docx|" + strings.excel + "|*.xlsx";
            sfd.ShowDialog();

            var fileName = sfd.FileName;


            if (fileName != "" || fileName != null)
            {
                if (sfd.FilterIndex == 1)
                    await Task.Factory.StartNew(delegate
                    {
                        try
                        {
                            var doc = DocX.Create(fileName);

                            var titleFormat = new Formatting();
                            titleFormat.FontFamily = new Font("Calibri");
                            titleFormat.Size = 18;
                            titleFormat.Position = 40;
                            titleFormat.Bold = true;

                            doc.InsertParagraph(
                                strings.listSentence_1 + " " + main.user_loggedin + " " + strings.listSentence_2 + " " +
                                DateTime.Today.Date.ToString("dd.MM.yyyy"), false, titleFormat);


                            if (dataGridView1.Rows.Count > 0)
                            {
                                var t = doc.AddTable(dataGridView1.Rows.Count, 4);
                                t.Alignment = Alignment.left;
                                t.Design = TableDesign.LightGrid;
                                t.SetBorder(TableBorderType.InsideH, new Border());
                                t.SetBorder(TableBorderType.InsideV, new Border());
                                t.SetBorder(TableBorderType.Left, new Border());
                                t.SetBorder(TableBorderType.Right, new Border());
                                t.SetBorder(TableBorderType.Top, new Border());
                                t.SetBorder(TableBorderType.Bottom, new Border());

                                t.Rows[0].Cells[0].Paragraphs.First().Append(strings.name);
                                t.Rows[0].Cells[1].Paragraphs.First().Append(strings.barcode);
                                t.Rows[0].Cells[2].Paragraphs.First().Append(strings.comment);
                                t.Rows[0].Cells[3].Paragraphs.First().Append(strings.tick);


                                for (var i = 0; i < t.RowCount - 1; i++)
                                {
                                    t.Rows[i + 1].Cells[0].Paragraphs.First()
                                        .Append(dataGridView1.Rows[i].Cells[1].Value.ToString());
                                    t.Rows[i + 1].Cells[1].Paragraphs.First()
                                        .Append(dataGridView1.Rows[i].Cells[0].Value.ToString());
                                    t.Rows[i + 1].Cells[2].Paragraphs.First().Append(this.doc
                                        .SelectSingleNode("/data/item[barcode='" +
                                                          dataGridView1.Rows[i].Cells[0].Value + "']/comment").InnerXml
                                        .Trim());
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
                            var fileNameSplit = fileName.Split('\\');
                            var fileNameOnly = fileNameSplit.Last();
                            if (File.Exists(fileName))
                                MessageBox.Show(strings.file + " \"" + fileNameOnly + "\" " +
                                                strings.fileSavedSentence_2);
                        }
                    });

                if (sfd.FilterIndex == 2)
                    await Task.Factory.StartNew(delegate
                    {
                        // Creating a Excel object. 
                        _Application excel = new Application();
                        _Workbook workbook = excel.Workbooks.Add(Type.Missing);
                        _Worksheet worksheet = null;

                        try
                        {
                            worksheet = workbook.ActiveSheet;

                            worksheet.Name = strings.list;

                            var cellRowIndex = 1;
                            var cellColumnIndex = 1;

                            worksheet.Cells[1, 1].EntireRow.Font.Bold = true;

                            for (var i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                for (var j = 0; j < 4; j++)
                                {
                                    worksheet.Cells[cellRowIndex, cellColumnIndex].Borders.LineStyle =
                                        XlLineStyle.xlContinuous;
                                    if (cellRowIndex == 1)
                                        switch (j)
                                        {
                                            case 0:
                                                worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                                    dataGridView1.Columns[0].HeaderText;
                                                break;
                                            case 1:
                                                worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                                    dataGridView1.Columns[1].HeaderText;
                                                break;
                                            case 2:
                                                worksheet.Cells[cellRowIndex, cellColumnIndex] = strings.comment;
                                                break;
                                            case 3:
                                                worksheet.Cells[cellRowIndex, cellColumnIndex] = strings.tick;
                                                break;
                                        }
                                    else
                                        switch (j)
                                        {
                                            case 0:
                                                worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                                    "\t" + dataGridView1.Rows[i - 1].Cells[0].Value;
                                                break;
                                            case 1:
                                                worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                                    "\t" + dataGridView1.Rows[i - 1].Cells[1].Value;
                                                break;
                                            case 2:
                                                worksheet.Cells[cellRowIndex, cellColumnIndex] =
                                                    doc.SelectSingleNode("/data/item[barcode='" +
                                                                         dataGridView1.Rows[i - 1].Cells[0].Value +
                                                                         "']/comment").InnerXml.Trim();
                                                break;
                                            case 3:
                                                worksheet.Cells[cellRowIndex, cellColumnIndex] = "";
                                                break;
                                        }

                                    cellColumnIndex++;
                                }

                                cellColumnIndex = 1;
                                cellRowIndex++;
                            }

                            worksheet.Cells[dataGridView1.Rows.Count, 1].Columns.EntireColumn.AutoFit();
                            worksheet.Cells[dataGridView1.Rows.Count, 2].Columns.EntireColumn.AutoFit();
                            worksheet.Cells[dataGridView1.Rows.Count, 3].Columns.EntireColumn.AutoFit();
                            worksheet.Cells[dataGridView1.Rows.Count, 4].Columns.EntireColumn.AutoFit();

                            workbook.SaveAs(fileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            var fileNameSplit = fileName.Split('\\');
                            var fileNameOnly = fileNameSplit.Last();
                            MessageBox.Show(strings.file + " \"" + fileNameOnly + "\" " + strings.fileSavedSentence_2);
                            excel.Quit();
                            workbook = null;
                            excel = null;
                        }
                    });
            }
        }

        private void reset_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == main.delHotKey.key && (e.Control || !main.delHotKey.controlNeeded) &&
                (e.Alt || !main.delHotKey.altNeeded) && (e.Shift || !main.delHotKey.shiftNeeded))
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    MessageBox.Show("");

                    dataGridView1.Rows.RemoveAt(row.Index);
                }

            if (e.KeyCode == main.saveHotKey.key && (e.Control || !main.saveHotKey.controlNeeded) &&
                (e.Alt || !main.saveHotKey.altNeeded) && (e.Shift || !main.saveHotKey.shiftNeeded)) saveFile();
        }

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row == 0)
                using (var brush = new SolidBrush(Color.Silver))
                {
                    e.Graphics.FillRectangle(brush, e.CellBounds);
                }
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