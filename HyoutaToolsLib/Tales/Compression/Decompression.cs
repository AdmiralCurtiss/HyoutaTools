using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Compression {
	public static class Decompression {
		// ported from https://github.com/AdmiralCurtiss/topdec

		private static void InitializeDictionary(byte[] dict) {
			int offset = 0;
			for (int i = 0; i < 0x100; ++i) {
				dict[offset++] = (byte)(i);
				dict[offset++] = (byte)(0);
				dict[offset++] = (byte)(i);
				dict[offset++] = (byte)(0);
				dict[offset++] = (byte)(i);
				dict[offset++] = (byte)(0);
				dict[offset++] = (byte)(i);
				dict[offset++] = (byte)(0);
			}
			for (int i = 0; i < 0x100; ++i) {
				dict[offset++] = (byte)(i);
				dict[offset++] = (byte)(0xff);
				dict[offset++] = (byte)(i);
				dict[offset++] = (byte)(0xff);
				dict[offset++] = (byte)(i);
				dict[offset++] = (byte)(0xff);
				dict[offset++] = (byte)(i);
			}
			for (int i = 0; i < 0x100; ++i) {
				dict[offset++] = 0;
			}
		}

		public static int decompress_reserve_extra_bytes() {
			return 273;
		}

		private static long decompress_internal(bool HasDict,
												bool HasMultiByte,
												System.IO.Stream compressed,
												uint compressedLength,
												System.IO.Stream uncompressed,
												uint uncompressedLength) {
			byte[] dict = new byte[0x1000];
			ulong inpos = 0;
			ulong outpos = 0;
			uint dictpos = 0;

			if (HasDict) {
				InitializeDictionary(dict);
				dictpos = HasMultiByte ? 0xfefu : 0xfeeu;
			}

			int literalBits = 0;
			while (true) {
				if (outpos >= uncompressedLength) {
					return (long)outpos;
				}
				if (inpos >= compressedLength) {
					return -1;
				}

				int isLiteralByte = (literalBits & 1);
				literalBits = (literalBits >> 1);
				if (literalBits == 0) {
					literalBits = (byte)(compressed.ReadByte());
					++inpos;
					isLiteralByte = (literalBits & 1);
					literalBits = (0x80 | (literalBits >> 1));
				}
				if (isLiteralByte != 0) {
					byte c = (byte)(compressed.ReadByte());
					uncompressed.WriteByte(c);
					dict[dictpos] = c;
					dictpos = (dictpos + 1u) & 0xfffu;
					++inpos;
					++outpos;
					continue;
				}

				if ((inpos + 1) >= compressedLength) {
					return -1;
				}

				byte bnext = (byte)(compressed.ReadByte());
				byte b = (byte)(compressed.ReadByte());
				byte blow = (byte)(b & 0xf);
				byte bhigh = (byte)((b & 0xf0) >> 4);
				byte nibble1 = HasDict ? blow : bhigh;
				byte nibble2 = HasDict ? bhigh : blow;
				if (HasMultiByte && (nibble1 == 0xf)) {
					// multiple copies of the same byte

					if (nibble2 == 0) {
						if ((inpos + 2) >= compressedLength) {
							return -1;
						}

						// 19 to 274 bytes
						ulong count = (ulong)((byte)(bnext)) + 19;
						byte c = (byte)(compressed.ReadByte());
						for (ulong i = 0; i < count; ++i) {
							uncompressed.WriteByte(c);
							dict[dictpos] = c;
							dictpos = (dictpos + 1u) & 0xfffu;
							++outpos;
						}
						inpos += 3;
					} else {
						// 4 to 18 bytes
						ulong count = (ulong)(nibble2) + 3;
						byte c = bnext;
						for (ulong i = 0; i < count; ++i) {
							uncompressed.WriteByte(c);
							dict[dictpos] = c;
							dictpos = (dictpos + 1u) & 0xfffu;
							++outpos;
						}
						inpos += 2;
					}
				} else {
					ushort offset = (ushort)((ushort)((byte)(bnext)) | ((ushort)(nibble2) << 8));
					ulong count = (ushort)(nibble1) + 3u;

					if (HasDict) {
						// reference into dictionary
						for (ulong i = 0; i < count; ++i) {
							byte c = dict[(offset + i) & 0xfffu];
							uncompressed.WriteByte(c);
							dict[dictpos] = c;
							dictpos = (dictpos + 1u) & 0xfffu;
							++outpos;
						}
					} else {
						// backref into decompressed data
						if (offset == 0) {
							// the game just reads the unwritten output buffer and copies it over itself inpos
							// this case... while I suppose one *could* use this behavior inpos a really
							// creative way by pre-initializing the output buffer to something known, I
							// doubt it actually does that. so consider this a corrupted data stream.
							return -1;
						}
						if (outpos < offset) {
							// backref to before start of uncompressed data. this is invalid.
							return -1;
						}

						for (ulong i = 0; i < count; ++i) {
							byte c = dict[(outpos - offset) & 0xfffu];
							uncompressed.WriteByte(c);
							dict[dictpos] = c;
							dictpos = (dictpos + 1u) & 0xfffu;
							++outpos;
						}
					}

					inpos += 2;
				}
			}
		}

		public static long decompress_81(System.IO.Stream compressed, uint compressedLength, System.IO.Stream uncompressed, uint uncompressedLength) {
			return decompress_internal(false, false, compressed, compressedLength, uncompressed, uncompressedLength);
		}

		public static long decompress_83(System.IO.Stream compressed, uint compressedLength, System.IO.Stream uncompressed, uint uncompressedLength) {
			return decompress_internal(false, true, compressed, compressedLength, uncompressed, uncompressedLength);
		}

		public static long decompress_01(System.IO.Stream compressed, uint compressedLength, System.IO.Stream uncompressed, uint uncompressedLength) {
			return decompress_internal(true, false, compressed, compressedLength, uncompressed, uncompressedLength);
		}

		public static long decompress_03(System.IO.Stream compressed, uint compressedLength, System.IO.Stream uncompressed, uint uncompressedLength) {
			return decompress_internal(true, true, compressed, compressedLength, uncompressed, uncompressedLength);
		}
	}
}
