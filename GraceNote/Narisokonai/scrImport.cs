using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.GraceNote.Narisokonai {
	class scrImport {
		public static int Import( List<string> args ) {
			if ( args.Count < 3 ) {
				Console.WriteLine( "Usage: scr Database GracesJapanese" );
				return -1;
			}

			string InFile = args[0];
			string OutDatabase = args[1];
			string GracesJapanese = args[2];
			var bscrFile = new HyoutaTools.Narisokonai.scr( System.IO.File.ReadAllBytes( InFile ) );
			//System.IO.File.WriteAllBytes( OutDatabase, Properties.Resources.gndb_template );

			//List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>( bscrFile.Strings.Count );
			//foreach ( HyoutaTools.LastRanker.bscrString e in bscrFile.Strings ) {
			//    int status = 0;
			//    if ( e.String.StartsWith( "func" ) || e.String.StartsWith( "■" ) || e.String.Trim() == "" ) {
			//        status = -1;
			//    }

			//    GraceNoteDatabaseEntry gn = new GraceNoteDatabaseEntry( e.String, e.String, "", status, (int)e.Position, "", 0 );
			//    Entries.Add( gn );
			//}

			//GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + OutDatabase, "Data Source=" + GracesJapanese );

			return 0;
		}
	}
}
