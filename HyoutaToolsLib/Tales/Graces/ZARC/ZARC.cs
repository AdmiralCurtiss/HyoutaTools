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

		public List<ZARCFileInfo> Files;

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
					Stream.Position = position;
					var ms = new MemoryStream();
					for (uint i = 0; i <= fi.BlockCount; ++i) {
						var decoder = new SevenZip.Compression.LZMA.Decoder();
						decoder.SetDecoderProperties(Stream.ReadBytes(5));
						long length = Stream.ReadInt64(EndianUtils.Endianness.LittleEndian);
						decoder.Code(Stream, ms, Stream.Length - Stream.Position, length, null);
					}
					return new FileFromStream(ms.CopyToByteArrayStreamAndDispose());
				}
			} catch (Exception ex) {
				// maybe not compressed in the first place?
				Console.WriteLine("Decompression failure, assuming uncompressed: " + index);
				return new FileFromStream(new PartialStream(Stream, position, (long)fi.FileLength));
			}
		}

		public static ulong CalculateFilenameHash(string str) {
			if (str.Length == 0) {
				return 0;
			}

			byte[] bytes = Encoding.UTF8.GetBytes(str.ToLowerInvariant());
			uint seed1 = 0x10215681;
			uint seed2 = (seed1 >> 16) | (seed1 << 16);
			uint hash1 = 0xffffffff;
			uint hash2 = 0xffffffff;
			for (int i = 0; i < bytes.Length; ++i) {
				uint b = bytes[i];
				hash1 = (hash1 ^ b);
				hash2 = (hash2 ^ b);
				for (int j = 0; j < 8; ++j) {
					hash1 = (uint)((-(hash1 & 1) & seed1) ^ (hash1 >> 1));
					hash2 = (uint)((-(hash2 & 1) & seed2) ^ (hash2 >> 1));
				}
			}
			hash1 = ~hash1;
			hash2 = ~hash2;
			return ((ulong)hash1 << 32) | ((ulong)hash2);
		}
	}

	public class ZARCFileInfo {
		public ulong FilenameHash; // starts at E0... and goes to FF..., sorted
		public uint BlockCount; // 3 bytes, number of compression blocks minus one, I think?
		public uint FileLength; // this is the last 16 bits of the file length, no idea where the remaining bits are
		public ushort Unknown2; // i thought this was some kind of flags but maybe not?
		public ushort Unknown3; // no idea???
		public uint Unknown4; // 3 bytes, some kind of index
		public uint FileOffset; // must be multiplied with Alignment

		public ZARCFileInfo(DuplicatableStream s, EndianUtils.Endianness e) {
			FilenameHash = s.ReadUInt64(e);
			BlockCount = s.ReadUInt24(e);
			FileLength = s.ReadUInt16(e);
			Unknown2 = s.ReadUInt16(e);
			Unknown3 = s.ReadUInt16(e);
			Unknown4 = s.ReadUInt24(e);
			FileOffset = s.ReadUInt32(e);
		}
	}
}
