using HyoutaPluginBase;
using HyoutaUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Graces {
	public class SHBP {
		public List<uint> Hashes;
		public byte[] Rest;

		public SHBP(DuplicatableStream duplicatableStream) {
			EndianUtils.Endianness e = EndianUtils.Endianness.BigEndian;
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

				s.ReadAlign(0x10);

				// unclear what this is... it's compto compressed and only shows up for voice files, maybe lipsync or timing data?
				Rest = s.ReadUInt8Array(s.Length - s.Position);
			}
		}
	}
}
