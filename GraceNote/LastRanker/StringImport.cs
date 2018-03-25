using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.LastRanker {
	class StringImport {
		public static int Import( List<string> args ) {
			if ( args.Count < 3 ) {
				Console.WriteLine( "Usage: file Database GracesJapanese" );
				return -1;
			}

			string InFile = args[0];
			string OutDatabase = args[1];
			string GracesJapanese = args[2];
			HyoutaTools.LastRanker.StringFile StringFile = new HyoutaTools.LastRanker.StringFile( System.IO.File.ReadAllBytes( InFile ) );
			GraceNoteUtil.GenerateEmptyDatabase( OutDatabase );

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>( StringFile.Strings.Count );
			foreach ( HyoutaTools.LastRanker.bscrString e in StringFile.Strings ) {
				int status = 0;
				if ( e.String.StartsWith( "func" ) || e.String.StartsWith( "■" ) || e.String.Trim() == "" ) {
					status = -1;
				}

				GraceNoteDatabaseEntry gn = new GraceNoteDatabaseEntry( e.String, e.String, "", status, (int)e.Position, "", 0 );
				Entries.Add( gn );
			}

			GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + OutDatabase, "Data Source=" + GracesJapanese );

			return 0;
		}
	}
}
