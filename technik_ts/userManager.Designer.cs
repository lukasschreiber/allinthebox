namespace allinthebox
{
    partial class userManager
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(userManager));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.closeButton = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.user_list = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.user_box = new Design.CustomTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pass_box = new Design.CustomTextBox();
            this.pid_out = new Design.SimpleComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.adminCheck = new Design.CustomRadioButton();
            this.userCheck = new Design.CustomRadioButton();
            this.save = new Bunifu.Framework.UI.BunifuFlatButton();
            this.del = new Bunifu.Framework.UI.BunifuFlatButton();
            this.separator1 = new Design.Separator();
            this.add = new Bunifu.Framework.UI.BunifuFlatButton();
            this.refresh = new Bunifu.Framework.UI.BunifuFlatButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.closeButton, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.user_list, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.82801F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.17199F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(539, 407);
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
            this.closeButton.Location = new System.Drawing.Point(509, 5);
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
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(10, 10, 3, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 19);
            this.label3.TabIndex = 7;
            this.label3.Text = "Nutzermanager";
            // 
            // user_list
            // 
            this.user_list.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.user_list.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.user_list.Font = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.user_list.FormattingEnabled = true;
            this.user_list.ItemHeight = 17;
            this.user_list.Location = new System.Drawing.Point(273, 43);
            this.user_list.Margin = new System.Windows.Forms.Padding(4);
            this.user_list.Name = "user_list";
            this.user_list.Size = new System.Drawing.Size(260, 357);
            this.user_list.TabIndex = 0;
            this.user_list.SelectedIndexChanged += new System.EventHandler(this.user_list_SelectedIndexChanged);
            this.user_list.Leave += new System.EventHandler(this.user_list_Leave);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.user_box);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.pass_box);
            this.flowLayoutPanel1.Controls.Add(this.pid_out);
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Controls.Add(this.save);
            this.flowLayoutPanel1.Controls.Add(this.del);
            this.flowLayoutPanel1.Controls.Add(this.separator1);
            this.flowLayoutPanel1.Controls.Add(this.add);
            this.flowLayoutPanel1.Controls.Add(this.refresh);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 43);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(261, 360);
            this.flowLayoutPanel1.TabIndex = 1;
            this.flowLayoutPanel1.Click += new System.EventHandler(this.flowLayoutPanel1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nutzername:";
            // 
            // user_box
            // 
            this.user_box.BackColor = System.Drawing.Color.White;
            this.user_box.BorderColor = System.Drawing.Color.Silver;
            this.user_box.BorderThickness = 3;
            this.user_box.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.user_box.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.user_box.FocusResizeConst = 0;
            this.user_box.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.user_box.HoverColor = System.Drawing.Color.Gray;
            this.user_box.Location = new System.Drawing.Point(4, 27);
            this.user_box.Margin = new System.Windows.Forms.Padding(4, 8, 4, 5);
            this.user_box.MaximumSize = new System.Drawing.Size(286, 30);
            this.user_box.Name = "user_box";
            this.user_box.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.user_box.ResizeConst = 0;
            this.user_box.Size = new System.Drawing.Size(210, 28);
            this.user_box.TabIndex = 40;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 60);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "Passwort:";
            // 
            // pass_box
            // 
            this.pass_box.BackColor = System.Drawing.Color.White;
            this.pass_box.BorderColor = System.Drawing.Color.Silver;
            this.pass_box.BorderThickness = 3;
            this.pass_box.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.pass_box.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.pass_box.FocusResizeConst = 0;
            this.pass_box.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pass_box.HoverColor = System.Drawing.Color.Gray;
            this.pass_box.Location = new System.Drawing.Point(4, 87);
            this.pass_box.Margin = new System.Windows.Forms.Padding(4, 8, 4, 5);
            this.pass_box.MaximumSize = new System.Drawing.Size(286, 30);
            this.pass_box.Name = "pass_box";
            this.pass_box.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.pass_box.ResizeConst = 0;
            this.pass_box.Size = new System.Drawing.Size(210, 28);
            this.pass_box.TabIndex = 41;
            // 
            // pid_out
            // 
            this.pid_out.AnchorSize = new System.Drawing.Size(210, 30);
            this.pid_out.BackColor = System.Drawing.Color.Silver;
            this.pid_out.DockSide = Design.DropDownControl.eDockSide.Right;
            this.pid_out.DropDownHeight = 50;
            this.pid_out.DropDownWidth = 230;
            this.pid_out.Font = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pid_out.Location = new System.Drawing.Point(4, 124);
            this.pid_out.Margin = new System.Windows.Forms.Padding(4);
            this.pid_out.Name = "pid_out";
            this.pid_out.Size = new System.Drawing.Size(210, 30);
            this.pid_out.TabIndex = 42;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.adminCheck);
            this.panel1.Controls.Add(this.userCheck);
            this.panel1.Location = new System.Drawing.Point(3, 161);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 22);
            this.panel1.TabIndex = 50;
            // 
            // adminCheck
            // 
            this.adminCheck.AutoSize = true;
            this.adminCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.adminCheck.HoverRadioColor = System.Drawing.Color.Silver;
            this.adminCheck.Location = new System.Drawing.Point(4, 3);
            this.adminCheck.Name = "adminCheck";
            this.adminCheck.RadioColor = System.Drawing.Color.Silver;
            this.adminCheck.Size = new System.Drawing.Size(68, 21);
            this.adminCheck.TabIndex = 49;
            this.adminCheck.TabStop = true;
            this.adminCheck.Text = "Admin";
            this.adminCheck.TextFont = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.adminCheck.UseVisualStyleBackColor = true;
            // 
            // userCheck
            // 
            this.userCheck.AutoSize = true;
            this.userCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userCheck.HoverRadioColor = System.Drawing.Color.Silver;
            this.userCheck.Location = new System.Drawing.Point(78, 3);
            this.userCheck.Name = "userCheck";
            this.userCheck.RadioColor = System.Drawing.Color.Silver;
            this.userCheck.Size = new System.Drawing.Size(71, 21);
            this.userCheck.TabIndex = 44;
            this.userCheck.TabStop = true;
            this.userCheck.Text = "Nutzer";
            this.userCheck.TextFont = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userCheck.UseVisualStyleBackColor = true;
            // 
            // save
            // 
            this.save.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.save.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.save.BorderRadius = 0;
            this.save.ButtonText = "Speichern";
            this.save.Cursor = System.Windows.Forms.Cursors.Default;
            this.save.DisabledColor = System.Drawing.Color.Gray;
            this.save.Iconcolor = System.Drawing.Color.Transparent;
            this.save.Iconimage = null;
            this.save.Iconimage_right = null;
            this.save.Iconimage_right_Selected = null;
            this.save.Iconimage_Selected = null;
            this.save.IconMarginLeft = 0;
            this.save.IconMarginRight = 0;
            this.save.IconRightVisible = true;
            this.save.IconRightZoom = 0D;
            this.save.IconVisible = true;
            this.save.IconZoom = 90D;
            this.save.IsTab = false;
            this.save.Location = new System.Drawing.Point(4, 190);
            this.save.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.save.Name = "save";
            this.save.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.save.OnHovercolor = System.Drawing.Color.Silver;
            this.save.OnHoverTextColor = System.Drawing.Color.Black;
            this.save.selected = false;
            this.save.Size = new System.Drawing.Size(210, 28);
            this.save.TabIndex = 45;
            this.save.Text = "Speichern";
            this.save.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.save.Textcolor = System.Drawing.Color.Black;
            this.save.TextFont = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // del
            // 
            this.del.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.del.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.del.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.del.BorderRadius = 0;
            this.del.ButtonText = "Löschen";
            this.del.Cursor = System.Windows.Forms.Cursors.Default;
            this.del.DisabledColor = System.Drawing.Color.Gray;
            this.del.Iconcolor = System.Drawing.Color.Transparent;
            this.del.Iconimage = null;
            this.del.Iconimage_right = null;
            this.del.Iconimage_right_Selected = null;
            this.del.Iconimage_Selected = null;
            this.del.IconMarginLeft = 0;
            this.del.IconMarginRight = 0;
            this.del.IconRightVisible = true;
            this.del.IconRightZoom = 0D;
            this.del.IconVisible = true;
            this.del.IconZoom = 90D;
            this.del.IsTab = false;
            this.del.Location = new System.Drawing.Point(4, 226);
            this.del.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.del.Name = "del";
            this.del.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.del.OnHovercolor = System.Drawing.Color.Silver;
            this.del.OnHoverTextColor = System.Drawing.Color.Black;
            this.del.selected = false;
            this.del.Size = new System.Drawing.Size(210, 28);
            this.del.TabIndex = 46;
            this.del.Text = "Löschen";
            this.del.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.del.Textcolor = System.Drawing.Color.Black;
            this.del.TextFont = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.del.Click += new System.EventHandler(this.del_Click);
            // 
            // separator1
            // 
            this.separator1.BackColor = System.Drawing.Color.Transparent;
            this.separator1.Color = System.Drawing.Color.Silver;
            this.separator1.ForeColor = System.Drawing.Color.Silver;
            this.separator1.Location = new System.Drawing.Point(3, 261);
            this.separator1.Name = "separator1";
            this.separator1.Size = new System.Drawing.Size(211, 10);
            this.separator1.StrokeSize = new System.Drawing.Size(110, 1);
            this.separator1.TabIndex = 43;
            // 
            // add
            // 
            this.add.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.add.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.add.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.add.BorderRadius = 0;
            this.add.ButtonText = "Neuer Nutzer";
            this.add.Cursor = System.Windows.Forms.Cursors.Default;
            this.add.DisabledColor = System.Drawing.Color.Gray;
            this.add.Iconcolor = System.Drawing.Color.Transparent;
            this.add.Iconimage = null;
            this.add.Iconimage_right = null;
            this.add.Iconimage_right_Selected = null;
            this.add.Iconimage_Selected = null;
            this.add.IconMarginLeft = 0;
            this.add.IconMarginRight = 0;
            this.add.IconRightVisible = true;
            this.add.IconRightZoom = 0D;
            this.add.IconVisible = true;
            this.add.IconZoom = 90D;
            this.add.IsTab = false;
            this.add.Location = new System.Drawing.Point(4, 278);
            this.add.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.add.Name = "add";
            this.add.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.add.OnHovercolor = System.Drawing.Color.Silver;
            this.add.OnHoverTextColor = System.Drawing.Color.Black;
            this.add.selected = false;
            this.add.Size = new System.Drawing.Size(210, 28);
            this.add.TabIndex = 47;
            this.add.Text = "Neuer Nutzer";
            this.add.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.add.Textcolor = System.Drawing.Color.Black;
            this.add.TextFont = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // refresh
            // 
            this.refresh.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.refresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.refresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.refresh.BorderRadius = 0;
            this.refresh.ButtonText = "Aktualisieren";
            this.refresh.Cursor = System.Windows.Forms.Cursors.Default;
            this.refresh.DisabledColor = System.Drawing.Color.Gray;
            this.refresh.Iconcolor = System.Drawing.Color.Transparent;
            this.refresh.Iconimage = null;
            this.refresh.Iconimage_right = null;
            this.refresh.Iconimage_right_Selected = null;
            this.refresh.Iconimage_Selected = null;
            this.refresh.IconMarginLeft = 0;
            this.refresh.IconMarginRight = 0;
            this.refresh.IconRightVisible = true;
            this.refresh.IconRightZoom = 0D;
            this.refresh.IconVisible = true;
            this.refresh.IconZoom = 90D;
            this.refresh.IsTab = false;
            this.refresh.Location = new System.Drawing.Point(4, 314);
            this.refresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.refresh.Name = "refresh";
            this.refresh.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.refresh.OnHovercolor = System.Drawing.Color.Silver;
            this.refresh.OnHoverTextColor = System.Drawing.Color.Black;
            this.refresh.selected = false;
            this.refresh.Size = new System.Drawing.Size(210, 28);
            this.refresh.TabIndex = 48;
            this.refresh.Text = "Aktualisieren";
            this.refresh.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.refresh.Textcolor = System.Drawing.Color.Black;
            this.refresh.TextFont = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refresh.Click += new System.EventHandler(this.refresh_Click);
            // 
            // userManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 406);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "userManager";
            this.Text = "Usermanager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.userManager_FormClosing);
            this.Load += new System.EventHandler(this.userManager_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox user_list;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox closeButton;
        private Design.CustomTextBox user_box;
        private Design.CustomTextBox pass_box;
        private Design.SimpleComboBox pid_out;
        private Design.Separator separator1;
        private Design.CustomRadioButton userCheck;
        private Bunifu.Framework.UI.BunifuFlatButton save;
        private Bunifu.Framework.UI.BunifuFlatButton del;
        private Bunifu.Framework.UI.BunifuFlatButton add;
        private Bunifu.Framework.UI.BunifuFlatButton refresh;
        private System.Windows.Forms.Panel panel1;
        private Design.CustomRadioButton adminCheck;
    }
}