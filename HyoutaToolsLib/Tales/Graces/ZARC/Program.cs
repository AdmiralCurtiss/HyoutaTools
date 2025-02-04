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
			if (args.Count != 1) {
				Console.WriteLine("Usage: Tales.Graces.ZARC.Extract file");
				return -1;
			}

			var zarc = new ZARC(new DuplicatableFileStream(args[0]));

			DirectoryInfo d = System.IO.Directory.CreateDirectory(args[0] + ".ext");
			for (int i = 0; i < zarc.NumberOfFiles; ++i) {
				var f = zarc.GetChildByIndex(i);
				if (f != null) {
					string path = Path.Combine(d.FullName, i.ToString());
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
