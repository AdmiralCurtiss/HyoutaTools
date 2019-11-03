using HyoutaTools.Textures.ColorFetchingIterators;
using HyoutaTools.Textures.PixelOrderIterators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Textures {
	public static class TextureUtil {
		public static Bitmap ConvertToBitmap( this IColorFetchingIterator colorFetchingIterator, IPixelOrderIterator pixelOrderIterator, uint width, uint height ) {
			var bitmap = new System.Drawing.Bitmap( (int)width, (int)height );
			foreach ( var cxy in new ColorPositionFetcher( colorFetchingIterator, pixelOrderIterator ) ) {
				if ( cxy.X < width && cxy.Y < height ) {
					bitmap.SetPixel( cxy.X, cxy.Y, cxy.Color );
				}
			}
			return bitmap;
		}

		public static Stream WriteSingleImageToPngStream( this IColorFetchingIterator colorFetchingIterator, IPixelOrderIterator pixelOrderIterator, uint width, uint height ) {
			MemoryStream s = new MemoryStream();
			colorFetchingIterator.ConvertToBitmap( pixelOrderIterator, width, height ).Save( s, System.Drawing.Imaging.ImageFormat.Png );
			return s;
		}
	}
}
