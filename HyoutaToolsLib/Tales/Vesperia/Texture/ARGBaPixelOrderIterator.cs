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

		private static IEnumerable<(int x, int y)> GenerateBlockOrderMacroblocks( int blocksX, int blocksY ) {
			int blockOffset = blocksY / 2;
			for ( int macroblock = 0; macroblock < blocksX / blocksY; ++macroblock ) {
				for ( int y = 0; y < blocksY; y += blockOffset ) {
					for ( int x = 0; x < blocksY; x += blockOffset ) {
						foreach ( var v in GenerateBlockOrder( x + ( macroblock * blocksY ), y, blockOffset, blockOffset ) ) {
							yield return v;
						}
					}
				}
			}
		}

		private static IEnumerable<(int X, int Y)> GeneratePixelOrder( int width, int height ) {
			int blocksX = ( width + 1 ) / 2;
			int blocksY = ( height + 1 ) / 2;
			IEnumerable<(int x, int y)> generator;
			if ( blocksX > blocksY && blocksX > 1 && blocksY > 1 ) { // ???
				Console.WriteLine( "WARNING: Proper pixel order not fully understood for this image, result may look scrambled." );
				generator = GenerateBlockOrderMacroblocks( blocksX, blocksY );
			} else {
				generator = GenerateBlockOrder( 0, 0, blocksX, blocksY );
			}
			foreach ( var block in generator ) {
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
