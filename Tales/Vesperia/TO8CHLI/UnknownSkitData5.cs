using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class UnknownSkitData5 {
		public uint Unknown1;
		public UnknownSkitData5( System.IO.Stream stream ) {
			Unknown1 = stream.ReadUInt32().SwapEndian();
		}
	}
}
