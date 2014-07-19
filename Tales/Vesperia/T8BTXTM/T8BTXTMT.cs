using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTXTM {
	public class T8BTXTMT {
		// treasure chest definitions
		public T8BTXTMT( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading T8BTXTMT failed!" );
				}
			}
		}

		public T8BTXTMT( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading T8BTXTMT failed!" );
			}
		}

		public List<TreasureInfo> TreasureInfoList;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint infoCount = stream.ReadUInt32().SwapEndian();
			uint refStringStart = stream.ReadUInt32().SwapEndian();

			TreasureInfoList = new List<TreasureInfo>( (int)infoCount );
			for ( uint i = 0; i < infoCount; ++i ) {
				TreasureInfo ti = new TreasureInfo( stream, refStringStart );
				TreasureInfoList.Add( ti );
			}

			return true;
		}
	}
}
