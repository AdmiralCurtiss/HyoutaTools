using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.LastRanker;

namespace HyoutaTools.GraceNote.LastRanker {
	class SscrImport {
		public static int Import( List<string> args ) {
			if ( args.Count < 3 ) {
				Console.WriteLine( "Usage: sscr Database GracesJapanese" );
				return -1;
			}

			string InFile = args[0];
			string OutDatabase = args[1];
			string GracesJapanese = args[2];
			SSCR SscrFile = new SSCR( System.IO.File.ReadAllBytes( InFile ) );
			GraceNoteUtil.GenerateEmptyDatabase( OutDatabase );

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>( SscrFile.Names.Count + SscrFile.SystemTerms.Count + SscrFile.Somethings.Count );
			foreach ( var x in SscrFile.Names ) {
				Entries.Add( new GraceNoteDatabaseEntry( x.Name, x.Name, "", 0, 1, x.Id, (int)x.Unknown ) );
			}
			foreach ( var x in SscrFile.SystemTerms ) {
				Entries.Add( new GraceNoteDatabaseEntry( x.Term, x.Term, "", 0, 2, "", (int)x.Unknown ) );
			}
			foreach ( var x in SscrFile.Somethings ) {
				Entries.Add( new GraceNoteDatabaseEntry( x.Text, x.Text, "", 0, 3, x.Id, (int)x.Unknown4 ) );
			}

			GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + OutDatabase, "Data Source=" + GracesJapanese );

			return 0;
		}
	}
}
