using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.Scenario {
	public class Program {
		public static int ExecuteExtract( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: scenario.dat [outfolder]" );
				return -1;
			}

			string inPath = args[0];
			string outPath = args.Count > 1 ? args[1] : args[0] + ".ext";
			
			var scenario = new ScenarioDat( new System.IO.FileStream( inPath, System.IO.FileMode.Open ) );
			scenario.Extract( outPath );

			return 0;
		}
	}
}
