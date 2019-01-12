using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTXTM {
	public class T8BTXTMA {
		// area definitions
		public T8BTXTMA( String filename, Util.Endianness endian, Util.Bitness bits ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, System.IO.FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian, bits ) ) {
					throw new Exception( "Loading T8BTXTMA failed!" );
				}
			}
		}

		public T8BTXTMA( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			if ( !LoadFile( stream, endian, bits ) ) {
				throw new Exception( "Loading T8BTXTMA failed!" );
			}
		}

		public List<FloorInfo> FloorList;

		private bool LoadFile( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			string magic = stream.ReadAscii( 8 );
			uint floorInfoCount = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );

			FloorList = new List<FloorInfo>( (int)floorInfoCount );
			for ( uint i = 0; i < floorInfoCount; ++i ) {
				FloorInfo fi = new FloorInfo( stream, refStringStart, endian, bits );
				FloorList.Add( fi );
			}

			return true;
		}
	}
}
