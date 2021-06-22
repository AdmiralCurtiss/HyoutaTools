using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Other.PSP.GIM {
	class FileInfoSection : ISection {
		public ushort Type;
		public ushort Unknown;
		public uint PartSizeDuplicate;
		public uint PartSize;
		public uint Unknown2;

		public byte[] FileInfo;

		public FileInfoSection( byte[] File, int Offset ) {
			Type = BitConverter.ToUInt16( File, Offset );
			Unknown = BitConverter.ToUInt16( File, Offset + 0x02 );
			PartSizeDuplicate = BitConverter.ToUInt32( File, Offset + 0x04 );
			PartSize = BitConverter.ToUInt32( File, Offset + 0x08 );
			Unknown2 = BitConverter.ToUInt32( File, Offset + 0x0C );

			uint size = PartSize - 0x10;
			FileInfo = new byte[size];
			ArrayUtils.CopyByteArrayPart( File, Offset + 0x10, FileInfo, 0, (int)size );
		}

		public uint GetPartSize() {
			return PartSize;
		}


		public void Recalculate( int NewFilesize ) {
			PartSize = (uint)FileInfo.Length + 0x10;
			PartSizeDuplicate = PartSize;
		}


		public byte[] Serialize() {
			List<byte> serialized = new List<byte>( (int)PartSize );
			serialized.AddRange( BitConverter.GetBytes( Type ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown ) );
			serialized.AddRange( BitConverter.GetBytes( PartSizeDuplicate ) );
			serialized.AddRange( BitConverter.GetBytes( PartSize ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown2 ) );
			serialized.AddRange( FileInfo );
			return serialized.ToArray();
		}
	}
}
