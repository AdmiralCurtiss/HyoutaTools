using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Patches.Bps {
	public static class BpsUtils {
		public static ulong ReadUnsignedNumber(this Stream s) {
			ulong data = 0;
			ulong shift = 1;
			while (true) {
				int x = s.ReadByte();
				if (x == -1) {
					throw new Exception("Reached end of stream while decoding BPS number.");
				}
				data += ((ulong)(x & 0x7f)) * shift;
				if ((x & 0x80) != 0) {
					break;
				}
				shift <<= 7;
				data += shift;
			}
			return data;
		}

		public static long ReadSignedNumber(this Stream s) {
			ulong n = s.ReadUnsignedNumber();
			long v = (long)(n >> 1);
			return ((n & 1) != 0) ? -v : v;
		}

		public static (Action action, ulong length) ReadAction(this Stream s) {
			ulong n = s.ReadUnsignedNumber();
			ulong command = n & 3;
			ulong length = (n >> 2) + 1;
			switch (command) {
				case 0: return (Action.SourceRead, length);
				case 1: return (Action.TargetRead, length);
				case 2: return (Action.SourceCopy, length);
				case 3: return (Action.TargetCopy, length);
			}
			throw new Exception("should never reach here");
		}
	}

	public enum Action {
		SourceRead,
		TargetRead,
		SourceCopy,
		TargetCopy,
	}
}
