using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace HyoutaTools.Other.GZip {
	class GZipHandler {
		public static int ExecuteExtract( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: infile outfile" );
				return -1;
			}

			Extract( new FileStream( args[0], FileMode.Open ), args[1] );

			return 0;
		}
		public static int ExecuteCompress( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: infile outfile" );
				return -1;
			}

			Compress( new FileStream( args[0], FileMode.Open ), args[1] );

			return 0;
		}

		public static void Extract( FileStream fs, string outpath ) {
			fs.Position = 0;
			var outfile = new FileStream( outpath, System.IO.FileMode.Create );
			using ( var decompressed = new GZipStream( fs, CompressionMode.Decompress ) ) {
				byte[] buffer = new byte[4096];
				int numRead;
				while ( ( numRead = decompressed.Read( buffer, 0, buffer.Length ) ) != 0 ) {
					outfile.Write( buffer, 0, numRead );
				}
			}
			outfile.Close();
		}

		public static void Compress( FileStream fs, string outpath ) {
			fs.Position = 0;
			var outfile = new FileStream( outpath, System.IO.FileMode.Create );
			using ( var compressed = new Ionic.Zlib.GZipStream( outfile, Ionic.Zlib.CompressionMode.Compress ) ) {
				byte[] buffer = new byte[4096];
				int numRead;
				while ( ( numRead = fs.Read( buffer, 0, buffer.Length ) ) != 0 ) {
					compressed.Write( buffer, 0, numRead );
				}
			}
			outfile.Close();
		}
	}
}
