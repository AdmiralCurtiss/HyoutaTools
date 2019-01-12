using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.ShopData {
	public class ShopDefinition {
		public uint InGameID;
		public uint StringDicID;
		public uint OnTrigger;
		public uint ChangeToShop;

		public ShopItem[] ShopItems;

		public ShopDefinition( System.IO.Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			InGameID = stream.ReadUInt32().FromEndian( endian );
			StringDicID = stream.ReadUInt32().FromEndian( endian );
			stream.ReadUInt32().FromEndian( endian ); // second instance of string dic id?
			stream.ReadUInt32().FromEndian( endian ); // always empty?
			OnTrigger = stream.ReadUInt32().FromEndian( endian );
			ChangeToShop = stream.ReadUInt32().FromEndian( endian );
			stream.DiscardBytes( 4 + bits.NumberOfBytes() ); // always empty?
		}

		public string GetDataAsHtml( GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, ItemDat.ItemDat items, ShopData shops, Dictionary<uint, TSS.TSSEntry> inGameIdDict, bool phpLinks = false ) {
			StringBuilder sb = new StringBuilder();

			int colspan = websiteLanguage.WantsBoth() ? 3 : 6;
			sb.Append( "<tr id=\"shop" + InGameID + "\">" );
			if ( websiteLanguage.WantsJp() ) {
				sb.Append( "<td class=\"synopsistitle\" colspan=\"" + colspan + "\">" );
				sb.Append( inGameIdDict[StringDicID].StringJpnHtml( version ) );
				sb.Append( "</td>" );
			}
			if ( websiteLanguage.WantsEn() ) {
				sb.Append( "<td class=\"synopsistitle\" colspan=\"" + colspan + "\">" );
				sb.Append( inGameIdDict[StringDicID].StringEngHtml( version ) );
				sb.Append( "</td>" );
			}
			sb.Append( "</tr>" );
			sb.Append( "<tr>" );
			for ( int i = 2; i < 9; ++i ) {
				if ( i == 4 ) { continue; }
				sb.Append( "<td>" );
				foreach ( var item in ShopItems ) {
					if ( items.itemIdDict[item.ItemID].Data[(int)ItemDat.ItemData.Category] == i ) {
						sb.Append( item.GetDataAsHtml( version, versionPostfix, locale, websiteLanguage, items, inGameIdDict, phpLinks: phpLinks ) );
						sb.Append( "<br>" );
					}
				}
				sb.Append( "</td>" );
			}
			sb.Append( "</tr>" );
			if ( OnTrigger > 0 ) {
				sb.Append( "<tr>" );
				sb.Append( "<td class=\"strategychar\" colspan=\"6\">" );
				sb.Append( "Changes to <a href=\"#shop" + ChangeToShop + "\">" );
				sb.Append( inGameIdDict[shops.ShopDictionary[ChangeToShop].StringDicID].StringEngOrJpnHtml( version, websiteLanguage ) );
				sb.Append( "</a>" );
				sb.Append( "</td>" );
				sb.Append( "</tr>" );
			}

			return sb.ToString();
		}
	}
}
