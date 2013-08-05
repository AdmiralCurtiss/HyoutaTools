using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using HyoutaTools.LastRanker;

namespace HyoutaTools.GraceNote.LastRanker {
	class StringExport {
		public static int Export( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: db outfile" );
				return -1;
			}

			string InDatabase = args[0];
			string OutFile = args[1];
			StringFile StringFile = new StringFile();
			ReadDatabase( StringFile, "Data Source=" + InDatabase );
			StringFile.CreateFile( OutFile );

			return 0;
		}

		public static void ReadDatabase( StringFile StringFile, string ConnectionString ) {
			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();

			StringFile.Strings = new List<bscrString>();

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
					StringFile.Strings.Add( b );
				}

				Transaction.Rollback();
			}
			return;
		}

	}
}
