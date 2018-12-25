using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHRD {
	public class OtherModelAddition {
		public string Str;
		byte Unknown1;
		byte Unknown2;

		public OtherModelAddition( System.IO.Stream stream, uint refStringStart, Util.Endianness endian ) {
			Str = stream.ReadAsciiNulltermFromLocationAndReset( stream.ReadUInt32().FromEndian( endian ) + refStringStart );
			Unknown1 = (byte)stream.ReadByte();
			Unknown2 = (byte)stream.ReadByte();
			stream.DiscardBytes( 0x1A );
		}

		public override string ToString() {
			return Str;
		}
	}
}
