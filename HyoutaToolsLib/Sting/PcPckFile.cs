using HyoutaPluginBase;
using HyoutaTools.Tales.Vesperia.NUB;
using HyoutaUtils;
using HyoutaUtils.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Sting {
	public class PcPckFile {
		public static void Extract(DuplicatableStream file, string destination) {
			System.IO.Directory.CreateDirectory(destination);

			uint fileCount = file.ReadUInt32();
			uint headerLength = file.ReadUInt32();

			for (uint i = 0; i < fileCount; ++i) {
				string filename = file.ReadSizedString(0x40, TextUtils.GameTextEncoding.ShiftJIS).TrimNull();
				uint length = file.ReadUInt32();
				uint offset = file.ReadUInt32();
				string destPath = System.IO.Path.Combine(destination, filename);
				System.IO.Directory.CreateDirectory(Path.GetDirectoryName(destPath));
				using (FileStream newFile = new FileStream(destPath, FileMode.Create)) {
					long p = file.Position;
					file.Position = offset;
					StreamUtils.CopyStream(file, newFile, length);
					file.Position = p;
				}
			}
		}

		public static int ExecuteExtract(List<string> args) {
			if (args.Count == 0) {
				Console.WriteLine("Sting.PcPckFile.Extract file.pck [output_folder]");
				return -1;
			}

			try {
				Extract(new DuplicatableFileStream(args[0]), args.Count > 1 ? args[1] : (args[0] + ".ex"));
				return 0;
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				return -1;
			}
		}
	}
}
