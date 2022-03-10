using HyoutaUtils;
using HyoutaUtils.Bps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Patches.Bps {
	public class BpsToTextConverter {
		private long SourcePosition;
		private long SourceLength;
		private Stream Source;
		private Stream Patch;
		private Stream Target;
		private long TargetPosition;
		private long TargetLength;
		private Stream TextOutput;

		private ulong SourceRelativeOffset = 0;
		private ulong TargetRelativeOffset = 0;

		public static int Execute(List<string> args) {
			if (args.Count < 1) {
				Console.WriteLine("Usage: infile.bps [outfile.txt] [source.bin]");
				return -1;
			}

			string inpath = args[0];
			string outpath = args.Count >= 2 ? args[1] : (inpath + ".txt");
			string sourcepath = args.Count >= 3 ? args[2] : null;

			using (var instream = new HyoutaUtils.Streams.DuplicatableFileStream(inpath))
			using (var outstream = new FileStream(outpath, FileMode.Create)) {
				BpsToTextConverter.ApplyPatchToStream(instream, outstream, sourcepath != null ? new HyoutaUtils.Streams.DuplicatableFileStream(sourcepath).CopyToMemoryAndDispose() : null);
			}

			return 0;
		}

		private BpsToTextConverter(Stream patch, Stream textout, Stream source) {
			Patch = patch;
			TextOutput = textout;
			Source = source;
			Target = source != null ? new MemoryStream() : null;
		}

		private void WriteText(string str) {
			TextOutput.Write(Encoding.UTF8.GetBytes(str));
			TextOutput.WriteByte((byte)'\n');
		}

		private void ApplyPatchToStreamInternal() {
			Patch.Position = 0;
			TargetPosition = 0;
			if (Target != null) Target.Position = 0;

			long patchSize = Patch.Length;
			if (patchSize < 12) {
				throw new Exception("Patch too small to be a valid patch.");
			}
			long patchFooterPosition = patchSize - 12L;

			// note: spec doesn't actually say what endian, but files in the wild suggest little
			Patch.Position = patchFooterPosition;
			uint checksumSource = Patch.ReadUInt32().FromEndian(EndianUtils.Endianness.LittleEndian);
			uint checksumTarget = Patch.ReadUInt32().FromEndian(EndianUtils.Endianness.LittleEndian);
			uint checksumPatch = Patch.ReadUInt32().FromEndian(EndianUtils.Endianness.LittleEndian);

			Patch.Position = 0;
			uint actualChecksumPatch = ChecksumUtils.CalculateCRC32FromCurrentPosition(Patch, patchSize - 4).Value;
			if (checksumPatch != actualChecksumPatch) {
				throw new Exception("Patch checksum incorrect.");
			}

			Patch.Position = 0;
			uint magic = Patch.ReadUInt32().FromEndian(EndianUtils.Endianness.LittleEndian);
			if (magic != 0x31535042) {
				throw new Exception("Wrong patch magic.");
			}

			ulong sourceSize = Patch.ReadUnsignedNumber();
			if (Source != null && sourceSize != (ulong)Source.Length) {
				throw new Exception(string.Format("Source size mismatch. Actual size is {0}, patch wants {1}.", Source.Length, sourceSize));
			}
			WriteText(string.Format("SourceSize 0x{0:x}", sourceSize));
			SourceLength = (long)sourceSize;
			ulong targetSize = Patch.ReadUnsignedNumber();
			WriteText(string.Format("TargetSize 0x{0:x}", targetSize));
			TargetLength = 0;
			ulong metadataSize = Patch.ReadUnsignedNumber();
			if (metadataSize > 0) {
				StringBuilder sb = new StringBuilder("Metadata");
				ReadPatchBytesToStringBuilder(sb, metadataSize);
				WriteText(sb.ToString());
			}

			WriteText(string.Format("SourceChecksum 0x{0:x}", checksumSource));
			WriteText(string.Format("TargetChecksum 0x{0:x}", checksumTarget));

			SourcePosition = 0;
			if (Source != null) {
				Source.Position = 0;
				uint actualChecksumSource = ChecksumUtils.CalculateCRC32FromCurrentPosition(Source, Source.Length).Value;
				if (checksumSource != actualChecksumSource) {
					throw new Exception("Source checksum incorrect.");
				}
			}

			if (Source != null) Source.Position = 0;
			SourcePosition = 0;
			if (Target != null) Target.Position = 0;
			TargetPosition = 0;
			while (Patch.Position < patchFooterPosition) {
				(HyoutaUtils.Bps.Action action, ulong length) = Patch.ReadAction();
				switch (action) {
					case HyoutaUtils.Bps.Action.SourceRead: DoSourceRead(length); break;
					case HyoutaUtils.Bps.Action.TargetRead: DoTargetRead(length); break;
					case HyoutaUtils.Bps.Action.SourceCopy: DoSourceCopy(length); break;
					case HyoutaUtils.Bps.Action.TargetCopy: DoTargetCopy(length); break;
				}
			}

			if (Patch.Position != patchFooterPosition) {
				throw new Exception("Malformed action stream.");
			}

			if ((ulong)TargetPosition != targetSize) {
				throw new Exception("Target size incorrect.");
			}

			TargetPosition = 0;
			if (Target != null) {
				Target.Position = 0;
				uint actualChecksumTarget = ChecksumUtils.CalculateCRC32FromCurrentPosition(Target, Target.Length).Value;
				if (checksumTarget != actualChecksumTarget) {
					throw new Exception("Target checksum incorrect.");
				}
			}
		}

		private string CopyStreamAndMakeText(Stream input, Stream output, ulong count) {
			StringBuilder sb = new StringBuilder();
			for (ulong i = 0; i < count; ++i) {
				int b = input.ReadByte();
				if (b == -1) {
					throw new Exception("Invalid stream copy.");
				}
				output.WriteByte((byte)b);
				sb.AppendFormat("{0:x2} ", (byte)b);
			}
			return sb.ToString();
		}

		private void DoSourceRead(ulong length) {
			if (SourceLength < TargetPosition) {
				throw new Exception("Invalid length in SourceRead.");
			}
			SourcePosition = TargetPosition;
			if (((ulong)SourcePosition) + length > (ulong)SourceLength) {
				throw new Exception("Invalid length in SourceRead.");
			}
			if (Source != null && Target != null) {
				Source.Position = Target.Position;
				string str = CopyStreamAndMakeText(Source, Target, length);
				WriteText(string.Format("SourceRead absolute 0x{0:x} 0x{1:x}   # {2}", SourcePosition, length, str));
			} else {
				WriteText(string.Format("SourceRead absolute 0x{0:x} 0x{1:x}", SourcePosition, length));
			}
			SourcePosition += (long)length;
			TargetPosition += (long)length;
			SyncTargetLength();
		}

		private void DoTargetRead(ulong length) {
			if (((ulong)Patch.Position) + length > (ulong)Patch.Length) {
				throw new Exception("Invalid length in TargetRead.");
			}
			if (Source != null && Target != null) {
				long p = Patch.Position;
				StreamUtils.CopyStream(Patch, Target, length);
				Patch.Position = p;
			}
			StringBuilder sb = new StringBuilder("TargetRead");
			ReadPatchBytesToStringBuilder(sb, length);
			WriteText(sb.ToString());
			TargetPosition += (long)length;
			SyncTargetLength();
		}

		private void ReadPatchBytesToStringBuilder(StringBuilder sb, ulong length) {
			for (ulong i = 0; i < length; ++i) {
				byte b = Patch.ReadUInt8();
				sb.AppendFormat(" {0:x2}", b);
			}
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

		private void DoSourceCopy(ulong length) {
			long d = Patch.ReadSignedNumber();
			AddChecked(ref SourceRelativeOffset, d, SourceLength);
			if (SourceRelativeOffset + length > (ulong)SourceLength) {
				throw new Exception("Invalid length in SourceCopy.");
			}
			SourcePosition = (long)SourceRelativeOffset;
			if (Source != null && Target != null) {
				Source.Position = (long)SourceRelativeOffset;
				string str = CopyStreamAndMakeText(Source, Target, length);
				WriteText(string.Format("SourceCopy absolute 0x{0:x} 0x{1:x}   # {2}", SourcePosition, length, str));
			} else {
				WriteText(string.Format("SourceCopy absolute 0x{0:x} 0x{1:x}", SourcePosition, length));
			}
			SourcePosition += (long)length;
			TargetPosition += (long)length;
			SyncTargetLength();
			SourceRelativeOffset += length;
		}

		private void DoTargetCopy(ulong length) {
			long d = Patch.ReadSignedNumber();
			AddChecked(ref TargetRelativeOffset, d, TargetLength);
			if (Source != null && Target != null) {
				StringBuilder sb = new StringBuilder();
				for (ulong i = 0; i < length; ++i) {
					long p = Target.Position;
					Target.Position = (long)TargetRelativeOffset;
					++TargetRelativeOffset;
					byte b = Target.ReadUInt8();
					sb.AppendFormat("{0:x2} ", b);
					Target.Position = p;
					Target.WriteByte(b);
				}
				WriteText(string.Format("TargetCopy absolute 0x{0:x} 0x{1:x}   # {2}", TargetRelativeOffset, length, sb.ToString()));
			} else {
				WriteText(string.Format("TargetCopy absolute 0x{0:x} 0x{1:x}", TargetRelativeOffset, length));
				TargetRelativeOffset += length;
			}
			TargetPosition += (long)length;
			SyncTargetLength();
		}

		private void SyncTargetLength() {
			TargetLength = Math.Max(TargetLength, TargetPosition);
		}

		public static void ApplyPatchToStream(Stream patch, Stream textout, Stream source = null) {
			new BpsToTextConverter(patch, textout, source).ApplyPatchToStreamInternal();
		}
	}
}