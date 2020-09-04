namespace allinthebox
{
    partial class add_user
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(add_user));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.user_box = new Design.CustomTextBox();
            this.pass_box = new Design.CustomTextBox();
            this.add = new Bunifu.Framework.UI.BunifuFlatButton();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Controls.Add(this.user_box);
            this.flowLayoutPanel1.Controls.Add(this.pass_box);
            this.flowLayoutPanel1.Controls.Add(this.add);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bg_MouseDown);
            this.flowLayoutPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bg_MouseMove);
            this.flowLayoutPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bg_MouseUp);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Controls.Add(this.closeButton);
            this.panel1.Controls.Add(this.label4);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bg_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bg_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bg_MouseUp);
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
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Name = "label4";
            // 
            // user_box
            // 
            this.user_box.BackColor = System.Drawing.Color.White;
            this.user_box.BorderColor = System.Drawing.Color.Silver;
            this.user_box.BorderThickness = 3;
            this.user_box.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.user_box.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.user_box.FocusResizeConst = 0;
            resources.ApplyResources(this.user_box, "user_box");
            this.user_box.HoverColor = System.Drawing.Color.Gray;
            this.user_box.Name = "user_box";
            this.user_box.ResizeConst = 0;
            // 
            // pass_box
            // 
            this.pass_box.BackColor = System.Drawing.Color.White;
            this.pass_box.BorderColor = System.Drawing.Color.Silver;
            this.pass_box.BorderThickness = 3;
            this.pass_box.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.pass_box.FocusBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.pass_box.FocusResizeConst = 0;
            resources.ApplyResources(this.pass_box, "pass_box");
            this.pass_box.HoverColor = System.Drawing.Color.Gray;
            this.pass_box.Name = "pass_box";
            this.pass_box.ResizeConst = 0;
            // 
            // add
            // 
            this.add.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(175)))), ((int)(((byte)(175)))), ((int)(((byte)(175)))));
            this.add.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            resources.ApplyResources(this.add, "add");
            this.add.BorderRadius = 0;
            this.add.ButtonText = "Hinzufügen";
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
            this.add.Name = "add";
            this.add.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.add.OnHovercolor = System.Drawing.Color.Silver;
            this.add.OnHoverTextColor = System.Drawing.Color.Black;
            this.add.selected = false;
            this.add.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.add.Textcolor = System.Drawing.Color.Black;
            this.add.TextFont = new System.Drawing.Font("Century Gothic", 9.75F);
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // add_user
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "add_user";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.add_user_FormClosing);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox closeButton;
        private Design.CustomTextBox user_box;
        private Design.CustomTextBox pass_box;
        private Bunifu.Framework.UI.BunifuFlatButton add;
    }
}