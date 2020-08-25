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
		private uint LastFileEnd;

		public int MaxFileCount => Files.Count * 3;

		public SPKD(DuplicatableStream duplicatableStream) {
			EndianUtils.Endianness e = EndianUtils.Endianness.BigEndian;
			Stream = duplicatableStream.Duplicate();
			Stream.Position = 4;
			uint fileCount = Stream.ReadUInt32(e);
			Stream.Position = 12;
			uint dataStart = Stream.ReadUInt32(e);

			Files = new List<SpkdFileData>((int)fileCount);
			Stream.Position = dataStart;
			for (uint i = 0; i < fileCount; ++i) {
				var f = new SpkdFileData();
				f.Name = Stream.ReadAscii(16).TrimNull();
				f.Unknown = Stream.ReadUInt32(e);
				f.FileStart0 = Stream.ReadUInt32(e);
				f.FileStart1 = Stream.ReadUInt32(e);
				f.FileStart2 = Stream.ReadUInt32(e);
				Files.Add(f);
			}
			LastFileEnd = (uint)Stream.Length;
		}

		public string GetFileName(int index) {
			return Files[index / 3].Name + "." + (index % 3);
		}

		public override void Dispose() {
			if (Stream != null) {
				Stream.Close();
				Stream.Dispose();
				Stream = null;
			}
		}

		public override INode GetChildByIndex(long index) {
			if (index >= 0 && index < Files.Count * 3) {
				int mainfile = (int)(index / 3);
				int subfile = (int)(index % 3);
				uint start = Files[mainfile].GetFileStart(subfile);
				if (start == 0xffffffffu) {
					return null;
				}
				uint end = FindFileEnd(index);
				long len = end - start;
				if (len > 0) {
					return new FileFromStream(new PartialStream(Stream, start, len));
				} else {
					return new FileFromStream(EmptyStream.Instance);
				}
			}
			return null;
		}

		private uint FindFileEnd(long index) {
			++index;
			while (index < Files.Count * 3) {
				int mainfile = (int)(index / 3);
				int subfile = (int)(index % 3);
				uint end = Files[mainfile].GetFileStart(subfile);
				if (end != 0xffffffffu) {
					return end;
				}
				++index;
			}
			return LastFileEnd;
		}

		public override INode GetChildByName(string name) {
			for (int i = 0; i < Files.Count * 3; ++i) {
				if (GetFileName(i) == name) {
					return GetChildByIndex(i);
				}
			}
			return null;
		}

		public override IEnumerable<string> GetChildNames() {
			List<string> names = new List<string>(Files.Count);
			for (int i = 0; i < Files.Count * 3; ++i) {
				names.Add(GetFileName(i));
			}
			return names;
		}

		public class SpkdFileData {
			public string Name;
			public uint Unknown;
			public uint FileStart0;
			public uint FileStart1;
			public uint FileStart2;

			public uint GetFileStart(int i) {
				return i == 0 ? FileStart0 : i == 1 ? FileStart1 : i == 2 ? FileStart2 : 0xffffffffu;
			}
		}
	}
}
