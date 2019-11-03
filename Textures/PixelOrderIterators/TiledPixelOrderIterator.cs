using HyoutaUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Textures.PixelOrderIterators {
	public class TiledPixelOrderIterator : IPixelOrderIterator {
		int Width;
		int Height;
		int TileWidth;
		int TileHeight;

		public TiledPixelOrderIterator( int width, int height, int tileWidth, int tileHeight ) {
			Width = NumberUtils.Align( width, tileWidth );
			Height = NumberUtils.Align( height, tileHeight );
			TileWidth = tileWidth;
			TileHeight = tileHeight;
		}

		public IEnumerator<(int X, int Y)> GetEnumerator() {
			int tileX = 0;
			int tileY = 0;
			int counterInTile = -1;

			for ( int y = 0; y < Height; ++y ) {
				for ( int x = 0; x < Width; ++x ) {
					++counterInTile;
					if ( counterInTile == TileWidth * TileHeight ) {
						counterInTile = 0;
						tileX += TileWidth;
						if ( tileX >= Width ) {
							tileX = 0;
							tileY += TileHeight;
						}
					}

					yield return (tileX + ( counterInTile % TileWidth ), tileY + ( counterInTile / TileWidth ));
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
