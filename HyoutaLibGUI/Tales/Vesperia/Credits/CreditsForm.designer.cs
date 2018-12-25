namespace HyoutaLibGUI.Tales.Vesperia.Credits
{
	partial class CreditsForm
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
			this.buttonReload = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.AutoScroll = true;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(297, 68);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(355, 400);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(13, 13);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(278, 446);
			this.listBox1.TabIndex = 1;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// labelName
			// 
			this.labelName.AutoSize = true;
			this.labelName.Location = new System.Drawing.Point(297, 13);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(35, 13);
			this.labelName.TabIndex = 2;
			this.labelName.Text = "Name";
			// 
			// labelDescription
			// 
			this.labelDescription.AutoSize = true;
			this.labelDescription.Location = new System.Drawing.Point(297, 26);
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
			// buttonReload
			// 
			this.buttonReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonReload.Location = new System.Drawing.Point(577, 13);
			this.buttonReload.Name = "buttonReload";
			this.buttonReload.Size = new System.Drawing.Size(75, 23);
			this.buttonReload.TabIndex = 5;
			this.buttonReload.Text = "Reload";
			this.buttonReload.UseVisualStyleBackColor = true;
			this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
			// 
			// CreditsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(664, 480);
			this.Controls.Add(this.buttonReload);
			this.Controls.Add(this.labelUnknown);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "CreditsForm";
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
		private System.Windows.Forms.Button buttonReload;

    }
}