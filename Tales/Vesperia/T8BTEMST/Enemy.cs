using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTEMST {
	public class Enemy {
		public uint[] Data;
		public float[] DataFloat;

		public uint NameStringDicID;
		public uint InGameID;
		public string RefString;

		public Enemy( System.IO.Stream stream, uint refStringStart ) {
			Data = new uint[0x134 / 4];
			DataFloat = new float[0x134 / 4];

			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
				DataFloat[i] = Data[i].UIntToFloat();
			}

			NameStringDicID = Data[2];
			InGameID = Data[5];
			
			uint refStringLocation = Data[6];
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
