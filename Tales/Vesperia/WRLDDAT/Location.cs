using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.WRLDDAT {
	public class Location {
		public uint[] Data;

		public uint LocationID;
		public uint[] NameStringDicIDs;
		public uint[] DescStringDicIDs;
		public uint DefaultStringDicID;
		public string[] RefStrings;

		public Location( System.IO.Stream stream ) {
			Data = new uint[0x74 / 4]; // + 0x20*4 strings, + 4*4 StringDicIDs

			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
			}

			LocationID = Data[0];
			DefaultStringDicID = Data[1];
			DescStringDicIDs = new uint[4];
			for ( int i = 0; i < 4; ++i ) {
				DescStringDicIDs[i] = Data[5 + i];
			}

			long pos = stream.Position;
			RefStrings = new string[4];
			for ( int i = 0; i < 4; ++i ) {
				RefStrings[i] = stream.ReadAsciiNullterm();
				stream.Position = pos + 0x20 * ( i + 1 );
			}

			stream.Position = pos + 0x20 * 4;
			NameStringDicIDs = new uint[4];
			for ( int i = 0; i < 4; ++i ) {
				NameStringDicIDs[i] = stream.ReadUInt32().SwapEndian();
			}
		}

		public override string ToString() {
			return RefStrings[0];
		}

		public string GetDataAsHtml( GameVersion version, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			string def = inGameIdDict[DefaultStringDicID].StringEngOrJpn;

			sb.Append( "<div id=\"location" + LocationID + "\">" );
			//sb.Append( def + "<br>" );
			int validLocationCount = 0;
			for ( int i = 0; i < 4; ++i ) {
				if ( inGameIdDict[DescStringDicIDs[i]].StringEngOrJpn != "" ) {
					validLocationCount++;
				}
			}
			sb.Append( "<table><tr>" );
			for ( int i = 0; i < validLocationCount; ++i ) {
				sb.Append( "<td>" );
				sb.Append( inGameIdDict[NameStringDicIDs[i]].StringEngOrJpn + "<br>" );
				sb.Append( inGameIdDict[DescStringDicIDs[i]].StringEngOrJpn.Replace( "\n", "<br>" ) + "<br>" );
				if ( RefStrings[i] != "" ) {
					sb.Append( "<img src=\"worldmap/U_" + RefStrings[i] + ".png\"><br>" );
				}
				sb.Append( "</td>" );
			}
			sb.Append( "</tr></table>" );
			sb.Append( "</div>" );
			return sb.ToString();
		}
	}
}
