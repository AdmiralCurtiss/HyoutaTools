using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaUtils.Streams;
using HyoutaUtils;

namespace HyoutaTools.Tales.Graces.ZARC {
	class Program {
		public static int ExecuteExtract(List<string> args) {
			if (args.Count < 1) {
				Console.WriteLine("Usage: Tales.Graces.ZARC.Extract file [filepaths.txt]");
				Console.WriteLine();
				Console.WriteLine("filepaths.txt should contain a list of possible file paths");
				Console.WriteLine("and will be used to match the hashes to the filenames.");
				Console.WriteLine("If the hash doesn't match anything given, the hash itself");
				Console.WriteLine("is used as the filename.");
				return -1;
			}

			Dictionary<ulong, string> filepaths = new Dictionary<ulong, string>();
			if (args.Count >= 2) {
				foreach (string path in File.ReadAllLines(args[1])) {
					string p = path.Trim().Replace('\\', '/');
					if (p != "") {
						filepaths.Add(ZARC.CalculateFilenameHash(p), p);
					}
				}
			}

			var zarc = new ZARC(new DuplicatableFileStream(args[0]));

			DirectoryInfo d = System.IO.Directory.CreateDirectory(args[0] + ".ext");
			for (int i = 0; i < zarc.NumberOfFiles; ++i) {
				var f = zarc.GetChildByIndex(i);
				if (f != null) {
					string path;
					string filename;
					ulong hash = zarc.Files[i].FilenameHash;
					if (!filepaths.TryGetValue(hash, out filename)) {
						StringBuilder sb = new StringBuilder();
						for (int j = 0; j < 8; ++j) {
							sb.Append(((hash >> ((7 - j) * 8)) & 0xff).ToString("x2"));
						}
						filename = sb.ToString();
					}
					Console.WriteLine("Extracting " + filename);
					path = Path.Combine(d.FullName, filename);
					Directory.CreateDirectory(Path.GetDirectoryName(path));
					using (var ds = f.AsFile.DataStream.Duplicate())
					using (var fs = new FileStream(path, FileMode.Create)) {
						StreamUtils.CopyStream(ds, fs);
					}
				}
			}

			return 0;
		}
	}
}
