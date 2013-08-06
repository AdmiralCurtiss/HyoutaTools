using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.LastRanker;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.LastRanker {
	class bscrExport {
		public static int Export( List<string> args ) {
			if ( args.Count < 3 ) {
				Console.WriteLine( "Usage: db bscr.old bscr.new" );
				return -1;
			}

			string InDatabase = args[0];
			string InFile = args[1];
			string OutFile = args[2];

			bscr b = new bscr( InFile );
			ReadDatabase( b, "Data Source=" + InDatabase );
			b.CreateFile( OutFile );

			return 0;
		}

		public static void ReadDatabase( bscr bscrFile, string ConnectionString ) {
			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();

			bscrFile.Strings = new List<bscrString>();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText = "SELECT english, PointerRef FROM Text ORDER BY PointerRef";
				SQLiteDataReader r = Command.ExecuteReader();
				while ( r.Read() ) {
					String SQLText;

					try {
						SQLText = r.GetString( 0 ).Replace( "''", "'" );
					} catch ( System.InvalidCastException ) {
						SQLText = "";
					}

					int PointerRef = r.GetInt32( 1 );


					bscrString b = new bscrString();
					b.Position = (uint)PointerRef;
					b.String = SQLText;
					bscrFile.Strings.Add( b );
				}

				Transaction.Rollback();
			}
			return;
		}

	}
}
