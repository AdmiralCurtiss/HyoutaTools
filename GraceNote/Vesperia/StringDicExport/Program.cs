using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.TSS;

namespace HyoutaTools.GraceNote.Vesperia.StringDicExport {
	class Program {
		public static int Execute( List<string> args ) {
			bool UseInsaneNames = false;
			bool RealMode = false;
			bool UpdateInGameId = false;

			if ( args.Count == 1 ) {
				if ( args[0] == "-insane" ) {
					Console.WriteLine( "Wesker-Dumbledore Mode Activated!" );
					UseInsaneNames = true;
				}
				if ( args[0] == "-real" ) {
					Console.WriteLine( "Real Mode activated, resulting file will contain both English and Japanese data as expected by an unmodified game." );
					RealMode = true;
				}
				if ( args[0] == "-updateInGameId" ) {
					Console.WriteLine( "Updating InGameIDs in the database." );
					UpdateInGameId = true;
				}
			}


			Console.WriteLine( "Opening STRING_DIC.SO..." );
			TSSFile TSS;
			try {
				TSS = new TSSFile( System.IO.File.ReadAllBytes( "STRING_DIC.SO" ) );
			} catch ( System.IO.FileNotFoundException ) {
				Console.WriteLine( "Could not open STRING_DIC.SO, exiting." );
				return -1;
			}


			if ( args.Count == 1 ) {
				if ( args[0] == "-engorigdmp" ) {
					Console.WriteLine( "Exporting original text (english)..." );
					System.IO.File.WriteAllBytes( "STRING_DIC_original_eng_export.txt", TSS.ExportTextForEnglishDump() );
				} else if ( args[0] == "-origdmp" ) {
					Console.WriteLine( "Exporting original text (all)..." );
					System.IO.File.WriteAllBytes( "STRING_DIC_original_export.txt", TSS.ExportText() );
				}
			}


			Console.WriteLine( "Importing databases..." );
			if ( !TSS.ImportSQL( placeEnglishInJpnEntry: !RealMode, updateDatabaseWithInGameStringId: UpdateInGameId ) ) {
				Console.WriteLine( "Could not import all databases! Exiting..." );
				return -1;
			}


			// Empty unused strings, alter names if wanted
			if ( !RealMode ) {
				foreach ( TSSEntry e in TSS.Entries ) {
					if ( e.StringENG != null ) {
						e.StringENG = "";
					}

					if ( UseInsaneNames ) {
						e.StringJPN = HyoutaTools.Tales.Vesperia.VesperiaUtil.ReplaceWithInsaneNames( e.StringJPN );
					}
				}
			}


			Console.WriteLine( "Writing translated file..." );
			System.IO.File.WriteAllBytes( "STRING_DIC_translated.SO", TSS.Serialize() );


			Console.WriteLine( "Writing text dump..." );
			System.IO.File.WriteAllBytes( "STRING_DIC_translated_export.txt", TSS.ExportText() );


			Console.WriteLine( "Done!" );
			return 0;
		}
	}
}
