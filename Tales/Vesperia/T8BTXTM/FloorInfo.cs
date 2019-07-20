using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.T8BTXTM {
	public class FloorInfo {
		public uint EntrySize;

		public string RefString1;
		public string RefString2;

		public FloorInfo( System.IO.Stream stream, uint refStringStart, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			uint atLeastBytes = ( 0x10 + 2 * ( bits.NumberOfBytes() ) );
			EntrySize = stream.ReadUInt32().FromEndian( endian );
			if ( EntrySize < atLeastBytes ) {
				throw new Exception( "This file confuses me." );
			}

			uint unknown2 = stream.ReadUInt32().FromEndian( endian );
			uint unknown3 = stream.ReadUInt32().FromEndian( endian );
			ulong unknown4 = stream.ReadUInt( bits, endian );
			uint unknown5 = stream.ReadUInt32().FromEndian( endian );
			ulong unknown6 = stream.ReadUInt( bits, endian );
			if ( EntrySize > atLeastBytes ) {
				stream.DiscardBytes( EntrySize - atLeastBytes );
			}

			RefString1 = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + unknown4 ) );
			RefString2 = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + unknown6 ) );
		}

		public override string ToString() {
			return RefString1 + " / " + RefString2;
		}
	}
}
