using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTMA {
	public class Arte {
		public enum ArteType {
			Generic = 0,
			NoviceSpell = 1,
			IntermediateSpell = 2,
			AdvancedSpell = 3,
			Base = 4,
			Arcane = 5,
			BurstSpell = 6,
			Burst = 7,
			AlteredSpell = 8,
			Altered = 9,
			AlteredBurstSpell = 10,
			AlteredBurst = 11,
			FatalStrike = 12,
			Mystic = 13,
			OverLimit = 14,
			SkillAutomatic = 15
		}

		public byte[] Data;
		public uint StringIdName;
		public uint StringIdDescription;
		public ArteType Type;

		public Arte( byte[] Bytes, uint Location, uint Size ) {
			Data = new byte[Size];
			Util.CopyByteArrayPart( Bytes, (int)Location, Data, 0, (int)Size );

			StringIdName = BitConverter.ToUInt32( Data, 0x14 ).SwapEndian();
			StringIdDescription = BitConverter.ToUInt32( Data, 0x18 ).SwapEndian();
			Type = (ArteType)BitConverter.ToUInt32( Data, 0x1C ).SwapEndian();
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			foreach ( byte b in Data ) {
				sb.Append( b.ToString( "X2" ) );
				sb.Append( ' ' );
			}
			return sb.ToString();
		}

	}
}
