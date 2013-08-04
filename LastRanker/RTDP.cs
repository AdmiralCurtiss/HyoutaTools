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
