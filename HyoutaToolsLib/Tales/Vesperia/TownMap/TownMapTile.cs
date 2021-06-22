using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EndianUtils = HyoutaUtils.EndianUtils;

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
			Time = EndianUtils.SwapEndian( BitConverter.ToInt32( File, Pointer ) );
			XTextureOffset = EndianUtils.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x04 ) );
			YTextureOffset = EndianUtils.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x08 ) );
			XSize = EndianUtils.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x0C ) );
			YSize = EndianUtils.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x10 ) );
			XDisplayOffset = EndianUtils.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x14 ) );
			YDisplayOffset = EndianUtils.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x18 ) );
			Unknown = EndianUtils.SwapEndian( BitConverter.ToInt32( File, Pointer + 0x1C ) );

			this.Pointer = Pointer;
		}
	}
}
