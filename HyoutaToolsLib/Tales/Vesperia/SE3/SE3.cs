using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaPluginBase;
using HyoutaUtils;
using HyoutaUtils.Streams;

namespace HyoutaTools.Tales.Vesperia.SE3 {
	public class SE3 {
		public SE3(string filename, EndianUtils.Endianness? endian, TextUtils.GameTextEncoding encoding) {
			if (!LoadFile(new DuplicatableFileStream(filename), endian, encoding)) {
				throw new Exception("Loading SE3 failed!");
			}
		}

		public SE3(DuplicatableStream stream, EndianUtils.Endianness? endian, TextUtils.GameTextEncoding encoding) {
			if (!LoadFile(stream, endian, encoding)) {
				throw new Exception("Loading SE3 failed!");
			}
		}

		private uint DataBegin;
		public uint FileCount { get; private set; }
		public List<string> Filenames { get; private set; }

		private DuplicatableStream Data;
		public EndianUtils.Endianness Endian { get; private set; }

		private bool LoadFile(DuplicatableStream inputStream, EndianUtils.Endianness? endianParam, TextUtils.GameTextEncoding encoding) {
			DuplicatableStream stream = inputStream.Duplicate();
			stream.Position = 0;
			EndianUtils.Endianness endian;
			if (endianParam == null) {
				uint magic = stream.ReadUInt32().FromEndian(EndianUtils.Endianness.BigEndian);
				if (magic == 0x53453320) {
					endian = EndianUtils.Endianness.BigEndian;
				} else if (magic == 0x20334553) {
					endian = EndianUtils.Endianness.LittleEndian;
				} else {
					Console.WriteLine("Invalid magic: " + magic);
					return false;
				}
			} else {
				endian = endianParam.Value;
				uint magic = stream.ReadUInt32(endian);
				if (magic != 0x53453320) {
					Console.WriteLine("Invalid magic: " + magic);
					return false;
				}
			}

			this.Endian = endian;

			uint lengthOfFilenameSection = stream.ReadUInt32(endian); // probably?
			uint startOfFilenameSection = stream.ReadUInt32(endian); // probably?
			DataBegin = stream.ReadUInt32(endian);

			stream.Position = startOfFilenameSection;
			uint magicOfFilenameSection = stream.ReadUInt32(endian);
			FileCount = stream.ReadUInt32(endian);
			Filenames = new List<string>((int)FileCount);
			for (uint i = 0; i < FileCount; ++i) {
				Filenames.Add(stream.ReadSizedString(48, encoding).TrimNull());
			}

			Data = stream;

			return true;
		}

		public DuplicatableStream ExtractSe3HeaderStream() {
			return new PartialStream(Data, 0, DataBegin);
		}

		public DuplicatableStream ExtractNubStream() {
			return new PartialStream(Data, DataBegin, Data.Length - DataBegin);
		}
	}
}
