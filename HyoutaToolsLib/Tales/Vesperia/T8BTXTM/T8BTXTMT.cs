using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.T8BTXTM {
	public class T8BTXTMT {
		// treasure chest definitions
		public T8BTXTMT( String filename, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, System.IO.FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian, bits ) ) {
					throw new Exception( "Loading T8BTXTMT failed!" );
				}
			}
		}

		public T8BTXTMT( Stream stream, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			if ( !LoadFile( stream, endian, bits ) ) {
				throw new Exception( "Loading T8BTXTMT failed!" );
			}
		}

		public List<TreasureInfo> TreasureInfoList;

		private bool LoadFile( Stream stream, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			string magic = stream.ReadAscii( 8 );
			if ( magic != "T8BTXTMT" ) {
				throw new Exception( "Invalid magic." );
			}
			uint infoCount = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );

			TreasureInfoList = new List<TreasureInfo>( (int)infoCount );
			for ( uint i = 0; i < infoCount; ++i ) {
				TreasureInfo ti = new TreasureInfo( stream, refStringStart, endian, bits );
				TreasureInfoList.Add( ti );
			}

			return true;
		}
	}
}
