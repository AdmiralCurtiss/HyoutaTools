using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.SYNPDAT {
	public class SynopsisEntry {
		public uint ID;
		public uint StoryIdMin;
		public uint StoryIdMax;
		public uint NameStringDicId;
		public uint TextStringDicId;

		public string RefString1;
		public string RefString2;

		public SynopsisEntry( System.IO.Stream stream, EndianUtils.Endianness endian ) {
			ID = stream.ReadUInt32().FromEndian( endian );
			StoryIdMin = stream.ReadUInt32().FromEndian( endian );
			StoryIdMax = stream.ReadUInt32().FromEndian( endian );
			NameStringDicId = stream.ReadUInt32().FromEndian( endian );
			TextStringDicId = stream.ReadUInt32().FromEndian( endian );
			stream.DiscardBytes( 0xC );

			long pos = stream.Position;
			RefString1 = stream.ReadAsciiNullterm();
			stream.Position = pos + 0x10;
			RefString2 = stream.ReadAsciiNullterm();
			stream.Position = pos + 0x20;
		}

		public override string ToString() {
			return RefString1;
		}

		public string GetDataAsHtml( GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			var synopsisEntry = inGameIdDict[TextStringDicId];
			string jp = synopsisEntry.StringJpn != null ? synopsisEntry.StringJpn : "";
			jp = Website.WebsiteGenerator.ReplaceIconsWithHtml( new StringBuilder( jp ), inGameIdDict, version, true ).ToString();
			string en = synopsisEntry.StringEng != null ? synopsisEntry.StringEng : "";
			en = Website.WebsiteGenerator.ReplaceIconsWithHtml( new StringBuilder( en ), inGameIdDict, version, false ).ToString();

			string[] textJpn = jp.Split( '\f' );
			string[] textEng = en.Split( '\f' );
			for ( int i = 0; i < textJpn.Length; ++i ) { textJpn[i] = VesperiaUtil.RemoveTags( textJpn[i], inGameIdDict, true, true ).Replace( "\n", "<br />" ); }
			for ( int i = 0; i < textEng.Length; ++i ) { textEng[i] = VesperiaUtil.RemoveTags( textEng[i], inGameIdDict, false, true ).Replace( "\n", "<br />" ); }

			//sb.Append( "Unlocks between " + StoryIdMin + " and " + StoryIdMax + "<br>" );

			sb.Append( "<table class=\"synopsis\">" );
			sb.Append( "<tr id=\"synopsis" + ID + "\"><td class=\"synopsistitle\" colspan=\"" + textJpn.Length + "\">" );
			if ( version.HasPS3Content() ) {
				sb.Append( "<img src=\"synopsis/U_" + RefString1 + ".png\"><br><br>" );
			}
			if ( websiteLanguage.WantsJp() ) {
				sb.Append( inGameIdDict[NameStringDicId].StringJpnHtml( version, inGameIdDict ) + "</td></tr><tr>" );
				foreach ( string s in textJpn ) {
					sb.Append( "<td>" + s + "</td>" );
				}
			}
			sb.Append( "</tr>" );
			sb.Append( "</table>" );

			if ( websiteLanguage.WantsBoth() ) {
				sb.Append( "<br>" );
			}
			if ( websiteLanguage.WantsEn() ) {
				sb.Append( "<table class=\"synopsis\">" );
				sb.Append( "<tr id=\"synopsis" + ID + "\"><td class=\"synopsistitle\" colspan=\"" + textEng.Length + "\">" );
				sb.Append( inGameIdDict[NameStringDicId].StringEngHtml( version, inGameIdDict ) + "</td></tr><tr>" );
				foreach ( string s in textEng ) {
					sb.Append( "<td>" + s + "</td>" );
				}
				sb.Append( "</tr>" );
				sb.Append( "</table>" );
			}

			return sb.ToString();
		}
	}
}
