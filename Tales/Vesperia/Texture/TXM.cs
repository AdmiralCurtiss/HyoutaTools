using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public uint Unknown1;
		public uint HeaderSize;
		public uint Unknown3;
		public uint RegularCount;
		public uint CubemapCount;
		public uint VolumeCount;

		public List<TXMSingleRegular> TXMRegulars;
		public List<TXMSingleCubemap> TXMCubemaps;
		public List<TXMSingleVolume> TXMVolumes;

		private bool LoadFile( Stream stream ) {
			Unknown1 = stream.ReadUInt32().SwapEndian();
			HeaderSize = stream.ReadUInt32().SwapEndian();
			Unknown3 = stream.ReadUInt32().SwapEndian();
			RegularCount = stream.ReadUInt32().SwapEndian();
			CubemapCount = stream.ReadUInt32().SwapEndian();
			VolumeCount = stream.ReadUInt32().SwapEndian();

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
		public uint Mipmaps;
		public TextureFormat Format;
		public uint NameOffset;
		public uint TxvLocation;
		public uint Unknown;
		public string Name;

		public uint GetBitPerPixel() {
			switch ( Format ) {
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
			throw new Exception( "Unknown format." );
		}

		public uint GetByteCount() {
			uint mip0 = GetBitPerPixel() * Width * Height * Depth;
			uint sum = 0;
			uint mipdiv = 1;
			for ( uint i = 0; i < Mipmaps; ++i ) {
				sum += mip0 / mipdiv;
				mipdiv *= 4;
			}
			return sum / 8;
		}

		public (uint width, uint height) GetDimensions( int mipmaplevel ) {
			uint w = Math.Max( 1u, Width >> mipmaplevel );
			uint h = Math.Max( 1u, Height >> mipmaplevel );
			// depth too? how do mips interact with volume textures?
			return (w, h);
		}
	}

	public class TXMSingleRegular : TXMSingle {
		public TXMSingleRegular( Stream stream ) {
			Width = stream.ReadUInt32().SwapEndian();
			Height = stream.ReadUInt32().SwapEndian();
			Depth = 1;
			Mipmaps = stream.ReadUInt32().SwapEndian();
			Format = (TextureFormat)stream.ReadUInt32().SwapEndian();
			NameOffset = stream.ReadUInt32().SwapEndian();
			Name = stream.ReadAsciiNulltermFromLocationAndReset( NameOffset + stream.Position - 4 );
			TxvLocation = stream.ReadUInt32().SwapEndian();
			Unknown = stream.ReadUInt32().SwapEndian();
		}
	}

	public class TXMSingleCubemap : TXMSingle {
		public TXMSingleCubemap( Stream stream ) {
			Width = stream.ReadUInt32().SwapEndian();
			Height = Width;
			Depth = 6;
			Mipmaps = stream.ReadUInt32().SwapEndian();
			Format = (TextureFormat)stream.ReadUInt32().SwapEndian();
			NameOffset = stream.ReadUInt32().SwapEndian();
			Name = stream.ReadAsciiNulltermFromLocationAndReset( NameOffset + stream.Position - 4 );
			TxvLocation = stream.ReadUInt32().SwapEndian();
			Unknown = stream.ReadUInt32().SwapEndian();
		}
	}

	public class TXMSingleVolume : TXMSingle {
		public TXMSingleVolume( Stream stream ) {
			Width = stream.ReadUInt32().SwapEndian();
			Height = stream.ReadUInt32().SwapEndian();
			Depth = stream.ReadUInt32().SwapEndian();
			Mipmaps = stream.ReadUInt32().SwapEndian();
			Format = (TextureFormat)stream.ReadUInt32().SwapEndian();
			NameOffset = stream.ReadUInt32().SwapEndian();
			Name = stream.ReadAsciiNulltermFromLocationAndReset( NameOffset + stream.Position - 4 );
			TxvLocation = stream.ReadUInt32().SwapEndian();
			Unknown = stream.ReadUInt32().SwapEndian();
		}
	}
}
