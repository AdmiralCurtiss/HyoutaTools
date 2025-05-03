using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Generic {
	class MarkdownToHtml {
		public static string EscapeHtml(string s, bool raw = false) {
			StringBuilder sb = new StringBuilder();
			bool inItalics = false;
			foreach (var c in s.EnumerateRunes()) {
				if (c.Value >= ' ' && c.Value <= '~') {
					if (c.Value == '<') {
						sb.Append("&lt;");
					} else if (c.Value == '>') {
						sb.Append("&gt;");
					} else if (c.Value == '"') {
						sb.Append("&quot;");
					} else if (c.Value == '&') {
						sb.Append("&amp;");
					} else if (!raw && c.Value == '*') {
						if (!inItalics) {
							sb.Append("<i>");
						} else {
							sb.Append("</i>");
						}
						inItalics = !inItalics;
					} else {
						sb.Append(c);
					}
				} else {
					sb.Append("&#x");
					sb.Append(c.Value.ToString("x"));
					sb.Append(";");
				}
			}
			if (inItalics) {
				sb.Append("</i>");
			}
			return sb.ToString();
		}

		public static int Execute(List<string> args) {
			if (args.Count < 1) {
				Console.WriteLine("Usage: MarkdownToHtml filename");
				return -1;
			}

			string[] lines;
			try {
				lines = System.IO.File.ReadAllLines(args[0], Encoding.UTF8);
			} catch (Exception) {
				Console.WriteLine("Can't open File " + args[0]);
				return -1;
			}


			List<string> result = new List<string>();
			result.Add("<!DOCTYPE html>");
			result.Add("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
			result.Add("<title>" + EscapeHtml(System.IO.Path.GetFileNameWithoutExtension(args[0]), true) + "</title>");
			result.Add("<style>");
			result.Add(".code {");
			result.Add(" font-family: monospace;");
			result.Add(" padding: 4px 8px 4px 8px;");
			result.Add(" margin: 2px 0px 2px 0px;");
			result.Add(" background-color: #dddddd;");
			result.Add(" white-space: pre;");
			result.Add("}");
			result.Add("</style>");
			result.Add("</head><body>");

			bool skipEmptyLines = true;
			bool inCodeBlock = false;
			bool needsLineOnHeader = false;
			StringBuilder code = new StringBuilder();
			for (int i = 0; i < lines.Length; ++i) {
				string line = lines[i];
				if (!inCodeBlock) {
					if (line.Length == 0) {
						if (!skipEmptyLines) {
							result.Add("<br/>");
						}
						continue;
					}

					if (IsAll(line, '-')) {
						result.Add("<hr/>");
						continue;
					}

					int leadingEquals = CountLeading(line, '=');
					if (leadingEquals >= 1 && leadingEquals == CountTrailing(line, '=')) {
						int headerNum = 3;
						if (leadingEquals >= 3) {
							headerNum = 1;
						}
						if (leadingEquals == 2) {
							headerNum = 2;
						}
						DropTrailingEmptyLines(result);
						if (needsLineOnHeader) {
							needsLineOnHeader = false;
							result.Add("<br/>");
							result.Add("<hr/>");
						}
						result.Add(string.Format("<h{0}>{1}</h{0}>", headerNum, EscapeHtml(line.Substring(leadingEquals, line.Length - leadingEquals * 2).Trim())));
						skipEmptyLines = true;
						continue;
					}

					if (line.StartsWith('[') && line.EndsWith(']')) {
						string l = line.Substring(1, line.Length - 2);
						result.Add("<a href=\"" + l + "\">" + EscapeHtml(l) + "</a><br/>");
						needsLineOnHeader = true;
						continue;
					}

					skipEmptyLines = false;
				}

				if (line == "```") {
					if (inCodeBlock) {
						result.Add("<div class=\"code\">" + code.ToString() + "</div>");
						code.Clear();
					}
					inCodeBlock = !inCodeBlock;
					needsLineOnHeader = true;
					continue;
				}

				if (inCodeBlock) {
					if (code.Length > 0) {
						code.Append('\n');
					}
					code.Append(EscapeHtml(lines[i], true));
				} else {
					result.Add(EscapeHtml(lines[i], false) + "<br/>");
				}
				needsLineOnHeader = true;
			}

			result.Add("</body></html>");

			System.IO.File.WriteAllLines(args[0] + ".html", result, Encoding.UTF8);

			return 0;
		}

		private static void DropTrailingEmptyLines(List<string> lines) {
			while (lines.Count > 0 && lines[lines.Count - 1] == "<br/>") {
				lines.RemoveAt(lines.Count - 1);
			}
		}

		private static bool IsAll(string line, char v) {
			foreach (char c in line) {
				if (c != v) {
					return false;
				}
			}
			return true;
		}

		private static int CountTrailing(string line, char v) {
			int count = 0;
			for (int i = line.Length; i > 0; --i) {
				if (line[i - 1] != v) {
					break;
				}
				++count;
			}
			return count;
		}

		private static int CountLeading(string line, char v) {
			int count = 0;
			foreach (char c in line) {
				if (c != v) {
					break;
				}
				++count;
			}
			return count;
		}
	}
}
