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

		public TSS.TSSEntry GetLastValidName( Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			for ( int i = 3; i >= 0; --i ) {
				if ( inGameIdDict[DescStringDicIDs[i]].StringEngOrJpn != "" ) {
					return inGameIdDict[NameStringDicIDs[i]];
				}
			}
			return inGameIdDict[DefaultStringDicID];
		}

		public string GetDataAsHtml( GameVersion version, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			string defJpn = VesperiaUtil.RemoveTags( inGameIdDict[DefaultStringDicID].StringJPN, true, true );
			string defEng = inGameIdDict[DefaultStringDicID].StringENG;

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
				sb.Append( VesperiaUtil.RemoveTags( inGameIdDict[NameStringDicIDs[i]].StringJPN, true, true ) + "<br>" );
				sb.Append( VesperiaUtil.RemoveTags( inGameIdDict[DescStringDicIDs[i]].StringJPN, true, true ).Replace( "\n", "<br>" ) + "<br>" );
				sb.Append( inGameIdDict[NameStringDicIDs[i]].StringENG + "<br>" );
				sb.Append( inGameIdDict[DescStringDicIDs[i]].StringENG.Replace( "\n", "<br>" ) + "<br>" );
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
