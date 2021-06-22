using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.FindEarliestGracesJapaneseEntry {
	static class Program {
		public static void FindEarliestGracesJapaneseEntry( String ConnectionString, String GracesJapaneseConnectionString ) {
			using ( SQLiteConnection ConnectionE = new SQLiteConnection( ConnectionString ) )
			using ( SQLiteConnection ConnectionJ = new SQLiteConnection( GracesJapaneseConnectionString ) ) {
				ConnectionE.Open();
				ConnectionJ.Open();

				using ( SQLiteTransaction TransactionE = ConnectionE.BeginTransaction() )
				using ( SQLiteTransaction TransactionJ = ConnectionJ.BeginTransaction() )
				using ( SQLiteCommand CommandEFetch = new SQLiteCommand( ConnectionE ) )
				using ( SQLiteCommand CommandEUpdate = new SQLiteCommand( ConnectionE ) )
				using ( SQLiteCommand CommandJ = new SQLiteCommand( ConnectionJ ) ) {

					// fetch, from the individual game file Database, all IDs and corresponding GracesJapanese StringIDs
					CommandEFetch.CommandText = "SELECT ID, StringID FROM Text ORDER BY ID";
					SQLiteDataReader r = CommandEFetch.ExecuteReader();
					List<GraceNoteDatabaseEntry> DatabaseEntries = new List<GraceNoteDatabaseEntry>();
					while ( r.Read() ) {
						int ID = r.GetInt32( 0 );
						int StringID = r.GetInt32( 1 );

						var gn = new GraceNoteDatabaseEntry();
						gn.ID = ID;
						gn.JPID = StringID;
						DatabaseEntries.Add( gn );
					}
					r.Close();

					CommandJ.CommandText = "PRAGMA case_sensitive_like = ON";
					int affected = CommandJ.ExecuteNonQuery();

					// This finds all entries in GracesJapanese that have the same Japanese text as the current game file DB entry
					CommandJ.CommandText =
						"SELECT ID FROM Japanese WHERE CAST(string AS BLOB) = "
						+ "( SELECT CAST(string AS BLOB) FROM Japanese WHERE ID = ? ) ORDER BY ID ASC";
					SQLiteParameter ParamJId = new SQLiteParameter();
					CommandJ.Parameters.Add( ParamJId );

					// This updates the game file DB with the new StringID
					CommandEUpdate.CommandText = "UPDATE Text SET StringID = ? WHERE ID = ?";
					SQLiteParameter ParamEStringId = new SQLiteParameter();
					SQLiteParameter ParamEId = new SQLiteParameter();
					CommandEUpdate.Parameters.Add( ParamEStringId );
					CommandEUpdate.Parameters.Add( ParamEId );

					int entryCounter = 0;
					int alreadyCorrectChainCounter = 0;
					foreach ( var e in DatabaseEntries ) {
						++entryCounter;

						// get the lowest StringID
						ParamJId.Value = e.JPID;
						int? EarliestStringId = (int?)CommandJ.ExecuteScalar();

						// and put it into the game file DB, if needed
						if ( EarliestStringId != null && EarliestStringId != e.JPID ) {
							alreadyCorrectChainCounter = 0;
							Console.WriteLine( "Changing Entry #" + e.ID + " from StringID " + e.JPID + " to " + EarliestStringId );

							ParamEId.Value = e.ID;
							ParamEStringId.Value = EarliestStringId;
							CommandEUpdate.ExecuteNonQuery();
						} else {
							++alreadyCorrectChainCounter;
							if ( alreadyCorrectChainCounter >= 10 ) {
								Console.WriteLine( "Processing Entry " + entryCounter + " of " + DatabaseEntries.Count );
								alreadyCorrectChainCounter = 0;
							}
						}
					}

					TransactionJ.Rollback();
					TransactionE.Commit();
				}
			}
		}

		public static int Execute( List<string> args ) {

			if ( args.Count != 2 ) {
				Console.WriteLine( "FindEarliestGracesJapaneseEntry Database GracesJapanese" );
				return -1;
			}

			FindEarliestGracesJapaneseEntry( "Data Source=" + args[0], "Data Source=" + args[1] );
			return 0;
		}
	}
}
