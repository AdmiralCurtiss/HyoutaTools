using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaUtils;

namespace HyoutaTools.Gust.g1t {
	class DDSConverter {
		public static int Execute( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: infile.g1t" );
				return -1;
			}

			string infile = args[0];
			string outdir = Path.GetDirectoryName( infile );
			string filename = Path.GetFileName( infile );

			var g1t = new g1t( infile );

			for ( int i = 0; i < g1t.Textures.Count; ++i ) {
				var tex = g1t.Textures[i];

				if ( Textures.DDSHeader.IsDDSTextureFormat( tex.Format ) ) {
					string path = System.IO.Path.Combine( outdir, filename + "_" + i.ToString( "D4" ) + ".dds" );
					using ( var fs = new System.IO.FileStream( path, System.IO.FileMode.Create ) ) {
						fs.Write( Textures.DDSHeader.Generate( tex.Width, tex.Height, tex.Mipmaps, tex.Format ).ToBytes() );
						fs.Write( tex.Data );
						fs.Close();
					}
				} else {
					for ( int mipmapLevel = 0; mipmapLevel < tex.Mipmaps; ++mipmapLevel ) {
						string path = System.IO.Path.Combine( outdir, filename + "_" + i.ToString( "D4" ) + ( mipmapLevel > 0 ? "_Mipmap" + mipmapLevel : "" ) + ".png" );
						int width = (int)tex.Width / ( 1 << mipmapLevel );
						int height = (int)tex.Height / ( 1 << mipmapLevel );

						var bitmap = new System.Drawing.Bitmap( width, height );
						long offset = tex.GetDataStart( mipmapLevel );
						for ( int j = 0; j < tex.GetDataLength( mipmapLevel ) / 4; ++j ) {
							long idx = offset + j * 4;
							System.Drawing.Color color;
							switch ( tex.Format ) {
								case Textures.TextureFormat.RGBA:
									color = System.Drawing.Color.FromArgb( tex.Data[idx + 3], tex.Data[idx + 2], tex.Data[idx + 1], tex.Data[idx] );
									break;
								case Textures.TextureFormat.ABGR:
									color = System.Drawing.Color.FromArgb( tex.Data[idx], tex.Data[idx + 3], tex.Data[idx + 2], tex.Data[idx + 1] );
									break;
								default:
									throw new Exception( "Unsupported texture format in png generation color loop." );
							}
							bitmap.SetPixel( (int)( j % width ), (int)( j / width ), color );
						}
						bitmap.Save( path, System.Drawing.Imaging.ImageFormat.Png );
					}
				}
			}

			return 0;
		}
	}
}
