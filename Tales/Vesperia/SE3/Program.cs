using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.SE3 {
	public class Program {
		public static int ExtractToNub( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage SE3toNUB in.se3 out.nub" );
				return -1;
			}

			string infile = args[0];
			string outfile = args.Count >= 2 ? args[1] : infile + ".nub";

			new SE3( infile, null, Util.GameTextEncoding.ASCII ).ExtractToNub( outfile );

			return 0;
		}
	}
}
