using HyoutaPluginBase;
using HyoutaPluginBase.FileContainer;
using HyoutaTools.FileContainer;
using HyoutaUtils;
using HyoutaUtils.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.NUB {
	public class NUB : FileContainer.ContainerBase {
		private DuplicatableStream Stream;
		private NubHeader Header;
		private EndianUtils.Endianness Endian;
		private uint[] Entries;

		public uint EntryCount => Header.EntryCount;

		public NUB(DuplicatableStream duplicatableStream, EndianUtils.Endianness? e) {
			Stream = duplicatableStream.Duplicate();
			Stream.Position = 0;
			Header = new NubHeader(Stream, e);
			Endian = Header.Endian;
			Stream.Position = Header.StartOfEntries;
			Entries = Stream.ReadUInt32Array(Header.EntryCount, Endian);
		}

		public static void ExtractNub(DuplicatableStream duplicatableStream, string targetFolder, EndianUtils.Endianness? e) {
			Directory.CreateDirectory(targetFolder);
			using (var nub = new NUB(duplicatableStream, e)) {
				nub.Extract(targetFolder);
			}
		}

		public void Extract(string targetFolder) {
			for (long i = 0; i < EntryCount; ++i) {
				var file = GetChildByIndex(i) as NubFile;
				if (file != null) {
					using (var ds = file.DataStream.Duplicate())
					using (var fs = new FileStream(Path.Combine(targetFolder, i.ToString("D8") + "." + file.Type), FileMode.Create)) {
						StreamUtils.CopyStream(ds, fs);
					}
				}
			}
		}

		public override INode GetChildByIndex(long index) {
			if (index >= 0 && index < Entries.LongLength) {
				uint entryLoc = Entries[index];
				Stream.Position = entryLoc;
				uint type = Stream.ReadUInt32(Endian);

				switch (type) {
					// PC vesperia can't agree with itself whether this is endian-agnostic or not, the JP files have it swapped compared to the EN files...
					case 0x69733134:
					case 0x34317369: {
						Stream.Position = entryLoc + 0x14;
						uint length = Stream.ReadUInt32(Endian);
						uint offset = Stream.ReadUInt32(Endian);
						Stream.Position = entryLoc + 0xbc;
						byte[] bnsfheader = Stream.ReadUInt8Array(0x30);
						var bnsfheaderstream = new DuplicatableByteArrayStream(bnsfheader);
						bnsfheaderstream.Position = 4;
						uint bnsflength = bnsfheaderstream.ReadUInt32(EndianUtils.Endianness.BigEndian);
						if (bnsfheaderstream.ReadUInt32(EndianUtils.Endianness.BigEndian) != 0x49533232) {
							bnsflength += 8;
						}

						Stream.Position = Header.StartOfFiles + offset;
						using (var ms = new MemoryStream()) {
							ms.Write(bnsfheader);
							StreamUtils.CopyStream(Stream, ms, Math.Min(length, bnsflength - bnsfheader.Length));
							return new NubFile(ms.CopyToByteArrayStreamAndDispose(), "bnsf");
						}
					}
					case 0x64737000: {
						Stream.Position = entryLoc + 0x14;
						uint length = Stream.ReadUInt32(Endian);
						uint offset = Stream.ReadUInt32(Endian);
						Stream.Position = entryLoc + 0xbc;
						byte[] dspheader = Stream.ReadUInt8Array(0x60);

						Stream.Position = Header.StartOfFiles + offset;
						using (var ms = new MemoryStream()) {
							ms.Write(dspheader);
							StreamUtils.CopyStream(Stream, ms, length);
							return new NubFile(ms.CopyToByteArrayStreamAndDispose(), "dsp");
						}
					}
					case 0x61743300: {
						Stream.Position = entryLoc;
						byte[] at3header = Stream.ReadUInt8Array(0x100);

						Stream.Position = entryLoc + 0x14;
						uint length = Stream.ReadUInt32(Endian);
						uint offset = Stream.ReadUInt32(Endian);
						Stream.Position = Header.StartOfFiles + offset;

						using (var ms = new MemoryStream()) {
							ms.Write(at3header);
							StreamUtils.CopyStream(Stream, ms, length);
							return new NubFile(ms.CopyToByteArrayStreamAndDispose(), "at3");
						}
					}
					case 0x76616700: {
						Stream.Position = entryLoc;
						byte[] vagheader = Stream.ReadUInt8Array(0xc0);

						Stream.Position = entryLoc + 0x14;
						uint length = Stream.ReadUInt32(Endian);
						uint offset = Stream.ReadUInt32(Endian);
						Stream.Position = Header.StartOfFiles + offset;

						using (var ms = new MemoryStream()) {
							if (Endian == EndianUtils.Endianness.LittleEndian) {
								// unclear if this needs to be all swapped or just a subset...
								for (int i = 0; i < 0xc0; i += 4) {
									ms.WriteByte(vagheader[i + 3]);
									ms.WriteByte(vagheader[i + 2]);
									ms.WriteByte(vagheader[i + 1]);
									ms.WriteByte(vagheader[i + 0]);
								}
							} else {
								ms.Write(vagheader);
							}
							ms.WriteUInt64(0);
							ms.WriteUInt64(0);
							StreamUtils.CopyStream(Stream, ms, length);
							return new NubFile(ms.CopyToByteArrayStreamAndDispose(), "vag");
						}
					}
					default:
						Console.WriteLine("Unimplemented nub subtype: 0x" + type.ToString("x8"));
						return null;
				}
			}
			return null;
		}

		public static void RebuildNub(DuplicatableStream duplicatableStream, string infolder, string outpath, EndianUtils.Endianness? e) {
			using (var outstream = new FileStream(outpath, FileMode.Create)) {
				RebuildNub(duplicatableStream, infolder, outstream, e);
			}
		}

		public static void RebuildNub(DuplicatableStream duplicatableStream, string infolder, Stream outstream, EndianUtils.Endianness? inEndian) {
			outstream.Position = 0;
			using (var stream = duplicatableStream.Duplicate()) {
				stream.Position = 0;
				var header = new NubHeader(stream, inEndian);
				var e = header.Endian;

				// copy all header stuff to outstream, we'll modify it later
				stream.Position = 0;
				StreamUtils.CopyStream(stream, outstream, header.StartOfFiles);

				stream.Position = header.StartOfEntries;
				uint[] entries = stream.ReadUInt32Array(header.EntryCount, e);
				for (long i = 0; i < entries.LongLength; ++i) {
					uint entryLoc = entries[i];
					stream.Position = entryLoc;
					uint type = stream.ReadUInt32(EndianUtils.Endianness.LittleEndian);

					switch (type) {
						case 0x34317369: {
							using (var fs = new DuplicatableFileStream(Path.Combine(infolder, i.ToString("D8") + ".bnsf"))) {
								byte[] bnsfheader = fs.ReadUInt8Array(0x30);

								// write file to outstream
								long filestart = outstream.Position;
								StreamUtils.CopyStream(fs, outstream, fs.Length - 0x30);
								outstream.WriteAlign(0x10);
								long fileend = outstream.Position;
								long filelen = fileend - filestart;

								// update headers
								outstream.Position = entryLoc + 0xbc;
								outstream.Write(bnsfheader);
								outstream.Position = entryLoc + 0x14;
								outstream.WriteUInt32((uint)filelen, e);
								outstream.WriteUInt32((uint)(filestart - header.StartOfFiles), e);

								outstream.Position = fileend;
							}
						}
						break;
						case 0x707364: {
							using (var fs = new DuplicatableFileStream(Path.Combine(infolder, i.ToString("D8") + ".dsp"))) {
								byte[] dspheader = fs.ReadUInt8Array(0x60);

								// write file to outstream
								long filestart = outstream.Position;
								StreamUtils.CopyStream(fs, outstream, fs.Length - 0x60);
								outstream.WriteAlign(0x10);
								long fileend = outstream.Position;
								long filelen = fileend - filestart;

								// update headers
								outstream.Position = entryLoc + 0xbc;
								outstream.Write(dspheader);
								outstream.Position = entryLoc + 0x14;
								outstream.WriteUInt32((uint)filelen, e);
								outstream.WriteUInt32((uint)(filestart - header.StartOfFiles), e);

								outstream.Position = fileend;
							}
						}
						break;
						default:
							Console.WriteLine("Unimplemented nub subtype (rebuild): 0x" + type.ToString("x8"));
							break;
					}
				}

				long filesSize = outstream.Position - header.StartOfFiles;
				outstream.Position = 0x14;
				outstream.WriteUInt32((uint)filesSize, e);
			}
		}

		public override void Dispose() {
			if (Stream != null) {
				Stream.Close();
				Stream.Dispose();
				Stream = null;
			}
		}

		public override INode GetChildByName(string name) {
			return null;
		}

		public override IEnumerable<string> GetChildNames() {
			return new List<string>();
		}
	}

	public class NubHeader {
		public ulong Magic;
		public uint Fileid; // or something like that? seems unique per archive in each game
		public uint EntryCount;
		public uint StartOfFiles;
		public uint FilesSize;
		public uint StartOfEntries;
		public uint StartOfHeaders;

		public EndianUtils.Endianness Endian; // not actually a field in the header, at least not here -- might be the first byte?

		public NubHeader(DuplicatableStream stream, EndianUtils.Endianness? endian) {
			if (endian.HasValue) {
				Init(stream, endian.Value);
			} else {
				Init(stream, EndianUtils.Endianness.BigEndian);
				if (StartOfEntries > 0xffff) {
					// probably little endian actually
					Fileid = Fileid.SwapEndian();
					EntryCount = EntryCount.SwapEndian();
					StartOfFiles = StartOfFiles.SwapEndian();
					FilesSize = FilesSize.SwapEndian();
					StartOfEntries = StartOfEntries.SwapEndian();
					StartOfHeaders = StartOfHeaders.SwapEndian();
					Endian = EndianUtils.Endianness.LittleEndian;
				}
			}
		}

		private void Init(DuplicatableStream stream, EndianUtils.Endianness e) {
			Magic = stream.ReadUInt64(EndianUtils.Endianness.LittleEndian);
			if (!(Magic == 0x10200 || Magic == 0x10201)) {
				throw new Exception("unexpected magic in NUB");
			}

			Fileid = stream.ReadUInt32(e); // or something like that? seems unique per archive in each game
			EntryCount = stream.ReadUInt32(e);
			StartOfFiles = stream.ReadUInt32(e);
			FilesSize = stream.ReadUInt32(e);
			StartOfEntries = stream.ReadUInt32(e);
			StartOfHeaders = stream.ReadUInt32(e);

			Endian = e;
		}
	}

	public class NubFile : FileFromStream {
		public string Type { get; private set; }

		public NubFile(DuplicatableStream stream, string type) : base(stream) {
			Type = type;
		}
	}
}
