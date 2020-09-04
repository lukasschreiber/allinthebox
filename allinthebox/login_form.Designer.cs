namespace allinthebox
{
    partial class login_form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(login_form));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.password = new Bunifu.Framework.UI.BunifuMaterialTextbox();
            this.userName = new Bunifu.Framework.UI.BunifuMaterialTextbox();
            this.exit = new Bunifu.Framework.UI.BunifuThinButton2();
            this.open = new Bunifu.Framework.UI.BunifuThinButton2();
            this.label3 = new System.Windows.Forms.Label();
            this.errorText = new System.Windows.Forms.Label();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.closeButton = new System.Windows.Forms.PictureBox();
            this.VersionLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            this.SuspendLayout();
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
            // pictureBox1
            // 
            this.pictureBox1.Image = global::allinthebox.Properties.Resources.Logo_Square;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // password
            // 
            resources.ApplyResources(this.password, "password");
            this.password.BackColor = System.Drawing.Color.White;
            this.password.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.password.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.password.HintForeColor = System.Drawing.Color.Empty;
            this.password.HintText = "";
            this.password.isPassword = true;
            this.password.LineFocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.password.LineIdleColor = System.Drawing.Color.DarkGray;
            this.password.LineMouseHoverColor = System.Drawing.Color.Transparent;
            this.password.LineThickness = 3;
            this.password.Name = "password";
            this.password.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.password.OnValueChanged += new System.EventHandler(this.password_OnValueChanged);
            this.password.KeyUp += new System.Windows.Forms.KeyEventHandler(this.password_KeyUp);
            // 
            // userName
            // 
            this.userName.BackColor = System.Drawing.Color.White;
            this.userName.Cursor = System.Windows.Forms.Cursors.IBeam;
            resources.ApplyResources(this.userName, "userName");
            this.userName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.userName.HintForeColor = System.Drawing.Color.Empty;
            this.userName.HintText = "";
            this.userName.isPassword = false;
            this.userName.LineFocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.userName.LineIdleColor = System.Drawing.Color.DarkGray;
            this.userName.LineMouseHoverColor = System.Drawing.Color.Transparent;
            this.userName.LineThickness = 3;
            this.userName.Name = "userName";
            this.userName.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.userName.OnValueChanged += new System.EventHandler(this.userName_OnValueChanged);
            this.userName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.userName_KeyUp);
            // 
            // exit
            // 
            this.exit.ActiveBorderThickness = 1;
            this.exit.ActiveCornerRadius = 1;
            this.exit.ActiveFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.exit.ActiveForecolor = System.Drawing.Color.White;
            this.exit.ActiveLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.exit.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.exit, "exit");
            this.exit.ButtonText = "Verlassen";
            this.exit.Cursor = System.Windows.Forms.Cursors.Default;
            this.exit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.exit.IdleBorderThickness = 1;
            this.exit.IdleCornerRadius = 1;
            this.exit.IdleFillColor = System.Drawing.Color.White;
            this.exit.IdleForecolor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.exit.IdleLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.exit.Name = "exit";
            this.exit.TabStop = false;
            this.exit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // open
            // 
            this.open.ActiveBorderThickness = 1;
            this.open.ActiveCornerRadius = 1;
            this.open.ActiveFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.open.ActiveForecolor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.open.ActiveLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.open.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.open, "open");
            this.open.ButtonText = "Einloggen";
            this.open.Cursor = System.Windows.Forms.Cursors.Default;
            this.open.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.open.IdleBorderThickness = 1;
            this.open.IdleCornerRadius = 1;
            this.open.IdleFillColor = System.Drawing.Color.White;
            this.open.IdleForecolor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.open.IdleLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(170)))));
            this.open.Name = "open";
            this.open.TabStop = false;
            this.open.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.open.Click += new System.EventHandler(this.open_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // errorText
            // 
            resources.ApplyResources(this.errorText, "errorText");
            this.errorText.ForeColor = System.Drawing.Color.Red;
            this.errorText.Name = "errorText";
            // 
            // bunifuImageButton1
            // 
            this.bunifuImageButton1.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.bunifuImageButton1, "bunifuImageButton1");
            this.bunifuImageButton1.ImageActive = null;
            this.bunifuImageButton1.Name = "bunifuImageButton1";
            this.bunifuImageButton1.TabStop = false;
            this.bunifuImageButton1.Zoom = 10;
            this.bunifuImageButton1.MouseEnter += new System.EventHandler(this.bunifuImageButton1_MouseEnter);
            this.bunifuImageButton1.MouseLeave += new System.EventHandler(this.bunifuImageButton1_MouseLeave);
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
            // VersionLabel
            // 
            resources.ApplyResources(this.VersionLabel, "VersionLabel");
            this.VersionLabel.ForeColor = System.Drawing.Color.Silver;
            this.VersionLabel.Name = "VersionLabel";
            // 
            // login_form
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.bunifuImageButton1);
            this.Controls.Add(this.errorText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.open);
            this.Controls.Add(this.exit);
            this.Controls.Add(this.userName);
            this.Controls.Add(this.password);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "login_form";
            this.Load += new System.EventHandler(this.login_form_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.login_form_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.login_form_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.login_form_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.login_form_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Bunifu.Framework.UI.BunifuMaterialTextbox password;
        private Bunifu.Framework.UI.BunifuMaterialTextbox userName;
        private Bunifu.Framework.UI.BunifuThinButton2 exit;
        private Bunifu.Framework.UI.BunifuThinButton2 open;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label errorText;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
        private System.Windows.Forms.PictureBox closeButton;
        private System.Windows.Forms.Label VersionLabel;
    }
}

