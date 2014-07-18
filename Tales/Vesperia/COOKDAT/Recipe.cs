using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.COOKDAT {
	public class Recipe {
		public uint[] Data;

		public uint ID;
		public uint NameStringDicID;
		public uint DescriptionStringDicID;
		public uint EffectStringDicID;
		public string RefString;

		public Recipe( System.IO.Stream stream ) {
			Data = new uint[0xCC / 4]; // + 0x20

			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
			}
			long pos = stream.Position;
			RefString = stream.ReadAsciiNullterm();
			stream.Position = pos + 0x20;

			ID = Data[0];
			NameStringDicID = Data[1];
			DescriptionStringDicID = Data[2];
			EffectStringDicID = Data[3];
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( GameVersion version, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();
			sb.Append( "<div id=\"recipe" + ID + "\">" );
			sb.Append( "<img src=\"recipes/U_" + RefString + ".png\"><br>" );
			sb.Append( inGameIdDict[NameStringDicID].StringEngOrJpn + "<br>" );
			sb.Append( inGameIdDict[DescriptionStringDicID].StringEngOrJpn + "<br>" );
			sb.Append( inGameIdDict[EffectStringDicID].StringEngOrJpn + "<br>" );
			sb.Append( "</div>" );
			return sb.ToString();
		}
	}
}
