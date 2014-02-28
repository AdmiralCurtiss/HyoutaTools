using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PicrossDS {
	class SaveFile {
		public SaveFile( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "PicrossDS.SaveFile: Load Failed!" );
			}
		}

		public SaveFile( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "PicrossDS.SaveFile: Load Failed!" );
			}
		}

		byte[] File;

		private bool LoadFile( byte[] File ) {
			this.File = File;
			return true;
		}

		public void RecalculateChecksum() {
			byte[] checksumTable = new byte[] { 1, 2, 4, 8,
											    2, 4, 8, 1,
											    4, 8, 1, 2,
											    8, 1, 2, 4 };

			uint[] checksum = new uint[] { 0, 0, 0, 0 };
			for ( uint i = 0; i < 0x39D00; ++i ) {
				checksum[0] += (uint)( File[i] * checksumTable[( i + 0 ) % 16] );
				checksum[1] += (uint)( File[i] * checksumTable[( i + 1 ) % 16] );
				checksum[2] += (uint)( File[i] * checksumTable[( i + 2 ) % 16] );
				checksum[3] += (uint)( File[i] * checksumTable[( i + 3 ) % 16] );
			}

			for ( int i = 0; i < 4; ++i ) {
				Util.CopyByteArrayPart( BitConverter.GetBytes( checksum[i] ), 0, File, 0x39E00 + i * 4, 4 );
			}
		}

		public void WriteFile( string Filename ) {
			RecalculateChecksum();
			System.IO.File.WriteAllBytes( Filename, File );
		}
	}
}
