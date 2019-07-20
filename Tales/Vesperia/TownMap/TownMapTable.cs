using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EndianUtils = HyoutaUtils.EndianUtils;

namespace HyoutaTools.Tales.Vesperia.TownMap {
	public class TownMapTable {
		public int PointerOffset;

		public int LocationTileTableJPN;
		public int LocationTileTableENG;
		public int LocationOffsetTableJPN;
		public int LocationOffsetTableENG;
		public int LocationInfoTable;

		public int InfoAmount;

		public TownMapInfo[] TownMapInfos;
		public string Filepath;

		public TownMapTable( byte[] File ) {
			Initialize( File );
		}

		public TownMapTable( string Filepath ) {
			this.Filepath = Filepath;
			byte[] File = System.IO.File.ReadAllBytes( Filepath );
			Initialize( File );
		}

		private void Initialize( byte[] File ) {
			PointerOffset = EndianUtils.SwapEndian( BitConverter.ToInt32( File, 0x0C ) );

			LocationTileTableJPN = PointerOffset + EndianUtils.SwapEndian( BitConverter.ToInt32( File, 0x156DC ) );
			LocationTileTableENG = PointerOffset + EndianUtils.SwapEndian( BitConverter.ToInt32( File, 0x15710 ) );

			LocationOffsetTableJPN = PointerOffset + EndianUtils.SwapEndian( BitConverter.ToInt32( File, 0x156E4 ) );
			LocationOffsetTableENG = 0x04 + PointerOffset + EndianUtils.SwapEndian( BitConverter.ToInt32( File, 0x15718 ) ); // increment by 4 so we don't have to check if we're on JPN or ENG later, those two originally point to the exact same location

			LocationInfoTable = PointerOffset + EndianUtils.SwapEndian( BitConverter.ToInt32( File, 0x156BC ) );

			// 0x188 / 8 -> 0x31, not sure where in file it's set but whatever
			InfoAmount = 0x31;

			TownMapInfos = new TownMapInfo[InfoAmount];

			for ( int i = 0; i < InfoAmount; i++ ) {
				int OffsetLocation = LocationOffsetTableJPN + ( i * 0x08 );
				int TilesToSkip = EndianUtils.SwapEndian( BitConverter.ToInt32( File, OffsetLocation ) );
				int TilesLocationJPN = LocationTileTableJPN + ( TilesToSkip * 0x20 );
				OffsetLocation = LocationOffsetTableENG + ( i * 0x08 );
				TilesToSkip = EndianUtils.SwapEndian( BitConverter.ToInt32( File, OffsetLocation ) );
				int TilesLocationENG = LocationTileTableENG + ( TilesToSkip * 0x20 );

				int InfoLocation = LocationInfoTable + ( i * 0xAC );

				TownMapInfos[i] = new TownMapInfo( File, TilesLocationJPN, TilesLocationENG, InfoLocation );
			}
		}
	}
}
