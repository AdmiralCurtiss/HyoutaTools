using HyoutaPluginBase;
using HyoutaPluginBase.FileContainer;
using HyoutaTools.FileContainer;
using HyoutaTools.Tales.tlzc;
using HyoutaUtils;
using HyoutaUtils.Streams;
using SevenZip.Buffer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HyoutaTools.Tales.Vesperia.SpkdUnpack.SPKD;

namespace HyoutaTools.Tales.Graces.ZARC {
	public class ZARC : ContainerBase {
		private DuplicatableStream Stream;
		private uint Unknown1;
		private uint SizeOfHeader;
		private uint SizeOfSingleFileInfo;
		public uint NumberOfFiles { get; private set; }
		private uint Unknown2;
		private uint Unknown3;
		private uint Unknown4;
		private uint Alignment;
		private uint Unknown5;

		private List<ZARCFileInfo> Files;

		public ZARC(DuplicatableStream duplicatableStream, EndianUtils.Endianness e = EndianUtils.Endianness.BigEndian) {
			Stream = duplicatableStream.Duplicate();

			if (Stream.ReadAscii(4) != "ZARC") {
				throw new Exception("Wrong magic.");
			}

			// since I only have a single zarc file to compare with a lot of this is just guesses...
			Unknown1 = Stream.ReadUInt32(e);
			SizeOfHeader = Stream.ReadUInt32(e);
			SizeOfSingleFileInfo = Stream.ReadUInt32(e); // should be 0x18
			NumberOfFiles = Stream.ReadUInt32(e);
			Unknown2 = Stream.ReadUInt32(e);
			Unknown3 = Stream.ReadUInt32(e);
			Unknown4 = Stream.ReadUInt32(e);
			Alignment = Stream.ReadUInt32(e);
			Unknown5 = Stream.ReadUInt32(e);

			Files = new List<ZARCFileInfo>();
			for (uint i = 0; i < NumberOfFiles; ++i) {
				Files.Add(new ZARCFileInfo(Stream, e));
			}
		}

		public override void Dispose() {
			Stream.Dispose();
		}

		public override IEnumerable<string> GetChildNames() {
			return new List<string>();
		}

		public override INode GetChildByName(string name) {
			return null;
		}

		public override INode GetChildByIndex(long index) {
			ZARCFileInfo fi = Files[(int)index];
			long position = (long)(((ulong)fi.FileOffset) * Alignment);
			Stream.Position = position;
			try {
				if (Stream.PeekUInt32(EndianUtils.Endianness.BigEndian) == 0x544c5a43) {
					// tlzc-compressed
					Stream.Position = position + 8;
					uint length = Stream.ReadUInt32(EndianUtils.Endianness.LittleEndian);
					Stream.Position = position;
					return new FileFromStream(new DuplicatableByteArrayStream(TLZC.Decompress(new PartialStream(Stream, position, length).CopyToByteArrayAndDispose())));
				} else {
					// in-archive compressed, probably?
					var decoder = new SevenZip.Compression.LZMA.Decoder();
					Stream.Position = position;
					decoder.SetDecoderProperties(Stream.ReadBytes(5));
					long length = Stream.ReadInt64(EndianUtils.Endianness.LittleEndian);
					var ms = new MemoryStream();
					decoder.Code(Stream, ms, Stream.Length - Stream.Position, length, null);
					return new FileFromStream(ms.CopyToByteArrayStreamAndDispose());
				}
			} catch (Exception ex) {
				// maybe not compressed in the first place?
				return new FileFromStream(new PartialStream(Stream, position, (long)fi.FileLength));
			}
		}
	}

	public class ZARCFileInfo {
		public ulong Unknown1; // starts at E0... and goes to FF..., sorted
		public ulong FileLength; // this seems to be the file length but not always???
		public ushort Unknown2; // i though this was some kind of flags but maybe not?
		public ushort Unknown3; // no idea???
		public uint Unknown4; // 3 bytes, some kind of index
		public uint FileOffset; // must be multiplied with Alignment

		public ZARCFileInfo(DuplicatableStream s, EndianUtils.Endianness e) {
			Unknown1 = s.ReadUInt64(e);
			FileLength = s.ReadUInt40(e);
			Unknown2 = s.ReadUInt16(e);
			Unknown3 = s.ReadUInt16(e);
			Unknown4 = s.ReadUInt24(e);
			FileOffset = s.ReadUInt32(e);
		}
	}
}
