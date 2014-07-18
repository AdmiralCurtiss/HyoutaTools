using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.SYNPDAT {
	public class SYNPDAT {
		public SYNPDAT( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading SYNPDAT failed!" );
				}
			}
		}

		public SYNPDAT( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading SYNPDAT failed!" );
			}
		}

		public List<SynopsisEntry> SynopsisList;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint entrySize = stream.ReadUInt32().SwapEndian();
			uint synopsisCount = stream.ReadUInt32().SwapEndian();
			uint unknown = stream.ReadUInt32().SwapEndian();
			stream.DiscardBytes( 0xC );

			SynopsisList = new List<SynopsisEntry>( (int)synopsisCount );
			for ( uint i = 0; i < synopsisCount; ++i ) {
				SynopsisEntry l = new SynopsisEntry( stream );
				SynopsisList.Add( l );
			}

			return true;
		}
	}
}
