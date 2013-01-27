using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.MapList {
	class Program {
		public static int Execute( string[] args ) {
			// 0xCB20

			if ( args.Length != 1 ) {
				Console.WriteLine( "MAPLIST path/to/MAPLIST.DAT" );
				return -1;
			}

			String Path = args[0];

			byte[] Bytes = System.IO.File.ReadAllBytes( Path );


			int i = 0;

			bool print_rename = false;

			foreach ( MapName m in new MapList( Bytes ).MapNames ) {
				if ( !print_rename ) {
					Console.WriteLine( i + " = " + m.ToString() );
				} else {
					if ( m.Name3 != "dummy" ) {
						Console.WriteLine( "REN VScenario" + i + "_360.txt VScenario_360_" + m.Name3 + "_[" + i.ToString( "D4" ) + "].txt" );
					}
				}

				i++;
			}

			return 0;
		}
	}
}
