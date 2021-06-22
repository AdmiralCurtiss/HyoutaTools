using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.FAMEDAT {
	public class FAMEDAT {
		public FAMEDAT( String filename, EndianUtils.Endianness endian ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian ) ) {
					throw new Exception( "Loading FAMEDAT failed!" );
				}
			}
		}

		public FAMEDAT( Stream stream, EndianUtils.Endianness endian ) {
			if ( !LoadFile( stream, endian ) ) {
				throw new Exception( "Loading FAMEDAT failed!" );
			}
		}

		public List<Title> TitleList;

		private bool LoadFile( Stream stream, EndianUtils.Endianness endian ) {
			string magic = stream.ReadAscii( 8 );
			uint unknown = stream.ReadUInt32().FromEndian( endian );
			uint titleCount = stream.ReadUInt32().FromEndian( endian );

			TitleList = new List<Title>( (int)titleCount );
			for ( uint i = 0; i < titleCount; ++i ) {
				Title t = new Title( stream, endian );
				TitleList.Add( t );
			}

			return true;
		}
	}
}
