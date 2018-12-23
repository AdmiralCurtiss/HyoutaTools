using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.ItemDat;

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

		public string GetDataAsHtml( GameVersion version, ItemDat.ItemDat items, Dictionary<uint, TSS.TSSEntry> inGameIdDict, bool phpLinks = false ) {
			StringBuilder sb = new StringBuilder();

			var item = items.itemIdDict[ItemID];
			sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
			sb.Append( "<a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Item, version, phpLinks, id: (int)item.Data[(int)ItemData.ID], icon: (int)item.Data[(int)ItemData.Icon] ) + "\">" );
			sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpnHtml( version ) + "</a>" );

			return sb.ToString();
		}
	}
}
