using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EndianUtils = HyoutaUtils.EndianUtils;

namespace HyoutaTools.Trophy.Viewer {
	public class TropUsrSingleTrophy {
		/* 0x08 */
		public uint TrophyID;
		/* 0x10 */
		public uint TrophyIDDuplicate;
		/* 0x14 */
		public uint Unlocked;
		/* 0x20 */
		public UInt64 Timestamp1; // in microseconds
		/* 0x28 */
		public UInt64 Timestamp2; // in microseconds
		public TropUsrSingleTrophy( byte[] File, uint Offset ) {
			TrophyID = EndianUtils.SwapEndian( BitConverter.ToUInt32( File, (int)( Offset + 0x08 ) ) );
			TrophyIDDuplicate = EndianUtils.SwapEndian( BitConverter.ToUInt32( File, (int)( Offset + 0x10 ) ) );
			Unlocked = EndianUtils.SwapEndian( BitConverter.ToUInt32( File, (int)( Offset + 0x14 ) ) );
			Timestamp1 = EndianUtils.SwapEndian( BitConverter.ToUInt64( File, (int)( Offset + 0x20 ) ) );
			Timestamp2 = EndianUtils.SwapEndian( BitConverter.ToUInt64( File, (int)( Offset + 0x28 ) ) );
		}

		public static int SortByTimestamp( TropUsrSingleTrophy t1, TropUsrSingleTrophy t2 ) {
			if ( t1 == null )
				if ( t2 == null ) return 0;
				else return -1;
			else
				if ( t2 == null ) return 1;
			//return (int)((t2.Timestamp1 - t1.Timestamp1)/1000000);
			if ( t1.Timestamp1 > t2.Timestamp1 )
				return 1;
			else if ( t1.Timestamp1 < t2.Timestamp1 )
				return -1;
			return 0;
		}

		public static int SortByTrophyID( TropUsrSingleTrophy t1, TropUsrSingleTrophy t2 ) {
			if ( t1 == null )
				if ( t2 == null ) return 0;
				else return -1;
			else
				if ( t2 == null ) return 1;
			//return (int)((t2.Timestamp1 - t1.Timestamp1)/1000000);
			if ( t1.TrophyID > t2.TrophyID )
				return 1;
			else if ( t1.TrophyID < t2.TrophyID )
				return -1;
			return 0;
		}

		public override string ToString() {
			return Util.PS3TimeToDateTime( Timestamp1 ).ToString();
		}
	}
}
