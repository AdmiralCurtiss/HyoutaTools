using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTXTM {
	public class TreasureInfo {
		public uint EntrySize;
		public uint ID;
		public uint IDAgain;
		public uint RefStringLocation;

		// treasure chest types?
		public uint[] ChestTypes;

		// treasure chest positions on x/y ground plane?
		public float[] ChestPositions;

		// four slots, 0/1/2 -> possible treasures for slot 1, 3/4/5 -> for slot 2, etc.
		public uint[] Items;
		public uint[] Chances;

		public string RefString;

		public TreasureInfo( System.IO.Stream stream, uint refStringStart ) {
			EntrySize = stream.ReadUInt32().SwapEndian();
			ID = stream.ReadUInt32().SwapEndian();
			IDAgain = stream.ReadUInt32().SwapEndian();
			RefStringLocation = stream.ReadUInt32().SwapEndian();

			ChestTypes = new uint[4];
			for ( int i = 0; i < ChestTypes.Length; ++i ) {
				ChestTypes[i] = stream.ReadUInt32().SwapEndian();
			}

			ChestPositions = new float[8];
			for ( int i = 0; i < ChestPositions.Length; ++i ) {
				ChestPositions[i] = stream.ReadUInt32().SwapEndian().UIntToFloat();
			}

			Items = new uint[12];
			for ( int i = 0; i < Items.Length; ++i ) {
				Items[i] = stream.ReadUInt32().SwapEndian();
			}
			Chances = new uint[12];
			for ( int i = 0; i < Chances.Length; ++i ) {
				Chances[i] = stream.ReadUInt32().SwapEndian();
			}

			long pos = stream.Position;
			stream.Position = refStringStart + RefStringLocation;
			RefString = stream.ReadAsciiNullterm();
			stream.Position = pos;
		}

		public override string ToString() {
			return RefString;
		}

	}
}
