using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.BTLBDAT {
	public class BTLBDAT {
		public BTLBDAT( String filename, EndianUtils.Endianness endian ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian ) ) {
					throw new Exception( "Loading BTLBDAT failed!" );
				}
			}
		}

		public BTLBDAT( Stream stream, EndianUtils.Endianness endian ) {
			if ( !LoadFile( stream, endian ) ) {
				throw new Exception( "Loading BTLBDAT failed!" );
			}
		}

		public List<BattleBookEntry> BattleBookEntryList;

		private bool LoadFile( Stream stream, EndianUtils.Endianness endian ) {
			string magic = stream.ReadAscii( 8 );
			uint unknown1 = stream.ReadUInt32().FromEndian( endian );
			uint entryCount = stream.ReadUInt32().FromEndian( endian );
			uint unknown2 = stream.ReadUInt32().FromEndian( endian );
			stream.DiscardBytes( 12 );

			BattleBookEntryList = new List<BattleBookEntry>( (int)entryCount );
			for ( uint i = 0; i < entryCount; ++i ) {
				BattleBookEntry e = new BattleBookEntry( stream, endian );
				BattleBookEntryList.Add( e );
			}

			return true;
		}
	}
}
