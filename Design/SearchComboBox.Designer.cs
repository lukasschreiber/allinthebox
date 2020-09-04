namespace Design
{
    partial class SimpleComboBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.drop = new System.Windows.Forms.Panel();
            this.customScrollbar1 = new Design.CustomScrollbar();
            this.panel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.drop.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputText
            // 
            this.inputText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inputText.BackColor = System.Drawing.Color.White;
            this.inputText.Font = new System.Drawing.Font("Century Gothic", 8.25F);
            this.inputText.ForeColor = System.Drawing.Color.Black;
            this.inputText.Size = new System.Drawing.Size(142, 18);
            // 
            // drop
            // 
            this.drop.Controls.Add(this.customScrollbar1);
            this.drop.Controls.Add(this.panel1);
            this.drop.Location = new System.Drawing.Point(0, 21);
            this.drop.Name = "drop";
            this.drop.Size = new System.Drawing.Size(167, 129);
            this.drop.TabIndex = 0;
            // 
            // customScrollbar1
            // 
            this.customScrollbar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customScrollbar1.BackColor = System.Drawing.Color.White;
            this.customScrollbar1.ChannelColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.customScrollbar1.DownArrowImage = global::Design.Resource.downarrowLight;
            this.customScrollbar1.ExtraSmallChange = 1;
            this.customScrollbar1.LargeChange = 10;
            this.customScrollbar1.Location = new System.Drawing.Point(152, 0);
            this.customScrollbar1.Maximum = 100;
            this.customScrollbar1.Minimum = 0;
            this.customScrollbar1.MinimumSize = new System.Drawing.Size(15, 56);
            this.customScrollbar1.Name = "customScrollbar1";
            this.customScrollbar1.Size = new System.Drawing.Size(15, 129);
            this.customScrollbar1.SmallChange = 1;
            this.customScrollbar1.TabIndex = 0;
            this.customScrollbar1.ThumbBottomImage = global::Design.Resource.ThumbBottomLight;
            this.customScrollbar1.ThumbBottomSpanImage = global::Design.Resource.ThumbSpanBottomLight;
            this.customScrollbar1.ThumbMiddleImage = global::Design.Resource.ThumbMiddleLight;
            this.customScrollbar1.ThumbTopImage = global::Design.Resource.ThumbTopLight;
            this.customScrollbar1.ThumbTopSpanImage = global::Design.Resource.ThumbSpanTopLight;
            this.customScrollbar1.UpArrowImage = global::Design.Resource.uparrowLight;
            this.customScrollbar1.Value = 0;
            this.customScrollbar1.Scroll += new System.EventHandler(this.customScrollbar1_Scroll);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(257, 129);
            this.panel1.TabIndex = 1;
            this.panel1.WrapContents = false;
            // 
            // SimpleComboBox
            // 
            this.AnchorSize = new System.Drawing.Size(167, 21);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.drop);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SimpleComboBox";
            this.Size = new System.Drawing.Size(167, 150);
            this.Controls.SetChildIndex(this.drop, 0);
            this.Controls.SetChildIndex(this.inputText, 0);
            this.drop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel drop;
        private System.Windows.Forms.FlowLayoutPanel panel1;
        private CustomScrollbar customScrollbar1;
    }
}
