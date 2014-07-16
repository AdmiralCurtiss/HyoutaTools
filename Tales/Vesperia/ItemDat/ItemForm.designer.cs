namespace HyoutaTools.Tales.Vesperia.ItemDat
{
    partial class ItemForm
    {
        /// <summary>
        /// Required designer variable.
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.labelName = new System.Windows.Forms.Label();
			this.labelDescription = new System.Windows.Forms.Label();
			this.labelUnknown = new System.Windows.Forms.Label();
			this.textBoxGeneratedText = new System.Windows.Forms.TextBox();
			this.buttonGenerateText = new System.Windows.Forms.Button();
			this.buttonGenerateHtml = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.AutoScroll = true;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(161, 235);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(989, 390);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(13, 13);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(142, 602);
			this.listBox1.TabIndex = 1;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// labelName
			// 
			this.labelName.AutoSize = true;
			this.labelName.Location = new System.Drawing.Point(162, 13);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(35, 13);
			this.labelName.TabIndex = 2;
			this.labelName.Text = "Name";
			// 
			// labelDescription
			// 
			this.labelDescription.AutoSize = true;
			this.labelDescription.Location = new System.Drawing.Point(162, 26);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(60, 13);
			this.labelDescription.TabIndex = 3;
			this.labelDescription.Text = "Description";
			// 
			// labelUnknown
			// 
			this.labelUnknown.AutoSize = true;
			this.labelUnknown.Location = new System.Drawing.Point(165, 55);
			this.labelUnknown.Name = "labelUnknown";
			this.labelUnknown.Size = new System.Drawing.Size(0, 13);
			this.labelUnknown.TabIndex = 4;
			// 
			// textBoxGeneratedText
			// 
			this.textBoxGeneratedText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxGeneratedText.Location = new System.Drawing.Point(413, 13);
			this.textBoxGeneratedText.Multiline = true;
			this.textBoxGeneratedText.Name = "textBoxGeneratedText";
			this.textBoxGeneratedText.Size = new System.Drawing.Size(737, 216);
			this.textBoxGeneratedText.TabIndex = 5;
			// 
			// buttonGenerateText
			// 
			this.buttonGenerateText.Location = new System.Drawing.Point(161, 177);
			this.buttonGenerateText.Name = "buttonGenerateText";
			this.buttonGenerateText.Size = new System.Drawing.Size(246, 23);
			this.buttonGenerateText.TabIndex = 6;
			this.buttonGenerateText.Text = "Generate Text for all items to Clipboard";
			this.buttonGenerateText.UseVisualStyleBackColor = true;
			this.buttonGenerateText.Click += new System.EventHandler(this.buttonGenerateText_Click);
			// 
			// buttonGenerateHtml
			// 
			this.buttonGenerateHtml.Location = new System.Drawing.Point(161, 206);
			this.buttonGenerateHtml.Name = "buttonGenerateHtml";
			this.buttonGenerateHtml.Size = new System.Drawing.Size(246, 23);
			this.buttonGenerateHtml.TabIndex = 7;
			this.buttonGenerateHtml.Text = "Generate HTML for all items to Clipboard";
			this.buttonGenerateHtml.UseVisualStyleBackColor = true;
			this.buttonGenerateHtml.Click += new System.EventHandler(this.buttonGenerateHtml_Click);
			// 
			// ItemForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1162, 637);
			this.Controls.Add(this.buttonGenerateHtml);
			this.Controls.Add(this.buttonGenerateText);
			this.Controls.Add(this.textBoxGeneratedText);
			this.Controls.Add(this.labelUnknown);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "ItemForm";
			this.Text = "ItemForm";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label labelUnknown;
		private System.Windows.Forms.TextBox textBoxGeneratedText;
		private System.Windows.Forms.Button buttonGenerateText;
		private System.Windows.Forms.Button buttonGenerateHtml;

    }
}