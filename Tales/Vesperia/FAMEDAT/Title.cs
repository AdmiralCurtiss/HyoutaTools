using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.FAMEDAT {
	public class Title {
		public uint ID;
		public uint NameStringDicID;
		public uint DescStringDicID;
		public uint Character;
		public uint BunnyGuildPointsMaybe;

		public string CostumeString;

		public Title( System.IO.Stream stream, Util.Endianness endian ) {
			ID = stream.ReadUInt32().FromEndian( endian );
			NameStringDicID = stream.ReadUInt32().FromEndian( endian );
			DescStringDicID = stream.ReadUInt32().FromEndian( endian );
			Character = stream.ReadUInt32().FromEndian( endian );

			CostumeString = stream.ReadAscii( 0x10 ).TrimNull();

			BunnyGuildPointsMaybe = stream.ReadUInt32().FromEndian( endian );
			stream.DiscardBytes( 0xC );
		}

		public override string ToString() {
			return ID.ToString();
		}

		public string GetDataAsHtml( GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			var nameEn = inGameIdDict[NameStringDicID].StringEng;
			var nameJp = inGameIdDict[NameStringDicID].StringJpn;

			StringBuilder sb = new StringBuilder();
			sb.Append( "<tr>" );
			sb.Append( "<td>" );
			Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, inGameIdDict, version, 1u << (int)( Character - 1 ), websiteLanguage.MainJp() );
			sb.Append( "</td>" );

			int colspan = websiteLanguage.WantsBoth() ? 1 : 2;
			if ( websiteLanguage.WantsJp() ) {
				sb.Append( "<td colspan=\"" + colspan + "\">" );
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

				if ( nameEn.Contains( "\x06(COS)" ) && !nameJp.Contains( "\x06(COS)" ) ) {
					sb.Append( nameEn.Replace( "\x06(COS)", "" ).ToHtmlEng( inGameIdDict, version ) );
					Console.WriteLine( "Removed EN costume icon for " + nameEn );
				} else {
					sb.Append( inGameIdDict[NameStringDicID].StringEngHtml( version, inGameIdDict ) );
				}

				sb.Append( "</span>" );
				sb.Append( "<br>" );
				sb.Append( inGameIdDict[DescStringDicID].StringEngHtml( version, inGameIdDict ) );
				sb.Append( "</td>" );
			}

			sb.Append( "<td>" );
			sb.Append( BunnyGuildPointsMaybe + " Fame point" + ( BunnyGuildPointsMaybe != 1 ? "s" : "" ) );
			sb.Append( "</td>" );

			return sb.ToString();
		}
	}
}
