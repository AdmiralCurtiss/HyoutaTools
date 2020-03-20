using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.Patches.Bps {
	public class BpsPatcher {
		private Stream Source;
		private Stream Patch;
		private Stream Target;

		private ulong SourceRelativeOffset = 0;
		private ulong TargetRelativeOffset = 0;

		private BpsPatcher(Stream source, Stream patch, Stream target) {
			Source = source;
			Patch = patch;
			Target = target;
		}

		private void ApplyPatchToStreamInternal() {
			Patch.Position = 0;
			Target.Position = 0;

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
			uint actualChecksumPatch = Checksums.CRC32.CalculateCRC32FromCurrentPosition(Patch, patchSize - 4);
			if (checksumPatch != actualChecksumPatch) {
				throw new Exception("Patch checksum incorrect.");
			}

			Patch.Position = 0;
			uint magic = Patch.ReadUInt32().FromEndian(EndianUtils.Endianness.LittleEndian);
			if (magic != 0x31535042) {
				throw new Exception("Wrong patch magic.");
			}

			ulong sourceSize = Patch.ReadUnsignedNumber();
			if (sourceSize != (ulong)Source.Length) {
				throw new Exception(string.Format("Source size mismatch. Actual size is {0}, patch wants {1}.", Source.Length, sourceSize));
			}
			ulong targetSize = Patch.ReadUnsignedNumber();
			ulong metadataSize = Patch.ReadUnsignedNumber();
			if (metadataSize > 0) {
				// skip metadata, we don't care about it
				Patch.Position = (long)(((ulong)Patch.Position) + metadataSize);
			}

			Source.Position = 0;
			uint actualChecksumSource = Checksums.CRC32.CalculateCRC32FromCurrentPosition(Source, Source.Length);
			if (checksumSource != actualChecksumSource) {
				throw new Exception("Source checksum incorrect.");
			}

			Source.Position = 0;
			Target.Position = 0;
			while (Patch.Position < patchFooterPosition) {
				(Action action, ulong length) = Patch.ReadAction();
				switch (action) {
					case Action.SourceRead: DoSourceRead(length); break;
					case Action.TargetRead: DoTargetRead(length); break;
					case Action.SourceCopy: DoSourceCopy(length); break;
					case Action.TargetCopy: DoTargetCopy(length); break;
				}
			}

			if (Patch.Position != patchFooterPosition) {
				throw new Exception("Malformed action stream.");
			}

			if ((ulong)Target.Position != targetSize) {
				throw new Exception("Target size incorrect.");
			}

			Target.Position = 0;
			uint actualChecksumTarget = Checksums.CRC32.CalculateCRC32FromCurrentPosition(Target, Target.Length);
			if (checksumTarget != actualChecksumTarget) {
				throw new Exception("Target checksum incorrect.");
			}
		}

		private void DoSourceRead(ulong length) {
			if (Source.Length < Target.Position) {
				throw new Exception("Invalid length in SourceRead.");
			}
			Source.Position = Target.Position;
			if (((ulong)Source.Position) + length > (ulong)Source.Length) {
				throw new Exception("Invalid length in SourceRead.");
			}
			StreamUtils.CopyStream(Source, Target, length);
		}

		private void DoTargetRead(ulong length) {
			if (((ulong)Patch.Position) + length > (ulong)Patch.Length) {
				throw new Exception("Invalid length in TargetRead.");
			}
			StreamUtils.CopyStream(Patch, Target, length);
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
			AddChecked(ref SourceRelativeOffset, d, Source.Length);
			if (SourceRelativeOffset + length > (ulong)Source.Length) {
				throw new Exception("Invalid length in SourceCopy.");
			}
			Source.Position = (long)SourceRelativeOffset;
			StreamUtils.CopyStream(Source, Target, length);
			SourceRelativeOffset += length;
		}

		private void DoTargetCopy(ulong length) {
			long d = Patch.ReadSignedNumber();
			AddChecked(ref TargetRelativeOffset, d, Target.Length);
			for (ulong i = 0; i < length; ++i) {
				long p = Target.Position;
				Target.Position = (long)TargetRelativeOffset;
				++TargetRelativeOffset;
				byte b = Target.ReadUInt8();
				Target.Position = p;
				Target.WriteByte(b);
			}
		}

		public static void ApplyPatchToStream(Stream source, Stream patch, Stream target) {
			new BpsPatcher(source, patch, target).ApplyPatchToStreamInternal();
		}
	}
}
