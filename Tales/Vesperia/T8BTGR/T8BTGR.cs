using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTGR {
	public class T8BTGR {
		public T8BTGR( String filename, Util.Endianness endian, Util.Bitness bits ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, System.IO.FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian, bits ) ) {
					throw new Exception( "Loading T8BTGR failed!" );
				}
			}
		}

		public T8BTGR( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			if ( !LoadFile( stream, endian, bits ) ) {
				throw new Exception( "Loading T8BTGR failed!" );
			}
		}

		public List<GradeShopEntry> GradeShopEntryList;

		private bool LoadFile( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			string magic = stream.ReadAscii( 8 );
			if ( magic != "T8BTGR  " ) {
				throw new Exception( "Invalid magic." );
			}
			uint entryCount = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );

			GradeShopEntryList = new List<GradeShopEntry>( (int)entryCount );
			for ( uint i = 0; i < entryCount; ++i ) {
				GradeShopEntry e = new GradeShopEntry( stream, refStringStart, endian, bits );
				GradeShopEntryList.Add( e );
			}

			return true;
		}
	}
}
