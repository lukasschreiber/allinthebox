namespace allinthebox
{
    partial class rm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(rm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.nameText = new System.Windows.Forms.TextBox();
            this.add = new System.Windows.Forms.Button();
            this.delete = new System.Windows.Forms.Button();
            this.refresh = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.preset = new System.Windows.Forms.Button();
            this.load_preset = new System.Windows.Forms.Button();
            this.bg = new System.Windows.Forms.Panel();
            this.number = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.bg.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.27011F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.72988F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.bg, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(933, 740);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.nameText);
            this.flowLayoutPanel1.Controls.Add(this.add);
            this.flowLayoutPanel1.Controls.Add(this.delete);
            this.flowLayoutPanel1.Controls.Add(this.refresh);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.preset);
            this.flowLayoutPanel1.Controls.Add(this.load_preset);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 4);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(199, 732);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // nameText
            // 
            this.nameText.Location = new System.Drawing.Point(4, 4);
            this.nameText.Margin = new System.Windows.Forms.Padding(4);
            this.nameText.Name = "nameText";
            this.nameText.Size = new System.Drawing.Size(155, 22);
            this.nameText.TabIndex = 2;
            // 
            // add
            // 
            this.add.Location = new System.Drawing.Point(4, 34);
            this.add.Margin = new System.Windows.Forms.Padding(4);
            this.add.Name = "add";
            this.add.Size = new System.Drawing.Size(156, 28);
            this.add.TabIndex = 0;
            this.add.Text = "Hinzufügen";
            this.add.UseVisualStyleBackColor = true;
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // delete
            // 
            this.delete.Location = new System.Drawing.Point(4, 70);
            this.delete.Margin = new System.Windows.Forms.Padding(4);
            this.delete.Name = "delete";
            this.delete.Size = new System.Drawing.Size(156, 28);
            this.delete.TabIndex = 1;
            this.delete.Text = "Löschen";
            this.delete.UseVisualStyleBackColor = true;
            this.delete.Click += new System.EventHandler(this.delete_Click);
            // 
            // refresh
            // 
            this.refresh.Location = new System.Drawing.Point(4, 106);
            this.refresh.Margin = new System.Windows.Forms.Padding(4);
            this.refresh.Name = "refresh";
            this.refresh.Size = new System.Drawing.Size(156, 28);
            this.refresh.TabIndex = 3;
            this.refresh.Text = "Aktualisieren";
            this.refresh.UseVisualStyleBackColor = true;
            this.refresh.Click += new System.EventHandler(this.refresh_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 138);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 136);
            this.label1.TabIndex = 5;
            this.label1.Text = "Um die Regale zuzuordnen, kann man diese links bearbeiten und frei auf dem Bild u" +
    "mher verschieben.  Das Hintergrundbild kann in Paint zum Beispiel erstellt werde" +
    "n. Dazu einfach die Vorlage speichern.";
            // 
            // preset
            // 
            this.preset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.preset.Location = new System.Drawing.Point(4, 278);
            this.preset.Margin = new System.Windows.Forms.Padding(4);
            this.preset.Name = "preset";
            this.preset.Size = new System.Drawing.Size(156, 28);
            this.preset.TabIndex = 4;
            this.preset.Text = "Leere Vorlage";
            this.preset.UseVisualStyleBackColor = true;
            this.preset.Click += new System.EventHandler(this.preset_Click);
            // 
            // load_preset
            // 
            this.load_preset.Location = new System.Drawing.Point(4, 314);
            this.load_preset.Margin = new System.Windows.Forms.Padding(4);
            this.load_preset.Name = "load_preset";
            this.load_preset.Size = new System.Drawing.Size(156, 28);
            this.load_preset.TabIndex = 6;
            this.load_preset.Text = "Vorlage Laden";
            this.load_preset.UseVisualStyleBackColor = true;
            this.load_preset.Click += new System.EventHandler(this.load_preset_Click);
            // 
            // bg
            // 
            this.bg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bg.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.bg.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bg.BackgroundImage")));
            this.bg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.bg.Controls.Add(this.number);
            this.bg.Cursor = System.Windows.Forms.Cursors.Default;
            this.bg.Location = new System.Drawing.Point(211, 4);
            this.bg.Margin = new System.Windows.Forms.Padding(4);
            this.bg.Name = "bg";
            this.bg.Size = new System.Drawing.Size(718, 732);
            this.bg.TabIndex = 2;
            this.bg.Click += new System.EventHandler(this.bg_Click);
            // 
            // number
            // 
            this.number.AutoSize = true;
            this.number.Font = new System.Drawing.Font("Lucida Sans Typewriter", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.number.ForeColor = System.Drawing.Color.Red;
            this.number.Location = new System.Drawing.Point(635, 705);
            this.number.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.number.Name = "number";
            this.number.Size = new System.Drawing.Size(69, 19);
            this.number.TabIndex = 0;
            this.number.Text = "0 / 32";
            // 
            // rm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 738);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "rm";
            this.Text = "Regal Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.rm_FormClosing);
            this.Load += new System.EventHandler(this.rm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.bg.ResumeLayout(false);
            this.bg.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button add;
        private System.Windows.Forms.Button delete;
        private System.Windows.Forms.TextBox nameText;
        private System.Windows.Forms.Button refresh;
        private System.Windows.Forms.Panel bg;
        private System.Windows.Forms.Button preset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label number;
        private System.Windows.Forms.Button load_preset;
    }
}