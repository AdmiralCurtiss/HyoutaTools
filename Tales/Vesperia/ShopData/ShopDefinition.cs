using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.ShopData {
	public class ShopDefinition {
		uint[] Data;

		public uint InGameID;
		public uint StringDicID;
		public uint OnTrigger;
		public uint ChangeToShop;

		public ShopItem[] ShopItems;

		public ShopDefinition( System.IO.Stream stream ) {
			Data = new uint[8];
			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
			}

			InGameID = Data[0];
			StringDicID = Data[1];
			OnTrigger = Data[4];
			ChangeToShop = Data[5];
		}

		public string GetDataAsHtml( GameVersion version, ItemDat.ItemDat items, ShopData shops, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			sb.AppendLine( "<tr id=\"shop" + InGameID + "\">" );
			sb.AppendLine( "<td class=\"synopsistitle\" colspan=\"3\">" );
			sb.AppendLine( VesperiaUtil.RemoveTags( inGameIdDict[StringDicID].StringJPN, true, true ) );
			sb.AppendLine( "</td>" );
			sb.AppendLine( "<td class=\"synopsistitle\" colspan=\"3\">" );
			sb.AppendLine( inGameIdDict[StringDicID].StringENG );
			sb.AppendLine( "</td>" );
			sb.AppendLine( "</tr>" );
			sb.AppendLine( "<tr>" );
			for ( int i = 2; i < 9; ++i ) {
				if ( i == 4 ) { continue; }
				sb.AppendLine( "<td>" );
				foreach ( var item in ShopItems ) {
					if ( items.itemIdDict[item.ItemID].Data[(int)ItemDat.ItemData.Category] == i ) {
						sb.Append( item.GetDataAsHtml( version, items, inGameIdDict ) );
						sb.Append( "<br>" );
					}
				}
				sb.AppendLine( "</td>" );
			}
			sb.AppendLine( "</tr>" );
			if ( OnTrigger > 0 ) {
				sb.Append( "<tr>" );
				sb.Append( "<td class=\"strategychar\" colspan=\"6\">" );
				sb.Append( "Changes to <a href=\"#shop" + ChangeToShop + "\">" );
				sb.Append( inGameIdDict[shops.ShopDictionary[ChangeToShop].StringDicID].StringEngOrJpn );
				sb.Append( "</a>" );
				sb.Append( "</td>" );
				sb.Append( "</tr>" );
			}

			return sb.ToString();
		}
	}
}
