using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PicrossDS {
	class OriginalPuzzle {
		// starts at 0x64, 0x7C8 bytes per puzzle
		ClassicPuzzle PuzzleData;
		byte[] Picture; // 0x708 bytes, 60x60 image, half a byte per color (4bpp, pre-existing palette)

		public OriginalPuzzle( byte[] File, uint Offset ) {
			PuzzleData = new ClassicPuzzle( File, Offset );
			Picture = new byte[0x708];
			Util.CopyByteArrayPart( File, (int)Offset + 0xC0, Picture, 0, 0x708 );
		}
	}
}
