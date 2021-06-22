using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Other.PSP.GIM {
	class HeaderSection : ISection {
		public byte[] Header;
		public HeaderSection( byte[] File, int Offset ) {
			Header = new byte[0x10];

			ArrayUtils.CopyByteArrayPart( File, Offset, Header, 0, 0x10 );
		}

		public uint GetPartSize() {
			return 0x10;
		}

		public void Recalculate( int NewFilesize ) {
			return;
		}


		public byte[] Serialize() {
			return Header;
		}
	}
}
