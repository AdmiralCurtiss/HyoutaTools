using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.BTLBDAT {
	public class BTLBDAT {
		public BTLBDAT( String filename, Util.Endianness endian ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream, endian ) ) {
					throw new Exception( "Loading BTLBDAT failed!" );
				}
			}
		}

		public BTLBDAT( Stream stream, Util.Endianness endian ) {
			if ( !LoadFile( stream, endian ) ) {
				throw new Exception( "Loading BTLBDAT failed!" );
			}
		}

		public List<BattleBookEntry> BattleBookEntryList;

		private bool LoadFile( Stream stream, Util.Endianness endian ) {
			string magic = stream.ReadAscii( 8 );
			uint entryCount = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );

			BattleBookEntryList = new List<BattleBookEntry>( (int)entryCount );
			for ( uint i = 0; i < entryCount; ++i ) {
				BattleBookEntry e = new BattleBookEntry( stream, endian );
				BattleBookEntryList.Add( e );
			}

			return true;
		}
	}
}
