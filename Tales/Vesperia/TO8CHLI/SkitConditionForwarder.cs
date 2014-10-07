using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class SkitConditionForwarder {
		public uint SkitConditionReference;
		public uint SkitConditionCount;
		public SkitConditionForwarder( System.IO.Stream stream ) {
			SkitConditionReference = stream.ReadUInt32().SwapEndian();
			SkitConditionCount = stream.ReadUInt32().SwapEndian();
		}
	}
}
