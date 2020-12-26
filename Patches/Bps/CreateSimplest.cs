using HyoutaUtils;
using HyoutaUtils.Bps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Patches.Bps {
	public class CreateSimplest {
		private Stream Source;
		private Stream Target;
		private Stream PatchOut;

		private CreateSimplest(Stream source, Stream target, Stream patch) {
			Source = source;
			Target = target;
			PatchOut = patch;
		}

		private void CreatePatchInternal() {
			Source.Position = 0;
			Target.Position = 0;
			ulong sourceSize = (ulong)Source.Length;
			ulong targetSize = (ulong)Target.Length;
			var sourceChecksum = ChecksumUtils.CalculateCRC32ForEntireStream(Source);
			var targetChecksum = ChecksumUtils.CalculateCRC32ForEntireStream(Target);

			// header
			PatchOut.WriteUInt32(0x31535042, EndianUtils.Endianness.LittleEndian);
			PatchOut.WriteUnsignedNumber(sourceSize);
			PatchOut.WriteUnsignedNumber(targetSize);
			PatchOut.WriteUnsignedNumber(0); // metadata

			// actions
			while (Target.Position < Target.Length) {
				int s;
				int t;
				bool advancedS;
				bool advancedT;
				if (IsNextByteSameInSourceAndTarget(out s, out t, out advancedS, out advancedT)) {
					ulong count = 1;
					while (IsNextByteSameInSourceAndTarget(out s, out t, out advancedS, out advancedT)) {
						++count;
					}
					if (advancedS) {
						Source.Position -= 1;
					}
					if (advancedT) {
						Target.Position -= 1;
					}
					PatchOut.WriteAction(HyoutaUtils.Bps.Action.SourceRead, count);
				} else {
					if (advancedS) {
						Source.Position -= 1;
					}
					if (advancedT) {
						Target.Position -= 1;
					}
					long p = Target.Position;
					ulong count = 0;
					while (ShouldWriteNextByteFromTarget(out s, out t, out advancedS, out advancedT)) {
						++count;
					}
					if (advancedS) {
						Source.Position -= 1;
					}
					PatchOut.WriteAction(HyoutaUtils.Bps.Action.TargetRead, count);
					Target.Position = p;
					StreamUtils.CopyStream(Target, PatchOut, (long)count);
				}
			}

			PatchOut.WriteUInt32(sourceChecksum.Value, EndianUtils.Endianness.LittleEndian);
			PatchOut.WriteUInt32(targetChecksum.Value, EndianUtils.Endianness.LittleEndian);

			long checksumsize = PatchOut.Position;
			PatchOut.Position = 0;
			var patchChecksum = ChecksumUtils.CalculateCRC32FromCurrentPosition(PatchOut, checksumsize);
			PatchOut.WriteUInt32(patchChecksum.Value, EndianUtils.Endianness.LittleEndian);
		}

		private bool IsNextByteSameInSourceAndTarget(out int s, out int t, out bool advancedS, out bool advancedT) {
			s = -1;
			advancedS = false;
			advancedT = false;

			t = Target.ReadByte();
			if (t == -1) {
				// target is eof
				return false;
			}
			advancedT = true;

			s = Source.ReadByte();
			if (s == -1) {
				// source is eof
				return false;
			}
			advancedS = true;

			return s == t;
		}

		private bool ShouldWriteNextByteFromTarget(out int s, out int t, out bool advancedS, out bool advancedT) {
			s = -1;
			advancedS = false;
			advancedT = false;

			t = Target.ReadByte();
			if (t == -1) {
				// target is eof
				return false;
			}
			advancedT = true;

			s = Source.ReadByte();
			if (s == -1) {
				// source is eof
				return true;
			}
			advancedS = true;

			return s != t;
		}

		public static void CreatePatch(Stream source, Stream target, Stream binout) {
			new CreateSimplest(source, target, binout).CreatePatchInternal();
		}
	}
}
