using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHRD {
	public class Unknown0x20byteAreaB {
		float[] UnknownFloats;
		byte UnknownByte;

		public Unknown0x20byteAreaB( System.IO.Stream stream, uint refStringStart, Util.Endianness endian ) {
			UnknownFloats = new float[4];
			for ( int i = 0; i < UnknownFloats.Length; ++i ) {
				UnknownFloats[i] = stream.ReadUInt32().FromEndian( endian ).UIntToFloat();
			}
			UnknownByte = (byte)stream.ReadByte();
			stream.DiscardBytes( 0x0F );
		}
	}
}
