using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHRD {
	public class CharacterModelDefinition {
		public string[] Strings;

		public uint CustomIndex;
		public byte CustomCount;
		public byte OtherCount;
		public byte Unknown0x20AreaCount;
		public byte Unknown1d;
		public uint OtherIndex;
		public uint Unknown0x20AreaIndex;
		public byte Unknown4a;
		public byte Unknown4b;
		public byte Unknown4c;
		public byte Unknown4d;
		public uint Unknown0x80AreaIndex;
		public byte Unknown0x80AreaCount;
		public byte Unknown7b;

		public CustomModelAddition[] Custom;
		public OtherModelAddition[] Other;
		public Unknown0x20byteAreaB[] Unknown0x20Area;
		public Unknown0x80byteArea[] Unknown0x80Area;

		public CharacterModelDefinition( System.IO.Stream stream, uint refStringStart ) {
			Strings = new string[100];

			for ( uint i = 0; i < 100; ++i ) {
				Strings[i] = stream.ReadAsciiNulltermFromLocationAndReset( stream.ReadUInt32().SwapEndian() + refStringStart );
			}

			CustomIndex = stream.ReadUInt32().SwapEndian();
			CustomCount = (byte)stream.ReadByte();
			OtherCount = (byte)stream.ReadByte();
			Unknown0x20AreaCount = (byte)stream.ReadByte();
			Unknown1d = (byte)stream.ReadByte();
			OtherIndex = stream.ReadUInt32().SwapEndian();
			Unknown0x20AreaIndex = stream.ReadUInt32().SwapEndian();
			Unknown4a = (byte)stream.ReadByte();
			Unknown4b = (byte)stream.ReadByte();
			Unknown4c = (byte)stream.ReadByte();
			Unknown4d = (byte)stream.ReadByte();
			stream.DiscardBytes( 4 );
			Unknown0x80AreaIndex = stream.ReadUInt32().SwapEndian();
			Unknown0x80AreaCount = (byte)stream.ReadByte();
			Unknown7b = (byte)stream.ReadByte();
			stream.DiscardBytes( 2 );

			stream.DiscardBytes( 80 );
		}

		public override string ToString() {
			return Strings[8];
		}
	}
}
