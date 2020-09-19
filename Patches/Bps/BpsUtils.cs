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

		public static ulong EncodeSignedNumber(long v) {
			bool negative = v < 0;
			if (negative) {
				return (((ulong)-v) << 1) | 1ul;
			} else {
				return (((ulong)v) << 1);
			}
		}

		public static ulong EncodeAction(Action action, ulong length) {
			switch (action) {
				case Action.SourceRead: return 0ul | ((length - 1) << 2);
				case Action.TargetRead: return 1ul | ((length - 1) << 2);
				case Action.SourceCopy: return 2ul | ((length - 1) << 2);
				case Action.TargetCopy: return 3ul | ((length - 1) << 2);
			}
			throw new Exception("should never reach here");
		}

		public static void WriteUnsignedNumber(this Stream s, ulong d) {
			ulong data = d;
			while (true) {
				byte x = (byte)(data & 0x7f);
				data >>= 7;
				if (data == 0) {
					s.WriteByte((byte)(0x80 | x));
					break;
				}
				s.WriteByte(x);
				--data;
			}
		}

		public static void WriteSignedNumber(this Stream s, long d) {
			s.WriteUnsignedNumber(EncodeSignedNumber(d));
		}

		public static void WriteAction(this Stream s, Action action, ulong length) {
			s.WriteUnsignedNumber(EncodeAction(action, length));
		}
	}

	public enum Action {
		SourceRead,
		TargetRead,
		SourceCopy,
		TargetCopy,
	}
}
