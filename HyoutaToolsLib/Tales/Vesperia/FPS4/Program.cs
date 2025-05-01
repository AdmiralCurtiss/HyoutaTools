using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HyoutaTools.FinalFantasyCrystalChronicles.FileSections;
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

			if (!fps4.ContentBitmask.ContainsFileMetadata && metadata != null) {
				Console.WriteLine("ERROR: Content bitmask does not include metadata, but metadata was requested. Either add the metadata (0x40) bit to the bitmask, or remove the metadata.");
				return -1;
			}

			string[] files;
			if ( includeSubdirs ) {
				files = System.IO.Directory.GetFiles( dir, "*", System.IO.SearchOption.AllDirectories );
			} else {
				files = System.IO.Directory.GetFiles( dir );
			}

			// the filesystem may not give us a consistent order, make sure we have one for reproducability
			files = files.OrderBy(x => x.ToLowerInvariant()).ToArray();

			if ( orderByExtension ) {
				files = files.OrderBy( x => x.Split( '.' ).Last() ).ToArray();
			}

			Stream outStream = new FileStream(outName, FileMode.Create);
			Stream outHeaderStream = outHeaderName == null ? null : new FileStream(outHeaderName, FileMode.Create);

			List<PackFileInfo> packFileInfos = new List<PackFileInfo>(files.Length);
			for (int i = 0; i < files.Length; ++i) {
				string file = files[i];
				var fi = new System.IO.FileInfo(file);
				var p = new PackFileInfo();
				p.FileIndex = (uint)i;
				p.FileName = fi.Name;
				p.FileSize = fi.Length;
				if (metadata != null) {
					p.Metadata = new List<(string Key, string Value)>();
					if (metadata.Contains('p')) {
						try {
							p.Metadata.Add((null, FPS4.GetRelativePath(outHeaderName == null ? outName : outHeaderName, fi.FullName)));
						} catch (Exception) {
							Console.WriteLine("Failed to add relative path for file " + fi.FullName + ", skipping");
						}
					}
					if (metadata.Contains('n')) {
						p.Metadata.Add(("name", Path.GetFileNameWithoutExtension(p.FileName)));
					}
				}
				p.DataStream = new DuplicatableFileStream(file);
				p.SourcePath = file.Replace('\\', '/');
				packFileInfos.Add(p);
			}

			if (originalFps4 != null) {
				var oldArchive = new FPS4(originalFps4);
				var newFiles = new List<PackFileInfo>();
				var oldFiles = oldArchive.Files;
				for (int i = 0; i < oldFiles.Count - 1; ++i) {
					var p = new PackFileInfo();
					p.FileIndex = oldFiles[i].FileIndex;
					p.FileSize = oldFiles[i].FileSize ?? 0;
					p.FileName = oldFiles[i].FileName;
					p.FileType = oldFiles[i].FileType;
					p.Metadata = oldFiles[i].Metadata;
					p.Unknown0x0080 = oldFiles[i].Unknown0x0080;
					p.Unknown0x0100 = oldFiles[i].Unknown0x0100;
					newFiles.Add(p);
				}

				// now we try to match our collected files onto the new pack files
				for (int i = 0; i < packFileInfos.Count; i++) {
					int preferredTargetIndex = -1;
					PackFileInfo pfi = packFileInfos[i];
					for (int j = 0; j < newFiles.Count; ++j) {
						if (newFiles[j].DataStream == null) {
							string fullpath = newFiles[j].GuessFullFilePath();
							if (fullpath == pfi.FileName || fullpath == pfi.GuessFullFilePath()) {
								preferredTargetIndex = j;
								break;
							}
						}
					}
					if (preferredTargetIndex < 0) {
						for (int j = 0; j < newFiles.Count; ++j) {
							if (newFiles[j].DataStream == null) {
								string fullpath = newFiles[j].GuessFullFilePath();
								if (pfi.SourcePath.EndsWith(fullpath)) {
									preferredTargetIndex = j;
									break;
								}
							}
						}
					}
					if (preferredTargetIndex < 0) {
						int tmp;
						if (int.TryParse(Path.GetFileNameWithoutExtension(pfi.FileName), System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture, out tmp)) {
							preferredTargetIndex = tmp;
						}
					}

					if (preferredTargetIndex >= 0) {
						Console.WriteLine("Matched given file " + pfi.SourcePath + " to original file index #" + preferredTargetIndex);
						newFiles[preferredTargetIndex].FileSize = pfi.FileSize;
						newFiles[preferredTargetIndex].DataStream = pfi.DataStream;
						pfi.DataStream = null;
					} else {
						Console.WriteLine("Could not find a match in the original FPS4 for " + pfi.SourcePath);
					}
				}

				packFileInfos = newFiles;

				if (alignment == null) {
					uint align = 0xffffffff;
					align &= ~fps4.FirstFileStart;
					if (fps4.ContentBitmask.ContainsStartPointers) {
						for (int i = 0; i < oldFiles.Count; ++i) {
							if (oldFiles[i].Location.Value != 0xffffffff) {
								align &= ~oldFiles[i].Location.Value;
							}
						}
					}
					int bits = 0;
					for (int i = 0; i < 32; ++i) {
						if ((align & (1u << i)) == 0) {
							break;
						}
						++bits;
					}
					fps4.Alignment = (1u << bits);
					Console.WriteLine("Alignment not specified, guessing from original FPS4: 0x" + fps4.Alignment.ToString("x"));
				}
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
