using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;
using EndianUtils = HyoutaUtils.EndianUtils;

namespace HyoutaTools.Tales.Xillia.SDB {
	class SDBEntry {
		public uint PointerIDString;
		public uint PointerText;
		public String IDString;
		public String Text;
		public override string ToString() { return IDString + ": " + Text; }
	}

	class SDB {
		public uint Type;
		public List<uint> OtherData;
		public List<SDBEntry> TextList;
		public EndianUtils.Endianness Endian;

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
			Type = stream.ReadUInt32();
			if ( Type <= 0xFFFF ) {
				Endian = EndianUtils.Endianness.LittleEndian;
			} else {
				Endian = EndianUtils.Endianness.BigEndian;
				Type = Type.SwapEndian();
			}

			bool ToZ;
			switch ( Type ) {
				case 0x08: ToZ = false; break;
				case 0x100: ToZ = true; break;
				default: System.Console.WriteLine( "Unknown SDB file type!" ); return false;
			}

			if ( ToZ ) {
				// seems to always be 0x0C
				uint unknown = stream.ReadUInt32().FromEndian( Endian );
			}

			uint textAmount = stream.ReadUInt32().FromEndian( Endian );

			if ( ToZ ) {
				uint otherDataAmount = stream.ReadUInt32().FromEndian( Endian );
				OtherData = new List<uint>( (int)otherDataAmount );
				for ( uint i = 0; i < otherDataAmount; ++i ) {
					OtherData.Add( stream.ReadUInt32().FromEndian( Endian ) );
				}
				uint textOffset = stream.ReadUInt32().FromEndian( Endian );
			}

			TextList = new List<SDBEntry>( (int)textAmount );
			for ( uint i = 0; i < textAmount; ++i ) {
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

		public void Write( System.IO.Stream stream ) {
			stream.WriteUInt32( Type.ToEndian( Endian ) );

			bool ToZ = Type != 0x08;
			if ( ToZ ) {
				stream.WriteUInt32( EndianUtils.ToEndian( 0x0Cu, Endian ) );
			}

			stream.WriteUInt32( ( (uint)TextList.Count ).ToEndian( Endian ) );

			if ( ToZ ) {
				stream.WriteUInt32( ( (uint)OtherData.Count ).ToEndian( Endian ) );
				foreach ( uint d in OtherData ) {
					stream.WriteUInt32( d.ToEndian( Endian ) );
				}
				stream.WriteUInt32( (uint)( TextList.Count * 8 + 4 ) );
			}

			uint pointerLocation = (uint)stream.Position;
			for ( int i = 0; i < TextList.Count; ++i ) {
				// reserve space for pointers
				stream.WriteUInt32( 0 );
				stream.WriteUInt32( 0 );
			}

			foreach ( SDBEntry e in TextList ) {
				for ( int i = 0; i < 2; ++i ) {
					if ( ToZ ) {
						stream.WriteAlign( 2 );
					}
					uint pos = (uint)stream.Position;
					stream.Position = pointerLocation;
					stream.WriteUInt32( ( pos - pointerLocation ).ToEndian( Endian ) );
					stream.Position = pos;
					pointerLocation += 4;
					stream.WriteUTF8Nullterm( i == 0 ? e.IDString : e.Text );
				}
			}
		}
	}
}
