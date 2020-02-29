using HyoutaUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Textures.PixelOrderIterators {
	public class GamecubeCmpPixelOrderIterator : IPixelOrderIterator {
		int Width;
		int Height;

		public GamecubeCmpPixelOrderIterator(int width, int height) {
			Width = NumberUtils.Align(width, 8);
			Height = NumberUtils.Align(height, 8);
		}

		public IEnumerator<(int X, int Y)> GetEnumerator() {
			for (int y = 0; y < Height; y += 8) {
				for (int x = 0; x < Width; x += 8) {
					for (int ty = 0; ty < 8; ty += 4) {
						for (int tx = 0; tx < 8; tx += 4) {
							for (int stx = 3; stx >= 0; --stx) {
								yield return (x + tx + stx, y + ty + 1);
							}
							for (int stx = 3; stx >= 0; --stx) {
								yield return (x + tx + stx, y + ty + 0);
							}
							for (int stx = 3; stx >= 0; --stx) {
								yield return (x + tx + stx, y + ty + 3);
							}
							for (int stx = 3; stx >= 0; --stx) {
								yield return (x + tx + stx, y + ty + 2);
							}
						}
					}
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
