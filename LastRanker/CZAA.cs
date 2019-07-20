using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaUtils;

namespace HyoutaTools.LastRanker {
	class CZAA {
		public static int ExecuteExtract( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: infile outfile" );
				return -1;
			}

			System.IO.File.WriteAllBytes( args[1], new CZAA( args[0] ).ExtractedFile );

			return 0;
		}
		public static int ExecutePack( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: infile outfile" );
				return -1;
			}

			CZAA c = new CZAA();
			System.IO.File.WriteAllBytes( args[1], c.CompressFile( System.IO.File.ReadAllBytes( args[0] ) ) );

			return 0;
		}

		public CZAA( String filename ) {
			if ( !LoadFile( new System.IO.FileStream( filename, System.IO.FileMode.Open ) ) ) {
				throw new Exception( "CZAA: Load Failed!" );
			}
		}
		public CZAA( System.IO.Stream Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "CZAA: Load Failed!" );
			}
		}
		public CZAA() { }

		public byte[] ExtractedFile;

		private bool LoadFile( System.IO.Stream File ) {
			byte[] outsizebytes = new byte[4];
			File.Seek( 0x04, System.IO.SeekOrigin.Begin );
			File.Read( outsizebytes, 0, 4 );
			uint OutSize = BitConverter.ToUInt32( outsizebytes, 0 );

			byte[] compFile = new byte[(int)File.Length - 0x10];
			File.Seek( 0x10, System.IO.SeekOrigin.Begin );
			File.Read( compFile, 0, (int)File.Length - 0x10 );

			byte[] decompFile = Ionic.Zlib.ZlibStream.UncompressBuffer( compFile );
			ExtractedFile = decompFile;

			File.Close();
			return true;
		}

		public byte[] CompressFile( byte[] File ) {
			byte[] comp = Ionic.Zlib.ZlibStream.CompressBuffer( File );
			byte[] NewFile = new byte[comp.Length + 0x10];

			NewFile[0] = (byte)'C'; NewFile[1] = (byte)'Z'; NewFile[2] = (byte)'A'; NewFile[3] = (byte)'A';
			ArrayUtils.CopyByteArrayPart( BitConverter.GetBytes( (uint)File.Length ), 0, NewFile, 4, 4 );
			ArrayUtils.CopyByteArrayPart( comp, 0, NewFile, 0x10, comp.Length );

			return NewFile;
		}
	}
}
