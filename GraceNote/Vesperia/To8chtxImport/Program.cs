using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.To8chtx;

namespace HyoutaTools.GraceNote.Vesperia.To8chtxImport {
	class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 3 ) {
				Console.WriteLine( "Usage: TO8CHTX_GraceNote ChatFilename NewDBFilename GracesJapanese" );
				return -1;
			}

			String Filename = args[0];
			String NewDB = args[1];
			String GracesDB = args[2];

			ChatFile c = new ChatFile( System.IO.File.ReadAllBytes( Filename ) );

			System.IO.File.Copy( "VTemplate", NewDB, true );

			c.InsertSQL( "Data Source=" + NewDB, "Data Source=" + GracesDB );
			return 0;
		}
	}
}
