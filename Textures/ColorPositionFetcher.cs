using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Textures {
	public class ColorPositionFetcher : IEnumerable<(System.Drawing.Color Color, int X, int Y)> {
		ColorFetchingIterators.IColorFetchingIterator ColorFetchingIterator;
		PixelOrderIterators.IPixelOrderIterator PixelOrderIterator;

		public ColorPositionFetcher( ColorFetchingIterators.IColorFetchingIterator colorFetchingIterator, PixelOrderIterators.IPixelOrderIterator pixelOrderIterator ) {
			ColorFetchingIterator = colorFetchingIterator;
			PixelOrderIterator = pixelOrderIterator;
		}

		public IEnumerator<(Color Color, int X, int Y)> GetEnumerator() {
			var c = ColorFetchingIterator.GetEnumerator();
			var p = PixelOrderIterator.GetEnumerator();

			while ( c.MoveNext() && p.MoveNext() ) {
				var col = c.Current;
				var xy = p.Current;
				yield return (col, xy.X, xy.Y);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
