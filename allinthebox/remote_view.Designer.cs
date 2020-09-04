namespace allinthebox
{
    partial class remote_view
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(remote_view));
            this.panel1 = new System.Windows.Forms.Panel();
            this.world = new System.Windows.Forms.PictureBox();
            this.refresh = new System.Windows.Forms.PictureBox();
            this.closeButton = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.connection_label = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.world)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.refresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Controls.Add(this.world);
            this.panel1.Controls.Add(this.refresh);
            this.panel1.Controls.Add(this.closeButton);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(365, 40);
            this.panel1.TabIndex = 39;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bg_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.bg_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bg_MouseUp);
            // 
            // world
            // 
            this.world.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.world.BackColor = System.Drawing.Color.Transparent;
            this.world.Image = global::allinthebox.Properties.Resources.lock_black;
            this.world.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.world.Location = new System.Drawing.Point(276, 5);
            this.world.Margin = new System.Windows.Forms.Padding(175, 5, 5, 0);
            this.world.Name = "world";
            this.world.Padding = new System.Windows.Forms.Padding(3, 2, 3, 4);
            this.world.Size = new System.Drawing.Size(25, 25);
            this.world.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.world.TabIndex = 39;
            this.world.TabStop = false;
            this.world.Tag = "";
            this.world.Click += new System.EventHandler(this.World_Click);
            this.world.MouseEnter += new System.EventHandler(this.World_MouseEnter);
            this.world.MouseLeave += new System.EventHandler(this.World_MouseLeave);
            // 
            // refresh
            // 
            this.refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refresh.BackColor = System.Drawing.Color.Transparent;
            this.refresh.Image = global::allinthebox.Properties.Resources.i_black;
            this.refresh.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.refresh.Location = new System.Drawing.Point(305, 5);
            this.refresh.Margin = new System.Windows.Forms.Padding(175, 5, 5, 0);
            this.refresh.Name = "refresh";
            this.refresh.Padding = new System.Windows.Forms.Padding(5);
            this.refresh.Size = new System.Drawing.Size(25, 25);
            this.refresh.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.refresh.TabIndex = 38;
            this.refresh.TabStop = false;
            this.refresh.Tag = "";
            this.refresh.Click += new System.EventHandler(this.Refresh_Click);
            this.refresh.MouseEnter += new System.EventHandler(this.Refresh_MouseEnter);
            this.refresh.MouseLeave += new System.EventHandler(this.Refresh_MouseLeave);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.Color.Transparent;
            this.closeButton.Image = global::allinthebox.Properties.Resources.close;
            this.closeButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.closeButton.Location = new System.Drawing.Point(335, 5);
            this.closeButton.Margin = new System.Windows.Forms.Padding(175, 5, 5, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Padding = new System.Windows.Forms.Padding(5);
            this.closeButton.Size = new System.Drawing.Size(25, 25);
            this.closeButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.closeButton.TabIndex = 37;
            this.closeButton.TabStop = false;
            this.closeButton.Tag = "";
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            this.closeButton.MouseEnter += new System.EventHandler(this.closeButton_MouseEnter);
            this.closeButton.MouseLeave += new System.EventHandler(this.closeButton_MouseLeave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Bold);
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(10, 0, 3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 19);
            this.label2.TabIndex = 6;
            this.label2.Text = "Remote";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Century Gothic", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 386);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(348, 57);
            this.label1.TabIndex = 40;
            this.label1.Text = "Um die Fernsteuerung zu verbinden einfach mit der App den QR Code scannen.";
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(31, 64);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(300, 300);
            this.panel2.TabIndex = 41;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel2_Paint);
            // 
            // connection_label
            // 
            this.connection_label.AutoSize = true;
            this.connection_label.Font = new System.Drawing.Font("Century Gothic", 7.8F);
            this.connection_label.Location = new System.Drawing.Point(4, 431);
            this.connection_label.Name = "connection_label";
            this.connection_label.Size = new System.Drawing.Size(116, 19);
            this.connection_label.TabIndex = 42;
            this.connection_label.Text = "No connection.";
            // 
            // remote_view
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 472);
            this.Controls.Add(this.connection_label);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "remote_view";
            this.Text = "Remote";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.world)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.refresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox closeButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox refresh;
        public System.Windows.Forms.Label connection_label;
        private System.Windows.Forms.PictureBox world;
    }
}