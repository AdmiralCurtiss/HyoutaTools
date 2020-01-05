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
	public class ColorFetcherARGB8888Gamecube : IColorFetchingIterator {
		private Stream SourceStream;
		private long PixelCount;

		public ColorFetcherARGB8888Gamecube(Stream stream, long width, long height) {
			SourceStream = stream;
			PixelCount = width * height;
		}

		public IEnumerator<Color> GetEnumerator() {
			// see YAGCD 15.20.1, format 6
			byte[] data = new byte[0x40];
			for (long i = 0; i < PixelCount; i += 16) {
				SourceStream.Read(data, 0, 0x40);
				for (long j = 0; j < 0x10; ++j) {
					byte a = data[j * 2 + 0];
					byte r = data[j * 2 + 1];
					byte g = data[j * 2 + 0 + 0x20];
					byte b = data[j * 2 + 1 + 0x20];
					yield return Color.FromArgb(a, r, g, b);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
