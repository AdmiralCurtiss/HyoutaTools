using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote {
	public class GraceNoteDatabaseEntry {

		public string TextEN;
		public string TextJP;
		public string Comment;
		public int ID;

		/// <summary>Also known as StringID. References the corresponding GracesJapanese entry.</summary>
		public int JPID;
		public int Status;

		public int PointerRef;
		public string IdentifyString;
		public int IdentifyPointerRef;

		public int UpdatedTimestamp;
		public string UpdatedBy;
		
		public string Database;

		public GraceNoteDatabaseEntry() { }

		/// <summary>
		/// Creates a GNDB Entry suitable for inserting into a fresh database.
		/// All arguments except the original Japanese string are optional.
		/// TextEN defaults to a copy of TextJP if not provided.
		/// </summary>
		public GraceNoteDatabaseEntry( string TextJP, string TextEN = null, string Comment = "", int Status = 0, int PointerRef = 0, string IdentifyString = "", int IdentifyPointerRef = 0 ) {
			this.TextJP = TextJP;
			if ( TextEN == null ) {
				this.TextEN = TextJP;
			} else {
				this.TextEN = TextEN;
			}
			this.Comment = Comment;
			this.Status = Status;
			this.PointerRef = PointerRef;
			this.IdentifyString = IdentifyString;
			this.IdentifyPointerRef = IdentifyPointerRef;
		}

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
				Command.CommandText = "SELECT english, ID, StringID, status, comment, updated, PointerRef, IdentifyString, IdentifyPointerRef, UpdatedBy, UpdatedTimestamp " +
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
					String Comment;
					try {
						Comment = r.GetString( 4 );
					} catch ( System.InvalidCastException ) {
						Comment = null;
					}
					int Updated = r.GetInt32( 5 );
					int PointerRef = r.GetInt32( 6 );
					String IdentifyString;
					try {
						IdentifyString = r.GetString( 7 );
					} catch ( System.InvalidCastException ) {
						IdentifyString = null;
					}
					
					int IdentifyPointerRef;
					try {
						IdentifyPointerRef = r.GetInt32( 8 );
					} catch ( System.InvalidCastException ) {
						IdentifyPointerRef = 0;
					}

					String UpdatedBy;
					try {
						UpdatedBy = r.GetString( 9 );
					} catch ( System.InvalidCastException ) {
						UpdatedBy = null;
					}
					
					int UpdatedTimestamp;
					try {
						UpdatedTimestamp = r.GetInt32( 10 );
					} catch ( System.InvalidCastException ) {
						UpdatedTimestamp = 0;
					}


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
					de.Comment = Comment;
					de.PointerRef = PointerRef;
					de.IdentifyString = IdentifyString;
					de.IdentifyPointerRef = IdentifyPointerRef;
					de.UpdatedBy = UpdatedBy;
					de.UpdatedTimestamp = UpdatedTimestamp;
					de.Database = ConnectionString;

					Entries.Add( de );
				}

				Transaction.Rollback();
				TransactionGJ.Rollback();
			}

			ConnectionGJ.Close();
			Connection.Close();

			return Entries.ToArray();
		}

		/// <summary>Utility Function to insert Entries into a GraceNote database. Usually used when ripping text from a game file.</summary>
		/// <param name="Entries">The Entries to insert. Required values in the entries are: TextJP, TextEN, Comment, Status, PointerRef, IdentifyString, and IdentifyPointerRef. The rest is filled automatically.</param>
		public static void InsertSQL( GraceNoteDatabaseEntry[] Entries, String ConnectionString, String ConnectionStringGracesJapanese ) {
			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			SQLiteConnection ConnectionGracesJapanese = new SQLiteConnection( ConnectionStringGracesJapanese );
			Connection.Open();
			ConnectionGracesJapanese.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteTransaction TransactionGracesJapanese = ConnectionGracesJapanese.BeginTransaction() )
			using ( SQLiteCommand CommandInsertEntry = new SQLiteCommand( Connection ) )
			using ( SQLiteCommand CommandInsertJapanese = new SQLiteCommand( ConnectionGracesJapanese ) )
			using ( SQLiteCommand CommandGetMaxJapaneseID = new SQLiteCommand( ConnectionGracesJapanese ) )
			using ( SQLiteCommand CommandSearchJapanese = new SQLiteCommand( ConnectionGracesJapanese ) ) {
				SQLiteParameter JapaneseIDParam = new SQLiteParameter();
				SQLiteParameter JapaneseParam = new SQLiteParameter();

				SQLiteParameter EnglishIDParam = new SQLiteParameter();
				SQLiteParameter StringIDParam = new SQLiteParameter();
				SQLiteParameter EnglishParam = new SQLiteParameter();
				SQLiteParameter CommentParam = new SQLiteParameter();
				SQLiteParameter EnglishStatusParam = new SQLiteParameter();
				SQLiteParameter PointerRefParam = new SQLiteParameter();
				SQLiteParameter IdentifyStringParam = new SQLiteParameter();
				SQLiteParameter IdentifyPointerRefParam = new SQLiteParameter();
				SQLiteParameter UpdatedTimestampParam = new SQLiteParameter();

				SQLiteParameter JapaneseSearchParam = new SQLiteParameter();


				CommandInsertJapanese.CommandText = "INSERT INTO Japanese (ID, string, debug) VALUES (?, ?, 0)";
				CommandInsertJapanese.Parameters.Add( JapaneseIDParam );
				CommandInsertJapanese.Parameters.Add( JapaneseParam );

				CommandInsertEntry.CommandText = "INSERT INTO Text (ID, StringID, english, comment, updated, status, PointerRef, IdentifyString, IdentifyPointerRef, UpdatedBy, UpdatedTimestamp)"
														+ " VALUES (?,  ?,        ?,       ?,       0,       ?,      ?,          ?,              ?,                  \"HyoutaTools\", ?)";
				CommandInsertEntry.Parameters.Add( EnglishIDParam );
				CommandInsertEntry.Parameters.Add( StringIDParam );
				CommandInsertEntry.Parameters.Add( EnglishParam );
				CommandInsertEntry.Parameters.Add( CommentParam );
				CommandInsertEntry.Parameters.Add( EnglishStatusParam );
				CommandInsertEntry.Parameters.Add( PointerRefParam );
				CommandInsertEntry.Parameters.Add( IdentifyStringParam );
				CommandInsertEntry.Parameters.Add( IdentifyPointerRefParam );
				CommandInsertEntry.Parameters.Add( UpdatedTimestampParam );

				CommandGetMaxJapaneseID.CommandText = "SELECT MAX(ID)+1 FROM Japanese";

				CommandSearchJapanese.CommandText = "SELECT ID FROM Japanese WHERE string = ? AND debug = 0";
				CommandSearchJapanese.Parameters.Add( JapaneseSearchParam );

				int JPID;
				object JPMaxIDObject = CommandGetMaxJapaneseID.ExecuteScalar();
				int JPMaxID;
				try {
					JPMaxID = Int32.Parse( JPMaxIDObject.ToString() );
				} catch ( System.FormatException ) {
					// there's no ID in the database, just start with 0
					JPMaxID = 0;
				}
				int ENID = 1;

				foreach ( GraceNoteDatabaseEntry e in Entries ) {
					// fetch GracesJapanese ID or generate new & insert new text
					JapaneseSearchParam.Value = e.TextJP;
					object JPIDobj = CommandSearchJapanese.ExecuteScalar();
					if ( JPIDobj != null ) {
						JPID = (int)JPIDobj;
					} else {
						JPID = JPMaxID++;
						JapaneseIDParam.Value = JPID;
						JapaneseParam.Value = e.TextJP;
						CommandInsertJapanese.ExecuteNonQuery();
					}

					// insert text into English table
					EnglishIDParam.Value = ENID;
					StringIDParam.Value = JPID;
					EnglishParam.Value = e.TextEN;
					CommentParam.Value = e.Comment;
					EnglishStatusParam.Value = e.Status;
					PointerRefParam.Value = e.PointerRef;
					IdentifyStringParam.Value = e.IdentifyString;
					IdentifyPointerRefParam.Value = e.IdentifyPointerRef;
					UpdatedTimestampParam.Value = Util.DateTimeToUnixTime( DateTime.Now );
					CommandInsertEntry.ExecuteNonQuery();

					ENID++;
				}
				Transaction.Commit();
				TransactionGracesJapanese.Commit();
			}
			ConnectionGracesJapanese.Close();
			Connection.Close();

			return;
		}

	}
}
