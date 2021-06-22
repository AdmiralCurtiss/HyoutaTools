using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class SkitConditionForwarder {
		public ulong SkitConditionReference;
		public uint SkitConditionCount;
		public SkitConditionForwarder( System.IO.Stream stream, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			SkitConditionReference = stream.ReadUInt( bits, endian );
			SkitConditionCount = stream.ReadUInt32().FromEndian( endian );
		}
	}
}
