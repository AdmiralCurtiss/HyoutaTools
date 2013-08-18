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
		public static int Execute( List<string> args ) {
			if ( args.Count != 4 ) {
				Console.WriteLine( "Usage: btlpack_GraceNote btlpackexFile NewDBFile GracesJapanese StartOfTextpointersInHex" );
				return -1;
			}

			String Filename = args[0];
			String NewDB = args[1];
			String GracesDB = args[2];
			int TextPointersAddress = Int32.Parse( args[3], System.Globalization.NumberStyles.AllowHexSpecifier );

			byte[] btlpack = System.IO.File.ReadAllBytes( Filename );
			byte[] PointerDifferenceBytes = { btlpack[0x27], btlpack[0x26], btlpack[0x25], btlpack[0x24] };
			int PointerDifference = BitConverter.ToInt32( PointerDifferenceBytes, 0 );
			PointerDifference -= 0x400;


			ScenarioString[] AllStrings = FindAllStrings( btlpack, TextPointersAddress, PointerDifference );
			System.IO.File.WriteAllBytes( NewDB, Properties.Resources.gndb_template );

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>( AllStrings.Length );
			foreach ( ScenarioString str in AllStrings ) {
				GraceNoteDatabaseEntry gn = new GraceNoteDatabaseEntry( str.Jpn, str.Eng, "", str.Eng == str.Jpn ? 1 : 0, str.Pointer, "", 0 );
				Entries.Add( gn );
			}
			GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + NewDB, "Data Source=" + GracesDB );

			return 0;
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

					ScenarioString Name = new ScenarioString( Pointer, Util.GetTextShiftJis( File, Pointer1 + PointerDifference ), Util.GetTextShiftJis( File, Pointer3 + PointerDifference ) );
					ScenarioString Text = new ScenarioString( Pointer + 4, Util.GetTextShiftJis( File, Pointer2 + PointerDifference ), Util.GetTextShiftJis( File, Pointer4 + PointerDifference ) );

					AllStrings.Add( Name );
					AllStrings.Add( Text );

					Pointer += 0x18;
				} catch ( Exception ) {
					break;
				}
			}


			return AllStrings.ToArray();
		}
	}
}
