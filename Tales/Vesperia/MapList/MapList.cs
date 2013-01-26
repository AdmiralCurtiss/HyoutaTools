using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.MapList {
	class MapList {
		public uint Liststart;
		public uint Textstart;

		public List<MapName> MapNames;

		public MapList( byte[] Bytes ) {
			Liststart = Util.SwapEndian( BitConverter.ToUInt32( Bytes, 0x0C ) );
			Textstart = Util.SwapEndian( BitConverter.ToUInt32( Bytes, 0x14 ) );

			MapNames = new List<MapName>();

			for ( uint i = Liststart; i < Textstart; i += 0x20 ) {
				MapName m = new MapName( Bytes, i, Textstart );

				m.Name1 = Util.GetTextShiftJis( Bytes, (int)m.Pointer1 );
				m.Name2 = Util.GetTextShiftJis( Bytes, (int)m.Pointer2 );
				m.Name3 = Util.GetTextShiftJis( Bytes, (int)m.Pointer3 );

				MapNames.Add( m );
			}
		}
	}
}
