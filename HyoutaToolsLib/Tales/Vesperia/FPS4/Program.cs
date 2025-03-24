using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HyoutaUtils;
using HyoutaUtils.Streams;

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
				fps4 = new FPS4(headerFile, inFile, printProgressToConsole: true);
			} else {
				fps4 = new FPS4(inFile, printProgressToConsole: true);
			}
			fps4.Extract(outFile, noMetadataParsing: nometa, printProgressToConsole: true);

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
			Console.WriteLine( " -c comment             Default: none" );
			Console.WriteLine( "   Write a comment at the end of the header, unclear what this string actually means." );
			Console.WriteLine( "   This input uses escape sequences." );
			Console.WriteLine( " -e" );
			Console.WriteLine( "   Order the files in the archive by extension instead of name." );
			Console.WriteLine( " -s" );
			Console.WriteLine( "   Include subdirectories of DirectoryToPack as well." );
			Console.WriteLine( " -h filename.svo        Default: none" );
			Console.WriteLine( "   Write header to a different file, only actual data goes to OutputFilename." );
			Console.WriteLine( " -o filename.svo        Default: none" );
			Console.WriteLine( "   Read header data from another FPS4 file and use it in the new one." );
			Console.WriteLine( "   Do NOT use this when filenames change or files are added/removed." );
			Console.WriteLine( " --firstalign value     Default: same as file alignment" );
			Console.WriteLine( "   Align first file within the container to a different offset than the rest." );
			Console.WriteLine( " --multiplier value     Default: 1" );
			Console.WriteLine( "   Apply a multiplier to the file offsets." );
			Console.WriteLine( "   This is necessary when packing over 4 GB of data." );
			Console.WriteLine( "   This should ideally be a power of two." );
			Console.WriteLine( "   The file alignment must divide by this multiplier." );
		}

		// from http://stackoverflow.com/a/25471811
		private static string UnEscape( string s ) {
			StringBuilder sb = new StringBuilder();
			Regex r = new Regex( "\\\\[abfnrtv?\"'\\\\]|\\\\[0-3]?[0-7]{1,2}|\\\\u[0-9a-fA-F]{4}|\\\\U[0-9a-fA-F]{8}|." );
			MatchCollection mc = r.Matches( s, 0 );

			foreach ( Match m in mc ) {
				if ( m.Length == 1 ) {
					sb.Append( m.Value );
				} else {
					if ( m.Value[1] >= '0' && m.Value[1] <= '7' ) {
						int i = Convert.ToInt32( m.Value.Substring( 1 ), 8 );
						sb.Append( (char)i );
					} else if ( m.Value[1] == 'u' ) {
						int i = Convert.ToInt32( m.Value.Substring( 2 ), 16 );
						sb.Append( (char)i );
					} else if ( m.Value[1] == 'U' ) {
						int i = Convert.ToInt32( m.Value.Substring( 2 ), 16 );
						sb.Append( Char.ConvertFromUtf32( i ) );
					} else {
						switch ( m.Value[1] ) {
							case 'a':
								sb.Append( '\a' );
								break;
							case 'b':
								sb.Append( '\b' );
								break;
							case 'f':
								sb.Append( '\f' );
								break;
							case 'n':
								sb.Append( '\n' );
								break;
							case 'r':
								sb.Append( '\r' );
								break;
							case 't':
								sb.Append( '\t' );
								break;
							case 'v':
								sb.Append( '\v' );
								break;
							default:
								sb.Append( m.Value[1] );
								break;
						}
					}
				}
			}

			return sb.ToString();
		}

		public static int Pack( List<string> args ) {
			if ( args.Count < 2 ) {
				PrintPackUsage();
				return -1;
			}

			string dir = null;
			string outName = null;
			string outHeaderName = null;

			ushort? bitmask = null;
			uint? alignment = null;
			uint? alignmentFirstFile = null;
			bool orderByExtension = false;
			bool includeSubdirs = false;
			bool littleEndian = false;
			string originalFps4 = null;
			string metadata = null;
			string comment = null;
			uint multiplier = 1;

			try {
				for ( int i = 0; i < args.Count; ++i ) {
					switch ( args[i] ) {
						case "-a":
							alignment = HexUtils.ParseDecOrHex( args[++i] );
							break;
						case "--firstalign":
							alignmentFirstFile = HexUtils.ParseDecOrHex( args[++i] );
							break;
						case "-b":
							bitmask = (ushort)HexUtils.ParseDecOrHex( args[++i] );
							break;
						case "-l":
							littleEndian = true;
							break;
						case "-m":
							metadata = args[++i];
							break;
						case "-c":
							comment = UnEscape( args[++i] );
							break;
						case "-e":
							orderByExtension = true;
							break;
						case "-s":
							includeSubdirs = true;
							break;
						case "-h":
							outHeaderName = args[++i];
							break;
						case "-o":
							originalFps4 = args[++i];
							break;
						case "--multiplier":
							multiplier = HexUtils.ParseDecOrHex( args[++i] );
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
				fps4 = new FPS4(originalFps4, printProgressToConsole: true);
			} else {
				fps4 = new FPS4();
			}

			if ( bitmask != null ) { fps4.ContentBitmask = new ContentInfo( (ushort)bitmask ); }
			if ( alignment != null ) { fps4.Alignment = (uint)alignment; }
			if ( littleEndian ) { fps4.Endian = EndianUtils.Endianness.LittleEndian; }
			if ( comment != null ) { fps4.ArchiveName = comment; }

			string[] files;
			if ( includeSubdirs ) {
				files = System.IO.Directory.GetFiles( dir, "*", System.IO.SearchOption.AllDirectories );
			} else {
				files = System.IO.Directory.GetFiles( dir );
			}

			if ( orderByExtension ) {
				files = files.OrderBy( x => x.Split( '.' ).Last() ).ToArray();
			}

			Stream outStream = new FileStream(outName, FileMode.Create);
			Stream outHeaderStream = outHeaderName == null ? null : new FileStream(outHeaderName, FileMode.Create);

			List<PackFileInfo> packFileInfos = new List<PackFileInfo>(files.Length);
			foreach (var file in files) {
				var fi = new System.IO.FileInfo(file);
				var p = new PackFileInfo();
				p.Name = fi.Name;
				p.Length = fi.Length;
				if (metadata != null && metadata.Contains('p')) {
					try {
						p.RelativePath = FPS4.GetRelativePath(outHeaderName == null ? outName : outHeaderName, fi.FullName);
					} catch (Exception) { }
				}
				p.DataStream = new DuplicatableFileStream(file);
				packFileInfos.Add(p);
			}


			try {
				FPS4.Pack(
					packFileInfos,
					outStream,
					fps4.ContentBitmask,
					fps4.Endian,
					fps4.Unknown2,
					originalFps4 != null ? new System.IO.FileStream(originalFps4, System.IO.FileMode.Open) : null,
					fps4.ArchiveName,
					fps4.FirstFileStart,
					fps4.Alignment,
					outputHeaderStream: outHeaderStream,
					metadata: metadata,
					alignmentFirstFile: alignmentFirstFile,
					fileLocationMultiplier: multiplier,
					printProgressToConsole: true
				);
			} finally {
				outStream.Close();
				if (outHeaderStream != null) {
					outHeaderStream.Close();
				}
			}

			return 0;
		}
	}
}
