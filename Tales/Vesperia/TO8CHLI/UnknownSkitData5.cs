using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class UnknownSkitData5 {
		public ulong Unknown1;
		public UnknownSkitData5( System.IO.Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			Unknown1 = stream.ReadUInt( bits ).FromEndian( endian );
		}

		public override string ToString() {
			return Unknown1.ToString();
		}
	}
}
