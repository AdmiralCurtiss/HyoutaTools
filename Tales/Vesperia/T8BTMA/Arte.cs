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

		public uint[] Data;
		public uint StringIdName;
		public uint StringIdDescription;
		public ArteType Type;

		//public float Something;
		public string RefString;

		public Arte( byte[] Bytes, uint Location, uint Size, uint refStringStart ) {
			Data = new uint[Size / 4];
			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = BitConverter.ToUInt32( Bytes, (int)( Location + i * 4 ) ).SwapEndian();
			}

			uint refStringLocaton = Data[3];
			RefString = Util.GetTextAscii( Bytes, (int)( refStringStart + refStringLocaton ) );

			StringIdName = Data[5];
			StringIdDescription = Data[6];
			Type = (ArteType)Data[7];
			//Something = Data[170].UIntToFloat();
		}

		public override string ToString() {
			return RefString;
		}

	}
}
