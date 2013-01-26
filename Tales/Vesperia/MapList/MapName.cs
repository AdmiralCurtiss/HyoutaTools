using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.MapList {
	class MapName {
		public uint Pointer1;
		public uint Pointer2;
		public uint Pointer3;

		public String Name1 = null;
		public String Name2 = null;
		public String Name3 = null;

		public MapName( byte[] Bytes, uint Offset, uint Textstart ) {
			Pointer1 = Util.SwapEndian( BitConverter.ToUInt32( Bytes, (int)Offset ) );
			Pointer1 += Textstart;
			Pointer2 = Util.SwapEndian( BitConverter.ToUInt32( Bytes, (int)( Offset + 4 ) ) );
			Pointer2 += Textstart;
			Pointer3 = Util.SwapEndian( BitConverter.ToUInt32( Bytes, (int)( Offset + 8 ) ) );
			Pointer3 += Textstart;
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();

			bool first = true;

			if ( !String.IsNullOrEmpty( Name1 ) ) {
				sb.Append( Name1 );
				first = false;
			}
			if ( !String.IsNullOrEmpty( Name2 ) ) {
				if ( !first ) sb.Append( " | " );
				sb.Append( Name2 );
				first = false;
			}
			if ( !String.IsNullOrEmpty( Name3 ) ) {
				if ( !first ) sb.Append( " | " );
				sb.Append( Name3 );
				first = false;
			}

			return sb.ToString();
		}
	}
}
