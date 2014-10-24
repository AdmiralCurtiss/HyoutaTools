using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.WRLDDAT {
	public class Location {
		public uint LocationID;
		public uint[] NameStringDicIDs;
		public uint[] DescStringDicIDs;
		public uint DefaultStringDicID;
		public string[] RefStrings;
		public uint Category; // 1 = Town, 2 = Dungeon, 3 = Field
		public uint[] ChangeEventTriggers;
		public uint[] ShopsOrEnemyGroups;
		public uint Unused2;

		public Location( System.IO.Stream stream ) {
			uint[] Data = new uint[0x74 / 4]; // + 0x20*4 strings, + 4*4 StringDicIDs

			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
			}

			LocationID = Data[0];
			DefaultStringDicID = Data[1];
			Unused2 = Data[2];
			Category = Data[3];
			DescStringDicIDs = new uint[4];
			for ( int i = 0; i < 4; ++i ) {
				DescStringDicIDs[i] = Data[5 + i];
			}

			ShopsOrEnemyGroups = new uint[16];
			for ( int i = 0; i < 16; ++i ) {
				ShopsOrEnemyGroups[i] = Data[9 + i];
			}

			// Data[26 ~ 28] appear to be references to event triggers for when an area advances to its next 'state'
			// Data[25] is always 0, which makes sense as the initial state of a location should not have a required trigger
			ChangeEventTriggers = new uint[4];
			for ( int i = 0; i < 4; ++i ) {
				ChangeEventTriggers[i] = Data[25 + i];
			}

			long pos = stream.Position;
			RefStrings = new string[4];
			for ( int i = 0; i < 4; ++i ) {
				RefStrings[i] = stream.ReadAsciiNullterm();
				stream.Position = pos + 0x20 * ( i + 1 );
			}

			stream.Position = pos + 0x20 * 4;
			NameStringDicIDs = new uint[4];
			for ( int i = 0; i < 4; ++i ) {
				NameStringDicIDs[i] = stream.ReadUInt32().SwapEndian();
			}
		}

		public override string ToString() {
			return RefStrings[0];
		}

		public TSS.TSSEntry GetLastValidName( Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			for ( int i = 3; i >= 0; --i ) {
				if ( inGameIdDict[DescStringDicIDs[i]].StringEngOrJpn != "" ) {
					return inGameIdDict[NameStringDicIDs[i]];
				}
			}
			return inGameIdDict[DefaultStringDicID];
		}

		public string GetDataAsHtml( GameVersion version, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict, T8BTEMEG.T8BTEMEG encounterGroups, T8BTEMGP.T8BTEMGP enemyGroups, T8BTEMST.T8BTEMST enemies, ShopData.ShopData shops ) {
			StringBuilder sb = new StringBuilder();

			StringBuilder shopEnemySb = new StringBuilder();
			List<uint> alreadyPrinted = new List<uint>();
			for ( int i = 0; i < ShopsOrEnemyGroups.Length; ++i ) {
				if ( ShopsOrEnemyGroups[i] == 0 ) { continue; }
				if ( Category == 1 ) {
					// references to shops
					var shop = shops.ShopDictionary[ShopsOrEnemyGroups[i]];
					shopEnemySb.Append( "<a href=\"shops-" + version + ".html#shop" + shop.InGameID + "\">" );
					shopEnemySb.Append( inGameIdDict[shop.StringDicID].StringEngOrJpnHtml( version ) );
					shopEnemySb.Append( "</a>" );
				} else {
					// references to encounter groups
					foreach ( uint groupId in encounterGroups.EncounterGroupIdDict[ShopsOrEnemyGroups[i]].EnemyGroupIDs ) {
						if ( groupId == 0xFFFFFFFFu ) { continue; }
						foreach ( uint id in enemyGroups.EnemyGroupIdDict[groupId].EnemyIDs ) {
							if ( id == 0xFFFFFFFFu ) { continue; }
							if ( alreadyPrinted.Contains( id ) ) { continue; }

							var enemy = enemies.EnemyIdDict[id];
							shopEnemySb.Append( "<img src=\"monster-icons/44px/monster-" + enemy.IconID.ToString( "D3" ) + ".png\" height=\"22\" width=\"22\"> " );
							shopEnemySb.Append( "<a href=\"enemies-c" + enemy.Category + "-" + version + ".html#enemy" + enemy.InGameID + "\">" );
							shopEnemySb.Append( inGameIdDict[enemy.NameStringDicID].StringEngOrJpnHtml( version ) + "</a>" );
							shopEnemySb.Append( "<br>" );

							alreadyPrinted.Add( id );
						}
					}
				}
			}

			int variantCount = ChangeEventTriggers.Count( x => x != 0 ) + 1;
			for ( int i = 0; i < 4; ++i ) {
				if ( i >= 1 && ChangeEventTriggers[i] == 0 ) { continue; }

				if ( i == 0 ) {
					sb.Append( "<tr id=\"location" + LocationID + "\">" );
				} else {
					sb.Append( "<tr>" );
				}
				sb.Append( "<td>" );
				if ( RefStrings[i] != "" ) {
					sb.Append( "<img src=\"worldmap/U_" + RefStrings[i] + ".png\">" );
				}
				sb.Append( "</td>" );
				sb.Append( "<td>" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( inGameIdDict[NameStringDicIDs[i]].StringJpnHtml( version ) + "<br>" );
				sb.Append( "</span>" );
				sb.Append( inGameIdDict[DescStringDicIDs[i]].StringJpnHtml( version ) + "<br>" );
				sb.Append( "<br>" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( inGameIdDict[NameStringDicIDs[i]].StringEngHtml( version ) + "<br>" );
				sb.Append( "</span>" );
				sb.Append( inGameIdDict[DescStringDicIDs[i]].StringEngHtml( version ) + "<br>" );
				if ( RefStrings[i] == "" ) {
					sb.Append( "<br>" );
				}
				sb.Append( "</td>" );
				if ( i == 0 ) {
					sb.Append( "<td rowspan=\"" + variantCount + "\">" );
					sb.Append( shopEnemySb );
					sb.Append( "</td>" );
				}
				sb.Append( "</tr>" );
			}

			return sb.ToString();
		}
	}
}
