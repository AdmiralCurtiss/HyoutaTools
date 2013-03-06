using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using HyoutaTools.Other.LuxPain;

namespace HyoutaTools.GraceNote.LuxPainEvtImport {
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
			if ( args.Count != 5 ) {
				Console.WriteLine( "Usage: LuxPainEvt_GraceNote event.evt NewDBFile TemplateDBFile GracesJapanese event.jp.evt" );
				return -1;
			}

			// templateDB must contain:
			/***
			 * CREATE TABLE Text(ID int primary key, StringID int, english text, comment text, updated tinyint, status tinyint, PointerRef integer, IdentifyString text, IdentifyPointerRef integer);
			 ***/

			//*
			String Filename = args[0];
			String NewDB = args[1];
			String TemplateDB = args[2];
			String GracesDB = args[3];
			String JapaneseFilename = args[4];
			//*/

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
			System.IO.File.Copy( TemplateDB, NewDB, true );
			InsertSQL( Evt, EvtJp, "Data Source=" + NewDB, "Data Source=" + GracesDB );
			Console.WriteLine( "Successfully imported entries!" );

			return 0;
		}



		public static bool InsertSQL( LuxPainEvt Evt, LuxPainEvt EvtJp, String ConnectionString, String ConnectionStringGracesJapanese ) {
			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			SQLiteConnection ConnectionGracesJapanese = new SQLiteConnection( ConnectionStringGracesJapanese );
			Connection.Open();
			ConnectionGracesJapanese.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteTransaction TransactionGracesJapanese = ConnectionGracesJapanese.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) )
			using ( SQLiteCommand CommandGracesJapanese = new SQLiteCommand( ConnectionGracesJapanese ) )
			using ( SQLiteCommand CommandJapaneseID = new SQLiteCommand( ConnectionGracesJapanese ) )
			using ( SQLiteCommand CommandSearchJapanese = new SQLiteCommand( ConnectionGracesJapanese ) ) {
				SQLiteParameter JapaneseIDParam = new SQLiteParameter();
				SQLiteParameter JapaneseParam = new SQLiteParameter();
				SQLiteParameter EnglishIDParam = new SQLiteParameter();
				SQLiteParameter StringIDParam = new SQLiteParameter();
				SQLiteParameter EnglishParam = new SQLiteParameter();
				SQLiteParameter PointerRefParam = new SQLiteParameter();
				SQLiteParameter JapaneseSearchParam = new SQLiteParameter();
				SQLiteParameter EnglishStatusParam = new SQLiteParameter();


				CommandGracesJapanese.CommandText = "INSERT INTO Japanese (ID, string, debug) VALUES (?, ?, 0)";
				CommandGracesJapanese.Parameters.Add( JapaneseIDParam );
				CommandGracesJapanese.Parameters.Add( JapaneseParam );

				Command.CommandText = "INSERT INTO Text (ID, StringID, english, comment, updated, status, PointerRef) VALUES (?, ?, ?, \"\", 0, ?, ?)";
				Command.Parameters.Add( EnglishIDParam );
				Command.Parameters.Add( StringIDParam );
				Command.Parameters.Add( EnglishParam );  // Line.SENG
				Command.Parameters.Add( EnglishStatusParam );
				Command.Parameters.Add( PointerRefParam ); // Line.Location

				CommandJapaneseID.CommandText = "SELECT MAX(ID)+1 FROM Japanese";

				CommandSearchJapanese.CommandText = "SELECT ID FROM Japanese WHERE string = ? AND debug = 0";
				CommandSearchJapanese.Parameters.Add( JapaneseSearchParam );

				int JPID;
				object JPMaxIDObject = CommandJapaneseID.ExecuteScalar();
				int JPMaxID;
				try {
					JPMaxID = Int32.Parse( JPMaxIDObject.ToString() ); // wtf why doesn't this work directly?
				} catch ( System.FormatException ) {
					// there's no ID in the database, just start with 0
					JPMaxID = 0;
				}
				int ENID = 1;

				for ( int i = 0; i < Evt.TextEntries.Count; ++i ) {
					// fetch GracesJapanese ID or generate new & insert new text
					String EnPlusJp = Evt.TextEntries[i].Text + "\n\n" + EvtJp.TextEntries[i].Text;
					JapaneseSearchParam.Value = EnPlusJp;
					object JPIDobj = CommandSearchJapanese.ExecuteScalar();
					if ( JPIDobj != null ) {
						JPID = (int)JPIDobj;
					} else {
						JPID = JPMaxID++;
						JapaneseIDParam.Value = JPID;
						JapaneseParam.Value = EnPlusJp;
						CommandGracesJapanese.ExecuteNonQuery();
					}

					// insert text into English table
					EnglishIDParam.Value = ENID;
					StringIDParam.Value = JPID;
					EnglishParam.Value = Evt.TextEntries[i].Text;
					EnglishStatusParam.Value = 0;
					PointerRefParam.Value = Evt.TextEntries[i].OffsetLocation;
					Command.ExecuteNonQuery();

					ENID++;
				}
				Transaction.Commit();
				TransactionGracesJapanese.Commit();
			}
			ConnectionGracesJapanese.Close();
			Connection.Close();

			return true;
		}


	}
}
