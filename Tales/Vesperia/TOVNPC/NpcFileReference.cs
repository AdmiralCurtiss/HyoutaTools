using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TOVNPC {
	public class NpcFileReference {
		public string Map;
		public string Filename;
		public uint Filesize;

		public NpcFileReference( System.IO.Stream stream, uint refStringStart, Util.Endianness endian, Util.Bitness bits ) {
			ulong refStringLocation1 = stream.ReadUInt( bits, endian );
			ulong refStringLocation2 = stream.ReadUInt( bits, endian );
			Filesize = stream.ReadUInt32().FromEndian( endian );
			stream.ReadUInt32();

			Map = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + refStringLocation1 ) );
			Filename = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + refStringLocation2 ) );
		}

		public override string ToString() {
			return Map + " / " + Filename + " / " + Filesize;
		}
	}
}
