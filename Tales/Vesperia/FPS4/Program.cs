using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.FPS4 {
	public class Program {
		public static void PrintExtractUsage() {
			Console.WriteLine( "Usage: file.svo [-h file.header] [OutputDirectory] [-nometa]" );
		}
		public static int Extract( List<string> args ) {
			string inFile = null;
			string headerFile = null;
			string outFile = null;
			bool nometa = false;

			try {
				for ( int i = 0; i < args.Count; ++i ) {
					switch ( args[i] ) {
						case "-h":
							headerFile = args[++i];
							break;
						case "-nometa":
							nometa = true;
							break;
						default:
							if ( inFile == null ) { inFile = args[i]; }
							else if ( outFile == null ) { outFile = args[i]; }
							else { PrintExtractUsage(); return -1; }
							break;
					}
				}
			} catch ( IndexOutOfRangeException ) {
				PrintExtractUsage();
				return -1;
			}

			if ( inFile == null ) {
				PrintExtractUsage();
				return -1;
			}

			if ( outFile == null ) {
				outFile = inFile + ".ext";
			}

			FPS4 fps4;
			if ( headerFile != null ) {
				fps4 = new FPS4( headerFile, inFile );
			} else {
				fps4 = new FPS4( inFile );
			}
			fps4.Extract( outFile, noMetadataParsing: nometa );

			return 0;
		}

		public static void PrintPackUsage() {
			Console.WriteLine( "Usage: [args] DirectoryToPack OutputFilename" );
			Console.WriteLine( "Possible Arguments:" );
			Console.WriteLine( " -a alignment           Default: 0x0800" );
			Console.WriteLine( "   Align files within the container to a specific boundary." );
			Console.WriteLine( " -b bitmask             Default: 0x000F" );
			Console.WriteLine( "   Set the bitmask to a specific value, to in- or exclude header data." );
			Console.WriteLine( " -l" );
			Console.WriteLine( "   Write headers etc. as little endian instead of big endian." );
			Console.WriteLine( " -m metadata            Default: none" );
			Console.WriteLine( "   Specify which metadata to write if the bitmask uses bit 0x0040." );
			Console.WriteLine( "    p    filepath" );
			Console.WriteLine( "    n    filename" );
			Console.WriteLine( "   Combine multiple letters to write multiple." );
			Console.WriteLine( " -e" );
			Console.WriteLine( "   Order the files in the archive by extension instead of name." );
			Console.WriteLine( " -s" );
			Console.WriteLine( "   Include subdirectories of DirectoryToPack as well." );
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
			bool orderByExtension = false;
			bool includeSubdirs = false;
			bool littleEndian = false;
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
						case "-l":
							littleEndian = true;
							break;
						case "-m":
							metadata = args[++i];
							break;
						case "-e":
							orderByExtension = true;
							break;
						case "-s":
							includeSubdirs = true;
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
			if ( littleEndian ) { fps4.Endian = Util.Endianness.LittleEndian; }

			string[] files;
			if ( includeSubdirs ) {
				files = System.IO.Directory.GetFiles( dir, "*", System.IO.SearchOption.AllDirectories );
			} else {
				files = System.IO.Directory.GetFiles( dir );
			}

			if ( orderByExtension ) {
				files = files.OrderBy( x => x.Split( '.' ).Last() ).ToArray();
			}

			fps4.Pack( files, outName, metadata );

			return 0;
		}
	}
}
