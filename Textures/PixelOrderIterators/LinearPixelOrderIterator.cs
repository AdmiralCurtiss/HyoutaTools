using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Textures.PixelOrderIterators {
	public class LinearPixelOrderIterator : IPixelOrderIterator {
		int Width;
		int Height;

		public LinearPixelOrderIterator( int width, int height ) {
			Width = width;
			Height = height;
		}

		public IEnumerator<(int X, int Y)> GetEnumerator() {
			for ( int y = 0; y < Height; ++y ) {
				for ( int x = 0; x < Width; ++x ) {
					yield return ( x, y );
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
