using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class SkitCondition {
		// what this condition checks for
		public ushort Type;
		// the type of comparison this condition performs, i.e. 4 <=, 5 >=
		public ushort MathOp;
		public ushort Unknown2a;
		public ushort Value1;
		public uint Value2;
		public uint Value3;
		public SkitCondition( System.IO.Stream stream ) {
			Type = stream.ReadUInt16().SwapEndian();
			MathOp = stream.ReadUInt16().SwapEndian();
			Unknown2a = stream.ReadUInt16().SwapEndian();
			Value1 = stream.ReadUInt16().SwapEndian();
			Value2 = stream.ReadUInt32().SwapEndian();
			Value3 = stream.ReadUInt32().SwapEndian();
		}
	}
}
