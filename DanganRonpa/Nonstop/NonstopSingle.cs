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
		AudioSpeaker = 21,
		AudioSampleId = 25,
		AudioChapter = 27,
		StructureSize = 0x3C / 2
	}


	public class NonstopSingle {
		public static int size = 0x3C;
		public UInt16[] data;

		public NonstopSingle( int offset, byte[] file ) {
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
