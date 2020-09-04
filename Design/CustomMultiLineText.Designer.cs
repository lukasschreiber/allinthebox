namespace Design
{
    partial class CustomMultiLineText
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
            this.customScrollbar1 = new Design.CustomScrollbar();
            this.richTextBoxC1 = new Design.RichTextBoxC();
            this.SuspendLayout();
            // 
            // customScrollbar1
            // 
            this.customScrollbar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.customScrollbar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.customScrollbar1.ChannelColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.customScrollbar1.DownArrowImage = global::Design.Resource.downarrowLight;
            this.customScrollbar1.ExtraSmallChange = 1;
            this.customScrollbar1.LargeChange = 10;
            this.customScrollbar1.Location = new System.Drawing.Point(237, 0);
            this.customScrollbar1.Margin = new System.Windows.Forms.Padding(4);
            this.customScrollbar1.Maximum = 100;
            this.customScrollbar1.MaximumSize = new System.Drawing.Size(20, 985);
            this.customScrollbar1.Minimum = 0;
            this.customScrollbar1.MinimumSize = new System.Drawing.Size(15, 69);
            this.customScrollbar1.Name = "customScrollbar1";
            this.customScrollbar1.Size = new System.Drawing.Size(15, 185);
            this.customScrollbar1.SmallChange = 5;
            this.customScrollbar1.TabIndex = 1;
            this.customScrollbar1.ThumbBottomImage = global::Design.Resource.ThumbBottomLight;
            this.customScrollbar1.ThumbBottomSpanImage = global::Design.Resource.ThumbSpanBottomLight;
            this.customScrollbar1.ThumbMiddleImage = global::Design.Resource.ThumbMiddleLight;
            this.customScrollbar1.ThumbTopImage = global::Design.Resource.ThumbTopLight;
            this.customScrollbar1.ThumbTopSpanImage = global::Design.Resource.ThumbSpanTopLight;
            this.customScrollbar1.UpArrowImage = global::Design.Resource.uparrowLight;
            this.customScrollbar1.Value = 0;
            this.customScrollbar1.Scroll += new System.EventHandler(this.customScrollbar1_Scroll);
            // 
            // richTextBoxC1
            // 
            this.richTextBoxC1.AcceptsTab = true;
            this.richTextBoxC1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxC1.Font = new System.Drawing.Font("Century Gothic", 8.25F);
            this.richTextBoxC1.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxC1.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBoxC1.Name = "richTextBoxC1";
            this.richTextBoxC1.parent = null;
            this.richTextBoxC1.ScrollBar = null;
            this.richTextBoxC1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBoxC1.Size = new System.Drawing.Size(229, 185);
            this.richTextBoxC1.TabIndex = 2;
            this.richTextBoxC1.Text = "";
            this.richTextBoxC1.TextChanged += new System.EventHandler(this.richTextBoxC1_TextChanged);
            this.richTextBoxC1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.richTextBoxC1_KeyUp);
            // 
            // CustomMultiLineText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.customScrollbar1);
            this.Controls.Add(this.richTextBoxC1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CustomMultiLineText";
            this.Size = new System.Drawing.Size(257, 185);
            this.ResumeLayout(false);

        }

        #endregion

        private CustomScrollbar customScrollbar1;
        private RichTextBoxC richTextBoxC1;
    }
}
