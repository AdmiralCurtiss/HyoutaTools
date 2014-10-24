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
		public uint Inactive;

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
			Inactive = stream.ReadUInt32().SwapEndian();

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
			sb.Append( "<tr id=\"skill" + ID + "\">" );
			//sb.Append( RefString + "<br>" );

			sb.Append( "<td>" );
			sb.Append( "<img src=\"skill-icons/category-" + Category + ".png\" width=\"32\" height=\"32\">" );
			sb.Append( "</td>" );

			sb.Append( "<td class=\"skilljpn\">" );
			sb.Append( "<span class=\"itemname\">" );
			sb.Append( inGameIdDict[NameStringDicID].StringJpnHtml( version ) );
			sb.Append( "</span>" );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[DescStringDicID].StringJpnHtml( version ) );
			sb.Append( "</td>" );
			
			sb.Append( "<td>" );
			sb.Append( "<span class=\"itemname\">" );
			sb.Append( inGameIdDict[NameStringDicID].StringEngHtml( version ) );
			sb.Append( "</span>" );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[DescStringDicID].StringEngHtml( version ) );
			sb.Append( "</td>" );

			sb.Append( "<td class=\"skilldata\">" );
			if ( LearnableByBitmask > 0 ) {
				sb.Append( "<span class=\"equip\">" );
				Website.GenerateWebsite.AppendCharacterBitfieldAsImageString( sb, version, LearnableByBitmask );
				sb.Append( "</span>" );
			}
			sb.Append( EquipCost + "&nbsp;SP<br>" );
			sb.Append( LearnCost + "&nbsp;LP<br>" );
			sb.Append( "Symbol Weight: " + SymbolValue + "<br>" );

			//sb.Append( "~7: " + Unknown7 + "<br>" );
			//sb.Append( "~13: " + Unknown13 + "<br>" );
			//sb.Append( "~14: " + Unknown14 + "<br>" );
			//sb.Append( "~15: " + Unknown15 + "<br>" );
			if ( Inactive == 0 ) { sb.Append( "Unusable<br>" ); }
			sb.Append( "</td>" );
			sb.Append( "</tr>" );
			return sb.ToString();
		}
	}
}
