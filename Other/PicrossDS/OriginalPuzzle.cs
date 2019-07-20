using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Other.PicrossDS {
	public class OriginalPuzzle : ClassicPuzzle {
		// starts at 0x64, 0x7C8 bytes per puzzle
		byte[] Picture; // 0x708 bytes, 60x60 image, half a byte per color (4bpp, pre-existing palette)

		public OriginalPuzzle() : base() { }
		public OriginalPuzzle( byte[] File, uint Offset )
			: base( File, Offset ) {
			Picture = new byte[0x708];
			if ( File.Length >= Offset + 0x7C8 ) {
				ArrayUtils.CopyByteArrayPart( File, (int)Offset + 0xC0, Picture, 0, 0x708 );
			}
		}

		public override void Write( byte[] File, uint Offset ) {
			base.Write( File, Offset );
			Picture.CopyTo( File, Offset + 0xC0 );
		}

		public override string ToString() {
			string pzlName = PuzzleName.Contains( '\0' ) ? PuzzleName.Substring( 0, PuzzleName.IndexOf( '\0' ) ) : PuzzleName;
			string pckName = PackName.Contains( '\0' ) ? PackName.Substring( 0, PackName.IndexOf( '\0' ) ) : PackName;
			return "Original Puzzle #" + IndexForGui.ToString( "D3" ) + " - " + pckName + " - " + pzlName;
		}
	}
}
