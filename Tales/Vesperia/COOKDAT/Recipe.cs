using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.COOKDAT {
	public class Recipe {
		public uint[] Data;

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

			NameStringDicID = Data[1];
			DescriptionStringDicID = Data[2];
			EffectStringDicID = Data[3];
		}

		public override string ToString() {
			return RefString;
		}
	}
}
