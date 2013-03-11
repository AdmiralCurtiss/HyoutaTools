using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Abyss.SB7 {
	class DumpText {
		public static int Execute( List<string> args ) {
			string infile = args[0];
			string outfile = args.Count >= 2 ? args[1] : args[0] + ".txt";

			System.IO.File.WriteAllLines( outfile, new SB7( infile ).Texts.ToArray() );
			return 0;
		}
	}
}
