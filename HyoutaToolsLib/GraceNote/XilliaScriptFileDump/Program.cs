using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using HyoutaTools.Tales.Xillia.SDB;

namespace HyoutaTools.GraceNote.XilliaScriptFileDump {
	class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 3 ) {
				Console.WriteLine( "Usage: XilliaScript_GraceNote XilliaScriptFile NewDBFile GracesJapanese" );
				return -1;
			}
			String Filename = args[0];
			String NewDB = args[1];
			String GracesDB = args[2];

			SDB XSF = new SDB( Filename );
			GraceNoteUtil.GenerateEmptyDatabase( NewDB );
			if ( !System.IO.File.Exists( GracesDB ) ) {
				GraceNoteUtil.GenerateEmptyGracesJapanese( GracesDB );
			}

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>( XSF.TextList.Count );
			foreach ( SDBEntry entry in XSF.TextList ) {
				GraceNoteDatabaseEntry gn = new GraceNoteDatabaseEntry( entry.Text, entry.Text, "", 0, (int)entry.PointerText, entry.IDString, (int)entry.PointerIDString );
				Entries.Add( gn );
			}

			GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + NewDB, "Data Source=" + GracesDB );

			return 0;
		}
	}
}
