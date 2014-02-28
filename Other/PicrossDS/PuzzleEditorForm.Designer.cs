namespace HyoutaTools.Other.PicrossDS {
	partial class PuzzleEditorForm {
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
			this.comboBoxPuzzleSlot = new System.Windows.Forms.ComboBox();
			this.buttonExport = new System.Windows.Forms.Button();
			this.buttonImport = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.comboBoxPackLetter = new System.Windows.Forms.ComboBox();
			this.comboBoxPackNumber = new System.Windows.Forms.ComboBox();
			this.textBoxPack = new System.Windows.Forms.TextBox();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.checkBoxFreeMode = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxCleartime = new System.Windows.Forms.TextBox();
			this.checkBoxCleared = new System.Windows.Forms.CheckBox();
			this.comboBoxPuzzleDimensions = new System.Windows.Forms.ComboBox();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonSaveAs = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBoxPuzzleSlot
			// 
			this.comboBoxPuzzleSlot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxPuzzleSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPuzzleSlot.FormattingEnabled = true;
			this.comboBoxPuzzleSlot.Location = new System.Drawing.Point(13, 13);
			this.comboBoxPuzzleSlot.Name = "comboBoxPuzzleSlot";
			this.comboBoxPuzzleSlot.Size = new System.Drawing.Size(331, 21);
			this.comboBoxPuzzleSlot.TabIndex = 0;
			this.comboBoxPuzzleSlot.SelectedIndexChanged += new System.EventHandler(this.comboBoxPuzzleSlot_SelectedIndexChanged);
			// 
			// buttonExport
			// 
			this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonExport.Location = new System.Drawing.Point(350, 12);
			this.buttonExport.Name = "buttonExport";
			this.buttonExport.Size = new System.Drawing.Size(75, 23);
			this.buttonExport.TabIndex = 1;
			this.buttonExport.Text = "Export";
			this.buttonExport.UseVisualStyleBackColor = true;
			this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
			// 
			// buttonImport
			// 
			this.buttonImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonImport.Location = new System.Drawing.Point(431, 12);
			this.buttonImport.Name = "buttonImport";
			this.buttonImport.Size = new System.Drawing.Size(75, 23);
			this.buttonImport.TabIndex = 2;
			this.buttonImport.Text = "Import";
			this.buttonImport.UseVisualStyleBackColor = true;
			this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.comboBoxPackLetter);
			this.panel1.Controls.Add(this.comboBoxPackNumber);
			this.panel1.Controls.Add(this.textBoxPack);
			this.panel1.Controls.Add(this.textBoxName);
			this.panel1.Controls.Add(this.checkBoxFreeMode);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.textBoxCleartime);
			this.panel1.Controls.Add(this.checkBoxCleared);
			this.panel1.Controls.Add(this.comboBoxPuzzleDimensions);
			this.panel1.Location = new System.Drawing.Point(13, 41);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(493, 242);
			this.panel1.TabIndex = 3;
			// 
			// comboBoxPackLetter
			// 
			this.comboBoxPackLetter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxPackLetter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPackLetter.FormattingEnabled = true;
			this.comboBoxPackLetter.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "AA"});
			this.comboBoxPackLetter.Location = new System.Drawing.Point(456, 27);
			this.comboBoxPackLetter.Name = "comboBoxPackLetter";
			this.comboBoxPackLetter.Size = new System.Drawing.Size(34, 21);
			this.comboBoxPackLetter.TabIndex = 8;
			this.comboBoxPackLetter.SelectedIndexChanged += new System.EventHandler(this.WriteGuiPuzzleDataToSave);
			// 
			// comboBoxPackNumber
			// 
			this.comboBoxPackNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxPackNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPackNumber.FormattingEnabled = true;
			this.comboBoxPackNumber.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25"});
			this.comboBoxPackNumber.Location = new System.Drawing.Point(418, 27);
			this.comboBoxPackNumber.Name = "comboBoxPackNumber";
			this.comboBoxPackNumber.Size = new System.Drawing.Size(34, 21);
			this.comboBoxPackNumber.TabIndex = 7;
			this.comboBoxPackNumber.SelectedIndexChanged += new System.EventHandler(this.WriteGuiPuzzleDataToSave);
			// 
			// textBoxPack
			// 
			this.textBoxPack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxPack.Location = new System.Drawing.Point(249, 28);
			this.textBoxPack.MaxLength = 25;
			this.textBoxPack.Name = "textBoxPack";
			this.textBoxPack.Size = new System.Drawing.Size(163, 20);
			this.textBoxPack.TabIndex = 6;
			this.textBoxPack.TextChanged += new System.EventHandler(this.WriteGuiPuzzleDataToSave);
			// 
			// textBoxName
			// 
			this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxName.Location = new System.Drawing.Point(249, 6);
			this.textBoxName.MaxLength = 25;
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(241, 20);
			this.textBoxName.TabIndex = 5;
			this.textBoxName.TextChanged += new System.EventHandler(this.WriteGuiPuzzleDataToSave);
			// 
			// checkBoxFreeMode
			// 
			this.checkBoxFreeMode.AutoSize = true;
			this.checkBoxFreeMode.Location = new System.Drawing.Point(132, 6);
			this.checkBoxFreeMode.Name = "checkBoxFreeMode";
			this.checkBoxFreeMode.Size = new System.Drawing.Size(77, 17);
			this.checkBoxFreeMode.TabIndex = 4;
			this.checkBoxFreeMode.Text = "Free Mode";
			this.checkBoxFreeMode.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(179, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "frames";
			// 
			// textBoxCleartime
			// 
			this.textBoxCleartime.Location = new System.Drawing.Point(72, 29);
			this.textBoxCleartime.Name = "textBoxCleartime";
			this.textBoxCleartime.Size = new System.Drawing.Size(100, 20);
			this.textBoxCleartime.TabIndex = 2;
			this.textBoxCleartime.TextChanged += new System.EventHandler(this.WriteGuiPuzzleDataToSave);
			// 
			// checkBoxCleared
			// 
			this.checkBoxCleared.AutoSize = true;
			this.checkBoxCleared.Location = new System.Drawing.Point(4, 31);
			this.checkBoxCleared.Name = "checkBoxCleared";
			this.checkBoxCleared.Size = new System.Drawing.Size(62, 17);
			this.checkBoxCleared.TabIndex = 1;
			this.checkBoxCleared.Text = "Cleared";
			this.checkBoxCleared.UseVisualStyleBackColor = true;
			// 
			// comboBoxPuzzleDimensions
			// 
			this.comboBoxPuzzleDimensions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPuzzleDimensions.FormattingEnabled = true;
			this.comboBoxPuzzleDimensions.Items.AddRange(new object[] {
            "5 x 5",
            "10 x 10",
            "15 x 15",
            "20 x 20",
            "25 x 20"});
			this.comboBoxPuzzleDimensions.Location = new System.Drawing.Point(4, 4);
			this.comboBoxPuzzleDimensions.Name = "comboBoxPuzzleDimensions";
			this.comboBoxPuzzleDimensions.Size = new System.Drawing.Size(121, 21);
			this.comboBoxPuzzleDimensions.TabIndex = 0;
			this.comboBoxPuzzleDimensions.SelectedIndexChanged += new System.EventHandler(this.WriteGuiPuzzleDataToSave);
			// 
			// buttonSave
			// 
			this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSave.Location = new System.Drawing.Point(431, 290);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(75, 23);
			this.buttonSave.TabIndex = 4;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// buttonSaveAs
			// 
			this.buttonSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSaveAs.Location = new System.Drawing.Point(350, 290);
			this.buttonSaveAs.Name = "buttonSaveAs";
			this.buttonSaveAs.Size = new System.Drawing.Size(75, 23);
			this.buttonSaveAs.TabIndex = 5;
			this.buttonSaveAs.Text = "Save As...";
			this.buttonSaveAs.UseVisualStyleBackColor = true;
			this.buttonSaveAs.Click += new System.EventHandler(this.buttonSaveAs_Click);
			// 
			// PuzzleEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(518, 325);
			this.Controls.Add(this.buttonSaveAs);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.buttonImport);
			this.Controls.Add(this.buttonExport);
			this.Controls.Add(this.comboBoxPuzzleSlot);
			this.Name = "PuzzleEditorForm";
			this.Text = "PuzzleEditorForm";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxPuzzleSlot;
		private System.Windows.Forms.Button buttonExport;
		private System.Windows.Forms.Button buttonImport;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ComboBox comboBoxPuzzleDimensions;
		private System.Windows.Forms.CheckBox checkBoxCleared;
		private System.Windows.Forms.TextBox textBoxCleartime;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBoxFreeMode;
		private System.Windows.Forms.TextBox textBoxPack;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.Button buttonSaveAs;
		private System.Windows.Forms.ComboBox comboBoxPackLetter;
		private System.Windows.Forms.ComboBox comboBoxPackNumber;
	}
}