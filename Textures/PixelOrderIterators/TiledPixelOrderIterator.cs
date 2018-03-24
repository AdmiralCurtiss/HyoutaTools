using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Textures.PixelOrderIterators {
	public class TiledPixelOrderIterator : IPixelOrderIterator {
		int Width;
		int Height;

		int CurrentTileX;
		int CurrentTileY;
		int CounterInTile;

		int TileWidth;
		int TileHeight;

		public TiledPixelOrderIterator( int width, int height, int tileWidth, int tileHeight ) {
			Width = width;
			Height = height;
			CurrentTileX = 0;
			CurrentTileY = 0;
			CounterInTile = 0;
			TileWidth = tileWidth;
			TileHeight = tileHeight;
		}

		public int X { get { return CurrentTileX + ( CounterInTile % TileWidth ); } }
		public int Y { get { return CurrentTileY + ( CounterInTile / TileWidth ); } }

		public void Next() {
			++CounterInTile;

			if ( CounterInTile == TileWidth * TileHeight ) {
				CounterInTile = 0;
				CurrentTileX += TileWidth;
				if ( CurrentTileX >= Width ) {
					CurrentTileX = 0;
					CurrentTileY += TileHeight;
				}
			}
		}
	}
}
