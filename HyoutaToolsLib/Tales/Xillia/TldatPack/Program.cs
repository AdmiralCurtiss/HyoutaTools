using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaTools.Tales.Xillia.TldatExtract;
using HyoutaUtils;

namespace HyoutaTools.Tales.Xillia.TldatPack {
	class Program {
		public static int Execute(List<string> args) {
			if (args.Count != 4) {
				Console.WriteLine("Usage: OldTOFHDB Folder NewTLDAT NewTOFHDB");
				Console.WriteLine("Metadata for files is copied from the old TOFHDB. This means that files can only be replaced, no new files added.");
				Console.WriteLine("A file is identified by its index, which is extracted from its filename.");
				Console.WriteLine("There must be exactly one file for each file in the original archive, and they must be named by continuous indices starting from 1. Extension and subfolder path is ignored.");
				return -1;
			}

			string OldTOFHDB = args[0];
			string ExtractFolder = args[1];
			string TLDAT = args[2];
			string TOFHDB = args[3];
			TOFHDBheader header = new TOFHDBheader(OldTOFHDB);

			// collect files
			SortedDictionary<uint, string> filenameMap = new SortedDictionary<uint, string>();
			foreach (string dir in Directory.EnumerateDirectories(ExtractFolder)) {
				foreach (string file in Directory.EnumerateFiles(dir)) {
					string filenumstr = Path.GetFileNameWithoutExtension(file);
					uint num;
					try {
						num = HexUtils.ParseDecOrHex(filenumstr);
					} catch (Exception) {
						Console.WriteLine("Could not parse {0} as a file index, aborting.", file);
						return 3;
					}
					if (filenameMap.ContainsKey(num)) {
						Console.WriteLine("Files {0} and {1} both map to index {2}, not allowed.", file, filenameMap[num], num);
						return 4;
					}

					if (num < 1 || num > header.FileArray.Count) {
						Console.WriteLine("Index {0} (from file {1}) is outside of allowed range. Must be 1 <= index <= {2}.", num, file, header.FileArray.Count);
						return 5;
					}
					filenameMap.Add(num, file);
				}
			}

			if (filenameMap.Count != header.FileArray.Count) {
				Console.WriteLine("File count mismatch!");
				return 1;
			}
			if (filenameMap.First().Key != 1 || filenameMap.Last().Key != header.FileArray.Count) {
				Console.WriteLine("Filenames are wrong!");
				return 2;
			}

			// write files and populate header
			using (FileStream fsData = File.Open(TLDAT, FileMode.Create, FileAccess.ReadWrite)) {
				foreach (var f in filenameMap) {
					using (FileStream fs = new FileStream(f.Value, FileMode.Open)) {
						header.FileArray[(int)(f.Key - 1)].Filesize = (ulong)fs.Length;
						header.FileArray[(int)(f.Key - 1)].CompressedSize = (ulong)fs.Length;
						header.FileArray[(int)(f.Key - 1)].Offset = (ulong)fsData.Position;

						// check if TLZC compressed and write uncompressed size if so
						if (fs.PeekUInt32() == 0x435A4C54) {
							fs.DiscardBytes(12);
							header.FileArray[(int)(f.Key - 1)].Filesize = fs.ReadUInt32();
							fs.Position = 0;
						}

						fs.CopyTo(fsData);
						fs.Close();
					}
				}
				fsData.Close();
			}

			// write header
			using (FileStream fs = new FileStream(TOFHDB, FileMode.Create, FileAccess.ReadWrite)) {
				header.Write(fs);
				fs.Close();
			}

			return 0;
		}
	}
}
