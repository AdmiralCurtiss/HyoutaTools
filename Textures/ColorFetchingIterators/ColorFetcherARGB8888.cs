using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.Textures.ColorFetchingIterators {
	public class ColorFetcherARGB8888 : IColorFetchingIterator {
		private Stream SourceStream;
		private long PixelCount;

		public ColorFetcherARGB8888( Stream stream, long width, long height ) {
			SourceStream = stream;
			PixelCount = width * height;
		}

		public IEnumerator<Color> GetEnumerator() {
			for ( long i = 0; i < PixelCount; ++i ) {
				yield return ColorFromARGB8888( SourceStream.ReadUInt32() );
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public static Color ColorFromARGB8888( uint color ) {
			int a = (int)( ( color & 0x000000FF )       );
			int r = (int)( ( color & 0x0000FF00 ) >>  8 );
			int g = (int)( ( color & 0x00FF0000 ) >> 16 );
			int b = (int)( ( color & 0xFF000000 ) >> 24 );
			return Color.FromArgb( a, r, g, b );
		}
	}
}
