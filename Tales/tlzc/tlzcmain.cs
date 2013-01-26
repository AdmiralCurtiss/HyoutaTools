using System;
using System.IO;

namespace HyoutaTools.Tales.tlzc {
	class tlzcmain {
		public static int Execute( string[] args ) {

			String Usage = "usage: tlzc [-c/-d] infile outfile";

			bool ForceDecompress = false;
			bool ForceCompress = false;
			String Filename;
			String FilenameOut = null;

			if ( args.Length < 1 ) {
				Console.WriteLine( Usage );
				return -1;
			}

			// the most convoluted argument checking
			if ( args[0] == "-c" || args[0] == "-d" ) {
				if ( args.Length < 2 ) {
					Console.WriteLine( Usage );
					return -1;
				}
				if ( args[0] == "-c" ) ForceCompress = true;
				else ForceDecompress = true;
				Filename = args[1];
				if ( args.Length > 2 ) {
					FilenameOut = args[2];
				}
			} else {
				Filename = args[0];
				if ( args.Length > 1 ) {
					FilenameOut = args[1];
				}
			}
			// args end

			byte[] input = File.ReadAllBytes( Filename );
			byte[] output;

			if
			(
				( ForceDecompress || ( input[0] == 'T' && input[1] == 'L' && input[2] == 'Z' && input[3] == 'C' ) )
				&& !ForceCompress
			) {
				try {
					Console.WriteLine( "decompressing {0}", Filename );
					output = TLZC.Decompress( input );
					if ( FilenameOut == null ) FilenameOut = Filename + ".dec";
					File.WriteAllBytes( FilenameOut, output );
				} catch ( Exception ex ) {
					Console.WriteLine( "Decompression failed: " + ex.ToString() );
					return -1;
				}
			} else {
				try {
					Console.WriteLine( "compressing {0}", Filename );
					output = TLZC.Compress( input, 4 );
					if ( FilenameOut == null ) FilenameOut = Filename + ".tlzc";
					File.WriteAllBytes( FilenameOut, output );
				} catch ( Exception ex ) {
					Console.WriteLine( "Decompression failed: " + ex.ToString() );
					return -1;
				}
			}
			return 0;
		}
	}
}