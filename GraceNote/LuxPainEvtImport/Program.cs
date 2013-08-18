using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using HyoutaTools.Other.LuxPain;

namespace HyoutaTools.GraceNote.LuxPainEvtImport {
	class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 4 ) {
				Console.WriteLine( "Usage: LuxPainEvt_GraceNote event.evt NewDBFile GracesJapanese event.jp.evt" );
				return -1;
			}
			String Filename = args[0];
			String NewDB = args[1];
			String GracesDB = args[2];
			String JapaneseFilename = args[3];

			LuxPainEvt Evt;
			LuxPainEvt EvtJp;
			try {
				Evt = new LuxPainEvt( Filename );
				EvtJp = new LuxPainEvt( JapaneseFilename );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				Console.WriteLine( "Failed loading text file!" );
				return -1;
			}
			Evt.FormatTextForEditing();
			EvtJp.FormatTextForEditing();

			if ( Evt.TextEntries.Count != EvtJp.TextEntries.Count ) {
				Console.WriteLine( "Entry count over languages doesn't match, padding..." );

				while ( Evt.TextEntries.Count < EvtJp.TextEntries.Count ) {
					LuxPainEvtText t = new LuxPainEvtText();
					t.OffsetLocation = 0x7FFFFFFF;
					t.Text = "[Entry does not exist in this language, this is just for completion's sake.]";
					Evt.TextEntries.Add( t );
				}

				while ( Evt.TextEntries.Count > EvtJp.TextEntries.Count ) {
					LuxPainEvtText t = new LuxPainEvtText();
					t.OffsetLocation = 0x7FFFFFFF;
					t.Text = "[Entry does not exist in this language, this is just for completion's sake.]";
					EvtJp.TextEntries.Add( t );
				}

				if ( Evt.TextEntries.Count != EvtJp.TextEntries.Count ) { throw new Exception( "this shouldn't happen!" ); }
			}

			Console.WriteLine( "Found " + Evt.TextEntries.Count + " entries, importing..." );
			System.IO.File.WriteAllBytes( NewDB, Properties.Resources.gndb_template );

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>( Evt.TextEntries.Count );
			for ( int i = 0; i < Evt.TextEntries.Count; ++i ) {
				// fetch GracesJapanese ID or generate new & insert new text
				String EnPlusJp = Evt.TextEntries[i].Text + "\n\n" + EvtJp.TextEntries[i].Text;
				GraceNoteDatabaseEntry gn =
					new GraceNoteDatabaseEntry( EnPlusJp, Evt.TextEntries[i].Text, "", 0, (int)Evt.TextEntries[i].OffsetLocation, "", 0 );
				Entries.Add( gn );
			}
			GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + NewDB, "Data Source=" + GracesDB );
			Console.WriteLine( "Successfully imported entries!" );

			return 0;
		}
	}
}
