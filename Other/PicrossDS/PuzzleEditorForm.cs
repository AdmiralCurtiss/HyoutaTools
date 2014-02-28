using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.Other.PicrossDS {
	public partial class PuzzleEditorForm : Form {
		private SaveFile Save;
		private String OriginalFilename;

		public PuzzleEditorForm() {
			InitializeComponent();

			if ( !LoadSave() ) {
				this.Close();
			}
		}

		private bool LoadSave() {
			OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
			dialog.Filter = "NDS Save (*.sav, *.dsv)|*.sav;*.dsv|Any File|*.*";
			DialogResult result = dialog.ShowDialog();
			if ( result == DialogResult.OK ) {
				this.SuspendLayout();
				OriginalFilename = dialog.FileName;
				Save = new SaveFile( OriginalFilename );
				Save.LoadClassicPuzzles();
				foreach ( var puzzle in Save.ClassicPuzzles ) {
					comboBoxPuzzleSlot.Items.Add( puzzle );
				}
				comboBoxPuzzleSlot.SelectedIndex = 0;

				this.ResumeLayout( true );

				return true;
			}

			return false;
		}

		void PopulateGuiWithPuzzle( ClassicPuzzle puzzle ) {
			try {
				comboBoxPuzzleDimensions.SelectedIndex = ( puzzle.Width / 5 ) - 1; // there's only five valid dimensions, this should always get the right one
			} catch ( ArgumentOutOfRangeException ) {
				comboBoxPuzzleDimensions.SelectedIndex = 0;
			}

			checkBoxFreeMode.Checked = puzzle.Mode == 0x02;
			checkBoxCleared.Checked = puzzle.Unknown2 == 0x59; // almost definitely wrong, should look at this more
			textBoxCleartime.Text = puzzle.ClearTime.ToString();
			textBoxName.Text = puzzle.PuzzleName.Trim( '\0' );
			textBoxPack.Text = puzzle.PackName.Trim( '\0' );
			
			try {
				comboBoxPackLetter.SelectedIndex = puzzle.PackLetter;
			} catch ( ArgumentOutOfRangeException ) {
				comboBoxPackLetter.SelectedIndex = 0;
			}
			try {
				comboBoxPackNumber.SelectedIndex = puzzle.PackNumber;
			} catch ( ArgumentOutOfRangeException ) {
				comboBoxPackNumber.SelectedIndex = 0;
			}
		}

		private void WriteGuiPuzzleDataToSave( object sender, EventArgs e ) {

		}

		private void comboBoxPuzzleSlot_SelectedIndexChanged( object sender, EventArgs e ) {
			PopulateGuiWithPuzzle( (ClassicPuzzle)comboBoxPuzzleSlot.SelectedItem );
		}

		private void buttonExport_Click( object sender, EventArgs e ) {

		}

		private void buttonImport_Click( object sender, EventArgs e ) {

		}

		private void buttonSave_Click( object sender, EventArgs e ) {
			Save.WriteFile( OriginalFilename );
		}

		private void buttonSaveAs_Click( object sender, EventArgs e ) {

		}
	}
}
