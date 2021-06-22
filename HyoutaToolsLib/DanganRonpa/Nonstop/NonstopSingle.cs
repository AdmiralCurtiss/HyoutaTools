using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.DanganRonpa.Nonstop {
	public enum NonstopSingleStructure {
		StringID = 0x00,
		TimeBeforeLineAdvance = 7,
		TimeBeforeFadeoutMaybe = 11,
		HorizontalPosition = 12,
		VerticalPosition = 13,
		Character = 21,
		Sprite = 22,
		AudioSampleId = 25,
		Chapter = 27,
		Type = 1,
		HasWeakPoint = 6,
		ShootWithEvidence = 3,
		ShootWithWeakpoint = 4,
		//StructureSize = 0x3C / 2 // DR1
		//StructureSize = 0x44 / 2 // SDR2
	}


	public class NonstopSingle {
		public int size = 0x3C;
		public UInt16[] data;

		public NonstopSingle( int offset, byte[] file, int size ) {
			this.size = size;
			data = new UInt16[size / 2];

			for ( int i = 0; i < size / 2; ++i ) {
				data[i] = BitConverter.ToUInt16( file, offset + i * 0x02 );
			}

			return;
		}

		public override string ToString() {
			return data[0].ToString();
		}
	}
}
