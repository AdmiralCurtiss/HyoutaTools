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
		static void Execute( string[] args ) {
			if ( args.Length != 4 ) {
				Console.WriteLine( "Usage: DanganRonpaMenu_GraceNote menu.pak NewDBFile TemplateDBFile GracesJapanese" );
				return;
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
			//*/

			/*
			String Filename = @"c:\Users\Georg\Downloads\Xillia Script files\69753.SDBJPN";
			String NewDB = @"c:\Users\Georg\Downloads\Xillia Script files\X69753";
			String TemplateDB = @"c:\Users\Georg\Downloads\Xillia Script files\XTemplate";
			String GracesDB = @"c:\Users\Georg\Downloads\Xillia Script files\GracesJapanese";
			//*/

			PakText DRMF;
			try {
				DRMF = new PakText( Filename );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				Console.WriteLine( "Failed loading menu file!" );
				return;
			}
			Console.WriteLine( "Found " + DRMF.TextList.Count + " entries, importing..." );
			System.IO.File.Copy( TemplateDB, NewDB, true );
			InsertSQL( DRMF, "Data Source=" + NewDB, "Data Source=" + GracesDB );
			Console.WriteLine( "Successfully imported entries!" );

			//byte[] newfile = DRMF.CreateFile();
			//System.IO.File.WriteAllBytes(Filename + ".new", newfile);

			//System.IO.File.WriteAllBytes(@"C:\TROPHY\newTrophyConf.trp", Encoding.UTF8.GetBytes(TROPSFM.ExportTropSFM(true)));
			//System.IO.File.WriteAllBytes(@"C:\TROPHY\newTrophy.trp", Encoding.UTF8.GetBytes(TROPSFM.ExportTropSFM(false)));

			return;
		}




		public static bool InsertSQL( PakText DRTF, String ConnectionString, String ConnectionStringGracesJapanese ) {
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

				foreach ( PakTextEntry entry in DRTF.TextList ) {
					// fetch GracesJapanese ID or generate new & insert new text
					JapaneseSearchParam.Value = entry.Text;
					object JPIDobj = CommandSearchJapanese.ExecuteScalar();
					if ( JPIDobj != null ) {
						JPID = (int)JPIDobj;
					} else {
						JPID = JPMaxID++;
						JapaneseIDParam.Value = JPID;
						JapaneseParam.Value = entry.Text;
						CommandGracesJapanese.ExecuteNonQuery();
					}

					// insert text into English table
					EnglishIDParam.Value = ENID;
					StringIDParam.Value = JPID;
					EnglishParam.Value = entry.Text;
					EnglishStatusParam.Value = 0;
					PointerRefParam.Value = entry.OffsetLocation;
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
