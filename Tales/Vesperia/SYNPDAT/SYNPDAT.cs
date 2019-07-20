using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.SYNPDAT {
	public class SYNPDAT {
		public SYNPDAT( String filename, EndianUtils.Endianness endian ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream, endian ) ) {
					throw new Exception( "Loading SYNPDAT failed!" );
				}
			}
		}

		public SYNPDAT( Stream stream, EndianUtils.Endianness endian ) {
			if ( !LoadFile( stream, endian ) ) {
				throw new Exception( "Loading SYNPDAT failed!" );
			}
		}

		public List<SynopsisEntry> SynopsisList;

		private bool LoadFile( Stream stream, EndianUtils.Endianness endian ) {
			string magic = stream.ReadAscii( 8 );
			if ( magic != "SYNPDAT\0" ) {
				throw new Exception( "Invalid magic." );
			}
			uint entrySize = stream.ReadUInt32().FromEndian( endian );
			uint synopsisCount = stream.ReadUInt32().FromEndian( endian );
			uint unknown = stream.ReadUInt32().FromEndian( endian );
			stream.DiscardBytes( 0xC );

			SynopsisList = new List<SynopsisEntry>( (int)synopsisCount );
			for ( uint i = 0; i < synopsisCount; ++i ) {
				SynopsisEntry l = new SynopsisEntry( stream, endian );
				SynopsisList.Add( l );
			}

			return true;
		}
	}
}
