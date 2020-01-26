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
			Console.WriteLine("Usage: file.cpk [OutputDirectory] [--recursive]");
		}

		public static int Extract(List<string> args) {
			string inpath = null;
			string outpath = null;
			bool recursive = false;

			try {
				for (int i = 0; i < args.Count; ++i) {
					switch (args[i]) {
						case "-r":
						case "--recursive":
							recursive = true;
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
			DoExtract(cpk, outpath, recursive);

			return 0;
		}

		private static void DoExtract(CpkContainer cpk, string outdir, bool recursive) {
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
					DoExtract(subcpk, Path.Combine(outdir, name + ".ext"), recursive);
					continue;
				}

				string outpath = Path.Combine(outdir, name);
				Console.WriteLine("Extracting {0}...", outpath);
				Directory.CreateDirectory(Path.GetDirectoryName(outpath));
				using (var ds = cpk.GetChildByIndex(i).AsFile.DataStream)
				using (FileStream fs = new FileStream(outpath, FileMode.Create, FileAccess.Write)) {
					StreamUtils.CopyStream(ds, fs, ds.Length);
				}
			}
		}
	}
}
