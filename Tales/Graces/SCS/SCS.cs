using HyoutaPluginBase;
using HyoutaUtils;
using HyoutaUtils.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HyoutaUtils.BitUtils;
using static HyoutaUtils.EndianUtils;
using static HyoutaUtils.TextUtils;

namespace HyoutaTools.Tales.Graces.SCS {
	public class SCS {
		public List<string> Entries;

		public SCS(List<string> entries) {
			this.Entries = entries;
		}

		public SCS(string filename, Endianness endian = Endianness.BigEndian, Bitness bits = Bitness.B32, GameTextEncoding encoding = GameTextEncoding.ShiftJIS)
			: this(new DuplicatableFileStream(filename), endian, bits, encoding) { }

		public SCS(DuplicatableStream stream, Endianness endian = Endianness.BigEndian, Bitness bits = Bitness.B32, GameTextEncoding encoding = GameTextEncoding.ShiftJIS) {
			ulong entryCount = stream.ReadUInt(bits, endian);
			Entries = new List<string>((int)entryCount);
			for (uint i = 0; i < entryCount; ++i) {
				ulong offset = stream.ReadUInt(bits, endian);
				if (offset != 0) {
					Entries.Add(stream.ReadNulltermStringFromLocationAndReset(offset, encoding));
				} else {
					Entries.Add(null);
				}
			}
		}

		public System.IO.Stream WriteToScs(Endianness endian = Endianness.BigEndian, Bitness bits = Bitness.B32, GameTextEncoding encoding = GameTextEncoding.ShiftJIS) {
			System.IO.Stream stream = new System.IO.MemoryStream();
			uint entryCount = (uint)Entries.Count;
			stream.WriteUInt32(entryCount.ToEndian(endian));
			for (uint i = 0; i < entryCount; ++i) {
				stream.WriteUInt32(0); // reserve space
			}
			List<long> positions = new List<long>();
			for (uint i = 0; i < entryCount; ++i) {
				string s = Entries[(int)i];
				if (s != null) {
					positions.Add(stream.Position);
					stream.WriteNulltermString(s, encoding);
				} else {
					positions.Add(0);
				}
			}
			stream.Position = 4;
			for (uint i = 0; i < entryCount; ++i) {
				stream.WriteUInt32(((uint)positions[(int)i]).ToEndian(endian));
			}
			stream.Position = 0;
			return stream;
		}

		public static int DecodeDigit(char c) {
			switch (c) {
				case '0': return 0;
				case '1': return 1;
				case '2': return 2;
				case '3': return 3;
				case '4': return 4;
				case '5': return 5;
				case '6': return 6;
				case '7': return 7;
				case '8': return 8;
				case '9': return 9;
				case 'a': return 10;
				case 'b': return 11;
				case 'c': return 12;
				case 'd': return 13;
				case 'e': return 14;
				case 'f': return 15;
				case 'g': return 16;
				case 'h': return 17;
				case 'i': return 18;
				case 'j': return 19;
				case 'k': return 20;
				case 'l': return 21;
				case 'm': return 22;
				case 'n': return 23;
				case 'o': return 24;
				case 'p': return 25;
				case 'q': return 26;
				case 'r': return 27;
				case 's': return 28;
				case 't': return 29;
				case 'u': return 30;
				case 'v': return 31;
				case 'w': return 32;
				case 'x': return 33;
				case 'y': return 34;
				case 'z': return 35;
				case 'A': return 36;
				case 'B': return 37;
				case 'C': return 38;
				case 'D': return 39;
				case 'E': return 40;
				case 'F': return 41;
				case 'G': return 42;
				case 'H': return 43;
				case 'I': return 44;
				case 'J': return 45;
				case 'K': return 46;
				case 'L': return 47;
				case 'M': return 48;
				case 'N': return 49;
				case 'O': return 50;
				case 'P': return 51;
				case 'Q': return 52;
				case 'R': return 53;
				case 'S': return 54;
				case 'T': return 55;
				case 'U': return 56;
				case 'V': return 57;
				case 'W': return 58;
				case 'X': return 59;
				case 'Y': return 60;
				case 'Z': return 61;
				case '~': return 62;
				case '_': return 63;
			}
			throw new Exception("?");
		}

		public static char EncodeDigit(int d) {
			switch (d) {
				case 0: return '0';
				case 1: return '1';
				case 2: return '2';
				case 3: return '3';
				case 4: return '4';
				case 5: return '5';
				case 6: return '6';
				case 7: return '7';
				case 8: return '8';
				case 9: return '9';
				case 10: return 'a';
				case 11: return 'b';
				case 12: return 'c';
				case 13: return 'd';
				case 14: return 'e';
				case 15: return 'f';
				case 16: return 'g';
				case 17: return 'h';
				case 18: return 'i';
				case 19: return 'j';
				case 20: return 'k';
				case 21: return 'l';
				case 22: return 'm';
				case 23: return 'n';
				case 24: return 'o';
				case 25: return 'p';
				case 26: return 'q';
				case 27: return 'r';
				case 28: return 's';
				case 29: return 't';
				case 30: return 'u';
				case 31: return 'v';
				case 32: return 'w';
				case 33: return 'x';
				case 34: return 'y';
				case 35: return 'z';
				case 36: return 'A';
				case 37: return 'B';
				case 38: return 'C';
				case 39: return 'D';
				case 40: return 'E';
				case 41: return 'F';
				case 42: return 'G';
				case 43: return 'H';
				case 44: return 'I';
				case 45: return 'J';
				case 46: return 'K';
				case 47: return 'L';
				case 48: return 'M';
				case 49: return 'N';
				case 50: return 'O';
				case 51: return 'P';
				case 52: return 'Q';
				case 53: return 'R';
				case 54: return 'S';
				case 55: return 'T';
				case 56: return 'U';
				case 57: return 'V';
				case 58: return 'W';
				case 59: return 'X';
				case 60: return 'Y';
				case 61: return 'Z';
				case 62: return '~';
				case 63: return '_';
			}
			throw new Exception("?");
		}

		public static int DecodeNumber(string t) {
			if (t.StartsWith("$")) {
				int result = 0;
				for (int i = t.Length - 1; i > 0; --i) {
					result = result * 64 + DecodeDigit(t[i]);
				}
				return result;
			} else {
				return int.Parse(t);
			}
		}

		public static string EncodeNumber(int n) {
			if (n < 0) {
				throw new Exception("don't know how to encode negative numbers");
			}
			if (n < 10) {
				return n.ToString("D");
			}
			return "$" + EncodeNumberInternal(n);
		}

		private static string EncodeNumberInternal(int n) {
			if (n < 64) {
				return EncodeDigit(n).ToString();
			}

			int div = n / 64;
			int mod = n % 64;
			return EncodeDigit(mod).ToString() + EncodeNumberInternal(div);
		}
	}
}
