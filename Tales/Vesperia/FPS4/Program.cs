using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.FPS4 {
	public class Program {
		public static int Extract( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: file.svo [OutputDirectory]" );
				return -1;
			}

			string inFile = args[0];
			string outFile = args.Count >= 2 ? args[1] : inFile + ".ext";

			new FPS4( inFile ).Extract( outFile );

			return 0;
		}

		public static void PrintPackUsage() {
			Console.WriteLine( "Usage: [args] DirectoryToPack OutputFilename" );
			Console.WriteLine( "Possible Arguments:" );
			Console.WriteLine( " -b bitmask             Default: 0x000F" );
			Console.WriteLine( "   Set the bitmask to a specific value, to in- or exclude header data." );
			Console.WriteLine( " -o filename.svo        Default: none" );
			Console.WriteLine( "   Read header data from another FPS4 file and use it in the new one." );
		}
		public static int Pack( List<string> args ) {
			if ( args.Count < 2 ) {
				PrintPackUsage();
				return -1;
			}

			string dir = null;
			string outName = null;
			ushort bitmask = 0x000F;
			string originalFps4 = null;

			try {
				for ( int i = 0; i < args.Count; ++i ) {
					switch ( args[i] ) {
						case "-b":
							bitmask = (ushort)Util.ParseDecOrHexToByte( args[++i] );
							break;
						case "-o":
							originalFps4 = args[++i];
							break;
						default:
							if ( dir == null ) { dir = args[i]; } else if ( outName == null ) { outName = args[i]; } else { PrintPackUsage(); return -1; }
							break;
					}
				}
			} catch ( IndexOutOfRangeException ) {
				PrintPackUsage();
				return -1;
			}

			if ( dir == null || outName == null ) {
				PrintPackUsage();
				return -1;
			}

			FPS4 fps4;
			if ( originalFps4 != null ) {
				fps4 = new FPS4( originalFps4 );
			} else {
				fps4 = new FPS4( bitmask );
			}
			
			fps4.Pack( dir, outName );

			return 0;
		}
	}
}
