using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace HyoutaTools.GraceNote.FindIdenticalEntries {
	class Finder {
		public static int Execute( List<string> args ) {
			List<string> Game1_Databases = new List<string>();
			List<string> Game2_Databases = new List<string>();
			string Game1_GracesJapanese = null;
			string Game2_GracesJapanese = null;
			string DiffLogPath = null;
			string MatchLogPath = null;

			for ( int i = 0; i < args.Count; ++i ) {
				switch ( args[i] ) {
					case "-db1": Game1_Databases.Add( args[++i] ); break;
					case "-db2": Game2_Databases.Add( args[++i] ); break;
					case "-gj1": Game1_GracesJapanese = args[++i]; break;
					case "-gj2": Game2_GracesJapanese = args[++i]; break;
					case "-difflog": DiffLogPath = args[++i]; break;
					case "-matchlog": MatchLogPath = args[++i]; break;
				}
			}

			if ( Game1_Databases.Count == 0 || Game2_Databases.Count == 0 || Game1_GracesJapanese == null || Game2_GracesJapanese == null ) {
				Console.WriteLine( "Tool to compare entries from two games." );
				Console.WriteLine( "This can be used for checking consistency between e.g. multiple games in the same series." );
				Console.WriteLine();
				Console.WriteLine( "Usage:" );
				Console.WriteLine( " Required:" );
				Console.WriteLine( "  -db1 path       Add a database of Game 1. (can be used multiple times)" );
				Console.WriteLine( "  -db2 path       Add a database of Game 2. (can be used multiple times)" );
				Console.WriteLine( "  -gj1 path       GracesJapanese of Game 1." );
				Console.WriteLine( "  -gj2 path       GracesJapanese of Game 2." );
				Console.WriteLine();
				Console.WriteLine( " Optional:" );
				Console.WriteLine( "  -difflog path   Log all entries where the Japanese matches, but the English does not." );
				Console.WriteLine( "  -matchlog path  Log all entries where the Japanese and English both match." );
				return -1;
			}

			List<GraceNoteDatabaseEntry> Game1_Entries = new List<GraceNoteDatabaseEntry>();
			List<GraceNoteDatabaseEntry> Game2_Entries = new List<GraceNoteDatabaseEntry>();
			
			Stream DiffLogStream = null;
			StreamWriter DiffLogWriter = null;
			Stream MatchLogStream = null;
			StreamWriter MatchLogWriter = null;

			if ( DiffLogPath != null ) {
				DiffLogStream = new FileStream( DiffLogPath, FileMode.Append );
			} else {
				DiffLogStream = Stream.Null;
			}
			if ( MatchLogPath != null ) {
				MatchLogStream = new FileStream( MatchLogPath, FileMode.Append );
			} else {
				MatchLogStream = Stream.Null;
			}

			DiffLogWriter = new StreamWriter( DiffLogStream );
			MatchLogWriter = new StreamWriter( MatchLogStream );

			//DirectoryInfo di1 = new DirectoryInfo( @"e:\_\ToV\" );
			//DirectoryInfo di2 = new DirectoryInfo( @"e:\_\ToX\" );
			//foreach ( var x in di1.GetFiles() ) {
			//	Game1_Databases.Add( x.FullName );
			//}
			//foreach ( var x in di2.GetFiles() ) {
			//	Game2_Databases.Add( x.FullName );
			//}

			foreach ( var db in Game1_Databases ) {
				Game1_Entries.AddRange(
					GraceNoteDatabaseEntry.GetAllEntriesFromDatabase( "Data Source=" + db, "Data Source=" + Game1_GracesJapanese )
				);
			}
			foreach ( var db in Game2_Databases ) {
				Game2_Entries.AddRange(
					GraceNoteDatabaseEntry.GetAllEntriesFromDatabase( "Data Source=" + db, "Data Source=" + Game2_GracesJapanese )
				);
			}

			Regex VariableRemoveRegex = new Regex( "<[^<>]+>" );
			Regex VesperiaFuriRemoveRegex = new Regex( "\r[(][0-9]+[,][\\p{IsHiragana}\\p{IsKatakana}]+[)]" );
			foreach ( var e1 in Game1_Entries ) {
				string j1 = VesperiaFuriRemoveRegex.Replace( VariableRemoveRegex.Replace( e1.TextJP, "" ), "" );
				foreach ( var e2 in Game2_Entries ) {
					string j2 = VariableRemoveRegex.Replace( e2.TextJP, "" );
					if ( j1 == j2 ) {
						if ( e1.TextEN != e2.TextEN ) {
							DiffLogWriter.WriteLine( j1 );
							DiffLogWriter.WriteLine( e1.Database + "/" + e1.ID + ": " + e1.TextEN );
							DiffLogWriter.WriteLine( e2.Database + "/" + e2.ID + ": " + e2.TextEN );
							DiffLogWriter.WriteLine();
							DiffLogWriter.WriteLine( "------------------------------------------------------" );
							DiffLogWriter.WriteLine();
						} else {
							MatchLogWriter.WriteLine( j1 );
							MatchLogWriter.WriteLine( e1.Database + "/" + e1.ID + ": " + e1.TextEN );
							MatchLogWriter.WriteLine( e2.Database + "/" + e2.ID + ": " + e2.TextEN );
							MatchLogWriter.WriteLine();
							MatchLogWriter.WriteLine( "------------------------------------------------------" );
							MatchLogWriter.WriteLine();
							Util.GenericSqliteUpdate( e1.Database, "UPDATE Text SET updated = 1, status = 4 WHERE ID = " + e1.ID );
						}
					}
				}
			}

			MatchLogWriter.Close();
			MatchLogStream.Close();
			DiffLogWriter.Close();
			DiffLogStream.Close();

			return 0;
		}
	}
}
