using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.GoogleTranslate {
	public class TranslateDatabase {

		public static int Execute( List<string> args ) {
			Translate( args[0], args[1] );
			return 0;
		}


		public static void Translate( string Filename, string FilenameGracesJapanese ) {
			CleanGoogleTranslatedString( "" );

			DatabaseEntry[] entries = GetSQL( "Data Source=" + Filename, "Data Source=" + FilenameGracesJapanese );

			WebClient webClient = new WebClient();
			webClient.BaseAddress = "http://translate.google.com";


			foreach ( DatabaseEntry e in entries ) {
				try {
					webClient.Encoding = Encoding.UTF8;
					webClient.Headers["User-Agent"] = "Mozilla/5.0 (X11; U; Linux x86_64; en-US; rv:1.9.2.3) Gecko/20100402 Namoroka/3.6.3 (.NET CLR 3.5.30729)";
					webClient.QueryString.Clear();
					webClient.QueryString["client"] = "t";
					webClient.QueryString["sl"] = "ja";
					webClient.QueryString["tl"] = "en";
					webClient.QueryString["text"] = e.TextJP;
					webClient.QueryString["text"] = "パーティー\nの\n料理";
					string translateResult = webClient.DownloadString( "/translate_a/t" );
					string english = CleanGoogleTranslatedString( translateResult );
					e.TextEN = english;
				} catch ( WebException ) {
				}
			}


			return;
		}


		public static string CleanGoogleTranslatedString( string str ) {
			//string example = "[[[\"Cooking Party\",\"パーティーの料理\",\"\",\"Pātī no ryōri\"]],,\"ja\",,[[\"Cooking\",[4],1,0,320,0,1,0],[\"Party\",[5],1,0,314,1,2,0]],[[\"料理\",4,[[\"Cooking\",320,1,0],[\"Cuisine\",15,1,0],[\"Food\",8,1,0],[\"Dishes\",0,1,0],[\"Cuisines\",0,1,0]],[[6,8]],\"パーティーの料理\"],[\"の パーティー\",5,[[\"Party\",314,1,0],[\"a party\",0,1,0],[\"parties\",0,1,0],[\"for party\",0,1,0]],[[0,6]],\"\"]],,,[[\"ja\"]],30]";
			string example = "[[[\"[ Cuisine \\\" of \\\" party ]\",\"[パーティー\\\"の\\\"料理]\",\"\",\"[Pātī\\\" no\\\" ryōri]\"]],,\"ja\",,[[\"[\",[4],1,1,1000,0,1,0],[\"Cuisine \\\"\",[5],0,1,902,1,3,0],[\"of\",[6],0,0,821,3,4,0],[\"\\\"\",[7],0,0,821,4,5,0],[\"party\",[8],1,0,782,5,6,0],[\"]\",[9],0,0,782,6,7,0]],[[\"[\",4,[[\"[\",1000,1,1]],[[0,1]],\"[パーティー\\\"の\\\"料理]\"],[\"料理 \\\"\",5,[[\"Cuisine \\\"\",902,0,1]],[[6,7],[9,11]],\"\"],[\"の\",6,[[\"of\",821,0,0],[\"of the\",106,0,0],[\"in\",2,0,0],[\"the\",0,0,0],[\"for\",0,0,0]],[[7,8]],\"\"],[\"\\\"\",7,[[\"\\\"\",821,0,0]],[[8,9]],\"\"],[\"パーティー\",8,[[\"party\",782,1,0],[\"parties\",0,1,0],[\"Event\",0,1,0],[\"the party\",0,1,0],[\"a party\",0,1,0]],[[1,6]],\"\"],[\"]\",9,[[\"]\",782,0,0]],[[11,12]],\"\"]],,,[[\"ja\"]],1]";
			string exmaple = "[[[\"Party\\n\",\"パーティー\",\"\",\"Pātī\"],[\"Of\\n\",\"の\",\"\",\"No\"],[\"Cooking\",\"料理\",\"\",\"Ryōri\"]],,\"ja\",,[[\"Party\",[4],1,0,1000,0,1,0],[\"\\n\",,0,0,0,0,0,0],[\"Of\",[11],1,0,1000,0,1,1],[\"\\n\",,0,0,0,0,0,0],[\"Cooking\",[21],1,0,794,0,1,1]],[[\"パーティー\",4,[[\"Party\",1000,1,0],[\"Parties\",0,1,0],[\"Event\",0,1,0],[\"The party\",0,1,0],[\"A party\",0,1,0]],[[0,5]],\"パーティー\"],[\"の\",11,[[\"Of\",1000,1,0],[\"The\",0,1,0],[\"Of the\",0,1,0],[\"In\",0,1,0],[\"For\",0,1,0]],[[0,1]],\"の\"],[\"料理\",21,[[\"Cooking\",794,1,0],[\"Cuisine\",166,1,0],[\"Dish\",38,1,0],[\"Fare\",0,1,0],[\"Cuisines\",0,1,0]],[[0,2]],\"料理\"]],,,[[\"ja\"]],60]";
			str = example;

			// first grab the part that actually has the translated string
			StringBuilder sb = new StringBuilder();
			int bracketCount = 0;
			foreach ( char c in str ) {
				switch ( c ) {
					case '[':
						bracketCount++;
						if ( bracketCount > 3 ) sb.Append( c );
						break;
					case ']':
						if ( bracketCount > 3 ) sb.Append( c ); 
						bracketCount--;
						if ( bracketCount == 2 ) goto post; break;
					default: sb.Append( c ); break;
				}
			}

		post:
			str = sb.ToString();
			for ( int i = 1; i < str.Length; ++i ) {
				if ( str[i] == '"' ) {
					if ( str[i-1] != '\\' ) {
						str = str.Substring(1, i-1);
						break;
					}
				}
			}

			str = str.Replace( "\\\"", "\"" );
			str = str.Replace( "\\\\", "\\" );

			return str;
		}

		public class DatabaseEntry {
			public string TextEN;
			public string TextJP;
			public int ID;
			public int JPID;
			public int Status;
		}

		public static DatabaseEntry[] GetSQL( String ConnectionString, String GracesJapaneseConnectionString ) {
			List<DatabaseEntry> Entries = new List<DatabaseEntry>();

			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();
			SQLiteConnection ConnectionGJ = new SQLiteConnection( GracesJapaneseConnectionString );
			ConnectionGJ.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) )
			using ( SQLiteTransaction TransactionGJ = ConnectionGJ.BeginTransaction() )
			using ( SQLiteCommand CommandGJ = new SQLiteCommand( ConnectionGJ ) ) {
				Command.CommandText = "SELECT english, ID, StringID, status " +
									  "FROM Text ORDER BY ID";
				CommandGJ.CommandText = "SELECT string FROM Japanese WHERE ID = ?";
				SQLiteParameter StringIdParam = new SQLiteParameter();
				CommandGJ.Parameters.Add( StringIdParam );

				SQLiteDataReader r = Command.ExecuteReader();
				while ( r.Read() ) {
					String SQLText;

					try {
						SQLText = r.GetString( 0 ).Replace( "''", "'" );
					} catch ( System.InvalidCastException ) {
						SQLText = null;
					}

					int ID = r.GetInt32( 1 );
					int StringID = r.GetInt32( 2 );
					int Status = r.GetInt32( 3 );


					StringIdParam.Value = StringID;
					String JPText;
					try {
						JPText = ( (string)CommandGJ.ExecuteScalar() ).Replace( "''", "'" );
					} catch ( System.InvalidCastException ) {
						JPText = null;
					}


					DatabaseEntry de = new DatabaseEntry();
					de.TextEN = SQLText;
					de.TextJP = JPText;
					de.ID = ID;
					de.JPID = StringID;
					de.Status = Status;

					Entries.Add( de );
				}

				Transaction.Rollback();
				TransactionGJ.Rollback();
			}

			ConnectionGJ.Close();
			Connection.Close();

			return Entries.ToArray();
		}


	}
}
