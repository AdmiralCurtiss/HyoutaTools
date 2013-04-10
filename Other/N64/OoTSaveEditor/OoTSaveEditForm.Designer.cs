namespace HyoutaTools.Other.N64.OoTSaveEditor {
	partial class OoTSaveEditForm {
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
			this.ZTargettingHoldCheckbox = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.NameTextBox = new System.Windows.Forms.TextBox();
			this.SoundComboBox = new System.Windows.Forms.ComboBox();
			this.SelectedSaveNumUpDown = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label7 = new System.Windows.Forms.Label();
			this.MaxHealthNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.RupeesNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.HealthNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.SixtyFourDiskDriveCheckBox = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.DeathsNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.ChildLinkCheckbox = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonOpen = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.SelectedSaveNumUpDown)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.MaxHealthNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.RupeesNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.HealthNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.DeathsNumericUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// ZTargettingHoldCheckbox
			// 
			this.ZTargettingHoldCheckbox.AutoSize = true;
			this.ZTargettingHoldCheckbox.Location = new System.Drawing.Point(139, 14);
			this.ZTargettingHoldCheckbox.Name = "ZTargettingHoldCheckbox";
			this.ZTargettingHoldCheckbox.Size = new System.Drawing.Size(109, 17);
			this.ZTargettingHoldCheckbox.TabIndex = 0;
			this.ZTargettingHoldCheckbox.Text = "Z Targetting Hold";
			this.ZTargettingHoldCheckbox.UseVisualStyleBackColor = true;
			this.ZTargettingHoldCheckbox.CheckedChanged += new System.EventHandler(this.ZTargettingHoldCheckbox_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Sound";
			// 
			// NameTextBox
			// 
			this.NameTextBox.Location = new System.Drawing.Point(44, 4);
			this.NameTextBox.Name = "NameTextBox";
			this.NameTextBox.Size = new System.Drawing.Size(100, 20);
			this.NameTextBox.TabIndex = 2;
			this.NameTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
			// 
			// SoundComboBox
			// 
			this.SoundComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SoundComboBox.FormattingEnabled = true;
			this.SoundComboBox.Items.AddRange(new object[] {
            "Stereo",
            "Mono",
            "Headset",
            "Surround"});
			this.SoundComboBox.Location = new System.Drawing.Point(56, 12);
			this.SoundComboBox.Name = "SoundComboBox";
			this.SoundComboBox.Size = new System.Drawing.Size(77, 21);
			this.SoundComboBox.TabIndex = 3;
			this.SoundComboBox.SelectedIndexChanged += new System.EventHandler(this.SoundComboBox_SelectedIndexChanged);
			// 
			// SelectedSaveNumUpDown
			// 
			this.SelectedSaveNumUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.SelectedSaveNumUpDown.Location = new System.Drawing.Point(726, 12);
			this.SelectedSaveNumUpDown.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.SelectedSaveNumUpDown.Name = "SelectedSaveNumUpDown";
			this.SelectedSaveNumUpDown.Size = new System.Drawing.Size(30, 20);
			this.SelectedSaveNumUpDown.TabIndex = 4;
			this.SelectedSaveNumUpDown.ValueChanged += new System.EventHandler(this.NumUpDownSelectedSave_ValueChanged);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(643, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Selected Save";
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.label7);
			this.panel1.Controls.Add(this.MaxHealthNumericUpDown);
			this.panel1.Controls.Add(this.label6);
			this.panel1.Controls.Add(this.RupeesNumericUpDown);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.HealthNumericUpDown);
			this.panel1.Controls.Add(this.SixtyFourDiskDriveCheckBox);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.DeathsNumericUpDown);
			this.panel1.Controls.Add(this.ChildLinkCheckbox);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.NameTextBox);
			this.panel1.Location = new System.Drawing.Point(12, 39);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(744, 394);
			this.panel1.TabIndex = 6;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(138, 33);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(58, 13);
			this.label7.TabIndex = 13;
			this.label7.Text = "MaxHealth";
			// 
			// MaxHealthNumericUpDown
			// 
			this.MaxHealthNumericUpDown.Location = new System.Drawing.Point(202, 31);
			this.MaxHealthNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.MaxHealthNumericUpDown.Name = "MaxHealthNumericUpDown";
			this.MaxHealthNumericUpDown.Size = new System.Drawing.Size(58, 20);
			this.MaxHealthNumericUpDown.TabIndex = 12;
			this.MaxHealthNumericUpDown.ValueChanged += new System.EventHandler(this.MaxHealthNumericUpDown_ValueChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(275, 33);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(44, 13);
			this.label6.TabIndex = 11;
			this.label6.Text = "Rupees";
			// 
			// RupeesNumericUpDown
			// 
			this.RupeesNumericUpDown.Location = new System.Drawing.Point(325, 31);
			this.RupeesNumericUpDown.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
			this.RupeesNumericUpDown.Name = "RupeesNumericUpDown";
			this.RupeesNumericUpDown.Size = new System.Drawing.Size(58, 20);
			this.RupeesNumericUpDown.TabIndex = 10;
			this.RupeesNumericUpDown.ValueChanged += new System.EventHandler(this.RupeesNumericUpDown_ValueChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(2, 33);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(68, 13);
			this.label5.TabIndex = 9;
			this.label5.Text = "Health (16th)";
			// 
			// HealthNumericUpDown
			// 
			this.HealthNumericUpDown.Location = new System.Drawing.Point(76, 31);
			this.HealthNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.HealthNumericUpDown.Name = "HealthNumericUpDown";
			this.HealthNumericUpDown.Size = new System.Drawing.Size(58, 20);
			this.HealthNumericUpDown.TabIndex = 8;
			this.HealthNumericUpDown.ValueChanged += new System.EventHandler(this.HealthNumericUpDown_ValueChanged);
			// 
			// SixtyFourDiskDriveCheckBox
			// 
			this.SixtyFourDiskDriveCheckBox.AutoSize = true;
			this.SixtyFourDiskDriveCheckBox.Location = new System.Drawing.Point(353, 6);
			this.SixtyFourDiskDriveCheckBox.Name = "SixtyFourDiskDriveCheckBox";
			this.SixtyFourDiskDriveCheckBox.Size = new System.Drawing.Size(82, 17);
			this.SixtyFourDiskDriveCheckBox.TabIndex = 7;
			this.SixtyFourDiskDriveCheckBox.Text = "64DD Save";
			this.SixtyFourDiskDriveCheckBox.UseVisualStyleBackColor = true;
			this.SixtyFourDiskDriveCheckBox.CheckedChanged += new System.EventHandler(this.SixtyFourDiskDriveCheckBox_CheckedChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(229, 7);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(41, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Deaths";
			// 
			// DeathsNumericUpDown
			// 
			this.DeathsNumericUpDown.Location = new System.Drawing.Point(276, 4);
			this.DeathsNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.DeathsNumericUpDown.Name = "DeathsNumericUpDown";
			this.DeathsNumericUpDown.Size = new System.Drawing.Size(58, 20);
			this.DeathsNumericUpDown.TabIndex = 5;
			this.DeathsNumericUpDown.ValueChanged += new System.EventHandler(this.DeathsNumericUpDown_ValueChanged);
			// 
			// ChildLinkCheckbox
			// 
			this.ChildLinkCheckbox.AutoSize = true;
			this.ChildLinkCheckbox.Location = new System.Drawing.Point(151, 6);
			this.ChildLinkCheckbox.Name = "ChildLinkCheckbox";
			this.ChildLinkCheckbox.Size = new System.Drawing.Size(72, 17);
			this.ChildLinkCheckbox.TabIndex = 4;
			this.ChildLinkCheckbox.Text = "Child Link";
			this.ChildLinkCheckbox.UseVisualStyleBackColor = true;
			this.ChildLinkCheckbox.CheckedChanged += new System.EventHandler(this.ChildLinkCheckbox_CheckedChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 7);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Name";
			// 
			// buttonSave
			// 
			this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSave.Location = new System.Drawing.Point(681, 439);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(75, 23);
			this.buttonSave.TabIndex = 7;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// buttonOpen
			// 
			this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOpen.Location = new System.Drawing.Point(600, 439);
			this.buttonOpen.Name = "buttonOpen";
			this.buttonOpen.Size = new System.Drawing.Size(75, 23);
			this.buttonOpen.TabIndex = 8;
			this.buttonOpen.Text = "Open";
			this.buttonOpen.UseVisualStyleBackColor = true;
			this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
			// 
			// OoTSaveEditForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(768, 474);
			this.Controls.Add(this.buttonOpen);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.SelectedSaveNumUpDown);
			this.Controls.Add(this.SoundComboBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.ZTargettingHoldCheckbox);
			this.Name = "OoTSaveEditForm";
			this.Text = "OoT Save Editor";
			((System.ComponentModel.ISupportInitialize)(this.SelectedSaveNumUpDown)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.MaxHealthNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.RupeesNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.HealthNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.DeathsNumericUpDown)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox ZTargettingHoldCheckbox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox NameTextBox;
		private System.Windows.Forms.ComboBox SoundComboBox;
		private System.Windows.Forms.NumericUpDown SelectedSaveNumUpDown;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.Button buttonOpen;
		private System.Windows.Forms.CheckBox ChildLinkCheckbox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown DeathsNumericUpDown;
		private System.Windows.Forms.CheckBox SixtyFourDiskDriveCheckBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown RupeesNumericUpDown;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown HealthNumericUpDown;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown MaxHealthNumericUpDown;
	}
}