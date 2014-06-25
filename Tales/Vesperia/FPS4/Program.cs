using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.FPS4 {
	public class Program {
		public static int Extract( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: file.svo [OutputDirectory] [-nometa]" );
				return -1;
			}

			string inFile = args[0];
			string outFile = args.Count >= 2 ? args[1] : inFile + ".ext";
			bool nometa = args.Count >= 3 ? args[2] == "-nometa" : false;

			new FPS4( inFile ).Extract( outFile, noMetadataParsing: nometa );

			return 0;
		}

		public static void PrintPackUsage() {
			Console.WriteLine( "Usage: [args] DirectoryToPack OutputFilename" );
			Console.WriteLine( "Possible Arguments:" );
			Console.WriteLine( " -a alignment           Default: 0x0800" );
			Console.WriteLine( "   Align files within the container to a specific boundary." );
			Console.WriteLine( " -b bitmask             Default: 0x000F" );
			Console.WriteLine( "   Set the bitmask to a specific value, to in- or exclude header data." );
			Console.WriteLine( " -m metadata            Default: none" );
			Console.WriteLine( "   Specify which metadata to write if the bitmask uses bit 0x0040." );
			//Console.WriteLine( "   f    filename" );
			Console.WriteLine( "   Combine multiple letters to write multiple." );
			Console.WriteLine( " -o filename.svo        Default: none" );
			Console.WriteLine( "   Read header data from another FPS4 file and use it in the new one." );
			Console.WriteLine( "   Do NOT use this when filenames change or files are added/removed." );
		}
		public static int Pack( List<string> args ) {
			if ( args.Count < 2 ) {
				PrintPackUsage();
				return -1;
			}

			string dir = null;
			string outName = null;
			
			ushort? bitmask = null;
			uint? alignment = null;
			string originalFps4 = null;
			string metadata = null;

			try {
				for ( int i = 0; i < args.Count; ++i ) {
					switch ( args[i] ) {
						case "-a":
							alignment = Util.ParseDecOrHex( args[++i] );
							break;
						case "-b":
							bitmask = (ushort)Util.ParseDecOrHex( args[++i] );
							break;
						case "-m":
							metadata = args[++i];
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
				fps4 = new FPS4();
			}

			if ( bitmask != null ) { fps4.ContentBitmask = (ushort)bitmask; }
			if ( alignment != null ) { fps4.Alignment = (uint)alignment; }
			
			fps4.Pack( dir, outName, metadata );

			return 0;
		}
	}
}
