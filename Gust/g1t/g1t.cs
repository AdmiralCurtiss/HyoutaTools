using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaUtils;

namespace HyoutaTools.Gust.g1t {
	public class g1tTexture {
		public byte[] Data;
		public uint Width;
		public uint Height;
		public byte Mipmaps;
		public Textures.TextureFormat Format;

		uint BitPerPixel;

		public g1tTexture( Stream stream, EndianUtils.Endianness endian ) {
			Mipmaps = (byte)stream.ReadByte();
			byte format = (byte)stream.ReadByte();
			byte dimensions = (byte)stream.ReadByte();
			byte unknown4 = (byte)stream.ReadByte();
			byte unknown5 = (byte)stream.ReadByte();
			byte unknown6 = (byte)stream.ReadByte();
			byte unknown7 = (byte)stream.ReadByte();
			byte unknown8 = (byte)stream.ReadByte();

			if ( endian == EndianUtils.Endianness.LittleEndian ) {
				Mipmaps = Mipmaps.SwapEndian4Bits();
				dimensions = dimensions.SwapEndian4Bits();
				unknown4 = unknown4.SwapEndian4Bits();
				unknown5 = unknown5.SwapEndian4Bits();
				unknown6 = unknown6.SwapEndian4Bits();
				unknown7 = unknown7.SwapEndian4Bits();
				unknown8 = unknown8.SwapEndian4Bits();
			}

			if ( unknown8 == 0x01 ) {
				stream.DiscardBytes( 0x0C );
			}

			switch ( format ) {
				case 0x00: Format = Textures.TextureFormat.ABGR; BitPerPixel = 32; break;
				case 0x01: Format = Textures.TextureFormat.RGBA; BitPerPixel = 32; break;
				case 0x06: Format = Textures.TextureFormat.DXT1a; BitPerPixel = 4; break;
				case 0x08: Format = Textures.TextureFormat.DXT5; BitPerPixel = 8; break;
				case 0x12: Format = Textures.TextureFormat.DXT5; BitPerPixel = 8; break; // swizzled from vita
				case 0x5B: Format = Textures.TextureFormat.DXT5; BitPerPixel = 8; break; // unsure what the difference to 0x08 is
				default: throw new Exception( String.Format( "g1t: Unknown Format ({0:X2})", format ) );
			}

			Width = (uint)( 1 << ( dimensions >> 4 ) );
			Height = (uint)( 1 << ( dimensions & 0x0F ) );

			uint highestMipmapSize = ( Width * Height * BitPerPixel ) / 8;
			long textureSize = highestMipmapSize;
			for ( int i = 0; i < Mipmaps - 1; ++i ) {
				textureSize += highestMipmapSize / ( 4 << ( i * 2 ) );
			}

			Data = new byte[textureSize];
			stream.Read( Data, 0, Data.Length );
		}

		public long GetDataStart( int mipmapLevel ) {
			if ( mipmapLevel == 0 ) { return 0; }

			uint highestMipmapSize = ( Width * Height * BitPerPixel ) / 8;
			long textureSize = highestMipmapSize;
			for ( int i = 0; i < mipmapLevel - 1; ++i ) {
				textureSize += highestMipmapSize / ( 4 << ( i * 2 ) );
			}
			return textureSize;
		}

		public long GetDataLength( int mipmapLevel ) {
			uint highestMipmapSize = ( Width * Height * BitPerPixel ) / 8;
			return highestMipmapSize / ( 1 << ( mipmapLevel * 2 ) );
		}
	}

	public class g1t {
		public g1t( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading g1t failed!" );
				}
			}
		}

		public g1t( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading g1t failed!" );
			}
		}

		public List<g1tTexture> Textures;
		public EndianUtils.Endianness Endian = EndianUtils.Endianness.BigEndian;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 4 );
			switch ( magic ) {
				case "G1TG": Endian = EndianUtils.Endianness.BigEndian; break;
				case "GT1G": Endian = EndianUtils.Endianness.LittleEndian; break;
				default: throw new Exception( "Not a g1t file!" );
			}

			uint version = stream.ReadUInt32().FromEndian( Endian );

			switch ( version ) {
				case 0x30303530: break;
				case 0x30303630: break;
				default: throw new Exception( "Unsupported g1t version!" );
			}

			uint fileSize = stream.ReadUInt32().FromEndian( Endian );
			uint headerSize = stream.ReadUInt32().FromEndian( Endian );
			uint numberTextures = stream.ReadUInt32().FromEndian( Endian );
			uint unknown = stream.ReadUInt32().FromEndian( Endian );

			stream.Position = headerSize;
			uint bytesUnknownData = stream.ReadUInt32().FromEndian( Endian );
			stream.Position = headerSize + bytesUnknownData;
			Textures = new List<g1tTexture>( (int)numberTextures );
			for ( uint i = 0; i < numberTextures; ++i ) {
				var g = new g1tTexture( stream, Endian );
				Textures.Add( g );
			}

			return true;
		}
	}
}
