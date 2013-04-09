using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.N64.OoTSaveEditor {
	public class OoTSaveHeader {
		private byte[] File;

		public byte SoundOptions;
		public byte ZTargetOptions;
		public ushort Unknown0x02;
		public ushort Unknown0x04;
		public byte Unknown0x06;
		public byte IdentifyTextZ;
		public uint IdentifyTextELDA;
		public uint GameVersion;

		public uint Unknown0x10;
		public uint Unknown0x14;
		public uint Unknown0x18;
		public uint Unknown0x1C;

		public OoTSaveHeader( byte[] File ) {
			this.File = File;

			SoundOptions = File[0x00];
			ZTargetOptions = File[0x01];
			Unknown0x02 = Util.SwapEndian( BitConverter.ToUInt16( File, 0x02 ) );
			Unknown0x04 = Util.SwapEndian( BitConverter.ToUInt16( File, 0x04 ) );
			Unknown0x06 = File[0x06];
			IdentifyTextZ = File[0x07];
			IdentifyTextELDA = Util.SwapEndian( BitConverter.ToUInt32( File, 0x08 ) );
			GameVersion = Util.SwapEndian( BitConverter.ToUInt32( File, 0x0C ) );

			Unknown0x10 = Util.SwapEndian( BitConverter.ToUInt32( File, 0x10 ) );
			Unknown0x14 = Util.SwapEndian( BitConverter.ToUInt32( File, 0x14 ) );
			Unknown0x18 = Util.SwapEndian( BitConverter.ToUInt32( File, 0x18 ) );
			Unknown0x1C = Util.SwapEndian( BitConverter.ToUInt32( File, 0x1C ) );

			return;
		}

		public void WriteToFile() {
			File[0x00] = SoundOptions;
			File[0x01] = ZTargetOptions;
			BitConverter.GetBytes( Util.SwapEndian( Unknown0x02 ) ).CopyTo( File, 0x02 );
			BitConverter.GetBytes( Util.SwapEndian( Unknown0x04 ) ).CopyTo( File, 0x04 );
			File[0x06] = Unknown0x06;
			File[0x07] = IdentifyTextZ;
			BitConverter.GetBytes( Util.SwapEndian( IdentifyTextELDA ) ).CopyTo( File, 0x08 );
			BitConverter.GetBytes( Util.SwapEndian( GameVersion ) ).CopyTo( File, 0x0C );

			BitConverter.GetBytes( Util.SwapEndian( Unknown0x10 ) ).CopyTo( File, 0x10 );
			BitConverter.GetBytes( Util.SwapEndian( Unknown0x14 ) ).CopyTo( File, 0x14 );
			BitConverter.GetBytes( Util.SwapEndian( Unknown0x18 ) ).CopyTo( File, 0x18 );
			BitConverter.GetBytes( Util.SwapEndian( Unknown0x1C ) ).CopyTo( File, 0x1C );
		}
	}
}
