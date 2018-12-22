using System;
using System.Collections.Generic;
using System.IO;
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

		public TSSHeader( Stream Header ) {
			Magic = Header.ReadUInt32().SwapEndian();
			CodeStart = Header.ReadUInt32().SwapEndian();
			CodeLength = Header.ReadUInt32().SwapEndian();
			TextStart = Header.ReadUInt32().SwapEndian();
			EntryCodeStart = Header.ReadUInt32().SwapEndian();
			EntryPointerEnd = Header.ReadUInt32().SwapEndian();
			TextLength = Header.ReadUInt32().SwapEndian();
			SectorSize = Header.ReadUInt32().SwapEndian();
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
