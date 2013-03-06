using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.Vesperia.VVoicesGenerate {
	class Program {
		public static int Execute( List<string> args ) {
			String VOBTLDir = @"c:\Users\Georg\Documents\Tales of Vesperia\ToV_Voices_Japanese_PS3_mp3v8\PS3_JP\VOBTL\";
			String VoiceDB = @"c:\Users\Georg\Documents\Tales of Vesperia\_voice\VVoices";
			String GracesDB = @"c:\Users\Georg\Documents\Tales of Vesperia\_voice\GracesJapanese";


			String[] VOBTLFiles = System.IO.Directory.GetFiles( VOBTLDir );

			foreach ( String VOBTL in VOBTLFiles ) {
				String VOBTLcut = VOBTL.Substring( VOBTL.LastIndexOf( '\\' ) + 1 );
				if ( VOBTLcut.Contains( '.' ) ) {
					VOBTLcut = VOBTLcut.Remove( VOBTLcut.LastIndexOf( '.' ) );
				}
				InsertSQL( "Data Source=" + VoiceDB, "Data Source=" + GracesDB, VOBTLcut );
			}
			return 0;
		}

		public static int ENID = 1;


		public static bool InsertSQL( String ConnectionString, String ConnectionStringGracesJapanese, String VOBTL ) {
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
				SQLiteParameter VoiceParam = new SQLiteParameter();

				CommandGracesJapanese.CommandText = "INSERT INTO Japanese (ID, string, debug) VALUES (?, ?, 0)";
				CommandGracesJapanese.Parameters.Add( JapaneseIDParam );
				CommandGracesJapanese.Parameters.Add( JapaneseParam );

				Command.CommandText = "INSERT INTO Text (ID, StringID, english, comment, updated, status, PointerRef, voice) VALUES (?, ?, ?, null, 0, ?, ?, ?)";
				Command.Parameters.Add( EnglishIDParam );
				Command.Parameters.Add( StringIDParam );
				Command.Parameters.Add( EnglishParam );  // Line.SENG
				Command.Parameters.Add( EnglishStatusParam );
				Command.Parameters.Add( LocationParam ); // Line.Location
				Command.Parameters.Add( VoiceParam );

				CommandJapaneseID.CommandText = "SELECT MAX(ID)+1 FROM Japanese";

				CommandSearchJapanese.CommandText = "SELECT ID FROM Japanese WHERE string = ?";
				CommandSearchJapanese.Parameters.Add( JapaneseSearchParam );

				int JPID;
				object JPMaxIDObject = CommandJapaneseID.ExecuteScalar();
				int JPMaxID = Int32.Parse( JPMaxIDObject.ToString() ); // wtf why doesn't this work directly?



				String Entry = "\t(" + VOBTL + ')';



				// Name
				JapaneseSearchParam.Value = Entry;
				object JPIDobj = CommandSearchJapanese.ExecuteScalar();
				if ( JPIDobj != null ) {
					JPID = (int)JPIDobj;
				} else {
					JPID = JPMaxID++;
					JapaneseIDParam.Value = JPID;
					JapaneseParam.Value = Entry;
					CommandGracesJapanese.ExecuteNonQuery();
				}

				EnglishIDParam.Value = ENID;
				StringIDParam.Value = JPID;
				EnglishParam.Value = Entry;
				EnglishStatusParam.Value = 0;
				LocationParam.Value = -1;
				VoiceParam.Value = VOBTL;
				Command.ExecuteNonQuery();

				ENID++;

				Transaction.Commit();
				TransactionGracesJapanese.Commit();
			}
			ConnectionGracesJapanese.Close();
			Connection.Close();

			return true;
		}
	}
}
