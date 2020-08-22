using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils.Streams;

namespace HyoutaTools.Tales.Vesperia.NUB {
	public class Program {
		public static int ExecuteExtract(List<string> args) {
			if (args.Count < 1) {
				Console.WriteLine("Usage: infile.nub [outdir]");
				return -1;
			}

			string inpath = args[0];
			string outpath = args.Count > 1 ? args[1] : args[0] + ".ext";

			NUB.ExtractNub(new DuplicatableFileStream(inpath), outpath, HyoutaUtils.EndianUtils.Endianness.BigEndian);

			return 0;
		}

		public static int ExecuteRebuild(List<string> args) {
			if (args.Count < 2) {
				Console.WriteLine("Usage: original.nub modifieddir [modified.nub]");
				return -1;
			}

			string innub = args[0];
			string infolder = args[1];
			string outpath = args.Count > 2 ? args[2] : args[0] + ".rebuild.nub";

			NUB.RebuildNub(new DuplicatableFileStream(innub), infolder, outpath, HyoutaUtils.EndianUtils.Endianness.BigEndian);

			return 0;
		}
	}
}
