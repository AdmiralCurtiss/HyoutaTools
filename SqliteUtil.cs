using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools {
	public static class SqliteUtil {
		public static object SelectScalar( string connString, string statement ) {
			return SelectScalar( connString, statement, new object[0] );
		}
		public static object SelectScalar( string connString, string statement, IEnumerable<object> parameters ) {
			object retval = null;
			SQLiteConnection Connection = new SQLiteConnection( connString );
			Connection.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText = statement;
				foreach ( object p in parameters ) {
					SQLiteParameter sqp = new SQLiteParameter();
					sqp.Value = p;
					Command.Parameters.Add( sqp );
				}
				retval = Command.ExecuteScalar();
				Transaction.Commit();
			}
			Connection.Close();

			return retval;
		}

		public static List<Object[]> SelectArray( string connString, string statement, IEnumerable<object> parameters ) {
			List<Object[]> rows = null;
			SQLiteConnection Connection = new SQLiteConnection( connString );
			Connection.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText = statement;
				foreach ( object p in parameters ) {
					SQLiteParameter sqp = new SQLiteParameter();
					sqp.Value = p;
					Command.Parameters.Add( sqp );
				}

				SQLiteDataReader rd = Command.ExecuteReader();

				rows = new List<object[]>();
				if ( rd.Read() ) {
					Object[] fields = new Object[rd.FieldCount];
					do {
						rd.GetValues( fields );
						rows.Add( fields );
						fields = new Object[rd.FieldCount];
					} while ( rd.Read() );
				}

				Transaction.Commit();
			}
			Connection.Close();

			return rows;
		}

		public static int Update( string connString, string statement ) {
			return Update( connString, statement, new object[0] );
		}
		public static int Update( string connString, string statement, IEnumerable<object> parameters ) {
			SQLiteConnection Connection = new SQLiteConnection( connString );
			Connection.Open();
			int retval = Update( Connection, statement, parameters );
			Connection.Close();
			return retval;
		}
		public static int Update( SQLiteConnection connection, string statement, IEnumerable<object> parameters ) {
			int affected = -1;
			using ( SQLiteTransaction Transaction = connection.BeginTransaction() ) {
				affected = Update( Transaction, statement, parameters );
				Transaction.Commit();
			}
			return affected;
		}
		public static int Update( SQLiteTransaction transaction, string statement, IEnumerable<object> parameters ) {
			int affected = -1;

			using ( SQLiteCommand Command = new SQLiteCommand() ) {
				Command.Connection = transaction.Connection;
				Command.CommandText = statement;
				foreach ( object p in parameters ) {
					SQLiteParameter sqp = new SQLiteParameter();
					sqp.Value = p;
					Command.Parameters.Add( sqp );
				}
				affected = Command.ExecuteNonQuery();
			}

			return affected;
		}
	}
}
