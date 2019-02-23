using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTEMGP {
	public class T8BTEMGP {
		public T8BTEMGP( String filename, Util.Endianness endian, Util.Bitness bits ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, System.IO.FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian, bits ) ) {
					throw new Exception( "Loading T8BTEMGP failed!" );
				}
			}
		}

		public T8BTEMGP( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			if ( !LoadFile( stream, endian, bits ) ) {
				throw new Exception( "Loading T8BTEMGP failed!" );
			}
		}

		public List<EnemyGroup> EnemyGroupList;
		public Dictionary<uint, EnemyGroup> EnemyGroupIdDict;

		private bool LoadFile( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			string magic = stream.ReadAscii( 8 );
			if ( magic != "T8BTEMGP" ) {
				throw new Exception( "Invalid magic." );
			}
			uint enemyGroupCount = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );

			EnemyGroupList = new List<EnemyGroup>( (int)enemyGroupCount );
			for ( uint i = 0; i < enemyGroupCount; ++i ) {
				EnemyGroup s = new EnemyGroup( stream, refStringStart, endian, bits );
				EnemyGroupList.Add( s );
			}

			EnemyGroupIdDict = new Dictionary<uint, EnemyGroup>( EnemyGroupList.Count );
			foreach ( EnemyGroup e in EnemyGroupList ) {
				EnemyGroupIdDict.Add( e.InGameID, e );
			}

			return true;
		}
	}
}
