using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Compression {
	public class Program {
		public static void PrintDecompressUsage() {
			Console.WriteLine("Usage: compressed.bin decompressed.bin");
		}

		public static int Decompress(List<string> args) {
			string inpath = null;
			string outpath = null;

			try {
				for (int i = 0; i < args.Count; ++i) {
					switch (args[i]) {
						default:
							if (inpath == null) { inpath = args[i]; } else if (outpath == null) { outpath = args[i]; } else { PrintDecompressUsage(); return -1; }
							break;
					}
				}
			} catch (IndexOutOfRangeException) {
				PrintDecompressUsage();
				return -1;
			}

			if (inpath == null) {
				PrintDecompressUsage();
				return -1;
			}

			if (outpath == null) {
				outpath = inpath + ".dec";
			}

			using (var infile = new HyoutaUtils.Streams.DuplicatableFileStream(inpath))
			using (var outfile = new FileStream(outpath, FileMode.Create)) {
				if (Decompressor.DecompressToStream(infile, outfile) < 0) {
					Console.WriteLine("decompression failure");
					return -1;
				}
			}

			return 0;
		}
	}
}
