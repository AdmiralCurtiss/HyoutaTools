using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Textures {
	public enum TextureFormat {
		ABGR,
		RGBA,
		DXT1c, // no alpha
		DXT1a, // with 1 bit alpha
		DXT3,
		DXT5,
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

			return header;
		}

		public byte[] ToBytes() {
			byte[] data = new byte[0x80];

			DDSHeader header = this;
			BitConverter.GetBytes( header.Magic ).CopyTo( data, 0x00 );
			BitConverter.GetBytes( header.Size ).CopyTo( data, 0x04 );
			BitConverter.GetBytes( header.Flags ).CopyTo( data, 0x08 );
			BitConverter.GetBytes( header.Height ).CopyTo( data, 0x0C );
			BitConverter.GetBytes( header.Width ).CopyTo( data, 0x10 );
			BitConverter.GetBytes( header.PitchOrLinearSize ).CopyTo( data, 0x14 );
			BitConverter.GetBytes( header.Depth ).CopyTo( data, 0x18 );
			BitConverter.GetBytes( header.MipMapCount ).CopyTo( data, 0x1C );

			BitConverter.GetBytes( header.PixelFormat.Size ).CopyTo( data, 0x4C );
			BitConverter.GetBytes( header.PixelFormat.Flags ).CopyTo( data, 0x50 );
			BitConverter.GetBytes( header.PixelFormat.FourCC ).CopyTo( data, 0x54 );
			BitConverter.GetBytes( header.PixelFormat.RGBBitCount ).CopyTo( data, 0x58 );
			BitConverter.GetBytes( header.PixelFormat.RBitMask ).CopyTo( data, 0x5C );
			BitConverter.GetBytes( header.PixelFormat.GBitMask ).CopyTo( data, 0x60 );
			BitConverter.GetBytes( header.PixelFormat.BBitMask ).CopyTo( data, 0x64 );
			BitConverter.GetBytes( header.PixelFormat.ABitMask ).CopyTo( data, 0x68 );

			BitConverter.GetBytes( header.Caps ).CopyTo( data, 0x6C );
			BitConverter.GetBytes( header.Caps2 ).CopyTo( data, 0x70 );

			return data;
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
