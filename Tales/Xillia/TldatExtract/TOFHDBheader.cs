using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Xillia.TldatExtract {
	public class TOFHDBhash {
		public UInt32 Key;
		public UInt32 Value;
	}
	public class TOFHDBfile {
		public UInt64 Filesize;
		public UInt64 CompressedSize;
		public UInt64 Offset;
		public UInt32 Hash;
		public String Extension;
		public UInt16 Unknown;
	}

	public class TOFHDBheader {
		public Util.Endianness Endian = Util.Endianness.BigEndian;

		public UInt64 CreationTime;
		public UInt32 FileHashArrayOffset;
		public UInt32 FileHashArraySize;
		public UInt32 VirtualHashArrayOffset;
		public UInt32 VirtualHashArraySize;
		public UInt32 FileArrayOffset;
		public UInt32 FileArraySize;
		public UInt32 VirtualPackArrayOffset;
		public UInt32 VirtualPackArraySize;

		public List<TOFHDBhash> FileHashArray;
		public List<TOFHDBfile> FileArray;

		public TOFHDBheader( String Filename ) {
			Load( new System.IO.FileStream( Filename, System.IO.FileMode.Open ) );
		}

		public UInt64 BiggestFile() {
			UInt64 Biggest = 0;

			foreach ( TOFHDBfile e in FileArray ) {
				if ( e.Filesize > Biggest ) Biggest = e.Filesize;
			}

			return Biggest;
		}

		private bool Load( System.IO.FileStream stream ) {
			CreationTime = stream.ReadUInt64();
			FileHashArrayOffset = stream.ReadUInt32();

			// This should always be 0x20, so we can use it to detect endianness
			if ( FileHashArrayOffset <= 0xFFFF ) {
				Endian = Util.Endianness.LittleEndian;
			} else {
				Endian = Util.Endianness.BigEndian;
				CreationTime = CreationTime.SwapEndian();
				FileHashArrayOffset = FileHashArrayOffset.SwapEndian();
			}

			// We adjust the offsets to be from file start rather than from position, makes them easier to seek to.
			FileHashArrayOffset += (uint)stream.Position - 4;
			FileHashArraySize = stream.ReadUInt32().FromEndian( Endian );
			VirtualHashArrayOffset = (uint)stream.Position;
			VirtualHashArrayOffset += stream.ReadUInt32().FromEndian( Endian );
			VirtualHashArraySize = stream.ReadUInt32().FromEndian( Endian );
			FileArrayOffset = (uint)stream.Position;
			FileArrayOffset += stream.ReadUInt32().FromEndian( Endian );
			FileArraySize = stream.ReadUInt32().FromEndian( Endian );
			VirtualPackArrayOffset = (uint)stream.Position;
			VirtualPackArrayOffset += stream.ReadUInt32().FromEndian( Endian );
			VirtualPackArraySize = stream.ReadUInt32().FromEndian( Endian );

			// Hashes
			stream.Position = FileHashArrayOffset;
			FileHashArray = new List<TOFHDBhash>( (int)FileHashArraySize );
			for ( UInt32 i = 0; i < FileHashArraySize; i++ ) {
				FileHashArray.Add( new TOFHDBhash() {
					Key = stream.ReadUInt32().FromEndian( Endian ),
					Value = stream.ReadUInt32().FromEndian( Endian ),
				} );
			}

			// Files
			stream.Position = FileArrayOffset;
			FileArray = new List<TOFHDBfile>( (int)FileArraySize );
			for ( UInt32 i = 0; i < FileArraySize; i++ ) {
				FileArray.Add( new TOFHDBfile() {
					Filesize = stream.ReadUInt64().FromEndian( Endian ),
					CompressedSize = stream.ReadUInt64().FromEndian( Endian ),
					Offset = stream.ReadUInt64().FromEndian( Endian ),
					Hash = stream.ReadUInt32().FromEndian( Endian ),
					Extension = stream.ReadAscii( 0x0A ).TrimNull(),
					Unknown = stream.ReadUInt16().FromEndian( Endian ),
				} );
			}

			return true;
		}

		public void Write( System.IO.FileStream stream ) {
			stream.WriteUInt64( CreationTime.ToEndian( Endian ) );
			stream.WriteUInt32( ( FileHashArrayOffset - (uint)stream.Position ).ToEndian( Endian ) );
			stream.WriteUInt32( FileHashArraySize.ToEndian( Endian ) );
			stream.WriteUInt32( ( VirtualHashArrayOffset - (uint)stream.Position ).ToEndian( Endian ) );
			stream.WriteUInt32( VirtualHashArraySize.ToEndian( Endian ) );
			stream.WriteUInt32( ( FileArrayOffset - (uint)stream.Position ).ToEndian( Endian ) );
			stream.WriteUInt32( FileArraySize.ToEndian( Endian ) );
			stream.WriteUInt32( ( VirtualPackArrayOffset - (uint)stream.Position ).ToEndian( Endian ) );
			stream.WriteUInt32( VirtualPackArraySize.ToEndian( Endian ) );

			foreach ( var h in FileHashArray ) {
				stream.WriteUInt32( h.Key.ToEndian( Endian ) );
				stream.WriteUInt32( h.Value.ToEndian( Endian ) );
			}

			foreach ( var f in FileArray ) {
				stream.WriteUInt64( f.Filesize.ToEndian( Endian ) );
				stream.WriteUInt64( f.CompressedSize.ToEndian( Endian ) );
				stream.WriteUInt64( f.Offset.ToEndian( Endian ) );
				stream.WriteUInt32( f.Hash.ToEndian( Endian ) );
				stream.WriteAscii( f.Extension, 0x0A );
				stream.WriteUInt16( f.Unknown.ToEndian( Endian ) );
			}
		}
	}
}
