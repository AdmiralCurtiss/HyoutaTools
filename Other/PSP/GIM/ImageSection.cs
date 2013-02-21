using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PSP.GIM {
	enum ImageFormat : short {
		RGBA5650 = 0,
		RGBA5551 = 1,
		RGBA4444 = 2,
		RGBA8888 = 3,
		Index4 = 4,
		Index8 = 5,
		Index16 = 6,
		Index32 = 7,
	}

	enum PixelOrder : short {
		Normal = 0,
		Faster = 1
	}

	class ImageSection : ISection {
		public int Offset;

		public ushort Type;
		public ushort Unknown;
		public uint PartSizeDuplicate;
		public uint PartSize;
		public uint Unknown2;

		public ushort DataOffset;
		public ushort Unknown3;
		public ImageFormat Format;
		public ushort PixelOrder;
		public ushort Width;
		public ushort Height;
		public ushort ColorDepth;
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

		public uint[] ImageOffsets;
		public byte[][] Images;


		public uint ImageCount;

		public ImageSection( byte[] File, int Offset ) {
			this.Offset = Offset;


			Type = BitConverter.ToUInt16( File, Offset );
			Unknown = BitConverter.ToUInt16( File, Offset + 0x02 );
			PartSizeDuplicate = BitConverter.ToUInt32( File, Offset + 0x04 );
			PartSize = BitConverter.ToUInt32( File, Offset + 0x08 );
			Unknown2 = BitConverter.ToUInt32( File, Offset + 0x0C );

			DataOffset = BitConverter.ToUInt16( File, Offset + 0x10 );
			Unknown3 = BitConverter.ToUInt16( File, Offset + 0x12 );
			Format = (ImageFormat)BitConverter.ToUInt16( File, Offset + 0x14 );
			PixelOrder = BitConverter.ToUInt16( File, Offset + 0x16 );
			Width = BitConverter.ToUInt16( File, Offset + 0x18 );
			Height = BitConverter.ToUInt16( File, Offset + 0x1A );
			ColorDepth = BitConverter.ToUInt16( File, Offset + 0x1C );
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

			ImageCount = Math.Max( LayerCount, FrameCount );
			ImageOffsets = new uint[ImageCount];
			for ( int i = 0; i < ImageCount; ++i ) {
				ImageOffsets[i] = BitConverter.ToUInt32( File, Offset + 0x40 + i * 0x04 );
			}


			Images = new byte[ImageCount][];
			for ( int i = 0; i < ImageOffsets.Length; ++i ) {
				uint poffs = ImageOffsets[i];
				uint nextpoffs;
				if ( i == ImageOffsets.Length - 1 ) {
					nextpoffs = PartSizeMinus0x10;
				} else {
					nextpoffs = ImageOffsets[i + 1];
				}
				uint size = nextpoffs - poffs;
				Images[i] = new byte[size];

				Util.CopyByteArrayPart( File, Offset + (int)poffs + 0x10, Images[i], 0, (int)size );
			}
		}

		public uint GetPartSize() {
			return PartSize;
		}


		public void Recalculate( int NewFilesize ) {
			if ( ImageOffsets.Length != Images.Length ) {
				ImageOffsets = new uint[Images.Length];
			}

			uint totalLength = 0;
			for ( int i = 0; i < Images.Length; ++i ) {
				ImageOffsets[i] = totalLength + 0x40;
				totalLength += (uint)Images[i].Length;
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
			serialized.AddRange( BitConverter.GetBytes( PixelOrder ) );
			serialized.AddRange( BitConverter.GetBytes( Width ) );
			serialized.AddRange( BitConverter.GetBytes( Height ) );
			serialized.AddRange( BitConverter.GetBytes( ColorDepth ) );
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

			for ( int i = 0; i < ImageOffsets.Length; ++i ) {
				serialized.AddRange( BitConverter.GetBytes( ImageOffsets[i] ) );
			}
			while ( serialized.Count % 16 != 0 ) {
				serialized.Add( 0x00 );
			}
			for ( int i = 0; i < Images.Length; ++i ) {
				serialized.AddRange( Images[i] );
			}
			return serialized.ToArray();
		}
	}
}
