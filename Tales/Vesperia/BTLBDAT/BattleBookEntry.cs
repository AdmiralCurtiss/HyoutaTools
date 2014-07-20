using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.BTLBDAT {
	public class BattleBookEntry {
		public uint UnlockReferenceMaybe;
		public uint NameStringDicId;
		public uint TextStringDicId;

		public BattleBookEntry( System.IO.Stream stream ) {
			UnlockReferenceMaybe = stream.ReadUInt32().SwapEndian();
			NameStringDicId = stream.ReadUInt32().SwapEndian();
			TextStringDicId = stream.ReadUInt32().SwapEndian();
			stream.DiscardBytes( 0x4 );
		}

		public string GetDataAsHtml( GameVersion version, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();
			sb.Append( "<td>" );

			sb.Append( "<span class=\"itemname\">" );
			sb.Append( VesperiaUtil.RemoveTags( inGameIdDict[NameStringDicId].StringJPN, true, true ) );
			sb.Append( "</span><br>" );
			sb.Append( VesperiaUtil.RemoveTags( inGameIdDict[TextStringDicId].StringJPN, true, true ).Replace( "\n", "<br>" ) );
			sb.Append( "</td>" );
			sb.Append( "<td>" );
			sb.Append( "<span class=\"itemname\">" );
			sb.Append( inGameIdDict[NameStringDicId].StringENG );
			sb.Append( "</span><br>" );
			sb.Append( inGameIdDict[TextStringDicId].StringENG.Replace( "\n", "<br>" ) );
			sb.Append( "</td>" );
			return sb.ToString();
		}
	}
}
