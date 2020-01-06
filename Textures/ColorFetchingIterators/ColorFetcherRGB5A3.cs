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
	public class ColorFetcherRGB5A3 : IColorFetchingIterator {
		private Stream SourceStream;
		private long PixelCount;
		private EndianUtils.Endianness Endian;

		public ColorFetcherRGB5A3( Stream stream, long width, long height, EndianUtils.Endianness endian ) {
			SourceStream = stream;
			PixelCount = width * height;
			Endian = endian;
		}

		public IEnumerator<Color> GetEnumerator() {
			for ( long i = 0; i < PixelCount; ++i ) {
				yield return ColorFromRGB5A3( SourceStream.ReadUInt16().FromEndian( Endian ) );
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public static Color ColorFromRGB5A3( ushort color ) {
			bool is555 = ( color & 0x8000 ) > 0;
			if ( is555 ) {
				int b = (int)( ( ( color & 0x001F )       ) << 3 );
				int g = (int)( ( ( color & 0x03E0 ) >>  5 ) << 3 );
				int r = (int)( ( ( color & 0x7C00 ) >> 10 ) << 3 );
				return Color.FromArgb( r, g, b );
			} else {
				int b = (int)( ( ( color & 0x000F )       ) << 4 );
				int g = (int)( ( ( color & 0x00F0 ) >> 4  ) << 4 );
				int r = (int)( ( ( color & 0x0F00 ) >> 8  ) << 4 );
				int a = (int)( ( ( color & 0x7000 ) >> 12 ) << 5 );
				return Color.FromArgb( a, r, g, b );
			}
		}

		public static ushort ColorToRGB5A3(Color color) {
			bool is555 = color.A == 255;
			if (is555) {
				int b = (color.B >> 3) & 0x001F;
				int g = (color.G >> 3) & 0x001F;
				int r = (color.R >> 3) & 0x001F;
				return (ushort)(b | (g << 5) | (r << 10) | 0x8000);
			} else {
				int b = (color.B >> 4) & 0x000F;
				int g = (color.G >> 4) & 0x000F;
				int r = (color.R >> 4) & 0x000F;
				int a = (color.A >> 5) & 0x0007;
				return (ushort)(b | (g << 4) | (r << 8) | (a << 12));
			}
		}
	}
}
