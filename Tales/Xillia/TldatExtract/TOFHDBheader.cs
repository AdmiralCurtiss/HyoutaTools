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
			FileAmount = Util.SwapEndian( BitConverter.ToUInt32( Bytes, 0xC ) );

			UInt32 FileListStart = 0x18 + Util.SwapEndian( BitConverter.ToUInt32( Bytes, 0x18 ) );

			EntryList = new List<TOFHDBEntry>( (int)FileAmount );

			for ( UInt32 i = 0; i < FileAmount; i++ ) {
				TOFHDBEntry e = new TOFHDBEntry();
				e.Filesize = Util.SwapEndian( BitConverter.ToUInt64( Bytes, (int)( FileListStart + ( i * 0x28 ) + 0x00 ) ) );
				e.CompressedSize = Util.SwapEndian( BitConverter.ToUInt64( Bytes, (int)( FileListStart + ( i * 0x28 ) + 0x08 ) ) );
				e.Offset = Util.SwapEndian( BitConverter.ToUInt64( Bytes, (int)( FileListStart + ( i * 0x28 ) + 0x10 ) ) );
				e.Hash = Util.SwapEndian( BitConverter.ToUInt32( Bytes, (int)( FileListStart + ( i * 0x28 ) + 0x18 ) ) );
				e.Extension = Util.TrimNull( Encoding.ASCII.GetString( Bytes, (int)( FileListStart + ( i * 0x28 ) + 0x1C ), 0x0A ) );
				e.Unknown = Util.SwapEndian( BitConverter.ToUInt16( Bytes, (int)( FileListStart + ( i * 0x28 ) + 0x26 ) ) );
				EntryList.Add( e );
			}


			return true;
		}
	}
}
