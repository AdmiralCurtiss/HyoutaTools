using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.LastRanker {
	class StringImport {
		public static int Import( List<string> args ) {
			if ( args.Count < 3 ) {
				Console.WriteLine( "Usage: file Database GracesJapanese" );
				return -1;
			}

			string InFile = args[0];
			string OutDatabase = args[1];
			string GracesJapanese = args[2];
			HyoutaTools.LastRanker.StringFile StringFile = new HyoutaTools.LastRanker.StringFile( System.IO.File.ReadAllBytes( InFile ) );
			System.IO.File.WriteAllBytes( OutDatabase, Properties.Resources.gndb_template );
			InsertSQL( StringFile, "Data Source=" + OutDatabase, "Data Source=" + GracesJapanese );

			return 0;
		}

		public static bool InsertSQL( HyoutaTools.LastRanker.StringFile StringFile, String ConnectionString, String ConnectionStringGracesJapanese ) {
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
				SQLiteParameter IdentifyStringParam = new SQLiteParameter();
				SQLiteParameter IdentifyPointerRefParam = new SQLiteParameter();
				SQLiteParameter UpdatedTimestampParam = new SQLiteParameter();
				SQLiteParameter JapaneseSearchParam = new SQLiteParameter();
				SQLiteParameter EnglishStatusParam = new SQLiteParameter();


				CommandGracesJapanese.CommandText = "INSERT INTO Japanese (ID, string, debug) VALUES (?, ?, 0)";
				CommandGracesJapanese.Parameters.Add( JapaneseIDParam );
				CommandGracesJapanese.Parameters.Add( JapaneseParam );

				Command.CommandText = "INSERT INTO Text (ID, StringID, english, comment, updated, status, PointerRef, IdentifyString, IdentifyPointerRef, UpdatedBy, UpdatedTimestamp)"
											 + " VALUES (?,  ?,        ?,       \"\",    0,       ?,      ?,          ?,              ?,                  \"HyoutaTools\", ?)";
				Command.Parameters.Add( EnglishIDParam );
				Command.Parameters.Add( StringIDParam );
				Command.Parameters.Add( EnglishParam );
				Command.Parameters.Add( EnglishStatusParam );
				Command.Parameters.Add( PointerRefParam );
				Command.Parameters.Add( IdentifyStringParam );
				Command.Parameters.Add( IdentifyPointerRefParam );
				Command.Parameters.Add( UpdatedTimestampParam );

				CommandJapaneseID.CommandText = "SELECT MAX(ID)+1 FROM Japanese";

				CommandSearchJapanese.CommandText = "SELECT ID FROM Japanese WHERE string = ? AND debug = 0";
				CommandSearchJapanese.Parameters.Add( JapaneseSearchParam );

				int JPID;
				object JPMaxIDObject = CommandJapaneseID.ExecuteScalar();
				int JPMaxID;
				try {
					JPMaxID = Int32.Parse( JPMaxIDObject.ToString() );
				} catch ( System.FormatException ) {
					// there's no ID in the database, just start with 0
					JPMaxID = 0;
				}
				int ENID = 1;

				foreach ( HyoutaTools.LastRanker.bscrString e in StringFile.Strings ) {
					// fetch GracesJapanese ID or generate new & insert new text
					JapaneseSearchParam.Value = e.String;
					object JPIDobj = CommandSearchJapanese.ExecuteScalar();
					if ( JPIDobj != null ) {
						JPID = (int)JPIDobj;
					} else {
						JPID = JPMaxID++;
						JapaneseIDParam.Value = JPID;
						JapaneseParam.Value = e.String;
						CommandGracesJapanese.ExecuteNonQuery();
					}

					// insert text into English table
					EnglishIDParam.Value = ENID;
					StringIDParam.Value = JPID;
					EnglishParam.Value = e.String;
					int status = 0;

					if ( e.String.StartsWith( "func" ) || e.String.StartsWith( "■" ) || e.String.Trim() == "" ) {
						status = -1;
					}

					EnglishStatusParam.Value = status;
					PointerRefParam.Value = e.Position;
					IdentifyStringParam.Value = "";
					IdentifyPointerRefParam.Value = 0;
					UpdatedTimestampParam.Value = Util.DateTimeToUnixTime( DateTime.Now );
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
