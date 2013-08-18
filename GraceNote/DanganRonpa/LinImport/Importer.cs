using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.DanganRonpa.Lin;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.DanganRonpa.LinImport {
	public static class Importer {

		public static int Import( List<string> args ) {
			if ( args.Count < 3 ) {
				Console.WriteLine( "Usage: text.lin NewDBFile GracesJapanese [LIN version (default 1)]" );
				return -1;
			}
			uint Version;
			if ( !( args.Count >= 4 && UInt32.TryParse( args[3], out Version ) ) ) {
				Version = 1;
			}
			HyoutaTools.DanganRonpa.DanganUtil.GameVersion = Version;
			return LinImport.Importer.Import( args[0], args[1], args[2] );
		}
		public static int Import( String Filename, String NewDB, String GracesDB ) {
			LIN lin;
			try {
				lin = new LIN( Filename );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				Console.WriteLine( "Failed loading text file!" );
				return -1;
			}

			Console.WriteLine( "Importing..." );
			System.IO.File.WriteAllBytes( NewDB, Properties.Resources.gndb_template );

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>();

			string TextToInsert = "";
			foreach ( ScriptEntry s in lin.ScriptData ) {
				if ( s.Type == 0x02 ) {
					Entries.Add( new GraceNoteDatabaseEntry( TextToInsert, TextToInsert, "", -1, 1, "[Game Code]", 0 ) );
					Entries.Add( new GraceNoteDatabaseEntry( s.Text, s.Text, "", 0, 2, s.IdentifyString, 0 ) );
					TextToInsert = "";
					continue;
				}
				TextToInsert = TextToInsert + s.FormatForGraceNote() + '\n';
			}
			if ( TextToInsert != null ) {
				Entries.Add( new GraceNoteDatabaseEntry( TextToInsert, TextToInsert, "", -1, 1, "[Game Code]", 0 ) );
			}
			if ( lin.UnreferencedText != null ) {
				foreach ( KeyValuePair<int, string> u in lin.UnreferencedText ) {
					Entries.Add( new GraceNoteDatabaseEntry( u.Value, u.Value, "", 0, 3, "[Unreferenced Text]", u.Key ) );
				}
			}
			GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + NewDB, "Data Source=" + GracesDB );

			Console.WriteLine( "Successfully imported entries!" );
			return 0;
		}
	}
}
