using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Generic.ByteSwap {
	class ByteSwap {
		public static int Execute( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: ByteSwap filename [bytes-per-block, default 4]" );
				return -1;
			}

			byte[] File;
			try {
				File = System.IO.File.ReadAllBytes( args[0] );
			} catch ( Exception ) {
				Console.WriteLine( "Can't open File " + args[0] );
				return -1;
			}

			int BytesPerBlock;
			try { BytesPerBlock = Int32.Parse( args[1] ); } catch ( Exception ) { BytesPerBlock = 4; }

			int OutByteCount = File.Length.Align( BytesPerBlock );
			byte[] OutFile = new byte[OutByteCount];
			for ( int i = 0; i < OutByteCount; i += BytesPerBlock ) {
				for ( int j = 0; j < BytesPerBlock; ++j ) {
					OutFile[i + j] = File[( i + BytesPerBlock ) - ( j + 1 )];
				}
			}


			System.IO.File.WriteAllBytes( args[0], OutFile );

			return 0;
		}
	}
}
