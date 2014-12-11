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
			using ( SQLiteConnection connection = new SQLiteConnection( connString ) ) {
				connection.Open();
				return SelectScalar( connection, statement, parameters );
			}
		}
		public static object SelectScalar( SQLiteConnection connection, string statement, IEnumerable<object> parameters ) {
			object retval = null;
			using ( SQLiteTransaction transaction = connection.BeginTransaction() ) {
				retval = SelectScalar( transaction, statement, parameters );
				transaction.Commit();
			}
			return retval;
		}
		public static object SelectScalar( SQLiteTransaction transaction, string statement, IEnumerable<object> parameters ) {
			object retval = null;
			using ( SQLiteCommand Command = new SQLiteCommand() ) {
				Command.Connection = transaction.Connection;
				Command.CommandText = statement;
				foreach ( object p in parameters ) {
					SQLiteParameter sqp = new SQLiteParameter();
					sqp.Value = p;
					Command.Parameters.Add( sqp );
				}
				retval = Command.ExecuteScalar();
			}
			return retval;
		}

		public static List<Object[]> SelectArray( string connString, string statement ) {
			return SelectArray( connString, statement, new object[0] );
		}
		public static List<Object[]> SelectArray( string connString, string statement, IEnumerable<object> parameters ) {
			using ( SQLiteConnection Connection = new SQLiteConnection( connString ) ) {
				Connection.Open();
				return SelectArray( Connection, statement, parameters );
			}
		}
		public static List<Object[]> SelectArray( SQLiteConnection connection, string statement, IEnumerable<object> parameters ) {
			List<Object[]> retval = null;
			using ( SQLiteTransaction transaction = connection.BeginTransaction() ) {
				retval = SelectArray( transaction, statement, parameters );
				transaction.Commit();
			}
			return retval;
		}
		public static List<Object[]> SelectArray( SQLiteTransaction transaction, string statement, IEnumerable<object> parameters ) {
			List<Object[]> rows = null;
			using ( SQLiteCommand Command = new SQLiteCommand() ) {
				Command.Connection = transaction.Connection;
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
			}
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
