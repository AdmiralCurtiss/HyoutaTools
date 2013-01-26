using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TownMap {
	public class TownMapTile {
		public int Time;
		public int XTextureOffset;
		public int YTextureOffset;
		public int XSize;
		public int YSize;
		public int XDisplayOffset;
		public int YDisplayOffset;
		public int Unknown;

		public int Pointer;

		public TownMapTile( byte[] File, int Pointer ) {
			Time = Util.SwapEndian( BitConverter.ToInt32( File, Pointer ) );
			XTextureOffset = Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x04 ) );
			YTextureOffset = Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x08 ) );
			XSize = Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x0C ) );
			YSize = Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x10 ) );
			XDisplayOffset = Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x14 ) );
			YDisplayOffset = Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x18 ) );
			Unknown = Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x1C ) );

			this.Pointer = Pointer;
		}
	}
}
