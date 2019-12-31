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
	public class ColorFetcherGrey8Alpha8 : IColorFetchingIterator {
		private Stream SourceStream;
		private long PixelCount;
		private EndianUtils.Endianness Endian;

		public ColorFetcherGrey8Alpha8( Stream stream, long width, long height, EndianUtils.Endianness endian ) {
			SourceStream = stream;
			PixelCount = width * height;
			Endian = endian;
		}

		public IEnumerator<Color> GetEnumerator() {
			for ( long i = 0; i < PixelCount; ++i ) {
				yield return ColorFromGrey8Alpha8( SourceStream.ReadUInt16().FromEndian( Endian ) );
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public static Color ColorFromGrey8Alpha8( ushort color ) {
			int a = ( color >> 8 ) & 0xFF;
			int g = color & 0xFF;
			return Color.FromArgb( a, g, g, g );
		}

		public static ushort ColorToGrey8Alpha8( Color color ) {
			int a = color.A;
			int g = color.R;
			return (ushort)( ( a << 8 ) | g );
		}
	}
}
