using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
				uint ptr = Util.ToUInt24( File, i * 3 + 2 );
				FilePointers.Add( ptr );
			}

			return true;
		}

		public void GenerateFiles( String Outdir ) {
			System.IO.Directory.CreateDirectory( Outdir );
			for ( int i = 0; i < FilePointers.Count - 1; ++i ) {
				uint start = Util.Align( FilePointers[i], 0x10 );
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
			uint RequiredBytesForHeader = Util.Align( Filecount * 3u + 2u, 0x10u );
			var Filestream = new System.IO.FileStream( Outfile, System.IO.FileMode.Create );

			// header
			int CurrentLocation = 0;
			Filestream.Write( BitConverter.GetBytes( Filecount ), CurrentLocation, 2 ); CurrentLocation += 2;
			Filestream.Write( Util.GetBytesForUInt24( RequiredBytesForHeader ), CurrentLocation, 3 ); CurrentLocation += 3;
			uint TotalFilesize = RequiredBytesForHeader;
			foreach ( string Path in Filepaths ) {
				TotalFilesize += (uint)( new System.IO.FileInfo( Path ).Length );
				Filestream.Write( Util.GetBytesForUInt24( TotalFilesize ), CurrentLocation, 3 ); CurrentLocation += 3;
				TotalFilesize = Util.Align( TotalFilesize, 0x10u );
			}
			while ( Filestream.Length < RequiredBytesForHeader ) { Filestream.WriteByte( 0x00 ); }

			// files
			foreach ( string Path in Filepaths ) {
				var File = new System.IO.FileStream( Path, System.IO.FileMode.Open );
				Util.CopyStream( File, Filestream, (int)File.Length );
				File.Close();
				while ( Filestream.Length % 0x10 != 0 ) { Filestream.WriteByte( 0x00 ); }
			}

			Filestream.Close();
		}
	}
}
