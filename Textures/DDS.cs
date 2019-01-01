using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace HyoutaTools.Textures {
	public enum TextureFormat {
		ABGR,
		RGBA,
		DXT1 = 0x31545844,
		DXT5 = 0x35545844,
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
		public uint[] Reserved1; // 11 elements
		public DDSPixelFormat PixelFormat = new DDSPixelFormat();
		public uint Caps;
		public uint Caps2;
		public uint Caps3;
		public uint Caps4;
		public uint Reserved2;

		public static DDSHeader FromStream( Stream stream ) {
			throw new NotImplementedException(); // TODO
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
			header.PixelFormat.FourCC = format;

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
			BitConverter.GetBytes( (uint)header.PixelFormat.FourCC ).CopyTo( data, 0x54 );
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
			switch ( format ) {
				case TextureFormat.DXT1:
				case TextureFormat.DXT5:
					return true;
				default:
					return false;
			}
		}
	}
	public class DDSPixelFormat {
		public uint Size = 0x20;
		public uint Flags;
		public TextureFormat FourCC;
		public uint RGBBitCount;
		public uint RBitMask;
		public uint GBitMask;
		public uint BBitMask;
		public uint ABitMask;
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
		private DDSHeader Header;
		private Stream Data;

		public DDS( DDSHeader header, Stream imageData ) {
			Header = header;
			Data = new MemoryStream( (int)imageData.Length );
			Util.CopyStream( imageData, Data, imageData.Length );
		}

		public DDS( Stream ddsFile ) {
			if ( ddsFile.Length < 0x80 ) {
				throw new Exception( "Invalid size for DDS file." );
			}
			Header = DDSHeader.FromStream( ddsFile );
			Data = new MemoryStream( (int)ddsFile.Length - 0x80 );
			Util.CopyStream( ddsFile, Data, ddsFile.Length - 0x80 );
		}

		public Bitmap ConvertToBitmap() {
			throw new NotImplementedException(); // TODO
		}
	}
}
