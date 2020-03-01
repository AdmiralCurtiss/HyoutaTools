using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;
using NumberUtils = HyoutaUtils.NumberUtils;

namespace HyoutaTools.Tales.Vesperia.Texture {
	public class TXM {
		public TXM( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading TXM failed!" );
				}
			}
		}

		public TXM( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading TXM failed!" );
			}
		}

		public TXM() {
			Unknown1 = 0;
			HeaderSize = 0;
			Unknown3 = 0;
			TXMRegulars = new List<TXMSingleRegular>();
			TXMCubemaps = new List<TXMSingleCubemap>();
			TXMVolumes = new List<TXMSingleVolume>();
		}

		public uint Unknown1;
		public uint HeaderSize;
		public uint Unknown3;

		public List<TXMSingleRegular> TXMRegulars;
		public List<TXMSingleCubemap> TXMCubemaps;
		public List<TXMSingleVolume> TXMVolumes;

		private bool LoadFile( Stream stream ) {
			Unknown1 = stream.ReadUInt32().SwapEndian();
			HeaderSize = stream.ReadUInt32().SwapEndian();
			Unknown3 = stream.ReadUInt32().SwapEndian();
			uint RegularCount = stream.ReadUInt32().SwapEndian();
			uint CubemapCount = stream.ReadUInt32().SwapEndian();
			uint VolumeCount = stream.ReadUInt32().SwapEndian();

			TXMRegulars = new List<TXMSingleRegular>();
			for ( uint i = 0; i < RegularCount; ++i ) {
				TXMRegulars.Add( new TXMSingleRegular( stream ) );
			}

			TXMCubemaps = new List<TXMSingleCubemap>();
			for ( uint i = 0; i < CubemapCount; ++i ) {
				TXMCubemaps.Add( new TXMSingleCubemap( stream ) );
			}

			TXMVolumes = new List<TXMSingleVolume>();
			for ( uint i = 0; i < VolumeCount; ++i ) {
				TXMVolumes.Add( new TXMSingleVolume( stream ) );
			}

			return true;
		}

		public IEnumerable<TXMSingle> TXMSingles => TXMRegulars.Concat<TXMSingle>( TXMCubemaps ).Concat( TXMVolumes );
	}

	public enum TextureFormat : uint {
		RGB565 = 0x00000004,
		GamecubeRGBA8 = 0x00000006,
		Indexed4Bits_Grey8Alpha8 = 0x00010008,
		Indexed4Bits_RGB565 = 0x10010008,
		Indexed4Bits_RGB5A3 = 0x20010008,
		Indexed8Bits_Grey8Alpha8 = 0x00010009,
		Indexed8Bits_RGB565 = 0x10010009,
		Indexed8Bits_RGB5A3 = 0x20010009,
		// no idea what the difference between those CMP formats is
		GamecubeCMP = 0x0000000E,
		GamecubeCMP2 = 0x0002000E,
		GamecubeCMP4 = 0x0004000E,
		GamecubeCMPA = 0x000A000E,
		GamecubeCMPC = 0x000C000E,
		Unknown0x8101AAE4 = 0x8101AAE4u,
		DXT1a = 0x8602AAE4u,
		DXT1b = 0xA602AAE4u,
		Unknown0x8302AAE4 = 0x8302AAE4u,
		ARGBa = 0x8504AAE4u,
		ARGBb = 0xA504AAE4u,
		DXT5a = 0x8804AAE4u,
		DXT5b = 0xA804AAE4u,
	}

	public abstract class TXMSingle {
		public uint Width;
		public uint Height;
		public uint Depth;
		public uint _Mipmaps;
		public uint Mipmaps { get => Math.Max( 1, _Mipmaps ); }
		public TextureFormat Format;
		public uint NameOffset;
		public uint TxvLocation;
		public uint Unknown;
		public string Name;

		public uint GetBitPerPixel() {
			switch ( Format ) {
				case TextureFormat.RGB565:
					return 16;
				case TextureFormat.GamecubeRGBA8:
					return 32;
				case TextureFormat.GamecubeCMP:
				case TextureFormat.GamecubeCMP2:
				case TextureFormat.GamecubeCMP4:
				case TextureFormat.GamecubeCMPA:
				case TextureFormat.GamecubeCMPC:
					return 4;
				case TextureFormat.Indexed4Bits_Grey8Alpha8:
				case TextureFormat.Indexed4Bits_RGB565:
				case TextureFormat.Indexed4Bits_RGB5A3:
					return 4;
				case TextureFormat.Indexed8Bits_Grey8Alpha8:
				case TextureFormat.Indexed8Bits_RGB565:
				case TextureFormat.Indexed8Bits_RGB5A3:
					return 8;
				case TextureFormat.Unknown0x8101AAE4:
					return 8;
				case TextureFormat.DXT1a:
				case TextureFormat.DXT1b:
					return 4;
				case TextureFormat.Unknown0x8302AAE4:
					return 16;
				case TextureFormat.ARGBa:
				case TextureFormat.ARGBb:
					return 32;
				case TextureFormat.DXT5a:
				case TextureFormat.DXT5b:
					return 8;
			}
			throw new Exception( "Unknown format: 0x" + ( (uint)Format ).ToString( "X8" ) );
		}

		public uint GetExtraPaletteBytes() {
			switch ( Format ) {
				case TextureFormat.Indexed4Bits_Grey8Alpha8:
				case TextureFormat.Indexed4Bits_RGB565:
				case TextureFormat.Indexed4Bits_RGB5A3:
					return 16 * 2;
				case TextureFormat.Indexed8Bits_Grey8Alpha8:
				case TextureFormat.Indexed8Bits_RGB565:
				case TextureFormat.Indexed8Bits_RGB5A3:
					return 256 * 2;
				default:
					return 0;
			}
		}

		public uint GetByteCount() {
			uint bpp = GetBitPerPixel();
			uint sum = 0;

			uint w = NumberUtils.Align( Width, GetWidthAlignment() );
			uint h = NumberUtils.Align( Height, GetHeightAlignment() );
			uint d = Depth;
			for ( uint i = 0; i < Mipmaps; ++i ) {
				uint mip = bpp * w * h * d;
				sum += mip;
				w = Math.Max( MinimumWidthPerMip(), w / 2 );
				h = Math.Max( MinimumHeightPerMip(), h / 2 );
				d = Math.Max( MinimumDepthPerMip(), d / 2 );
			}
			return sum / 8 + GetExtraPaletteBytes();
		}

		private uint MinimumWidthPerMip() {
			if ( Format == TextureFormat.ARGBa ) {
				return 2;
			} else {
				return 1;
			}
		}

		private uint MinimumHeightPerMip() {
			if ( Format == TextureFormat.ARGBa ) {
				return 2;
			} else {
				return 1;
			}
		}

		private uint MinimumDepthPerMip() {
			return 1;
		}

		private uint GetWidthAlignment() {
			switch ( Format ) {
				case TextureFormat.Indexed4Bits_Grey8Alpha8:
				case TextureFormat.Indexed4Bits_RGB565:
				case TextureFormat.Indexed4Bits_RGB5A3:
				case TextureFormat.Indexed8Bits_Grey8Alpha8:
				case TextureFormat.Indexed8Bits_RGB565:
				case TextureFormat.Indexed8Bits_RGB5A3:
				case TextureFormat.GamecubeCMP:
				case TextureFormat.GamecubeCMP2:
				case TextureFormat.GamecubeCMP4:
				case TextureFormat.GamecubeCMPA:
				case TextureFormat.GamecubeCMPC:
					return 8;
				case TextureFormat.RGB565:
				case TextureFormat.GamecubeRGBA8:
					return 4;
				default:
					return 1;
			}
		}

		private uint GetHeightAlignment() {
			switch ( Format ) {
				case TextureFormat.Indexed4Bits_Grey8Alpha8:
				case TextureFormat.Indexed4Bits_RGB565:
				case TextureFormat.Indexed4Bits_RGB5A3:
				case TextureFormat.GamecubeCMP:
				case TextureFormat.GamecubeCMP2:
				case TextureFormat.GamecubeCMP4:
				case TextureFormat.GamecubeCMPA:
				case TextureFormat.GamecubeCMPC:
					return 8;
				case TextureFormat.Indexed8Bits_Grey8Alpha8:
				case TextureFormat.Indexed8Bits_RGB565:
				case TextureFormat.Indexed8Bits_RGB5A3:
				case TextureFormat.RGB565:
				case TextureFormat.GamecubeRGBA8:
					return 4;
				default:
					return 1;
			}
		}

		public uint GetByteCount( int mipmaplevel ) {
			return GetByteCountSingleDepthPlane( mipmaplevel ) * Depth;
		}

		public uint GetByteCountSingleDepthPlane( int mipmaplevel ) {
			uint w = NumberUtils.Align( Width, GetWidthAlignment() );
			uint h = NumberUtils.Align( Height, GetHeightAlignment() );
			return ( ( GetBitPerPixel() * w * h ) / ( 1u << ( mipmaplevel * 2 ) ) ) / 8;
		}

		public (uint width, uint height) GetDimensions( int mipmaplevel ) {
			uint w = Math.Max( 1u, Width >> mipmaplevel );
			uint h = Math.Max( 1u, Height >> mipmaplevel );
			// depth too? how do mips interact with volume textures?
			return (w, h);
		}

		public abstract Stream GetSinglePlane( Stream inputStream, uint depthlevel );
	}

	public class TXMSingleRegular : TXMSingle {
		public TXMSingleRegular( Stream stream ) {
			Width = stream.ReadUInt32().SwapEndian();
			Height = stream.ReadUInt32().SwapEndian();
			Depth = 1;
			_Mipmaps = stream.ReadUInt32().SwapEndian();
			Format = (TextureFormat)stream.ReadUInt32().SwapEndian();
			NameOffset = stream.ReadUInt32().SwapEndian();
			Name = stream.ReadAsciiNulltermFromLocationAndReset( NameOffset + stream.Position - 4 );
			TxvLocation = stream.ReadUInt32().SwapEndian();
			Unknown = stream.ReadUInt32().SwapEndian();
		}

		public override Stream GetSinglePlane( Stream inputStream, uint depthlevel ) {
			return inputStream;
		}

		public override string ToString() {
			return String.Format( "{4}: {0}x{1}, {2} mips, {3}, @0x{5:X8}", Width, Height, Mipmaps, Format.ToString(), Name, TxvLocation );
		}
	}

	public class TXMSingleCubemap : TXMSingle {
		public TXMSingleCubemap( Stream stream ) {
			Width = stream.ReadUInt32().SwapEndian();
			Height = Width;
			Depth = 6;
			_Mipmaps = stream.ReadUInt32().SwapEndian();
			Format = (TextureFormat)stream.ReadUInt32().SwapEndian();
			NameOffset = stream.ReadUInt32().SwapEndian();
			Name = stream.ReadAsciiNulltermFromLocationAndReset( NameOffset + stream.Position - 4 );
			TxvLocation = stream.ReadUInt32().SwapEndian();
			Unknown = stream.ReadUInt32().SwapEndian();
		}

		public override Stream GetSinglePlane( Stream inputStream, uint depthlevel ) {
			return GetSinglePlane_MipBeforeDepth( inputStream, depthlevel );
		}

		public Stream GetSinglePlane_MipBeforeDepth( Stream inputStream, uint depthlevel ) {
			Stream s = new MemoryStream();
			uint planesBeforeCurrent = depthlevel;
			uint planesAfterCurrent = Depth - depthlevel - 1;
			uint sum = 0u;
			for ( int i = 0; i < Mipmaps; ++i ) {
				uint bytecount = GetByteCountSingleDepthPlane( i );
				sum += bytecount;
			}
			if ( Format == TextureFormat.DXT1a || Format == TextureFormat.DXT1b || Format == TextureFormat.DXT5a || Format == TextureFormat.DXT5b ) {
				// this feels stupid??
				sum = NumberUtils.Align( sum, 0x80u );
			}
			inputStream.DiscardBytes( planesBeforeCurrent * sum );
			StreamUtils.CopyStream( inputStream, s, sum );
			inputStream.DiscardBytes( planesAfterCurrent * sum );
			return s;
		}

		public Stream GetSinglePlane_DepthBeforeMip( Stream inputStream, uint depthlevel ) {
			Stream s = new MemoryStream();
			for ( int i = 0; i < Mipmaps; ++i ) {
				uint bytecount = GetByteCountSingleDepthPlane( i );
				uint planesBeforeCurrent = depthlevel;
				uint planesAfterCurrent = Depth - depthlevel - 1;
				inputStream.DiscardBytes( planesBeforeCurrent * bytecount );
				StreamUtils.CopyStream( inputStream, s, bytecount );
				inputStream.DiscardBytes( planesAfterCurrent * bytecount );
			}
			return s;
		}

		public Stream GetSinglePlaneSingleDepth( Stream inputStream, uint depthlevel, int mipmaplevel ) {
			uint sum = 0u;
			for ( int i = 0; i < mipmaplevel; ++i ) {
				sum += GetByteCountSingleDepthPlane( i ) * Depth;
			}
			uint countRequestedPlane = GetByteCountSingleDepthPlane( mipmaplevel );
			sum += countRequestedPlane * depthlevel;

			Stream s = new MemoryStream( (int)countRequestedPlane );
			StreamUtils.CopyStream( inputStream, s, countRequestedPlane );
			return s;
		}
	}

	public class TXMSingleVolume : TXMSingle {
		public TXMSingleVolume( Stream stream ) {
			Width = stream.ReadUInt32().SwapEndian();
			Height = stream.ReadUInt32().SwapEndian();
			Depth = stream.ReadUInt32().SwapEndian();
			_Mipmaps = stream.ReadUInt32().SwapEndian();
			Format = (TextureFormat)stream.ReadUInt32().SwapEndian();
			NameOffset = stream.ReadUInt32().SwapEndian();
			Name = stream.ReadAsciiNulltermFromLocationAndReset( NameOffset + stream.Position - 4 );
			TxvLocation = stream.ReadUInt32().SwapEndian();
			Unknown = stream.ReadUInt32().SwapEndian();
		}

		public override Stream GetSinglePlane( Stream inputStream, uint depthlevel ) {
			if ( Mipmaps > 1 ) {
				throw new Exception( "Don't know how to deal with volume textures with mipmaps." );
			}
			if ( !( Format == TextureFormat.DXT1a || Format == TextureFormat.DXT1b || Format == TextureFormat.DXT5a || Format == TextureFormat.DXT5b ) ) {
				throw new Exception( "Volume textures only implemented for DXT formats." );
			}

			// Format reference: https://www.khronos.org/registry/OpenGL/extensions/NV/NV_texture_compression_vtc.txt
			uint bytecount = GetByteCountSingleDepthPlane( 0 );
			uint group = depthlevel / 4u;
			uint texInGroupBefore = depthlevel % 4u;
			uint texInGroupAfter = 3u - texInGroupBefore;

			// special handling for last group if depth not divisible by 4
			uint nonDivisiblePlanesAtEnd = Depth % 4;
			uint groupCount = NumberUtils.Align( Depth, 4u ) / 4;
			if ( nonDivisiblePlanesAtEnd != 0 && group == ( groupCount - 1u ) ) {
				texInGroupAfter = ( nonDivisiblePlanesAtEnd - texInGroupBefore ) - 1u;
			}

			uint bytesPerTexLine;
			switch ( Format ) {
				case TextureFormat.DXT1a:
				case TextureFormat.DXT1b:
					bytesPerTexLine = 8;
					break;
				case TextureFormat.DXT5a:
				case TextureFormat.DXT5b:
					bytesPerTexLine = 16;
					break;
				default:
					throw new Exception( "Unexpected format during volume texture plane fetching: " + Format + "." );
			}

			// skip to correct group
			inputStream.DiscardBytes( group * bytecount * 4u );

			// copy texture
			MemoryStream s = new MemoryStream( (int)bytecount );
			for ( uint i = 0; i < bytecount; i += bytesPerTexLine ) {
				inputStream.DiscardBytes( texInGroupBefore * bytesPerTexLine );
				StreamUtils.CopyStream( inputStream, s, bytesPerTexLine );
				inputStream.DiscardBytes( texInGroupAfter * bytesPerTexLine );
			}
			return s;
		}
	}
}
