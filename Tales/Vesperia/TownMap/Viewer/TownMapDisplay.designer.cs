namespace HyoutaTools.Tales.Vesperia.TownMap.Viewer
{
    partial class TownMapDisplay
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
			this.textBoxFilename = new System.Windows.Forms.TextBox();
			this.checkBoxEnglish = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelTiles = new System.Windows.Forms.Label();
			this.numericUpDownTileNumber = new System.Windows.Forms.NumericUpDown();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxTileOffLoc = new System.Windows.Forms.TextBox();
			this.textBoxInfoLoc = new System.Windows.Forms.TextBox();
			this.textBoxTileLoc = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textBoxDisplayDelay = new System.Windows.Forms.TextBox();
			this.textBoxTextureOffset = new System.Windows.Forms.TextBox();
			this.textBoxSize = new System.Windows.Forms.TextBox();
			this.textBoxDisplayOffset = new System.Windows.Forms.TextBox();
			this.textBoxClickCoords = new System.Windows.Forms.TextBox();
			this.textBoxClickCoordsTexture = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.TownMapIDBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTileNumber)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.SuspendLayout();
			// 
			// TownMapIDBox
			// 
			this.TownMapIDBox.Location = new System.Drawing.Point(37, 12);
			this.TownMapIDBox.Name = "TownMapIDBox";
			this.TownMapIDBox.Size = new System.Drawing.Size(85, 20);
			this.TownMapIDBox.TabIndex = 0;
			this.TownMapIDBox.Value = new decimal(new int[] {
            45,
            0,
            0,
            0});
			this.TownMapIDBox.ValueChanged += new System.EventHandler(this.TownMapIDBox_ValueChanged);
			// 
			// textBoxFilename
			// 
			this.textBoxFilename.Location = new System.Drawing.Point(128, 12);
			this.textBoxFilename.Name = "textBoxFilename";
			this.textBoxFilename.ReadOnly = true;
			this.textBoxFilename.Size = new System.Drawing.Size(100, 20);
			this.textBoxFilename.TabIndex = 1;
			// 
			// checkBoxEnglish
			// 
			this.checkBoxEnglish.AutoSize = true;
			this.checkBoxEnglish.Location = new System.Drawing.Point(372, 12);
			this.checkBoxEnglish.Name = "checkBoxEnglish";
			this.checkBoxEnglish.Size = new System.Drawing.Size(49, 17);
			this.checkBoxEnglish.TabIndex = 2;
			this.checkBoxEnglish.Text = "ENG";
			this.checkBoxEnglish.UseVisualStyleBackColor = true;
			this.checkBoxEnglish.CheckedChanged += new System.EventHandler(this.TownMapIDBox_ValueChanged);
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
			this.labelTiles.Location = new System.Drawing.Point(234, 14);
			this.labelTiles.Name = "labelTiles";
			this.labelTiles.Size = new System.Drawing.Size(68, 13);
			this.labelTiles.TabIndex = 4;
			this.labelTiles.Text = "Tile (Max ??)";
			// 
			// numericUpDownTileNumber
			// 
			this.numericUpDownTileNumber.Location = new System.Drawing.Point(308, 11);
			this.numericUpDownTileNumber.Name = "numericUpDownTileNumber";
			this.numericUpDownTileNumber.Size = new System.Drawing.Size(58, 20);
			this.numericUpDownTileNumber.TabIndex = 5;
			this.numericUpDownTileNumber.ValueChanged += new System.EventHandler(this.TownMapIDBox_ValueChanged);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.pictureBox1.BackColor = System.Drawing.Color.Black;
			this.pictureBox1.Location = new System.Drawing.Point(16, 226);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(512, 512);
			this.pictureBox1.TabIndex = 6;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
			this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox2.BackColor = System.Drawing.Color.Black;
			this.pictureBox2.Location = new System.Drawing.Point(16, 124);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(954, 96);
			this.pictureBox2.TabIndex = 7;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
			this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
			this.pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(234, 79);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Tile Location";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 52);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(69, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "Info Location";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 79);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 13);
			this.label4.TabIndex = 10;
			this.label4.Text = "TileOffset Location";
			// 
			// textBoxTileOffLoc
			// 
			this.textBoxTileOffLoc.Location = new System.Drawing.Point(115, 76);
			this.textBoxTileOffLoc.Name = "textBoxTileOffLoc";
			this.textBoxTileOffLoc.ReadOnly = true;
			this.textBoxTileOffLoc.Size = new System.Drawing.Size(100, 20);
			this.textBoxTileOffLoc.TabIndex = 11;
			// 
			// textBoxInfoLoc
			// 
			this.textBoxInfoLoc.Location = new System.Drawing.Point(115, 50);
			this.textBoxInfoLoc.Name = "textBoxInfoLoc";
			this.textBoxInfoLoc.ReadOnly = true;
			this.textBoxInfoLoc.Size = new System.Drawing.Size(100, 20);
			this.textBoxInfoLoc.TabIndex = 12;
			// 
			// textBoxTileLoc
			// 
			this.textBoxTileLoc.Location = new System.Drawing.Point(308, 76);
			this.textBoxTileLoc.Name = "textBoxTileLoc";
			this.textBoxTileLoc.ReadOnly = true;
			this.textBoxTileLoc.Size = new System.Drawing.Size(100, 20);
			this.textBoxTileLoc.TabIndex = 13;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(452, 13);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(68, 13);
			this.label5.TabIndex = 14;
			this.label5.Text = "DisplayDelay";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(452, 36);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(71, 13);
			this.label6.TabIndex = 15;
			this.label6.Text = "TextureOffset";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(470, 57);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(27, 13);
			this.label7.TabIndex = 16;
			this.label7.Text = "Size";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(452, 79);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(69, 13);
			this.label8.TabIndex = 17;
			this.label8.Text = "DisplayOffset";
			// 
			// textBoxDisplayDelay
			// 
			this.textBoxDisplayDelay.Location = new System.Drawing.Point(526, 10);
			this.textBoxDisplayDelay.Name = "textBoxDisplayDelay";
			this.textBoxDisplayDelay.ReadOnly = true;
			this.textBoxDisplayDelay.Size = new System.Drawing.Size(100, 20);
			this.textBoxDisplayDelay.TabIndex = 18;
			// 
			// textBoxTextureOffset
			// 
			this.textBoxTextureOffset.Location = new System.Drawing.Point(526, 33);
			this.textBoxTextureOffset.Name = "textBoxTextureOffset";
			this.textBoxTextureOffset.ReadOnly = true;
			this.textBoxTextureOffset.Size = new System.Drawing.Size(100, 20);
			this.textBoxTextureOffset.TabIndex = 19;
			// 
			// textBoxSize
			// 
			this.textBoxSize.Location = new System.Drawing.Point(526, 54);
			this.textBoxSize.Name = "textBoxSize";
			this.textBoxSize.ReadOnly = true;
			this.textBoxSize.Size = new System.Drawing.Size(100, 20);
			this.textBoxSize.TabIndex = 20;
			// 
			// textBoxDisplayOffset
			// 
			this.textBoxDisplayOffset.Location = new System.Drawing.Point(526, 76);
			this.textBoxDisplayOffset.Name = "textBoxDisplayOffset";
			this.textBoxDisplayOffset.ReadOnly = true;
			this.textBoxDisplayOffset.Size = new System.Drawing.Size(100, 20);
			this.textBoxDisplayOffset.TabIndex = 21;
			// 
			// textBoxClickCoords
			// 
			this.textBoxClickCoords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBoxClickCoords.Location = new System.Drawing.Point(534, 226);
			this.textBoxClickCoords.Name = "textBoxClickCoords";
			this.textBoxClickCoords.ReadOnly = true;
			this.textBoxClickCoords.Size = new System.Drawing.Size(100, 20);
			this.textBoxClickCoords.TabIndex = 22;
			// 
			// textBoxClickCoordsTexture
			// 
			this.textBoxClickCoordsTexture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBoxClickCoordsTexture.Location = new System.Drawing.Point(534, 252);
			this.textBoxClickCoordsTexture.Name = "textBoxClickCoordsTexture";
			this.textBoxClickCoordsTexture.ReadOnly = true;
			this.textBoxClickCoordsTexture.Size = new System.Drawing.Size(100, 20);
			this.textBoxClickCoordsTexture.TabIndex = 23;
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
			// TownMapDisplay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(982, 750);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBoxClickCoordsTexture);
			this.Controls.Add(this.textBoxClickCoords);
			this.Controls.Add(this.textBoxDisplayOffset);
			this.Controls.Add(this.textBoxSize);
			this.Controls.Add(this.textBoxTextureOffset);
			this.Controls.Add(this.textBoxDisplayDelay);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textBoxTileLoc);
			this.Controls.Add(this.textBoxInfoLoc);
			this.Controls.Add(this.textBoxTileOffLoc);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.numericUpDownTileNumber);
			this.Controls.Add(this.labelTiles);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkBoxEnglish);
			this.Controls.Add(this.textBoxFilename);
			this.Controls.Add(this.TownMapIDBox);
			this.Name = "TownMapDisplay";
			this.Text = "TownMap Viewer";
			((System.ComponentModel.ISupportInitialize)(this.TownMapIDBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownTileNumber)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown TownMapIDBox;
        private System.Windows.Forms.TextBox textBoxFilename;
        private System.Windows.Forms.CheckBox checkBoxEnglish;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTiles;
        private System.Windows.Forms.NumericUpDown numericUpDownTileNumber;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxTileOffLoc;
        private System.Windows.Forms.TextBox textBoxInfoLoc;
        private System.Windows.Forms.TextBox textBoxTileLoc;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxDisplayDelay;
        private System.Windows.Forms.TextBox textBoxTextureOffset;
        private System.Windows.Forms.TextBox textBoxSize;
        private System.Windows.Forms.TextBox textBoxDisplayOffset;
        private System.Windows.Forms.TextBox textBoxClickCoords;
        private System.Windows.Forms.TextBox textBoxClickCoordsTexture;
        private System.Windows.Forms.Button button1;
    }
}

