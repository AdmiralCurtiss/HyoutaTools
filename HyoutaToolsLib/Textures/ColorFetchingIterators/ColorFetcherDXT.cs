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
	// see https://www.khronos.org/registry/OpenGL/extensions/EXT/EXT_texture_compression_s3tc.txt
	public enum DxtFormat {
		COMPRESSED_RGB_S3TC_DXT1_EXT,
		COMPRESSED_RGBA_S3TC_DXT1_EXT,
		COMPRESSED_RGBA_S3TC_DXT3_EXT,
		COMPRESSED_RGBA_S3TC_DXT5_EXT,
		GamecubeCMPR, // see YAGCD, 17.14.1; very similar to DXT1 so I've implemented it in here
	}

	public class ColorFetcherDXT : IColorFetchingIterator {
		private Stream SourceStream;
		private long BlockCount;
		private DxtFormat Format;

		public ColorFetcherDXT( Stream stream, long width, long height, DxtFormat format ) {
			SourceStream = stream;
			BlockCount = ( ( width + 3 ) / 4 ) * ( ( height + 3 ) / 4 );
			Format = format;
		}

		public ColorFetcherDXT( Stream stream, long blockCount, DxtFormat format ) {
			SourceStream = stream;
			BlockCount = blockCount;
			Format = format;
		}

		public IEnumerator<Color> GetEnumerator() {
			ulong alpha_dxt3 = 0;
			byte alpha0_dxt5 = 0;
			byte alpha1_dxt5 = 0;
			ulong alphabits_dxt5 = 0;
			for ( long i = 0; i < BlockCount; ++i ) {
				if ( Format == DxtFormat.COMPRESSED_RGBA_S3TC_DXT3_EXT ) {
					byte a0 = SourceStream.ReadUInt8();
					byte a1 = SourceStream.ReadUInt8();
					byte a2 = SourceStream.ReadUInt8();
					byte a3 = SourceStream.ReadUInt8();
					byte a4 = SourceStream.ReadUInt8();
					byte a5 = SourceStream.ReadUInt8();
					byte a6 = SourceStream.ReadUInt8();
					byte a7 = SourceStream.ReadUInt8();
					alpha_dxt3 = a0 + 256u * ( a1 + 256u * ( a2 + 256u * ( a3 + 256u * ( a4 + 256u * ( a5 + 256u * ( a6 + 256u * (ulong)a7 ) ) ) ) ) );
				} else if ( Format == DxtFormat.COMPRESSED_RGBA_S3TC_DXT5_EXT ) {
					alpha0_dxt5 = SourceStream.ReadUInt8();
					alpha1_dxt5 = SourceStream.ReadUInt8();
					byte abits_0 = SourceStream.ReadUInt8();
					byte abits_1 = SourceStream.ReadUInt8();
					byte abits_2 = SourceStream.ReadUInt8();
					byte abits_3 = SourceStream.ReadUInt8();
					byte abits_4 = SourceStream.ReadUInt8();
					byte abits_5 = SourceStream.ReadUInt8();
					alphabits_dxt5 = abits_0 + 256u * ( abits_1 + 256u * ( abits_2 + 256u * ( abits_3 + 256u * ( abits_4 + 256u * (ulong)abits_5 ) ) ) );
				}
				byte c0_lo;
				byte c0_hi;
				byte c1_lo;
				byte c1_hi;
				byte bits_0;
				byte bits_1;
				byte bits_2;
				byte bits_3;
				if (Format == DxtFormat.GamecubeCMPR) {
					c0_hi = SourceStream.ReadUInt8();
					c0_lo = SourceStream.ReadUInt8();
					c1_hi = SourceStream.ReadUInt8();
					c1_lo = SourceStream.ReadUInt8();
					// TODO: is this right?
					bits_1 = SourceStream.ReadUInt8();
					bits_0 = SourceStream.ReadUInt8();
					bits_3 = SourceStream.ReadUInt8();
					bits_2 = SourceStream.ReadUInt8();
				} else {
					c0_lo = SourceStream.ReadUInt8();
					c0_hi = SourceStream.ReadUInt8();
					c1_lo = SourceStream.ReadUInt8();
					c1_hi = SourceStream.ReadUInt8();
					bits_0 = SourceStream.ReadUInt8();
					bits_1 = SourceStream.ReadUInt8();
					bits_2 = SourceStream.ReadUInt8();
					bits_3 = SourceStream.ReadUInt8();
				}
				ushort color0 = (ushort)( c0_lo + c0_hi * 256u );
				ushort color1 = (ushort)( c1_lo + c1_hi * 256u );
				uint bits = bits_0 + 256u * ( bits_1 + 256u * ( bits_2 + 256u * bits_3 ) );

				uint _b0 = ( ( color0 & 0x001Fu )       ) & 0x1F;
				uint _g0 = ( ( color0 & 0x07E0u ) >>  5 ) & 0x3F;
				uint _r0 = ( ( color0 & 0xF800u ) >> 11 ) & 0x1F;
				uint _b1 = ( ( color1 & 0x001Fu )       ) & 0x1F;
				uint _g1 = ( ( color1 & 0x07E0u ) >>  5 ) & 0x3F;
				uint _r1 = ( ( color1 & 0xF800u ) >> 11 ) & 0x1F;

				// placing the high bits into the low bits of the result isn't explicitly said in the OpenGL extension doc,
				// but is suggested in various places on the internet and gets us closer to the reference readdxt output
				// I assume the idea here is that we want to scale better over the entire [0,255] color range
				// instead of arbitrarily clamping at [0,252] or [0,248], which makes sense
				int b0 = (int)( ( _b0 << 3 ) | ( _b0 >> 2 ) );
				int g0 = (int)( ( _g0 << 2 ) | ( _g0 >> 4 ) );
				int r0 = (int)( ( _r0 << 3 ) | ( _r0 >> 2 ) );
				int b1 = (int)( ( _b1 << 3 ) | ( _b1 >> 2 ) );
				int g1 = (int)( ( _g1 << 2 ) | ( _g1 >> 4 ) );
				int r1 = (int)( ( _r1 << 3 ) | ( _r1 >> 2 ) );

				for ( int j = 0; j < 16; ++j ) {
					int code = BitUtils.ExtractBit( bits, j * 2 ) + BitUtils.ExtractBit( bits, j * 2 + 1 ) * 2;

					int a;
					if ( Format == DxtFormat.COMPRESSED_RGBA_S3TC_DXT3_EXT ) {
						int alpha =
							BitUtils.ExtractBit( alpha_dxt3, j * 4 ) +
							BitUtils.ExtractBit( alpha_dxt3, j * 4 + 1 ) * 2 +
							BitUtils.ExtractBit( alpha_dxt3, j * 4 + 2 ) * 4 +
							BitUtils.ExtractBit( alpha_dxt3, j * 4 + 3 ) * 8;

						// same idea as the color bits here, instead of leaving the low bits empty shove the high bits in
						// to interpolate better over the whole range
						a = alpha << 4 | alpha;
					} else if ( Format == DxtFormat.COMPRESSED_RGBA_S3TC_DXT5_EXT ) {
						int acode =
							BitUtils.ExtractBit( alphabits_dxt5, j * 3 ) +
							BitUtils.ExtractBit( alphabits_dxt5, j * 3 + 1 ) * 2 +
							BitUtils.ExtractBit( alphabits_dxt5, j * 3 + 2 ) * 4;

						if ( alpha0_dxt5 > alpha1_dxt5 ) {
							switch ( acode ) {
								case 0: a = alpha0_dxt5; break;
								case 1: a = alpha1_dxt5; break;
								case 2: a = ( 6 * alpha0_dxt5 + 1 * alpha1_dxt5 ) / 7; break;
								case 3: a = ( 5 * alpha0_dxt5 + 2 * alpha1_dxt5 ) / 7; break;
								case 4: a = ( 4 * alpha0_dxt5 + 3 * alpha1_dxt5 ) / 7; break;
								case 5: a = ( 3 * alpha0_dxt5 + 4 * alpha1_dxt5 ) / 7; break;
								case 6: a = ( 2 * alpha0_dxt5 + 5 * alpha1_dxt5 ) / 7; break;
								case 7: a = ( 1 * alpha0_dxt5 + 6 * alpha1_dxt5 ) / 7; break;
								default: throw new Exception( "Invalid alpha code." );
							}
						} else {
							switch ( acode ) {
								case 0: a = alpha0_dxt5; break;
								case 1: a = alpha1_dxt5; break;
								case 2: a = ( 4 * alpha0_dxt5 + 1 * alpha1_dxt5 ) / 5; break;
								case 3: a = ( 3 * alpha0_dxt5 + 2 * alpha1_dxt5 ) / 5; break;
								case 4: a = ( 2 * alpha0_dxt5 + 3 * alpha1_dxt5 ) / 5; break;
								case 5: a = ( 1 * alpha0_dxt5 + 4 * alpha1_dxt5 ) / 5; break;
								case 6: a = 0; break;
								case 7: a = 0xFF; break;
								default: throw new Exception( "Invalid alpha code." );
							}
						}
					} else {
						a = 0xFF;
					}

					if ( Format == DxtFormat.COMPRESSED_RGBA_S3TC_DXT3_EXT || Format == DxtFormat.COMPRESSED_RGBA_S3TC_DXT5_EXT || color0 > color1 ) {
						switch ( code ) {
							case 0: yield return Color.FromArgb( a, r0, g0, b0 ); break;
							case 1: yield return Color.FromArgb( a, r1, g1, b1 ); break;
							case 2: yield return InterpolateThirds( r0, g0, b0, r1, g1, b1, a ); break;
							case 3: yield return InterpolateThirds( r1, g1, b1, r0, g0, b0, a ); break;
							default: throw new Exception( "Invalid code." );
						}
					} else {
						switch ( code ) {
							case 0: yield return Color.FromArgb( a, r0, g0, b0 ); break;
							case 1: yield return Color.FromArgb( a, r1, g1, b1 ); break;
							case 2: yield return InterpolateHalf( r0, g0, b0, r1, g1, b1, a ); break;
							case 3:
								if (Format == DxtFormat.GamecubeCMPR) {
									yield return InterpolateThirds(r1, g1, b1, r0, g0, b0, 0); break;
								} else {
									yield return Color.FromArgb(Format == DxtFormat.COMPRESSED_RGB_S3TC_DXT1_EXT ? 0xFF : 0, 0, 0, 0); break;
								}
							default: throw new Exception( "Invalid code." );
						}
					}
				}
			}
		}

		// color 0 is weighted 2/3, color 1 is weighted 1/3
		public static Color InterpolateThirds( int r0, int g0, int b0, int r1, int g1, int b1, int a ) {
			int r = ( 2 * r0 + r1 ) / 3;
			int g = ( 2 * g0 + g1 ) / 3;
			int b = ( 2 * b0 + b1 ) / 3;
			return Color.FromArgb( a, r, g, b );
		}

		public static Color InterpolateHalf( int r0, int g0, int b0, int r1, int g1, int b1, int a ) {
			int r = ( r0 + r1 ) / 2;
			int g = ( g0 + g1 ) / 2;
			int b = ( b0 + b1 ) / 2;
			return Color.FromArgb( a, r, g, b );
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
