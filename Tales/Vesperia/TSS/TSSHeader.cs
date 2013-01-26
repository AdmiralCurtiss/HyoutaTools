using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TSS {
	public class TSSHeader {
		public UInt32 Magic;
		public UInt32 CodeStart;
		public UInt32 CodeLength;
		public UInt32 TextStart;
		public UInt32 EntryCodeStart;
		public UInt32 EntryPointerEnd;
		public UInt32 TextLength;
		public UInt32 SectorSize;

		public TSSHeader( byte[] Header ) {
			Magic = HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( Header, 0x00 ) );
			CodeStart = HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( Header, 0x04 ) );
			CodeLength = HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( Header, 0x08 ) );
			TextStart = HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( Header, 0x0C ) );
			EntryCodeStart = HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( Header, 0x10 ) );
			EntryPointerEnd = HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( Header, 0x14 ) );
			TextLength = HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( Header, 0x18 ) );
			SectorSize = HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( Header, 0x1C ) );
		}

		public byte[] Serialize() {
			List<byte> Serialized = new List<byte>( 0x20 );

			Serialized.AddRange( System.BitConverter.GetBytes( HyoutaTools.Util.SwapEndian( Magic ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( HyoutaTools.Util.SwapEndian( CodeStart ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( HyoutaTools.Util.SwapEndian( CodeLength ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( HyoutaTools.Util.SwapEndian( TextStart ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( HyoutaTools.Util.SwapEndian( EntryCodeStart ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( HyoutaTools.Util.SwapEndian( EntryPointerEnd ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( HyoutaTools.Util.SwapEndian( TextLength ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( HyoutaTools.Util.SwapEndian( SectorSize ) ) );

			return Serialized.ToArray();
		}
	}
}
