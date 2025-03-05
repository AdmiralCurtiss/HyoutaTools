using System;
using System.Drawing;
using System.IO;
using HyoutaUtils;

namespace HyoutaTools.Textures {
	public enum TextureFormat {
		ABGR,
		RGBA,
		DXT1c, // no alpha
		DXT1a, // with 1 bit alpha
		DXT3,
		DXT5,
		BC5_UNORM,
		BC7_UNORM,
	}

	public class DDSHeader {
		public uint Magic = 0x20534444;
		public uint Size = 0x7C;
		public uint Flags;
		public uint Height;
		public uint Width;
		public uint PitchOrLinearSize;
		public uint Depth;
		public uint MipMapCount;
		public uint Reserved1a;
		public uint Reserved1b;
		public uint Reserved1c;
		public uint Reserved1d;
		public uint Reserved1e;
		public uint Reserved1f;
		public uint Reserved1g;
		public uint Reserved1h;
		public uint Reserved1i;
		public uint Reserved1j;
		public uint Reserved1k;
		public DDSPixelFormat PixelFormat = new DDSPixelFormat();
		public uint Caps;
		public uint Caps2;
		public uint Caps3;
		public uint Caps4;
		public uint Reserved2;

		public uint DXT10_DxgiFormat = 0;
		public uint DXT10_ResourceDimension = 0;
		public uint DXT10_MiscFlag = 0;
		public uint DXT10_ArraySize = 0;
		public uint DXT10_MiscFlags2 = 0;

		public static DDSHeader FromStream( Stream stream ) {
			DDSHeader header = new DDSHeader();
			header.Magic = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			if ( header.Magic != 0x20534444 ) {
				throw new Exception( "Invalid magic." );
			}
			header.Size = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			if ( header.Size != 0x7C ) {
				throw new Exception( "Invalid size." );
			}
			header.Flags = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Height = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Width = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.PitchOrLinearSize = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Depth = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.MipMapCount = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Reserved1a = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Reserved1b = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Reserved1c = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Reserved1d = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Reserved1e = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Reserved1f = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Reserved1g = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Reserved1h = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Reserved1i = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Reserved1j = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Reserved1k = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.PixelFormat = DDSPixelFormat.FromStream( stream );
			header.Caps = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Caps2 = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Caps3 = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Caps4 = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			header.Reserved2 = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );

			if (header.PixelFormat.FourCC == 0x30315844) {
				header.DXT10_DxgiFormat = stream.ReadUInt32().FromEndian(EndianUtils.Endianness.LittleEndian);
				header.DXT10_ResourceDimension = stream.ReadUInt32().FromEndian(EndianUtils.Endianness.LittleEndian);
				header.DXT10_MiscFlag = stream.ReadUInt32().FromEndian(EndianUtils.Endianness.LittleEndian);
				header.DXT10_ArraySize = stream.ReadUInt32().FromEndian(EndianUtils.Endianness.LittleEndian);
				header.DXT10_MiscFlags2 = stream.ReadUInt32().FromEndian(EndianUtils.Endianness.LittleEndian);
			}

			return header;
		}

		public static DDSHeader Generate( uint width, uint height, uint mipmaps, TextureFormat format ) {
			if ( !IsDDSTextureFormat( format ) ) { throw new Exception( "Texture format must be compatible with the DDS file format!" ); }

			DDSHeader header = new DDSHeader();
			header.Flags = DDSFlags.DDSD_CAPS | DDSFlags.DDSD_HEIGHT | DDSFlags.DDSD_WIDTH | DDSFlags.DDSD_PIXELFORMAT;
			header.Width = width;
			header.Height = height;
			if ( mipmaps > 1 ) {
				header.Flags |= DDSFlags.DDSD_MIPMAPCOUNT;
				header.MipMapCount = mipmaps;
			}
			header.PixelFormat.Flags = DDSFlags.DDPF_FOURCC;
			header.PixelFormat.FourCC = ToDDSFourCC( format ).Value;

			if (header.PixelFormat.FourCC == 0x30315844) {
				switch (format) {
					case TextureFormat.BC5_UNORM:
						header.DXT10_DxgiFormat = 83;
						break;
					case TextureFormat.BC7_UNORM:
						header.DXT10_DxgiFormat = 98;
						break;
					default:
						throw new Exception("Unknown format");
				}
				header.DXT10_ResourceDimension = 3;
				header.DXT10_MiscFlag = 0;
				header.DXT10_ArraySize = 1;
				header.DXT10_MiscFlags2 = 0;
			}

			return header;
		}

		public byte[] ToBytes() {
			MemoryStream ms = new MemoryStream((PixelFormat.FourCC == 0x30315844) ? 0x94 : 0x80);
			ms.WriteUInt32(Magic);
			ms.WriteUInt32(Size);
			ms.WriteUInt32(Flags);
			ms.WriteUInt32(Height);
			ms.WriteUInt32(Width);
			ms.WriteUInt32(PitchOrLinearSize);
			ms.WriteUInt32(Depth);
			ms.WriteUInt32(MipMapCount);
			ms.WriteUInt32(Reserved1a);
			ms.WriteUInt32(Reserved1b);
			ms.WriteUInt32(Reserved1c);
			ms.WriteUInt32(Reserved1d);
			ms.WriteUInt32(Reserved1e);
			ms.WriteUInt32(Reserved1f);
			ms.WriteUInt32(Reserved1g);
			ms.WriteUInt32(Reserved1h);
			ms.WriteUInt32(Reserved1i);
			ms.WriteUInt32(Reserved1j);
			ms.WriteUInt32(Reserved1k);
			ms.WriteUInt32(PixelFormat.Size);
			ms.WriteUInt32(PixelFormat.Flags);
			ms.WriteUInt32(PixelFormat.FourCC);
			ms.WriteUInt32(PixelFormat.RGBBitCount);
			ms.WriteUInt32(PixelFormat.RBitMask);
			ms.WriteUInt32(PixelFormat.GBitMask);
			ms.WriteUInt32(PixelFormat.BBitMask);
			ms.WriteUInt32(PixelFormat.ABitMask);
			ms.WriteUInt32(Caps);
			ms.WriteUInt32(Caps2);
			ms.WriteUInt32(Caps3);
			ms.WriteUInt32(Caps4);
			ms.WriteUInt32(Reserved2);

			if (PixelFormat.FourCC == 0x30315844) {
				ms.WriteUInt32(DXT10_DxgiFormat);
				ms.WriteUInt32(DXT10_ResourceDimension);
				ms.WriteUInt32(DXT10_MiscFlag);
				ms.WriteUInt32(DXT10_ArraySize);
				ms.WriteUInt32(DXT10_MiscFlags2);
			}

			return ms.ToArray();
		}

		public static bool IsDDSTextureFormat( TextureFormat format ) {
			return ToDDSFourCC( format ) != null;
		}
		public static uint? ToDDSFourCC( TextureFormat format ) {
			switch ( format ) {
				case TextureFormat.DXT1a:
				case TextureFormat.DXT1c:
					return 0x31545844;
				case TextureFormat.DXT3:
					return 0x33545844;
				case TextureFormat.DXT5:
					return 0x35545844;
				case TextureFormat.BC5_UNORM:
				case TextureFormat.BC7_UNORM:
					return 0x30315844;
				default:
					return null;
			}
		}
	}
	public class DDSPixelFormat {
		public uint Size = 0x20;
		public uint Flags;
		public uint FourCC;
		public uint RGBBitCount;
		public uint RBitMask;
		public uint GBitMask;
		public uint BBitMask;
		public uint ABitMask;

		public static DDSPixelFormat FromStream( Stream stream ) {
			DDSPixelFormat format = new DDSPixelFormat();
			format.Size = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			if ( format.Size != 0x20 ) {
				throw new Exception( "Invalid size of pixel format." );
			}
			format.Flags = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			format.FourCC = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			format.RGBBitCount = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			format.RBitMask = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			format.GBitMask = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			format.BBitMask = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			format.ABitMask = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
			return format;
		}
	};
	public static class DDSFlags {
		public static uint DDSD_CAPS = 0x1;
		public static uint DDSD_HEIGHT = 0x2;
		public static uint DDSD_WIDTH = 0x4;
		public static uint DDSD_PITCH = 0x8;
		public static uint DDSD_PIXELFORMAT = 0x1000;
		public static uint DDSD_MIPMAPCOUNT = 0x20000;
		public static uint DDSD_LINEARSIZE = 0x80000;
		public static uint DDSD_DEPTH = 0x800000;

		public static uint DDPF_ALPHAPIXELS = 0x1;
		public static uint DDPF_ALPHA = 0x2;
		public static uint DDPF_FOURCC = 0x4;
		public static uint DDPF_RGB = 0x40;
		public static uint DDPF_YUV = 0x200;
		public static uint DDPF_LUMINANCE = 0x20000;

		public static uint DDSCAPS_COMPLEX = 0x8;
		public static uint DDSCAPS_MIPMAP = 0x400000;
		public static uint DDSCAPS_TEXTURE = 0x1000;
	}

	public class DDS {
		public DDSHeader Header;
		public Stream Data;

		public DDS( DDSHeader header, Stream imageData ) {
			Header = header;
			Data = new MemoryStream( (int)imageData.Length );
			StreamUtils.CopyStream( imageData, Data, imageData.Length );
		}

		public DDS( Stream ddsFile ) {
			if ( ddsFile.Length < 0x80 ) {
				throw new Exception( "Invalid size for DDS file." );
			}
			Header = DDSHeader.FromStream( ddsFile );
			Data = new MemoryStream( (int)ddsFile.Length - 0x80 );
			StreamUtils.CopyStream( ddsFile, Data, ddsFile.Length - 0x80 );
		}

		public Bitmap ConvertToBitmap() {
			var po = new PixelOrderIterators.TiledPixelOrderIterator( (int)Header.Width, (int)Header.Height, 4, 4 );
			ColorFetchingIterators.IColorFetchingIterator cf;
			Data.Position = 0;
			switch ( Header.PixelFormat.FourCC ) {
				case 0x31545844: cf = new ColorFetchingIterators.ColorFetcherDXT( Data, Header.Width, Header.Height, ColorFetchingIterators.DxtFormat.COMPRESSED_RGBA_S3TC_DXT1_EXT ); break;
				case 0x33545844: cf = new ColorFetchingIterators.ColorFetcherDXT( Data, Header.Width, Header.Height, ColorFetchingIterators.DxtFormat.COMPRESSED_RGBA_S3TC_DXT3_EXT ); break;
				case 0x35545844: cf = new ColorFetchingIterators.ColorFetcherDXT( Data, Header.Width, Header.Height, ColorFetchingIterators.DxtFormat.COMPRESSED_RGBA_S3TC_DXT5_EXT ); break;
				default: throw new Exception( "Not implemented." );
			}

			return cf.ConvertToBitmap( po, Header.Width, Header.Height );
		}

		public byte[] ToBytes() {
			long pos = Data.Position;
			try {
				Data.Position = 0;
				byte[] header = Header.ToBytes();
				byte[] result = new byte[header.Length + Data.Length];
				ArrayUtils.CopyByteArrayPart(header, 0, result, 0, header.Length);
				if (Data.Read(result, header.Length, (int)Data.Length) != Data.Length) {
					throw new Exception("internal read error");
				}
				return result;
			} finally {
				Data.Position = pos;
			}
		}
	}
}
