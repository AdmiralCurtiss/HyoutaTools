using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.Credits {
	public class CreditsInfo {
		public List<CreditsInfoSingle> items;
		public uint CreditInfoStartOffset = 0;
		public uint CreditInfoCount = 0;

		public CreditsInfo( string Filename ) {
			Initialize( System.IO.File.ReadAllBytes( Filename ) );
		}

		private void Initialize( byte[] file ) {
			CreditInfoStartOffset = Util.SwapEndian( BitConverter.ToUInt32( file, 0x24 ) );
			CreditInfoCount = Util.SwapEndian( BitConverter.ToUInt32( file, (int)CreditInfoStartOffset ) );
			CreditInfoCount = 2995; // PS3 // not accurate in file...?
			//CreditInfoCount = 0x1ACE8 / 40; // 360

			items = new List<CreditsInfoSingle>( (int)CreditInfoCount );
			for ( int i = 0; i < CreditInfoCount; ++i ) {
				items.Add( new CreditsInfoSingle( (int)CreditInfoStartOffset + 4 + i * 40, file ) );
			}

		}
	}
}
