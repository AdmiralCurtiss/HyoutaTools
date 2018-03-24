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
		public static Stream WriteSingleImageToPngStream( this IColorFetchingIterator colorFetchingIterator, IPixelOrderIterator pixelOrderIterator, uint width, uint height ) {
			var bitmap = new System.Drawing.Bitmap( (int)width, (int)height );
			foreach ( Color c in colorFetchingIterator ) {
				bitmap.SetPixel( pixelOrderIterator.X, pixelOrderIterator.Y, c );
				pixelOrderIterator.Next();
			}

			MemoryStream s = new MemoryStream();
			bitmap.Save( s, System.Drawing.Imaging.ImageFormat.Png );
			return s;
		}
	}
}
