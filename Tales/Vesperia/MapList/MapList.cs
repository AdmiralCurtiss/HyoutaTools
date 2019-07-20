using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.MapList {
	public class MapList {
		public List<MapName> MapNames;

		public MapList( string filename, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			using ( System.IO.Stream stream = new System.IO.FileStream( filename, System.IO.FileMode.Open, System.IO.FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian, bits ) ) {
					throw new Exception( "Loading MapList failed!" );
				}
			}
		}

		public MapList( System.IO.Stream stream, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			if ( !LoadFile( stream, endian, bits ) ) {
				throw new Exception( "Loading MapList failed!" );
			}
		}

		private bool LoadFile( System.IO.Stream stream, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			string magic = stream.ReadAscii( 8 );
			if ( magic != "TO8MAPL\0" ) {
				return false;
			}

			uint filesize = stream.ReadUInt32().FromEndian( endian );
			uint liststart = stream.ReadUInt32().FromEndian( endian );
			uint mapcount = stream.ReadUInt32().FromEndian( endian );
			uint textstart = stream.ReadUInt32().FromEndian( endian );
			uint littleEndianFilesize = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian ); // ???

			MapNames = new List<MapName>();

			stream.Position = liststart;
			for ( uint i = 0; i < mapcount; ++i ) {
				MapNames.Add( new MapName( stream, textstart, endian, bits ) );
			}

			return true;
		}
	}
}
