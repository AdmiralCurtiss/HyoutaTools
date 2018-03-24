using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Textures.PixelOrderIterators {
	public class LinearPixelOrderIterator : IPixelOrderIterator {
		int Width;
		int Height;
		int Counter;

		public LinearPixelOrderIterator( int width, int height ) {
			Width = width;
			Height = height;
			Counter = 0;
		}

		public int X { get { return Counter % Width; } }
		public int Y { get { return Counter / Width; } }

		public void Next() {
			++Counter;
		}
	}
}
