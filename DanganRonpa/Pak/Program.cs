using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaUtils;

namespace HyoutaTools.DanganRonpa.Pak {
	class Program {

		public static void Extract( FileStream file, String destination ) {
			System.IO.Directory.CreateDirectory( destination );

			byte[] buffer = new byte[4];
			file.Read( buffer, 0, 4 );
			int count = BitConverter.ToInt32( buffer, 0 );

			int[] FileOffsets = new int[count + 1];
			for ( int i = 0; i < count; ++i ) {
				file.Read( buffer, 0, 4 );
				FileOffsets[i] = BitConverter.ToInt32( buffer, 0 );
			}
			FileOffsets[count] = (int)file.Length;

			for ( int i = 0; i < count; ++i ) {
				FileStream newFile = new FileStream( System.IO.Path.Combine( destination, i.ToString( "D4" ) ), FileMode.Create );
				file.Position = FileOffsets[i];
				StreamUtils.CopyStream( file, newFile, FileOffsets[i + 1] - FileOffsets[i] );
				newFile.Close();
			}

			return;
		}

		static int Align( int value, int alignment ) {
			int diff = value % alignment;
			if ( diff == 0 ) {
				return value;
			} else {
				return ( value + ( alignment - diff ) );
			}
		}

		static void Pack( List<String> files, string outfile, string ebootname ) {
			files.Sort();

			bool useEboot = ebootname != null;

			List<int> FileLocations = null;
			List<int> FileSizes = null;
			if ( useEboot ) {
				FileLocations = new List<int>( files.Count );
				FileSizes = new List<int>( files.Count );
			}

			uint byteswritten = 0;
			FileStream newFile = new FileStream( outfile, FileMode.Create );
			newFile.Write( BitConverter.GetBytes( files.Count ), 0, 4 );
			byteswritten += 4;

			int firstfilestart = Align( files.Count * 4 + 4, 0x10 );
			int filestart = firstfilestart;
			for ( int i = 0; i < files.Count; ++i ) {
				newFile.Write( BitConverter.GetBytes( filestart ), 0, 4 );
				byteswritten += 4;

				int size = (int)new System.IO.FileInfo( files[i] ).Length;
				if ( useEboot ) {
					FileLocations.Add( filestart );
					FileSizes.Add( size );
				}

				filestart += size;
			}

			while ( byteswritten < firstfilestart ) {
				newFile.WriteByte( 0x00 );
				byteswritten++;
			}

			for ( int i = 0; i < files.Count; ++i ) {
				FileStream f = new System.IO.FileStream( files[i], FileMode.Open );
				StreamUtils.CopyStream( f, newFile, (int)f.Length );
				f.Close();
			}

			newFile.Close();

			if ( useEboot ) {
				// write vals to eboot
				byte[] eboot = System.IO.File.ReadAllBytes( ebootname );

				int currentEbootLoc = 0xF5200;
				for ( int i = 0; i < FileLocations.Count; ++i ) {
					byte[] loc = BitConverter.GetBytes( FileLocations[i] );
					byte[] siz = BitConverter.GetBytes( FileSizes[i] );

					loc.CopyTo( eboot, currentEbootLoc + 4 );
					siz.CopyTo( eboot, currentEbootLoc + 8 );

					currentEbootLoc += 12;
				}

				System.IO.File.WriteAllBytes( ebootname, eboot );
			}
		}

		public static int ExecuteExtract( List<string> args ) {
			if ( args.Count == 0 ) {
				Console.WriteLine( "DanganRonpa.Pak.Extract file.pak [file2.pak ...]" );
				return -1;
			}

			bool fail = false;
			foreach ( string arg in args ) {
				try {
					Extract( new FileStream( arg, FileMode.Open ), arg + ".ex" );
				} catch ( Exception ) {
					Console.WriteLine( "Failed on " + arg );
					fail = true;
				}
			}
			return fail ? -1 : 0;
		}

		public static int ExecutePack( List<string> args ) {
			try {
				Pack( System.IO.Directory.GetFiles( args[0] ).ToList(), args[1], args.Count >= 3 ? args[2] : null );
			} catch ( Exception ) {
				Console.WriteLine( "DanganRonpa.Pak.Pack directory outfile.pak" );
				return -1;
			}
			return 0;
		}
	}
}
