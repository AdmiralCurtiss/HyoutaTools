using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class UnknownSkitData4 {
		public uint ScenarioBegin;
		public uint ScenarioEnd;
		public ulong DataBegin;
		public uint DataCount;
		public UnknownSkitData4( System.IO.Stream stream, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			ScenarioBegin = stream.ReadUInt32().FromEndian( endian );
			ScenarioEnd = stream.ReadUInt32().FromEndian( endian );
			DataBegin = stream.ReadUInt( bits, endian );
			DataCount = stream.ReadUInt32().FromEndian( endian );
		}

		public override string ToString() {
			return String.Format( "{0} / {1} / {2} / {3}", ScenarioBegin, ScenarioEnd, DataBegin, DataCount );
		}

	}
}
