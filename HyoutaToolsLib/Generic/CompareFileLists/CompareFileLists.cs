using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Generic.CompareFileLists {
	public static class CompareFileLists {
		public static int Execute(List<string> args) {
			List<HashEntry> entries = new List<HashEntry>();
			List<List<HashEntry>> entriesByVersion = new List<List<HashEntry>>();
			foreach (string fn in args) {
				List<HashEntry> bv = new List<HashEntry>();
				foreach (string line in File.ReadAllLines(fn)) {
					if (!string.IsNullOrWhiteSpace(line)) {
						string hash = line.Substring(0, 40);
						string path = line.Substring(42);
						var he = new HashEntry() { List = fn, Hash = hash, Path = path };
						bv.Add(he);
						entries.Add(he);
					}
				}
				entriesByVersion.Add(bv);
			}

			for (int i = 1; i < entriesByVersion.Count; ++i) {
				string s = CompareVersions(entriesByVersion[i - 1], entriesByVersion[i]);
				string p = Path.Combine(Path.GetDirectoryName(args[i]), Path.GetFileNameWithoutExtension(args[i - 1]) + "_to_" + Path.GetFileNameWithoutExtension(args[i]) + ".txt");
				File.WriteAllText(p, s);
			}

			{
				string s = DetectUnchangedFiles(entriesByVersion);
				string p = Path.Combine(Path.GetDirectoryName(args[0]), "all_identical.txt");
				File.WriteAllText(p, s);
			}

			return 0;
		}

		public static string CompareVersions(List<HashEntry> a, List<HashEntry> b) {
			bool printHashes = false;
			Dictionary<string, HashEntry> byPathA = new Dictionary<string, HashEntry>();
			Dictionary<string, HashEntry> byPathB = new Dictionary<string, HashEntry>();
			HashSet<string> paths = new HashSet<string>();
			foreach (HashEntry e in a) {
				byPathA.Add(e.Path, e);
				paths.Add(e.Path);
			}
			foreach (HashEntry e in b) {
				byPathB.Add(e.Path, e);
				paths.Add(e.Path);
			}

			List<HashEntry> removed = new List<HashEntry>();
			List<HashEntry> added = new List<HashEntry>();
			foreach (string p in paths) {
				bool inA = byPathA.ContainsKey(p);
				bool inB = byPathB.ContainsKey(p);
				if (inA && inB) {
					if (byPathA[p].Hash == byPathB[p].Hash) {
						// ignore all files that are identical in a and b
						byPathA.Remove(p);
						byPathB.Remove(p);
					}
				} else if (inA && !inB) {
					// file was removed
					removed.Add(byPathA[p]);
					byPathA.Remove(p);
				} else if (!inA && inB) {
					// file is new
					added.Add(byPathB[p]);
					byPathB.Remove(p);
				}
			}

			// byPathA/B now contain only changed files
			Util.Assert(byPathA.Count == byPathB.Count);

			StringBuilder sb = new StringBuilder();

			if (byPathA.Count > 0) {
				sb.AppendFormat("{0} file{1} changed:", byPathA.Count, byPathA.Count == 1 ? "" : "s");
				sb.AppendLine();
				foreach (var kvp in byPathA) {
					HashEntry ea = kvp.Value;
					HashEntry eb = byPathB[kvp.Key];
					if (printHashes) {
						sb.Append(ea.Hash);
						sb.Append(" -> ");
						sb.Append(eb.Hash);
						sb.Append(" ");
					}
					sb.Append(ea.Path);
					sb.AppendLine();
				}
				sb.AppendLine();
			}

			if (removed.Count > 0) {
				sb.AppendFormat("{0} file{1} removed:", removed.Count, removed.Count == 1 ? "" : "s");
				sb.AppendLine();
				foreach (var e in removed) {
					if (printHashes) {
						sb.Append(e.Hash);
						sb.Append(" ");
					}
					sb.Append(e.Path);
					sb.AppendLine();
				}
				sb.AppendLine();
			}

			if (added.Count > 0) {
				sb.AppendFormat("{0} file{1} added:", added.Count, added.Count == 1 ? "" : "s");
				sb.AppendLine();
				foreach (var e in added) {
					if (printHashes) {
						sb.Append(e.Hash);
						sb.Append(" ");
					}
					sb.Append(e.Path);
					sb.AppendLine();
				}
				sb.AppendLine();
			}

			return sb.ToString();
		}

		public static string DetectUnchangedFiles(List<List<HashEntry>> list) {
			Dictionary<string, List<HashEntry>> byPath = new Dictionary<string, List<HashEntry>>();
			List<string> paths = new List<string>();
			foreach (var a in list) {
				foreach (HashEntry e in a) {
					if (!byPath.ContainsKey(e.Path)) {
						byPath.Add(e.Path, new List<HashEntry>());
						paths.Add(e.Path);
					}
					byPath[e.Path].Add(e);
				}
			}

			StringBuilder sbIdentical = new StringBuilder();

			foreach (string p in paths) {
				if (AllSameFile(byPath[p])) {
					sbIdentical.AppendLine(p);
				}
			}

			return sbIdentical.ToString();
		}

		private static bool AllSameFile(List<HashEntry> entries) {
			if (entries.Count == 0) {
				return true;
			}

			HashEntry e = entries[0];
			foreach (HashEntry ee in entries) {
				if (e.Hash != ee.Hash) {
					return false;
				}
			}

			return true;
		}
	}
	public class HashEntry {
		public string List;
		public string Hash;
		public string Path;
	}
}
