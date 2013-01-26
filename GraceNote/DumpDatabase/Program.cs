using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;

namespace HyoutaTools.GraceNote.DumpDatabase {
	class Program {
		public static String GetJapanese( String ConnectionString, int ID ) {
			String SQLText = "";

			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText = "SELECT string FROM Japanese WHERE ID = " + ID.ToString();
				SQLiteDataReader r = Command.ExecuteReader();
				while ( r.Read() ) {
					try {
						SQLText = r.GetString( 0 ).Replace( "''", "'" );
					} catch ( System.InvalidCastException ) {
						SQLText = "";
					}
				}

				Transaction.Rollback();
			}

			return SQLText;
		}

		public static ScenarioFile[] GetSQL( String ConnectionString, int FileNumber, String GracesJapaneseConnectionString, bool DumpIdentifyerStrings, bool ForceJapaneseDump, bool DumpDebug ) {
			List<ScenarioFile> ScenarioFiles = new List<ScenarioFile>();

			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				if ( FileNumber == -1 ) {
					Command.CommandText = "SELECT english, PointerRef, StringID";
					if ( DumpIdentifyerStrings ) {
						Command.CommandText += ", IdentifyString";
					}
					Command.CommandText += " FROM Text"
						+ ( DumpDebug ? "" : " WHERE status != -1" )
						+ " ORDER BY PointerRef";
				} else {
					// for VScenarioMissing
					Command.CommandText = "SELECT english, PointerRef, StringID FROM Text WHERE"
						+ ( DumpDebug ? "" : " status != -1 AND" )
						+ " OriginalFile = " + FileNumber.ToString() + " ORDER BY PointerRef";
				}
				SQLiteDataReader r = Command.ExecuteReader();
				while ( r.Read() ) {
					String SQLText;

					try {
						SQLText = r.GetString( 0 ).Replace( "''", "'" );
					} catch ( System.InvalidCastException ) {
						SQLText = null;
					}

					int PointerRef = r.GetInt32( 1 );
					int StringID = r.GetInt32( 2 );

					String IdentifyString = null;

					if ( DumpIdentifyerStrings ) {
						try {
							IdentifyString = r.GetString( 3 );
						} catch ( System.InvalidCastException ) {
							IdentifyString = null;
						}
					}

					ScenarioFile sc = new ScenarioFile();

					if ( ForceJapaneseDump || String.IsNullOrEmpty( SQLText ) ) {
						sc.Text = GetJapanese( GracesJapaneseConnectionString, StringID );
					} else {
						sc.Text = SQLText;
					}

					sc.Pointer = PointerRef;
					sc.IdentifyerString = IdentifyString;

					ScenarioFiles.Add( sc );
				}

				Transaction.Rollback();
			}
			return ScenarioFiles.ToArray();
		}

		static void Execute( string[] args ) {
			String Database;
			String TxtFilename;
			String GracesJapanese;

			/*
			args = new string[] {
				@"c:\Users\Georg\Documents\Tales of Vesperia\GraceNote92\Databases\VScenario723",
				@"c:\Users\Georg\Documents\Tales of Vesperia\GraceNote92\Databases\VScenario723.txt",
				@"c:\Users\Georg\Documents\Tales of Vesperia\GraceNote92\Databases\GracesJapanese"
			};
			//*/

			bool IdentifyNames = false;
			bool RemovingNewlines = false;
			bool VesperiaTagReplacement = false;
			bool DumpIdentifyerStrings = false;
			bool ForceJapaneseDump = false;
			bool DumpDebug = false;

			if ( args.Length != 3 && args.Length != 4 ) {
				Console.WriteLine( "Usage: GraceNote_DumpText DBFilename TxtName GracesJapanese [nrvijd]" );
				Console.WriteLine( "n: Try to identify names (for Vesperia!)" );
				Console.WriteLine( "r: Remove newlines within a single entry" );
				Console.WriteLine( "v: Remove & Replace Vesperia Tags" );
				Console.WriteLine( "i: Dump Identifyer Strings" );
				Console.WriteLine( "j: Force Japanese dump" );
				Console.WriteLine( "d: Dump Debug entries too" );
				return;
			} else {
				Database = args[0];
				TxtFilename = args[1];
				GracesJapanese = args[2];

				if ( args.Length == 4 ) {
					String bonusargs = args[3];
					foreach ( char c in bonusargs ) {
						switch ( c ) {
							case 'n':
								IdentifyNames = true;
								break;
							case 'r':
								RemovingNewlines = true;
								break;
							case 'v':
								VesperiaTagReplacement = true;
								break;
							case 'i':
								DumpIdentifyerStrings = true;
								break;
							case 'j':
								ForceJapaneseDump = true;
								break;
							case 'd':
								DumpDebug = true;
								break;
						}
					}
				}
			}

			ScenarioFile[] ScenarioStrings;
			try {
				ScenarioStrings = GetSQL( "Data Source=" + Database, -1, "Data Source=" + GracesJapanese, DumpIdentifyerStrings, ForceJapaneseDump, DumpDebug );
				List<ScenarioFile> FullStrings = new List<ScenarioFile>( ScenarioStrings );
				FullStrings.Sort();
				ScenarioStrings = FullStrings.ToArray();
			} catch ( Exception ex ) {
				Console.WriteLine( ex.ToString() );
				return;
			}


			ScenarioFile[] ScenarioBlocks;
			if ( IdentifyNames ) {
				ScenarioBlocks = Block( ScenarioStrings );
			} else {
				ScenarioBlocks = ScenarioStrings;
			}
			List<string> Strings = new List<string>( ScenarioBlocks.Length );

			foreach ( ScenarioFile b in ScenarioBlocks ) {
				String currentString = "";

				if ( DumpIdentifyerStrings ) {
					currentString += "[" + b.IdentifyerString + "]\n";
				}

				if ( String.IsNullOrEmpty( b.Name ) ) {
					currentString += b.Text;
				} else {
					currentString += b.Name + " " + b.Text;
				}

				if ( DumpIdentifyerStrings ) {
					currentString += "\n";
				}

				Strings.Add( currentString );
			}

			for ( int i = 0; i < Strings.Count; ++i ) {
				if ( VesperiaTagReplacement ) Strings[i] = RemoveTags( Strings[i] );
				if ( RemovingNewlines ) Strings[i] = RemoveNewlines( Strings[i] );
			}

			System.IO.File.WriteAllLines( TxtFilename, Strings.ToArray() );

			return;
		}

		private static String RemoveNewlines( String s ) {
			s = s.Replace( " \n", " " );
			s = s.Replace( "\n", " " );
			return s;
		}

		private static String RemoveTags( String s ) {
			s = s.Replace( "''", "'" );
			s = s.Replace( "(YUR)", "Yuri" );
			s = s.Replace( "(EST)", "Estellise" );
			s = s.Replace( "(EST_P)", "Estelle" );
			s = s.Replace( "(RIT)", "Rita" );
			s = s.Replace( "(KAR)", "Karol" );
			s = s.Replace( "(RAP)", "Repede" );
			s = s.Replace( "(RAV)", "Raven" );
			s = s.Replace( "(JUD)", "Judith" );
			s = s.Replace( "(FRE)", "Flynn" );
			s = s.Replace( "(PAT)", "Patty" );
			s = s.Replace( "(JUD_P)", "Judy" );
			s = s.Replace( "", " " );
			s = Regex.Replace( s, "\t[(][A-Za-z0-9_]+[)]", "" ); // voice command
			return s;
		}

		private static ScenarioFile[] Block( ScenarioFile[] ScenarioStrings ) {
			List<ScenarioFile> ScenarioBlocks = new List<ScenarioFile>();

			for ( int i = 0; i < ScenarioStrings.Length; i++ ) {
				try {
					if ( ScenarioStrings[i].Pointer + 4 == ScenarioStrings[i + 1].Pointer ) {
						ScenarioFile s = new ScenarioFile();
						s.Pointer = ScenarioStrings[i].Pointer - 8;
						s.Name = ScenarioStrings[i].Text;
						s.Text = ScenarioStrings[i + 1].Text;
						ScenarioBlocks.Add( s );
						i++;
					} else {
						ScenarioFile s = new ScenarioFile();
						s.Pointer = ScenarioStrings[i].Pointer - 12;
						s.Name = "";
						s.Text = ScenarioStrings[i].Text;
						ScenarioBlocks.Add( s );
					}
				} catch ( IndexOutOfRangeException ) {
					ScenarioFile s = new ScenarioFile();
					s.Pointer = ScenarioStrings[i].Pointer - 12;
					s.Name = "";
					s.Text = ScenarioStrings[i].Text;
					ScenarioBlocks.Add( s );
				}
			}

			return ScenarioBlocks.ToArray();
		}

		private static int[] GetLocation( byte[] File, int Max, int Pointer ) {
			byte[] PointerBytes = System.BitConverter.GetBytes( HyoutaTools.Util.SwapEndian( (uint)Pointer ) );
			byte[] SearchBytes = new byte[] { 0x04, 0x0C, 0x00, 0x18, PointerBytes[0], PointerBytes[1], PointerBytes[2], PointerBytes[3] };

			List<int> PointerArrayList = new List<int>();

			for ( int i = 0; i < Max; i++ ) {
				if ( File[i] == SearchBytes[0]
					&& File[i + 1] == SearchBytes[1]
					&& File[i + 2] == SearchBytes[2]
					&& File[i + 3] == SearchBytes[3]
					&& File[i + 4] == SearchBytes[4]
					&& File[i + 5] == SearchBytes[5]
					&& File[i + 6] == SearchBytes[6]
					&& File[i + 7] == SearchBytes[7] ) {
					PointerArrayList.Add( i + 4 );
				}
			}

			return PointerArrayList.ToArray();
		}
	}
}
