using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using HyoutaTools.Trophy;

namespace HyoutaTools.GraceNote.Trophy.TropSfmImport {
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
			if ( args.Length != 5 ) {
				Console.WriteLine( "Usage: TropSFM_GraceNote TROP.SFM TROPCONF.SFM NewDBFile TemplateDBFile GracesJapanese" );
				//   return;
			}

			// templateDB must contain:
			/***
			 * CREATE TABLE Text(ID int primary key, StringID int, english text, comment text, updated tinyint, status tinyint, PointerRef integer, t_id text, t_hidden tinyint, t_ttype text, t_pid text)
			 ***/

			//*
			String Filename = args[0];
			String FilenameTropConf = args[1];
			String NewDB = args[2];
			String TemplateDB = args[3];
			String GracesDB = args[4];
			//*/

			/*
			String Filename = @"c:\TROPHY\TROP.SFM";
			String FilenameTropConf = @"c:\TROPHY\TROPCONF.SFM";
			String NewDB = @"c:\TROPHY\VTrophies";
			String TemplateDB = @"c:\TROPHY\VTemplate";
			String GracesDB = @"c:\TROPHY\GracesJapanese";
			//*/


			TrophyConfNode TROPSFM = Util.ReadTropSfm( Filename, FilenameTropConf );
			System.IO.File.Copy( TemplateDB, NewDB );
			InsertSQL( TROPSFM, "Data Source=" + NewDB, "Data Source=" + GracesDB );
			//System.IO.File.WriteAllBytes(@"C:\TROPHY\newTrophyConf.trp", Encoding.UTF8.GetBytes(TROPSFM.ExportTropSFM(true)));
			//System.IO.File.WriteAllBytes(@"C:\TROPHY\newTrophy.trp", Encoding.UTF8.GetBytes(TROPSFM.ExportTropSFM(false)));

			return;
		}







		public static bool InsertSQL( TrophyConfNode TROPSFM, String ConnectionString, String ConnectionStringGracesJapanese ) {
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
				SQLiteParameter LocationParam = new SQLiteParameter();
				SQLiteParameter JapaneseSearchParam = new SQLiteParameter();
				SQLiteParameter EnglishStatusParam = new SQLiteParameter();
				SQLiteParameter TrophyIDParam = new SQLiteParameter();
				SQLiteParameter TrophyHiddenParam = new SQLiteParameter();
				SQLiteParameter TrophyTTypeParam = new SQLiteParameter();
				SQLiteParameter TrophyPIDParam = new SQLiteParameter();


				CommandGracesJapanese.CommandText = "INSERT INTO Japanese (ID, string, debug) VALUES (?, ?, 0)";
				CommandGracesJapanese.Parameters.Add( JapaneseIDParam );
				CommandGracesJapanese.Parameters.Add( JapaneseParam );

				Command.CommandText = "INSERT INTO Text (ID, StringID, english, comment, updated, status, PointerRef, t_id, t_hidden, t_ttype, t_pid) VALUES (?, ?, ?, \"\", 0, ?, ?, ?, ?, ?, ?)";
				Command.Parameters.Add( EnglishIDParam );
				Command.Parameters.Add( StringIDParam );
				Command.Parameters.Add( EnglishParam );  // Line.SENG
				Command.Parameters.Add( EnglishStatusParam );
				Command.Parameters.Add( LocationParam ); // Line.Location
				Command.Parameters.Add( TrophyIDParam );
				Command.Parameters.Add( TrophyHiddenParam );
				Command.Parameters.Add( TrophyTTypeParam );
				Command.Parameters.Add( TrophyPIDParam );

				CommandJapaneseID.CommandText = "SELECT MAX(ID)+1 FROM Japanese";

				CommandSearchJapanese.CommandText = "SELECT ID FROM Japanese WHERE string = ? AND debug = 0";
				CommandSearchJapanese.Parameters.Add( JapaneseSearchParam );

				int JPID;
				object JPMaxIDObject = CommandJapaneseID.ExecuteScalar();
				int JPMaxID = Int32.Parse( JPMaxIDObject.ToString() ); // wtf why doesn't this work directly?
				int ENID = 1;

				foreach ( TrophyNode Trophy in TROPSFM.Trophies ) {
					// fetch GracesJapanese ID or generate new & insert new text
					JapaneseSearchParam.Value = Trophy.Name;
					object JPIDobj = CommandSearchJapanese.ExecuteScalar();
					if ( JPIDobj != null ) {
						JPID = (int)JPIDobj;
					} else {
						JPID = JPMaxID++;
						JapaneseIDParam.Value = JPID;
						JapaneseParam.Value = Trophy.Name;
						CommandGracesJapanese.ExecuteNonQuery();
					}

					// insert text into English table
					EnglishIDParam.Value = ENID;
					StringIDParam.Value = JPID;
					EnglishParam.Value = Trophy.Name;
					EnglishStatusParam.Value = 0;
					LocationParam.Value = 0; // == Trophy Name
					TrophyIDParam.Value = Trophy.ID;
					TrophyHiddenParam.Value = Trophy.Hidden ? 1 : 0;
					TrophyTTypeParam.Value = Trophy.TType;
					TrophyPIDParam.Value = Trophy.PID;
					Command.ExecuteNonQuery();

					ENID++;



					// --- repeat for trophy.detail

					// fetch GracesJapanese ID or generate new & insert new text
					JapaneseSearchParam.Value = Trophy.Detail;
					JPIDobj = CommandSearchJapanese.ExecuteScalar();
					if ( JPIDobj != null ) {
						JPID = (int)JPIDobj;
					} else {
						JPID = JPMaxID++;
						JapaneseIDParam.Value = JPID;
						JapaneseParam.Value = Trophy.Detail;
						CommandGracesJapanese.ExecuteNonQuery();
					}

					// insert text into English table
					EnglishIDParam.Value = ENID;
					StringIDParam.Value = JPID;
					EnglishParam.Value = Trophy.Detail;
					EnglishStatusParam.Value = 0;
					LocationParam.Value = 1; // == Trophy Detail
					TrophyIDParam.Value = Trophy.ID;
					TrophyHiddenParam.Value = Trophy.Hidden ? 1 : 0;
					TrophyTTypeParam.Value = Trophy.TType;
					TrophyPIDParam.Value = Trophy.PID;
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
