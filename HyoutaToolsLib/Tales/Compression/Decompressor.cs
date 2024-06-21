using HyoutaPluginBase;
using HyoutaUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Compression {
	internal class Decompressor : HyoutaPluginBase.IDecompressor {
		public CanDecompressAnswer CanDecompress(DuplicatableStream stream) {
			long pos = stream.Position;
			try {
				int b0 = stream.ReadByte();
				if (b0 == 0x00 || b0 == 0x01 || b0 == 0x03 || b0 == 0x81 || b0 == 0x83) {
					uint compressed = stream.ReadUInt32();
					uint uncompressed = stream.ReadUInt32();

					if (b0 == 0) {
						return (compressed == uncompressed && compressed + 9 == stream.Length) ? CanDecompressAnswer.Yes : CanDecompressAnswer.No;
					} else {
						return (compressed + 9 == stream.Length) ? CanDecompressAnswer.Yes : CanDecompressAnswer.No;
					}
				}

				return CanDecompressAnswer.No;
			} finally {
				stream.Position = pos;
			}
		}

		// returns negative on failure, or number of bytes written to output on success
		public static long DecompressToStream(DuplicatableStream input, Stream output) {
			int b0 = input.ReadByte();
			uint compressed = input.ReadUInt32();
			uint uncompressed = input.ReadUInt32();
			long result = -1;
			if (b0 == 0x00 && compressed == uncompressed) {
				StreamUtils.CopyStream(input, output, compressed);
				result = compressed;
			} else if (b0 == 0x01) {
				result = Decompression.decompress_01(input, compressed, output, uncompressed);
			} else if (b0 == 0x03) {
				result = Decompression.decompress_03(input, compressed, output, uncompressed);
			} else if (b0 == 0x81) {
				result = Decompression.decompress_81(input, compressed, output, uncompressed);
			} else if (b0 == 0x83) {
				result = Decompression.decompress_83(input, compressed, output, uncompressed);
			}
			return result;
		}

		public DuplicatableStream Decompress(DuplicatableStream input) {
			using (MemoryStream ms = new MemoryStream()) {
				input.Position = 0;
				long result = DecompressToStream(input, ms);
				if (result < 0 || result > ms.Length) {
					return null;
				}

				ms.Position = 0;
				byte[] data = new byte[(int)result];
				ms.Read(data, 0, (int)result);
				return new HyoutaUtils.Streams.DuplicatableByteArrayStream(data);
			}
		}

		public string GetId() {
			return "compto";
		}
	}
}
