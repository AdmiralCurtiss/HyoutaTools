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
	public class ColorFetcherIndexed4Bits : IColorFetchingIterator {
		private Stream SourceStream;
		private long PixelCount;
		private bool Flipped;
		private Color[] colors;

		public ColorFetcherIndexed4Bits( Stream stream, long width, long height, IColorFetchingIterator colorConverter, bool flipped = false ) {
			SourceStream = stream;
			PixelCount = width * height;
			Flipped = flipped;
			colors = colorConverter.ToArray();
		}

		public IEnumerator<Color> GetEnumerator() {
			for ( long i = 0; i < PixelCount / 2; ++i ) {
				byte b = SourceStream.ReadUInt8();
				if (Flipped) {
					yield return colors[b & 0x0F];
					yield return colors[(b >> 4) & 0x0F];
				} else {
					yield return colors[(b >> 4) & 0x0F];
					yield return colors[b & 0x0F];
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
