using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PSP.GIM {
	class EndOfFileSection : ISection {

		public ushort Type;
		public ushort Unknown;
		public uint EndOfFileAddress;
		public uint PartSize;
		public uint Unknown2;

		public EndOfFileSection( byte[] File, int Offset ) {
			Type = BitConverter.ToUInt16( File, Offset );
			Unknown = BitConverter.ToUInt16( File, Offset + 0x02 );
			EndOfFileAddress = BitConverter.ToUInt32( File, Offset + 0x04 );
			PartSize = BitConverter.ToUInt32( File, Offset + 0x08 );
			Unknown2 = BitConverter.ToUInt32( File, Offset + 0x0C );
		}


		public uint GetPartSize() {
			return PartSize;
		}


		public void Recalculate( int NewFilesize ) {
			EndOfFileAddress = (uint)NewFilesize;
		}


		public byte[] Serialize() {
			List<byte> serialized = new List<byte>( (int)PartSize );
			serialized.AddRange( BitConverter.GetBytes( Type ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown ) );
			serialized.AddRange( BitConverter.GetBytes( EndOfFileAddress ) );
			serialized.AddRange( BitConverter.GetBytes( PartSize ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown2 ) );
			return serialized.ToArray();
		}
	}
}
