using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EndianUtils = HyoutaUtils.EndianUtils;
using HyoutaUtils.Streams;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.SpkdUnpack {
	class Program {
		public static int Execute(List<string> args) {
			if (args.Count != 1) {
				Console.WriteLine("Usage: SPKDunpack file");
				return -1;
			}

			var spkd = new SPKD(new DuplicatableFileStream(args[0]));

			DirectoryInfo d = System.IO.Directory.CreateDirectory(args[0] + ".ext");
			for (int i = 0; i < spkd.FileCount; ++i) {
				string path = Path.Combine(d.FullName, spkd.GetFileName(i));
				using (var ds = spkd.GetChildByIndex(i).AsFile.DataStream.Duplicate())
				using (var fs = new FileStream(path, FileMode.Create)) {
					StreamUtils.CopyStream(ds, fs);
				}
			}

			return 0;
		}
	}
}
