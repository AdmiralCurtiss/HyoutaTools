using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Gust.g1t {
	public class g1tTexture {
		public byte[] Data;
		public uint Width;
		public uint Height;
		public uint Mipmaps;
		public Textures.DDSFormat Format;

		public g1tTexture( Stream stream ) {
			Mipmaps = (byte)stream.ReadByte();
			byte format = (byte)stream.ReadByte();
			byte dimensions = (byte)stream.ReadByte();
			byte unknown4 = (byte)stream.ReadByte();
			uint bitPerPixel = 8;

			stream.DiscardBytes( 0x10 );

			switch ( format ) {
				case 0x06: Format = Textures.DDSFormat.DXT1; bitPerPixel = 4; break;
				case 0x08: Format = Textures.DDSFormat.DXT5; bitPerPixel = 8; break;
				default: throw new Exception( "g1t: Unknown Format" );
			}

			Width = (uint)( 1 << ( dimensions >> 4 ) );
			Height = (uint)( 1 << ( dimensions & 0x0F ) );

			Data = new byte[( Width * Height * bitPerPixel ) / 8];
			stream.Read( Data, 0, Data.Length );
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

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );

			uint fileSize = stream.ReadUInt32().SwapEndian();
			uint headerSize = stream.ReadUInt32().SwapEndian();
			uint numberTextures = stream.ReadUInt32().SwapEndian();
			uint unknown = stream.ReadUInt32().SwapEndian();

			stream.Position = headerSize;
			uint bytesUnknownData = stream.ReadUInt32().SwapEndian();
			stream.Position = headerSize + bytesUnknownData;
			Textures = new List<g1tTexture>( (int)numberTextures );
			for ( uint i = 0; i < numberTextures; ++i ) {
				var g = new g1tTexture( stream );
				Textures.Add( g );
			}

			return true;
		}
	}
}
