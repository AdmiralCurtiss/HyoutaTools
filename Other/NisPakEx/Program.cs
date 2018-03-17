using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.NisPakEx {
	class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 2 ) {
				Console.WriteLine( "Usage: NisPakEx data.pak outdir" );
				return -1;
			}
			string Filename = args[0];
			string OutPath = args[1];

			Console.WriteLine( "Opening " + Filename + "..." );

			byte[] b = System.IO.File.ReadAllBytes( Filename );

			uint FileAmount = BitConverter.ToUInt32( b, 0 );

			System.IO.Directory.CreateDirectory( OutPath );
			for ( int i = 0; i < FileAmount; i++ ) {
				uint Offset = BitConverter.ToUInt32( b, ( i * 8 ) + 4 );
				uint FileSize = BitConverter.ToUInt32( b, ( i * 8 ) + 8 );
				String f = System.IO.Path.Combine( OutPath , "0x" + Offset.ToString( "X8" ) + ".ex" );

				Console.WriteLine( "Writing " + f + "... (" + ( i + 1 ).ToString() + "/" + FileAmount.ToString() + ")" );

				System.IO.FileStream fs =
					new System.IO.FileStream( f, System.IO.FileMode.Create );
				fs.Write( b, (int)Offset, (int)FileSize );
			}

			return 0;
		}
	}
}
