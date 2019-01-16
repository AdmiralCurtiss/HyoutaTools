using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTSK {
	public class Skill {
		public uint ID;
		public uint InGameID;
		public uint Unknown4;
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

		public Skill( System.IO.Stream stream, uint refStringStart, Util.Endianness endian, Util.Bitness bits ) {
			uint entrySize = stream.ReadUInt32().FromEndian( endian );
			if ( entrySize != ( 0x3C + bits.NumberOfBytes() ) ) {
				throw new Exception( "Unknown Skill size." );
			}

			ID = stream.ReadUInt32().FromEndian( endian );
			InGameID = stream.ReadUInt32().FromEndian( endian );
			ulong refStringLocation = stream.ReadUInt( bits, endian );

			NameStringDicID = stream.ReadUInt32().FromEndian( endian );
			DescStringDicID = stream.ReadUInt32().FromEndian( endian );
			Unknown7 = stream.ReadUInt32().FromEndian( endian );
			LearnableByBitmask = stream.ReadUInt32().FromEndian( endian );

			EquipCost = stream.ReadUInt32().FromEndian( endian );
			LearnCost = stream.ReadUInt32().FromEndian( endian );
			Category = stream.ReadUInt32().FromEndian( endian );
			// Game sums up this value per category, then figures out the OVL-symbol from the totals
			SymbolValue = stream.ReadUInt32().FromEndian( endian );

			Unknown13 = stream.ReadUInt32().FromEndian( endian ).UIntToFloat();
			Unknown14 = stream.ReadUInt32().FromEndian( endian ).UIntToFloat();
			Unknown15 = stream.ReadUInt32().FromEndian( endian ).UIntToFloat();
			Inactive = stream.ReadUInt32().FromEndian( endian );

			RefString = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + refStringLocation ) );
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();
			sb.Append( "<tr id=\"skill" + InGameID + "\">" );
			//sb.Append( RefString + "<br>" );

			sb.Append( "<td>" );
			sb.Append( "<img src=\"skill-icons/category-" + Category + ".png\" width=\"32\" height=\"32\">" );
			sb.Append( "</td>" );

			int colspan = websiteLanguage.WantsBoth() ? 1 : 2;
			if ( websiteLanguage.WantsJp() ) {
				sb.Append( "<td colspan=\"" + colspan + "\" class=\"skilljpn\">" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( inGameIdDict[NameStringDicID].StringJpnHtml( version, inGameIdDict ) );
				sb.Append( "</span>" );
				sb.Append( "<br>" );
				sb.Append( inGameIdDict[DescStringDicID].StringJpnHtml( version, inGameIdDict ) );
				sb.Append( "</td>" );
			}

			if ( websiteLanguage.WantsEn() ) {
				sb.Append( "<td colspan=\"" + colspan + "\">" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( inGameIdDict[NameStringDicID].StringEngHtml( version, inGameIdDict ) );
				sb.Append( "</span>" );
				sb.Append( "<br>" );
				sb.Append( inGameIdDict[DescStringDicID].StringEngHtml( version, inGameIdDict ) );
				sb.Append( "</td>" );
			}

			sb.Append( "<td class=\"skilldata\">" );
			if ( LearnableByBitmask > 0 ) {
				sb.Append( "<span class=\"equip\">" );
				Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, inGameIdDict, version, LearnableByBitmask, websiteLanguage.MainJp() );
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
