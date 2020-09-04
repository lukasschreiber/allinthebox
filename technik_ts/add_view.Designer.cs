namespace allinthebox
{
    partial class add_view
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(add_view));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.closeButton = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.barcode = new Design.CustomTextBox();
            this.item_name = new Design.CustomTextBox();
            this.comment = new Design.CustomTextBox();
            this.submit = new Bunifu.Framework.UI.BunifuFlatButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.closeButton, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.barcode, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.item_name, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.comment, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.submit, 1, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.tableLayoutPanel1_CellPaint);
            this.tableLayoutPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bg_MouseDown);
            this.tableLayoutPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bg_MouseMove);
            this.tableLayoutPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bg_MouseUp);
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.BackColor = System.Drawing.Color.Transparent;
            this.closeButton.Image = global::allinthebox.Properties.Resources.close;
            this.closeButton.Name = "closeButton";
            this.closeButton.TabStop = false;
            this.closeButton.Tag = "";
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            this.closeButton.MouseEnter += new System.EventHandler(this.closeButton_MouseEnter);
            this.closeButton.MouseLeave += new System.EventHandler(this.closeButton_MouseLeave);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Name = "label4";
            // 
            // barcode
            // 
            this.barcode.BackColor = System.Drawing.Color.White;
            this.barcode.BorderColor = System.Drawing.Color.Silver;
            this.barcode.BorderThickness = 3;
            this.barcode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.barcode.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.barcode.FocusResizeConst = 0;
            resources.ApplyResources(this.barcode, "barcode");
            this.barcode.HoverColor = System.Drawing.Color.Gray;
            this.barcode.Name = "barcode";
            this.barcode.ResizeConst = 0;
            this.barcode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyUp);
            // 
            // item_name
            // 
            this.item_name.BackColor = System.Drawing.Color.White;
            this.item_name.BorderColor = System.Drawing.Color.Silver;
            this.item_name.BorderThickness = 3;
            this.item_name.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.item_name.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.item_name.FocusResizeConst = 0;
            resources.ApplyResources(this.item_name, "item_name");
            this.item_name.HoverColor = System.Drawing.Color.Gray;
            this.item_name.Name = "item_name";
            this.item_name.ResizeConst = 0;
            this.item_name.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyUp);
            // 
            // comment
            // 
            this.comment.BackColor = System.Drawing.Color.White;
            this.comment.BorderColor = System.Drawing.Color.Silver;
            this.comment.BorderThickness = 3;
            this.comment.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.comment.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.comment.FocusResizeConst = 0;
            resources.ApplyResources(this.comment, "comment");
            this.comment.HoverColor = System.Drawing.Color.Gray;
            this.comment.Name = "comment";
            this.comment.ResizeConst = 0;
            this.comment.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyUp);
            // 
            // submit
            // 
            this.submit.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            resources.ApplyResources(this.submit, "submit");
            this.submit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.submit.BorderRadius = 0;
            this.submit.ButtonText = "Hinzufügen";
            this.submit.Cursor = System.Windows.Forms.Cursors.Default;
            this.submit.DisabledColor = System.Drawing.Color.Gray;
            this.submit.Iconcolor = System.Drawing.Color.Transparent;
            this.submit.Iconimage = null;
            this.submit.Iconimage_right = null;
            this.submit.Iconimage_right_Selected = null;
            this.submit.Iconimage_Selected = null;
            this.submit.IconMarginLeft = 0;
            this.submit.IconMarginRight = 0;
            this.submit.IconRightVisible = true;
            this.submit.IconRightZoom = 0D;
            this.submit.IconVisible = true;
            this.submit.IconZoom = 90D;
            this.submit.IsTab = false;
            this.submit.Name = "submit";
            this.submit.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.submit.OnHovercolor = System.Drawing.Color.Silver;
            this.submit.OnHoverTextColor = System.Drawing.Color.Black;
            this.submit.selected = false;
            this.submit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.submit.Textcolor = System.Drawing.Color.Black;
            this.submit.TextFont = new System.Drawing.Font("Century Gothic", 9.75F);
            this.submit.Click += new System.EventHandler(this.submit_Click);
            // 
            // add_view
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "add_view";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox closeButton;
        private Design.CustomTextBox barcode;
        public Design.CustomTextBox item_name;
        private Design.CustomTextBox comment;
        private Bunifu.Framework.UI.BunifuFlatButton submit;
    }
}