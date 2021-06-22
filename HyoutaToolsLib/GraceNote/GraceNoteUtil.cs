using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.GraceNote {
	public static class GraceNoteUtil {
		public static void GenerateEmptyDatabase( string path ) {
			if ( System.IO.File.Exists( path ) ) {
				System.IO.File.Delete( path );
			}

			SQLiteConnection connection = new SQLiteConnection( "Data Source=" + path );
			connection.Open();
			using ( SQLiteTransaction transaction = connection.BeginTransaction() ) {
				using ( SQLiteCommand command = new SQLiteCommand( connection ) ) {
					command.CommandText = "CREATE TABLE Text(ID int primary key, StringID int, english text, comment text, updated tinyint, status tinyint, PointerRef integer, IdentifyString text, IdentifyPointerRef int, UpdatedBy text, UpdatedTimestamp int)";
					command.ExecuteNonQuery();
				}

				using ( SQLiteCommand command = new SQLiteCommand( connection ) ) {
					command.CommandText = "CREATE TABLE History(ID int, english text, comment text, status tinyint, UpdatedBy text, UpdatedTimestamp int)";
					command.ExecuteNonQuery();
				}

				using ( SQLiteCommand command = new SQLiteCommand( connection ) ) {
					command.CommandText = "CREATE INDEX History_ID_Index ON History(ID)";
					command.ExecuteNonQuery();
				}

				transaction.Commit();
			}
			connection.Close();
		}

		public static void GenerateEmptyGracesJapanese( string path ) {
			if ( System.IO.File.Exists( path ) ) {
				System.IO.File.Delete( path );
			}

			SQLiteConnection connection = new SQLiteConnection( "Data Source=" + path );
			connection.Open();
			using ( SQLiteTransaction transaction = connection.BeginTransaction() ) {
				using ( SQLiteCommand command = new SQLiteCommand( connection ) ) {
					command.CommandText = "CREATE TABLE Japanese(ID int primary key, string text, debug int)";
					command.ExecuteNonQuery();
				}

				using ( SQLiteCommand command = new SQLiteCommand( connection ) ) {
					command.CommandText = "CREATE TABLE descriptions(filename TEXT PRIMARY KEY, shortdesc TEXT, desc TEXT)";
					command.ExecuteNonQuery();
				}

				transaction.Commit();
			}
			connection.Close();
		}
	}
}
