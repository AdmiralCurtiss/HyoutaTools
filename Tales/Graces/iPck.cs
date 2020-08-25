using HyoutaPluginBase;
using HyoutaUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Graces {
	public class iPck {
		public List<byte[]> Data;

		public iPck(List<byte[]> data) {
			Data = data;
		}

		public iPck(DuplicatableStream duplicatableStream, EndianUtils.Endianness e = EndianUtils.Endianness.BigEndian) {
			using (DuplicatableStream s = duplicatableStream.Duplicate()) {
				uint magic = s.ReadUInt32(EndianUtils.Endianness.LittleEndian);
				if (magic != 0x6b635069) {
					throw new Exception("wrong magic");
				}

				uint count = s.ReadUInt32(e);
				uint offsetsStart = s.ReadUInt32(e);
				uint dataStart = s.ReadUInt32(e);

				s.Position = offsetsStart;
				uint[] offsets = s.ReadUInt32Array(count, e);

				s.Position = dataStart;
				long dataEnd = s.Length - dataStart;
				Data = new List<byte[]>((int)count);
				for (uint i = 0; i < count; ++i) {
					long start = offsets[i];
					if (start == 0xffffffffu) {
						Data.Add(null);
					} else {
						long end = FindFileEnd(offsets, i, dataEnd);
						long len = end - start;
						s.Position = dataStart + start;
						Data.Add(s.ReadUInt8Array(len));
					}
				}
			}
		}

		private static long FindFileEnd(uint[] offsets, uint index, long eof) {
			++index;
			while (index < offsets.Length) {
				uint end = offsets[index];
				if (end != 0xffffffffu) {
					return end;
				}
				++index;
			}
			return eof;
		}

		public DuplicatableStream WriteToStream(EndianUtils.Endianness e = EndianUtils.Endianness.BigEndian) {
			using (MemoryStream ms = new MemoryStream()) {
				ms.WriteUInt32(0x6b635069, EndianUtils.Endianness.LittleEndian);
				ms.WriteUInt32((uint)Data.Count, e);
				ms.WriteUInt32(0x10, e);
				ms.WriteUInt32((uint)(0x10 + Data.Count * 4), e);
				uint datapos = 0;
				for (int i = 0; i < Data.Count; ++i) {
					if (Data[i] != null) {
						ms.WriteUInt32(datapos, e);
						datapos += (uint)Data[i].Length;
					} else {
						ms.WriteUInt32(0xffffffffu, e);
					}
				}
				for (int i = 0; i < Data.Count; ++i) {
					if (Data[i] != null) {
						ms.Write(Data[i]);
					}
				}
				return ms.CopyToByteArrayStreamAndDispose();
			}
		}
	}
}
