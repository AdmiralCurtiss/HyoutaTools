using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using HyoutaTools.Tales.Xillia;

namespace HyoutaTools.GraceNote.XilliaScriptFileDump {
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
		public static int Execute( string[] args ) {
			if ( args.Length != 4 ) {
				Console.WriteLine( "Usage: XilliaScript_GraceNote XilliaScriptFile NewDBFile TemplateDBFile GracesJapanese" );
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
			//*/

			/*
			String Filename = @"c:\Users\Georg\Downloads\Xillia Script files\69753.SDBJPN";
			String NewDB = @"c:\Users\Georg\Downloads\Xillia Script files\X69753";
			String TemplateDB = @"c:\Users\Georg\Downloads\Xillia Script files\XTemplate";
			String GracesDB = @"c:\Users\Georg\Downloads\Xillia Script files\GracesJapanese";
			//*/

			XilliaScriptFile XSF = new XilliaScriptFile( Filename );
			System.IO.File.Copy( TemplateDB, NewDB );
			InsertSQL( XSF, "Data Source=" + NewDB, "Data Source=" + GracesDB );

			//System.IO.File.WriteAllBytes(@"C:\TROPHY\newTrophyConf.trp", Encoding.UTF8.GetBytes(TROPSFM.ExportTropSFM(true)));
			//System.IO.File.WriteAllBytes(@"C:\TROPHY\newTrophy.trp", Encoding.UTF8.GetBytes(TROPSFM.ExportTropSFM(false)));

			return 0;
		}







		public static bool InsertSQL( XilliaScriptFile XSF, String ConnectionString, String ConnectionStringGracesJapanese ) {
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
				SQLiteParameter IdentifyStringParam = new SQLiteParameter();
				SQLiteParameter IdentifyPointerParam = new SQLiteParameter();


				CommandGracesJapanese.CommandText = "INSERT INTO Japanese (ID, string, debug) VALUES (?, ?, 0)";
				CommandGracesJapanese.Parameters.Add( JapaneseIDParam );
				CommandGracesJapanese.Parameters.Add( JapaneseParam );

				Command.CommandText = "INSERT INTO Text (ID, StringID, english, comment, updated, status, PointerRef, IdentifyString, IdentifyPointerRef) VALUES (?, ?, ?, \"\", 0, ?, ?, ?, ?)";
				Command.Parameters.Add( EnglishIDParam );
				Command.Parameters.Add( StringIDParam );
				Command.Parameters.Add( EnglishParam );  // Line.SENG
				Command.Parameters.Add( EnglishStatusParam );
				Command.Parameters.Add( PointerRefParam ); // Line.Location
				Command.Parameters.Add( IdentifyStringParam );
				Command.Parameters.Add( IdentifyPointerParam );

				CommandJapaneseID.CommandText = "SELECT MAX(ID)+1 FROM Japanese";

				CommandSearchJapanese.CommandText = "SELECT ID FROM Japanese WHERE string = ? AND debug = 0";
				CommandSearchJapanese.Parameters.Add( JapaneseSearchParam );

				int JPID;
				object JPMaxIDObject = CommandJapaneseID.ExecuteScalar();
				int JPMaxID = Int32.Parse( JPMaxIDObject.ToString() ); // wtf why doesn't this work directly?
				int ENID = 1;

				foreach ( XS xs in XSF.TextList ) {
					// fetch GracesJapanese ID or generate new & insert new text
					JapaneseSearchParam.Value = xs.Text;
					object JPIDobj = CommandSearchJapanese.ExecuteScalar();
					if ( JPIDobj != null ) {
						JPID = (int)JPIDobj;
					} else {
						JPID = JPMaxID++;
						JapaneseIDParam.Value = JPID;
						JapaneseParam.Value = xs.Text;
						CommandGracesJapanese.ExecuteNonQuery();
					}

					// insert text into English table
					EnglishIDParam.Value = ENID;
					StringIDParam.Value = JPID;
					EnglishParam.Value = xs.Text;
					EnglishStatusParam.Value = 0;
					PointerRefParam.Value = xs.PointerText; // == Trophy Name
					IdentifyStringParam.Value = xs.IDString;
					IdentifyPointerParam.Value = xs.PointerIDString;
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
