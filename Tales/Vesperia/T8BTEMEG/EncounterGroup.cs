using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTEMEG {
	public class EncounterGroup {
		public uint ID;
		public uint StringDicID;
		public uint InGameID;
		public string RefString;
		public uint[] EnemyGroupIDs;

		public EncounterGroup( System.IO.Stream stream, uint refStringStart ) {
			uint[] Data;
			uint entryLength = stream.PeekUInt32().SwapEndian();
			Data = new uint[entryLength / 4];
			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
			}

			ID = Data[1];
			StringDicID = Data[2];
			InGameID = Data[3];

			EnemyGroupIDs = new uint[10];
			for ( int i = 0; i < 10; ++i ) {
				EnemyGroupIDs[i] = Data[5 + i];
			}

			long pos = stream.Position;
			stream.Position = refStringStart + Data[4];
			RefString = stream.ReadAsciiNullterm();
			stream.Position = pos;
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( T8BTEMGP.T8BTEMGP enemyGroups, T8BTEMST.T8BTEMST enemies, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			sb.Append( "<tr><td>" );
			sb.Append( RefString );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[StringDicID].StringEngOrJpnHtml( GameVersion.PS3 ) );
			for ( int i = 0; i < EnemyGroupIDs.Length; ++i ) {
				if ( EnemyGroupIDs[i] == 0xFFFFFFFFu ) { continue; }

				var group = enemyGroups.EnemyGroupIdDict[EnemyGroupIDs[i]];
				foreach ( int enemyId in group.EnemyIDs ) {
					if ( enemyId < 0 ) { continue; }
					var enemy = enemies.EnemyIdDict[(uint)enemyId];
					sb.Append( inGameIdDict[enemy.NameStringDicID].StringEngOrJpnHtml( GameVersion.PS3 ) );
					sb.Append( "<br>" );
				}
			}
			sb.Append( "</td></tr>" );

			return sb.ToString();
		}
	}
}
