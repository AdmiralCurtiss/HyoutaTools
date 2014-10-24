using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.TSS;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using HyoutaTools.GraceNote.Vesperia.ScfombinImport;

namespace HyoutaTools.GraceNote.Vesperia.StringDicNpcCompare360 {

	static class Program {


		public static void FindEarliestGracesJapaneseEntry( String ConnectionString, String GracesJapaneseConnectionString ) {
			using ( SQLiteConnection ConnectionE = new SQLiteConnection( ConnectionString ) )
			using ( SQLiteConnection ConnectionJ = new SQLiteConnection( GracesJapaneseConnectionString ) ) {
				ConnectionE.Open();
				ConnectionJ.Open();

				using ( SQLiteTransaction TransactionE = ConnectionE.BeginTransaction() )
				using ( SQLiteTransaction TransactionJ = ConnectionJ.BeginTransaction() )
				using ( SQLiteCommand CommandEFetch = new SQLiteCommand( ConnectionE ) )
				using ( SQLiteCommand CommandEUpdate = new SQLiteCommand( ConnectionE ) )
				using ( SQLiteCommand CommandJ = new SQLiteCommand( ConnectionJ ) ) {
					CommandEFetch.CommandText = "SELECT ID, StringID FROM Text ORDER BY ID";
					SQLiteDataReader r = CommandEFetch.ExecuteReader();

					List<KeyValuePair<int, int>> DatabaseEntries = new List<KeyValuePair<int, int>>();
					while ( r.Read() ) {
						int ID = r.GetInt32( 0 );
						int StringID = r.GetInt32( 1 );
						DatabaseEntries.Add( new KeyValuePair<int, int>( ID, StringID ) );
					}
					r.Close();

					CommandEUpdate.CommandText = "UPDATE Text SET StringID = ? WHERE ID = ?";
					CommandJ.CommandText = "SELECT ID FROM Japanese WHERE string = ( SELECT string FROM Japanese WHERE ID = ? ) ORDER BY ID ASC";
					SQLiteParameter ParamEStringId = new SQLiteParameter();
					SQLiteParameter ParamEId = new SQLiteParameter();
					SQLiteParameter ParamJId = new SQLiteParameter();
					CommandEUpdate.Parameters.Add( ParamEStringId );
					CommandEUpdate.Parameters.Add( ParamEId );
					CommandJ.Parameters.Add( ParamJId );

					foreach ( KeyValuePair<int, int> e in DatabaseEntries ) {
						ParamJId.Value = e.Value;
						int? EarliestStringId = (int?)CommandJ.ExecuteScalar();
						if ( EarliestStringId != null ) {
							ParamEId.Value = e.Key;
							ParamEStringId.Value = EarliestStringId;
							CommandEUpdate.ExecuteNonQuery();
						}
					}

					TransactionJ.Rollback();
					TransactionE.Commit();
				}
			}
		}


		public static ScenarioString[] GetSQL( String ConnectionString, String GracesJapaneseConnectionString ) {
			List<ScenarioString> ScenarioFiles = new List<ScenarioString>();

			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText = "SELECT english, ID, StringID FROM Text WHERE status != -1 AND IdentifyString != '(x360)' ORDER BY ID";
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

					String JapaneseString = GetJapanese( GracesJapaneseConnectionString, StringID );
					String EnglishString;
					if ( !String.IsNullOrEmpty( SQLText ) ) {
						EnglishString = SQLText;
					} else {
						EnglishString = JapaneseString;
					}

					ScenarioString sc = new ScenarioString( PointerRef, JapaneseString, EnglishString );
					ScenarioFiles.Add( sc );
				}

				Transaction.Rollback();
			}
			return ScenarioFiles.ToArray();
		}

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

		static void UpdateEntry( String ConnectionString, int ID, int updateStatus, string newEnglishEntry ) {
			//String SQLText = "";

			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {

				Command.CommandText = "UPDATE Text SET IdentifyString = '(x360)', updated = 1, english = ?, status = " + updateStatus.ToString() + " WHERE ID = " + ID.ToString();
				SQLiteParameter param = new SQLiteParameter();
				param.Value = newEnglishEntry;
				Command.Parameters.Add( param );


				int cnt = Command.ExecuteNonQuery();
				if ( cnt == 1 ) {
					Transaction.Commit();
				} else {
					Console.WriteLine( "FAILED UPDATING ENTRY " + ID.ToString() );
					Transaction.Rollback();
				}
			}
		}

		public static int Execute( string[] args ) {

			//Console.WriteLine( "おっさん試されてる？↵新騎士団長はおっかないわ……" );
			//FindEarliestGracesJapaneseEntry( "Data Source=VSkit Text", "Data Source=GracesJapanese" );
			//return;

			Console.WriteLine( "Opening STRING_DIC.SO..." );
			TSSFile TSS;
			try {
				TSS = new TSSFile( System.IO.File.ReadAllBytes( "STRING_DIC.SO" ) );
			} catch ( System.IO.FileNotFoundException ) {
				Console.WriteLine( "Could not open STRING_DIC.SO, exiting." );
				return -1;
			}

			List<ScenarioString> npcLines = GetSQL( "Data Source=VSkit Text", "Data Source=GracesJapanese" ).ToList();
			List<ScenarioString> x360Lines = new List<ScenarioString>( TSS.Entries.Length );
			foreach ( TSSEntry e in TSS.Entries ) {
				x360Lines.Add( new ScenarioString( e.StringJpnIndex, e.StringJpn, e.StringEng ) );
			}
			List<ScenarioString> foundLinesNpc = new List<ScenarioString>();
			List<ScenarioString> foundLines360 = new List<ScenarioString>();

			Regex furi = new Regex( "\r[(][0-9]+[,][\\p{IsHiragana}\\p{IsKatakana}]+[)]" );
			Regex colortag = new Regex( "\u0003[(][0-9]+[)]" );
			Regex inn = new Regex( "　【[0-9]+ガルド】" );
			foreach ( ScenarioString s in npcLines ) {
				if ( s.Jpn.Contains( '\r' ) ) {
					s.Jpn = furi.Replace( s.Jpn, "" );
				}
				if ( s.Jpn.Contains( '\u0003' ) ) {
					s.Jpn = colortag.Replace( s.Jpn, "" );
				}
				if ( s.Jpn.Contains( '【' ) ) {
					s.Jpn = inn.Replace( s.Jpn, "" );
				}
				s.Jpn = s.Jpn.Replace( "ギルド", "帝国" );
				s.Jpn = s.Jpn.Replace( "～", "〜" );
				s.Jpn = s.Jpn.TrimEnd( new char[] { '。' } );
				s.Jpn = s.Jpn.Replace( "。\f", "\f" );
				s.Jpn = s.Jpn.Replace( "ズェ", "ずぇ" );
				s.Jpn = s.Jpn.Replace( "※", "" );
				s.Jpn = s.Jpn.Replace( "卑賤", "下町" );
				s.Jpn = s.Jpn.Replace( "!", "！" );
				s.Jpn = s.Jpn.Replace( "?", "？" );
				s.Jpn = s.Jpn.Replace( "！　", "！" );
				s.Jpn = s.Jpn.Replace( "？　", "？" );
			}
			foreach ( ScenarioString s in x360Lines ) {
				if ( s.Jpn == null ) continue;
				if ( s.Jpn.Contains( '\r' ) ) {
					s.Jpn = furi.Replace( s.Jpn, "" );
				}
				if ( s.Jpn.Contains( '\u0003' ) ) {
					s.Jpn = colortag.Replace( s.Jpn, "" );
				}
				if ( s.Jpn.Contains( '【' ) ) {
					s.Jpn = inn.Replace( s.Jpn, "" );
				}
				s.Jpn = s.Jpn.Replace( "ギルド", "帝国" );
				s.Jpn = s.Jpn.Replace( "～", "〜" );
				s.Jpn = s.Jpn.TrimEnd( new char[] { '。' } );
				s.Jpn = s.Jpn.Replace( "。\f", "\f" );
				s.Jpn = s.Jpn.Replace( "ズェ", "ずぇ" );
				s.Jpn = s.Jpn.Replace( "※", "" );
				s.Jpn = s.Jpn.Replace( "卑賤", "下町" );
				s.Jpn = s.Jpn.Replace( "!", "！" );
				s.Jpn = s.Jpn.Replace( "?", "？" );
				s.Jpn = s.Jpn.Replace( "！　", "！" );
				s.Jpn = s.Jpn.Replace( "？　", "？" );
			}

			for ( int i = 0; i < npcLines.Count; ++i ) {
				for ( int j = 0; j < x360Lines.Count; ++j ) {
					if ( npcLines[i].Jpn == x360Lines[j].Jpn ) {
						foundLinesNpc.Add( npcLines[i] );
						foundLines360.Add( x360Lines[j] );

						npcLines.RemoveAt( i );
						x360Lines.RemoveAt( j );
						--i;

						break;
					}
				}
			}


			int encnt = 0;
			for ( int i = 0; i < foundLines360.Count; ++i ) {
				if ( foundLinesNpc[i].Eng != foundLines360[i].Eng ) {
					encnt++;
				}
			}
			Console.WriteLine( encnt.ToString() );

			for ( int i = 0; i < foundLines360.Count; ++i ) {
				Console.WriteLine( ( i + 1 ).ToString() + "/" + foundLines360.Count.ToString() );
				if ( foundLinesNpc[i].Eng == foundLines360[i].Eng ) {
					UpdateEntry( "Data Source=VSkit Text", foundLinesNpc[i].Pointer, 4, foundLinesNpc[i].Eng );
				} else {
					//if ( i < 4500 ) continue;

					string x = foundLines360[i].Eng;
					string d = foundLinesNpc[i].Eng;

					{
						x = x.Replace( "↵", "\n" );
						d = d.Replace( "↵", "\n" );
						x = x.Replace( "～", "〜" );
						d = d.Replace( "～", "〜" );
						x = x.Replace( " \n", "\n" );
						d = d.Replace( " \n", "\n" );
						x = x.Replace( " \n", "\n" );
						d = d.Replace( " \n", "\n" );
						x = x.Replace( " \f", "\f" );
						d = d.Replace( " \f", "\f" );
						x = x.Replace( "\n ", "\n" );
						d = d.Replace( "\n ", "\n" );
						x = x.Replace( "‘", "'" );
						x = x.Replace( "’", "'" );
						x = x.Replace( "‛", "'" );
						d = d.Replace( "‘", "'" );
						d = d.Replace( "’", "'" );
						d = d.Replace( "‛", "'" );
						x = x.TrimEnd( new char[] { ' ' } );
						d = d.TrimEnd( new char[] { ' ' } );

						if ( x == d ) {
							UpdateEntry( "Data Source=VSkit Text", foundLinesNpc[i].Pointer, 4, d );
							continue;
						}
					}

					encnt = 0;
					for ( int j = i; j < foundLines360.Count; ++j ) {
						if ( foundLinesNpc[j].Eng != foundLines360[j].Eng ) {
							encnt++;
						}
					}
					Console.WriteLine( encnt.ToString() + " to go!" );

					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine( "Ambigous Entry found:" );
					Console.WriteLine( "Japanese:" );
					Console.WriteLine( foundLinesNpc[i].Jpn );
					Console.WriteLine();
					Console.WriteLine( "English from Database:" );
					Console.WriteLine( foundLinesNpc[i].Eng.Replace( "\n", "↵\n" ) );
					Console.WriteLine();
					Console.WriteLine( "English from 360:" );
					Console.WriteLine( foundLines360[i].Eng.Replace( "\n", "↵\n" ) );
					Console.WriteLine();

					Console.WriteLine( "x: use from 360, mark 4" );
					Console.WriteLine( "4: use from DB, mark 4" );
					Console.WriteLine( "3: use from DB, mark 3" );
					Console.WriteLine( "2: use from DB, mark 2" );
					Console.WriteLine( "1: use from DB, mark 1" );

					string opt = Console.ReadLine();
					if ( opt.StartsWith( "x" ) ) {
						UpdateEntry( "Data Source=VSkit Text", foundLinesNpc[i].Pointer, 4, foundLines360[i].Eng );
					} else if ( opt.StartsWith( "4" ) ) {
						UpdateEntry( "Data Source=VSkit Text", foundLinesNpc[i].Pointer, 4, foundLinesNpc[i].Eng );
					} else if ( opt.StartsWith( "3" ) ) {
						UpdateEntry( "Data Source=VSkit Text", foundLinesNpc[i].Pointer, 3, foundLinesNpc[i].Eng );
					} else if ( opt.StartsWith( "2" ) ) {
						UpdateEntry( "Data Source=VSkit Text", foundLinesNpc[i].Pointer, 2, foundLinesNpc[i].Eng );
					} else if ( opt.StartsWith( "1" ) ) {
						UpdateEntry( "Data Source=VSkit Text", foundLinesNpc[i].Pointer, 1, foundLinesNpc[i].Eng );
					}
				}
			}

			return 0;
		}
	}
}
