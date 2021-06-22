using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HyoutaUtils;
using EndianUtils = HyoutaUtils.EndianUtils;

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

		public TSSHeader( Stream Header, EndianUtils.Endianness endian ) {
			Magic = Header.ReadUInt32().SwapEndian();
			CodeStart = Header.ReadUInt32().FromEndian( endian );
			CodeLength = Header.ReadUInt32().FromEndian( endian );
			TextStart = Header.ReadUInt32().FromEndian( endian );
			EntryCodeStart = Header.ReadUInt32().FromEndian( endian );
			EntryPointerEnd = Header.ReadUInt32().FromEndian( endian );
			TextLength = Header.ReadUInt32().FromEndian( endian );
			SectorSize = Header.ReadUInt32().FromEndian( endian );
		}

		public byte[] Serialize() {
			List<byte> Serialized = new List<byte>( 0x20 );

			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( Magic ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( CodeStart ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( CodeLength ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( TextStart ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( EntryCodeStart ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( EntryPointerEnd ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( TextLength ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( SectorSize ) ) );

			return Serialized.ToArray();
		}
	}
}
