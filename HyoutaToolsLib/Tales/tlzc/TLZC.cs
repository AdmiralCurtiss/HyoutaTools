using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using HyoutaUtils;
using zlib_sharp;
using HyoutaPluginBase;
using HyoutaUtils.Streams;

namespace HyoutaTools.Tales.tlzc {
	public class TLZC {
		public static Stream Decompress(Stream tlzcBuffer, int? compressionType = null, string compressionSubtype = null) {
			tlzcBuffer.Position = 0;
			if (tlzcBuffer.ReadUInt32(EndianUtils.Endianness.BigEndian) != 0x544c5a43) {
				throw new InvalidDataException("buffer does not have TLZC header");
			}
			uint type = tlzcBuffer.ReadUInt32(EndianUtils.Endianness.LittleEndian);
			uint compressedSize = tlzcBuffer.ReadUInt32(EndianUtils.Endianness.LittleEndian);
			uint uncompressedSize = tlzcBuffer.ReadUInt32(EndianUtils.Endianness.LittleEndian);
			uint unknown1 = tlzcBuffer.ReadUInt32(EndianUtils.Endianness.LittleEndian);
			uint unknown2 = tlzcBuffer.ReadUInt32(EndianUtils.Endianness.LittleEndian);
			if (compressedSize != tlzcBuffer.Length) {
				throw new InvalidDataException("buffer length does not match declared buffer length");
			}

			int ctype = compressionType.HasValue ? compressionType.Value : (int)((type >> 8) & 0xff);
			switch (ctype) {
				case 2:
					return new Compression2().Decompress(tlzcBuffer, compressedSize, uncompressedSize, compressionSubtype);
				case 4:
					return new Compression4().Decompress(tlzcBuffer, uncompressedSize);
			}

			throw new InvalidDataException("unknown TLZC compression type");
		}

		public static Stream Compress(Stream data, int compressionType, string compressionSubtype, int numFastBytes = 64) {
			switch (compressionType) {
				case 2:
					return new Compression2().Compress(data, compressionSubtype);
				case 4:
					return new Compression4().Compress(data, numFastBytes);
			}

			throw new InvalidDataException("unknown TLZC compression type");
		}

		class Compression2 {
			public Stream Compress(Stream buffer, string compressionSubtype = null) {
				buffer.Position = 0;
				if (buffer.Length > uint.MaxValue) {
					throw new Exception("Input >= 4GB, cannot compress this.");
				}

				bool assume_zlib = compressionSubtype != "deflate";
				if (assume_zlib) {
					Console.WriteLine("compressing with zlib...");

					ulong insize = (ulong)buffer.Length;
					ulong maxsize = zlib.compressBound(insize);
					byte[] tempout = new byte[maxsize];
					ulong size = maxsize;
					int result = zlib.compress2(tempout, 0, ref size, buffer.CopyToByteArray(), 0, insize, zlib.Z_BEST_COMPRESSION);
					if (result != zlib.Z_OK) {
						throw new Exception(string.Format("zlib compression error ({0})", result));
					}

					ulong outsize = size + 0x18;
					if (outsize > uint.MaxValue) {
						throw new Exception("Output >= 4GB, cannot compress this.");
					}

					byte[] output = new byte[outsize];
					output[0x00] = 0x54;
					output[0x01] = 0x4C;
					output[0x02] = 0x5A;
					output[0x03] = 0x43;
					output[0x04] = 0x01;
					output[0x05] = 0x02;
					output[0x06] = 0x00;
					output[0x07] = 0x00;
					output[0x08] = (byte)(outsize & 0xFF);
					output[0x09] = (byte)((outsize >> 8) & 0xFF);
					output[0x0A] = (byte)((outsize >> 16) & 0xFF);
					output[0x0B] = (byte)((outsize >> 24) & 0xFF);
					output[0x0C] = (byte)(insize & 0xFF);
					output[0x0D] = (byte)((insize >> 8) & 0xFF);
					output[0x0E] = (byte)((insize >> 16) & 0xFF);
					output[0x0F] = (byte)((insize >> 24) & 0xFF);
					output[0x10] = 0x00;
					output[0x11] = 0x00;
					output[0x12] = 0x00;
					output[0x13] = 0x00;
					output[0x14] = 0x00;
					output[0x15] = 0x00;
					output[0x16] = 0x00;
					output[0x17] = 0x00;
					for (ulong i = 0; i < size; ++i) {
						output[0x18 + i] = tempout[i];
					}
					return new DuplicatableByteArrayStream(output);
				} else {
					Console.WriteLine("compressing with deflate...");

					Stream result;
					uint inSize = (uint)buffer.Length;
					if (inSize >= 0x40000000) {
						result = new MemoryStream64(inSize);
					} else {
						result = new MemoryStream((int)inSize);
					}

					result.WriteUInt32(0x544c5a43, EndianUtils.Endianness.BigEndian);
					result.WriteUInt32(0x0201);
					result.WriteUInt32(0);   // compressed size - we'll fill this in once we know it
					result.WriteUInt32(inSize);   // decompressed size
					result.WriteUInt32(0);   // unknown, 0
					result.WriteUInt32(0);   // unknown, 0

					// weird workaround for DeflateStream.Dispose() also disposing the wrapped stream
					Stream tmp;
					if (inSize >= 0x40000000) {
						tmp = new MemoryStream64();
					} else {
						tmp = new MemoryStream();
					}
					using (DeflateStream compressionStream = new DeflateStream(tmp, CompressionLevel.Optimal)) {
						compressionStream.Write(buffer.CopyToByteArray());
						compressionStream.Flush();
						tmp.Position = 0;
						StreamUtils.CopyStream(tmp, result);
					}

					if (result.Length > uint.MaxValue) {
						throw new Exception("Output >= 4GB, cannot compress this.");
					}

					// fill in compressed size
					result.Position = 8;
					result.WriteUInt32((uint)result.Length);

					return result;
				}
			}

			public Stream Decompress(Stream buffer, uint compressedSize, uint uncompressedSize, string compressionSubtype = null) {
				if (compressionSubtype == "zlib") {
					Console.WriteLine("decompressing with zlib...");
					return DecompressZlib(buffer, compressedSize, uncompressedSize);
				}
				if (compressionSubtype == "deflate") {
					Console.WriteLine("decompressing with deflate...");
					return DecompressDeflate(buffer, compressedSize, uncompressedSize);
				}

				try {
					Console.WriteLine("assuming zlib compression, trying to decompress...");
					return DecompressZlib(buffer, compressedSize, uncompressedSize);
				} catch (Exception ex) {
					Console.WriteLine("zlib decompression failed with error '{0}', trying deflate...", ex.Message);
					return DecompressDeflate(buffer, compressedSize, uncompressedSize);
				}
			}

			public Stream DecompressZlib(Stream buffer, uint compressedSize, uint uncompressedSize) {
				ulong insize = compressedSize - 0x18;
				ulong outsize = uncompressedSize;
				byte[] output = new byte[(long)outsize];
				int result = zlib.uncompress(output, 0, ref outsize, buffer.CopyToByteArray(), 0x18, insize);
				if (result != zlib.Z_OK) {
					throw new Exception(string.Format("zlib decompression error ({0})", result));
				}
				return new DuplicatableByteArrayStream(output);
			}

			public Stream DecompressDeflate(Stream buffer, uint compressedSize, uint uncompressedSize) {
				uint inSize = compressedSize - 0x18;
				uint outSize = uncompressedSize;
				Stream result;
				if (outSize >= 0x80000000) {
					result = new MemoryStream64(outSize);
				} else {
					result = new MemoryStream((int)outSize);
				}
				using (DeflateStream decompressionStream = new DeflateStream(buffer.ReadDuplicatableSubstream(inSize), CompressionMode.Decompress)) {
					StreamUtils.CopyStream(decompressionStream, result, outSize);
				}
				return result;
			}
		}

		class Compression4 {
			public Stream Compress(Stream buffer, int numFastBytes = 64, int litContextBits = 3, int litPosBits = 0, int posStateBits = 2, int blockSize = 0, int matchFinderCycles = 32) {
				buffer.Position = 0;
				if (buffer.Length > uint.MaxValue) {
					throw new Exception("Input >= 4GB, cannot compress this.");
				}
				uint inSize = (uint)buffer.Length;
				int streamCount = (int)((((long)inSize) + 0xffff) >> 16);

				Stream result;
				if (inSize >= 0x40000000) {
					result = new MemoryStream64(inSize);
				} else {
					result = new MemoryStream((int)inSize);
				}

				result.WriteUInt32(0x544c5a43, EndianUtils.Endianness.BigEndian);
				result.WriteUInt32(0x0401);
				result.WriteUInt32(0);   // compressed size - we'll fill this in once we know it
				result.WriteUInt32(inSize);   // decompressed size
				result.WriteUInt32(0);   // unknown, 0
				result.WriteUInt32(0);   // unknown, 0

				// next comes the coder properties (5 bytes), followed by stream lengths, followed by the streams themselves.

				var encoder = new SevenZip.Compression.LZMA.Encoder();
				var props = new Dictionary<SevenZip.CoderPropID, object>();
				props[SevenZip.CoderPropID.DictionarySize] = 0x10000;
				props[SevenZip.CoderPropID.MatchFinder] = "BT4";
				props[SevenZip.CoderPropID.NumFastBytes] = numFastBytes;
				props[SevenZip.CoderPropID.LitContextBits] = litContextBits;
				props[SevenZip.CoderPropID.LitPosBits] = litPosBits;
				props[SevenZip.CoderPropID.PosStateBits] = posStateBits;
				//props[SevenZip.CoderPropID.BlockSize] = blockSize; // this always throws an exception when set
				//props[SevenZip.CoderPropID.MatchFinderCycles] = matchFinderCycles; // ^ same here
				//props[SevenZip.CoderPropID.DefaultProp] = 0;
				//props[SevenZip.CoderPropID.UsedMemorySize] = 100000;
				//props[SevenZip.CoderPropID.Order] = 1;
				//props[SevenZip.CoderPropID.NumPasses] = 10;
				//props[SevenZip.CoderPropID.Algorithm] = 0;
				//props[SevenZip.CoderPropID.NumThreads] = ;
				//props[SevenZip.CoderPropID.EndMarker] = ;

				encoder.SetCoderProperties(props.Keys.ToArray(), props.Values.ToArray());

				encoder.WriteCoderProperties(result);

				// reserve space for the stream lengths. we'll fill them in later after we know what they are.
				long streamCountPos = result.Position;
				for (int i = 0; i < streamCount; ++i) {
					result.WriteUInt16(0);
				}

				List<ushort> streamSizes = new List<ushort>();

				using (MemoryStream tmp = new MemoryStream(0x10000)) {
					for (int i = 0; i < streamCount; ++i) {
						long preLength = result.Length;

						// encoder doesn't seem to respect the passed inSize??
						uint chunkSize = Math.Min(inSize, 0x10000);
						if (chunkSize == 0x10000) {
							tmp.Position = 0;
							StreamUtils.CopyStream(buffer, tmp, chunkSize);
							tmp.Position = 0;
							encoder.Code(tmp, result, chunkSize, -1, null);
						} else {
							MemoryStream tmp2 = new MemoryStream((int)chunkSize);
							StreamUtils.CopyStream(buffer, tmp2, chunkSize);
							tmp2.Position = 0;
							encoder.Code(tmp2, result, chunkSize, -1, null);
						}

						int streamSize = (int)(result.Length - preLength);
						if (streamSize >= 0x10000) {
							System.Diagnostics.Trace.WriteLine("Warning! stream did not compress at all. This will cause a different code path to be executed on the PS3 whose operation is assumed and not tested!");
							result.Position = preLength;
							result.SetLength(preLength);
							tmp.Position = 0;
							StreamUtils.CopyStream(tmp, result, chunkSize);
							streamSize = 0;
						}

						inSize -= 0x10000;
						streamSizes.Add((ushort)streamSize);
					}
				}

				if (result.Length > uint.MaxValue) {
					throw new Exception("Output >= 4GB, cannot compress this.");
				}

				// fill in compressed size
				result.Position = 8;
				result.WriteUInt32((uint)result.Length);

				// fill in stream sizes
				result.Position = streamCountPos;
				for (int i = 0; i < streamCount; ++i) {
					result.WriteUInt16(streamSizes[i]);
				}

				return result;
			}

			public Stream Decompress(Stream buffer, uint outSize) {
				Stream result;
				if (outSize >= 0x80000000) {
					result = new MemoryStream64(outSize);
				} else {
					result = new MemoryStream((int)outSize);
				}
				int streamCount = (int)((((long)outSize) + 0xffff) >> 16);
				var decoder = new SevenZip.Compression.LZMA.Decoder();
				decoder.SetDecoderProperties(buffer.ReadBytes(5));
				ushort[] streamSizes = new ushort[streamCount];
				for (int i = 0; i < streamCount; ++i) {
					streamSizes[i] = buffer.ReadUInt16(EndianUtils.Endianness.LittleEndian);
				}

				uint decompressedBytesLeft = outSize;
				using (MemoryStream tmp = new MemoryStream(0x10000)) {
					for (int i = 0; i < streamCount; ++i) {
						int streamSize = streamSizes[i];
						if (streamSize != 0) {
							tmp.Position = 0;
							StreamUtils.CopyStream(buffer, tmp, streamSize);
							tmp.Position = 0;
							decoder.Code(tmp, result, streamSize, Math.Min(decompressedBytesLeft, 0x10000), null);
						} else {
							StreamUtils.CopyStream(buffer, result, Math.Min(decompressedBytesLeft, 0x10000));
						}
						decompressedBytesLeft -= 0x10000;
					}
				}

				return result;
			}
		}
	}
}
