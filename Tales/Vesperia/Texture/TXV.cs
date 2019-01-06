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

namespace HyoutaTools.Tales.Vesperia.Texture {
	public class TXV {
		public TXV( TXM txm, String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( txm, stream ) ) {
					throw new Exception( "Loading TXV failed!" );
				}
			}
		}

		public TXV( TXM txm, Stream stream ) {
			if ( !LoadFile( txm, stream ) ) {
				throw new Exception( "Loading TXV failed!" );
			}
		}

		public List<TXVSingle> textures;

		private bool LoadFile( TXM txm, Stream stream ) {
			textures = new List<TXVSingle>();

			foreach ( TXMSingle ts in txm.TXMSingles ) {
				textures.Add( new TXVSingle( stream, ts ) );
			}

			return true;
		}
	}

	public class TXVSingle {
		public TXMSingle TXM;
		public Stream Data;

		public TXVSingle( Stream stream, TXMSingle txm ) {
			uint bytecount = txm.GetByteCount();
			TXM = txm;
			Data = new MemoryStream( (int)bytecount );

			stream.Position = txm.TxvLocation;
			Util.CopyStream( stream, Data, bytecount );
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

		// TODO: Duplicated code between GetBitmaps() and GetDiskWritableStreams()..
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
							DDSHeader header = DDSHeader.Generate( TXM.Width, TXM.Height, TXM.Mipmaps, ( TXM.Format == TextureFormat.DXT1a || TXM.Format == TextureFormat.DXT1b ) ? Textures.TextureFormat.DXT1 : Textures.TextureFormat.DXT5 );
							list.Add( new DDS( header, plane ).ConvertToBitmap() );
						}
						break;
					case TextureFormat.ARGBa:
					case TextureFormat.ARGBb:
						for ( uint mip = 0; mip < TXM.Mipmaps; ++mip ) {
							var dims = TXM.GetDimensions( (int)mip );
							IPixelOrderIterator pxit;
							if ( TXM.Format == TextureFormat.ARGBa ) {
								pxit = new ARGBaPixelOrderIterator( (int)dims.width, (int)dims.height );
							} else {
								pxit = new LinearPixelOrderIterator( (int)dims.width, (int)dims.height );
							}
							var bmp = new ColorFetcherARGB8888( plane, dims.width, dims.height ).ConvertToBitmap( pxit, dims.width, dims.height );
							list.Add( bmp );
						}
						break;
					default:
						throw new Exception( "Unhandled texture format " + TXM.Format );
				}
			}

			return list;
		}

		public List<(string name, Stream data)> GetDiskWritableStreams() {
			List<(string name, Stream data)> list = new List<(string name, Stream data)>();

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
							stream.Write( Textures.DDSHeader.Generate( TXM.Width, TXM.Height, TXM.Mipmaps, ( TXM.Format == TextureFormat.DXT1a || TXM.Format == TextureFormat.DXT1b ) ? Textures.TextureFormat.DXT1 : Textures.TextureFormat.DXT5 ).ToBytes() );
							Util.CopyStream( plane, stream, plane.Length );
							string name = TXM.Name + ( TXM.Depth > 1 ? ( "_Plane" + depth ) : "" ) + ".dds";
							list.Add( (name, stream) );
						}
						break;
					case TextureFormat.ARGBa:
					case TextureFormat.ARGBb:
						for ( uint mip = 0; mip < TXM.Mipmaps; ++mip ) {
							var dims = TXM.GetDimensions( (int)mip );
							IPixelOrderIterator pxit;
							if ( TXM.Format == TextureFormat.ARGBa ) {
								pxit = new ARGBaPixelOrderIterator( (int)dims.width, (int)dims.height );
							} else {
								pxit = new LinearPixelOrderIterator( (int)dims.width, (int)dims.height );
							}
							Stream stream = new ColorFetcherARGB8888( plane, dims.width, dims.height ).WriteSingleImageToPngStream( pxit, dims.width, dims.height );
							string name = TXM.Name + ( TXM.Depth > 1 ? ( "_Plane" + depth ) : "" ) + ( TXM.Mipmaps > 1 ? ( "_Mip" + mip ) : "" ) + ".png";
							list.Add( (name, stream) );
						}
						break;
					default:
						throw new Exception( "Unhandled texture format " + TXM.Format );
				}
			}

			return list;
		}
	}
}
