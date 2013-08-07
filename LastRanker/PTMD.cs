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
		public uint Uint0x08;
		public uint PalettePointer;
		public byte[] ImageBytes;
		public byte[] PaletteBytes;

		public uint ImageWidth;
		public uint BitPerPixel;
		public bool Greyscale;

		private bool LoadFile( byte[] File ) {
			Magic = BitConverter.ToUInt32( File, 0x00 );
			Uint0x04 = BitConverter.ToUInt32( File, 0x04 );
			ImageWidth = 1u << (int)( Uint0x04 >> 28 );
			Uint0x08 = BitConverter.ToUInt32( File, 0x08 );
			PalettePointer = BitConverter.ToUInt32( File, 0x0C );

			ImageBytes = new byte[PalettePointer - 0x10];
			Util.CopyByteArrayPart( File, 0x10, ImageBytes, 0, ImageBytes.Length );
			PaletteBytes = new byte[File.Length - PalettePointer];
			Util.CopyByteArrayPart( File, (int)PalettePointer, PaletteBytes, 0, PaletteBytes.Length );

			// i'm sure there's a better way to identify this stuff...
			switch ( PaletteBytes.Length ) {
				case 0x400: BitPerPixel = 8; Greyscale = false; break;
				case 0x40: BitPerPixel = 4; Greyscale = false; break;
				case 0x20: BitPerPixel = 4; Greyscale = true; break;
				default: throw new Exception( "PTMD: Unknown bit per pixel: " + PaletteBytes.Length + " Bytes in Palette" );
			}

			return true;
		}

		public void SaveAsPNG( string Path ) {
			int w = 16;
			int h = 8;
			int ImageHeight = 0;
			int TotalPixels = 0;

			switch ( BitPerPixel ) {
				case 8:
					ImageHeight = ImageBytes.Length / (int)ImageWidth;
					w = 16;
					h = 8;
					TotalPixels = ImageBytes.Length;
					break;
				case 4:
					ImageHeight = ( ImageBytes.Length * 2 ) / (int)ImageWidth;
					w = 32;
					h = 8;
					TotalPixels = ImageBytes.Length * 2;
					break;
			}
			int wh = w * h;

			Bitmap bmp = new Bitmap( (int)ImageWidth, ImageHeight );
			int BlocksPerLine = bmp.Width / w;

			// image in blocks of 16x8 px
			for ( int i = 0; i < TotalPixels; ++i ) {
				byte r, g, b, a;
				int idx = 0;
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
