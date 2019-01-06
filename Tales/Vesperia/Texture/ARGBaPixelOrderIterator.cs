using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Textures.PixelOrderIterators {
	public class ARGBaPixelOrderIterator : IPixelOrderIterator {
		int Width;
		int Height;

		public ARGBaPixelOrderIterator( int width, int height ) {
			Width = width;
			Height = height;
		}

		private static IEnumerable<(int x, int y)> GenerateBlockOrder( int px, int py, int blocksX, int blocksY ) {
			int blockOffset = Math.Max( 1, Math.Min( blocksY / 2, blocksX / 2 ) );
			for ( int y = 0; y < blocksY; y += blockOffset ) {
				for ( int x = 0; x < blocksX; x += blockOffset ) {
					if ( blockOffset == 1 ) {
						yield return (px + x, py + y);
					} else {
						foreach ( var v in GenerateBlockOrder( px + x, py + y, blockOffset, blockOffset ) ) {
							yield return v;
						}
					}
				}
			}
		}

		private static IEnumerable<(int X, int Y)> GeneratePixelOrder( int width, int height ) {
			int blocksX = ( width + 1 ) / 2;
			int blocksY = ( height + 1 ) / 2;
			if ( blocksX > blocksY ) {
				Console.WriteLine( "WARNING: Proper pixel order not known for this image, result may look scrambled." );
			}
			foreach ( var block in GenerateBlockOrder( 0, 0, blocksX, blocksY ) ) {
				yield return (block.x * 2 + 0, block.y * 2 + 0);
				yield return (block.x * 2 + 1, block.y * 2 + 0);
				yield return (block.x * 2 + 0, block.y * 2 + 1);
				yield return (block.x * 2 + 1, block.y * 2 + 1);
			}
		}

		public IEnumerator<(int X, int Y)> GetEnumerator() {
			foreach ( var px in GeneratePixelOrder( Width, Height ) ) {
				if ( px.X < Width && px.Y < Height ) {
					yield return px;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
