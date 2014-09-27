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

		public string GetDataAsHtml( GameVersion Version, ItemDat.ItemDat Items, Dictionary<uint, TSS.TSSEntry> InGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			sb.AppendLine( "<tr><td>" );
			sb.AppendLine( InGameID.ToString() );
			sb.AppendLine( InGameIdDict[StringDicID].StringEngOrJpn );
			if ( OnTrigger > 0 ) {
				sb.AppendLine( " -&gt; " + ChangeToShop );
			}
			sb.AppendLine( "</td></tr>" );

			return sb.ToString();
		}
	}
}
