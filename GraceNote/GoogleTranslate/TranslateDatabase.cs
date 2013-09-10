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

		public static List<string> ReplacementOriginals;
		public static List<string> ReplacementSubstitutes;

		public static void SetupReplacements() {
			ReplacementOriginals = new List<string>();
			ReplacementSubstitutes = new List<string>();

			// Nari variable stuff, with %
			for ( int i = 99; i >= 0; --i ) {
				ReplacementOriginals.Add( "%NT" + i.ToString() );
				ReplacementSubstitutes.Add( "VARIABLE_NT_" + i.ToString() + "__" );
			}
			for ( int i = 99; i >= 0; --i ) {
				ReplacementOriginals.Add( "%PN" + i.ToString("D2") );
				ReplacementSubstitutes.Add( "VARIABLE_PN_" + i.ToString() + "__" );
			}
			for ( int i = 99; i >= 0; --i ) {
				ReplacementOriginals.Add( "%H" + i.ToString() );
				ReplacementSubstitutes.Add( "VARIABLE_H_" + i.ToString() + "__" );
			}
			for ( int i = 9; i >= 0; --i ) {
				ReplacementOriginals.Add( "%P" + i.ToString() );
				ReplacementSubstitutes.Add( "__VARIABLE_P_" + i.ToString() + "__" );
			}
			for ( int i = 99; i >= 0; --i ) {
				ReplacementOriginals.Add( "%N" + i.ToString() );
				ReplacementSubstitutes.Add( "__VARIABLE_N_" + i.ToString() + "__" );
			}

			ReplacementOriginals.Add( "%NA" );
			ReplacementSubstitutes.Add( "__VARIABLE_NA__" );
			ReplacementOriginals.Add( "%NE" );
			ReplacementSubstitutes.Add( "__VARIABLE_NE__" );
			ReplacementOriginals.Add( "%S" );
			ReplacementSubstitutes.Add( "__VARIABLE_S__" );
			ReplacementOriginals.Add( "%GH" );
			ReplacementSubstitutes.Add( "__VARIABLE_GH__" );
			ReplacementOriginals.Add( "%GT" );
			ReplacementSubstitutes.Add( "__VARIABLE_GT__" );
			ReplacementOriginals.Add( "%G" );
			ReplacementSubstitutes.Add( "__VARIABLE_G__" );
			ReplacementOriginals.Add( "%C" );
			ReplacementSubstitutes.Add( "__VARIABLE_C__" );
			ReplacementOriginals.Add( "%L" );
			ReplacementSubstitutes.Add( "__VARIABLE_L__" );
			ReplacementOriginals.Add( "%r" );
			ReplacementSubstitutes.Add( "__VARIABLE_r__" );

		}

		public static int Execute( List<string> args ) {
			Translate( args[0], args[1] );
			return 0;
		}

		public static void Translate( string Filename, string FilenameGracesJapanese ) {
			SetupReplacements();
			CleanGoogleTranslatedString( "" );

			GraceNoteDatabaseEntry[] rawEntries = GraceNoteDatabaseEntry.GetAllEntriesFromDatabase( "Data Source=" + Filename, "Data Source=" + FilenameGracesJapanese );
			TranslatableGraceNoteEntry[] entries = new TranslatableGraceNoteEntry[rawEntries.Length];
			for ( int i = 0; i < entries.Length; ++i ) {
				entries[i] = new TranslatableGraceNoteEntry();
				entries[i].Entry = rawEntries[i];
			}

			WebClient webClient = new WebClient();
			webClient.BaseAddress = "http://translate.google.com";

			System.Random rng = new System.Random();

			foreach ( TranslatableGraceNoteEntry e in entries ) {
				// THIS IS CERTAINLY EASIER TO SOLVE WITH A REGEX
				// but I've been fucked by greedy regexes ( "a<b>c<d>e".Remove("<*>") -> "ae" instead of "ace" ) so bleeh
				string VariableLess = e.Entry.TextJP.Replace( "<CLT>", "" );
				for ( int i = 0; i < 100; ++i ) {
					VariableLess = VariableLess.Replace( "<CLT " + i.ToString( "D2" ) + ">", "" );
				}
				e.NewLineAtEnd = VariableLess.EndsWith( "\n" );
				e.NewLineCount = VariableLess.Count( ch => ch == '\n' );
				e.Entry.TextJP = e.Entry.TextJP.Replace( "\n", "" );

				if ( e.Entry.TextJP.Length >= 2 && e.Entry.TextJP[e.Entry.TextJP.Length - 2] == '\\' ) {
					e.PreserveStringAtEnd = e.Entry.TextJP.Substring( e.Entry.TextJP.Length - 2 );
					e.Entry.TextJP = e.Entry.TextJP.Substring( 0, e.Entry.TextJP.Length - 2 );
				}
				if ( e.Entry.TextJP.StartsWith( "#" ) ) {
					e.PreserveStringAtStart = "#";
					e.Entry.TextJP = e.Entry.TextJP.Substring( 1 );
				}

				for ( int i = 0; i < ReplacementOriginals.Count; ++i ) {
					e.Entry.TextJP = e.Entry.TextJP.Replace( ReplacementOriginals[i], ReplacementSubstitutes[i] );
				}
			}


			SQLiteConnection conn = new SQLiteConnection( "Data Source=" + Filename );
			conn.Open();
			Object[] param = new Object[2];

			FileStream FailLog = new FileStream( "googletranslate.log", FileMode.Append );
			StreamWriter LogWriter = new StreamWriter( FailLog );

			int entryCount = 0;
			foreach ( TranslatableGraceNoteEntry e in entries ) {
				entryCount++;
				if ( e.Entry.Status == -1 ) { continue; }
				if ( e.Entry.UpdatedBy == "GoogleTranslate" ) { continue; }
				if ( e.Entry.TextJP.Trim() == "" ) { continue; }
				try {
					Console.WriteLine( "Processing Entry " + entryCount + "/" + entries.Length + "..." );
					webClient.Encoding = Encoding.UTF8;
					webClient.Headers["User-Agent"] = "Mozilla/5.0 (X11; U; Linux x86_64; en-US; rv:1.9.2.3) Gecko/20100402 Namoroka/3.6.3 (.NET CLR 3.5.30729)";
					webClient.QueryString.Clear();
					webClient.QueryString["client"] = "t";
					webClient.QueryString["sl"] = "ja";
					webClient.QueryString["tl"] = "en";
					webClient.QueryString["text"] = e.Entry.TextJP;
					string translateResult = webClient.DownloadString( "/translate_a/t" );
					string english = CleanGoogleTranslatedString( translateResult );
					e.Entry.TextEN = english;

					ReinsertEntry( conn, e, param );
				} catch ( WebException ex ) {
					LogWriter.WriteLine( "Failure in File " + Filename + ":" );
					LogWriter.WriteLine( "ID: " + e.Entry.ID );
					LogWriter.WriteLine( e.Entry.TextJP );
					LogWriter.WriteLine( ex.ToString() );
					LogWriter.WriteLine();
					Console.WriteLine( ex.ToString() );
					Console.WriteLine();
				}
				System.Threading.Thread.Sleep( rng.Next( 2000, 8000 ) );
			}

			LogWriter.Close();
			FailLog.Close();

			conn.Close();

			return;
		}

		public static void ReinsertEntry( SQLiteConnection conn, TranslatableGraceNoteEntry e, Object[] param ) {

			Console.WriteLine( "ENGLISH WITHOUT LINEBREAKS:" );
			Console.WriteLine( e.Entry.TextEN );

			int NewLineCount = e.NewLineCount;
			if ( e.NewLineAtEnd ) { NewLineCount--; }


			if ( NewLineCount != 0 ) {

				int approxCharsPerLine = e.Entry.TextEN.Length / ( NewLineCount + 1 );

				List<string> parts = new List<string>( NewLineCount + 1 );
				int loc = 0;
				while ( loc < e.Entry.TextEN.Length ) {
					int start = loc;
					loc += approxCharsPerLine;
					while ( loc < e.Entry.TextEN.Length && e.Entry.TextEN[loc] != ' ' ) {
						loc++;
					}

					if ( loc >= e.Entry.TextEN.Length ) {
						loc = e.Entry.TextEN.Length;
					}

					string sub = e.Entry.TextEN.Substring( start, loc - start );
					parts.Add( sub );
					loc++;
				}

				e.Entry.TextEN = "";
				foreach ( string part in parts ) {
					e.Entry.TextEN = e.Entry.TextEN + part + "\n";
				}
				e.Entry.TextEN = e.Entry.TextEN.TrimEnd( '\n' );
			}


			if ( e.NewLineAtEnd ) { e.Entry.TextEN = e.Entry.TextEN + '\n'; }
			e.Entry.TextEN = e.PreserveStringAtStart + e.Entry.TextEN + e.PreserveStringAtEnd;

			Console.WriteLine( "JAPANESE:" );
			Console.WriteLine( e.Entry.TextJP );
			Console.WriteLine( "ENGLISH WITH LINEBREAKS:" );
			Console.WriteLine( e.Entry.TextEN );
			Console.WriteLine( "----------------------------------------" );
			Console.WriteLine();

			param[0] = e.Entry.TextEN;
			param[1] = e.Entry.ID;
			Util.GenericSqliteUpdate( conn, "UPDATE Text SET english = ?, UpdatedBy = 'GoogleTranslate', UpdatedTimestamp = " + Util.DateTimeToUnixTime( DateTime.Now ) + " WHERE ID = ?", param );
		}


		public static string CleanGoogleTranslatedString( string str ) {
			//string example = "[[[\"Cooking Party\",\"パーティーの料理\",\"\",\"Pātī no ryōri\"]],,\"ja\",,[[\"Cooking\",[4],1,0,320,0,1,0],[\"Party\",[5],1,0,314,1,2,0]],[[\"料理\",4,[[\"Cooking\",320,1,0],[\"Cuisine\",15,1,0],[\"Food\",8,1,0],[\"Dishes\",0,1,0],[\"Cuisines\",0,1,0]],[[6,8]],\"パーティーの料理\"],[\"の パーティー\",5,[[\"Party\",314,1,0],[\"a party\",0,1,0],[\"parties\",0,1,0],[\"for party\",0,1,0]],[[0,6]],\"\"]],,,[[\"ja\"]],30]";
			//string example = "[[[\"[ Cuisine \\\" of \\\" party ]\",\"[パーティー\\\"の\\\"料理]\",\"\",\"[Pātī\\\" no\\\" ryōri]\"]],,\"ja\",,[[\"[\",[4],1,1,1000,0,1,0],[\"Cuisine \\\"\",[5],0,1,902,1,3,0],[\"of\",[6],0,0,821,3,4,0],[\"\\\"\",[7],0,0,821,4,5,0],[\"party\",[8],1,0,782,5,6,0],[\"]\",[9],0,0,782,6,7,0]],[[\"[\",4,[[\"[\",1000,1,1]],[[0,1]],\"[パーティー\\\"の\\\"料理]\"],[\"料理 \\\"\",5,[[\"Cuisine \\\"\",902,0,1]],[[6,7],[9,11]],\"\"],[\"の\",6,[[\"of\",821,0,0],[\"of the\",106,0,0],[\"in\",2,0,0],[\"the\",0,0,0],[\"for\",0,0,0]],[[7,8]],\"\"],[\"\\\"\",7,[[\"\\\"\",821,0,0]],[[8,9]],\"\"],[\"パーティー\",8,[[\"party\",782,1,0],[\"parties\",0,1,0],[\"Event\",0,1,0],[\"the party\",0,1,0],[\"a party\",0,1,0]],[[1,6]],\"\"],[\"]\",9,[[\"]\",782,0,0]],[[11,12]],\"\"]],,,[[\"ja\"]],1]";
			//string exmaple = "[[[\"Party\\n\",\"パーティー\",\"\",\"Pātī\"],[\"Of\\n\",\"の\",\"\",\"No\"],[\"Cooking\",\"料理\",\"\",\"Ryōri\"]],,\"ja\",,[[\"Party\",[4],1,0,1000,0,1,0],[\"\\n\",,0,0,0,0,0,0],[\"Of\",[11],1,0,1000,0,1,1],[\"\\n\",,0,0,0,0,0,0],[\"Cooking\",[21],1,0,794,0,1,1]],[[\"パーティー\",4,[[\"Party\",1000,1,0],[\"Parties\",0,1,0],[\"Event\",0,1,0],[\"The party\",0,1,0],[\"A party\",0,1,0]],[[0,5]],\"パーティー\"],[\"の\",11,[[\"Of\",1000,1,0],[\"The\",0,1,0],[\"Of the\",0,1,0],[\"In\",0,1,0],[\"For\",0,1,0]],[[0,1]],\"の\"],[\"料理\",21,[[\"Cooking\",794,1,0],[\"Cuisine\",166,1,0],[\"Dish\",38,1,0],[\"Fare\",0,1,0],[\"Cuisines\",0,1,0]],[[0,2]],\"料理\"]],,,[[\"ja\"]],60]";
			//string example = "[[[\"Corpse of the victim was discovered , hall of hotel and future old wing . \",\"被害者の死体が発見されたのは、ホテル・ミライ旧館の大広間。\",\"\",\"Higaisha no shitai ga hakken sa reta no wa, hoteru mirai kyūkan no ōhiroma.\"],[\"Time of death is 30 minutes around 11:00 pm . \",\"死亡時刻は午後１１時３０分頃。\",\"\",\"Shibō jikoku wa gogo 11-ji 30-bu koro.\"],[\"In the fatal stabbing , have been rarely stabbed dozens of locations through the throat from the abdomen cause of death . \",\"死因は刺殺で、腹部から喉にかけての数十ヵ所をめった刺しにされている。\",\"\",\"Shiin wa shisatsu de, fukubu kara nodo ni kakete no sū jū-kasho o metta-zashi ni sa rete iru.\"],[\"No trauma especially otherwise , no trace of the ingested chemicals such as toxins .\",\"それ以外には特に外傷もなく、毒物などの薬品類を摂取した痕跡もない。\",\"\",\"Sore igai ni wa tokuni gaishō mo naku, dokubutsu nado no yakuhin-rui o sesshu shita konseki mo nai.\"]],,\"ja\",,[[\"Corpse\",[4],1,0,1000,0,1,0],[\"of\",[5],1,0,1000,1,2,0],[\"the victim\",[6],1,0,1000,2,4,0],[\"was discovered\",[7],1,0,994,4,6,0],[\",\",[8],0,0,984,6,7,0],[\"hall\",[9],1,0,673,7,8,0],[\"of\",[10],1,0,673,8,9,0],[\"hotel\",[11],1,0,456,9,10,0],[\"and future\",[12],1,0,449,10,12,0],[\"old wing\",[13],1,0,464,12,14,0],[\".\",[14],0,0,945,14,15,0],[\"Time of death\",[29],1,0,989,0,3,1],[\"is\",[30],1,0,773,3,4,1],[\"30\",[31],1,0,552,4,5,1],[\"minutes\",[32],1,0,552,5,6,1],[\"around\",[33],1,0,773,6,7,1],[\"11:00\",[34],1,0,923,7,8,1],[\"pm\",[35],1,0,976,8,9,1],[\".\",[36],0,0,976,9,10,1],[\"In the\",[60],1,0,1000,0,2,1],[\"fatal stabbing\",[61],1,0,1000,2,4,1],[\",\",[62],0,0,1000,4,5,1],[\"have been\",[63],1,0,991,5,7,1],[\"rarely\",[64],1,0,953,7,8,1],[\"stabbed\",[65],1,0,941,8,9,1],[\"dozens\",[66],1,0,987,9,10,1],[\"of locations\",[67],1,0,991,10,12,1],[\"through\",[68],1,0,669,12,13,1],[\"the\",[69],1,0,669,13,14,1],[\"throat\",[70],1,0,669,14,15,1],[\"from\",[71],1,0,570,15,16,1],[\"the abdomen\",[72],1,0,551,16,18,1],[\"cause of death\",[73],1,0,941,18,21,1],[\".\",[74],0,0,998,21,22,1],[\"No\",[114],1,0,821,0,1,1],[\"trauma\",[115],1,0,617,1,2,1],[\"especially\",[116],1,0,532,2,3,1],[\"otherwise\",[117],1,0,442,3,4,1],[\",\",[118],0,0,450,4,5,1],[\"no\",[119],1,0,450,5,6,1],[\"trace of\",[120],1,0,450,6,8,1],[\"the\",[121],1,0,617,8,9,1],[\"ingested\",[122],1,0,617,9,10,1],[\"chemicals\",[123],1,0,617,10,11,1],[\"such as\",[124],1,0,617,11,13,1],[\"toxins\",[125],1,0,617,13,14,1],[\".\",[126],0,0,617,14,15,1]],[[\"死体\",4,[[\"Corpse\",1000,1,0],[\"Corpses\",0,1,0],[\"Carcass\",0,1,0],[\"Carcasses\",0,1,0],[\"Dead bodies\",0,1,0]],[[4,6]],\"被害者の死体が発見されたのは、ホテル・ミライ旧館の大広間。\"],[\"の\",5,[[\"of\",1000,1,0],[\"the\",0,1,0],[\"of the\",0,1,0],[\"in\",0,1,0],[\"for\",0,1,0]],[[3,4]],\"\"],[\"被害 者 が\",6,[[\"the victim\",1000,1,0],[\"victim\",0,1,0],[\"victims\",0,1,0],[\"the victims\",0,1,0]],[[0,3],[6,7]],\"\"],[\"の た れ さ 発見 は\",7,[[\"was discovered\",994,1,0]],[[7,14]],\"\"],[\",\",8,[[\",\",984,0,0]],[[14,15]],\"\"],[\"大広間\",9,[[\"hall\",673,1,0],[\"salon\",90,1,0],[\"saloon\",33,1,0],[\"Ballroom\",0,1,0],[\"salons\",0,1,0]],[[25,28]],\"\"],[\"の\",10,[[\"of\",673,1,0],[\"the\",33,1,0],[\"of the\",0,1,0],[\"in\",0,1,0],[\"for\",0,1,0]],[[24,25]],\"\"],[\"ホテル\",11,[[\"hotel\",456,1,0],[\"hotels\",103,1,0],[\"hotels in\",0,1,0]],[[15,18]],\"\"],[\"· ミライ\",12,[[\"and future\",449,1,0],[\"and the future\",1,1,0]],[[18,22]],\"\"],[\"旧館\",13,[[\"old wing\",464,1,0],[\"old building\",22,1,0],[\"older building\",1,1,0],[\"older part\",0,1,0],[\"the old building\",0,1,0]],[[22,24]],\"\"],[\".\",14,[[\".\",945,0,0]],[[28,29]],\"\"],[\"死亡 時刻\",29,[[\"Time of death\",989,1,0],[\"The time of death\",0,1,0],[\"Hour of death\",0,1,0]],[[0,4]],\"死亡時刻は午後１１時３０分頃。\"],[\"は\",30,[[\"is\",773,1,0],[\"The\",10,1,0],[\"you\",0,1,0],[\"to\",0,1,0],[\"are\",0,1,0]],[[4,5]],\"\"],[\"30\",31,[[\"30\",552,1,0],[\"thirty\",0,1,0]],[[10,12]],\"\"],[\"分\",32,[[\"minutes\",552,1,0],[\"minute\",0,1,0],[\"min\",0,1,0],[\"worth\",0,1,0]],[[12,13]],\"\"],[\"頃\",33,[[\"around\",773,1,0],[\"Circa\",0,1,0],[\"around the\",0,1,0]],[[13,14]],\"\"],[\"11 時\",34,[[\"11:00\",923,1,0],[\"11 o'clock\",0,1,0],[\"eleven o'clock\",0,1,0]],[[7,10]],\"\"],[\"午後\",35,[[\"pm\",976,1,0],[\"afternoon\",0,1,0],[\"the afternoon\",0,1,0],[\"afternoons\",0,1,0],[\"afternoon of\",0,1,0]],[[5,7]],\"\"],[\".\",36,[[\".\",976,0,0]],[[14,15]],\"\"],[\"で\",60,[[\"In the\",1000,1,0],[\"In\",0,1,0],[\"At\",0,1,0],[\"With\",0,1,0],[\"By\",0,1,0]],[[5,6]],\"死因は刺殺で、腹部から喉にかけての数十ヵ所をめった刺しにされている。\"],[\"刺殺\",61,[[\"fatal stabbing\",1000,1,0],[\"stabbing\",0,1,0],[\"stabbing to death\",0,1,0],[\"putting out\",0,1,0]],[[3,5]],\"\"],[\",\",62,[[\",\",1000,0,0]],[[6,7]],\"\"],[\"いる て れ さ に\",63,[[\"have been\",991,1,0],[\"have been in\",0,1,0],[\"has been in\",0,1,0],[\"have been in the\",0,1,0],[\"has been on\",0,1,0]],[[27,33]],\"\"],[\"めった\",64,[[\"rarely\",953,1,0],[\"rare\",35,1,0],[\"seldom\",3,1,0]],[[22,25]],\"\"],[\"刺し を\",65,[[\"stabbed\",941,1,0],[\"pierced\",7,1,0],[\"stabbing\",0,1,0],[\"stab\",0,1,0],[\"puncture\",0,1,0]],[[21,22],[25,27]],\"\"],[\"数 十\",66,[[\"dozens\",987,1,0],[\"several tens\",0,1,0],[\"tens\",0,1,0],[\"several tens of\",0,1,0],[\"tens of\",0,1,0]],[[17,19]],\"\"],[\"ヵ所\",67,[[\"of locations\",991,1,0]],[[19,21]],\"\"],[\"にかけて\",68,[[\"through\",669,1,0],[\"between\",306,1,0],[\"over the\",0,1,0],[\"toward\",0,1,0],[\"toward the\",0,1,0]],[[12,16]],\"\"],[\"の\",69,[[\"the\",669,1,0],[\"of\",15,1,0],[\"in\",6,1,0],[\"for\",0,1,0],[\"of the\",0,1,0]],[[16,17]],\"\"],[\"喉\",70,[[\"throat\",669,1,0],[\"the throat\",0,1,0],[\"throats\",0,1,0],[\"a throat\",0,1,0]],[[11,12]],\"\"],[\"から\",71,[[\"from\",570,1,0],[\"from the\",86,1,0],[\"of\",14,1,0],[\"from a\",0,1,0],[\"the\",0,1,0]],[[9,11]],\"\"],[\"腹部 は\",72,[[\"the abdomen\",551,1,0],[\"abdomen\",123,1,0],[\"abdominal\",2,1,0],[\"ventral\",0,1,0]],[[2,3],[7,9]],\"\"],[\"死因\",73,[[\"cause of death\",941,1,0],[\"causes of death\",1,1,0],[\"death\",0,1,0],[\"cause\",0,1,0],[\"deaths\",0,1,0]],[[0,2]],\"\"],[\".\",74,[[\".\",998,0,0]],[[33,34]],\"\"],[\"なく も\",114,[[\"No\",821,1,0],[\"Without\",178,1,0],[\"Without a\",0,1,0],[\"Without any\",0,1,0],[\"Without the\",0,1,0]],[[10,13]],\"それ以外には特に外傷もなく、毒物などの薬品類を摂取した痕跡もない。\"],[\"外傷\",115,[[\"trauma\",617,1,0],[\"injury\",204,1,0],[\"traumatic\",0,1,0],[\"injuries\",0,1,0],[\"wound\",0,1,0]],[[8,10]],\"\"],[\"特に は\",116,[[\"especially\",532,1,0],[\"particularly\",23,1,0],[\"particular\",0,1,0],[\"in particular\",0,1,0],[\"and particularly\",0,1,0]],[[5,8]],\"\"],[\"に それ 以外\",117,[[\"otherwise\",442,1,0],[\"Other than\",0,1,0],[\"Other than that\",0,1,0]],[[0,5]],\"\"],[\",\",118,[[\",\",450,0,0]],[[13,14]],\"\"],[\"ない も\",119,[[\"no\",450,1,0],[\"there is no\",0,1,0],[\"nor\",0,1,0],[\"not even\",0,1,0],[\"neither\",0,1,0]],[[29,32]],\"\"],[\"痕跡 (aux:relc)\",120,[[\"trace of\",450,1,0],[\"traces\",0,1,0],[\"traces of\",0,1,0],[\"signs of\",0,1,0]],[[27,29]],\"\"],[\"(null:pronoun) は\",121,[[\"the\",617,1,0],[\"to\",0,1,0],[\"and\",0,1,0],[\"you\",0,1,0],[\"it\",0,1,0]],[[26,27]],\"\"],[\"た し 摂取 を\",122,[[\"ingested\",617,1,0],[\"ingesting\",0,1,0]],[[22,27]],\"\"],[\"薬品 類\",123,[[\"chemicals\",617,1,0],[\"chemicals for\",0,1,0],[\"chemicals away\",0,1,0]],[[19,22]],\"\"],[\"の など\",124,[[\"such as\",617,1,0],[\"such\",0,1,0],[\"such as a\",0,1,0]],[[16,19]],\"\"],[\"毒物\",125,[[\"toxins\",617,1,0],[\"poison\",0,1,0],[\"poisons\",0,1,0],[\"toxicology\",0,1,0],[\"toxicant\",0,1,0]],[[14,16]],\"\"],[\".\",126,[[\".\",617,0,0]],[[32,33]],\"\"]],,,[[\"ja\"]],6]";
			//str = example;

			// first grab the part that actually has the translated string
			StringBuilder sb = new StringBuilder();
			List<string> list = new List<string>();
			int bracketCount = 0;
			foreach ( char c in str ) {
				switch ( c ) {
					case ',':
						if ( sb.Length != 0 ) { sb.Append( c ); }
						break;
					case '[':
						bracketCount++;
						if ( bracketCount > 3 ) sb.Append( c );
						break;
					case ']':
						if ( bracketCount > 3 ) sb.Append( c );
						bracketCount--;
						if ( bracketCount == 2 ) {
							list.Add( sb.ToString() );
							sb = new StringBuilder();
						}
						if ( bracketCount == 1 ) goto post;
						break;
					default: sb.Append( c ); break;
				}
			}

		post:
			// then grab the quote-enclosed part (the translation)
			string newstr = "";
			foreach ( string s in list ) {
				str = s;
				for ( int i = 1; i < str.Length; ++i ) {
					if ( str[i] == '"' ) {
						if ( str[i - 1] != '\\' ) {
							str = str.Substring( 1, i - 1 );
							break;
						}
					}
				}

				// and fix up the escaped stuff
				str = str.Replace( "\\\"", "\"" );
				str = str.Replace( "\\\\", "\\" );
				str = str.Replace( "\\u003c", "<" );
				str = str.Replace( "\\u003e", ">" );

				newstr = newstr + str + " ";
			}

			// and fix up some weird spacing that google gives
			while ( newstr.Contains( " ." ) ) { newstr = newstr.Replace( " .", "." ); }
			while ( newstr.Contains( " ," ) ) { newstr = newstr.Replace( " ,", "," ); }
			while ( newstr.Contains( " !" ) ) { newstr = newstr.Replace( " !", "!" ); }
			while ( newstr.Contains( " ?" ) ) { newstr = newstr.Replace( " ?", "?" ); }
			while ( newstr.Contains( " '" ) ) { newstr = newstr.Replace( " '", "'" ); }
			while ( newstr.Contains( "' " ) ) { newstr = newstr.Replace( "' ", "'" ); }
			while ( newstr.Contains( "- " ) ) { newstr = newstr.Replace( "- ", "-" ); }
			while ( newstr.Contains( " -" ) ) { newstr = newstr.Replace( " -", "-" ); }
			while ( newstr.Contains( "\\ " ) ) { newstr = newstr.Replace( "\\ ", "\\" ); }
			while ( newstr.Contains( " \\" ) ) { newstr = newstr.Replace( " \\", "\\" ); }
			while ( newstr.Contains( "/ " ) ) { newstr = newstr.Replace( "/ ", "/" ); }
			while ( newstr.Contains( " /" ) ) { newstr = newstr.Replace( " /", "/" ); }
			while ( newstr.Contains( "% " ) ) { newstr = newstr.Replace( "% ", "%" ); }
			while ( newstr.Contains( " %" ) ) { newstr = newstr.Replace( " %", "%" ); }
			while ( newstr.Contains( "# " ) ) { newstr = newstr.Replace( "# ", "#" ); }
			while ( newstr.Contains( " #" ) ) { newstr = newstr.Replace( " #", "#" ); }

			while ( newstr.Contains( "...." ) ) { newstr = newstr.Replace( "....", "..." ); }
			while ( newstr.Contains( "  " ) ) { newstr = newstr.Replace( "  ", " " ); }


			for ( int i = 0; i < ReplacementOriginals.Count; ++i ) {
				newstr = newstr.Replace( ReplacementSubstitutes[i], ReplacementOriginals[i] );
			}

			return newstr;
		}

	}
}
