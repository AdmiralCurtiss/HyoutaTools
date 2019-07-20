using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.BTLBDAT {
	public class BattleBookEntry {
		public uint UnlockReferenceMaybe;
		public uint NameStringDicId;
		public uint TextStringDicId;

		public BattleBookEntry( System.IO.Stream stream, EndianUtils.Endianness endian ) {
			UnlockReferenceMaybe = stream.ReadUInt32().FromEndian( endian );
			NameStringDicId = stream.ReadUInt32().FromEndian( endian );
			TextStringDicId = stream.ReadUInt32().FromEndian( endian );
			stream.DiscardBytes( 0x4 );
		}

		public string GetDataAsHtml( GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			int colspan = websiteLanguage.WantsBoth() ? 1 : 2;
			if ( websiteLanguage.WantsJp() ) {
				sb.Append( "<td colspan=\"" + colspan + "\">" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( inGameIdDict[NameStringDicId].StringJpnHtml( version, inGameIdDict ) );
				sb.Append( "</span><br>" );
				sb.Append( inGameIdDict[TextStringDicId].StringJpnHtml( version, inGameIdDict ) );
				sb.Append( "</td>" );
			}

			if ( websiteLanguage.WantsEn() ) {
				sb.Append( "<td colspan=\"" + colspan + "\">" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( inGameIdDict[NameStringDicId].StringEngHtml( version, inGameIdDict ) );
				sb.Append( "</span><br>" );
				sb.Append( inGameIdDict[TextStringDicId].StringEngHtml( version, inGameIdDict ) );
				sb.Append( "</td>" );
			}

			return sb.ToString();
		}
	}
}
