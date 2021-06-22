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
			if ( args.Count < 11 ) {
				Console.WriteLine( "Usage: GraceNote_TropSFM VTrophies GracesJapanese SceNpTrophySignature_Trop SceNpTrophySignature_TropConf version npcommid trophyset-version parental-level parental-level_license-area title-name title-detail" );
				return -1;
			}

			String Database = args[0];
			String GracesJapanese = args[1];
			String SceNpTrophySignature_Trop = args[2];
			String SceNpTrophySignature_TropConf = args[3];
			String version = args[4];
			String npcommid = args[5];
			String trophysetversion = args[6];
			String parentallevel = args[7];
			String parentallevel_licensearea = args[8];
			String TitleName = args[9];
			String TitleDetail = args[10];

			List<TrophyNode> EnglishTrophies;
			try {
				EnglishTrophies = GetSQL( "Data Source=" + Database, "Data Source=" + GracesJapanese );
				EnglishTrophies.Sort();
			} catch ( Exception ) {
				return -1;
			}

			TrophyConfNode TROPSFM = new TrophyConfNode( SceNpTrophySignature_Trop, SceNpTrophySignature_TropConf, version, npcommid, trophysetversion, parentallevel, parentallevel_licensearea, TitleName, TitleDetail, "" );

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
