using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.BTLBDAT {
	public class BTLBDAT {
		public BTLBDAT( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading BTLBDAT failed!" );
				}
			}
		}

		public BTLBDAT( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading BTLBDAT failed!" );
			}
		}

		public List<BattleBookEntry> BattleBookEntryList;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint entryCount = stream.ReadUInt32().SwapEndian();
			uint refStringStart = stream.ReadUInt32().SwapEndian();

			BattleBookEntryList = new List<BattleBookEntry>( (int)entryCount );
			for ( uint i = 0; i < entryCount; ++i ) {
				BattleBookEntry e = new BattleBookEntry( stream );
				BattleBookEntryList.Add( e );
			}

			return true;
		}
	}
}
