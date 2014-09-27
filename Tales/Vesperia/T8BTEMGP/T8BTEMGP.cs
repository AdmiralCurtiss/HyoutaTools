using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTEMGP {
	public class T8BTEMGP {
		public T8BTEMGP( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading T8BTEMGP failed!" );
				}
			}
		}

		public T8BTEMGP( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading T8BTEMGP failed!" );
			}
		}

		public List<EnemyGroup> EnemyGroupList;
		public Dictionary<uint, EnemyGroup> EnemyGroupIdDict;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint enemyGroupCount = stream.ReadUInt32().SwapEndian();
			uint refStringStart = stream.ReadUInt32().SwapEndian();

			EnemyGroupList = new List<EnemyGroup>( (int)enemyGroupCount );
			for ( uint i = 0; i < enemyGroupCount; ++i ) {
				EnemyGroup s = new EnemyGroup( stream, refStringStart );
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
