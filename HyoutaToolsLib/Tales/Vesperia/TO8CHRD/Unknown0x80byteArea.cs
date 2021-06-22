using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.TO8CHRD {
	public class Unknown0x80byteArea {
		uint UnknownUInt;
		byte UnknownByte1;
		byte UnknownByte2;
		float UnknownFloat1;
		float UnknownFloat2;

		public Unknown0x80byteArea( System.IO.Stream stream, uint refStringStart, EndianUtils.Endianness endian ) {
			UnknownUInt = stream.ReadUInt32().FromEndian( endian );
			stream.DiscardBytes( 4 );
			UnknownByte1 = (byte)stream.ReadByte();
			UnknownByte2 = (byte)stream.ReadByte();
			stream.DiscardBytes( 2 );
			UnknownFloat1 = stream.ReadUInt32().FromEndian( endian ).UIntToFloat();
			UnknownFloat2 = stream.ReadUInt32().FromEndian( endian ).UIntToFloat();

			stream.DiscardBytes( 0x6C );
		}
	}
}
