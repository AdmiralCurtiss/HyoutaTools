using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class SkitConditionForwarder {
		public ulong SkitConditionReference;
		public uint SkitConditionCount;
		public SkitConditionForwarder( System.IO.Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			SkitConditionReference = stream.ReadUInt( bits, endian );
			SkitConditionCount = stream.ReadUInt32().FromEndian( endian );
		}
	}
}
