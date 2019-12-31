using HyoutaTools.Textures;
using HyoutaTools.Textures.ColorFetchingIterators;
using HyoutaTools.Textures.PixelOrderIterators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.Texture {
	public class TXV {
		public TXV( TXM txm, String filename, bool vesperiaPcTextureFormat ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( txm, stream, vesperiaPcTextureFormat ) ) {
					throw new Exception( "Loading TXV failed!" );
				}
			}
		}

		public TXV( TXM txm, Stream stream, bool vesperiaPcTextureFormat ) {
			if ( !LoadFile( txm, stream, vesperiaPcTextureFormat ) ) {
				throw new Exception( "Loading TXV failed!" );
			}
		}

		public List<TXVSingle> textures;

		private bool LoadFile( TXM txm, Stream stream, bool vesperiaPcTextureFormat ) {
			textures = new List<TXVSingle>();

			foreach ( TXMSingle ts in txm.TXMSingles ) {
				try {
					textures.Add( new TXVSingle( stream, ts, vesperiaPcTextureFormat ) );
				} catch ( Exception ex ) {
					Console.WriteLine( "Error loading " + ts.ToString() + ": " + ex.ToString() );
				}
			}

			return true;
		}
	}

	public class TXVSingle {
		public TXMSingle TXM;
		public Stream Data;
		public bool VesperiaPC;

		public TXVSingle( Stream stream, TXMSingle txm, bool vesperiaPcTextureFormat ) {
			uint bytecount = vesperiaPcTextureFormat ? stream.ReadUInt32().FromEndian( EndianUtils.Endianness.BigEndian ) : txm.GetByteCount();
			TXM = txm;
			Data = new MemoryStream( (int)bytecount );
			VesperiaPC = vesperiaPcTextureFormat;

			if ( !vesperiaPcTextureFormat ) {
				stream.Position = txm.TxvLocation;
			}
			StreamUtils.CopyStream( stream, Data, bytecount );
			Data.Position = 0;
		}

		private Stream DecodeGrayscale8ToPng( Stream data ) {
			var bitmap = new System.Drawing.Bitmap( (int)TXM.Width, (int)TXM.Height );

			for ( uint y = 0; y < TXM.Height; ++y ) {
				for ( uint x = 0; x < TXM.Width; ++x ) {
					int b = data.ReadByte();
					bitmap.SetPixel( (int)x, (int)y, Color.FromArgb( 255, b, b, b ) );
				}
			}

			MemoryStream s = new MemoryStream();
			bitmap.Save( s, System.Drawing.Imaging.ImageFormat.Png );
			return s;
		}

		public List<Bitmap> GetBitmaps() {
			List<Bitmap> list = new List<Bitmap>();

			for ( uint depth = 0; depth < TXM.Depth; ++depth ) {
				Data.Position = 0;
				Stream plane = TXM.GetSinglePlane( Data, depth );
				switch ( TXM.Format ) {
					case TextureFormat.DXT1a:
					case TextureFormat.DXT1b:
					case TextureFormat.DXT5a:
					case TextureFormat.DXT5b: {
							plane.Position = 0;
							DDSHeader header = DDSHeader.Generate( TXM.Width, TXM.Height, TXM.Mipmaps, ( TXM.Format == TextureFormat.DXT1a || TXM.Format == TextureFormat.DXT1b ) ? Textures.TextureFormat.DXT1a : Textures.TextureFormat.DXT5 );
							list.Add( new DDS( header, plane ).ConvertToBitmap() );
						}
						break;
					default:
						for ( uint mip = 0; mip < TXM.Mipmaps; ++mip ) {
							(uint width, uint height, IPixelOrderIterator pxit, IColorFetchingIterator colit) = GenerateIterators(plane, mip);
							var bmp = colit.ConvertToBitmap( pxit, width, height );
							list.Add( bmp );
						}
						break;
				}
			}

			return list;
		}

		private (uint width, uint height, IPixelOrderIterator pxit, IColorFetchingIterator colit) GenerateIterators(Stream plane, uint mip) {
			switch (TXM.Format) {
				case TextureFormat.ARGBa:
				case TextureFormat.ARGBb:
				case TextureFormat.Indexed4Bits_Grey8Alpha8:
				case TextureFormat.Indexed4Bits_RGB565:
				case TextureFormat.Indexed4Bits_RGB5A3:
				case TextureFormat.Indexed8Bits_Grey8Alpha8:
				case TextureFormat.Indexed8Bits_RGB565:
				case TextureFormat.Indexed8Bits_RGB5A3:
					// we can decode all of these
					break;
				default:
					throw new Exception("Unhandled texture format " + TXM.Format);
			}

			var dims = TXM.GetDimensions((int)mip);
			IPixelOrderIterator pxit;
			if (TXM.Format == TextureFormat.ARGBa) {
				pxit = new ARGBaPixelOrderIterator((int)dims.width, (int)dims.height);
			} else if (TXM.Format == TextureFormat.Indexed4Bits_Grey8Alpha8 || TXM.Format == TextureFormat.Indexed4Bits_RGB565 || TXM.Format == TextureFormat.Indexed4Bits_RGB5A3) {
				pxit = new TiledPixelOrderIterator((int)dims.width, (int)dims.height, 8, 8);
			} else if (TXM.Format == TextureFormat.Indexed8Bits_Grey8Alpha8 || TXM.Format == TextureFormat.Indexed8Bits_RGB565 || TXM.Format == TextureFormat.Indexed8Bits_RGB5A3) {
				pxit = new TiledPixelOrderIterator((int)dims.width, (int)dims.height, 8, 4);
			} else {
				pxit = new LinearPixelOrderIterator((int)dims.width, (int)dims.height);
			}
			IColorFetchingIterator colit;
			if (TXM.Format == TextureFormat.Indexed4Bits_Grey8Alpha8 || TXM.Format == TextureFormat.Indexed4Bits_RGB565 || TXM.Format == TextureFormat.Indexed4Bits_RGB5A3 || TXM.Format == TextureFormat.Indexed8Bits_Grey8Alpha8 || TXM.Format == TextureFormat.Indexed8Bits_RGB565 || TXM.Format == TextureFormat.Indexed8Bits_RGB5A3) {
				Stream ms = new MemoryStream();
				long tmp = plane.Position;
				plane.Position = plane.Length - TXM.GetExtraPaletteBytes();
				StreamUtils.CopyStream(plane, ms, TXM.GetExtraPaletteBytes());
				plane.Position = tmp;
				ms.Position = 0;
				IColorFetchingIterator colors;
				if (TXM.Format == TextureFormat.Indexed4Bits_Grey8Alpha8) {
					colors = new ColorFetcherGrey8Alpha8(ms, 16, 1, EndianUtils.Endianness.BigEndian);
					colit = new ColorFetcherIndexed4Bits(plane, NumberUtils.Align((long)dims.width, 8), NumberUtils.Align((long)dims.height, 8), colors);
				} else if (TXM.Format == TextureFormat.Indexed4Bits_RGB565) {
					colors = new ColorFetcherRGB565(ms, 16, 1, EndianUtils.Endianness.BigEndian);
					colit = new ColorFetcherIndexed4Bits(plane, NumberUtils.Align((long)dims.width, 8), NumberUtils.Align((long)dims.height, 8), colors);
				} else if (TXM.Format == TextureFormat.Indexed4Bits_RGB5A3) {
					colors = new ColorFetcherRGB5A3(ms, 16, 1, EndianUtils.Endianness.BigEndian);
					colit = new ColorFetcherIndexed4Bits(plane, NumberUtils.Align((long)dims.width, 8), NumberUtils.Align((long)dims.height, 8), colors);
				} else if (TXM.Format == TextureFormat.Indexed8Bits_Grey8Alpha8) {
					colors = new ColorFetcherGrey8Alpha8(ms, 256, 1, EndianUtils.Endianness.BigEndian);
					colit = new ColorFetcherIndexed8Bits(plane, NumberUtils.Align((long)dims.width, 8), NumberUtils.Align((long)dims.height, 4), colors);
				} else if (TXM.Format == TextureFormat.Indexed8Bits_RGB565) {
					colors = new ColorFetcherRGB565(ms, 256, 1, EndianUtils.Endianness.BigEndian);
					colit = new ColorFetcherIndexed8Bits(plane, NumberUtils.Align((long)dims.width, 8), NumberUtils.Align((long)dims.height, 4), colors);
				} else if (TXM.Format == TextureFormat.Indexed8Bits_RGB5A3) {
					colors = new ColorFetcherRGB5A3(ms, 256, 1, EndianUtils.Endianness.BigEndian);
					colit = new ColorFetcherIndexed8Bits(plane, NumberUtils.Align((long)dims.width, 8), NumberUtils.Align((long)dims.height, 4), colors);
				} else {
					throw new Exception("Internal error.");
				}
			} else {
				colit = new ColorFetcherARGB8888(plane, dims.width, dims.height);
			}

			return (dims.width, dims.height, pxit, colit);
		}

		public List<(string name, Stream data)> GetDiskWritableStreams() {
			List<(string name, Stream data)> list = new List<(string name, Stream data)>();

			if ( VesperiaPC ) {
				Stream output = new MemoryStream( (int)Data.Length );
				StreamUtils.CopyStream( Data, output, Data.Length );
				list.Add( (TXM.Name + ".dds", output) );
				return list;
			}

			for ( uint depth = 0; depth < TXM.Depth; ++depth ) {
				Data.Position = 0;
				Stream plane = TXM.GetSinglePlane( Data, depth );
				switch ( TXM.Format ) {
					case TextureFormat.DXT1a:
					case TextureFormat.DXT1b:
					case TextureFormat.DXT5a:
					case TextureFormat.DXT5b: {
							plane.Position = 0;
							Stream stream = new MemoryStream( (int)( plane.Length + 0x80 ) );
							stream.Write( Textures.DDSHeader.Generate( TXM.Width, TXM.Height, TXM.Mipmaps, ( TXM.Format == TextureFormat.DXT1a || TXM.Format == TextureFormat.DXT1b ) ? Textures.TextureFormat.DXT1a : Textures.TextureFormat.DXT5 ).ToBytes() );
							StreamUtils.CopyStream( plane, stream, plane.Length );
							string name = TXM.Name + ( TXM.Depth > 1 ? ( "_Plane" + depth ) : "" ) + ".dds";
							list.Add( (name, stream) );
						}
						break;
					default:
						for ( uint mip = 0; mip < TXM.Mipmaps; ++mip ) {
							(uint width, uint height, IPixelOrderIterator pxit, IColorFetchingIterator colit) = GenerateIterators(plane, mip);
							Stream stream = colit.WriteSingleImageToPngStream( pxit, width, height );
							string name = TXM.Name + ( TXM.Depth > 1 ? ( "_Plane" + depth ) : "" ) + ( TXM.Mipmaps > 1 ? ( "_Mip" + mip ) : "" ) + ".png";
							list.Add( (name, stream) );
						}
						break;
				}
			}

			return list;
		}
	}
}
