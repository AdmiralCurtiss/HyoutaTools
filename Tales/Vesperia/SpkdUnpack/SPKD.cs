using HyoutaPluginBase;
using HyoutaPluginBase.FileContainer;
using HyoutaTools.FileContainer;
using HyoutaUtils;
using HyoutaUtils.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.SpkdUnpack {
	public class SPKD : ContainerBase {
		private DuplicatableStream Stream;
		private List<SpkdFileData> Files;

		public int FileCount => Files.Count;

		public SPKD(DuplicatableStream duplicatableStream) {
			EndianUtils.Endianness e = EndianUtils.Endianness.BigEndian;
			Stream = duplicatableStream.Duplicate();
			Stream.Position = 4;
			uint fileCount = Stream.ReadUInt32(e);
			Stream.Position = 12;
			uint blockSize = Stream.ReadUInt32(e);

			Files = new List<SpkdFileData>((int)fileCount);
			for (uint i = 0; i < fileCount; ++i) {
				Stream.Position = (i + 1) * blockSize;
				var f = new SpkdFileData();
				f.Name = Stream.ReadAscii(16).TrimNull();
				f.Unknown1 = Stream.ReadUInt32(e);
				f.FileStart1 = Stream.ReadUInt32(e);
				f.FileStart2 = Stream.ReadUInt32(e);
				f.Unknown2 = Stream.ReadUInt32(e);
				Files.Add(f);
			}
		}

		public string GetFileName(int index) {
			return Files[index].Name;
		}

		public override void Dispose() {
			if (Stream != null) {
				Stream.Close();
				Stream.Dispose();
				Stream = null;
			}
		}

		public override INode GetChildByIndex(long index) {
			if (index >= 0 && index < Files.Count) {
				long start = Files[(int)index].FileStart1;
				long end = (index + 1) == Files.Count ? Stream.Length : Files[(int)index + 1].FileStart1;
				long len = end - start;
				if (len > 0) {
					return new FileFromStream(new PartialStream(Stream, start, len));
				} else {
					return new FileFromStream(EmptyStream.Instance);
				}
			}
			return null;
		}

		public override INode GetChildByName(string name) {
			for (int i = 0; i < Files.Count; ++i) {
				if (Files[i].Name == name) {
					return GetChildByIndex(i);
				}
			}
			return null;
		}

		public override IEnumerable<string> GetChildNames() {
			List<string> names = new List<string>(Files.Count);
			for (int i = 0; i < Files.Count; ++i) {
				names.Add(Files[i].Name);
			}
			return names;
		}

		public class SpkdFileData {
			public string Name;
			public uint Unknown1;
			public uint FileStart1;
			public uint FileStart2; // Why is this twice?
			public uint Unknown2;
		}
	}
}
