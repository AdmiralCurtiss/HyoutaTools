using HyoutaPluginBase;
using HyoutaUtils;
using HyoutaUtils.Streams;
using System;
using System.Collections.Generic;

namespace HyoutaTools.Tales.Graces {
	public class CharNameBin {
		public List<CharNameBinSection> Sections;
		public Dictionary<int, (ushort reg, ushort alt)> IdToScsMappings;
		public SCS.SCS Scs;

		public CharNameBin(List<CharNameBinSection> sections, Dictionary<int, (ushort reg, ushort alt)> idToScsMappings, SCS.SCS scs) {
			Sections = new List<CharNameBinSection>(sections.Count);
			foreach (CharNameBinSection s in sections) {
				Sections.Add(new CharNameBinSection() { NumberStart = s.NumberStart, NumberCount = s.NumberCount, Offset = 0 });
			}
			IdToScsMappings = new Dictionary<int, (ushort reg, ushort alt)>(idToScsMappings);
			Scs = new SCS.SCS(scs.Entries);
		}

		public CharNameBin(DuplicatableStream stream, EndianUtils.Endianness endian = EndianUtils.Endianness.BigEndian, BitUtils.Bitness bits = BitUtils.Bitness.B32, TextUtils.GameTextEncoding encoding = TextUtils.GameTextEncoding.ShiftJIS) {
			using (DuplicatableStream s = stream.Duplicate()) {
				s.ReStart();
				uint sectionCount = s.ReadUInt32().FromEndian(endian);
				uint scsOffset = s.ReadUInt32().FromEndian(endian);
				Sections = new List<CharNameBinSection>((int)sectionCount);
				for (uint i = 0; i < sectionCount; ++i) {
					CharNameBinSection sec = new CharNameBinSection();
					sec.NumberStart = s.ReadUInt16().FromEndian(endian);
					sec.NumberCount = s.ReadUInt16().FromEndian(endian);
					sec.Offset = s.ReadUInt32().FromEndian(endian);
					Sections.Add(sec);
				}

				IdToScsMappings = new Dictionary<int, (ushort reg, ushort alt)>();
				for (int i = 0; i < Sections.Count; ++i) {
					s.Position = Sections[i].Offset;
					for (int j = 0; j < Sections[i].NumberCount; ++j) {
						int n = Sections[i].NumberStart + j;
						ushort reg = s.ReadUInt16().FromEndian(endian);
						ushort alt = s.ReadUInt16().FromEndian(endian);
						if (IdToScsMappings.ContainsKey(n)) {
							Console.WriteLine("WARNING: Multiple mappings for character ID " + n);
						}
						IdToScsMappings.Add(n, (reg, alt));
					}
				}

				Scs = new SCS.SCS(new PartialStream(s, scsOffset, s.Length - scsOffset), endian, bits, encoding);
			}
		}

		public (string regular, string alt) GetName(int id) {
			var m = IdToScsMappings[id];
			string r = m.reg == 0 ? null : Scs.Entries[m.reg - 1];
			string a = m.alt == 0 ? null : Scs.Entries[m.alt - 1];
			return (r, a);
		}

		public Dictionary<int, (string regular, string alt)> GenerateNameMap() {
			var map = new Dictionary<int, (string regular, string alt)>();
			foreach (var kvp in IdToScsMappings) {
				map.Add(kvp.Key, GetName(kvp.Key));
			}
			return map;
		}

		public System.IO.MemoryStream GenerateFile(EndianUtils.Endianness endian = EndianUtils.Endianness.BigEndian, BitUtils.Bitness bits = BitUtils.Bitness.B32, TextUtils.GameTextEncoding encoding = TextUtils.GameTextEncoding.ShiftJIS) {
			System.IO.MemoryStream s = new System.IO.MemoryStream();
			uint sectionCount = (uint)Sections.Count;
			s.WriteUInt32(sectionCount.ToEndian(endian));
			s.WriteUInt32(0); // scsOffset, fill in later
			for (uint i = 0; i < sectionCount; ++i) {
				CharNameBinSection sec = Sections[(int)i];
				s.WriteUInt16(sec.NumberStart.ToEndian(endian));
				s.WriteUInt16(sec.NumberCount.ToEndian(endian));
				s.WriteUInt32(0); // offset, fill in later
			}

			for (uint i = 0; i < sectionCount; ++i) {
				CharNameBinSection sec = Sections[(int)i];
				long p = s.Position;
				s.Position = 12 + i * 8;
				s.WriteUInt32(((uint)p).ToEndian(endian)); // fill in offset
				s.Position = p;
				for (int j = 0; j < sec.NumberCount; ++j) {
					var m = IdToScsMappings[sec.NumberStart + j];
					s.WriteUInt16(m.reg.ToEndian(endian));
					s.WriteUInt16(m.alt.ToEndian(endian));
				}
			}

			{
				long p = s.Position;
				s.Position = 4;
				s.WriteUInt32(((uint)p).ToEndian(endian)); // fill in scsOffset
				s.Position = p;
			}

			System.IO.Stream scsstream = Scs.WriteToScs(endian, bits, encoding);
			scsstream.Position = 0;
			StreamUtils.CopyStream(scsstream, s, scsstream.Length);

			s.Position = 0;
			return s;
		}
	}

	public class CharNameBinSection {
		public ushort NumberStart;
		public ushort NumberCount;
		public uint Offset;
	}
}
