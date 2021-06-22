using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Abyss.SB7 {
	class DumpText {
		public static int Execute( List<string> args ) {
			string infile = args[0];
			string outfile = args.Count >= 2 ? args[1] : args[0] + ".txt";

			List<string> text = new SB7( infile ).Texts;

			for ( int i = 0; i < text.Count; ++i ) {
				text[i] = GraceNote.DumpDatabase.Program.RemoveNewlines( text[i].Replace( '\f', '\n' ) );
			}

			System.IO.File.WriteAllLines( outfile, text.ToArray() );
			return 0;
		}
	}
}
