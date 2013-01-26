using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.NisPakEx {
	class Program {
		static void Execute( string[] args ) {
			string Filename = @"c:\Users\Georg\Music\disgaea4\disg4.pak";
			string OutPath = @"c:\Users\Georg\Music\disgaea4\";

			Console.WriteLine( "Opening " + Filename + "..." );

			byte[] b = System.IO.File.ReadAllBytes( Filename );

			uint FileAmount = BitConverter.ToUInt32( b, 0 );

			for ( int i = 0; i < FileAmount; i++ ) {
				uint Offset = BitConverter.ToUInt32( b, ( i * 8 ) + 4 );
				uint FileSize = BitConverter.ToUInt32( b, ( i * 8 ) + 8 );
				String f = OutPath + "0x" + Offset.ToString( "X8" ) + ".ex";

				Console.WriteLine( "Writing " + f + "... (" + ( i + 1 ).ToString() + "/" + FileAmount.ToString() + ")" );

				System.IO.FileStream fs =
					new System.IO.FileStream( f, System.IO.FileMode.Create );
				fs.Write( b, (int)Offset, (int)FileSize );
			}
		}
	}
}
