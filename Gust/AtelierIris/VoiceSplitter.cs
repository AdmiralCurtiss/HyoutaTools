using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Gust.AtelierIris {
	public static class VoiceSplitter {
		public static int Execute(List<string> args) {
			if (args.Count < 2) {
				Console.WriteLine("Usage: V0000.SIF V0000.VP");
				return -1;
			}

			char[] comma = new char[] { ',' };
			string[] siflines = System.IO.File.ReadAllLines(args[0]);
			long filecount = long.Parse(siflines[0].Split(comma)[0]);
			var offsets = new List<(long offset, long size, string name)>();
			for (long i = 1; i < siflines.LongLength; ++i) {
				string[] split = siflines[i].Split(comma, 3);
				if (split.Length >= 2) {
					string name = null;
					if (split.Length == 3) {
						name = TryParseFilename(split[2]);
					}
					offsets.Add((long.Parse(split[0]), long.Parse(split[1]), name));
				}
			}

			// make sure we have no duplicate filenames
			HashSet<string> filenames = new HashSet<string>();
			for (int i = 0; i < offsets.Count; ++i) {
				string filename = offsets[i].name ?? i.ToString("D4");
				string basename = filename;
				int idx = 2;
				while (filenames.Contains(filename)) {
					filename = basename + "_" + idx;
					++idx;
				}
				offsets[i] = (offsets[i].offset, offsets[i].size, filename);
				filenames.Add(filename);
			}

			string outbasename = System.IO.Path.GetFileNameWithoutExtension(args[1]);
			long sectorsize = 2048;
			using (var vp = new HyoutaUtils.Streams.DuplicatableFileStream(args[1])) {
				for (int i = 0; i < offsets.Count; ++i) {
					using (var fs = new System.IO.FileStream(outbasename + "_" + offsets[i].name, System.IO.FileMode.Create)) {
						vp.Position = offsets[i].offset * sectorsize;
						HyoutaUtils.StreamUtils.CopyStream(vp, fs, offsets[i].size);
					}
				}
			}

			return 0;
		}

		public static string TryParseFilename(string str) {
			// the SIF files have filenames in a /**/ style comment, try to suss that out...
			string s = str;
			int commentend = s.LastIndexOf("*/");
			if (commentend == -1) {
				return null;
			}

			s = s.Substring(0, commentend).TrimEnd();
			int lastcomma = s.LastIndexOf(',');
			if (lastcomma != -1) {
				s = s.Substring(lastcomma + 1).TrimStart();
			}

			int lastpath = s.LastIndexOf('/');
			if (lastpath != -1) {
				s = s.Substring(lastpath + 1).TrimStart();
			}

			if (s == "") {
				return null;
			}

			return s.Replace('.', '_');
		}
	}
}
