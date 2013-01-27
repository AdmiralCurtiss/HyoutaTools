using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using HyoutaTools.DanganRonpa.Lin;

namespace HyoutaTools.GraceNote.DanganRonpa.LinImport {
	class ScenarioString {
		public int Pointer;
		public String Jpn;
		public String Eng;

		public ScenarioString( int Pointer, String Jpn, String Eng ) {
			this.Pointer = Pointer;
			this.Jpn = Jpn;
			this.Eng = Eng;
		}
	}

	class Program {
		public static int Execute( string[] args ) {
			if ( args.Length < 1 ) {
				Console.WriteLine( "Usage: DanganRonpaText_GraceNote -dump text.lin NewDBFile TemplateDBFile GracesJapanese" );
				Console.WriteLine( "Usage: DanganRonpaText_GraceNote -insert text.lin.orig text.lin.new database [alignment (default 1024)]" );
				Console.WriteLine( "Usage: DanganRonpaText_GraceNote -dumptxt text.lin out.txt" );
				return -1;
			}

			// templateDB must contain:
			/***
			 * CREATE TABLE Text(ID int primary key, StringID int, english text, comment text, updated tinyint, status tinyint, PointerRef integer, IdentifyString text, IdentifyPointerRef integer);
			 ***/

			if ( args[0] == "-dump" ) {
				return Dump( args[1], args[2], args[3], args[4] );
			} else if ( args[0] == "-insert" ) {
				int Alignment;
				if ( !( args.Length >= 5 && Int32.TryParse( args[4], out Alignment ) ) ) {
					Alignment = 1024;
				}
				return Insert( args[1], args[2], args[3], Alignment );
			} else if ( args[0] == "-check" ) {
				return Check( args[1] );
			} else if ( args[0] == "-dumpinsertcheck" ) {
				int Alignment = 16;
				Dump( args[1], args[2], args[3], args[4] );
				Insert( args[1], args[1] + ".new", args[2], Alignment );


				Byte[] OriginalFile = System.IO.File.ReadAllBytes( args[1] );
				LIN lin = new LIN( OriginalFile );
				lin.CreateFile( Alignment );
				return Compare( OriginalFile, System.IO.File.ReadAllBytes( args[1] + ".new" ), lin.UnalignedFilesize, args[1] + ".fail" );
			} else if ( args[0] == "-dumptxt" ) {
				return DumpTxt( args[1], args[2] );
			}

			return -1;
		}

		public static int Compare( Byte[] OriginalFile, Byte[] RecreatedFile, int UnalignedFilesize, string outputOnFail ) {
			try {
				if ( OriginalFile.Length != RecreatedFile.Length ) {
					throw new Exception( "Filesizes don't match!" );
				}

				bool fail = false;
				for ( int i = 0; i < OriginalFile.Length; ++i ) {
					if ( OriginalFile[i] != RecreatedFile[i] ) {
						if ( i >= UnalignedFilesize ) {
							Console.WriteLine( "Acceptable mismatch (stray end bytes) at byte 0x" + i.ToString( "X8" ) );
						} else {
							Console.WriteLine( "Mismatch at byte 0x" + i.ToString( "X8" ) );
							fail = true;
						}
					}
				}
				if ( fail ) { throw new Exception( "Mismatch in file." ); }
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				System.IO.File.WriteAllBytes( outputOnFail, RecreatedFile );
				return -1;
			}

			Console.WriteLine( "Check done." );
			return 0;
		}

		public static int Check( String Filename ) {
			Byte[] OriginalFile = System.IO.File.ReadAllBytes( Filename );
			LIN lin = new LIN( OriginalFile );
			Byte[] RecreatedFile = lin.CreateFile( 1024 );
			LIN linOrig = new LIN( OriginalFile );

			return Compare( OriginalFile, RecreatedFile, lin.UnalignedFilesize, Filename + ".fail" );
		}

		public static int Insert( String InFilename, String OutFilename, String DB, int Alignment ) {
			LIN lin;
			try {
				lin = new LIN( InFilename );
				lin.GetSQL( "Data Source=" + DB );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				Console.WriteLine( "Failed loading text file!" );
				return -1;
			}
			byte[] newfile = lin.CreateFile( Alignment );
			System.IO.File.WriteAllBytes( OutFilename, newfile );
			return 0;
		}

		public static int Dump( String Filename, String NewDB, String TemplateDB, String GracesDB ) {
			LIN lin;
			try {
				lin = new LIN( Filename );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				Console.WriteLine( "Failed loading text file!" );
				return -1;
			}

			Console.WriteLine( "Importing..." );
			System.IO.File.Copy( TemplateDB, NewDB, true );
			InsertSQL( lin, "Data Source=" + NewDB, "Data Source=" + GracesDB );
			Console.WriteLine( "Successfully imported entries!" );
			return 0;
		}

		public static int DumpTxt( String Filename, String TxtFilename ) {
			LIN lin;
			try {
				lin = new LIN( Filename );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				Console.WriteLine( "Failed loading text file!" );
				return -1;
			}



			List<string> Output = new List<string>();

			foreach ( ScriptEntry s in lin.ScriptData ) {
				if ( s.Type == 0x02 ) {
					Output.Add( "Text: " + s.Text );
				} else {
					Output.Add( s.FormatForGraceNote() );
				}
			}

			if ( lin.UnreferencedText != null ) {
				foreach ( KeyValuePair<int, string> u in lin.UnreferencedText ) {
					Output.Add( "Unreferenced Text (" + u.Value + "): " + u.Key );
				}
			}

			System.IO.File.WriteAllLines( TxtFilename, Output.ToArray() );
			return 0;
		}

		public static bool InsertSQL( LIN lin, String ConnectionString, String ConnectionStringGracesJapanese ) {
			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			SQLiteConnection ConnectionGracesJapanese = new SQLiteConnection( ConnectionStringGracesJapanese );
			Connection.Open();
			ConnectionGracesJapanese.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteTransaction TransactionGracesJapanese = ConnectionGracesJapanese.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) )
			using ( SQLiteCommand CommandGracesJapanese = new SQLiteCommand( ConnectionGracesJapanese ) )
			using ( SQLiteCommand CommandJapaneseID = new SQLiteCommand( ConnectionGracesJapanese ) )
			using ( SQLiteCommand CommandSearchJapanese = new SQLiteCommand( ConnectionGracesJapanese ) ) {
				SQLiteParameter JapaneseIDParam = new SQLiteParameter();
				SQLiteParameter JapaneseParam = new SQLiteParameter();
				SQLiteParameter EnglishIDParam = new SQLiteParameter();
				SQLiteParameter StringIDParam = new SQLiteParameter();
				SQLiteParameter EnglishParam = new SQLiteParameter();
				SQLiteParameter PointerRefParam = new SQLiteParameter();
				SQLiteParameter JapaneseSearchParam = new SQLiteParameter();
				SQLiteParameter EnglishStatusParam = new SQLiteParameter();
				SQLiteParameter IdentifyStringParam = new SQLiteParameter();
				SQLiteParameter IdentifyPointerRefParam = new SQLiteParameter();


				CommandGracesJapanese.CommandText = "INSERT INTO Japanese (ID, string, debug) VALUES (?, ?, 0)";
				CommandGracesJapanese.Parameters.Add( JapaneseIDParam );
				CommandGracesJapanese.Parameters.Add( JapaneseParam );

				Command.CommandText = "INSERT INTO Text (ID, StringID, english, comment, updated, status, PointerRef, IdentifyString, IdentifyPointerRef) VALUES (?, ?, ?, \"\", 0, ?, ?, ?, ?)";
				Command.Parameters.Add( EnglishIDParam );
				Command.Parameters.Add( StringIDParam );
				Command.Parameters.Add( EnglishParam );  // Line.SENG
				Command.Parameters.Add( EnglishStatusParam );
				Command.Parameters.Add( PointerRefParam ); // Line.Location
				Command.Parameters.Add( IdentifyStringParam ); // Line.Location
				Command.Parameters.Add( IdentifyPointerRefParam ); // Line.Location

				CommandJapaneseID.CommandText = "SELECT MAX(ID)+1 FROM Japanese";

				CommandSearchJapanese.CommandText = "SELECT ID FROM Japanese WHERE string = ? AND debug = 0";
				CommandSearchJapanese.Parameters.Add( JapaneseSearchParam );

				object JPMaxIDObject = CommandJapaneseID.ExecuteScalar();
				int JPMaxID;
				try {
					JPMaxID = Int32.Parse( JPMaxIDObject.ToString() ); // wtf why doesn't this work directly?
				} catch ( System.FormatException ) {
					// there's no ID in the database, just start with 0
					JPMaxID = 0;
				}
				int ENID = 1;


				string TextToInsert = "";
				foreach ( ScriptEntry s in lin.ScriptData ) {
					if ( s.Type == 0x02 ) {
						ExecuteInsert( TextToInsert, 1, "[Game Code]", 0, ref JapaneseSearchParam, CommandSearchJapanese, ref JPMaxID, ref ENID, ref JapaneseIDParam, ref JapaneseParam, CommandGracesJapanese, ref EnglishIDParam, ref StringIDParam, ref EnglishParam, ref EnglishStatusParam, ref PointerRefParam, Command, ref IdentifyStringParam, ref IdentifyPointerRefParam );
						ENID++;
						ExecuteInsert( s.Text, 2, s.IdentifyString, 0, ref JapaneseSearchParam, CommandSearchJapanese, ref JPMaxID, ref ENID, ref JapaneseIDParam, ref JapaneseParam, CommandGracesJapanese, ref EnglishIDParam, ref StringIDParam, ref EnglishParam, ref EnglishStatusParam, ref PointerRefParam, Command, ref IdentifyStringParam, ref IdentifyPointerRefParam );
						ENID++;
						TextToInsert = "";
						continue;
					}
					TextToInsert = TextToInsert + s.FormatForGraceNote() + '\n';
				}
				if ( TextToInsert != null ) {
					ExecuteInsert( TextToInsert, 1, "[Game Code]", 0, ref JapaneseSearchParam, CommandSearchJapanese, ref JPMaxID, ref ENID, ref JapaneseIDParam, ref JapaneseParam, CommandGracesJapanese, ref EnglishIDParam, ref StringIDParam, ref EnglishParam, ref EnglishStatusParam, ref PointerRefParam, Command, ref IdentifyStringParam, ref IdentifyPointerRefParam );
					ENID++;
				}

				if ( lin.UnreferencedText != null ) {
					foreach ( KeyValuePair<int, string> u in lin.UnreferencedText ) {
						ExecuteInsert( u.Value, 3, "[Unreferenced Text]", u.Key, ref JapaneseSearchParam, CommandSearchJapanese, ref JPMaxID, ref ENID, ref JapaneseIDParam, ref JapaneseParam, CommandGracesJapanese, ref EnglishIDParam, ref StringIDParam, ref EnglishParam, ref EnglishStatusParam, ref PointerRefParam, Command, ref IdentifyStringParam, ref IdentifyPointerRefParam );
						ENID++;
					}
				}

				Transaction.Commit();
				TransactionGracesJapanese.Commit();
			}
			ConnectionGracesJapanese.Close();
			Connection.Close();

			return true;
		}


		public static int ExecuteInsert( string TextToInsert, int PointerRefToInsert, string IdentStringToInsert, int IdentRefToInsert,
		ref SQLiteParameter JapaneseSearchParam, /*ref*/ SQLiteCommand CommandSearchJapanese,
		ref int JPMaxID, ref int ENID, ref SQLiteParameter JapaneseIDParam, ref SQLiteParameter JapaneseParam,
			/*ref*/ SQLiteCommand CommandGracesJapanese, ref SQLiteParameter EnglishIDParam, ref SQLiteParameter StringIDParam,
		ref SQLiteParameter EnglishParam, ref SQLiteParameter EnglishStatusParam, ref SQLiteParameter PointerRefParam,
			/*ref*/ SQLiteCommand Command, ref SQLiteParameter IdentifyStringParam, ref SQLiteParameter IdentifyPointerRefParam ) {
			int JPID;

			// fetch GracesJapanese ID or generate new & insert new text
			JapaneseSearchParam.Value = TextToInsert;
			object JPIDobj = CommandSearchJapanese.ExecuteScalar();
			if ( JPIDobj != null ) {
				JPID = (int)JPIDobj;
			} else {
				JPID = JPMaxID++;
				JapaneseIDParam.Value = JPID;
				JapaneseParam.Value = TextToInsert;
				CommandGracesJapanese.ExecuteNonQuery();
			}

			// insert text into English table
			EnglishIDParam.Value = ENID;
			StringIDParam.Value = JPID;
			EnglishParam.Value = TextToInsert;
			EnglishStatusParam.Value = 0;
			PointerRefParam.Value = PointerRefToInsert;
			IdentifyStringParam.Value = IdentStringToInsert;
			IdentifyPointerRefParam.Value = IdentRefToInsert;
			return Command.ExecuteNonQuery();
		}
	}
}
