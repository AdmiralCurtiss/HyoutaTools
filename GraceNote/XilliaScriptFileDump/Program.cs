using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using HyoutaTools.Tales.Xillia;

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

			XilliaScriptFile XSF = new XilliaScriptFile( Filename );
			System.IO.File.WriteAllBytes( NewDB, Properties.Resources.gndb_template );

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>( XSF.TextList.Count );
			foreach ( XS entry in XSF.TextList ) {
				GraceNoteDatabaseEntry gn = new GraceNoteDatabaseEntry( entry.Text, entry.Text, "", 0, entry.PointerText, entry.IDString, entry.PointerIDString );
				Entries.Add( gn );
			}

			GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + NewDB, "Data Source=" + GracesDB );

			return 0;
		}
	}
}
