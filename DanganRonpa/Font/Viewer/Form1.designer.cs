namespace HyoutaTools.DanganRonpa.Font.Viewer
{
    partial class Form1
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
			this.TownMapIDBox = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.labelTiles = new System.Windows.Forms.Label();
			this.numericUpDownTileNumber = new System.Windows.Forms.NumericUpDown();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.textBoxDisplayDelay = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.guiCharWidth = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonWriteNewCharLength = new System.Windows.Forms.Button();
			this.comboBoxFilename = new System.Windows.Forms.ComboBox();
			this.checkBoxApproxDialogue = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.guiCharHeight = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.guiCharY = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.guiCharX = new System.Windows.Forms.NumericUpDown();
			this.panel1 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.TownMapIDBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTileNumber)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.guiCharWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.guiCharHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.guiCharY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.guiCharX)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// TownMapIDBox
			// 
			this.TownMapIDBox.Location = new System.Drawing.Point(37, 12);
			this.TownMapIDBox.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
			this.TownMapIDBox.Name = "TownMapIDBox";
			this.TownMapIDBox.Size = new System.Drawing.Size(85, 20);
			this.TownMapIDBox.TabIndex = 0;
			this.TownMapIDBox.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.TownMapIDBox.ValueChanged += new System.EventHandler(this.TownMapIDBox_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(18, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "ID";
			// 
			// labelTiles
			// 
			this.labelTiles.AutoSize = true;
			this.labelTiles.Location = new System.Drawing.Point(265, 14);
			this.labelTiles.Name = "labelTiles";
			this.labelTiles.Size = new System.Drawing.Size(34, 13);
			this.labelTiles.TabIndex = 4;
			this.labelTiles.Text = "Scale";
			// 
			// numericUpDownTileNumber
			// 
			this.numericUpDownTileNumber.Location = new System.Drawing.Point(305, 12);
			this.numericUpDownTileNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDownTileNumber.Name = "numericUpDownTileNumber";
			this.numericUpDownTileNumber.Size = new System.Drawing.Size(34, 20);
			this.numericUpDownTileNumber.TabIndex = 5;
			this.numericUpDownTileNumber.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.numericUpDownTileNumber.ValueChanged += new System.EventHandler(this.TownMapIDBox_ValueChanged);
			// 
			// pictureBox2
			// 
			this.pictureBox2.BackColor = System.Drawing.Color.Silver;
			this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox2.Location = new System.Drawing.Point(0, 0);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(954, 138);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox2.TabIndex = 7;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
			this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
			this.pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
			// 
			// textBoxDisplayDelay
			// 
			this.textBoxDisplayDelay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxDisplayDelay.BackColor = System.Drawing.SystemColors.Window;
			this.textBoxDisplayDelay.Location = new System.Drawing.Point(345, 12);
			this.textBoxDisplayDelay.Multiline = true;
			this.textBoxDisplayDelay.Name = "textBoxDisplayDelay";
			this.textBoxDisplayDelay.Size = new System.Drawing.Size(544, 106);
			this.textBoxDisplayDelay.TabIndex = 18;
			this.textBoxDisplayDelay.TextChanged += new System.EventHandler(this.textBoxDisplayDelay_TextChanged);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(895, 11);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 24;
			this.button1.Text = "Reload";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// guiCharWidth
			// 
			this.guiCharWidth.Location = new System.Drawing.Point(54, 98);
			this.guiCharWidth.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.guiCharWidth.Name = "guiCharWidth";
			this.guiCharWidth.Size = new System.Drawing.Size(49, 20);
			this.guiCharWidth.TabIndex = 25;
			this.guiCharWidth.ValueChanged += new System.EventHandler(this.numUpDownCharLength_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 100);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 26;
			this.label2.Text = "Width";
			// 
			// buttonWriteNewCharLength
			// 
			this.buttonWriteNewCharLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonWriteNewCharLength.Location = new System.Drawing.Point(895, 40);
			this.buttonWriteNewCharLength.Name = "buttonWriteNewCharLength";
			this.buttonWriteNewCharLength.Size = new System.Drawing.Size(75, 37);
			this.buttonWriteNewCharLength.TabIndex = 27;
			this.buttonWriteNewCharLength.Text = "Write New Char Length";
			this.buttonWriteNewCharLength.UseVisualStyleBackColor = true;
			this.buttonWriteNewCharLength.Click += new System.EventHandler(this.buttonWriteNewCharLength_Click);
			// 
			// comboBoxFilename
			// 
			this.comboBoxFilename.FormattingEnabled = true;
			this.comboBoxFilename.Items.AddRange(new object[] {
            "ps3",
            "FONTTEX10.TXV",
            "FONTTEX11.TXV",
            "FONTTEX12.TXV",
            "FONTTEX13.TXV",
            "FONTTEX14.TXV",
            "FONTTEX15.TXV",
            "FONTTEX00.TXV",
            "FONTTEX01.TXV",
            "FONTTEX02.TXV",
            "FONTTEX03.TXV",
            "FONTTEX04.TXV",
            "FONTTEX05.TXV",
            "FONTDUMMY.TXV"});
			this.comboBoxFilename.Location = new System.Drawing.Point(128, 12);
			this.comboBoxFilename.Name = "comboBoxFilename";
			this.comboBoxFilename.Size = new System.Drawing.Size(131, 21);
			this.comboBoxFilename.TabIndex = 28;
			this.comboBoxFilename.Text = "ps3";
			this.comboBoxFilename.SelectedIndexChanged += new System.EventHandler(this.textBoxFilename_TextChanged);
			this.comboBoxFilename.TextUpdate += new System.EventHandler(this.textBoxFilename_TextChanged);
			// 
			// checkBoxApproxDialogue
			// 
			this.checkBoxApproxDialogue.AutoSize = true;
			this.checkBoxApproxDialogue.Location = new System.Drawing.Point(13, 40);
			this.checkBoxApproxDialogue.Name = "checkBoxApproxDialogue";
			this.checkBoxApproxDialogue.Size = new System.Drawing.Size(108, 17);
			this.checkBoxApproxDialogue.TabIndex = 29;
			this.checkBoxApproxDialogue.Text = "Print Set of Chars";
			this.checkBoxApproxDialogue.UseVisualStyleBackColor = true;
			this.checkBoxApproxDialogue.CheckedChanged += new System.EventHandler(this.checkBoxApproxDialogue_CheckedChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(115, 100);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(38, 13);
			this.label3.TabIndex = 31;
			this.label3.Text = "Height";
			// 
			// guiCharHeight
			// 
			this.guiCharHeight.Location = new System.Drawing.Point(156, 98);
			this.guiCharHeight.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.guiCharHeight.Name = "guiCharHeight";
			this.guiCharHeight.Size = new System.Drawing.Size(49, 20);
			this.guiCharHeight.TabIndex = 30;
			this.guiCharHeight.ValueChanged += new System.EventHandler(this.guiCharHeight_ValueChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(115, 76);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(14, 13);
			this.label4.TabIndex = 35;
			this.label4.Text = "Y";
			// 
			// guiCharY
			// 
			this.guiCharY.Location = new System.Drawing.Point(156, 74);
			this.guiCharY.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.guiCharY.Name = "guiCharY";
			this.guiCharY.Size = new System.Drawing.Size(49, 20);
			this.guiCharY.TabIndex = 34;
			this.guiCharY.ValueChanged += new System.EventHandler(this.guiCharY_ValueChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(13, 76);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(14, 13);
			this.label5.TabIndex = 33;
			this.label5.Text = "X";
			// 
			// guiCharX
			// 
			this.guiCharX.Location = new System.Drawing.Point(54, 74);
			this.guiCharX.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.guiCharX.Name = "guiCharX";
			this.guiCharX.Size = new System.Drawing.Size(49, 20);
			this.guiCharX.TabIndex = 32;
			this.guiCharX.ValueChanged += new System.EventHandler(this.guiCharX_ValueChanged);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.pictureBox2);
			this.panel1.Location = new System.Drawing.Point(16, 136);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(954, 138);
			this.panel1.TabIndex = 36;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(982, 286);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.guiCharY);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.guiCharX);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.guiCharHeight);
			this.Controls.Add(this.checkBoxApproxDialogue);
			this.Controls.Add(this.comboBoxFilename);
			this.Controls.Add(this.buttonWriteNewCharLength);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.guiCharWidth);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBoxDisplayDelay);
			this.Controls.Add(this.numericUpDownTileNumber);
			this.Controls.Add(this.labelTiles);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.TownMapIDBox);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.TownMapIDBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTileNumber)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.guiCharWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.guiCharHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.guiCharY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.guiCharX)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown TownMapIDBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTiles;
        private System.Windows.Forms.NumericUpDown numericUpDownTileNumber;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox textBoxDisplayDelay;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown guiCharWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonWriteNewCharLength;
        private System.Windows.Forms.ComboBox comboBoxFilename;
        private System.Windows.Forms.CheckBox checkBoxApproxDialogue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown guiCharHeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown guiCharY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown guiCharX;
		private System.Windows.Forms.Panel panel1;
    }
}

