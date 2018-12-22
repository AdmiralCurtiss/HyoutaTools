using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.TSS;

namespace HyoutaTools.GraceNote.Vesperia.StringDicExport {
	class Program {
		public static int Execute( List<string> args ) {
			bool RealMode = false;
			bool UpdateInGameId = false;
			bool GenerateGracesEnglish = false;

			if ( args.Contains( "-real" ) ) {
				Console.WriteLine( "Real Mode activated, resulting file will contain both English and Japanese data as expected by an unmodified game." );
				RealMode = true;
			}
			if ( args.Contains( "-updateInGameId" ) ) {
				Console.WriteLine( "Updating InGameIDs in the database." );
				UpdateInGameId = true;
			}
			if ( args.Contains( "-generateGracesEnglish" ) ) {
				Console.WriteLine( "Generating English version of GracesJapanese." );
				GenerateGracesEnglish = true;
			}


			Console.WriteLine( "Opening STRING_DIC.SO..." );
			TSSFile TSS;
			try {
				TSS = new TSSFile( "STRING_DIC.SO", Util.GameTextEncoding.ShiftJIS );
			} catch ( System.IO.FileNotFoundException ) {
				Console.WriteLine( "Could not open STRING_DIC.SO, exiting." );
				return -1;
			}


			if ( args.Contains( "-engorigdmp" ) ) {
				Console.WriteLine( "Exporting original text (english)..." );
				System.IO.File.WriteAllBytes( "STRING_DIC_original_eng_export.txt", TSS.ExportTextForEnglishDump() );
			}
			if ( args.Contains( "-origdmp" ) ) {
				Console.WriteLine( "Exporting original text (all)..." );
				System.IO.File.WriteAllBytes( "STRING_DIC_original_export.txt", TSS.ExportText() );
			}


			Console.WriteLine( "Importing databases..." );
			if ( !TSS.ImportSQL( placeEnglishInJpnEntry: !RealMode, updateDatabaseWithInGameStringId: UpdateInGameId, generateGracesEnglish: GenerateGracesEnglish ) ) {
				Console.WriteLine( "Could not import all databases! Exiting..." );
				return -1;
			}


			// Empty unused strings
			if ( !RealMode ) {
				foreach ( TSSEntry e in TSS.Entries ) {
					if ( e.StringEng != null ) {
						e.StringEng = "";
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
