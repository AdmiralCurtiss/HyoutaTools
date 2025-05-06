using System;
using System.IO;
using System.Collections.Generic;
using HyoutaUtils.Streams;
using HyoutaUtils;

namespace HyoutaTools.Tales.tlzc {
	class tlzcmain {
		public static void PrintUsage() {
			Console.WriteLine("Usage: tlzc [-c/-d] [--type number] [--subtype string] infile [outfile]");
			Console.WriteLine();
			Console.WriteLine(" -c/-d");
			Console.WriteLine("Force compression or decompression. Autodetected if neither is given.");
			Console.WriteLine();
			Console.WriteLine(" --type 2/4");
			Console.WriteLine("Selects the compression type used for compression or decompression. This is the byte at position 0x5 in the header.");
			Console.WriteLine("Autodetected for decompression if not given. Compression uses type '4' (as used in PS3 Vesperia) if not given.");
			Console.WriteLine();
			Console.WriteLine(" --subtype deflate/zlib");
			Console.WriteLine("Selects the compression subtype used for compression or decompression. This is not given in the header, as far as I can tell.");
			Console.WriteLine("This was born because type '2' can exist in both deflate and zlib variants, with no obvious way to detect it.");
			Console.WriteLine("zlib is used if not given. On decompression, if zlib fails, deflate is tried afterwards.");
		}

		public static int Execute( List<string> args ) {
			bool ForceDecompress = false;
			bool ForceCompress = false;
			string Filename = null;
			string FilenameOut = null;

			int? compressionNumFastBytes = null;
			int? compressionType = null;
			string compressionSubtype = null;

			if ( args.Count < 1 ) {
				PrintUsage();
				return -1;
			}

			int namecounter = 0;
			for ( int i = 0; i < args.Count; ++i ) {
				string arg = args[i];
				if ( arg.StartsWith( "-" ) ) {
					switch ( arg ) {
						case "-c": ForceCompress = true; break;
						case "-d": ForceDecompress = true; break;
						case "--type": compressionType = Int32.Parse( args[++i] ); break;
						case "--subtype": compressionSubtype = args[++i]; break;
						case "--fastbytes": compressionNumFastBytes = Int32.Parse( args[++i] ); break;
					}
				} else {
					++namecounter;
					switch ( namecounter ) {
						case 1: Filename = arg; break;
						case 2: FilenameOut = arg; break;
					}
				}
			}

			if ( Filename == null ) {
				PrintUsage();
				return -1;
			}

			DuplicatableFileStream input = new DuplicatableFileStream(Filename);
			Stream output;

			if ((ForceDecompress || input.PeekUInt32(EndianUtils.Endianness.BigEndian) == 0x544c5a43) && !ForceCompress) {
				try {
					Console.WriteLine("decompressing {0}", Filename);
					output = TLZC.Decompress(input, compressionType, compressionSubtype);
					if (FilenameOut == null) {
						FilenameOut = Filename + ".dec";
					}
					using (var outstream = new FileStream(FilenameOut, FileMode.Create)) {
						output.Position = 0;
						StreamUtils.CopyStream(output, outstream);
					}
				} catch (Exception ex) {
					Console.WriteLine("Decompression failed: " + ex.ToString());
					return -1;
				}
			} else {
				try {
					Console.WriteLine("compressing {0}", Filename);
					output = TLZC.Compress(input, compressionType.GetValueOrDefault(4), compressionSubtype, compressionNumFastBytes.GetValueOrDefault(64));
					if (FilenameOut == null) {
						FilenameOut = Filename + ".tlzc";
					}
					using (var outstream = new FileStream(FilenameOut, FileMode.Create)) {
						output.Position = 0;
						StreamUtils.CopyStream(output, outstream);
					}
				} catch (Exception ex) {
					Console.WriteLine("Compression failed: " + ex.ToString());
					return -1;
				}
			}

			return 0;
		}
	}
}