using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Trophy;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.Trophy.TropSfmExport {
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

			Connection.Close();

			return SQLText;
		}

		public static List<TrophyNode> GetSQL( String ConnectionString, String GracesJapaneseConnectionString ) {
			//List<ScenarioFile> Trophies = new List<TrophyNode>();
			Dictionary<string, TrophyNode> Trophies = new Dictionary<string, TrophyNode>();

			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText =
					"SELECT english, PointerRef, StringID, t_id, t_hidden, t_ttype, t_pid FROM Text ORDER BY ID";
				SQLiteDataReader r = Command.ExecuteReader();
				while ( r.Read() ) {
					String SQLText;
					String T_ID;
					bool T_Hidden;
					String T_TType;
					String T_PID;

					try {
						SQLText = r.GetString( 0 ).Replace( "''", "'" );
						T_ID = r.GetString( 3 );
						T_Hidden = r.GetInt32( 4 ) == 1;
						T_TType = r.GetString( 5 );
						T_PID = r.GetString( 6 );
					} catch ( System.InvalidCastException ) {
						SQLText = null;
						T_ID = null;
						T_Hidden = false;
						T_TType = null;
						T_PID = null;
					}

					int PointerRef = r.GetInt32( 1 );
					int StringID = r.GetInt32( 2 );

					if ( Trophies.ContainsKey( T_ID ) ) {
						if ( PointerRef == 0 ) {
							Trophies[T_ID].Name = SQLText;
						} else if ( PointerRef == 1 ) {
							Trophies[T_ID].Detail = SQLText;
						}
					} else {
						String Name = null;
						String Detail = null;
						if ( PointerRef == 0 ) {
							Name = SQLText;
						} else if ( PointerRef == 1 ) {
							Detail = SQLText;
						}
						TrophyNode Trophy = new TrophyNode( T_ID, T_Hidden, T_TType, T_PID, Name, Detail, null );
						Trophies[T_ID] = Trophy;
					}
				}

				Transaction.Rollback();
			}

			Connection.Close();

			return Trophies.Values.ToList();
		}

		public static int Execute( List<string> args ) {
			String Database;
			String GracesJapanese;

			/*
			args = new string[] {
				@"c:\Users\Georg\Documents\Tales of Vesperia\GraceNoteGrenade\Databases\VTrophies",
				@"c:\Users\Georg\Documents\Tales of Vesperia\GraceNoteGrenade\Databases\GracesJapanese"
			};
			//*/

			if ( args.Count != 2 ) {
				Console.WriteLine( "Usage: GraceNote_TropSFM VTrophies GracesJapanese" );
				return -1;
			} else {
				Database = args[0];
				GracesJapanese = args[1];
			}

			List<TrophyNode> EnglishTrophies;
			try {
				EnglishTrophies = GetSQL( "Data Source=" + Database, "Data Source=" + GracesJapanese );
				EnglishTrophies.Sort();
			} catch ( Exception ) {
				return -1;
			}

			TrophyConfNode TROPSFM = new TrophyConfNode(
				"4c39b98c0100000000000000b4bb7de046f205e74b24eabf731497511b639be57ca1ae0a3efd0519adba789cb32b91d97d3e11a7bf8302544fab919062647245e95796f11dd01c8db63f391756e0a1ddf6ddc7d64b0b8c86ec962e4ea2ef3b4caf82d178afb91a0a6fe3a082299c4a2fb1e3764ac6d42a7a11f52980a2e5149e9a8256fbc9dc438a4490ddace9a96c4d23f4652bbbb1cee1819a26e390724d7c",
				"4c39b98c010000000000000074abfed402a679fdff35652737f80e2368488f2f524b8721ab07366434ba133c4b545d55130ce41e31c07f8dc7c166cef530cd5f340c489b55626fd20a5d6b4b3851a3cb1ba04fc353619307033fc3b208e1e08d1de80c2c5e4be2ff131f22ed1f31e22f93697e9bd91b1e1e5285a1e2f50f8c324a03e515e006a85322f19c246d538c08177fcd9d302e34bb3b250ab40b704c61",
				"1.0", "NPWR00642_00", "01.00", "0", "default", "Tales of Vesperia", "Tales of Vesperia Trophy Set", "BLJS10053" // not sure on the folder but whatever, shouldn't matter
				);

			Dictionary<uint, TrophyNode> EngTrophyDict = new Dictionary<uint, TrophyNode>( EnglishTrophies.Count );
			foreach ( TrophyNode tn in EnglishTrophies ) {
				EngTrophyDict.Add( uint.Parse( tn.ID ), tn );
			}
			TROPSFM.Trophies = EngTrophyDict;
			System.IO.File.WriteAllBytes( @"newTrophyConf.SFM", Encoding.UTF8.GetBytes( TROPSFM.ExportTropSFM( true ) ) );
			System.IO.File.WriteAllBytes( @"newTrophy.SFM", Encoding.UTF8.GetBytes( TROPSFM.ExportTropSFM( false ) ) );

			return 0;
		}
	}
}
