using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTXTM {
	public class MapTile {
		public uint RoomType;
		public int FloorExitDiff;
		public uint EnemyGroup;
		public uint FramesToMove;

		public uint RegularTreasure;
		public uint SpecialTreasure;
		public uint MoveUpAllowed;
		public uint MoveDownAllowed;

		public uint MoveLeftAllowed;
		public uint MoveRightAllowed;

		public MapTile( System.IO.Stream stream ) {
			RoomType = stream.ReadUInt32().SwapEndian();
			FloorExitDiff = (int)stream.ReadUInt32().SwapEndian();
			EnemyGroup = stream.ReadUInt32().SwapEndian();
			FramesToMove = stream.ReadUInt32().SwapEndian();

			RegularTreasure = stream.ReadUInt32().SwapEndian();
			SpecialTreasure = stream.ReadUInt32().SwapEndian();
			MoveUpAllowed = stream.ReadUInt32().SwapEndian();
			MoveDownAllowed = stream.ReadUInt32().SwapEndian();

			MoveLeftAllowed = stream.ReadUInt32().SwapEndian();
			MoveRightAllowed = stream.ReadUInt32().SwapEndian();
		}

		public string GetDataAsHtml( string stratum, int floor, GameVersion version, T8BTXTMT treasures, ItemDat.ItemDat items, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			sb.Append( "<td class=\"necropolistile" + RoomType + "\">" );
			if ( RoomType != 0 ) {
				sb.Append( "<div class=\"necropolis-arrow-up\">" );
				if ( MoveUpAllowed > 0 ) { sb.Append( "<img src=\"etc/up.png\" width=\"16\" height=\"16\">" ); }
				sb.Append( "</div>" );
				sb.Append( "<div class=\"necropolis-arrow-side\">" );
				if ( MoveLeftAllowed > 0 ) { sb.Append( "<img src=\"etc/left.png\" width=\"16\" height=\"16\">" ); }
				sb.Append( "</div>" );


				sb.Append( "<div class=\"necropolis-data\">" );

				sb.Append( "<img src=\"item-icons/ICON60.png\" width=\"16\" height=\"16\"> " + ( FramesToMove / 60 ) + " sec<br>" );
				//sb.Append( "Type: " + RoomType + "<br>" );
				int targetFloor;
				string targetID;
				switch ( RoomType ) {
					case 1: sb.Append( "Entrance<br>" ); break;
					case 2:
					case 5:
						if ( RoomType == 5 ) {
							targetFloor = ( floor + FloorExitDiff );
						} else {
							targetFloor = ( floor + 1 );
						}
						if ( targetFloor == 11 ) {
							targetID = (char)( ( (int)stratum[0] ) + 1 ) + "1";
						} else {
							targetID = stratum + targetFloor;
						}
						sb.Append( "Exit to <a href=\"#" + targetID + "\">" + stratum + "-" + targetFloor + "</a><br>" );
						break;
					case 3:
						//sb.Append( "Regular Room<br>" );
						break;
					case 4:
						//sb.Append( "Treasure Room<br>" );
						break;
				}
				//sb.Append( "EnemyGrp #" + EnemyGroup + " <br>" );

				if ( RegularTreasure > 0 ) {
					// not a generic solution, but the unmodified game has all four slots identical for regular treasures
					var treasureInfo = treasures.TreasureInfoList[(int)RegularTreasure];
					sb.Append( "<table>" );
					sb.Append( "<tr>" );
					for ( int i = 0; i < 3; ++i ) {
						var item = items.itemIdDict[treasureInfo.Items[i]];
						sb.Append( "<td>" );
						sb.Append( "<a href=\"items-i" + item.Data[(int)ItemDat.ItemData.Icon] + "-" + version + ".html#item" + item.Data[(int)ItemDat.ItemData.ID] + "\">" );
						sb.Append( "<img src=\"items/U_" + item.ItemString.TrimNull() + ".png\" height=\"32\" width=\"32\" title=\"" + inGameIdDict[item.NamePointer].StringEngOrJpn + "\">" );
						sb.Append( "</a>" );
						sb.Append( "</td>" );
					}
					sb.Append( "</tr>" );
					sb.Append( "<tr>" );
					for ( int i = 0; i < 3; ++i ) {
						sb.Append( "<td>" );
						sb.Append( treasureInfo.Chances[i] + "%" );
						sb.Append( "</td>" );
					}
					sb.Append( "</tr>" );
					sb.Append( "</table>" );
				}

				if ( SpecialTreasure > 0 ) {
					// unmodified game always has special treasures as one in the first slot with 100% chance
					var treasureInfo = treasures.TreasureInfoList[(int)SpecialTreasure];
					var item = items.itemIdDict[treasureInfo.Items[0]];
					sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemDat.ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
					sb.Append( "<a href=\"items-i" + item.Data[(int)ItemDat.ItemData.Icon] + "-" + version + ".html#item" + item.Data[(int)ItemDat.ItemData.ID] + "\">" );
					sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpn + "</a><br>" );
				}

				sb.Append( "</div>" );


				sb.Append( "<div class=\"necropolis-arrow-side\">" );
				if ( MoveRightAllowed > 0 ) { sb.Append( "<img src=\"etc/right.png\" width=\"16\" height=\"16\">" ); }
				sb.Append( "</div>" );
				sb.Append( "<div class=\"necropolis-arrow-down\">" );
				if ( MoveDownAllowed > 0 ) { sb.Append( "<img src=\"etc/down.png\" width=\"16\" height=\"16\">" ); }
				sb.Append( "</div>" );
			}
			sb.Append( "</td>" );

			return sb.ToString();
		}
	}
}
