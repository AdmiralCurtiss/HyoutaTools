using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Pokemon.Gen5.PWT {
	class DownloadFile {
		public DownloadFile( byte[] data, int offset ) {
			if ( !Load( data, offset ) ) {
				throw new Exception( this.GetType().Name + ": Load Failed!" );
			}
		}
		public DownloadFile( string filename ) {
			if ( !Load( System.IO.File.ReadAllBytes( filename ), 0 ) ) {
				throw new Exception( this.GetType().Name + ": Load Failed!" );
			}
		}


		public Tournament[] Tournaments;

		private bool Load( byte[] data, int offset ) {
			Tournaments = new Tournament[12];

			for ( int i = 0; i < 12; ++i ) {
				Tournaments[i] = new Tournament( data, offset + i * 4628 );
			}

			return true;
		}

		public byte[] GetFile() {
			byte[] data = new byte[55540];

			for ( int i = 0; i < 12; ++i ) {
				Tournaments[i].RecalculateChecksum();
				Util.CopyByteArrayPart( Tournaments[i].Data, 0, data, i * 4628, 4628 );
			}

			var checksum = Checksum.Crc16Ccitt.StandardAlgorithm.ComputeChecksumBytes( data, 0, 55536 );
			Util.CopyByteArrayPart( checksum, 0, data, 55536, 2 );
			data[55538] = 0x00; data[55539] = 0x00;

			return data;
		}
	}
}
