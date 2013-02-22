using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PSP.GIM {
	class PaletteSection : ISection {
		public int Offset;

		public ushort Type;
		public ushort Unknown;
		public uint PartSizeDuplicate;
		public uint PartSize;
		public uint Unknown2;

		public ushort DataOffset;
		public ushort Unknown3;
		public ImageFormat Format;
		public ushort Unknown4;
		public ushort ColorDepth;
		public ushort Unknown5;
		public ushort Unknown6;
		public ushort Unknown7;

		public ushort Unknown8;
		public ushort Unknown9;
		public ushort Unknown10;
		public ushort Unknown11;
		public uint Unknown12;
		public uint Unknown13;

		public uint PartSizeMinus0x10;
		public uint Unknown14;
		public ushort Unknown15;
		public ushort LayerCount;
		public ushort Unknown17;
		public ushort FrameCount;

		public uint[] PaletteOffsets;
		public byte[][] PalettesRawBytes;
		public List<List<uint>> Palettes;


		public uint PaletteCount;

		public PaletteSection( byte[] File, int Offset ) {
			this.Offset = Offset;


			Type = BitConverter.ToUInt16( File, Offset );
			Unknown = BitConverter.ToUInt16( File, Offset + 0x02 );
			PartSizeDuplicate = BitConverter.ToUInt32( File, Offset + 0x04 );
			PartSize = BitConverter.ToUInt32( File, Offset + 0x08 );
			Unknown2 = BitConverter.ToUInt32( File, Offset + 0x0C );

			DataOffset = BitConverter.ToUInt16( File, Offset + 0x10 );
			Unknown3 = BitConverter.ToUInt16( File, Offset + 0x12 );
			Format = (ImageFormat)BitConverter.ToUInt16( File, Offset + 0x14 );
			Unknown4 = BitConverter.ToUInt16( File, Offset + 0x16 );
			ColorDepth = BitConverter.ToUInt16( File, Offset + 0x18 );
			Unknown5 = BitConverter.ToUInt16( File, Offset + 0x1A );
			Unknown6 = BitConverter.ToUInt16( File, Offset + 0x1C );
			Unknown7 = BitConverter.ToUInt16( File, Offset + 0x1E );

			Unknown8 = BitConverter.ToUInt16( File, Offset + 0x20 );
			Unknown9 = BitConverter.ToUInt16( File, Offset + 0x22 );
			Unknown10 = BitConverter.ToUInt16( File, Offset + 0x24 );
			Unknown11 = BitConverter.ToUInt16( File, Offset + 0x26 );
			Unknown12 = BitConverter.ToUInt32( File, Offset + 0x28 );
			Unknown13 = BitConverter.ToUInt32( File, Offset + 0x2C );

			PartSizeMinus0x10 = BitConverter.ToUInt32( File, Offset + 0x30 );
			Unknown14 = BitConverter.ToUInt32( File, Offset + 0x34 );
			Unknown15 = BitConverter.ToUInt16( File, Offset + 0x38 );
			LayerCount = BitConverter.ToUInt16( File, Offset + 0x3A );
			Unknown17 = BitConverter.ToUInt16( File, Offset + 0x3C );
			FrameCount = BitConverter.ToUInt16( File, Offset + 0x3E );

			PaletteCount = Math.Max( LayerCount, FrameCount );
			PaletteOffsets = new uint[PaletteCount];
			for ( int i = 0; i < PaletteCount; ++i ) {
				PaletteOffsets[i] = BitConverter.ToUInt32( File, Offset + 0x40 + i * 0x04 );
			}


			PalettesRawBytes = new byte[PaletteCount][];
			for ( int i = 0; i < PaletteOffsets.Length; ++i ) {
				uint poffs = PaletteOffsets[i];
				int size = ColorDepth * GetBytePerColor();
				PalettesRawBytes[i] = new byte[size];

				Util.CopyByteArrayPart( File, Offset + (int)poffs + 0x10, PalettesRawBytes[i], 0, size );
			}


			Palettes = new List<List<uint>>();
			foreach ( byte[] pal in PalettesRawBytes ) {
				int BytePerColor = GetBytePerColor();
				List<uint> IndividualPalette = new List<uint>();
				for ( int i = 0; i < pal.Length; i += BytePerColor ) {
					uint color = 0;
					if ( BytePerColor == 4 ) {
						color = BitConverter.ToUInt32( pal, i );
					} else if ( BytePerColor == 2 ) {
						color = BitConverter.ToUInt16( pal, i );
					}
					IndividualPalette.Add( color );
				}
				Palettes.Add( IndividualPalette );
			}


			return;
		}

		public int GetBytePerColor() {
			if ( Format == ImageFormat.RGBA4444 ) {
				return 2;
			}
			return 4;
		}


		public uint GetPartSize() {
			return PartSize;
		}
		public void Recalculate( int NewFilesize ) {
			if ( PaletteOffsets.Length != PalettesRawBytes.Length ) {
				PaletteOffsets = new uint[PalettesRawBytes.Length];
			}
			uint totalLength = 0;
			for ( int i = 0; i < PalettesRawBytes.Length; ++i ) {
				PaletteOffsets[i] = totalLength + 0x40;
				totalLength += (uint)PalettesRawBytes[i].Length;
			}

			PartSize = totalLength + 0x50;
			PartSizeDuplicate = totalLength + 0x50;
			PartSizeMinus0x10 = totalLength + 0x40;
			LayerCount = 1;
			FrameCount = 1;
		}


		public byte[] Serialize() {
			List<byte> serialized = new List<byte>( (int)PartSize );
			serialized.AddRange( BitConverter.GetBytes( Type ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown ) );
			serialized.AddRange( BitConverter.GetBytes( PartSizeDuplicate ) );
			serialized.AddRange( BitConverter.GetBytes( PartSize ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown2 ) );

			serialized.AddRange( BitConverter.GetBytes( DataOffset ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown3 ) );
			serialized.AddRange( BitConverter.GetBytes( (ushort)Format ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown4 ) );
			serialized.AddRange( BitConverter.GetBytes( ColorDepth ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown5 ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown6 ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown7 ) );

			serialized.AddRange( BitConverter.GetBytes( Unknown8 ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown9 ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown10 ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown11 ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown12 ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown13 ) );

			serialized.AddRange( BitConverter.GetBytes( PartSizeMinus0x10 ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown14 ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown15 ) );
			serialized.AddRange( BitConverter.GetBytes( LayerCount ) );
			serialized.AddRange( BitConverter.GetBytes( Unknown17 ) );
			serialized.AddRange( BitConverter.GetBytes( FrameCount ) );

			for ( int i = 0; i < PaletteOffsets.Length; ++i ) {
				serialized.AddRange( BitConverter.GetBytes( PaletteOffsets[i] ) );
			}
			while ( serialized.Count % 16 != 0 ) {
				serialized.Add( 0x00 );
			}
			int BytePerColor = GetBytePerColor();
			foreach ( List<uint> pal in Palettes ) {
				foreach ( uint col in pal ) {
					if ( BytePerColor == 4 ) {
						serialized.AddRange( BitConverter.GetBytes( col ) );
					} else if ( BytePerColor == 2 ) {
						serialized.AddRange( BitConverter.GetBytes( (ushort)col ) );
					}
				}
			}
			return serialized.ToArray();
		}
	}
}
