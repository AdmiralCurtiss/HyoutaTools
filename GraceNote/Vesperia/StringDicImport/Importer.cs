using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.TSS;

namespace HyoutaTools.GraceNote.Vesperia.StringDicImport {
	class Importer {
		public static int Execute( List<string> args ) {

			string string_dic = args[0];
			string databasename = args[1];
			string gracesjapanese = args[2];

			TSSFile TSS;
			try {
				TSS = new TSSFile( System.IO.File.ReadAllBytes( string_dic ) );
			} catch ( System.IO.FileNotFoundException ) {
				Console.WriteLine( "Could not open STRING_DIC.SO, exiting." );
				return -1;
			}


			List<GraceNoteDatabaseEntry> entries = new List<GraceNoteDatabaseEntry>();
			foreach ( TSSEntry e in TSS.Entries ) {
				GraceNoteDatabaseEntry g = new GraceNoteDatabaseEntry( e.StringJPN, e.StringENG );
				entries.Add( g );
			}

			System.IO.File.WriteAllBytes( databasename, Properties.Resources.gndb_template );
			GraceNoteDatabaseEntry.InsertSQL( entries.ToArray(), "Data Source=" + databasename, "Data Source=" + gracesjapanese );


			return 0;
		}
	}
}
