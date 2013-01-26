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
			Magic = Util.SwapEndian( BitConverter.ToUInt32( Header, 0x00 ) );
			CodeStart = Util.SwapEndian( BitConverter.ToUInt32( Header, 0x04 ) );
			CodeLength = Util.SwapEndian( BitConverter.ToUInt32( Header, 0x08 ) );
			TextStart = Util.SwapEndian( BitConverter.ToUInt32( Header, 0x0C ) );
			EntryCodeStart = Util.SwapEndian( BitConverter.ToUInt32( Header, 0x10 ) );
			EntryPointerEnd = Util.SwapEndian( BitConverter.ToUInt32( Header, 0x14 ) );
			TextLength = Util.SwapEndian( BitConverter.ToUInt32( Header, 0x18 ) );
			SectorSize = Util.SwapEndian( BitConverter.ToUInt32( Header, 0x1C ) );
		}

		public byte[] Serialize() {
			List<byte> Serialized = new List<byte>( 0x20 );

			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( Magic ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( CodeStart ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( CodeLength ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( TextStart ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( EntryCodeStart ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( EntryPointerEnd ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( TextLength ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( SectorSize ) ) );

			return Serialized.ToArray();
		}
	}
}
