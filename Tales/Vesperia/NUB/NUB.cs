using HyoutaPluginBase;
using HyoutaUtils;
using HyoutaUtils.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.NUB {
	public class NUB {
		public static void ExtractNub(DuplicatableStream duplicatableStream, string targetFolder, EndianUtils.Endianness e) {
			Directory.CreateDirectory(targetFolder);
			using (var stream = duplicatableStream.Duplicate()) {
				stream.Position = 0;
				var header = new NubHeader(stream, e);

				stream.Position = header.StartOfEntries;
				uint[] entries = stream.ReadUInt32Array(header.EntryCount, e);

				for (long i = 0; i < entries.LongLength; ++i) {
					uint entryLoc = entries[i];
					stream.Position = entryLoc;
					uint type = stream.ReadUInt32(EndianUtils.Endianness.LittleEndian);

					switch (type) {
						case 0x34317369: {
							stream.Position = entryLoc + 0x14;
							uint length = stream.ReadUInt32(e);
							uint offset = stream.ReadUInt32(e);
							stream.Position = entryLoc + 0xbc;
							byte[] bnsfheader = stream.ReadUInt8Array(0x30);

							stream.Position = header.StartOfFiles + offset;
							using (var fs = new FileStream(Path.Combine(targetFolder, i.ToString("D8") + ".bnsf"), FileMode.Create)) {
								fs.Write(bnsfheader);
								StreamUtils.CopyStream(stream, fs, length);
							}
						}
						break;
						case 0x337461: {
							stream.Position = entryLoc;
							byte[] at3header = stream.ReadUInt8Array(0x100);

							stream.Position = entryLoc + 0x14;
							uint length = stream.ReadUInt32(e);
							uint offset = stream.ReadUInt32(e);
							stream.Position = header.StartOfFiles + offset;

							using (var fs = new FileStream(Path.Combine(targetFolder, i.ToString("D8") + ".at3"), FileMode.Create)) {
								fs.Write(at3header);
								StreamUtils.CopyStream(stream, fs, length);
							}
						}
						break;
						case 0x676176: {
							stream.Position = entryLoc;
							byte[] vagheader = stream.ReadUInt8Array(0xc0);

							stream.Position = entryLoc + 0x14;
							uint length = stream.ReadUInt32(e);
							uint offset = stream.ReadUInt32(e);
							stream.Position = header.StartOfFiles + offset;

							using (var fs = new FileStream(Path.Combine(targetFolder, i.ToString("D8") + ".vag"), FileMode.Create)) {
								fs.Write(vagheader);
								fs.WriteUInt64(0);
								fs.WriteUInt64(0);
								StreamUtils.CopyStream(stream, fs, length);
							}
						}
						break;
						default:
							Console.WriteLine("Unimplemented nub subtype: 0x" + type.ToString("x8"));
							break;
					}

				}

				return;
			}
		}

		public static void RebuildNub(DuplicatableStream duplicatableStream, string infolder, string outpath, EndianUtils.Endianness e) {
			using (var outstream = new FileStream(outpath, FileMode.Create)) {
				RebuildNub(duplicatableStream, infolder, outstream, e);
			}
		}

		public static void RebuildNub(DuplicatableStream duplicatableStream, string infolder, Stream outstream, EndianUtils.Endianness e) {
			outstream.Position = 0;
			using (var stream = duplicatableStream.Duplicate()) {
				stream.Position = 0;
				var header = new NubHeader(stream, e);

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
	}

	public class NubHeader {
		public ulong Magic;
		public uint Fileid; // or something like that? seems unique per archive in each game
		public uint EntryCount;
		public uint StartOfFiles;
		public uint FilesSize;
		public uint StartOfEntries;
		public uint StartOfHeaders;

		public NubHeader(DuplicatableStream stream, EndianUtils.Endianness e) {
			Magic = stream.ReadUInt64(EndianUtils.Endianness.LittleEndian);
			if (Magic != 0x10200) {
				throw new Exception("unexpected magic in NUB");
			}

			Fileid = stream.ReadUInt32(e); // or something like that? seems unique per archive in each game
			EntryCount = stream.ReadUInt32(e);
			StartOfFiles = stream.ReadUInt32(e);
			FilesSize = stream.ReadUInt32(e);
			StartOfEntries = stream.ReadUInt32(e);
			StartOfHeaders = stream.ReadUInt32(e);
		}
	}
}
