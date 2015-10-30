using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Xillia.SDB {
	class SDBEntry {
		public uint PointerIDString;
		public uint PointerText;
		public String IDString;
		public String Text;
		public override string ToString() { return IDString + ": " + Text; }
	}

	class SDB {
		public List<SDBEntry> TextList;
		public Util.Endianness Endian;

		public SDB( String filename ) {
			using ( System.IO.Stream stream = new System.IO.FileStream( filename, System.IO.FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading SDB failed!" );
				}
			}
		}

		public SDB( System.IO.Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading SDB failed!" );
			}
		}

		private bool LoadFile( System.IO.Stream stream ) {
			// not entirely sure if this is actually type information?
			uint type = stream.ReadUInt32();
			if ( type <= 0xFFFF ) {
				Endian = Util.Endianness.LittleEndian;
			} else {
				Endian = Util.Endianness.BigEndian;
				type = type.SwapEndian();
			}
			if ( type != 0x08 ) {
				System.Console.WriteLine( "Unknown SDB file type!" );
				return false;
			}

			uint Amount = stream.ReadUInt32().FromEndian( Endian );
			TextList = new List<SDBEntry>( (int)Amount );
			for ( uint i = 0; i < Amount; ++i ) {
				SDBEntry x = new SDBEntry();
				x.PointerIDString = (uint)stream.Position + stream.ReadUInt32().FromEndian( Endian );
				x.PointerText = (uint)stream.Position + stream.ReadUInt32().FromEndian( Endian );
				TextList.Add( x );
			}
			for ( int i = 0; i < TextList.Count; ++i ) {
				SDBEntry x = TextList[i];
				stream.Position = x.PointerIDString;
				x.IDString = stream.ReadUTF8Nullterm();
				stream.Position = x.PointerText;
				x.Text = stream.ReadUTF8Nullterm();
			}

			return true;
		}
	}
}
