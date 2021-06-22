using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;
using EndianUtils = HyoutaUtils.EndianUtils;

namespace HyoutaTools.Tales.Vesperia.Credits {
	public class CreditsInfo {
		public List<CreditsInfoSingle> items;
		public uint CreditInfoStartOffset = 0;
		public uint CreditInfoCount = 0;
		public byte[] File;

		public CreditsInfo( string Filename ) {
			Initialize( System.IO.File.ReadAllBytes( Filename ) );
		}

		private void Initialize( byte[] file ) {
			this.File = file;
			CreditInfoStartOffset = EndianUtils.SwapEndian( BitConverter.ToUInt32( file, 0x24 ) );
			CreditInfoCount = EndianUtils.SwapEndian( BitConverter.ToUInt32( file, (int)CreditInfoStartOffset ) );
			CreditInfoCount = 2995; // PS3 // not accurate in file...?
			//CreditInfoCount = 0x1ACE8 / 40; // 360

			items = new List<CreditsInfoSingle>( (int)CreditInfoCount );
			for ( int i = 0; i < CreditInfoCount; ++i ) {
				items.Add( new CreditsInfoSingle( this, (int)CreditInfoStartOffset + 4 + i * 40, file ) );
			}

		}

		public string GetInFileString( int pointer ) {
			return TextUtils.GetTextUTF8( File, pointer );
		}
	}
}
