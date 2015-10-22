using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Xillia.TldatExtract {
	public class TOFHDBEntry {
		public UInt64 Filesize;
		public UInt64 CompressedSize;
		public UInt64 Offset;
		public UInt32 Hash;
		public String Extension;
		public UInt16 Unknown;
	}

	public class TOFHDBheader {
		public List<TOFHDBEntry> EntryList;
		public UInt32 FileAmount;
		public Util.Endianness Endian = Util.Endianness.BigEndian;

		public TOFHDBheader( String Filename ) {
			Load( System.IO.File.ReadAllBytes( Filename ) );
		}

		public UInt64 BiggestFile() {
			UInt64 Biggest = 0;

			foreach ( TOFHDBEntry e in EntryList ) {
				if ( e.Filesize > Biggest ) Biggest = e.Filesize;
			}

			return Biggest;
		}

		private bool Load( Byte[] Bytes ) {
			UInt32 unknown = BitConverter.ToUInt32( Bytes, 0x8 );
			// should be 0x20 or a similarly small number, use to detect endianness
			if ( unknown < 0xFFFF ) {
				Endian = Util.Endianness.LittleEndian;
			} else {
				Endian = Util.Endianness.BigEndian;
			}

			FileAmount = BitConverter.ToUInt32( Bytes, 0xC ).FromEndian( Endian );

			UInt32 FileListStart = 0x18 + BitConverter.ToUInt32( Bytes, 0x18 ).FromEndian( Endian );

			EntryList = new List<TOFHDBEntry>( (int)FileAmount );

			for ( UInt32 i = 0; i < FileAmount; i++ ) {
				TOFHDBEntry e = new TOFHDBEntry();
				e.Filesize = BitConverter.ToUInt64( Bytes, (int)( FileListStart + ( i * 0x28 ) + 0x00 ) ).FromEndian( Endian );
				e.CompressedSize = BitConverter.ToUInt64( Bytes, (int)( FileListStart + ( i * 0x28 ) + 0x08 ) ).FromEndian( Endian );
				e.Offset = BitConverter.ToUInt64( Bytes, (int)( FileListStart + ( i * 0x28 ) + 0x10 ) ).FromEndian( Endian );
				e.Hash = BitConverter.ToUInt32( Bytes, (int)( FileListStart + ( i * 0x28 ) + 0x18 ) ).FromEndian( Endian );
				e.Extension = Util.TrimNull( Encoding.ASCII.GetString( Bytes, (int)( FileListStart + ( i * 0x28 ) + 0x1C ), 0x0A ) );
				e.Unknown = BitConverter.ToUInt16( Bytes, (int)( FileListStart + ( i * 0x28 ) + 0x26 ) ).FromEndian( Endian );
				EntryList.Add( e );
			}


			return true;
		}
	}
}
