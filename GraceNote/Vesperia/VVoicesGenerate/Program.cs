using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.Vesperia.VVoicesGenerate {
	class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 3 ) {
				Console.WriteLine( "Usage: VVoicesGenerate VOBTLdir VVoices.db GracesJapanese.db" );
				return -1;
			}

			String VOBTLDir = args[0];
			String VoiceDB  = args[1];
			String GracesDB = args[2];

			String[] VOBTLFiles = System.IO.Directory.GetFiles( VOBTLDir );

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>();
			foreach ( String VOBTL in VOBTLFiles ) {
				String VOBTLcut = VOBTL.Substring( VOBTL.LastIndexOfAny( new char[] { '/', '\\' } ) + 1 );
				if ( VOBTLcut.Contains( '.' ) ) {
					VOBTLcut = VOBTLcut.Remove( VOBTLcut.LastIndexOf( '.' ) );
				}
				String s = "\t(" + VOBTLcut + ')';
				Entries.Add( new GraceNoteDatabaseEntry( s, s, "", 0, 0, "", 0 ) );
			}

			GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + VoiceDB, "Data Source=" + GracesDB );

			return 0;
		}
	}
}
