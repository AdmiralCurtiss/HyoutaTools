using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTMA {
	class T8BTMA {
		public T8BTMA( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "Loading T8BTMA failed!" );
			}
		}

		public T8BTMA( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "Loading T8BTMA failed!" );
			}
		}

		public List<Arte> ArteList;

		private bool LoadFile( byte[] Bytes ) {
			uint arteCount = BitConverter.ToUInt32( Bytes, 0x8 ).SwapEndian();

			uint location = 0x10;
			ArteList = new List<Arte>( (int)arteCount );
			for ( uint i = 0; i < arteCount; ++i ) {
				uint entrySize = BitConverter.ToUInt32( Bytes, (int)location ).SwapEndian();
				Arte a = new Arte( Bytes, location, entrySize );
				ArteList.Add( a );
				location += entrySize;
			}

			return true;
		}
	}
}
