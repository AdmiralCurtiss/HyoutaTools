using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTXTM {
	public class FloorInfo {
		public uint EntrySize;
		public uint[] RestData;

		public string RefString1;
		public string RefString2;

		public FloorInfo( System.IO.Stream stream, uint refStringStart ) {
			EntrySize = stream.ReadUInt32().SwapEndian();
			RestData = new uint[( EntrySize - 4 ) / 4];

			for ( int i = 0; i < RestData.Length; ++i ) {
				RestData[i] = stream.ReadUInt32().SwapEndian();
			}

			long pos = stream.Position;
			stream.Position = refStringStart + RestData[2];
			RefString1 = stream.ReadAsciiNullterm();
			stream.Position = refStringStart + RestData[4];
			RefString2 = stream.ReadAsciiNullterm();
			stream.Position = pos;
		}

		public override string ToString() {
			return RefString1 + " / " + RefString2;
		}
	}
}
