using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHRD {
	public class CustomModelAddition {
		public string Str;
		public CustomModelAddition( System.IO.Stream stream, uint refStringStart, Util.Endianness endian ) {
			Str = stream.ReadAsciiNulltermFromLocationAndReset( stream.ReadUInt32().FromEndian( endian ) + refStringStart );
		}

		public override string ToString() {
			return Str;
		}
	}
}
