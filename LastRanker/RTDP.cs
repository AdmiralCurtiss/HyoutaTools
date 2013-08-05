using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.LastRanker {
	public class RTDPfile {
		public string Name;
		public uint Size;
		public uint Pointer;
	}

	public class RTDP {
		public static int ExecuteExtract( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: RTDPfile" );
				return -1;
			}

			new RTDP( System.IO.File.ReadAllBytes( args[0] ) ).GenerateFiles( args[0] + ".ex" );

			return 0;
		}
		public static int ExecutePack( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: Dir NewRTDPfile" );
				return -1;
			}


			new RTDP().CreateFromDirectory( args[0], args[1] );

			return 0;
		}

		public RTDP( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "RTDP: Load Failed!" );
			}
		}
		public RTDP( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "RTDP: Load Failed!" );
			}
		}
		public RTDP() { }

		public byte[] File;
		public List<RTDPfile> FileData;
		public byte Xorbyte;

		private bool LoadFile( byte[] File ) {
			this.File = File;
			FileData = new List<RTDPfile>();

			uint HeaderSize = BitConverter.ToUInt32( File, 0x04 );
			uint FileCount = BitConverter.ToUInt32( File, 0x08 );
			uint FileSize = BitConverter.ToUInt32( File, 0x0C );
			Xorbyte = File[0x10];

			for ( int i = 0; i < FileCount; ++i ) {
				RTDPfile r = new RTDPfile();
				r.Name = Encoding.ASCII.GetString( File, i * 0x28 + 0x20, 0x20 ).TrimEnd( '\0' );
				r.Size = BitConverter.ToUInt32( File, i * 0x28 + 0x20 + 0x20 );
				r.Pointer = BitConverter.ToUInt32( File, i * 0x28 + 0x20 + 0x24 ) + HeaderSize;
				FileData.Add( r );
			}

			return true;
		}

		public void CreateFromDirectory( string Dir, string Outfile ) {
			string[] Filepaths = System.IO.Directory.GetFiles( Dir );
			var Filestream = new System.IO.FileStream( Outfile, System.IO.FileMode.Create );


			uint RequiredBytesForHeader;
			uint Filecount = (uint)Filepaths.Length;
			uint TotalFilesize;
			Xorbyte = 0x55;
			//Xorbyte = 0x00;

			// 0x20 Header + 0x28 per file
			RequiredBytesForHeader = Util.Align( Filecount * 0x28u + 0x20u, 0x20u );

			TotalFilesize = RequiredBytesForHeader;
			foreach ( string Path in Filepaths ) {
				TotalFilesize += (uint)( new System.IO.FileInfo( Path ).Length );
				TotalFilesize = Util.Align( TotalFilesize, 0x20u );
			}

			// header
			Filestream.WriteByte( (byte)'R' ); Filestream.WriteByte( (byte)'T' );
			Filestream.WriteByte( (byte)'D' ); Filestream.WriteByte( (byte)'P' );
			Filestream.Write( BitConverter.GetBytes( RequiredBytesForHeader ), 0, 4 );
			Filestream.Write( BitConverter.GetBytes( Filecount ), 0, 4 );
			Filestream.Write( BitConverter.GetBytes( TotalFilesize ), 0, 4 );
			Filestream.WriteByte( Xorbyte );
			Filestream.WriteByte( 0x28 ); Filestream.WriteByte( 0x25 );
			while ( Filestream.Length < 0x20 ) { Filestream.WriteByte( 0x00 ); }

			// header file info
			uint ptr = 0;
			foreach ( string Path in Filepaths ) {
				var fi = new System.IO.FileInfo( Path );
				uint size = (uint)( fi.Length );

				byte[] name = Encoding.ASCII.GetBytes( fi.Name );
				Filestream.Write( name, 0, Math.Min( 0x20, name.Length ) );
				for ( int i = name.Length; i < 0x20; ++i ) { Filestream.WriteByte( 0x00 ); }

				Filestream.Write( BitConverter.GetBytes( size ), 0, 4 );
				Filestream.Write( BitConverter.GetBytes( ptr ), 0, 4 );

				ptr = Util.Align( ptr + size, 0x20u );
			}
			while ( Filestream.Length < RequiredBytesForHeader ) { Filestream.WriteByte( 0x00 ); }

			// files
			foreach ( string Path in Filepaths ) {
				var File = new System.IO.FileStream( Path, System.IO.FileMode.Open );

				while ( true ) {
					int b = File.ReadByte();
					if ( b == -1 ) { break; }
					Filestream.WriteByte( (byte)( b ^ Xorbyte ) );
				}

				File.Close();
				while ( Filestream.Length % 0x20 != 0 ) { Filestream.WriteByte( 0x00 ); }
			}

			Filestream.Close();

		}

		public void GenerateFiles( String Outdir ) {
			System.IO.Directory.CreateDirectory( Outdir );
			for ( int i = 0; i < FileData.Count; ++i ) {
				RTDPfile r = FileData[i];
				uint start = r.Pointer;
				uint end = r.Pointer + r.Size;
				uint count = r.Size;

				string outfilename = r.Name;

				string outpath = System.IO.Path.Combine( Outdir, outfilename );
				var fs = new System.IO.FileStream( outpath, System.IO.FileMode.Create );

				for ( int j = 0; j < count; ++j ) {
					fs.WriteByte( (byte)( File[start + j] ^ Xorbyte ) );
				}
				fs.Close();
			}
		}
	}
}
