using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.ShopData {
	public class ShopItem {
		uint[] Data;

		public uint ShopID;
		public uint ItemID;

		public ShopItem( System.IO.Stream stream ) {
			Data = new uint[56 / 4];
			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
			}

			ShopID = Data[1];
			ItemID = Data[5];
		}

		public string GetDataAsHtml( GameVersion Version, ItemDat.ItemDat Items, Dictionary<uint, TSS.TSSEntry> InGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			sb.AppendLine( "<tr><td>" );
			sb.AppendLine( ShopID.ToString() );
			var item = Items.itemIdDict[ItemID];
			sb.AppendLine( InGameIdDict[item.NamePointer].StringEngOrJpn );
			sb.AppendLine( "</td></tr>" );

			return sb.ToString();
		}
	}
}
