using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.DanganRonpa.Font.Viewer {
	public partial class FontViewer : Form {
		private DRFontInfo FontInfo;
		private Bitmap Texture;

		private Color bgcolor;

		bool BoxByBox = false;

		public string Filepath;

		public FontViewer() {
			InitializeComponent();
		}

		public FontViewer( DRFontInfo f, Bitmap tex ) {
			InitializeComponent();
			this.FontInfo = f;

			//bgcolor = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xED);
			bgcolor = Color.FromArgb( 0xFF, 0xFF, 0xFF, 0x00 );
			Texture = tex;

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

		public void UpdateAny() {
			int CharacterID = (int)TownMapIDBox.Value;
			DRFontChar c = FontInfo.GetChar( CharacterID );
			guiCharWidth.Value = c.Width;
			guiCharHeight.Value = c.Height;
			guiCharX.Value = c.XOffset;
			guiCharY.Value = c.YOffset;

			pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
			if ( textBoxDisplayDelay.Text == "" ) {
				pictureBox2.Image = UpdateSingleCharacter( (int)TownMapIDBox.Value );
			} else {
				List<Bitmap> bmplist = new List<Bitmap>();
				for ( int i = 0; i <= (int)TownMapIDBox.Value; ++i ) {
					bmplist.Add( UpdateSingleCharacter( i ) );
				}
				pictureBox2.Image = CombineBitmaps( bmplist.ToArray() );
			}
		}

		public Bitmap CombineBitmaps( Bitmap[] bmps ) {
			int totalWidth = 0;
			int maxHeight = 0;
			foreach ( Bitmap bmp in bmps ) {
				totalWidth += bmp.Width;
				maxHeight = Math.Max( maxHeight, bmp.Height );
			}

			Bitmap combined = new Bitmap( totalWidth, maxHeight );

			int cx = 0, cy = 0;
			foreach ( Bitmap bmp in bmps ) {
				for ( int x = 0; x < bmp.Width; x++ ) {
					for ( int y = 0; y < bmp.Height; y++ ) {
						Color px = bmp.GetPixel( x, y );
						combined.SetPixel( cx + x, cy + y, px );
					}
				}
				cx += bmp.Width;
			}

			return combined;
		}

		public Bitmap UpdateSingleCharacter( int CharacterID ) {
			try {
				int scale = (int)numericUpDownTileNumber.Value;

				DRFontChar c = FontInfo.GetChar( CharacterID );
				System.Drawing.Bitmap img = new System.Drawing.Bitmap( c.Width * scale, c.Height * scale );

				for ( int x = 0; x < c.Width; x++ ) {
					for ( int y = 0; y < c.Height; y++ ) {
						Color px = Texture.GetPixel( x + c.XOffset, y + c.YOffset );
						if ( px.R <= 10 && px.G <= 10 && px.B <= 10 ) px = bgcolor;
						for ( int sx = 0; sx < scale; ++sx )
							for ( int sy = 0; sy < scale; ++sy )
								img.SetPixel( x * scale + sx, y * scale + sy, px );
					}
				}

				return img;




			} catch ( Exception ) {
				return null;
			}
		}

		/*
		public void Draw(int CharacterID, int xOffs, int yOffs)
		{
			try
			{
				int textureNumber = CharacterID / (16 * 16);
				int CharacterX = (CharacterID % 16);
				int CharacterY = (CharacterID % (16 * 16)) / 16;
				int CurrentFontInfoToUse = (int)numericUpDownTileNumber.Value;

				bool DisplayInBlack = checkBoxApproxDialogue.Checked;

				System.Drawing.Bitmap img = (Bitmap)pictureBox2.Image;

				for (int x = 0; x < FontInfos[CurrentFontInfoToUse].CharacterLengths[CharacterID] + 2; x++)
				{
					for (int y = 0; y < 32; y++)
					{
						Color px = Textures[textureNumber].GetPixel(x + CharacterX * 32, y + CharacterY * 32);
						if (DisplayInBlack)
						{
							if (px.A == 0)
							{
								px = bgcolor;
							}
							else
							{
								px = Color.FromArgb((byte)~px.R, (byte)~px.G, (byte)~px.B);
							}
						}
						else
						{
							if (px.A == 0) px = bgcolor;
						}
						img.SetPixel(x + xOffs, y + yOffs, px);
					}
				}

				pictureBox2.Image = img;
			}
			catch (Exception ex)
			{
                
			}
		}
		 */

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
			UpdateAny();
		}

		private void textBoxDisplayDelay_TextChanged( object sender, EventArgs e ) {
			UpdateAny();
		}

		private void buttonWriteNewCharLength_Click( object sender, EventArgs e ) {
			FontInfo.WriteFile( Filepath + ".new" );
		}

		private void numUpDownCharLength_ValueChanged( object sender, EventArgs e ) {
			int CharacterID = (int)TownMapIDBox.Value;
			DRFontChar c = FontInfo.GetChar( CharacterID );
			c.Width = (ushort)guiCharWidth.Value;

			UpdateAny();
		}

		private void checkBoxApproxDialogue_CheckedChanged( object sender, EventArgs e ) {
			UpdateAny();
		}

		private void guiCharX_ValueChanged( object sender, EventArgs e ) {
			int CharacterID = (int)TownMapIDBox.Value;
			DRFontChar c = FontInfo.GetChar( CharacterID );
			c.XOffset = (ushort)guiCharX.Value;

			UpdateAny();
		}

		private void guiCharY_ValueChanged( object sender, EventArgs e ) {
			int CharacterID = (int)TownMapIDBox.Value;
			DRFontChar c = FontInfo.GetChar( CharacterID );
			c.YOffset = (ushort)guiCharY.Value;

			UpdateAny();
		}

		private void guiCharHeight_ValueChanged( object sender, EventArgs e ) {
			int CharacterID = (int)TownMapIDBox.Value;
			DRFontChar c = FontInfo.GetChar( CharacterID );
			c.Height = (ushort)guiCharHeight.Value;

			UpdateAny();
		}

		private void buttonWriteGN_Click( object sender, EventArgs e ) {
			string[] gn = FontInfo.GetGnConfig();
			System.IO.File.WriteAllLines( "GnConfig.xml", gn );
		}
	}
}
