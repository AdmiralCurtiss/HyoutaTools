using HyoutaPluginBase;
using HyoutaUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HyoutaUtils.EndianUtils;

namespace HyoutaTools.Tales.CPK {
	public class CpkFile {
		public DuplicatableStream FileStream;
		public uint ID;
		public uint DecompressedSize;
		public string Directory;
		public string Name;

		public int WriteOrderPriority = 0;
	}

	public class CpkBuilder {
		public byte[] UnknownCpkHeaderBytes;
		public UtfBuilder MainUtfTable;
		public byte[] UnknownPostCpkHeaderBytes;
		public byte[] CopyrightText;

		public byte[] UnknownTocHeaderBytes;
		public UtfBuilder TocUtfTable;

		public byte[] UnknownItocHeaderBytes;
		public UtfBuilder ItocUtfTable;

		public byte[] UnknownEtocHeaderBytes;
		public UtfBuilder EtocUtfTable;

		public byte[] UnknownGtocHeaderBytes;
		public UtfBuilder GtocUtfTable;

		public List<CpkFile> Files;

		// initialize from existing CPK
		public CpkBuilder(DuplicatableStream stream, Endianness endian = Endianness.BigEndian) {
			// should start with CPK and some unknown header bytes
			long cpkHeaderOffset = stream.Position;
			uint cpkMagic = stream.ReadUInt32(Endianness.LittleEndian);
			if (cpkMagic != 0x204B5043) {
				throw new Exception("wrong CPK magic");
			}

			UnknownCpkHeaderBytes = stream.ReadBytes(12);

			// main UTF table should follow
			MainUtfTable = new UtfBuilder(stream, endian);
			if (MainUtfTable.Rows.Count != 1) {
				throw new Exception("wrong rowcount in main UTF table");
			}

			UnknownPostCpkHeaderBytes = stream.ReadBytes(4);
			while (stream.ReadByte() == 0) ;
			stream.Position = stream.Position - 1;
			CopyrightText = stream.ReadBytes(stream.Position.Align(0x800, cpkHeaderOffset) - stream.Position); // or something

			ulong tocOffset = (ulong)MainUtfTable.Rows[0].Cells[MainUtfTable.FindColumnIndex("TocOffset")].Data;
			//ulong tocSize = (ulong)MainUtfTable.Rows[0].Cells[MainUtfTable.FindColumnIndex("TocSize")].Data;
			if (tocOffset != 0) {
				stream.Position = cpkHeaderOffset + (long)tocOffset;
				uint magic = stream.ReadUInt32(Endianness.LittleEndian);
				if (magic != 0x20434F54) {
					throw new Exception("wrong TOC magic");
				}
				UnknownTocHeaderBytes = stream.ReadBytes(12);
				TocUtfTable = new UtfBuilder(stream, endian);
			}

			ulong itocOffset = (ulong)MainUtfTable.Rows[0].Cells[MainUtfTable.FindColumnIndex("ItocOffset")].Data;
			//ulong itocSize = (ulong)MainUtfTable.Rows[0].Cells[MainUtfTable.FindColumnIndex("ItocSize")].Data;
			if (itocOffset != 0) {
				stream.Position = cpkHeaderOffset + (long)itocOffset;
				uint magic = stream.ReadUInt32(Endianness.LittleEndian);
				if (magic != 0x434F5449) {
					throw new Exception("wrong ITOC magic");
				}
				UnknownItocHeaderBytes = stream.ReadBytes(12);
				ItocUtfTable = new UtfBuilder(stream, endian);
			}

			ulong etocOffset = (ulong)MainUtfTable.Rows[0].Cells[MainUtfTable.FindColumnIndex("EtocOffset")].Data;
			//ulong etocSize = (ulong)MainUtfTable.Rows[0].Cells[MainUtfTable.FindColumnIndex("EtocSize")].Data;
			if (etocOffset != 0) {
				stream.Position = cpkHeaderOffset + (long)etocOffset;
				uint magic = stream.ReadUInt32(Endianness.LittleEndian);
				if (magic != 0x434F5445) {
					throw new Exception("wrong ETOC magic");
				}
				UnknownEtocHeaderBytes = stream.ReadBytes(12);
				EtocUtfTable = new UtfBuilder(stream, endian);
			}

			ulong gtocOffset = (ulong)MainUtfTable.Rows[0].Cells[MainUtfTable.FindColumnIndex("GtocOffset")].Data;
			//ulong gtocSize = (ulong)MainUtfTable.Rows[0].Cells[MainUtfTable.FindColumnIndex("GtocSize")].Data;
			if (gtocOffset != 0) {
				// haven't actually seen this, but would make sense...
				stream.Position = cpkHeaderOffset + (long)gtocOffset;
				uint magic = stream.ReadUInt32(Endianness.LittleEndian);
				if (magic != 0x434F5447) {
					throw new Exception("wrong GTOC magic");
				}
				UnknownGtocHeaderBytes = stream.ReadBytes(12);
				GtocUtfTable = new UtfBuilder(stream, endian);
			}

			ulong contentOffset = (ulong)MainUtfTable.Rows[0].Cells[MainUtfTable.FindColumnIndex("ContentOffset")].Data;
			//ulong contentSize = (ulong)MainUtfTable.Rows[0].Cells[MainUtfTable.FindColumnIndex("ContentSize")].Data;
			uint fileCount = (uint)MainUtfTable.Rows[0].Cells[MainUtfTable.FindColumnIndex("Files")].Data;

			if (TocUtfTable == null || TocUtfTable.Rows.Count != fileCount) {
				throw new Exception("invalid TOC?");
			}

			Files = new List<CpkFile>((int)fileCount);
			int columnDirName = TocUtfTable.FindColumnIndex("DirName");
			int columnFileName = TocUtfTable.FindColumnIndex("FileName");
			int columnFileSize = TocUtfTable.FindColumnIndex("FileSize");
			int columnExtractSize = TocUtfTable.FindColumnIndex("ExtractSize");
			int columnFileOffset = TocUtfTable.FindColumnIndex("FileOffset");
			int columnFileIndexId = TocUtfTable.FindColumnIndex("ID");
			long baseFileOffset = cpkHeaderOffset + ((contentOffset < tocOffset) ? (long)contentOffset : (long)tocOffset);

			for (int i = 0; i < (int)fileCount; ++i) {
				var f = new CpkFile();
				f.Directory = (string)TocUtfTable.Rows[i].Cells[columnDirName].Data;
				f.Name = (string)TocUtfTable.Rows[i].Cells[columnFileName].Data;
				uint fileSize = (uint)TocUtfTable.Rows[i].Cells[columnFileSize].Data;
				f.DecompressedSize = (uint)TocUtfTable.Rows[i].Cells[columnExtractSize].Data;
				ulong fileOffset = (ulong)TocUtfTable.Rows[i].Cells[columnFileOffset].Data;
				f.FileStream = new HyoutaUtils.Streams.PartialStream(stream, baseFileOffset + (long)fileOffset, fileSize);
				f.ID = (uint)TocUtfTable.Rows[i].Cells[columnFileIndexId].Data;
				Files.Add(f);
			}

			return;
		}

		private void VerifyFileIds() {
			bool[] definedIds = new bool[Files.Count];
			for (int i = 0; i < Files.Count; ++i) {
				definedIds[i] = false;
			}
			for (int i = 0; i < Files.Count; ++i) {
				if (definedIds[Files[i].ID]) {
					throw new Exception(string.Format("ID {0} defined multiple times", Files[i].ID));
				}
				definedIds[Files[i].ID] = true;
			}
			for (int i = 0; i < Files.Count; ++i) {
				if (!definedIds[i]) {
					throw new Exception(string.Format("ID {0} not defined at all", i));
				}
			}
		}

		private int[] MakeFileIdToTocIndexMap() {
			int[] map = new int[Files.Count];
			for (int i = 0; i < Files.Count; ++i) {
				map[Files[i].ID] = i;
			}
			return map;
		}

		private void SyncToc() {
			var toc = TocUtfTable;
			if (toc == null) {
				return;
			}

			int dirNameCol = toc.FindColumnIndex("DirName");
			int fileNameCol = toc.FindColumnIndex("FileName");
			int fileSizeCol = toc.FindColumnIndex("FileSize");
			int extractSizeCol = toc.FindColumnIndex("ExtractSize");
			int fileOffsetCol = toc.FindColumnIndex("FileOffset");
			if (toc.Rows.Count != Files.Count) {
				throw new Exception("inconsistent row count");
			}

			for (int i = 0; i < toc.Rows.Count; ++i) {
				toc.Rows[i].Cells[dirNameCol].Data = Files[i].Directory;
				toc.Rows[i].Cells[fileNameCol].Data = Files[i].Name;
				toc.Rows[i].Cells[fileSizeCol].Data = (uint)Files[i].FileStream.Length;
				toc.Rows[i].Cells[extractSizeCol].Data = Files[i].DecompressedSize;
				toc.Rows[i].Cells[fileOffsetCol].Data = (ulong)0; // don't know this yet, fill in later
			}
		}
		private void WriteFilePositionsToToc(long baseFileOffset, long[] fileOffsets) {
			var toc = TocUtfTable;
			if (toc == null) {
				return;
			}

			int fileOffsetCol = toc.FindColumnIndex("FileOffset");
			if (toc.Rows.Count != Files.Count) {
				throw new Exception("inconsistent row count");
			}

			for (int i = 0; i < toc.Rows.Count; ++i) {
				toc.Rows[i].Cells[fileOffsetCol].Data = (ulong)(fileOffsets[i] - baseFileOffset);
			}
		}
		private void SyncIToc() {
			var toc = ItocUtfTable;
			if (toc == null) {
				return;
			}

			int idCol = toc.FindColumnIndex("ID");
			int tocIndexCol = toc.FindColumnIndex("TocIndex");
			if (toc.Rows.Count != Files.Count) {
				throw new Exception("inconsistent row count");
			}

			int[] map = MakeFileIdToTocIndexMap();
			for (int i = 0; i < toc.Rows.Count; ++i) {
				toc.Rows[i].Cells[idCol].Data = (uint)i;
				toc.Rows[i].Cells[tocIndexCol].Data = (uint)map[i];
			}
		}
		private void SyncEToc() {
			var toc = EtocUtfTable;
			if (toc == null) {
				return;
			}

			int localDirCol = toc.FindColumnIndex("LocalDir");
			if (toc.Rows.Count != Files.Count + 1) {
				throw new Exception("inconsistent row count");
			}

			for (int i = 0; i < Files.Count; ++i) {
				toc.Rows[i].Cells[localDirCol].Data = Files[i].Directory;
			}
		}

		private void PrintUtf(StringBuilder sb, UtfBuilder utf) {
			for (int i = 0; i < utf.Rows.Count; ++i) {
				for (int j = 0; j < utf.Columns.Count; ++j) {
					sb.AppendLine(string.Format("{0} -> {1} ({2})", utf.Columns[j].Name, utf.Rows[i].Cells[j].Data, (utf.Rows[i].Cells[j].Data).GetType()));
				}
				sb.AppendLine("--");
			}
			sb.AppendLine("-----");
		}

		public string Print() {
			StringBuilder sb = new StringBuilder();
			PrintUtf(sb, MainUtfTable);
			PrintUtf(sb, TocUtfTable);
			PrintUtf(sb, ItocUtfTable);
			PrintUtf(sb, EtocUtfTable);
			return sb.ToString();
		}

		public void WriteToHeader(string columnName, object value) {
			var toc = MainUtfTable;
			int col = toc.FindColumnIndex(columnName);
			byte type = toc.Columns[col].Type;
			switch (type & utf_tab_sharp.UtfTab.COLUMN_STORAGE_MASK) {
				case utf_tab_sharp.UtfTab.COLUMN_STORAGE_CONSTANT:
					toc.Columns[col].Data = value;
					break;
				case utf_tab_sharp.UtfTab.COLUMN_STORAGE_PERROW:
					toc.Rows[0].Cells[col].Data = value;
					break;
				default:
					throw new Exception("can't write to column " + columnName);
			}
		}

		public void Build(Stream outstream, Endianness endian = Endianness.BigEndian) {
			VerifyFileIds();
			SyncToc();
			SyncIToc();
			SyncEToc();

			ushort alignment = (ushort)MainUtfTable.Rows[0].Cells[MainUtfTable.FindColumnIndex("Align")].Data;

			var faketoc = BuildUtfChunk(endian, 0x20434F54u, UnknownTocHeaderBytes, TocUtfTable); // this is the right length but lacks the file offsets
			var itoc = BuildUtfChunk(endian, 0x434F5449u, UnknownItocHeaderBytes, ItocUtfTable);
			var etoc = BuildUtfChunk(endian, 0x434F5445u, UnknownEtocHeaderBytes, EtocUtfTable);
			var gtoc = BuildUtfChunk(endian, 0x434F5447u, UnknownGtocHeaderBytes, GtocUtfTable);

			long baseAddress = outstream.Position;

			// reserve space for main utf header chunk
			outstream.WriteZeros(0x800);

			// reserve space for TOC
			long tocpos = outstream.Position - baseAddress;
			outstream.WriteZeros(faketoc.Length);
			long tocsize = (outstream.Position - baseAddress) - tocpos;
			outstream.WriteAlign(alignment, 0, baseAddress);

			// no idea where the GTOC is supposed to go but assume here
			long gtocpos = 0;
			long gtocsize = 0;
			if (gtoc != null) {
				gtocpos = outstream.Position - baseAddress;
				StreamUtils.CopyStream(gtoc, outstream);
				gtocsize = (outstream.Position - baseAddress) - gtocpos;
				outstream.WriteAlign(alignment, 0, baseAddress);
			}

			// write actual ITOC
			long itocpos = 0;
			long itocsize = 0;
			if (itoc != null) {
				itocpos = outstream.Position - baseAddress;
				StreamUtils.CopyStream(itoc, outstream);
				itocsize = (outstream.Position - baseAddress) - itocpos;
				outstream.WriteAlign(alignment, 0, baseAddress);
			}

			// write actual ETOC
			long etocpos = 0;
			long etocsize = 0;
			if (etoc != null) {
				etocpos = outstream.Position - baseAddress;
				StreamUtils.CopyStream(etoc, outstream);
				etocsize = (outstream.Position - baseAddress) - etocpos;
				outstream.WriteAlign(alignment, 0, baseAddress);
			}

			// write files
			long[] fileOffsets = new long[Files.Count];
			outstream.WriteAlign(alignment, 0, baseAddress);
			long contentOffset = outstream.Position - baseAddress;
			int[] fileIdMap = MakeFileIdToTocIndexMap();
			foreach (int prio in GetFilePriorities()) {
				for (int i = 0; i < Files.Count; ++i) {
					var file = Files[fileIdMap[i]];
					if (file.WriteOrderPriority == prio) {
						fileOffsets[fileIdMap[i]] = outstream.Position - baseAddress;
						var s = Files[fileIdMap[i]].FileStream;
						s.Position = 0;
						StreamUtils.CopyStream(s, outstream);
						outstream.WriteAlign(alignment, 0, baseAddress);
					}
				}
			}
			long contentSize = (outstream.Position - baseAddress) - contentOffset;

			long endpos = outstream.Position;

			// update and write TOC
			long baseFileOffset = ((contentOffset < tocpos) ? (long)contentOffset : (long)tocpos);
			WriteFilePositionsToToc(baseFileOffset, fileOffsets);

			var toc = BuildUtfChunk(endian, 0x20434F54u, UnknownTocHeaderBytes, TocUtfTable);
			outstream.Position = baseAddress + tocpos;
			toc.Position = 0;
			StreamUtils.CopyStream(toc, outstream);

			// update and write header
			WriteToHeader("ContentOffset", (ulong)contentOffset);
			WriteToHeader("ContentSize", (ulong)contentSize);
			if (tocpos != 0) {
				WriteToHeader("TocOffset", (ulong)tocpos);
				WriteToHeader("TocSize", (ulong)tocsize);
			}
			if (etocpos != 0) {
				WriteToHeader("EtocOffset", (ulong)etocpos);
				WriteToHeader("EtocSize", (ulong)etocsize);
			}
			if (itocpos != 0) {
				WriteToHeader("ItocOffset", (ulong)itocpos);
				WriteToHeader("ItocSize", (ulong)itocsize);
			}
			if (gtocpos != 0) {
				WriteToHeader("GtocOffset", (ulong)gtocpos);
				WriteToHeader("GtocSize", (ulong)gtocsize);
			}
			// not sure what these actually mean...?
			//WriteToHeader("EnabledPackedSize", (ulong)???);
			//WriteToHeader("EnabledDataSize", (ulong))???);
			WriteToHeader("Files", (uint)Files.Count);
			WriteToHeader("Align", (ushort)alignment);
			outstream.Position = baseAddress;
			var header = BuildUtfChunk(endian, 0x204B5043u, UnknownCpkHeaderBytes, MainUtfTable);
			header.Position = 0;
			StreamUtils.CopyStream(header, outstream);
			if (UnknownPostCpkHeaderBytes != null) {
				outstream.Write(UnknownPostCpkHeaderBytes);
			}
			if (CopyrightText != null) {
				outstream.Position = baseAddress + 0x800 - CopyrightText.Length;
				outstream.Write(CopyrightText);
			}

			outstream.Position = endpos;
		}

		private int[] GetFilePriorities() {
			var set = new SortedSet<int>();
			for (int i = 0; i < Files.Count; ++i) {
				set.Add(Files[i].WriteOrderPriority);
			}
			return set.ToArray();
		}

		private Stream BuildUtfChunk(Endianness endian, uint magic, byte[] unknownHeaderBytes, UtfBuilder utf) {
			if (unknownHeaderBytes == null || utf == null) {
				return null;
			}

			MemoryStream ms = new MemoryStream();
			ms.WriteUInt32(magic, Endianness.LittleEndian);
			ms.Write(unknownHeaderBytes);
			utf.Build(ms, endian);
			ms.Position = 0;
			return ms;
		}
	}
}
