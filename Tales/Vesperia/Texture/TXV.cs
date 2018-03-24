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

		public List<(string name, Stream data)> Decode() {
			List<(string name, Stream data)> list = new List<(string name, Stream data)>();

			Stream stream;
			string name;
			switch ( TXM.Format ) {
				case TextureFormat.DXT1a:
				case TextureFormat.DXT1b:
				case TextureFormat.DXT5a:
				case TextureFormat.DXT5b:
					Data.Position = 0;
					stream = new MemoryStream( (int)( Data.Length + 0x80 ) );
					stream.Write( Textures.DDSHeader.Generate( TXM.Width, TXM.Height, TXM.Mipmaps, ( TXM.Format == TextureFormat.DXT1a || TXM.Format == TextureFormat.DXT1b ) ? Textures.TextureFormat.DXT1 : Textures.TextureFormat.DXT5 ) );
					Util.CopyStream( Data, stream, Data.Length );
					name = TXM.Name + ".dds";
					break;
				default:
					throw new Exception( "Unhandled texture format " + TXM.Format );
			}
			list.Add( (name, stream) );

			return list;
		}
	}
}
