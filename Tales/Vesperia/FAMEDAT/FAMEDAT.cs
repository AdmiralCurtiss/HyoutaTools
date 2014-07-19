using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.FAMEDAT {
	public class FAMEDAT {
		public FAMEDAT( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading FAMEDAT failed!" );
				}
			}
		}

		public FAMEDAT( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading FAMEDAT failed!" );
			}
		}

		public List<Title> TitleList;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint unknown = stream.ReadUInt32().SwapEndian();
			uint titleCount = stream.ReadUInt32().SwapEndian();

			TitleList = new List<Title>( (int)titleCount );
			for ( uint i = 0; i < titleCount; ++i ) {
				Title t = new Title( stream );
				TitleList.Add( t );
			}

			return true;
		}
	}
}
