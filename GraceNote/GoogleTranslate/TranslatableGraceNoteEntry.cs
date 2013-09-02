using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.GraceNote.GoogleTranslate {
	public class TranslatableGraceNoteEntry {

		public GraceNoteDatabaseEntry Entry;
		
		public int NewLineCount;
		public bool NewLineAtEnd;
		public string PreserveStringAtStart = "";
		public string PreserveStringAtEnd = "";

		public TranslatableGraceNoteEntry() { }
	}
}
