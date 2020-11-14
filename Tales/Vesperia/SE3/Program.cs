using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;
using HyoutaUtils.Streams;

namespace HyoutaTools.Tales.Vesperia.SE3 {
	public class Program {
		public static int ExtractToNub(List<string> args) {
			if (args.Count < 1) {
				Console.WriteLine("Usage: in.se3 [out.nub]");
				return -1;
			}

			string infile = args[0];
			string outfile = args.Count >= 2 ? args[1] : infile + ".nub";

			using (var s = new SE3(infile, null, TextUtils.GameTextEncoding.ASCII).ExtractNubStream())
			using (var fs = new System.IO.FileStream(outfile, System.IO.FileMode.Create)) {
				StreamUtils.CopyStream(s, fs);
			}

			return 0;
		}

		public static int ExtractSE3(List<string> args) {
			if (args.Count < 1) {
				Console.WriteLine("Usage: in.se3 [outdir]");
				return -1;
			}

			string infile = args[0];
			string outpath = args.Count > 1 ? args[1] : args[0] + ".ext";

			var se3 = new SE3(infile, null, TextUtils.GameTextEncoding.ASCII);
			var nub = new NUB.NUB(se3.ExtractNubStream(), se3.Endian);

			if (se3.FileCount != nub.EntryCount) {
				Console.WriteLine("WARNING: SE3 header and NUB header disagree on number of files. ({0} != {1})", se3.FileCount, nub.EntryCount);
			}

			System.IO.Directory.CreateDirectory(outpath);
			for (long i = 0; i < nub.EntryCount; ++i) {
				var file = nub.GetChildByIndex(i) as NUB.NubFile;
				if (file != null) {
					string fname = i < se3.Filenames.Count ? se3.Filenames[(int)i] : i.ToString("D8");
					using (var ds = file.DataStream.Duplicate())
					using (var fs = new FileStream(Path.Combine(outpath, fname + "." + file.Type), FileMode.Create)) {
						StreamUtils.CopyStream(ds, fs);
					}
				}
			}

			return 0;
		}
	}
}
