using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.N64.OoTSaveEditor {
	class Program {
		public static int Execute( List<string> args ) {
			OoTSaveFile save = new OoTSaveFile( @"c:\Users\Georg\Downloads\ZELDA MASTER QUEST.sra" );
			save.WriteSave( @"c:\Users\Georg\Downloads\ZELDA MASTER QUEST-mod.sra" );
			return 0;
		}
	}
}
