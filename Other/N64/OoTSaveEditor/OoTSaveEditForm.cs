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

		ComboBox[] InventoryBoxes;
		NumericUpDown[] AmmoCounts;
		ComboBox Room;

		public OoTSaveEditForm() {
			Room = new ComboBox();
			Room.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			Room.FormattingEnabled = true;
			Room.Location = new System.Drawing.Point( 450, 44 );
			Room.Name = "RoomComboBox";
			Room.Size = new System.Drawing.Size( 350, 21 );
			Room.DataSource = Enum.GetValues( typeof( Rooms ) );
			this.Controls.Add( Room );

			InventoryBoxes = new ComboBox[24];
			for ( int i = 0; i < 24; ++i ) {
				InventoryBoxes[i] = new ComboBox();
				InventoryBoxes[i].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
				InventoryBoxes[i].FormattingEnabled = true;
				InventoryBoxes[i].Location = new System.Drawing.Point( 15 + ( i % 6 ) * 120, 100 + ( i / 6 ) * 24 );
				InventoryBoxes[i].Name = "InventoryComboBox" + i;
				InventoryBoxes[i].Size = new System.Drawing.Size( 77, 21 );
				InventoryBoxes[i].DataSource = Enum.GetValues( typeof( Items ) );
				this.Controls.Add( InventoryBoxes[i] );
			}
			AmmoCounts = new NumericUpDown[15];
			for ( int i = 0; i < 15; ++i ) {
				AmmoCounts[i] = new NumericUpDown();
				AmmoCounts[i].Location = new System.Drawing.Point( 15 + 80 + ( i % 6 ) * 120, 100 + ( i / 6 ) * 24 );
				AmmoCounts[i].Maximum = new decimal( new int[] { 255, 0, 0, 0 } );
				AmmoCounts[i].Name = "AmmoCount" + i;
				AmmoCounts[i].Size = new System.Drawing.Size( 40, 21 );
				this.Controls.Add( AmmoCounts[i] );
			}
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
			for ( int i = 0; i < 24; ++i ) {
				this.InventoryBoxes[i].SelectedIndex = SaveFile.Saves[ID].ItemInventory[i];
			}
			for ( int i = 0; i < 15; ++i ) {
				this.AmmoCounts[i].Value = SaveFile.Saves[ID].ItemInventoryAmmoCounts[i];
			}
			Room.SelectedItem = (Rooms)SaveFile.Saves[ID].EntranceIndex;
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
			this.SaveFile.Saves[ID].Rupees = (ushort)RupeesNumericUpDown.Value;
			this.SaveFile.Saves[ID].MaxHealth = (ushort)MaxHealthNumericUpDown.Value;
			for ( int i = 0; i < 24; ++i ) {
				SaveFile.Saves[ID].ItemInventory[i] = (byte)this.InventoryBoxes[i].SelectedIndex;
			}
			for ( int i = 0; i < 15; ++i ) {
				SaveFile.Saves[ID].ItemInventoryAmmoCounts[i] = (byte)this.AmmoCounts[i].Value;
			}
			string rmstr = Room.SelectedValue.ToString();
			Rooms rm = (Rooms)Enum.Parse(typeof(Rooms), rmstr);
			this.SaveFile.Saves[ID].EntranceIndex = (ushort)rm;
			SaveFile.Saves[ID].Room = Byte.Parse(rmstr.Substring( rmstr.IndexOf( "Room" ) + 4, 2 ), System.Globalization.NumberStyles.AllowHexSpecifier);
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
