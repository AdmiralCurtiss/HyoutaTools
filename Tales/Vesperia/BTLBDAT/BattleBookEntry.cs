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
			sb.Append( inGameIdDict[NameStringDicId].StringJPN );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[TextStringDicId].StringJPN.Replace( "\n", "<br>" ) );
			sb.Append( "<br>" );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[NameStringDicId].StringENG );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[TextStringDicId].StringENG.Replace( "\n", "<br>" ) );
			sb.Append( "<br>" );
			sb.Append( "<br>" );
			return sb.ToString();
		}
	}
}
