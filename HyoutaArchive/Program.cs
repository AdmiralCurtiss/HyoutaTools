using HyoutaUtils;
using HyoutaUtils.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.HyoutaArchive {
	public static class Program {
		public static int Pack(List<string> args) {
			if (args.Count < 2) {
				Console.WriteLine("Usage: folder-to-pack chunk.hac");
				return -1;
			}

			List<HyoutaUtils.HyoutaArchive.HyoutaArchiveFileInfo> files = new List<HyoutaUtils.HyoutaArchive.HyoutaArchiveFileInfo>();
			foreach (var fi in new DirectoryInfo(args[0]).GetFiles()) {
				var f = new HyoutaUtils.HyoutaArchive.HyoutaArchiveFileInfo();
				f.Data = new DuplicatableFileStream(fi.FullName);
				f.Filename = fi.Name;
				files.Add(f);
			}
			using (var fs = new FileStream(args[1], FileMode.Create)) {
				HyoutaUtils.HyoutaArchive.HyoutaArchiveChunk.Pack(fs, files, 0, HyoutaUtils.EndianUtils.Endianness.LittleEndian, null, null);
			}

			return 0;
		}

		public static int Extract(List<string> args) {
			if (args.Count < 1) {
				Console.WriteLine("Usage: archive.hac folder-to-extract-to");
				return -1;
			}

			string infile = args[0];
			string outdir = args.Count == 1 ? infile + ".ext" : args[1];

			using (var archive = new HyoutaUtils.HyoutaArchive.HyoutaArchiveFile(new DuplicatableFileStream(infile))) {
				Directory.CreateDirectory(outdir);
				for (long i = 0; i < archive.Filecount; ++i) {
					var f = archive.GetFile(i);
					using (var ds = f.DataStream.Duplicate())
					using (var fs = new FileStream(Path.Combine(outdir, f.Filename ?? i.ToString("D8")), FileMode.Create)) {
						StreamUtils.CopyStream(ds, fs);
					}
				}
			}

			return 0;
		}
	}
}
