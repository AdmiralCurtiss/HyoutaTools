using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HyoutaTools.LastRanker {
	class PTMD {
		public static int PtmdToPng( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: infile.PTMD outfile.png" );
				return -1;
			}

			new PTMD( args[0] ).SaveAsPNG( args[1] );

			return 0;
		}

		public PTMD( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "PTMD: Load Failed!" );
			}
		}
		public PTMD( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "PTMD: Load Failed!" );
			}
		}
		public PTMD() { }

		public uint Magic;
		public uint Uint0x04;
		public byte BitPerPixelByte;
		public byte ImageTypeByte;
		public uint PalettePointer;

		public byte[] ImageBytes;
		public byte[] PaletteBytes;

		public uint ImageWidth;
		public uint BitPerPixel;
		public bool Greyscale;
		public bool Paletted;

		private bool LoadFile( byte[] File ) {
			Magic = BitConverter.ToUInt32( File, 0x00 );
			Uint0x04 = BitConverter.ToUInt32( File, 0x04 );
			ImageWidth = 1u << (int)( Uint0x04 >> 28 );
			BitPerPixelByte = File[0x08];
			ImageTypeByte = File[0x09];
			PalettePointer = BitConverter.ToUInt32( File, 0x0C );

			ImageBytes = new byte[PalettePointer - 0x10];
			Util.CopyByteArrayPart( File, 0x10, ImageBytes, 0, ImageBytes.Length );
			PaletteBytes = new byte[File.Length - PalettePointer];
			Util.CopyByteArrayPart( File, (int)PalettePointer, PaletteBytes, 0, PaletteBytes.Length );

			//*
			Console.Write( "PTMD: Header: " );
			for ( int i = 4; i < 12; ++i ) {
				Console.Write( File[i].ToString( "X2" ) + " " );
			}
			Console.Write( " / size: ~" + PalettePointer.ToString( "D6" ) );
			//*/

			switch ( BitPerPixelByte ) {
				case 0x04: BitPerPixel = 4; break;
				case 0x05: BitPerPixel = 8; break;
				case 0x01: BitPerPixel = 16; break;
				case 0x03: BitPerPixel = 32; break;
				case 0x00: BitPerPixel = 16; break; // seems like a nonsense file, probably early version or something?
				default: throw new Exception( "PTMD: Unknown bit per pixel byte: 0x" + BitPerPixelByte.ToString( "X2" ) );
			}
			switch ( ImageTypeByte ) {
				case 0x20: Greyscale = true; Paletted = true; break;
				case 0x30: Greyscale = false; Paletted = true; break;
				case 0x00: Greyscale = false; Paletted = false; break;
				case 0x10: Greyscale = true; Paletted = true; break; // ???
				default: throw new Exception( "PTMD: Unknown Image Type Byte: 0x" + ImageTypeByte.ToString( "X2" ) );
			}

			//*
			Console.WriteLine( " w: " + ImageWidth.ToString( "D4" ) + " / bpp: " + BitPerPixel.ToString( "D2" ) + " / grey: " + Greyscale + " / pal: " + Paletted );
			//*/

			return true;
		}

		public void SaveAsPNG( string Path ) {
			int w = 16;
			int h = 8;
			int ImageHeight = 0;
			int TotalPixels = 0;

			switch ( BitPerPixel ) {
				case 8:
					TotalPixels = ImageBytes.Length;
					ImageHeight = TotalPixels / (int)ImageWidth;
					w = 16;
					h = 8;
					break;
				case 4:
					TotalPixels = ImageBytes.Length * 2;
					ImageHeight = TotalPixels / (int)ImageWidth;
					w = 32;
					h = 8;
					break;
				case 16:
					TotalPixels = ImageBytes.Length / 2;
					ImageHeight = TotalPixels / (int)ImageWidth;
					w = 8;
					h = 8;
					break;
				case 32:
					TotalPixels = ImageBytes.Length / 4;
					ImageHeight = TotalPixels / (int)ImageWidth;
					w = 4;
					h = 4;
					break;
			}
			int wh = w * h;

			Bitmap bmp = new Bitmap( (int)ImageWidth, ImageHeight );
			int BlocksPerLine = bmp.Width / w;

			// image in blocks of w x h px
			for ( int i = 0; i < TotalPixels; ++i ) {
				byte r, g, b, a;
				int idx = 0;

				if ( Paletted ) {
					switch ( BitPerPixel ) {
						case 8:
							idx = ImageBytes[i];
							break;
						case 4:
							if ( i % 2 == 1 ) {
								idx = ( ImageBytes[i / 2] >> 4 ) & 0x0F;
							} else {
								idx = ImageBytes[i / 2] & 0x0F;
							}
							break;
						default: throw new Exception( "PTMD: BPP switch (paletted), shouldn't be here." );
					}

					if ( Greyscale ) {
						r = g = b = PaletteBytes[idx * 2 + 1];
						a = PaletteBytes[idx * 2];
					} else {
						r = PaletteBytes[idx * 4];
						g = PaletteBytes[idx * 4 + 1];
						b = PaletteBytes[idx * 4 + 2];
						a = PaletteBytes[idx * 4 + 3];
					}
				} else {
					int col;
					switch ( BitPerPixel ) {
						case 16:
							col = ImageBytes[i * 2] | ImageBytes[i * 2 + 1] << 8;
							r = (byte)( ( col & 0x1F ) << 3 );
							g = (byte)( ( ( col >> 5 ) & 0x1F ) << 3 );
							b = (byte)( ( ( col >> 10 ) & 0x1F ) << 3 );
							a = ( ( col >> 15 ) & 0x01 ) == 0 ? (byte)0x00 : (byte)0xFF;
							break;
						case 32:
							r = ImageBytes[i * 4];
							g = ImageBytes[i * 4 + 1];
							b = ImageBytes[i * 4 + 2];
							a = ImageBytes[i * 4 + 3];
							break;
						case 8: // no clue if this is right
							r = g = b = ImageBytes[i];
							a = 0xFF;
							break;
						case 4: // no clue if this is right
							if ( i % 2 == 1 ) {
								r = g = b = (byte)( ( ( ImageBytes[i / 2] >> 4 ) & 0x0F ) << 4 );
							} else {
								r = g = b = (byte)( ( ImageBytes[i / 2] & 0x0F ) << 4 );
							}
							a = 0xFF;
							break;
						default: throw new Exception( "PTMD: BPP switch, shouldn't be here." );
					}
				}

				int CurrentBlock = i / wh;
				int InBlockX = i % w;
				int InBlockY = ( i % wh ) / w;
				int CurrentBlockLocX = CurrentBlock % BlocksPerLine;
				int CurrentBlockLocY = CurrentBlock / BlocksPerLine;

				bmp.SetPixel( ( CurrentBlockLocX * w ) + InBlockX, ( CurrentBlockLocY * h ) + InBlockY, Color.FromArgb( a, r, g, b ) );
			}

			bmp.Save( Path );
		}
	}
}
