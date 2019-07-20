using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class UnknownSkitData5 {
		public ulong Unknown1;
		public UnknownSkitData5( System.IO.Stream stream, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			Unknown1 = stream.ReadUInt( bits, endian );
		}

		public override string ToString() {
			return Unknown1.ToString();
		}
	}
}
