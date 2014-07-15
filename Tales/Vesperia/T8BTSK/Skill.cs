using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTSK {
	public class Skill {
		public uint ID;
		public uint InGameID;
		public uint NameStringDicID;
		public uint DescStringDicID;

		public uint Unknown7;
		public uint LearnableByBitmask;
		
		public uint EquipCost;
		public uint LearnCost;
		public uint Category;
		public uint SymbolValue;

		public float Unknown13;
		public float Unknown14;
		public float Unknown15;
		public uint Unknown16;

		public string RefString;

		public Skill( System.IO.Stream stream, uint refStringStart ) {
			uint entrySize = stream.ReadUInt32().SwapEndian();
			ID = stream.ReadUInt32().SwapEndian();
			InGameID = stream.ReadUInt32().SwapEndian();
			uint refStringLocation = stream.ReadUInt32().SwapEndian();

			NameStringDicID = stream.ReadUInt32().SwapEndian();
			DescStringDicID = stream.ReadUInt32().SwapEndian();
			Unknown7 = stream.ReadUInt32().SwapEndian();
			LearnableByBitmask = stream.ReadUInt32().SwapEndian();

			EquipCost = stream.ReadUInt32().SwapEndian();
			LearnCost = stream.ReadUInt32().SwapEndian();
			Category = stream.ReadUInt32().SwapEndian();
			// Game sums up this value per category, then figures out the OVL-symbol from the totals
			SymbolValue = stream.ReadUInt32().SwapEndian();

			Unknown13 = stream.ReadUInt32().SwapEndian().UIntToFloat();
			Unknown14 = stream.ReadUInt32().SwapEndian().UIntToFloat();
			Unknown15 = stream.ReadUInt32().SwapEndian().UIntToFloat();
			Unknown16 = stream.ReadUInt32().SwapEndian();

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
