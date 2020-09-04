namespace allinthebox
{
    partial class multiBorrow
    {
        /// <summary>
        /// Required Designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(multiBorrow));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.closeButton = new System.Windows.Forms.PictureBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.barcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.names = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.borrow = new Bunifu.Framework.UI.BunifuFlatButton();
            this.back = new Bunifu.Framework.UI.BunifuFlatButton();
            this.saveToWord = new Bunifu.Framework.UI.BunifuFlatButton();
            this.reset = new Bunifu.Framework.UI.BunifuFlatButton();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.48598F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.51402F));
            this.tableLayoutPanel1.Controls.Add(this.closeButton, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.558139F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 92.44186F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(713, 516);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.tableLayoutPanel1_CellPaint);
            this.tableLayoutPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bg_MouseDown);
            this.tableLayoutPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bg_MouseMove);
            this.tableLayoutPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bg_MouseUp);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.Color.Transparent;
            this.closeButton.Image = global::allinthebox.Properties.Resources.close;
            this.closeButton.Location = new System.Drawing.Point(683, 5);
            this.closeButton.Margin = new System.Windows.Forms.Padding(175, 5, 5, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Padding = new System.Windows.Forms.Padding(5);
            this.closeButton.Size = new System.Drawing.Size(25, 25);
            this.closeButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.closeButton.TabIndex = 38;
            this.closeButton.TabStop = false;
            this.closeButton.Tag = "";
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            this.closeButton.MouseEnter += new System.EventHandler(this.closeButton_MouseEnter);
            this.closeButton.MouseLeave += new System.EventHandler(this.closeButton_MouseLeave);
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.Color.Silver;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.barcode,
            this.names});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(178, 42);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(531, 470);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyUp);
            // 
            // barcode
            // 
            this.barcode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.barcode.HeaderText = "Barcodes";
            this.barcode.MinimumWidth = 6;
            this.barcode.Name = "barcode";
            // 
            // names
            // 
            this.names.HeaderText = "Namen";
            this.names.MinimumWidth = 6;
            this.names.Name = "names";
            this.names.Width = 125;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.borrow);
            this.flowLayoutPanel1.Controls.Add(this.back);
            this.flowLayoutPanel1.Controls.Add(this.saveToWord);
            this.flowLayoutPanel1.Controls.Add(this.reset);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 42);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(166, 470);
            this.flowLayoutPanel1.TabIndex = 1;
            this.flowLayoutPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bg_MouseDown);
            this.flowLayoutPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bg_MouseMove);
            this.flowLayoutPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bg_MouseUp);
            // 
            // borrow
            // 
            this.borrow.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.borrow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.borrow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.borrow.BorderRadius = 0;
            this.borrow.ButtonText = "Ausleihen";
            this.borrow.Cursor = System.Windows.Forms.Cursors.Default;
            this.borrow.DisabledColor = System.Drawing.Color.Gray;
            this.borrow.Iconcolor = System.Drawing.Color.Transparent;
            this.borrow.Iconimage = null;
            this.borrow.Iconimage_right = null;
            this.borrow.Iconimage_right_Selected = null;
            this.borrow.Iconimage_Selected = null;
            this.borrow.IconMarginLeft = 0;
            this.borrow.IconMarginRight = 0;
            this.borrow.IconRightVisible = true;
            this.borrow.IconRightZoom = 0D;
            this.borrow.IconVisible = true;
            this.borrow.IconZoom = 90D;
            this.borrow.IsTab = false;
            this.borrow.Location = new System.Drawing.Point(10, 10);
            this.borrow.Margin = new System.Windows.Forms.Padding(10, 10, 5, 5);
            this.borrow.Name = "borrow";
            this.borrow.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.borrow.OnHovercolor = System.Drawing.Color.Silver;
            this.borrow.OnHoverTextColor = System.Drawing.Color.Black;
            this.borrow.selected = false;
            this.borrow.Size = new System.Drawing.Size(156, 34);
            this.borrow.TabIndex = 33;
            this.borrow.Text = "Ausleihen";
            this.borrow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.borrow.Textcolor = System.Drawing.Color.Black;
            this.borrow.TextFont = new System.Drawing.Font("Century Gothic", 9.75F);
            this.borrow.Click += new System.EventHandler(this.borrow_Click);
            // 
            // back
            // 
            this.back.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.back.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.back.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.back.BorderRadius = 0;
            this.back.ButtonText = "Zurückgeben";
            this.back.Cursor = System.Windows.Forms.Cursors.Default;
            this.back.DisabledColor = System.Drawing.Color.Gray;
            this.back.Iconcolor = System.Drawing.Color.Transparent;
            this.back.Iconimage = null;
            this.back.Iconimage_right = null;
            this.back.Iconimage_right_Selected = null;
            this.back.Iconimage_Selected = null;
            this.back.IconMarginLeft = 0;
            this.back.IconMarginRight = 0;
            this.back.IconRightVisible = true;
            this.back.IconRightZoom = 0D;
            this.back.IconVisible = true;
            this.back.IconZoom = 90D;
            this.back.IsTab = false;
            this.back.Location = new System.Drawing.Point(10, 59);
            this.back.Margin = new System.Windows.Forms.Padding(10, 10, 5, 5);
            this.back.Name = "back";
            this.back.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.back.OnHovercolor = System.Drawing.Color.Silver;
            this.back.OnHoverTextColor = System.Drawing.Color.Black;
            this.back.selected = false;
            this.back.Size = new System.Drawing.Size(156, 34);
            this.back.TabIndex = 34;
            this.back.Text = "Zurückgeben";
            this.back.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.back.Textcolor = System.Drawing.Color.Black;
            this.back.TextFont = new System.Drawing.Font("Century Gothic", 9.75F);
            this.back.Click += new System.EventHandler(this.back_Click);
            // 
            // saveToWord
            // 
            this.saveToWord.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.saveToWord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.saveToWord.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.saveToWord.BorderRadius = 0;
            this.saveToWord.ButtonText = "Als Dokument speichern";
            this.saveToWord.Cursor = System.Windows.Forms.Cursors.Default;
            this.saveToWord.DisabledColor = System.Drawing.Color.Gray;
            this.saveToWord.Iconcolor = System.Drawing.Color.Transparent;
            this.saveToWord.Iconimage = null;
            this.saveToWord.Iconimage_right = null;
            this.saveToWord.Iconimage_right_Selected = null;
            this.saveToWord.Iconimage_Selected = null;
            this.saveToWord.IconMarginLeft = 0;
            this.saveToWord.IconMarginRight = 0;
            this.saveToWord.IconRightVisible = true;
            this.saveToWord.IconRightZoom = 0D;
            this.saveToWord.IconVisible = true;
            this.saveToWord.IconZoom = 90D;
            this.saveToWord.IsTab = false;
            this.saveToWord.Location = new System.Drawing.Point(10, 108);
            this.saveToWord.Margin = new System.Windows.Forms.Padding(10, 10, 5, 5);
            this.saveToWord.Name = "saveToWord";
            this.saveToWord.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.saveToWord.OnHovercolor = System.Drawing.Color.Silver;
            this.saveToWord.OnHoverTextColor = System.Drawing.Color.Black;
            this.saveToWord.selected = false;
            this.saveToWord.Size = new System.Drawing.Size(156, 57);
            this.saveToWord.TabIndex = 35;
            this.saveToWord.Text = "Als Dokument speichern";
            this.saveToWord.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.saveToWord.Textcolor = System.Drawing.Color.Black;
            this.saveToWord.TextFont = new System.Drawing.Font("Century Gothic", 9.75F);
            this.saveToWord.Click += new System.EventHandler(this.saveToWord_Click);
            // 
            // reset
            // 
            this.reset.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.reset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.reset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.reset.BorderRadius = 0;
            this.reset.ButtonText = "Zurücksetzen";
            this.reset.Cursor = System.Windows.Forms.Cursors.Default;
            this.reset.DisabledColor = System.Drawing.Color.Gray;
            this.reset.Iconcolor = System.Drawing.Color.Transparent;
            this.reset.Iconimage = null;
            this.reset.Iconimage_right = null;
            this.reset.Iconimage_right_Selected = null;
            this.reset.Iconimage_Selected = null;
            this.reset.IconMarginLeft = 0;
            this.reset.IconMarginRight = 0;
            this.reset.IconRightVisible = true;
            this.reset.IconRightZoom = 0D;
            this.reset.IconVisible = true;
            this.reset.IconZoom = 90D;
            this.reset.IsTab = false;
            this.reset.Location = new System.Drawing.Point(10, 180);
            this.reset.Margin = new System.Windows.Forms.Padding(10, 10, 5, 5);
            this.reset.Name = "reset";
            this.reset.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.reset.OnHovercolor = System.Drawing.Color.Silver;
            this.reset.OnHoverTextColor = System.Drawing.Color.Black;
            this.reset.selected = false;
            this.reset.Size = new System.Drawing.Size(156, 34);
            this.reset.TabIndex = 36;
            this.reset.Text = "Zurücksetzen";
            this.reset.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.reset.Textcolor = System.Drawing.Color.Black;
            this.reset.TextFont = new System.Drawing.Font("Century Gothic", 9.75F);
            this.reset.Click += new System.EventHandler(this.reset_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 10);
            this.label2.Margin = new System.Windows.Forms.Padding(10, 10, 3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 18);
            this.label2.TabIndex = 39;
            this.label2.Text = "MultiAusleihTool";
            // 
            // multiBorrow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 516);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "multiBorrow";
            this.Text = "MultiAusleihen";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.multiBorrow_FormClosing);
            this.Load += new System.EventHandler(this.multiBorrow_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn barcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn names;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.PictureBox closeButton;
        private System.Windows.Forms.Label label2;
        private Bunifu.Framework.UI.BunifuFlatButton borrow;
        private Bunifu.Framework.UI.BunifuFlatButton back;
        private Bunifu.Framework.UI.BunifuFlatButton saveToWord;
        private Bunifu.Framework.UI.BunifuFlatButton reset;
    }
}