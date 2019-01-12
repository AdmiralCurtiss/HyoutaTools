using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTTA {
	public class StrategyOption {
		public uint[] Data;
		public uint NameStringDicID;
		public uint DescStringDicID;

		public uint Category;
		public uint InGameID;
		public uint Characters;
		public uint ID;

		public string RefString;
		public StrategyOption( System.IO.Stream stream, uint refStringStart, Util.Endianness endian, Util.Bitness bits ) {
			uint entrySize = stream.ReadUInt32().FromEndian( endian );
			Category = stream.ReadUInt32().FromEndian( endian );
			InGameID = stream.ReadUInt32().FromEndian( endian );
			ulong refStringLocation = stream.ReadUInt( bits, endian );
			NameStringDicID = stream.ReadUInt32().FromEndian( endian );
			DescStringDicID = stream.ReadUInt32().FromEndian( endian );
			Characters = stream.ReadUInt32().FromEndian( endian );
			ID = stream.ReadUInt32().FromEndian( endian );
			RefString = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + refStringLocation ) );
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();
			//sb.Append( RefString );
			sb.Append( "<tr id=\"strategyoption" + InGameID + "\">" );
			sb.Append( "<td>" );
			sb.Append( StrategySet.GetCategoryName( Category, version, websiteLanguage, inGameIdDict ) );
			sb.Append( "</td>" );

			int colspan = websiteLanguage.WantsBoth() ? 1 : 2;
			if ( websiteLanguage.WantsJp() ) {
				sb.Append( "<td colspan=\"" + colspan + "\">" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( inGameIdDict[NameStringDicID].StringJpnHtml( version ) );
				sb.Append( "</span>" );
				sb.Append( "<br>" );
				sb.Append( inGameIdDict[DescStringDicID].StringJpnHtml( version ) );
				sb.Append( "</td>" );
			}

			if ( websiteLanguage.WantsEn() ) {
				sb.Append( "<td colspan=\"" + colspan + "\">" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( inGameIdDict[NameStringDicID].StringEngHtml( version ) );
				sb.Append( "</span>" );
				sb.Append( "<br>" );
				sb.Append( inGameIdDict[DescStringDicID].StringEngHtml( version ) );
				sb.Append( "</td>" );
			}

			sb.Append( "<td>" );
			Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, version, Characters );
			sb.Append( "</td>" );

			sb.Append( "</tr>" );
			return sb.ToString();
		}
	}
}
