using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.GraceNote.DumpDatabase {
	class ScenarioFile : IComparable<ScenarioFile> {
		public int Pointer;
		//public int[] PointerArray;
		public String Name;
		public String Text;

		public String IdentifyerString;


		public override string ToString() {
			return Name + ": " + Text;
		}

		#region IComparable<ScenarioFile> Members

		public int CompareTo( ScenarioFile other ) {
			return Pointer.CompareTo( other.Pointer );
		}

		#endregion
	}
}
