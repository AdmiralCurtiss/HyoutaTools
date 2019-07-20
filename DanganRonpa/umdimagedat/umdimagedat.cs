using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaUtils;

namespace HyoutaTools.DanganRonpa.umdimagedat {
	class umdimagedat {
		static int EbootHeaderSize;
		static int EbootUmddataBinSectionStart;
		static int EbootUmddata2binSectionStart;

		static byte[] ProjectDespairEbootMd5 = { 0x8a, 0x24, 0xec, 0x6b, 0x44, 0xe3, 0x8a, 0x53, 0x23, 0xa9, 0x13, 0x9b, 0x0f, 0xc2, 0x72, 0x55 };
		static byte[] WeirdTranslationEbootHeaderMd5 = { 0xB2, 0xFA, 0x42, 0x39, 0x1B, 0xB4, 0xA9, 0x3A, 0xE0, 0x0D, 0x1D, 0xA5, 0x33, 0x37, 0x7B, 0x41 };

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

			if ( EbootHeaderSize != 0xC0 && EbootHeaderSize != 0xA0 ) {
				return -1;
			}

			byte[] header = new byte[EbootHeaderSize];
			ArrayUtils.CopyByteArrayPart( Eboot, 0, header, 0, EbootHeaderSize );

			byte[] headermd5 = System.Security.Cryptography.MD5.Create().ComputeHash( header );
			byte[] md5 = System.Security.Cryptography.MD5.Create().ComputeHash( Eboot );

			if ( EbootHeaderSize == 0xC0 ) {
				if ( ArrayMatch( md5, ProjectDespairEbootMd5, 0, 0, 16 ) ) {
					EbootUmddataBinSectionStart = 0xF5A18 + 0x20;
					EbootUmddata2binSectionStart = 0xF5200 + 0x20;
				} else if ( ArrayMatch( headermd5, WeirdTranslationEbootHeaderMd5, 0, 0, 16 ) ) {
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
				EbootUmddata2binSectionStart = 0xF5200;
				return 2;
			}

			// unknown version
			return -1;
		}

		public static int Execute( List<string> args ) {
			if ( args.Count == 0 ) {
				Console.WriteLine( "extract without eboot (no filenames!):" );
				Console.WriteLine( " umdimagedat e umdimage.dat" );
				Console.WriteLine( "extract with eboot:" );
				Console.WriteLine( " umdimagedat e umdimage.dat eboot [-nofilenames]" );
				Console.WriteLine( "pack:" );
				Console.WriteLine( " umdimagedat p umdimage.dat.new umdimagedatdir eboot_orig eboot_new" );
				return -1;
			}

			if ( args[0] == "e" ) {
				if ( args.Count == 2 ) {
					return Extract( args[1] );
				} else {
					return Extract( args[1], args[2], args.Count > 3 && args[3] == "-nofilenames" );
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
				Console.WriteLine( "Failed identifying Eboot version! You may need to decrypt it first." );
				return -1;
			}

			int EbootSectionStart = EbootUmddataBinSectionStart;
			int HeaderAlignment = 0x4000;
			int FileAlignment = 1024;

			Console.WriteLine( "Reading File list..." );
			string[] Files = System.IO.Directory.GetFiles( Directory );
			List<string> flist = Files.ToList();
			flist.Sort();
			Files = flist.ToArray();

			if ( Files.Length == 0xAA ) { // pretty shoddy detection but hey
				EbootSectionStart = EbootUmddata2binSectionStart;
				HeaderAlignment = 0x40;
				FileAlignment = 0x40;
			}

			Console.WriteLine( "Opening Files and calculating sizes..." );
			List<FileStream> fslist = new List<FileStream>( Files.Length );
			List<int> filesizelist = new List<int>( Files.Length );
			List<int> alignedfilesizelist = new List<int>( Files.Length );
			int unalignedsize = 0;
			foreach ( string file in Files ) {
				FileStream fs = System.IO.File.OpenRead( file );
				unalignedsize = (int)fs.Length;


				int alignedsize = AlignToByteBoundary( unalignedsize, FileAlignment );
				fslist.Add( fs );

				filesizelist.Add( unalignedsize );
				alignedfilesizelist.Add( alignedsize );
			}
			alignedfilesizelist[alignedfilesizelist.Count - 1] = unalignedsize;

			int Headersize = AlignToByteBoundary( ( Files.Length + 1 ) * 4, HeaderAlignment );

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
				int alignedsize = alignedfilesizelist[i];
				byte[] in_filesize = BitConverter.GetBytes( size );
				byte[] offset = BitConverter.GetBytes( total );

				// write to header
				outs.Write( offset, 0, 4 );

				// write to eboot
				offset.CopyTo( Eboot, EbootSectionStart + 4 + ( i * 12 ) );
				in_filesize.CopyTo( Eboot, EbootSectionStart + 8 + ( i * 12 ) );

				total += alignedsize;
			}
			while ( outs.Position < Headersize ) {
				outs.WriteByte( 0x00 );
			}

			byte[] buffer = new byte[0x2000];

			// files
			Console.WriteLine( "Writing files..." );
			for ( int i = 0; i < fslist.Count; i++ ) {
				FileStream fs = fslist[i];
				int filesize = alignedfilesizelist[i];
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
				Console.WriteLine( "Failed identifying Eboot version! You may need to decrypt it first." );
				return -1;
			}

			int FileAmount = BitConverter.ToInt32( File, 0 );
			DirectoryInfo dir = Directory.CreateDirectory( filepath + ".ex" );
			int EbootSectionStart = EbootUmddataBinSectionStart;
			if ( FileAmount == 0xAA ) { // pretty shoddy detection but hey
				EbootSectionStart = EbootUmddata2binSectionStart;
			}

			for ( int i = 0; i < FileAmount; i++ ) {
				int FilenameOffset = BitConverter.ToInt32( Eboot, EbootSectionStart + 0 + ( i * 12 ) );
				FilenameOffset += EbootHeaderSize;
				int OffsetStart = BitConverter.ToInt32( Eboot, EbootSectionStart + 4 + ( i * 12 ) );
				int Filesize = BitConverter.ToInt32( Eboot, EbootSectionStart + 8 + ( i * 12 ) );
				int OffsetEnd = OffsetStart + Filesize;
				if ( OffsetEnd == 0 ) {
					OffsetEnd = File.Length;
				}

				String filename = TextUtils.GetTextAscii( Eboot, FilenameOffset );

				System.IO.FileStream stream = System.IO.File.Create( dir.FullName + "/" + i.ToString( "D4" ) + ( nofilenames ? "" : "_" + filename ) );
				stream.Write( File, OffsetStart, ( OffsetEnd - OffsetStart ) );
				stream.Close();
			}

			return 0;
		}
	}
}
