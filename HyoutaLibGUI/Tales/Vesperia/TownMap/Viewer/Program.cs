using HyoutaTools.Tales.Vesperia.TownMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaLibGUI.Tales.Vesperia.TownMap.Viewer {
	static class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 2 ) {
				Console.WriteLine( "Usage: TownMapViewer scenario_0.bin folder_with_U_MAP_images" );
				return -1;
			}

			string scenario0path = args[0];
			string imagepath = args[1];

			TownMapTable t = new TownMapTable( scenario0path );
			new TownMapDisplay( t, imagepath ).Show();

			return 0;
		}
	}
}
