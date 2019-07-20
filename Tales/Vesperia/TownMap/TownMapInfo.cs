using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;
using EndianUtils = HyoutaUtils.EndianUtils;

namespace HyoutaTools.Tales.Vesperia.TownMap {
	public class TownMapInfo {
		public String Filename;
		public int TileAmountJPN;
		public int TileAmountENG;

		public TownMapTile[] TownMapTilesJPN;
		public TownMapTile[] TownMapTilesENG;

		public int InfoLocation;

		public TownMapInfo( byte[] File, int TilesLocationJPN, int TilesLocationENG, int InfoLocation ) {
			Filename = TextUtils.GetTextAscii( File, InfoLocation );

			this.InfoLocation = InfoLocation;

			TileAmountJPN = EndianUtils.SwapEndian( BitConverter.ToInt32( File, InfoLocation + 0xA4 ) );
			TileAmountENG = EndianUtils.SwapEndian( BitConverter.ToInt32( File, InfoLocation + 0xA8 ) );

			TownMapTilesJPN = new TownMapTile[TileAmountJPN];
			TownMapTilesENG = new TownMapTile[TileAmountENG];

			for ( int i = 0; i < TileAmountJPN; i++ ) {
				TownMapTilesJPN[i] = new TownMapTile( File, TilesLocationJPN + ( i * 0x20 ) );
			}
			for ( int i = 0; i < TileAmountENG; i++ ) {
				TownMapTilesENG[i] = new TownMapTile( File, TilesLocationENG + ( i * 0x20 ) );
			}
		}

		public override string ToString() {
			return Filename;
		}
	}
}
