using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using HyoutaTools.DanganRonpa.PakText;

namespace HyoutaTools.GraceNote.DanganRonpa.PakTextImport {
	class ScenarioString {
		public int Pointer;
		public String Jpn;
		public String Eng;

		public ScenarioString( int Pointer, String Jpn, String Eng ) {
			this.Pointer = Pointer;
			this.Jpn = Jpn;
			this.Eng = Eng;
		}
	}

	class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 3 ) {
				Console.WriteLine( "Usage: menu.pak NewDBFile GracesJapanese" );
				return -1;
			}

			String Filename = args[0];
			String NewDB = args[1];
			String GracesDB = args[2];

			PakText DRMF;
			try {
				DRMF = new PakText( Filename );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				Console.WriteLine( "Failed loading menu file!" );
				return -1;
			}
			Console.WriteLine( "Found " + DRMF.TextList.Count + " entries, importing..." );
			System.IO.File.WriteAllBytes( NewDB, Properties.Resources.gndb_template );

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>( DRMF.TextList.Count );
			foreach ( PakTextEntry entry in DRMF.TextList ) {
				GraceNoteDatabaseEntry gn = new GraceNoteDatabaseEntry( entry.Text, entry.Text, "", 0, entry.OffsetLocation, "", 0 );
				Entries.Add( gn );
			}
			GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + NewDB, "Data Source=" + GracesDB );

			Console.WriteLine( "Successfully imported entries!" );

			//byte[] newfile = DRMF.CreateFile();
			//System.IO.File.WriteAllBytes(Filename + ".new", newfile);

			return 0;
		}
	}
}
