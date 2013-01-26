using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.Vesperia.ScfombinImport {
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
		private static Encoding ShiftJISEncoding = Encoding.GetEncoding( "shift-jis" );

		static void Execute( string[] args ) {
			if ( args.Length != 5 ) {
				Console.WriteLine( "Usage: btlpack_GraceNote btlpackexFile NewDBFile TemplateDBFile GracesJapanese StartOfTextpointersInHex" );
				return;
			}

			//*
			String Filename = args[0];
			String NewDB = args[1];
			String TemplateDB = args[2];
			String GracesDB = args[3];
			int TextPointersAddress = Int32.Parse( args[4], System.Globalization.NumberStyles.AllowHexSpecifier );
			//*/

			/*
			String Filename = @"c:\Users\Georg\Documents\Tales of Vesperia\#gracenote_tov_repacks\btlpack_btl_ep\154.ex";
			String NewDB = @"c:\Users\Georg\Documents\Tales of Vesperia\#gracenote_tov_repacks\btlpack_btl_ep\VBattle154";
			String TemplateDB = @"c:\Users\Georg\Documents\Tales of Vesperia\#gracenote_tov_repacks\btlpack_btl_ep\VTemplate";
			String GracesDB = @"c:\Users\Georg\Documents\Tales of Vesperia\#gracenote_tov_repacks\GNDB_ToV_Repack_v7\GNDB_ToV_Repack_v7\db\GracesJapanese";
			int TextPointersAddress = 0x4ED0;
			//*/


			byte[] btlpack = System.IO.File.ReadAllBytes( Filename );
			byte[] PointerDifferenceBytes = { btlpack[0x27], btlpack[0x26], btlpack[0x25], btlpack[0x24] };
			int PointerDifference = BitConverter.ToInt32( PointerDifferenceBytes, 0 );
			PointerDifference -= 0x400;


			ScenarioString[] AllStrings = FindAllStrings( btlpack, TextPointersAddress, PointerDifference );
			System.IO.File.Copy( TemplateDB, NewDB );
			InsertSQL( AllStrings, "Data Source=" + NewDB, "Data Source=" + GracesDB );

			return;
		}







		public static bool InsertSQL( ScenarioString[] NewStrings, String ConnectionString, String ConnectionStringGracesJapanese ) {
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
				SQLiteParameter LocationParam = new SQLiteParameter();
				SQLiteParameter JapaneseSearchParam = new SQLiteParameter();
				SQLiteParameter EnglishStatusParam = new SQLiteParameter();

				CommandGracesJapanese.CommandText = "INSERT INTO Japanese (ID, string, debug) VALUES (?, ?, 0)";
				CommandGracesJapanese.Parameters.Add( JapaneseIDParam );
				CommandGracesJapanese.Parameters.Add( JapaneseParam );

				Command.CommandText = "INSERT INTO Text (ID, StringID, english, comment, updated, status, PointerRef) VALUES (?, ?, ?, null, 0, ?, ?)";
				Command.Parameters.Add( EnglishIDParam );
				Command.Parameters.Add( StringIDParam );
				Command.Parameters.Add( EnglishParam );  // Line.SENG
				Command.Parameters.Add( EnglishStatusParam );
				Command.Parameters.Add( LocationParam ); // Line.Location

				CommandJapaneseID.CommandText = "SELECT MAX(ID)+1 FROM Japanese";

				CommandSearchJapanese.CommandText = "SELECT ID FROM Japanese WHERE string = ? AND debug = 0";
				CommandSearchJapanese.Parameters.Add( JapaneseSearchParam );

				int JPID;
				object JPMaxIDObject = CommandJapaneseID.ExecuteScalar();
				int JPMaxID = Int32.Parse( JPMaxIDObject.ToString() ); // wtf why doesn't this work directly?
				int ENID = 1;

				foreach ( ScenarioString str in NewStrings ) {
					// Name
					JapaneseSearchParam.Value = str.Jpn;
					object JPIDobj = CommandSearchJapanese.ExecuteScalar();
					if ( JPIDobj != null ) {
						JPID = (int)JPIDobj;
					} else {
						JPID = JPMaxID++;
						JapaneseIDParam.Value = JPID;
						JapaneseParam.Value = str.Jpn;
						CommandGracesJapanese.ExecuteNonQuery();
					}

					EnglishIDParam.Value = ENID;
					StringIDParam.Value = JPID;
					EnglishParam.Value = str.Eng;
					if ( str.Eng == str.Jpn ) {
						EnglishStatusParam.Value = 1;
					} else {
						EnglishStatusParam.Value = 0;
					}
					LocationParam.Value = str.Pointer;
					Command.ExecuteNonQuery();

					ENID++;
				}
				Transaction.Commit();
				TransactionGracesJapanese.Commit();
			}
			ConnectionGracesJapanese.Close();
			Connection.Close();

			return true;
		}









		private static ScenarioString[] FindAllStrings( byte[] File, int StartLocation, int PointerDifference ) {
			List<ScenarioString> AllStrings = new List<ScenarioString>();

			int Pointer = StartLocation;

			while ( true ) {
				try {
					int Pointer1 = BitConverter.ToInt32( new byte[] {
                        File[Pointer+3], File[Pointer+2], File[Pointer+1], File[Pointer]
                    }, 0 );
					int Pointer2 = BitConverter.ToInt32( new byte[] {
                        File[Pointer+7], File[Pointer+6], File[Pointer+5], File[Pointer+4]
                    }, 0 );
					int Pointer3 = BitConverter.ToInt32( new byte[] {
                        File[Pointer+11], File[Pointer+10], File[Pointer+9], File[Pointer+8]
                    }, 0 );
					int Pointer4 = BitConverter.ToInt32( new byte[] {
                        File[Pointer+15], File[Pointer+14], File[Pointer+13], File[Pointer+12]
                    }, 0 );

					if ( Pointer1 == 0 || Pointer2 == 0 || Pointer3 == 0 || Pointer4 == 0
						|| ( Pointer1 + PointerDifference ) > File.Length
						|| ( Pointer2 + PointerDifference ) > File.Length
						|| ( Pointer3 + PointerDifference ) > File.Length
						|| ( Pointer4 + PointerDifference ) > File.Length
					   ) {
						break;
					}

					ScenarioString Name = new ScenarioString( Pointer, GetText( File, Pointer1 + PointerDifference ), GetText( File, Pointer3 + PointerDifference ) );
					ScenarioString Text = new ScenarioString( Pointer + 4, GetText( File, Pointer2 + PointerDifference ), GetText( File, Pointer4 + PointerDifference ) );

					AllStrings.Add( Name );
					AllStrings.Add( Text );

					Pointer += 0x18;
				} catch ( Exception ) {
					break;
				}
			}


			return AllStrings.ToArray();
		}

		public static String GetText( byte[] File, int Pointer ) {
			try {
				int i = Pointer;

				while ( File[i] != 0x00 ) {
					i++;
				}
				String Text = ShiftJISEncoding.GetString( File, Pointer, i - Pointer );

				return Text;
			} catch ( Exception ) {
				return null;
			}
		}

	}
}
