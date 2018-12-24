namespace HyoutaToolsGUI {
	partial class ToolSelectionForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing ) {
			if ( disposing && ( components != null ) ) {
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.listBoxTools = new System.Windows.Forms.ListBox();
			this.buttonRun = new System.Windows.Forms.Button();
			this.textBoxArgs = new System.Windows.Forms.TextBox();
			this.labelArgs = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// listBoxTools
			// 
			this.listBoxTools.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxTools.FormattingEnabled = true;
			this.listBoxTools.Location = new System.Drawing.Point(13, 13);
			this.listBoxTools.Name = "listBoxTools";
			this.listBoxTools.Size = new System.Drawing.Size(775, 394);
			this.listBoxTools.TabIndex = 0;
			// 
			// buttonRun
			// 
			this.buttonRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRun.Location = new System.Drawing.Point(713, 415);
			this.buttonRun.Name = "buttonRun";
			this.buttonRun.Size = new System.Drawing.Size(75, 23);
			this.buttonRun.TabIndex = 1;
			this.buttonRun.Text = "Run";
			this.buttonRun.UseVisualStyleBackColor = true;
			this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
			// 
			// textBoxArgs
			// 
			this.textBoxArgs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxArgs.Location = new System.Drawing.Point(75, 417);
			this.textBoxArgs.Name = "textBoxArgs";
			this.textBoxArgs.Size = new System.Drawing.Size(632, 20);
			this.textBoxArgs.TabIndex = 2;
			// 
			// labelArgs
			// 
			this.labelArgs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelArgs.AutoSize = true;
			this.labelArgs.Location = new System.Drawing.Point(12, 420);
			this.labelArgs.Name = "labelArgs";
			this.labelArgs.Size = new System.Drawing.Size(57, 13);
			this.labelArgs.TabIndex = 3;
			this.labelArgs.Text = "Arguments";
			// 
			// ToolSelectionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.labelArgs);
			this.Controls.Add(this.textBoxArgs);
			this.Controls.Add(this.buttonRun);
			this.Controls.Add(this.listBoxTools);
			this.Name = "ToolSelectionForm";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBoxTools;
		private System.Windows.Forms.Button buttonRun;
		private System.Windows.Forms.TextBox textBoxArgs;
		private System.Windows.Forms.Label labelArgs;
	}
}

