using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.Tales.Vesperia.Font.Viewer {
	public partial class FontViewer : Form {
		private FontInfo[] FontInfos;
		private Bitmap[] Textures;

		private Color bgcolor;

		private String LastTexture = null;
		private String LastText = null;

		bool IndicateCharacter = false;

		bool BoxByBox = false;

		public FontViewer() {
			InitializeComponent();
		}

		public FontViewer( FontInfo[] f, String Font, int Fontblock, String[] Textlines, bool BoxByBox, bool ApproxDialogueColor ) {
			InitializeComponent();
			this.FontInfos = f;
			this.BoxByBox = BoxByBox;
			this.checkBoxApproxDialogue.Checked = ApproxDialogueColor;

			numericUpDownTileNumber.Value = Fontblock;
			comboBoxFilename.Text = Font;
			bgcolor = Color.FromArgb( 0xFF, 0xFF, 0xFF, 0xED );
			UpdateTextures( comboBoxFilename.Text );

			UpdateText( Textlines );
			UpdateAny();
		}

		public void SaveAsPng( String pngname ) {
			pictureBox2.Image.Save( pngname, System.Drawing.Imaging.ImageFormat.Png );
		}

		public void UpdateText( String[] Textlines ) {
			if ( Textlines == null ) return;

			StringBuilder sb = new StringBuilder();

			foreach ( String line in Textlines ) {
				sb.AppendLine( line );
			}

			String Text = sb.ToString();
			Text = Util.RemoveTags( Text );

			if ( BoxByBox ) {
				List<string> lst = new List<string>();

				int Last = 0;

				for ( int i = 0; i < Text.Length; i++ ) {
					if ( Text[i] == '\n' ) {
						if ( !( i + 1 < Text.Length && Text[i + 1] == '\n' ) ) continue;

						int This = i;

						lst.Add( Text.Substring( Last, i - Last ) );

						while ( i + 1 < Text.Length && Text[i + 1] == '\n' ) i++;

						Last = i + 1;
					}
				}
				lst.Add( Text.Substring( Last, Text.Length - Last ) );

				int count = 0;
				foreach ( string s in lst ) {
					textBoxDisplayDelay.Text = s;
					UpdateAny();
					SaveAsPng( "textbox_" + String.Format( "{0:0000}", count ) + ".png" );
					count++;
				}
			}

			textBoxDisplayDelay.Text = Text;
		}

		public void UpdateTextures( String Filename ) {
			if ( LastTexture == Filename ) return;
			LastTexture = Filename;

			try {
				Textures = new Bitmap[16];
				for ( int i = 0; i < 16; i++ ) {
					try {
						Textures[i] = new System.Drawing.Bitmap( @"FontTex/" + Filename + "_" + i.ToString() + ".png" );
					} catch ( Exception ) {
						Textures[i] = new System.Drawing.Bitmap( @"../../FontTex/" + Filename + "_" + i.ToString() + ".png" );
					}
				}
			} catch ( Exception ex ) {
				Console.WriteLine( "Failed loading Textures: " + ex.ToString() );
			}
		}

		public void UpdateAny() {
			int CurrentFontInfoToUse = (int)numericUpDownTileNumber.Value;
			int CharacterID = (int)TownMapIDBox.Value;
			numUpDownCharLength.Value = FontInfos[CurrentFontInfoToUse].CharacterLengths[CharacterID];

			if ( textBoxDisplayDelay.Text == "" ) {
				UpdateSingleCharacter();
			} else if ( textBoxDisplayDelay.Text == "DISPLAY_ENGLISH_SUBSET" ) {
				UpdateAll();
			} else {
				UpdateText( textBoxDisplayDelay.Text );
			}
		}

		public void UpdateSingleCharacter() {
			try {
				int CharacterID = (int)TownMapIDBox.Value;
				int textureNumber = CharacterID / ( 16 * 16 );
				int CharacterX = ( CharacterID % 16 );
				int CharacterY = ( CharacterID % ( 16 * 16 ) ) / 16;
				int CurrentFontInfoToUse = (int)numericUpDownTileNumber.Value;

				System.Drawing.Bitmap img = new System.Drawing.Bitmap( FontInfos[CurrentFontInfoToUse].CharacterLengths[CharacterID], 32 );

				for ( int x = 0; x < FontInfos[CurrentFontInfoToUse].CharacterLengths[CharacterID]; x++ ) {
					for ( int y = 0; y < 32; y++ ) {
						Color px = Textures[textureNumber].GetPixel( x + CharacterX * 32, y + CharacterY * 32 );
						if ( px.A == 0 ) px = Color.Black;
						img.SetPixel( x, y, px );
					}
				}

				pictureBox2.Image = img;




			} catch ( Exception ) {
				//throw;
			}
		}

		public void WriteElf() {
			int CurrentFontInfoToUse = (int)numericUpDownTileNumber.Value;

			FontInfos[CurrentFontInfoToUse].WriteCharacterLengths( Util.Path, Util.FontInfoOffset + 0x880 * CurrentFontInfoToUse );
		}

		public void UpdateText( string Text ) {
			Text = Util.RemoveTags( Text );
			if ( LastText == Text ) return;
			LastText = Text;

			int CurrentFontInfoToUse = (int)numericUpDownTileNumber.Value;
			int CharacterIDSelected = (int)TownMapIDBox.Value;

			int x = 0;
			int y = 0;
			int xsize = 0;
			int ysize = 0;

			for ( int i = 0; i < Text.Length; i++ ) {
				if ( Text[i] == '\n' ) {
					xsize = Math.Max( xsize, x + 34 );
					x = 0;
					y += 43;
					continue;
				}
				int CharacterID = FontInfos[CurrentFontInfoToUse].GetCharacterIdFromCharacter( Text[i] );
				int Length = FontInfos[CurrentFontInfoToUse].CharacterLengths[CharacterID];
				x += Length + 2;
			}

			xsize = Math.Max( xsize, x + 34 );
			ysize = y + 43;

			//if (xsize > 10000) xsize = 10000;
			//if (ysize > 10000) ysize = 40000;

			Image img = new System.Drawing.Bitmap( xsize, ysize );
			Color cc;
			if ( this.checkBoxApproxDialogue.Checked ) {
				cc = bgcolor;
			} else {
				cc = Color.Black;
			}
			Graphics.FromImage( img ).Clear( cc );
			pictureBox2.Image = img;

			x = 0;
			y = 0;

			for ( int i = 0; i < Text.Length; i++ ) {
				if ( Text[i] == '\n' ) {
					x = 0;
					y += 43;
					continue;
				}
				int CharacterID = FontInfos[CurrentFontInfoToUse].GetCharacterIdFromCharacter( Text[i] );
				int Length = FontInfos[CurrentFontInfoToUse].CharacterLengths[CharacterID];
				Draw( CharacterID, x, y );
				if ( CharacterID == CharacterIDSelected ) {
					DrawLineBelowLetter( x, y, Length );
				}
				x += Length;
				x += 2;
			}
		}

		public void UpdateAll() {
			int CurrentFontInfoToUse = (int)numericUpDownTileNumber.Value;

			int x = 0;
			int y = 0;

			pictureBox2.Image = new System.Drawing.Bitmap( 2560, 2560 );

			int CharacterIDSelected = (int)TownMapIDBox.Value;

			for ( int i = 0; i < 0x220; i++ ) {
				int CharacterID = i;
				int Length = FontInfos[CurrentFontInfoToUse].CharacterLengths[CharacterID];

				if ( x + Length > pictureBox2.Width ) {
					x = 0;
					y += 43;
				}

				Draw( CharacterID, x, y );

				if ( CharacterID == CharacterIDSelected ) {
					DrawLineBelowLetter( x, y, Length );
				}

				x += Length;
				x += 2;
			}
		}

		public void DrawLineBelowLetter( int x, int y, int Length ) {
			if ( !IndicateCharacter ) return;

			System.Drawing.Bitmap img = (Bitmap)pictureBox2.Image;
			Color px = Color.Blue;
			for ( int xx = 0; xx < Length; xx++ ) {
				img.SetPixel( x + xx, y + 33, px );
				img.SetPixel( x + xx, y + 34, px );
				img.SetPixel( x + xx, y + 35, px );
			}
			pictureBox2.Image = img;
		}

		public void Draw( int CharacterID, int xOffs, int yOffs ) {
			try {
				int textureNumber = CharacterID / ( 16 * 16 );
				int CharacterX = ( CharacterID % 16 );
				int CharacterY = ( CharacterID % ( 16 * 16 ) ) / 16;
				int CurrentFontInfoToUse = (int)numericUpDownTileNumber.Value;

				bool DisplayInBlack = checkBoxApproxDialogue.Checked;

				System.Drawing.Bitmap img = (Bitmap)pictureBox2.Image;

				for ( int x = 0; x < FontInfos[CurrentFontInfoToUse].CharacterLengths[CharacterID] + 2; x++ ) {
					for ( int y = 0; y < 32; y++ ) {
						Color px = Textures[textureNumber].GetPixel( x + CharacterX * 32, y + CharacterY * 32 );
						if ( DisplayInBlack ) {
							if ( px.A == 0 ) {
								px = bgcolor;
							} else {
								px = Color.FromArgb( (byte)~px.R, (byte)~px.G, (byte)~px.B );
							}
						} else {
							if ( px.A == 0 ) px = bgcolor;
						}
						img.SetPixel( x + xOffs, y + yOffs, px );
					}
				}

				pictureBox2.Image = img;
			} catch ( Exception ) {

			}
		}

		private void TownMapIDBox_ValueChanged( object sender, EventArgs e ) {
			UpdateAny();
		}

		private void pictureBox2_Click( object sender, EventArgs e ) {

		}

		private void pictureBox2_MouseDown( object sender, MouseEventArgs e ) {
		}

		private void pictureBox1_MouseDown( object sender, MouseEventArgs e ) {
		}

		private void button1_Click( object sender, EventArgs e ) {
			UpdateAny();
		}

		private void textBoxFilename_TextChanged( object sender, EventArgs e ) {
			UpdateTextures( comboBoxFilename.Text );
			UpdateAny();
		}

		private void textBoxDisplayDelay_TextChanged( object sender, EventArgs e ) {
			UpdateAny();
		}

		private void buttonWriteNewCharLength_Click( object sender, EventArgs e ) {
			WriteElf();
		}

		private void numUpDownCharLength_ValueChanged( object sender, EventArgs e ) {
			int CurrentFontInfoToUse = (int)numericUpDownTileNumber.Value;
			int CharacterID = (int)TownMapIDBox.Value;

			FontInfos[CurrentFontInfoToUse].CharacterLengths[CharacterID] = (int)numUpDownCharLength.Value;

			UpdateAny();
		}

		private void checkBoxApproxDialogue_CheckedChanged( object sender, EventArgs e ) {
			UpdateAny();
		}
	}
}
