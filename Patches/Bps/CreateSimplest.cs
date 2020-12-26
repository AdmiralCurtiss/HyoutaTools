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
				if (IsNextByteSameInSourceAndTarget()) {
					Source.ReadByte();
					Target.ReadByte();
					ulong count = 1;
					while (IsNextByteSameInSourceAndTarget()) {
						++count;
						Source.ReadByte();
						Target.ReadByte();
					}
					PatchOut.WriteAction(HyoutaUtils.Bps.Action.SourceRead, count);
				} else {
					long p = Target.Position;
					ulong count = 0;
					while (ShouldWriteNextByteFromTarget()) {
						++count;
						Source.ReadByte();
						Target.ReadByte();
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

		private bool IsNextByteSameInSourceAndTarget() {
			int t = Target.ReadByte();
			if (t == -1) {
				// target is eof
				return false;
			}
			Target.Position -= 1;

			int s = Source.ReadByte();
			if (s == -1) {
				// source is eof
				return false;
			}
			Source.Position -= 1;

			return s == t;
		}

		private bool ShouldWriteNextByteFromTarget() {
			int t = Target.ReadByte();
			if (t == -1) {
				// target is eof
				return false;
			}
			Target.Position -= 1;

			int s = Source.ReadByte();
			if (s == -1) {
				// source is eof
				return true;
			}
			Source.Position -= 1;

			return s != t;
		}

		public static void CreatePatch(Stream source, Stream target, Stream binout) {
			new CreateSimplest(source, target, binout).CreatePatchInternal();
		}
	}
}
