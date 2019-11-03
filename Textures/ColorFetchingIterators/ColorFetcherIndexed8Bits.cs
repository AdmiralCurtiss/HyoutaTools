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
	public class ColorFetcherIndexed8Bits : IColorFetchingIterator {
		private Stream SourceStream;
		private long PixelCount;
		private Color[] colors;

		public ColorFetcherIndexed8Bits( Stream stream, long width, long height, IColorFetchingIterator colorConverter ) {
			SourceStream = stream;
			PixelCount = width * height;
			colors = colorConverter.ToArray();
		}

		public IEnumerator<Color> GetEnumerator() {
			for ( long i = 0; i < PixelCount; ++i ) {
				byte b = SourceStream.ReadUInt8();
				yield return colors[b];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
