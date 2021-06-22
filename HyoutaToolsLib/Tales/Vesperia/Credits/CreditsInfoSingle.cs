using HyoutaTools.Tales.Vesperia.TSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EndianUtils = HyoutaUtils.EndianUtils;
using NumberUtils = HyoutaUtils.NumberUtils;

namespace HyoutaTools.Tales.Vesperia.Credits {
	public enum CreditsData {
		Type = 0,
		EntryNumber = 1,
		InFileOffset = 2,
		XResolution = 3,
		YResolution = 4,
		ItemDataCount = 10
	}

	public class CreditsInfoSingle {
		public static int Size = 40;
		public CreditsInfo Credits;
		public int Offset = 0;
		public UInt32[] Data;
		public TSSFile TSS = null; // access for GetEntry()/ToString()

		public CreditsInfoSingle( CreditsInfo Credits, int offset, byte[] file ) {
			this.Credits = Credits;
			this.Offset = offset;
			this.Data = new UInt32[Size / 4];
			for ( int i = 0; i < Size / 4; ++i ) {
				Data[i] = EndianUtils.SwapEndian( BitConverter.ToUInt32( file, offset + i * 0x04 ) );
			}

			return;
		}

		public override string ToString() {
			return
					Offset.ToString( "X6" ) + ": [" + Data[0] + "] "
					+ ( Data[0] == 2 ? " --- Image: " + Credits.GetInFileString( (int)Data[2] + 0xD60 ) + " --- " : "" )
					+ ( Data[0] == 3 ? " --- Free Space: " + NumberUtils.UIntToFloat( Data[4] ).ToString() + " --- " : "" )
					+ ( Data[0] == 5 ? " --- Text Size?: " + NumberUtils.UIntToFloat( Data[4] ).ToString() + " --- " : "" )
					+ ( TSS != null ? this.GetEntry( Data[(int)CreditsData.EntryNumber] ).StringJpn : "" )
			;
		}

		public TSSEntry GetEntry( uint ptr ) {
			try {
				int ItemStartInTss = 17763; // PS3
				//ItemStartInTss = 17763 + 216; // 360 with PS3 string_dic
				return TSS.Entries[ptr - 340000 + ItemStartInTss];
			} catch ( Exception ) {
				return new TSSEntry( new uint[0], "", "", 0, 0, -1 );
			}
		}
	}
}
