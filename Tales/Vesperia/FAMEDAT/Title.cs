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

		public Title( System.IO.Stream stream ) {
			ID = stream.ReadUInt32().SwapEndian();
			NameStringDicID = stream.ReadUInt32().SwapEndian();
			DescStringDicID = stream.ReadUInt32().SwapEndian();
			Character = stream.ReadUInt32().SwapEndian();

			CostumeString = stream.ReadAscii( 0x10 ).TrimNull();

			BunnyGuildPointsMaybe = stream.ReadUInt32().SwapEndian();
			stream.DiscardBytes( 0xC );
		}

		public string GetDataAsHtml( GameVersion version, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();
			sb.Append( "<tr>" );
			sb.Append( "<td>" );
			Website.GenerateWebsite.AppendCharacterBitfieldAsImageString( sb, version, 1u << (int)( Character - 1 ) );
			sb.Append( "</td>" );
			sb.Append( "<td>" );
			sb.Append( "<span class=\"itemname\">" );
			sb.Append( VesperiaUtil.RemoveTags( inGameIdDict[NameStringDicID].StringJPN, true, true ) );
			sb.Append( "</span>" );
			sb.Append( "<br>" );
			sb.Append( VesperiaUtil.RemoveTags( inGameIdDict[DescStringDicID].StringJPN, true, true ).Replace( "\n", "<br>" ) );
			sb.Append( "</td>" );
			sb.Append( "<td>" );
			sb.Append( "<span class=\"itemname\">" );
			sb.Append( inGameIdDict[NameStringDicID].StringENG );
			sb.Append( "</span>" );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[DescStringDicID].StringENG.Replace( "\n", "<br>" ) );
			sb.Append( "</td>" );
			sb.Append( "<td>" );
			sb.Append( BunnyGuildPointsMaybe + " Fame point" + ( BunnyGuildPointsMaybe != 1 ? "s" : "" ) );
			sb.Append( "</td>" );

			return sb.ToString();
		}
	}
}
