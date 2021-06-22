using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Sting;

namespace HyoutaTools.GraceNote.Sting.BlazeUnionScript {
	public class Importer {
		public static int Import( List<string> args ) {
			if ( args.Count < 3 ) {
				Console.WriteLine( "Usage: ScriptFile NewDBFile GracesJapanese" );
				return -1;
			}
			return Import( args[0], args[1], args[2] ) ? 0 : -1;
		}
		public static bool Import( String filename, String newDb, String gracesJapanese ) {
			BlazeUnionScriptFile scriptfile = new BlazeUnionScriptFile( filename );
			List<GraceNoteDatabaseEntry> entries = new List<GraceNoteDatabaseEntry>();
			GraceNoteUtil.GenerateEmptyDatabase( newDb );
			if ( !System.IO.File.Exists( gracesJapanese ) ) {
				GraceNoteUtil.GenerateEmptyGracesJapanese( gracesJapanese );
			}

			foreach ( var section in scriptfile.Sections ) {
				foreach ( var kvp in section.Strings ) {
					entries.Add( new GraceNoteDatabaseEntry( kvp.Value, PointerRef: (int)kvp.Key ) );
				}
			}

			GraceNoteDatabaseEntry.InsertSQL( entries.ToArray(), "Data Source=" + newDb, "Data Source=" + gracesJapanese );

			return true;
		}
	}
}
