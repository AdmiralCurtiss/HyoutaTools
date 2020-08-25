using HyoutaPluginBase;
using HyoutaUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Graces {
	public class SHBP {
		public List<uint> Hashes;

		public SHBP(DuplicatableStream duplicatableStream, EndianUtils.Endianness e = EndianUtils.Endianness.BigEndian) {
			using (DuplicatableStream s = duplicatableStream.Duplicate()) {
				uint magic = s.ReadUInt32(EndianUtils.Endianness.LittleEndian);
				if (magic != 0x50424853) {
					throw new Exception("wrong magic");
				}

				uint count = s.ReadUInt32(e);
				Hashes = new List<uint>((int)count);
				for (uint i = 0; i < count; ++i) {
					Hashes.Add(s.ReadUInt32(e));
				}
			}
		}

		public DuplicatableStream WriteToStream(EndianUtils.Endianness e = EndianUtils.Endianness.BigEndian) {
			using (MemoryStream ms = new MemoryStream()) {
				ms.WriteUInt32(0x50424853, EndianUtils.Endianness.LittleEndian);
				ms.WriteUInt32((uint)Hashes.Count, e);
				for (int i = 0; i < Hashes.Count; ++i) {
					ms.WriteUInt32(Hashes[i], e);
				}
				return ms.CopyToByteArrayStreamAndDispose();
			}
		}
	}
}
