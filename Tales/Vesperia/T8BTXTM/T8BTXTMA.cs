using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTXTM {
	public class T8BTXTMA {
		// area definitions
		public T8BTXTMA( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading T8BTXTMA failed!" );
				}
			}
		}

		public T8BTXTMA( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading T8BTXTMA failed!" );
			}
		}

		public List<FloorInfo> SynopsisList;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint floorInfoCount = stream.ReadUInt32().SwapEndian();
			uint refStringStart = stream.ReadUInt32().SwapEndian();

			SynopsisList = new List<FloorInfo>( (int)floorInfoCount );
			for ( uint i = 0; i < floorInfoCount; ++i ) {
				FloorInfo fi = new FloorInfo( stream, refStringStart );
				SynopsisList.Add( fi );
			}

			return true;
		}
	}
}
