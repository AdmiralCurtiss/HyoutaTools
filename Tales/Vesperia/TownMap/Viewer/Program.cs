using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.Tales.Vesperia.TownMap.Viewer {
	static class Program {
		public static int Execute( List<string> args ) {
			string Path = @"../../0.bin";
			TownMapTable t = new TownMapTable( Path );

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			Application.Run( new TownMapDisplay( t ) );

			return 0;
		}
	}
}
