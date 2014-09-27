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
		public uint Unused25;

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

			Unused25 = Data[25];

			// Data[26 ~ 28] appear to be references to event triggers for when an area advances to its next 'state'
			ChangeEventTriggers = new uint[3];
			for ( int i = 0; i < 3; ++i ) {
				ChangeEventTriggers[i] = Data[26 + i];
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

		public string GetDataAsHtml( GameVersion version, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict, T8BTEMEG.T8BTEMEG encounterGroups, T8BTEMGP.T8BTEMGP enemyGroups, T8BTEMST.T8BTEMST enemies ) {
			StringBuilder sb = new StringBuilder();

			string defJpn = VesperiaUtil.RemoveTags( inGameIdDict[DefaultStringDicID].StringJPN, true, true );
			string defEng = inGameIdDict[DefaultStringDicID].StringENG;

			sb.Append( "<div id=\"location" + LocationID + "\">" );
			//sb.Append( def + "<br>" );
			for ( int i = 0; i < 4; ++i ) {
				if ( i >= 1 && ChangeEventTriggers[i - 1] == 0 ) { continue; }

				sb.Append( "<table>" );
				sb.Append( "<tr>" );
				if ( RefStrings[i] != "" ) {
					sb.Append( "<td>" );
					sb.Append( "<img src=\"worldmap/U_" + RefStrings[i] + ".png\"><br>" );
					sb.Append( "</td>" );
				}
				sb.Append( "<td>" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( VesperiaUtil.RemoveTags( inGameIdDict[NameStringDicIDs[i]].StringJPN, true, true ) + "<br>" );
				sb.Append( "</span>" );
				sb.Append( VesperiaUtil.RemoveTags( inGameIdDict[DescStringDicIDs[i]].StringJPN, true, true ).Replace( "\n", "<br>" ) + "<br>" );
				sb.Append( "<br>" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( inGameIdDict[NameStringDicIDs[i]].StringENG + "<br>" );
				sb.Append( "</span>" );
				sb.Append( inGameIdDict[DescStringDicIDs[i]].StringENG.Replace( "\n", "<br>" ) + "<br>" );
				sb.Append( "</td>" );
				sb.Append( "</tr>" );
				sb.Append( "</table>" );
				if ( RefStrings[i] == "" ) {
					sb.Append( "<br>" );
				}
			}

			List<uint> alreadyPrinted = new List<uint>();
			for ( int i = 0; i < ShopsOrEnemyGroups.Length; ++i ) {
				if ( Category == 1 ) {
					// references to shops
					// no idea where the game stores shop data
				} else {
					// references to encounter groups
					if ( ShopsOrEnemyGroups[i] == 0 ) { continue; }

					foreach ( uint groupId in encounterGroups.EncounterGroupIdDict[ShopsOrEnemyGroups[i]].EnemyGroupIDs ) {
						if ( groupId == 0xFFFFFFFFu ) { continue; }
						foreach ( uint id in enemyGroups.EnemyGroupIdDict[groupId].EnemyIDs ) {
							if ( id == 0xFFFFFFFFu ) { continue; }
							if ( alreadyPrinted.Contains( id ) ) { continue; }
							sb.AppendLine( inGameIdDict[enemies.EnemyIdDict[id].NameStringDicID].StringEngOrJpn );
							alreadyPrinted.Add( id );
						}
					}
				}
			}

			sb.Append( "</div>" );
			return sb.ToString();
		}
	}
}
