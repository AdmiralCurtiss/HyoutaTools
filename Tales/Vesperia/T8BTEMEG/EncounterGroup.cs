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

		public EncounterGroup( System.IO.Stream stream, uint refStringStart, Util.Endianness endian, Util.Bitness bits ) {
			uint entryLength = stream.ReadUInt32().FromEndian( endian );
			ID = stream.ReadUInt32().FromEndian( endian );
			StringDicID = stream.ReadUInt32().FromEndian( endian );
			InGameID = stream.ReadUInt32().FromEndian( endian );
			ulong refLoc = stream.ReadUInt( bits, endian );

			EnemyGroupIDs = new uint[10];
			for ( int i = 0; i < 10; ++i ) {
				EnemyGroupIDs[i] = stream.ReadUInt32().FromEndian( endian );
			}

			RefString = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + refLoc ) );
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( T8BTEMGP.T8BTEMGP enemyGroups, T8BTEMST.T8BTEMST enemies, Dictionary<uint, TSS.TSSEntry> inGameIdDict, GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage ) {
			StringBuilder sb = new StringBuilder();

			sb.Append( "<tr><td>" );
			sb.Append( RefString );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[StringDicID].StringEngOrJpnHtml( version, inGameIdDict, websiteLanguage ) );
			for ( int i = 0; i < EnemyGroupIDs.Length; ++i ) {
				if ( EnemyGroupIDs[i] == 0xFFFFFFFFu ) { continue; }

				var group = enemyGroups.EnemyGroupIdDict[EnemyGroupIDs[i]];
				foreach ( int enemyId in group.EnemyIDs ) {
					if ( enemyId < 0 ) { continue; }
					var enemy = enemies.EnemyIdDict[(uint)enemyId];
					sb.Append( inGameIdDict[enemy.NameStringDicID].StringEngOrJpnHtml( version, inGameIdDict, websiteLanguage ) );
					sb.Append( "<br>" );
				}
			}
			sb.Append( "</td></tr>" );

			return sb.ToString();
		}
	}
}
