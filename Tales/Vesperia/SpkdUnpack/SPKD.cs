using HyoutaPluginBase;
using HyoutaPluginBase.FileContainer;
using HyoutaTools.FileContainer;
using HyoutaUtils;
using HyoutaUtils.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.SpkdUnpack {
	public class SPKD : ContainerBase {
		private DuplicatableStream Stream;
		private List<SpkdFileData> Files;
		private uint LastFileEnd;

		public int MaxFileCount => Files.Count * 3;

		public SPKD(DuplicatableStream duplicatableStream, EndianUtils.Endianness e = EndianUtils.Endianness.BigEndian) {
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
			List<string> names = new List<string>(Files.Count * 3);
			for (int i = 0; i < Files.Count * 3; ++i) {
				names.Add(GetFileName(i));
			}
			return names;
		}

		public List<SpkdPackFileData> GetPackData() {
			var packs = new List<SpkdPackFileData>(Files.Count);
			for (int i = 0; i < Files.Count; ++i) {
				SpkdPackFileData p = new SpkdPackFileData();
				p.Name = Files[i].Name;
				p.Unknown = Files[i].Unknown;
				p.File0 = GetChildByIndex(i * 3 + 0)?.AsFile?.DataStream?.Duplicate();
				p.File1 = GetChildByIndex(i * 3 + 1)?.AsFile?.DataStream?.Duplicate();
				p.File2 = GetChildByIndex(i * 3 + 2)?.AsFile?.DataStream?.Duplicate();
				packs.Add(p);
			}
			return packs;
		}

		public static DuplicatableStream Pack(List<SpkdPackFileData> packs, EndianUtils.Endianness e = EndianUtils.Endianness.BigEndian) {
			using (MemoryStream ms = new MemoryStream()) {
				ms.WriteUInt32(0x444B5053, EndianUtils.Endianness.LittleEndian);
				ms.WriteUInt32((uint)packs.Count, e);
				ms.WriteUInt32(0, e);
				ms.WriteUInt32(0x20, e);
				ms.WriteAlign(0x20);
				long headerstart = ms.Position;
				for (int i = 0; i < packs.Count; ++i) {
					var p = packs[i];
					ms.WriteAscii(p.Name, 0x10);
					ms.WriteUInt32(p.Unknown, e);
					// file offsets, we'll fill these in later...
					ms.WriteUInt32(0xffffffffu, e);
					ms.WriteUInt32(0xffffffffu, e);
					ms.WriteUInt32(0xffffffffu, e);
				}
				for (int i = 0; i < packs.Count; ++i) {
					var p = packs[i];
					for (int j = 0; j < 3; ++j) {
						DuplicatableStream inject = j == 0 ? p.File0 : j == 1 ? p.File1 : p.File2;
						if (inject != null) {
							long pos = ms.Position;
							ms.Position = headerstart + (i * 0x20) + 0x14 + (j * 4);
							ms.WriteUInt32((uint)pos, e);
							ms.Position = pos;
							using (var ds = inject.Duplicate()) {
								ds.Position = 0;
								StreamUtils.CopyStream(ds, ms);
							}
							ms.WriteAlign(0x10);
						}
					}
				}

				return ms.CopyToByteArrayStreamAndDispose();
			}
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

		public class SpkdPackFileData {
			public string Name;
			public uint Unknown;
			public DuplicatableStream File0;
			public DuplicatableStream File1;
			public DuplicatableStream File2;
		}
	}
}
