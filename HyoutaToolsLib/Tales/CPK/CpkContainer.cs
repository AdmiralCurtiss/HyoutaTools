using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaPluginBase;
using HyoutaPluginBase.FileContainer;
using HyoutaUtils;

namespace HyoutaTools.Tales.CPK {
	public class CpkContainer : HyoutaPluginBase.FileContainer.IContainer {
		// NOTE: a decent amount of this is copy-pasted and adapted from CpkUnpack.analyze_CPK()
		// see also utf_tab license

		public bool IsFile => false;
		public bool IsContainer => true;
		public IFile AsFile => null;
		public IContainer AsContainer => this;

		private DuplicatableStream infile;
		private byte[] toc_string_table = null;
		public long CpkHeader_offset { get; private set; }
		public long toc_offset { get; private set; }
		public long content_offset { get; private set; }
		public long toc_entries { get; private set; }

		private Dictionary<string, int> filename_lookup = new Dictionary<string, int>();

		public DuplicatableStream DuplicateStream() {
			return infile.Duplicate();
		}

		public CpkContainer(DuplicatableStream stream, long CpkHeader_offset = 0x0) {
			this.CpkHeader_offset = CpkHeader_offset;
			infile = stream.Duplicate();

			// check header
			{
				byte[] buf = new byte[4];
				byte[] CPK_signature = Encoding.ASCII.GetBytes("CPK ");
				utf_tab_sharp.Util.get_bytes_seek(CpkHeader_offset, infile, buf, 4);
				utf_tab_sharp.ErrorStuff.CHECK_ERROR(!utf_tab_sharp.Util.memcmp(buf, CPK_signature), "CPK signature not found");
			}

			// check CpkHeader
			{
				utf_tab_sharp.utf_query_result result = utf_tab_sharp.UtfTab.query_utf_nofail(infile, CpkHeader_offset + 0x10, null);

				utf_tab_sharp.ErrorStuff.CHECK_ERROR(result.rows != 1, "wrong number of rows in CpkHeader");
			}

			// get TOC offset
			toc_offset = CpkHeader_offset + (long)utf_tab_sharp.UtfTab.query_utf_8byte(infile, CpkHeader_offset + 0x10, 0, "TocOffset");

			// get content offset
			content_offset = CpkHeader_offset + (long)utf_tab_sharp.UtfTab.query_utf_8byte(infile, CpkHeader_offset + 0x10, 0, "ContentOffset");

			// get file count from CpkHeader
			long CpkHeader_count = utf_tab_sharp.UtfTab.query_utf_4byte(infile, CpkHeader_offset + 0x10, 0, "Files");

			// check TOC header
			{
				byte[] buf = new byte[4];
				byte[] TOC_signature = Encoding.ASCII.GetBytes("TOC ");
				utf_tab_sharp.Util.get_bytes_seek(toc_offset, infile, buf, 4);
				utf_tab_sharp.ErrorStuff.CHECK_ERROR(!utf_tab_sharp.Util.memcmp(buf, TOC_signature), "TOC signature not found");
			}

			// get TOC entry count, string table
			{
				utf_tab_sharp.utf_query_result result = utf_tab_sharp.UtfTab.query_utf_nofail(infile, toc_offset + 0x10, null);

				toc_entries = result.rows;
				toc_string_table = utf_tab_sharp.UtfTab.load_utf_string_table(infile, toc_offset + 0x10);
			}

			// check that counts match
			utf_tab_sharp.ErrorStuff.CHECK_ERROR(toc_entries != CpkHeader_count, "CpkHeader file count and TOC entry count do not match");

			for (int i = 0; i < toc_entries; ++i) {
				string file_name = utf_tab_sharp.UtfTab.query_utf_string(infile, toc_offset + 0x10, i, "FileName", toc_string_table);
				string dir_name = utf_tab_sharp.UtfTab.query_utf_string(infile, toc_offset + 0x10, i, "DirName", toc_string_table);

				string full_name;
				if (string.IsNullOrEmpty(dir_name)) {
					full_name = file_name;
				} else {
					full_name = dir_name + "/" + file_name;
				}

				filename_lookup.Add(full_name, i);
			}
		}

		public class Entry {
			public string file_name;
			public string dir_name;
			public long file_size;
			public long extract_size;
			public long file_offset;

			public string name => string.IsNullOrEmpty(dir_name) ? file_name : dir_name + "/" + file_name;
			public bool compressed => extract_size > file_size;
		}

		public Entry GetEntryByIndex(long index) {
			if (index < 0 && index >= toc_entries) {
				return null;
			}

			int i = (int)index; // TODO: exceedingly unlikely but technically not impossible to have filecount over int32 max?

			// get file name
			string file_name = utf_tab_sharp.UtfTab.query_utf_string(infile, toc_offset + 0x10, i,
					"FileName", toc_string_table);

			// get directory name
			string dir_name = utf_tab_sharp.UtfTab.query_utf_string(infile, toc_offset + 0x10, i,
					"DirName", toc_string_table);

			// get file size
			long file_size = utf_tab_sharp.UtfTab.query_utf_4byte(infile, toc_offset + 0x10, i,
					"FileSize");

			// get extract size
			long extract_size = utf_tab_sharp.UtfTab.query_utf_4byte(infile, toc_offset + 0x10, i,
					"ExtractSize");

			// get file offset
			ulong file_offset_raw =
				utf_tab_sharp.UtfTab.query_utf_8byte(infile, toc_offset + 0x10, i, "FileOffset");
			if (content_offset < toc_offset) {
				file_offset_raw += (ulong)content_offset;
			} else {
				file_offset_raw += (ulong)toc_offset;
			}

			utf_tab_sharp.ErrorStuff.CHECK_ERROR(file_offset_raw > (ulong)long.MaxValue, "File offset too large, will be unable to seek");
			long file_offset = (long)file_offset_raw;

			return new Entry() { file_name = file_name, dir_name = dir_name, file_size = file_size, extract_size = extract_size, file_offset = file_offset };
		}

		public INode GetChildByIndex(long index) {
			Entry e = GetEntryByIndex(index);
			if (e == null) {
				return null;
			}

			if (e.extract_size > e.file_size) {
				System.IO.MemoryStream outfile = new System.IO.MemoryStream();

				long uncompressed_size =
					utf_tab_sharp.CpkUncompress.uncompress(infile, e.file_offset, e.file_size, outfile);

				utf_tab_sharp.ErrorStuff.CHECK_ERROR(uncompressed_size != e.extract_size,
						"uncompressed size != ExtractSize");

				outfile.Position = 0;
				byte[] data = outfile.ReadBytes(outfile.Length);
				return new FileContainer.FileFromStream(new HyoutaUtils.Streams.DuplicatableByteArrayStream(data));
			} else {
				return new FileContainer.FileFromStream(new HyoutaUtils.Streams.PartialStream(infile, e.file_offset, e.file_size));
			}
		}

		public int? GetChildIndexFromName(string name) {
			int v;
			if (filename_lookup.TryGetValue(name, out v)) {
				return v;
			}
			return null;
		}

		public INode GetChildByName(string name) {
			int? idx = GetChildIndexFromName(name);
			if (idx.HasValue) {
				return GetChildByIndex(idx.Value);
			}
			return null;
		}

		public string GetChildNameFromIndex(long i) {
			string file_name = utf_tab_sharp.UtfTab.query_utf_string(infile, toc_offset + 0x10, (int)i, "FileName", toc_string_table);
			string dir_name = utf_tab_sharp.UtfTab.query_utf_string(infile, toc_offset + 0x10, (int)i, "DirName", toc_string_table);

			if (string.IsNullOrEmpty(dir_name)) {
				return file_name;
			} else {
				return dir_name + "/" + file_name;
			}
		}

		public IEnumerable<string> GetChildNames() {
			for (int i = 0; i < toc_entries; ++i) {
				yield return GetChildNameFromIndex(i);
			}
		}

		public utf_tab_sharp.utf_query_result QueryInfo(string key) {
			return utf_tab_sharp.UtfTab.query_utf_key(infile, CpkHeader_offset + 0x10, 0, key);
		}

		public utf_tab_sharp.utf_query_result QueryChildInfoByIndex(int index, string key) {
			var query = new utf_tab_sharp.utf_query(key, index);
			return utf_tab_sharp.UtfTab.query_utf(infile, toc_offset + 0x10, query);
		}

		public utf_tab_sharp.utf_query_result QueryChildInfoByName(string name, string key) {
			int? idx = GetChildIndexFromName(name);
			if (idx.HasValue) {
				return QueryChildInfoByIndex(idx.Value, key);
			}
			return null;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					infile.Dispose();
				}

				infile = null;
				toc_string_table = null;
				disposedValue = true;
			}
		}

		public void Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
