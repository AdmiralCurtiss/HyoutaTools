using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTTA {
	public class StrategySet {
		public uint[] Data;
		public uint NameStringDicID;
		public uint DescStringDicID;

		public string RefString;
		public StrategySet( System.IO.Stream stream, uint refStringStart ) {
			uint entrySize = stream.PeekUInt32().SwapEndian();
			
			Data = new uint[entrySize / 4];
			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
			}

			uint refStringLocation = Data[2];
			NameStringDicID = Data[3];
			DescStringDicID = Data[4];

			long pos = stream.Position;
			stream.Position = refStringStart + refStringLocation;
			RefString = stream.ReadAsciiNullterm();
			stream.Position = pos;
		}

		public override string ToString() {
			return RefString;
		}
	}
}
