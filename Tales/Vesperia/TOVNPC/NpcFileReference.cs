using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TOVNPC {
	public class NpcFileReference {
		public string Map;
		public string Filename;
		public uint Filesize;

		public NpcFileReference( System.IO.Stream stream, uint refStringStart ) {
			uint refStringLocation1 = stream.ReadUInt32().SwapEndian();
			uint refStringLocation2 = stream.ReadUInt32().SwapEndian();
			Filesize = stream.ReadUInt32().SwapEndian();
			stream.ReadUInt32();

			long pos = stream.Position;
			stream.Position = refStringStart + refStringLocation1;
			Map = stream.ReadAsciiNullterm();
			stream.Position = refStringStart + refStringLocation2;
			Filename = stream.ReadAsciiNullterm();
			stream.Position = pos;
		}

		public override string ToString() {
			return Map + " / " + Filename + " / " + Filesize;
		}
	}
}
