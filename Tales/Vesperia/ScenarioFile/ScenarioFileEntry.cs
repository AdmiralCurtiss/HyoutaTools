using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.ScenarioFile {
	public enum TextboxType { Unknown = -1, Bubble = 0, Information = 1, Subtitle = 2 }
	public class ScenarioFileEntry : IComparable<ScenarioFileEntry> {
		public uint Pointer;

		public string JpName;
		public string JpText;
		public string EnName;
		public string EnText;

		public TextboxType Type = TextboxType.Unknown;

		public ScenarioFileEntry() { }
		public ScenarioFileEntry( TSS.TSSEntry name, TSS.TSSEntry text ) {
			JpName = name.StringJpn;
			JpText = text.StringJpn;
			EnName = name.StringEng;
			EnText = text.StringEng;
		}

		public override string ToString() {
			return EnName + ": " + EnText;
		}

		public int CompareTo( ScenarioFileEntry other ) {
			return Pointer.CompareTo( other.Pointer );
		}
	}
}
