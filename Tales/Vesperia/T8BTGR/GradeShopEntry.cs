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
		public GradeShopEntry( System.IO.Stream stream, uint refStringStart ) {
			uint entrySize = stream.ReadUInt32().SwapEndian();
			ID = stream.ReadUInt32().SwapEndian();
			InGameID = stream.ReadUInt32().SwapEndian();
			uint refStringLocation = stream.ReadUInt32().SwapEndian();

			NameStringDicID = stream.ReadUInt32().SwapEndian();
			DescStringDicID = stream.ReadUInt32().SwapEndian();
			GradeCost = stream.ReadUInt32().SwapEndian();

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
			sb.Append( "<tr>" );
			/*
			sb.Append( "<td>" );
			sb.Append( RefString );
			sb.Append( "</td>" );
			//*/
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
			sb.Append( GradeCost + " Grade" );
			sb.Append( "</td>" );

			return sb.ToString();
		}
	}
}
