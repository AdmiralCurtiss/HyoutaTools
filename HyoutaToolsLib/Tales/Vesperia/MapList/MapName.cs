using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.MapList {
	public class MapName {
		public string Name1;
		public string Name2;
		public string Name3;
		public short Unknown6a;

		public MapName( System.IO.Stream stream, uint textstart, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			ulong p1 = stream.ReadUInt( bits, endian );
			ulong p2 = stream.ReadUInt( bits, endian );
			ulong p3 = stream.ReadUInt( bits, endian );
			ulong unknown4 = stream.ReadUInt( bits, endian ); // either this or unknown5 is bitness affected, probably?
			Util.Assert( unknown4 == 0 );

			uint unknown5 = stream.ReadUInt32().FromEndian( endian );
			Util.Assert( unknown5 == 0 );
			Unknown6a = stream.ReadUInt16().FromEndian( endian ).AsSigned();
			uint unknown6b = stream.ReadUInt16().FromEndian( endian );
			Util.Assert( unknown6b == 0 );
			uint unknown7 = stream.ReadUInt32().FromEndian( endian );
			Util.Assert( unknown7 == 0 );
			uint unknown8 = stream.ReadUInt32().FromEndian( endian );
			Util.Assert( unknown8 == 0 );

			Name1 = stream.ReadAsciiNulltermFromLocationAndReset( (long)( textstart + p1 ) );
			Name2 = stream.ReadAsciiNulltermFromLocationAndReset( (long)( textstart + p2 ) );
			Name3 = stream.ReadAsciiNulltermFromLocationAndReset( (long)( textstart + p3 ) );
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();

			sb.Append( Unknown6a );
			if ( !String.IsNullOrEmpty( Name1 ) ) {
				sb.Append( " | " );
				sb.Append( Name1 );
			}
			if ( !String.IsNullOrEmpty( Name2 ) ) {
				sb.Append( " | " );
				sb.Append( Name2 );
			}
			if ( !String.IsNullOrEmpty( Name3 ) ) {
				sb.Append( " | " );
				sb.Append( Name3 );
			}

			return sb.ToString();
		}
	}
}
