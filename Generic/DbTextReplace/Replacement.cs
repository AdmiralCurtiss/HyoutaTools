using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.Generic.DbTextReplace {
	public static class Replacement {
		public static int Replace(string[] args) {
			string Database = args[0]; //@"c:\_svn\GraceNote\GraceNote\DanganRonpaBestOfDB\GracesJapanese";

			// remove the dumb unicode byte order mark
			
			List<object[]> objects =
				GenericSqliteSelect( "Data Source=" + Database,
					//"SELECT id, string FROM japanese ORDER BY id ASC", new object[0] );
					"SELECT id, english FROM text ORDER BY id ASC", new object[0] );

			SQLiteConnection Connection = new SQLiteConnection( "Data Source=" + Database );
			Connection.Open();
			
			for ( int i = 0; i < objects.Count; ++i ) {
				byte[] b = Encoding.Unicode.GetBytes( (string)objects[i][1] );
				if ( b.Length >= 2 && b[0] == '\xff' && b[1] == '\xfe' ) {
					int ID = (int)objects[i][0];
					string str = (string)objects[i][1];
					string fstr = Encoding.Unicode.GetString( b, 2, b.Length - 2 );
					Console.WriteLine( fstr );
					Util.GenericSqliteUpdate( Connection,
						//"UPDATE japanese SET string = ? WHERE id = ?",
						"UPDATE Text SET english = ?, updated = 1 WHERE id = ?",
						new object[] {
						fstr, ID	
					});
				}
			}

			Connection.Close();

			return 0;

		}

		public static List<object[]> GenericSqliteSelect( string connString, string statement, IEnumerable<object> parameters ) {
			SQLiteConnection Connection = new SQLiteConnection( connString );
			Connection.Open();
			List<object[]> retval = GenericSqliteSelect( Connection, statement, parameters );
			Connection.Close();
			return retval;
		}
		public static List<object[]> GenericSqliteSelect( SQLiteConnection Connection, string statement, IEnumerable<object> parameters )
		{
			List<object[]> retval = null;

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText = statement;
				foreach ( object p in parameters ) {
					SQLiteParameter sqp = new SQLiteParameter();
					sqp.Value = p;
					Command.Parameters.Add( sqp );
				}
				SQLiteDataReader rd = Command.ExecuteReader();

				List<object[]> objs = new List<object[]>();
				while ( rd.Read() ) {
					object[] obja = new object[rd.FieldCount];
					for ( int i = 0; i < rd.FieldCount; ++i ) {
						obja[i] = rd.GetValue( i );
					}
					objs.Add( obja );
				}

				retval = objs;
				Transaction.Commit();
			}

			return retval;
		}

	}
}
