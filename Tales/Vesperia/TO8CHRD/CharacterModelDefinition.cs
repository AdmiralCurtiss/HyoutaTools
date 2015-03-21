using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHRD {
	public class CharacterModelDefinition {
		uint[] OtherData;

		public string[] Strings;

		public uint CustomIndex { get { return OtherData[0]; } }
		public uint Unknown1 { get { return OtherData[1]; } }
		public uint OtherIndex { get { return OtherData[2]; } }
		public uint U20BIndex { get { return OtherData[3]; } }
		public uint Unknown4 { get { return OtherData[4]; } }
		public uint U80Index { get { return OtherData[6]; } }
		public uint Unknown7 { get { return OtherData[7]; } }

		public CustomModelAddition Custom;
		public OtherModelAddition Other;
		public Unknown0x20byteAreaB Unknown0x20Area;
		public Unknown0x80byteArea Unknown0x80Area;

		public CharacterModelDefinition( System.IO.Stream stream, uint refStringStart ) {
			Strings = new string[100];
			OtherData = new uint[28];

			for ( uint i = 0; i < 100; ++i ) {
				Strings[i] = stream.ReadAsciiNulltermFromLocationAndReset( stream.ReadUInt32().SwapEndian() + refStringStart );
			}
			for ( uint i = 0; i < 28; ++i ) {
				OtherData[i] = stream.ReadUInt32().SwapEndian();
			}
		}

		public override string ToString() {
			return Strings[8];
		}
	}
}
