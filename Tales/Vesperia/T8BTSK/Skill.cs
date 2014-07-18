using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTSK {
	public class Skill {
		public uint ID;
		public uint InGameID;
		public uint NameStringDicID;
		public uint DescStringDicID;

		public uint Unknown7;
		public uint LearnableByBitmask;

		public uint EquipCost;
		public uint LearnCost;
		public uint Category;
		public uint SymbolValue;

		public float Unknown13;
		public float Unknown14;
		public float Unknown15;
		public uint Unknown16;

		public string RefString;

		public Skill( System.IO.Stream stream, uint refStringStart ) {
			uint entrySize = stream.ReadUInt32().SwapEndian();
			ID = stream.ReadUInt32().SwapEndian();
			InGameID = stream.ReadUInt32().SwapEndian();
			uint refStringLocation = stream.ReadUInt32().SwapEndian();

			NameStringDicID = stream.ReadUInt32().SwapEndian();
			DescStringDicID = stream.ReadUInt32().SwapEndian();
			Unknown7 = stream.ReadUInt32().SwapEndian();
			LearnableByBitmask = stream.ReadUInt32().SwapEndian();

			EquipCost = stream.ReadUInt32().SwapEndian();
			LearnCost = stream.ReadUInt32().SwapEndian();
			Category = stream.ReadUInt32().SwapEndian();
			// Game sums up this value per category, then figures out the OVL-symbol from the totals
			SymbolValue = stream.ReadUInt32().SwapEndian();

			Unknown13 = stream.ReadUInt32().SwapEndian().UIntToFloat();
			Unknown14 = stream.ReadUInt32().SwapEndian().UIntToFloat();
			Unknown15 = stream.ReadUInt32().SwapEndian().UIntToFloat();
			Unknown16 = stream.ReadUInt32().SwapEndian();

			long pos = stream.Position;
			stream.Position = refStringStart + refStringLocation;
			RefString = stream.ReadAsciiNullterm();
			stream.Position = pos;
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( GameVersion version, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();
			sb.Append( "<div id=\"skill" + ID + "\">" );
			sb.Append( RefString + "<br>" );
			sb.Append( inGameIdDict[NameStringDicID].StringEngOrJpn + "<br>" );
			sb.Append( inGameIdDict[DescStringDicID].StringEngOrJpn + "<br>" );
			sb.Append( "Equip Cost: " + EquipCost + "<br>" );
			sb.Append( "Required LP to learn: " + LearnCost + "<br>" );
			sb.Append( "Category: " + Category + "<br>" );
			sb.Append( "Symbol Weight: " + SymbolValue + "<br>" );

			uint equip = LearnableByBitmask;
			if ( equip > 0 ) {
				sb.Append( "<span class=\"equip\">" );
				if ( ( equip & 1 ) == 1 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_YUR.gif\" height=\"32\" width=\"24\">" ); }
				if ( ( equip & 2 ) == 2 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_EST.gif\" height=\"32\" width=\"24\">" ); }
				if ( ( equip & 4 ) == 4 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_KAR.gif\" height=\"32\" width=\"24\">" ); }
				if ( ( equip & 8 ) == 8 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_RIT.gif\" height=\"32\" width=\"24\">" ); }
				if ( ( equip & 16 ) == 16 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_RAV.gif\" height=\"32\" width=\"24\">" ); }
				if ( ( equip & 32 ) == 32 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_JUD.gif\" height=\"32\" width=\"24\">" ); }
				if ( ( equip & 64 ) == 64 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_RAP.gif\" height=\"32\" width=\"24\">" ); }
				if ( ( equip & 128 ) == 128 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_FRE.gif\" height=\"32\" width=\"24\">" ); }
				if ( version == GameVersion.PS3 && ( equip & 256 ) == 256 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_PAT.gif\" height=\"32\" width=\"24\">" ); }
				sb.Append( "</span>" );
			}

			sb.Append( "Unknowns:<br>" );
			sb.Append( Unknown7 + "<br>" );
			sb.Append( Unknown13 + "<br>" );
			sb.Append( Unknown14 + "<br>" );
			sb.Append( Unknown15 + "<br>" );
			sb.Append( Unknown16 + "<br>" );
			sb.Append( "</div>" );
			return sb.ToString();
		}
	}
}
