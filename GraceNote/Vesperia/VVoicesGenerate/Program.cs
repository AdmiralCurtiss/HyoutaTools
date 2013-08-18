using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.Vesperia.VVoicesGenerate {
	class Program {
		public static int Execute( List<string> args ) {
			String VOBTLDir = @"c:\Users\Georg\Documents\Tales of Vesperia\ToV_Voices_Japanese_PS3_mp3v8\PS3_JP\VOBTL\";
			String VoiceDB = @"c:\Users\Georg\Documents\Tales of Vesperia\_voice\VVoices";
			String GracesDB = @"c:\Users\Georg\Documents\Tales of Vesperia\_voice\GracesJapanese";


			String[] VOBTLFiles = System.IO.Directory.GetFiles( VOBTLDir );

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>();
			foreach ( String VOBTL in VOBTLFiles ) {
				String VOBTLcut = VOBTL.Substring( VOBTL.LastIndexOf( '\\' ) + 1 );
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
