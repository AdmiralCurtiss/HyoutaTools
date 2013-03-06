using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PSP.GIM.HomogenizePalette {
	class Program {
		public static int Homogenize( List<string> args ) {
			if ( args.Count == 0 ) {
				Console.WriteLine( "HomogenizePalette in.gim [out.gim]" );
				Console.WriteLine( "Overwrites in.gim when no out.gim is provided." );
			}	

			string infilename = args[0];
			string outfilename = args.Count > 1 ? args[1] : args[0];
			
			GIM gim = new GIM ( infilename );
			gim.HomogenizePalette();
			System.IO.File.WriteAllBytes( outfilename, gim.Serialize() );


			return 0;
		}
	}
}
