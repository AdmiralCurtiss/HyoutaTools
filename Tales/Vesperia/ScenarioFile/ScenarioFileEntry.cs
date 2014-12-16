using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.ScenarioFile {
	public class ScenarioFileEntry : IComparable<ScenarioFileEntry> {
		public uint Pointer;

		public string JpName;
		public string JpText;
		public string EnName;
		public string EnText;

		public override string ToString() {
			return EnName + ": " + EnText;
		}

		public int CompareTo( ScenarioFileEntry other ) {
			return Pointer.CompareTo( other.Pointer );
		}
	}
}
