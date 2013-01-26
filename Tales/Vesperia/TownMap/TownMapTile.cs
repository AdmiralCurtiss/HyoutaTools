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
			Time = HyoutaTools.Util.SwapEndian( BitConverter.ToInt32( File, Pointer ) );
			XTextureOffset = HyoutaTools.Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x04 ) );
			YTextureOffset = HyoutaTools.Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x08 ) );
			XSize = HyoutaTools.Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x0C ) );
			YSize = HyoutaTools.Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x10 ) );
			XDisplayOffset = HyoutaTools.Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x14 ) );
			YDisplayOffset = HyoutaTools.Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x18 ) );
			Unknown = HyoutaTools.Util.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x1C ) );

			this.Pointer = Pointer;
		}
	}
}
