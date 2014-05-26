// from http://www.sanity-free.com/133/crc_16_ccitt_in_csharp.html

using System;

namespace HyoutaTools.Checksum {
	public enum InitialCrcValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F }

	public class Crc16Ccitt {
		private static Crc16Ccitt _StandardAlgorithm = null;
		public static Crc16Ccitt StandardAlgorithm {
			get {
				if ( _StandardAlgorithm == null ) {
					_StandardAlgorithm = new Checksum.Crc16Ccitt( Checksum.InitialCrcValue.NonZero1 );
				}
				return _StandardAlgorithm;
			}
		}

		const ushort poly = 4129;
		ushort[] table = new ushort[256];
		ushort initialValue = 0;

		public ushort ComputeChecksum( byte[] bytes ) {
			return ComputeChecksum( bytes, 0, bytes.Length );
		}
		public ushort ComputeChecksum( byte[] bytes, int offset, int length ) {
			ushort crc = this.initialValue;
			for ( int i = 0; i < length; ++i ) {
				crc = (ushort)( ( crc << 8 ) ^ table[( ( crc >> 8 ) ^ ( 0xff & bytes[offset + i] ) )] );
			}
			return crc;
		}

		public byte[] ComputeChecksumBytes( byte[] bytes ) {
			return ComputeChecksumBytes( bytes, 0, bytes.Length );
		}
		public byte[] ComputeChecksumBytes( byte[] bytes, int offset, int length ) {
			ushort crc = ComputeChecksum( bytes, offset, length );
			return BitConverter.GetBytes( crc );
		}

		public Crc16Ccitt( InitialCrcValue initialValue ) {
			this.initialValue = (ushort)initialValue;
			ushort temp, a;
			for ( int i = 0; i < table.Length; ++i ) {
				temp = 0;
				a = (ushort)( i << 8 );
				for ( int j = 0; j < 8; ++j ) {
					if ( ( ( temp ^ a ) & 0x8000 ) != 0 ) {
						temp = (ushort)( ( temp << 1 ) ^ poly );
					} else {
						temp <<= 1;
					}
					a <<= 1;
				}
				table[i] = temp;
			}
		}
	}
}
