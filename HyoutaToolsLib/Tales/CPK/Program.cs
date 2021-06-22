using HyoutaUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.CPK {
	public class Program {
		public static void PrintExtractUsage() {
			Console.WriteLine("Usage: [--recursive] [--no-decompress] file.cpk [OutputDirectory]");
		}

		public static int Extract(List<string> args) {
			string inpath = null;
			string outpath = null;
			bool recursive = false;
			bool decompress = true;

			try {
				for (int i = 0; i < args.Count; ++i) {
					switch (args[i]) {
						case "-r":
						case "--recursive":
							recursive = true;
							break;
						case "--no-decompress":
							decompress = false;
							break;
						default:
							if (inpath == null) { inpath = args[i]; } else if (outpath == null) { outpath = args[i]; } else { PrintExtractUsage(); return -1; }
							break;
					}
				}
			} catch (IndexOutOfRangeException) {
				PrintExtractUsage();
				return -1;
			}

			if (inpath == null) {
				PrintExtractUsage();
				return -1;
			}

			if (outpath == null) {
				outpath = inpath + ".ext";
			}

			CpkContainer cpk = new CpkContainer(new HyoutaUtils.Streams.DuplicatableFileStream(inpath));
			DoExtract(cpk, outpath, recursive, decompress);

			return 0;
		}

		private static HyoutaPluginBase.DuplicatableStream GetFile(CpkContainer cpk, long index, bool decompress) {
			if (decompress) {
				return cpk.GetChildByIndex(index).AsFile.DataStream;
			} else {
				var entry = cpk.GetEntryByIndex(index);
				return new HyoutaUtils.Streams.PartialStream(cpk.DuplicateStream(), entry.file_offset, entry.file_size);
			}
		}

		private static void DoExtract(CpkContainer cpk, string outdir, bool recursive, bool decompress) {
			for (long i = 0; i < cpk.toc_entries; ++i) {
				CpkContainer.Entry entry = cpk.GetEntryByIndex(i);
				if (entry == null) {
					continue;
				}

				string name = entry.name;
				if (recursive && name.ToLowerInvariant().EndsWith(".cpk")) {
					CpkContainer subcpk;
					if (entry.compressed) {
						subcpk = new CpkContainer(cpk.GetChildByIndex(i).AsFile.DataStream);
					} else {
						subcpk = new CpkContainer(cpk.DuplicateStream(), entry.file_offset);
					}
					DoExtract(subcpk, Path.Combine(outdir, name + ".ext"), recursive, decompress);
					continue;
				}

				string outpath = Path.Combine(outdir, name);
				Console.WriteLine("Extracting {0}...", outpath);
				Directory.CreateDirectory(Path.GetDirectoryName(outpath));
				using (var ds = GetFile(cpk, i, decompress))
				using (FileStream fs = new FileStream(outpath, FileMode.Create, FileAccess.Write)) {
					StreamUtils.CopyStream(ds, fs, ds.Length);
				}
			}
		}

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
				utf_tab_sharp.CpkUncompress.uncompress(infile, 0, infile.Length, outfile);
			}

			return 0;
		}

		public static void PrintCompressUsage() {
			Console.WriteLine("Usage: decompressed.bin compressed.bin");
		}

		public static int Compress(List<string> args) {
			string inpath = null;
			string outpath = null;

			try {
				for (int i = 0; i < args.Count; ++i) {
					switch (args[i]) {
						default:
							if (inpath == null) { inpath = args[i]; } else if (outpath == null) { outpath = args[i]; } else { PrintCompressUsage(); return -1; }
							break;
					}
				}
			} catch (IndexOutOfRangeException) {
				PrintCompressUsage();
				return -1;
			}

			if (inpath == null) {
				PrintCompressUsage();
				return -1;
			}

			if (outpath == null) {
				outpath = inpath + ".cmp";
			}

			using (var infile = new HyoutaUtils.Streams.DuplicatableFileStream(inpath))
			using (var outfile = new FileStream(outpath, FileMode.Create)) {
				utf_tab_sharp.CpkCompress.compress(infile, 0, infile.Length, outfile);
			}

			return 0;
		}
	}
}
