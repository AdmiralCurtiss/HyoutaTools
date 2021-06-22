using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EndianUtils = HyoutaUtils.EndianUtils;

namespace HyoutaTools.Trophy.Viewer {
	public class TropUsr {
		private byte[] File;

		public uint TrophyAmount;
		public uint UserInfoStart;
		public uint UserInfoSize;

		public Dictionary<uint, TropUsrSingleTrophy> TrophyInfos;

		public TropUsr( byte[] File ) {
			this.File = File;

			TrophyAmount = EndianUtils.SwapEndian( BitConverter.ToUInt32( File, 0xDC ) );
			UserInfoStart = EndianUtils.SwapEndian( BitConverter.ToUInt32( File, 0x144 ) );
			UserInfoSize = 0x70;

			TrophyInfos = new Dictionary<uint, TropUsrSingleTrophy>( (int)TrophyAmount );

			for ( uint i = 0; i < TrophyAmount; i++ ) {
				TropUsrSingleTrophy TrophyInfo = new TropUsrSingleTrophy( File, UserInfoStart + ( i * UserInfoSize ) );
				TrophyInfos.Add( TrophyInfo.TrophyID, TrophyInfo );
			}
		}
	}
}
