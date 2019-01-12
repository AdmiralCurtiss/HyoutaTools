using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.MapList {
	class Program {
		public static int Execute( List<string> args ) {
			// 0xCB20

			if ( args.Count != 2 ) {
				Console.WriteLine( "Generates a scenario db for use in Tales.Vesperia.Website from a MAPLIST.DAT." );
				Console.WriteLine( "Usage: maplist.dat scenario.db" );
				return -1;
			}

			String maplistFilename = args[0];
			string connStr = "Data Source=" + args[1];

			using ( var conn = new System.Data.SQLite.SQLiteConnection( connStr ) ) {
				conn.Open();
				using ( var ta = conn.BeginTransaction() ) {
					SqliteUtil.Update( ta, "CREATE TABLE descriptions( filename TEXT PRIMARY KEY, shortdesc TEXT, desc TEXT )" );
					int i = 0;
					foreach ( MapName m in new MapList( maplistFilename, Util.Endianness.BigEndian, Util.Bitness.B32 ).MapNames ) {
						Console.WriteLine( i + " = " + m.ToString() );
						List<string> p = new List<string>();
						p.Add( "VScenario" + i );
						p.Add( m.Name1 != "dummy" ? m.Name1 : m.Name3 );
						p.Add( m.Name1 != "dummy" ? m.Name1 : m.Name3 );
						SqliteUtil.Update( ta, "INSERT INTO descriptions ( filename, shortdesc, desc ) VALUES ( ?, ?, ? )", p );
						++i;
					}
					ta.Commit();
				}
				conn.Close();
			}

			return 0;
		}
	}
}
