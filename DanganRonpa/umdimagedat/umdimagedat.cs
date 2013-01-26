using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.DanganRonpa.umdimagedat {
	class umdimagedat {
		static int EbootHeaderSize;
		static int EbootUmddataBinSectionStart;

		static byte[] OriginalEbootHeader = { 0x7F, 0x45, 0x4C, 0x46, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA0, 0xFF, 0x08, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0C, 0x01, 0x00, 0x00, 0x34, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x30, 0xA2, 0x10, 0x34, 0x00, 0x20, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x44, 0xE0, 0x0D, 0x00, 0x88, 0xC1, 0x10, 0x00, 0x88, 0xC1, 0x10, 0x00, 0x07, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x80, 0xC2, 0x10, 0x00, 0xC0, 0xC1, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x9C, 0x00, 0x00, 0xC4, 0x27, 0x39, 0x00, 0x06, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x70, 0x70, 0x5F, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE0, 0x6A, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE0, 0xFF, 0xBD, 0x27, 0x14, 0x00, 0xBF, 0xAF, 0x2B, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00 };
		static byte[] WeirdTranslationEbootHeader = { 0x7F, 0x45, 0x4C, 0x46, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA0, 0xFF, 0x08, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0C, 0x01, 0x00, 0x00, 0x34, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x30, 0xA2, 0x10, 0x34, 0x00, 0x20, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x90, 0x90, 0x13, 0x00, 0x80, 0xE1, 0x14, 0x00, 0x80, 0xE1, 0x14, 0x00, 0x07, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x40, 0xE2, 0x14, 0x00, 0x80, 0xE1, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC8, 0x93, 0x00, 0x00, 0x84, 0x55, 0x35, 0x00, 0x06, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x70, 0x10, 0x76, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x58, 0x7B, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE0, 0xFF, 0xBD, 0x27, 0x14, 0x00, 0xBF, 0xAF, 0x2B, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00 };

		static bool ArrayMatch( Byte[] a1, Byte[] a2, int offset1, int offset2, int length ) {
			for ( int i = 0; i < length; ++i ) {
				if ( a1[offset1 + i] != a2[offset2 + i] ) {
					return false;
				}
			}
			return true;
		}

		static int IdentifyEbootVersion( Byte[] Eboot ) {
			EbootHeaderSize = BitConverter.ToInt32( Eboot, 0x38 );

			if ( EbootHeaderSize == 0xC0 ) {
				if ( ArrayMatch( Eboot, WeirdTranslationEbootHeader, 0, 0, EbootHeaderSize ) ) {
					// weird other fan translation
					EbootUmddataBinSectionStart = 0x145c24;
				} else {
					// original game version
					EbootUmddataBinSectionStart = 0xF8248;
				}
				return 1;
			} else if ( EbootHeaderSize == 0xA0 ) {
				// best of version
				EbootUmddataBinSectionStart = 0xF5A18;
				return 2;
			}

			// unknown version
			return -1;
		}

		public static int Execute( string[] args ) {
			if ( args.Length == 0 ) {
				Console.WriteLine( "extract without eboot (no filenames!):" );
				Console.WriteLine( " umdimagedat e umdimage.dat" );
				Console.WriteLine( "extract with eboot:" );
				Console.WriteLine( " umdimagedat e umdimage.dat eboot [-nofilenames]" );
				Console.WriteLine( "pack:" );
				Console.WriteLine( " umdimagedat p umdimage.dat.new umdimagedatdir eboot_orig eboot_new" );
				return -1;
			}

			if ( args[0] == "e" ) {
				if ( args.Length == 2 ) {
					return Extract( args[1] );
				} else {
					return Extract( args[1], args[2], args.Length > 3 && args[3] == "-nofilenames" );
				}
			} else if ( args[0] == "p" ) {
				return Pack( args[1], args[2], args[3], args[4] );
			}
			return -1;
		}

		public static int AlignToByteBoundary( int x, int boundary ) {
			int diff = x % boundary;
			if ( diff == 0 ) return x;

			diff = boundary - diff;
			return x + diff;
		}

		static int Pack( string Filename, string Directory, string SourceEboot, string NewEboot ) {
			Console.WriteLine( "Reading Eboot..." );
			Byte[] Eboot = System.IO.File.ReadAllBytes( SourceEboot );

			if ( IdentifyEbootVersion( Eboot ) == -1 ) {
				Console.WriteLine( "Failed identifying Eboot version!" );
				return -1;
			}		 

			Console.WriteLine( "Reading File list..." );
			string[] Files = System.IO.Directory.GetFiles( Directory );
			List<string> flist = Files.ToList();
			flist.Sort();
			Files = flist.ToArray();

			Console.WriteLine( "Opening Files and calculating sizes..." );
			List<FileStream> fslist = new List<FileStream>( Files.Length );
			List<int> filesizelist = new List<int>( Files.Length );
			int unalignedsize = 0;
			foreach ( string file in Files ) {
				FileStream fs = System.IO.File.OpenRead( file );
				unalignedsize = (int)fs.Length;
				// TODO test if this align here is really neccessary
				int filesize = AlignToByteBoundary( unalignedsize, 1024 );
				fslist.Add( fs );
				filesizelist.Add( filesize );
			}
			filesizelist[filesizelist.Count - 1] = unalignedsize;

			int Headersize = AlignToByteBoundary( ( Files.Length + 1 ) * 4, 0x4000 );

			Console.WriteLine( "Starting write..." );
			FileStream outs = new FileStream( Filename, FileMode.Create );
			outs.Write( BitConverter.GetBytes( Files.Length ), 0, 4 );

			// header
			Console.WriteLine( "Writing header & modifying Eboot..." );
			int total = Headersize;
			for ( int i = 0; i < filesizelist.Count; ++i ) {
				//Console.WriteLine("  File " + i.ToString());
				// get stuff
				int size = filesizelist[i];
				byte[] in_filesize = BitConverter.GetBytes( size );
				byte[] offset = BitConverter.GetBytes( total );

				// write to header
				outs.Write( offset, 0, 4 );

				// write to eboot
				offset.CopyTo( Eboot, EbootUmddataBinSectionStart + 4 + ( i * 12 ) );
				in_filesize.CopyTo( Eboot, EbootUmddataBinSectionStart + 8 + ( i * 12 ) );

				total += size;
			}
			while ( outs.Position < Headersize ) {
				outs.WriteByte( 0x00 );
			}

			byte[] buffer = new byte[0x2000];

			// files
			Console.WriteLine( "Writing files..." );
			for ( int i = 0; i < fslist.Count; i++ ) {
				FileStream fs = fslist[i];
				int filesize = filesizelist[i];
				int actualsize = (int)fs.Length;


				int offs = 0;

				while ( fs.Position < fs.Length ) {
					int bytesread = fs.Read( buffer, offs, 0x2000 );
					outs.Write( buffer, 0, bytesread );
				}

				while ( actualsize < filesize ) {
					outs.WriteByte( 0x00 );
					actualsize++;
				}

				fs.Close();
			}

			outs.Close();
			Console.WriteLine( "umdimage.dat done!" );

			Console.WriteLine( "Writing Eboot..." );
			System.IO.File.WriteAllBytes( NewEboot, Eboot );
			Console.WriteLine( "Eboot done!" );

			return 0;
		}

		static int Extract( string filepath ) {
			byte[] File = System.IO.File.ReadAllBytes( filepath );

			int FileAmount = BitConverter.ToInt32( File, 0 );
			DirectoryInfo dir = Directory.CreateDirectory( filepath + ".ex" );
			for ( int i = 0; i < FileAmount; i++ ) {
				int OffsetStart = BitConverter.ToInt32( File, 4 + i * 4 );
				int OffsetEnd = BitConverter.ToInt32( File, 8 + i * 4 );
				if ( OffsetEnd == 0 ) OffsetEnd = File.Length;

				System.IO.FileStream stream = System.IO.File.Create( dir.FullName + "/" + i.ToString( "D8" ) );
				stream.Write( File, OffsetStart, ( OffsetEnd - OffsetStart ) );
				stream.Close();
			}

			return 0;
		}

		static int Extract( string filepath, string ebootpath, bool nofilenames ) {
			byte[] File = System.IO.File.ReadAllBytes( filepath );
			byte[] Eboot = System.IO.File.ReadAllBytes( ebootpath );

			if ( IdentifyEbootVersion( Eboot ) == -1 ) {
				Console.WriteLine( "Failed identifying Eboot version!" );
				return -1;
			}

			int FileAmount = BitConverter.ToInt32( File, 0 );
			DirectoryInfo dir = Directory.CreateDirectory( filepath + ".ex" );

			for ( int i = 0; i < FileAmount; i++ ) {
				int FilenameOffset = BitConverter.ToInt32( Eboot, EbootUmddataBinSectionStart + 0 + ( i * 12 ) );
				FilenameOffset += EbootHeaderSize;
				int OffsetStart = BitConverter.ToInt32( File, 4 + i * 4 );
				int OffsetEnd = BitConverter.ToInt32( File, 8 + i * 4 );
				if ( OffsetEnd == 0 ) {
					OffsetEnd = File.Length;
				}

				String filename = Util.GetText( FilenameOffset, Eboot );

				System.IO.FileStream stream = System.IO.File.Create( dir.FullName + "/" + i.ToString( "D4" ) + ( nofilenames ? "" : "_" + filename ) );
				stream.Write( File, OffsetStart, ( OffsetEnd - OffsetStart ) );
				stream.Close();
			}

			return 0;
		}
	}
}
