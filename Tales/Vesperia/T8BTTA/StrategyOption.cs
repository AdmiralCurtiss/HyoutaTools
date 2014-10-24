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
		public StrategyOption( System.IO.Stream stream, uint refStringStart ) {
			uint entrySize = stream.PeekUInt32().SwapEndian();

			Data = new uint[entrySize / 4];
			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
			}

			Category = Data[1];
			InGameID = Data[2];
			uint refStringLocation = Data[3];
			NameStringDicID = Data[4];
			DescStringDicID = Data[5];
			Characters = Data[6];
			ID = Data[7];

			long pos = stream.Position;
			stream.Position = refStringStart + refStringLocation;
			RefString = stream.ReadAsciiNullterm();
			stream.Position = pos;
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( GameVersion version, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();
			//sb.Append( RefString );
			sb.Append( "<tr id=\"strategyoption" + InGameID + "\">" );
			sb.Append( "<td>" );
			sb.Append( StrategySet.GetCategoryName( Category, version, inGameIdDict ) );
			sb.Append( "</td>" );

			sb.Append( "<td>" );
			sb.Append( "<span class=\"itemname\">" );
			sb.Append( inGameIdDict[NameStringDicID].StringJpnHtml( version ) );
			sb.Append( "</span>" );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[DescStringDicID].StringJpnHtml( version ) );
			sb.Append( "</td>" );

			sb.Append( "<td>" );
			sb.Append( "<span class=\"itemname\">" );
			sb.Append( inGameIdDict[NameStringDicID].StringEngHtml( version ) );
			sb.Append( "</span>" );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[DescStringDicID].StringEngHtml( version ) );
			sb.Append( "</td>" );

			sb.Append( "<td>" );
			Website.GenerateWebsite.AppendCharacterBitfieldAsImageString( sb, version, Characters );
			sb.Append( "</td>" );

			sb.Append( "</tr>" );
			return sb.ToString();
		}
	}
}
