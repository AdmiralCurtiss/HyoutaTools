using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;
using NumberUtils = HyoutaUtils.NumberUtils;

namespace HyoutaTools.LastRanker {
	class NPK {
		public static int ExecuteExtract( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: NPKfile" );
				return -1;
			}

			new NPK( System.IO.File.ReadAllBytes( args[0] ) ).GenerateFiles( args[0] + ".ex" );

			return 0;
		}
		public static int ExecutePack( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: Dir NewNPKfile" );
				return -1;
			}

			new NPK().CreateFromDirectory( args[0], args[1] );

			return 0;
		}

		public NPK() { }

		public NPK( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "NPK: Load Failed!" );
			}
		}
		public NPK( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "NPK: Load Failed!" );
			}
		}

		public byte[] File;
		public List<uint> FilePointers;

		private bool LoadFile( byte[] File ) {
			this.File = File;
			FilePointers = new List<uint>();

			int FileCount = BitConverter.ToUInt16( File, 0 );
			for ( int i = 0; i < FileCount + 1; ++i ) {
				uint ptr = NumberUtils.ToUInt24( File, i * 3 + 2 );
				FilePointers.Add( ptr );
			}

			return true;
		}

		public void GenerateFiles( String Outdir ) {
			System.IO.Directory.CreateDirectory( Outdir );
			for ( int i = 0; i < FilePointers.Count - 1; ++i ) {
				uint start = FilePointers[i].Align( 0x10 );
				uint end = FilePointers[i + 1];
				uint count = end - start;

				string outfilename = i.ToString( "D4" );

				string outpath = System.IO.Path.Combine( Outdir, outfilename );
				var fs = new System.IO.FileStream( outpath, System.IO.FileMode.Create );

				fs.Write( File, (int)start, (int)count );
				fs.Close();
			}
		}

		public void CreateFromDirectory( string Dir, string Outfile ) {
			string[] Filepaths = System.IO.Directory.GetFiles( Dir );

			ushort Filecount = (ushort)Filepaths.Length;
			uint RequiredBytesForHeader = NumberUtils.Align( Filecount * 3u + 5u, 0x10u ); // 3 bytes per filesize + 3 bytes for an extra pointer to first file + 2 bytes for filecount
			var Filestream = new System.IO.FileStream( Outfile, System.IO.FileMode.Create );

			// header
			Filestream.Write( BitConverter.GetBytes( Filecount ), 0, 2 );
			Filestream.Write( NumberUtils.GetBytesForUInt24( RequiredBytesForHeader ), 0, 3 );
			uint TotalFilesize = RequiredBytesForHeader;
			foreach ( string Path in Filepaths ) {
				TotalFilesize += (uint)( new System.IO.FileInfo( Path ).Length );
				Filestream.Write( NumberUtils.GetBytesForUInt24( TotalFilesize ), 0, 3 );
				TotalFilesize = TotalFilesize.Align( 0x10u );
			}
			while ( Filestream.Length < RequiredBytesForHeader ) { Filestream.WriteByte( 0x00 ); }

			// files
			foreach ( string Path in Filepaths ) {
				var File = new System.IO.FileStream( Path, System.IO.FileMode.Open );
				StreamUtils.CopyStream( File, Filestream, (int)File.Length );
				File.Close();
				while ( Filestream.Length % 0x10 != 0 ) { Filestream.WriteByte( 0x00 ); }
			}

			Filestream.Close();
		}
	}
}
