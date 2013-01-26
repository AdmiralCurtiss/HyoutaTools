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

				m.Name1 = Util.GetText( (int)m.Pointer1, Bytes );
				m.Name2 = Util.GetText( (int)m.Pointer2, Bytes );
				m.Name3 = Util.GetText( (int)m.Pointer3, Bytes );

				MapNames.Add( m );
			}
		}
	}
}
