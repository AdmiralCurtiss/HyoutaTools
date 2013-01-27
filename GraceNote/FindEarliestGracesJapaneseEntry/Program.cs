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
					CommandEFetch.CommandText = "SELECT ID, StringID FROM Text ORDER BY ID";
					SQLiteDataReader r = CommandEFetch.ExecuteReader();

					List<KeyValuePair<int, int>> DatabaseEntries = new List<KeyValuePair<int, int>>();
					while ( r.Read() ) {
						int ID = r.GetInt32( 0 );
						int StringID = r.GetInt32( 1 );
						DatabaseEntries.Add( new KeyValuePair<int, int>( ID, StringID ) );
					}
					r.Close();

					CommandJ.CommandText = "PRAGMA case_sensitive_like = ON";
					int affected = CommandJ.ExecuteNonQuery();

					CommandEUpdate.CommandText = "UPDATE Text SET StringID = ? WHERE ID = ?";
					CommandJ.CommandText =
						"SELECT ID FROM Japanese WHERE CAST(string AS BLOB) = "
						+ "( SELECT CAST(string AS BLOB) FROM Japanese WHERE ID = ? ) ORDER BY ID ASC";
					SQLiteParameter ParamEStringId = new SQLiteParameter();
					SQLiteParameter ParamEId = new SQLiteParameter();
					SQLiteParameter ParamJId = new SQLiteParameter();
					CommandEUpdate.Parameters.Add( ParamEStringId );
					CommandEUpdate.Parameters.Add( ParamEId );
					CommandJ.Parameters.Add( ParamJId );

					foreach ( KeyValuePair<int, int> e in DatabaseEntries ) {
						ParamJId.Value = e.Value;
						int? EarliestStringId = (int?)CommandJ.ExecuteScalar();
						if ( EarliestStringId != null ) {
							ParamEId.Value = e.Key;
							ParamEStringId.Value = EarliestStringId;
							CommandEUpdate.ExecuteNonQuery();
						}
					}

					TransactionJ.Rollback();
					TransactionE.Commit();
				}
			}
		}

		public static int Execute( string[] args ) {

			if ( args.Length != 2 ) {
				Console.WriteLine( "FindEarliestGracesJapaneseEntry Database GracesJapanese" );
				return -1;
			}

			FindEarliestGracesJapaneseEntry( "Data Source=" + args[0], "Data Source=" + args[1] );
			return 0;
		}
	}
}
