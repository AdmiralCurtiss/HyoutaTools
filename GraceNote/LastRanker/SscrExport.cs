using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.LastRanker;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.LastRanker {
	class SscrExport {
		public static int Export( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: original.sscr db new.sscr" );
				return -1;
			}

			string SourceFile = args[0];
			string InDatabase = args[1];
			string OutFile = args[2];
			SSCR SscrFile = new SSCR( System.IO.File.ReadAllBytes( SourceFile ) );

			ReadDatabase( SscrFile, "Data Source=" + InDatabase );

			SscrFile.CreateFile( OutFile );

			return 0;
		}

		public static void ReadDatabase( SSCR SscrFile, string ConnectionString ) {
			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText = "SELECT english, PointerRef FROM Text ORDER BY ID";
				SQLiteDataReader r = Command.ExecuteReader();
				int NameCounter = 0;
				int SysTextCounter = 0;
				int SomethingCounter = 0;
				while ( r.Read() ) {
					String SQLText;
					try { SQLText = r.GetString( 0 ).Replace( "''", "'" ); }
					catch ( System.InvalidCastException ) { SQLText = ""; }
					int PointerRef = r.GetInt32( 1 );

					switch ( PointerRef ) {
						case 1:
							SscrFile.Names[NameCounter].Name = SQLText;
							NameCounter++;
							break;
						case 2:
							SscrFile.SystemTerms[SysTextCounter].Term = SQLText;
							SysTextCounter++;
							break;
						case 3:
							SscrFile.Somethings[SomethingCounter].Text = SQLText;
							SomethingCounter++;
							break;
					}
				}

				Transaction.Rollback();
			}
			return;
		}

	}
}
