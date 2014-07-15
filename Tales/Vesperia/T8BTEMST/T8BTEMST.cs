using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTEMST {
	public class T8BTEMST {
		public T8BTEMST( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading T8BTEMST failed!" );
				}
			}
		}

		public T8BTEMST( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading T8BTEMST failed!" );
			}
		}

		public List<Enemy> EnemyList;
		public Dictionary<uint, Enemy> EnemyIdDict;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint enemyCount = stream.ReadUInt32().SwapEndian();
			uint refStringStart = stream.ReadUInt32().SwapEndian();

			EnemyList = new List<Enemy>( (int)enemyCount );
			for ( uint i = 0; i < enemyCount; ++i ) {
				Enemy s = new Enemy( stream, refStringStart );
				EnemyList.Add( s );
			}

			EnemyIdDict = new Dictionary<uint, Enemy>( EnemyList.Count );
			foreach ( Enemy e in EnemyList ) {
				EnemyIdDict.Add( e.InGameID, e );
			}

			return true;
		}
	}
}
