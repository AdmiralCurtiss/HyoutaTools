using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTGR {
	public class GradeShopEntry {
		public uint ID;
		public uint InGameID;

		public uint NameStringDicID;
		public uint DescStringDicID;
		public uint GradeCost;

		public string RefString;
		public GradeShopEntry( System.IO.Stream stream, uint refStringStart, Util.Endianness endian, Util.Bitness bits ) {
			uint entrySize = stream.ReadUInt32().FromEndian( endian );
			if ( entrySize != ( 0x18 + bits.NumberOfBytes() ) ) {
				throw new Exception( "Unexpected GradeShopEntry size." );
			}

			ID = stream.ReadUInt32().FromEndian( endian );
			InGameID = stream.ReadUInt32().FromEndian( endian );
			ulong refStringLocation = stream.ReadUInt( bits ).FromEndian( endian );

			NameStringDicID = stream.ReadUInt32().FromEndian( endian );
			DescStringDicID = stream.ReadUInt32().FromEndian( endian );
			GradeCost = stream.ReadUInt32().FromEndian( endian );

			RefString = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + refStringLocation ) );
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();
			sb.Append( "<tr>" );
			/*
			sb.Append( "<td>" );
			sb.Append( RefString );
			sb.Append( "</td>" );
			//*/

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
			sb.Append( GradeCost + " Grade" );
			sb.Append( "</td>" );

			return sb.ToString();
		}
	}
}
