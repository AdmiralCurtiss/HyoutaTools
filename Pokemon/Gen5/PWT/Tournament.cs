using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Pokemon.Gen5.PWT {
	public class Tournament {
		public Tournament( byte[] data, int offset ) {
			if ( !Load( data, offset ) ) {
				throw new Exception( "bscr: Load Failed!" );
			}
		}
		public Tournament( string filename ) {
			if ( !Load( System.IO.File.ReadAllBytes( filename ), 0 ) ) {
				throw new Exception( "bscr: Load Failed!" );
			}
		}

		public byte[] Data;

		public byte Unknown0x00 { get { return Data[0x00]; } }
		public byte Unknown0x01 { get { return Data[0x01]; } }
		public byte Unknown0x02 { get { return Data[0x02]; } }
		public byte Unknown0x03 { get { return Data[0x03]; } }
		public byte Unknown0x04 { get { return Data[0x04]; } }
		public byte Unknown0x05 { get { return Data[0x05]; } }
		public byte Unknown0x06 { get { return Data[0x06]; } }
		public byte Language { get { return Data[0x07]; } }
		public ushort ID { get { return BitConverter.ToUInt16( Data, 0x08 ); } }

		private bool Load( byte[] data, int offset ) {
			Data = new byte[4628];
			Util.CopyByteArrayPart( data, offset, this.Data, 0, this.Data.Length ); 

			return true;
		}

		public void RecalculateChecksum() {
			var checksum = Checksum.Crc16Ccitt.StandardAlgorithm.ComputeChecksumBytes( Data, 0, 4624 );
			Util.CopyByteArrayPart( checksum, 0, Data, 4624, 2 );
		}
	}
}
