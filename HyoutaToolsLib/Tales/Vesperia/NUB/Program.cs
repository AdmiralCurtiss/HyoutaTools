using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils.Streams;

namespace HyoutaTools.Tales.Vesperia.NUB {
	public class Program {
		public static int ExecuteExtract(List<string> args) {
			HyoutaUtils.EndianUtils.Endianness? endian = null;
			string inpath = null;
			string outpath = null;

			for (int i = 0; i < args.Count; ++i) {
				if (args[i] == "--big-endian") {
					endian = HyoutaUtils.EndianUtils.Endianness.BigEndian;
				} else if (args[i] == "--little-endian") {
					endian = HyoutaUtils.EndianUtils.Endianness.LittleEndian;
				} else if (inpath == null) {
					inpath = args[i];
				} else if (outpath == null) {
					outpath = args[i];
				}
			}

			if (inpath == null) {
				Console.WriteLine("Usage: [--big-endian/--little-endian] infile.nub [outdir]");
				return -1;
			}
			if (outpath == null) {
				outpath = inpath + ".ext";
			}

			NUB.ExtractNub(new DuplicatableFileStream(inpath), outpath, endian);

			return 0;
		}

		public static int ExecuteRebuild(List<string> args) {
			HyoutaUtils.EndianUtils.Endianness? endian = null;
			string inpath = null;
			string infolder = null;
			string outpath = null;

			for (int i = 0; i < args.Count; ++i) {
				if (args[i] == "--big-endian") {
					endian = HyoutaUtils.EndianUtils.Endianness.BigEndian;
				} else if (args[i] == "--little-endian") {
					endian = HyoutaUtils.EndianUtils.Endianness.LittleEndian;
				} else if (inpath == null) {
					inpath = args[i];
				} else if (infolder == null) {
					infolder = args[i];
				} else if (outpath == null) {
					outpath = args[i];
				}
			}

			if (inpath == null || infolder == null) {
				Console.WriteLine("Usage: [--big-endian/--little-endian] original.nub modifieddir [modified.nub]");
				return -1;
			}
			if (outpath == null) {
				outpath = inpath + ".rebuild.nub";
			}

			NUB.RebuildNub(new DuplicatableFileStream(inpath), infolder, outpath, endian);

			return 0;
		}
	}
}
