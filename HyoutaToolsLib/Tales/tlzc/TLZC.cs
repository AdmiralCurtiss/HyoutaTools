using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using HyoutaUtils;
using zlib_sharp;

namespace HyoutaTools.Tales.tlzc {
	public class TLZC {
		public static byte[] Decompress(byte[] tlzcBuffer, int? compressionType = null, string compressionSubtype = null) {
			if ( tlzcBuffer[0] != 'T' || tlzcBuffer[1] != 'L' || tlzcBuffer[2] != 'Z' || tlzcBuffer[3] != 'C' )
				throw new InvalidDataException( "buffer does not have TLZC header" );
			if ( BitConverter.ToInt32( tlzcBuffer, 8 ) != tlzcBuffer.Length )
				throw new InvalidDataException( "buffer length does not match declared buffer length" );

			int ctype = compressionType.HasValue ? compressionType.Value : tlzcBuffer[5];
			switch (ctype) {
				case 2:
					return new Compression2().Decompress(tlzcBuffer, compressionSubtype);
				case 4:
					return new Compression4().Decompress(tlzcBuffer);
			}

			throw new InvalidDataException( "unknown TLZC compression type" );
		}

		public static byte[] Compress(byte[] data, int compressionType, string compressionSubtype, int numFastBytes = 64) {
			switch (compressionType) {
				case 2:
					return new Compression2().Compress(data, compressionSubtype);
				case 4:
					return new Compression4().Compress(data, numFastBytes);
			}

			throw new InvalidDataException("unknown TLZC compression type");
		}

		class Compression2 {
			public byte[] Compress(byte[] buffer, string compressionSubtype = null) {
				bool assume_zlib = compressionSubtype != "deflate";
				if (assume_zlib) {
					Console.WriteLine("compressing with zlib...");

					ulong insize = (ulong)buffer.LongLength;
					ulong maxsize = zlib.compressBound(insize);
					byte[] tempout = new byte[maxsize];
					ulong size = maxsize;
					int result = zlib.compress2(tempout, 0, ref size, buffer, 0, insize, zlib.Z_BEST_COMPRESSION);
					if (result != zlib.Z_OK) {
						throw new Exception(string.Format("zlib compression error ({0})", result));
					}

					ulong outsize = size + 0x18;
					byte[] output = new byte[outsize];
					output[0x00] = 0x54; output[0x01] = 0x4C; output[0x02] = 0x5A; output[0x03] = 0x43;
					output[0x04] = 0x01; output[0x05] = 0x02; output[0x06] = 0x00; output[0x07] = 0x00;
					output[0x08] = (byte)(outsize & 0xFF);
					output[0x09] = (byte)((outsize >> 8) & 0xFF);
					output[0x0A] = (byte)((outsize >> 16) & 0xFF);
					output[0x0B] = (byte)((outsize >> 24) & 0xFF);
					output[0x0C] = (byte)(insize & 0xFF);
					output[0x0D] = (byte)((insize >> 8) & 0xFF);
					output[0x0E] = (byte)((insize >> 16) & 0xFF);
					output[0x0F] = (byte)((insize >> 24) & 0xFF);
					output[0x10] = 0x00; output[0x11] = 0x00; output[0x12] = 0x00; output[0x13] = 0x00;
					output[0x14] = 0x00; output[0x15] = 0x00; output[0x16] = 0x00; output[0x17] = 0x00;
					for (ulong i = 0; i < size; ++i) {
						output[0x18 + i] = tempout[i];
					}
					return output;
				} else {
					Console.WriteLine("compressing with deflate...");

					MemoryStream result = new MemoryStream();
					BinaryWriter bw = new BinaryWriter(result);

					bw.Write(new byte[] { 0x54, 0x4C, 0x5A, 0x43 });
					bw.Write((byte)0x01);
					bw.Write((byte)0x02);
					bw.Write((byte)0x00);
					bw.Write((byte)0x00);
					bw.Write((int)0);   // compressed size - we'll fill this in once we know it
					bw.Write((int)buffer.Length);   // decompressed size
					bw.Write((int)0);   // unknown, 0
					bw.Write((int)0);   // unknown, 0

					using ( DeflateStream compressionStream = new DeflateStream( result, CompressionLevel.Optimal ) ) {
						compressionStream.Write( buffer );
					}

					byte[] retval = result.ToArray();

					// fill in compressed size
					ArrayUtils.CopyByteArrayPart(BitConverter.GetBytes((int)retval.Length), 0, retval, 8, 4);

					return retval;
				}
			}

			public byte[] Decompress(byte[] buffer, string compressionSubtype = null) {
				if (compressionSubtype == "zlib") {
					Console.WriteLine("decompressing with zlib...");
					return DecompressZlib(buffer);
				}
				if (compressionSubtype == "deflate") {
					Console.WriteLine("decompressing with deflate...");
					return DecompressDeflate(buffer);
				}

				try {
					Console.WriteLine("assuming zlib compression, trying to decompress...");
					return DecompressZlib(buffer);
				} catch (Exception ex) {
					Console.WriteLine("zlib decompression failed with error '{0}', trying deflate...", ex.Message);
					return DecompressDeflate(buffer);
				}
			}

			public byte[] DecompressZlib(byte[] buffer) {
				ulong insize = BitConverter.ToUInt32(buffer, 8) - 0x18;
				ulong outsize = BitConverter.ToUInt32(buffer, 12);
				byte[] output = new byte[(long)outsize];
				int result = zlib.uncompress(output, 0, ref outsize, buffer, 0x18, insize);
				if (result != zlib.Z_OK) {
					throw new Exception(string.Format("zlib decompression error ({0})", result));
				}
				return output;
			}

			public byte[] DecompressDeflate(byte[] buffer) {
				MemoryStream result = new MemoryStream();
				int inSize = BitConverter.ToInt32(buffer, 8);
				int outSize = BitConverter.ToInt32(buffer, 12);
				int offset = 0x18;
				using (DeflateStream decompressionStream = new DeflateStream(new MemoryStream(buffer, offset, inSize - offset), CompressionMode.Decompress)) {
					StreamUtils.CopyStream(decompressionStream, result, outSize);
				}
				return result.ToArray();
			}
		}

		class Compression4 {
			public byte[] Compress( byte[] buffer, int numFastBytes = 64, int litContextBits = 3, int litPosBits = 0, int posStateBits = 2, int blockSize = 0, int matchFinderCycles = 32 ) {
				MemoryStream result = new MemoryStream();
				int inSize = buffer.Length;
				int streamCount = ( inSize + 0xffff ) >> 16;
				int offset = 0;

				BinaryWriter bw = new BinaryWriter( result );

				bw.Write( new byte[] { 0x54, 0x4C, 0x5A, 0x43 } );
				bw.Write( (byte)0x01 );
				bw.Write( (byte)0x04 );
				bw.Write( (byte)0x00 );
				bw.Write( (byte)0x00 );
				bw.Write( (int)0 );   // compressed size - we'll fill this in once we know it
				bw.Write( (int)buffer.Length );   // decompressed size
				bw.Write( (int)0 );   // unknown, 0
				bw.Write( (int)0 );   // unknown, 0
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

				encoder.SetCoderProperties( props.Keys.ToArray(), props.Values.ToArray() );

				encoder.WriteCoderProperties( result );

				// reserve space for the stream lengths. we'll fill them in later after we know what they are.
				bw.Write( new byte[streamCount * 2] );

				List<int> streamSizes = new List<int>();

				for ( int i = 0; i < streamCount; i++ ) {
					long preLength = result.Length;

					encoder.Code( new MemoryStream( buffer, offset, Math.Min( inSize, 0x10000 ) ), result, Math.Min( inSize, 0x10000 ), -1, null );

					int streamSize = (int)( result.Length - preLength );
					if ( streamSize >= 0x10000 ) {
						System.Diagnostics.Trace.WriteLine( "Warning! stream did not compress at all. This will cause a different code path to be executed on the PS3 whose operation is assumed and not tested!" );
						result.Position = preLength;
						result.SetLength( preLength );
						result.Write( buffer, offset, Math.Min( inSize, 0x10000 ) );
						streamSize = 0;
					}

					inSize -= 0x10000;
					offset += 0x10000;
					streamSizes.Add( streamSize );
				}

				// fill in compressed size
				result.Position = 8;
				bw.Write( (int)result.Length );

				byte[] temp = result.ToArray();

				// fill in stream sizes
				for ( int i = 0; i < streamSizes.Count; i++ ) {
					temp[5 + 0x18 + i * 2] = (byte)streamSizes[i];
					temp[6 + 0x18 + i * 2] = (byte)( streamSizes[i] >> 8 );
				}

				return temp;
			}

			public byte[] Decompress( byte[] buffer ) {
				MemoryStream result = new MemoryStream();
				int outSize = BitConverter.ToInt32( buffer, 12 );
				int streamCount = ( outSize + 0xffff ) >> 16;
				int offset = 0x18 + streamCount * 2 + 5;

				var decoder = new SevenZip.Compression.LZMA.Decoder();
				decoder.SetDecoderProperties( new MemoryStream( buffer, 0x18, 5 ).ToArray() );

				for ( int i = 0; i < streamCount; i++ ) {
					int streamSize = ( buffer[5 + 0x18 + i * 2] ) + ( buffer[6 + 0x18 + i * 2] << 8 );
					if ( streamSize != 0 )
						decoder.Code( new MemoryStream( buffer, offset, streamSize ), result, streamSize, Math.Min( outSize, 0x10000 ), null );
					else
						result.Write( buffer, offset, streamSize = Math.Min( outSize, 0x10000 ) );
					outSize -= 0x10000;
					offset += streamSize;
				}

				return result.ToArray();

			}
		}
	}
}
