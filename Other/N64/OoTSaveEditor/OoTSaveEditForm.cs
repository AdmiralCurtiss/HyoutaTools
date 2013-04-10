using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.Other.N64.OoTSaveEditor {
	public partial class OoTSaveEditForm : Form {

		const byte False = 0x00;
		const byte True = 0x01;

		OoTSaveFile SaveFile = null;
		String Filename;

		public OoTSaveEditForm() {
			InitializeComponent();
		}

		private void NumUpDownSelectedSave_ValueChanged( object sender, EventArgs e ) {
			DisplaySave( (int)SelectedSaveNumUpDown.Value );
		}

		private void DisplaySave( int ID ) {
			NameTextBox.Text = this.SaveFile.Saves[ID].PlayerName;
			ChildLinkCheckbox.Checked = this.SaveFile.Saves[ID].Age != 0;
			SixtyFourDiskDriveCheckBox.Checked = this.SaveFile.Saves[ID].DiskDriveSaveFlag != 0;
			DeathsNumericUpDown.Value = this.SaveFile.Saves[ID].DeathCounter;
			HealthNumericUpDown.Value = this.SaveFile.Saves[ID].Health;
			RupeesNumericUpDown.Value = this.SaveFile.Saves[ID].Rupees;
			MaxHealthNumericUpDown.Value = this.SaveFile.Saves[ID].MaxHealth;
		}
		private void WriteSelectedSave() {
			WriteSave( (int)SelectedSaveNumUpDown.Value );
		}
		private void WriteSave( int ID ) {
			//NameTextBox.Text = this.SaveFile.Saves[ID].PlayerName;
			this.SaveFile.Saves[ID].Age = ChildLinkCheckbox.Checked ? True : False;
			this.SaveFile.Saves[ID].DiskDriveSaveFlag = SixtyFourDiskDriveCheckBox.Checked ? True : False;
			this.SaveFile.Saves[ID].DeathCounter = (ushort)DeathsNumericUpDown.Value;
			this.SaveFile.Saves[ID].Health = (ushort)HealthNumericUpDown.Value;
			this.SaveFile.Saves[ID].Rupees = (uint)RupeesNumericUpDown.Value;
			this.SaveFile.Saves[ID].MaxHealth = (ushort)MaxHealthNumericUpDown.Value;
		}

		private void buttonOpen_Click( object sender, EventArgs e ) {
			DisplayOpenFileDialog();
		}

		private void buttonSave_Click( object sender, EventArgs e ) {
			WriteHeader();
			WriteSelectedSave();
			SaveFile.WriteSave( Filename );
		}

		private void WriteHeader() {
			SaveFile.Header.ZTargetOptions = this.ZTargettingHoldCheckbox.Checked ? True : False;
			SaveFile.Header.SoundOptions = (byte)this.SoundComboBox.SelectedIndex;
		}

		private void DisplayOpenFileDialog() {
			OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
			dialog.FileName = "THE LEGEND OF ZELDA.sra";
			dialog.Filter = "OoT SRAM (*.sra)|*.sra|Any File|*.*";
			DialogResult result = dialog.ShowDialog();
			if ( result == DialogResult.OK ) {
				this.SuspendLayout();
				this.Filename = dialog.FileName;
				LoadFile( new OoTSaveFile( dialog.FileName ) );
				this.ResumeLayout( true );
			}
		}

		private void LoadFile( OoTSaveFile save ) {
			this.SaveFile = save;


			this.SoundComboBox.SelectedIndex = save.Header.SoundOptions;
			this.ZTargettingHoldCheckbox.Checked = save.Header.ZTargetOptions != 0;

			DisplaySave( (int)SelectedSaveNumUpDown.Value );
		}

		private void ZTargettingHoldCheckbox_CheckedChanged( object sender, EventArgs e ) {
		}
		private void NameTextBox_TextChanged( object sender, EventArgs e ) {
		}
		private void ChildLinkCheckbox_CheckedChanged( object sender, EventArgs e ) {
		}
		private void DeathsNumericUpDown_ValueChanged( object sender, EventArgs e ) {
		}
		private void SixtyFourDiskDriveCheckBox_CheckedChanged( object sender, EventArgs e ) {
		}
		private void HealthNumericUpDown_ValueChanged( object sender, EventArgs e ) {
		}
		private void RupeesNumericUpDown_ValueChanged( object sender, EventArgs e ) {
		}
		private void MaxHealthNumericUpDown_ValueChanged( object sender, EventArgs e ) {
		}
		private void SoundComboBox_SelectedIndexChanged( object sender, EventArgs e ) {
		}
	}
}
