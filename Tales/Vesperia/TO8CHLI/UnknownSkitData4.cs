using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class UnknownSkitData4 {
		public ushort Unknown1a;
		public ushort Unknown1b;
		public ushort Unknown2a;
		public ushort Unknown2b;
		public uint Unknown3;
		public uint Unknown4;
		public UnknownSkitData4( System.IO.Stream stream ) {
			Unknown1a = stream.ReadUInt16().SwapEndian();
			Unknown1b = stream.ReadUInt16().SwapEndian();
			Unknown2a = stream.ReadUInt16().SwapEndian();
			Unknown2b = stream.ReadUInt16().SwapEndian();
			Unknown3 = stream.ReadUInt32().SwapEndian();
			Unknown4 = stream.ReadUInt32().SwapEndian();
		}

		public override string ToString() {
			return String.Format( "{0} / {1} / {2} / {3} / {4} / {5}", Unknown1a, Unknown1b, Unknown2a, Unknown2b, Unknown3, Unknown4 );
		}

	}
}
