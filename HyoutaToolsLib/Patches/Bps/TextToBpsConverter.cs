using HyoutaUtils;
using HyoutaUtils.Bps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Patches.Bps {
	public class TextToBpsConverter {
		private Stream TextIn;
		private Stream PatchOut;

		private ulong SourceRelativeOffset = 0;
		private ulong TargetRelativeOffset = 0;

		private static readonly char[] whitespace = new char[] { ' ', '\r', '\n', '\t' };

		public static int Execute(List<string> args) {
			if (args.Count < 1) {
				Console.WriteLine("Usage: infile.txt [outfile.bps]");
				return -1;
			}

			string inpath = args[0];
			string outpath = args.Count >= 2 ? args[1] : (inpath + ".bps");

			using (var instream = new HyoutaUtils.Streams.DuplicatableFileStream(inpath))
			using (var outstream = new FileStream(outpath, FileMode.Create)) {
				TextToBpsConverter.GeneratePatchFromText(instream, outstream);
			}

			return 0;
		}

		private TextToBpsConverter(Stream textin, Stream patchout) {
			TextIn = textin;
			PatchOut = patchout;
		}

		private string ReadLine() {
			List<byte> bytes = new List<byte>();
			int b = TextIn.ReadByte();
			bool eof = b == -1;
			if (eof) {
				return null;
			}
			while (b != -1 && b != (byte)'\n') {
				bytes.Add((byte)b);
				b = TextIn.ReadByte();
			}
			if (bytes.Count == 0) {
				// line was empty, go to next
				return ReadLine();
			}
			string str = Encoding.UTF8.GetString(bytes.ToArray()).Trim();
			if (str.Length == 0) {
				// line was empty, go to next
				return ReadLine();
			}
			return str;
		}

		private static List<byte> ParseByteArray(string str) {
			List<byte> bytes = new List<byte>();
			foreach (string s in str.Split(whitespace, StringSplitOptions.RemoveEmptyEntries)) {
				bytes.Add(byte.Parse(s, System.Globalization.NumberStyles.HexNumber));
			}
			return bytes;
		}

		private void ApplyPatchToStreamInternal() {
			PatchOut.WriteUInt32(0x31535042, EndianUtils.Endianness.LittleEndian);

			ulong sourceSize = 0;
			ulong targetSize = 0;
			uint sourceChecksum = 0;
			uint targetChecksum = 0;
			List<byte> metadata = new List<byte>();

			// header
			string l = ReadLine();
			while (l != null && !l.StartsWith("SourceRead") && !l.StartsWith("TargetRead") && !l.StartsWith("SourceCopy") && !l.StartsWith("TargetCopy")) {
				if (l.StartsWith("SourceSize")) {
					sourceSize = HexUtils.ParseDecOrHexUInt64(l.Split(whitespace, StringSplitOptions.RemoveEmptyEntries)[1]);
				} else if (l.StartsWith("TargetSize")) {
					targetSize = HexUtils.ParseDecOrHexUInt64(l.Split(whitespace, StringSplitOptions.RemoveEmptyEntries)[1]);
				} else if (l.StartsWith("SourceChecksum")) {
					sourceChecksum = HexUtils.ParseDecOrHex(l.Split(whitespace, StringSplitOptions.RemoveEmptyEntries)[1]);
				} else if (l.StartsWith("TargetChecksum")) {
					targetChecksum = HexUtils.ParseDecOrHex(l.Split(whitespace, StringSplitOptions.RemoveEmptyEntries)[1]);
				} else if (l.StartsWith("Metadata")) {
					metadata = ParseByteArray(l.Split(whitespace, 2, StringSplitOptions.RemoveEmptyEntries)[1]);
				}

				l = ReadLine();
			}
			PatchOut.WriteUnsignedNumber(sourceSize);
			PatchOut.WriteUnsignedNumber(targetSize);
			PatchOut.WriteUnsignedNumber((ulong)metadata.Count);
			foreach (byte b in metadata) {
				PatchOut.WriteByte(b);
			}

			// actions
			while (l != null) {
				if (l.StartsWith("SourceRead")) {
					var split = l.Split(whitespace, StringSplitOptions.RemoveEmptyEntries);
					ulong length = HexUtils.ParseDecOrHexUInt64(split[3]);
					PatchOut.WriteAction(HyoutaUtils.Bps.Action.SourceRead, length);
				} else if (l.StartsWith("TargetRead")) {
					List<byte> bytes = ParseByteArray(l.Split(whitespace, 2, StringSplitOptions.RemoveEmptyEntries)[1]);
					PatchOut.WriteAction(HyoutaUtils.Bps.Action.TargetRead, (ulong)bytes.Count);
					foreach (byte b in bytes) {
						PatchOut.WriteByte(b);
					}
				} else if (l.StartsWith("SourceCopy")) {
					var split = l.Split(whitespace, StringSplitOptions.RemoveEmptyEntries);
					ulong length = HexUtils.ParseDecOrHexUInt64(split[3]);
					long offset;
					if (split[1].ToLowerInvariant() == "absolute") {
						long pos = HexUtils.ParseDecOrHexInt64(split[2]);
						offset = (pos - (long)SourceRelativeOffset);
					} else if (split[1].ToLowerInvariant() == "relative") {
						offset = HexUtils.ParseDecOrHexInt64(split[2]);
					} else {
						throw new Exception("SourceCopy argument must be 'absolute' or 'relative'.");
					}
					AddChecked(ref SourceRelativeOffset, offset, (long)sourceSize);
					SourceRelativeOffset += length;
					PatchOut.WriteAction(HyoutaUtils.Bps.Action.SourceCopy, length);
					PatchOut.WriteSignedNumber(offset);
				} else if (l.StartsWith("TargetCopy")) {
					var split = l.Split(whitespace, StringSplitOptions.RemoveEmptyEntries);
					ulong length = HexUtils.ParseDecOrHexUInt64(split[3]);
					long offset;
					if (split[1].ToLowerInvariant() == "absolute") {
						long pos = HexUtils.ParseDecOrHexInt64(split[2]);
						offset = (pos - (long)TargetRelativeOffset);
					} else if (split[1].ToLowerInvariant() == "relative") {
						offset = HexUtils.ParseDecOrHexInt64(split[2]);
					} else {
						throw new Exception("TargetCopy argument must be 'absolute' or 'relative'.");
					}
					AddChecked(ref TargetRelativeOffset, offset, (long)targetSize);
					TargetRelativeOffset += length;
					PatchOut.WriteAction(HyoutaUtils.Bps.Action.TargetCopy, length);
					PatchOut.WriteSignedNumber(offset);
				}

				l = ReadLine();
			}

			PatchOut.WriteUInt32(sourceChecksum, EndianUtils.Endianness.LittleEndian);
			PatchOut.WriteUInt32(targetChecksum, EndianUtils.Endianness.LittleEndian);

			long checksumsize = PatchOut.Position;
			PatchOut.Position = 0;
			var patchChecksum = ChecksumUtils.CalculateCRC32FromCurrentPosition(PatchOut, checksumsize);
			PatchOut.WriteUInt32(patchChecksum.Value, EndianUtils.Endianness.LittleEndian);
		}

		private void AddChecked(ref ulong pos, long d, long length) {
			// pos must end up in range [0, length-1], else error out
			if (d >= 0) {
				// d is positive, make sure we don't end up >= length
				ulong o = (ulong)d;
				if (o >= (((ulong)length) - pos)) {
					throw new Exception("Invalid offset.");
				}
				pos += o;
			} else {
				// d is negative, make sure we don't end up < 0
				ulong o = (ulong)(-d);
				if (o > pos) {
					throw new Exception("Invalid offset.");
				}
				pos -= o;
			}
		}

		public static void GeneratePatchFromText(Stream textin, Stream binout) {
			new TextToBpsConverter(textin, binout).ApplyPatchToStreamInternal();
		}
	}
}