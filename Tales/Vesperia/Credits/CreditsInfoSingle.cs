using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.Credits {
	public enum CreditsData {
		EntryNumber = 1,
		ItemDataCount = 10
	}

	public class CreditsInfoSingle {
		public static int Size = 40;
		public int Offset = 0;
		public UInt32[] Data;				

		public CreditsInfoSingle( int offset, byte[] file ) {
			this.Offset = offset;
			this.Data = new UInt32[Size / 4];
			for ( int i = 0; i < Size / 4; ++i ) {
				Data[i] = Util.SwapEndian( BitConverter.ToUInt32( file, offset + i * 0x04 ) );
			}

			return;
		}

		public override string ToString() {
			return "";
		}
	}
}
