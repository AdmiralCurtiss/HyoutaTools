﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.SYNPDAT {
	public class SynopsisEntry {
		public uint ID;
		public uint Unknown2;
		public uint Unknown3;
		public uint NameStringDicId;
		public uint TextStringDicId;

		public string RefString1;
		public string RefString2;

		public SynopsisEntry( System.IO.Stream stream ) {
			ID = stream.ReadUInt32().SwapEndian();
			Unknown2 = stream.ReadUInt32().SwapEndian();
			Unknown3 = stream.ReadUInt32().SwapEndian();
			NameStringDicId = stream.ReadUInt32().SwapEndian();
			TextStringDicId = stream.ReadUInt32().SwapEndian();
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

		public string GetDataAsHtml( GameVersion version, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			string[] textEng = inGameIdDict[TextStringDicId].StringENG.Replace( "\n", "<br>" ).Split( '\f' );
			string[] textJpn = inGameIdDict[TextStringDicId].StringJPN.Replace( "\n", "<br>" ).Split( '\f' );

			sb.Append( "<table class=\"synopsis\">" );
			sb.Append( "<tr id=\"synopsis" + ID + "\"><td class=\"synopsistitle\" colspan=\"" + textJpn.Length + "\">" );
			if ( version == GameVersion.PS3 ) {
				sb.Append( "<img src=\"synopsis/U_" + RefString1 + ".png\"><br>" );
			}
			sb.Append( inGameIdDict[NameStringDicId].StringJPN + "</td></tr><tr>" );
			foreach ( string s in textJpn ) {
				sb.Append( "<td>" + s + "</td>" );
			}
			sb.Append( "</tr>" );
			sb.Append( "</table>" );
			sb.Append( "<br>" );

			sb.Append( "<table class=\"synopsis\">" );
			sb.Append( "<tr id=\"synopsis" + ID + "\"><td class=\"synopsistitle\" colspan=\"" + textEng.Length + "\">" );
			sb.Append( inGameIdDict[NameStringDicId].StringENG + "</td></tr><tr>" );
			foreach ( string s in textEng ) {
				sb.Append( "<td>" + s + "</td>" );
			}
			sb.Append( "</tr>" );
			sb.Append( "</table>" );

			return sb.ToString();
		}
	}
}