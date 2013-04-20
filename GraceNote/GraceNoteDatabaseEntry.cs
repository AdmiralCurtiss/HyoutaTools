using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote {
	public class GraceNoteDatabaseEntry {

		public string TextEN;
		public string TextJP;
		public int ID;
		public int JPID;
		public int Status;

		public int NewLineCount;
		public bool NewLineAtEnd;

		public static GraceNoteDatabaseEntry[] GetAllEntriesFromDatabase( String ConnectionString, String GracesJapaneseConnectionString ) {
			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>();

			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();
			SQLiteConnection ConnectionGJ = new SQLiteConnection( GracesJapaneseConnectionString );
			ConnectionGJ.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) )
			using ( SQLiteTransaction TransactionGJ = ConnectionGJ.BeginTransaction() )
			using ( SQLiteCommand CommandGJ = new SQLiteCommand( ConnectionGJ ) ) {
				Command.CommandText = "SELECT english, ID, StringID, status " +
									  "FROM Text ORDER BY ID";
				CommandGJ.CommandText = "SELECT string FROM Japanese WHERE ID = ?";
				SQLiteParameter StringIdParam = new SQLiteParameter();
				CommandGJ.Parameters.Add( StringIdParam );

				SQLiteDataReader r = Command.ExecuteReader();
				while ( r.Read() ) {
					String SQLText;

					try {
						SQLText = r.GetString( 0 ).Replace( "''", "'" );
					} catch ( System.InvalidCastException ) {
						SQLText = null;
					}

					int ID = r.GetInt32( 1 );
					int StringID = r.GetInt32( 2 );
					int Status = r.GetInt32( 3 );


					StringIdParam.Value = StringID;
					String JPText;
					try {
						JPText = ( (string)CommandGJ.ExecuteScalar() ).Replace( "''", "'" );
					} catch ( System.InvalidCastException ) {
						JPText = null;
					}


					GraceNoteDatabaseEntry de = new GraceNoteDatabaseEntry();
					de.TextEN = SQLText;
					de.TextJP = JPText;
					de.ID = ID;
					de.JPID = StringID;
					de.Status = Status;

					Entries.Add( de );
				}

				Transaction.Rollback();
				TransactionGJ.Rollback();
			}

			ConnectionGJ.Close();
			Connection.Close();

			return Entries.ToArray();
		}

	}
}
