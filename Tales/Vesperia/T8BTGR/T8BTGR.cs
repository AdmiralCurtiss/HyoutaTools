using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTGR {
	public class T8BTGR {
		public T8BTGR( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading T8BTGR failed!" );
				}
			}
		}

		public T8BTGR( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading T8BTGR failed!" );
			}
		}

		public List<GradeShopEntry> GradeShopEntryList;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint entryCount = stream.ReadUInt32().SwapEndian();
			uint refStringStart = stream.ReadUInt32().SwapEndian();

			GradeShopEntryList = new List<GradeShopEntry>( (int)entryCount );
			for ( uint i = 0; i < entryCount; ++i ) {
				GradeShopEntry e = new GradeShopEntry( stream, refStringStart );
				GradeShopEntryList.Add( e );
			}

			return true;
		}
	}
}
