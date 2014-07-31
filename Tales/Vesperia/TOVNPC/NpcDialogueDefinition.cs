using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TOVNPC {
	public class NpcDialogueDefinition {
		public uint Unknown1;
		public uint Unknown2;
		public uint StringDicId;
		public uint Unknown4;

		public byte Unknown5a1;
		public byte Unknown5a2;
		public byte Unknown5b1;
		public byte Unknown5b2;
		public short Unknown6a;
		public short Unknown6b;
		public byte Unknown7a1;
		public byte Unknown7a2;
		public ushort Unknown7b;
		public uint Unknown8;

		public uint RefStringLocation1;
		public uint RefStringLocation2;
		public uint Unknown11;
		public uint Unknown12;

		public uint Unknown13;
		public uint Unknown14;
		public uint Unknown15;
		public uint Unknown16;

		public string RefString1;
		public string RefString2;

		public NpcDialogueDefinition( System.IO.Stream stream, uint refStringStart ) {
			Unknown1 = stream.ReadUInt32().SwapEndian();
			Unknown2 = stream.ReadUInt32().SwapEndian();
			StringDicId = stream.ReadUInt32().SwapEndian();
			Unknown4 = stream.ReadUInt32().SwapEndian();

			Unknown5a1 = (byte)stream.ReadByte();
			Unknown5a2 = (byte)stream.ReadByte();
			Unknown5b1 = (byte)stream.ReadByte();
			Unknown5b2 = (byte)stream.ReadByte();
			Unknown6a = (short)stream.ReadUInt16().SwapEndian();
			Unknown6b = (short)stream.ReadUInt16().SwapEndian();
			Unknown7a1 = (byte)stream.ReadByte();
			Unknown7a2 = (byte)stream.ReadByte();
			Unknown7b = stream.ReadUInt16().SwapEndian();
			Unknown8 = stream.ReadUInt32().SwapEndian();

			RefStringLocation1 = stream.ReadUInt32().SwapEndian();
			RefStringLocation2 = stream.ReadUInt32().SwapEndian();
			Unknown11 = stream.ReadUInt32().SwapEndian();
			Unknown12 = stream.ReadUInt32().SwapEndian();

			Unknown13 = stream.ReadUInt32().SwapEndian();
			Unknown14 = stream.ReadUInt32().SwapEndian();
			Unknown15 = stream.ReadUInt32().SwapEndian();
			Unknown16 = stream.ReadUInt32().SwapEndian();

			long pos = stream.Position;
			stream.Position = refStringStart + RefStringLocation1;
			RefString1 = stream.ReadAsciiNullterm();
			stream.Position = refStringStart + RefStringLocation2;
			RefString2 = stream.ReadAsciiNullterm();
			stream.Position = pos;
		}

		public override string ToString() {
			return RefString1 + " / " + RefString2;
		}
	}
}
