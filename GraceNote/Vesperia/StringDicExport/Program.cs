using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.TSS;

namespace HyoutaTools.GraceNote.Vesperia.StringDicExport {
	class Program {
		static void Execute( string[] args ) {
			bool UseInsaneNames = false;

			if ( args.Length == 1 ) {
				if ( args[0] == "-insane" ) {
					Console.WriteLine( "Wesker-Dumbledore Mode Activated!" );
					UseInsaneNames = true;
				}
			}


			Console.WriteLine( "Opening STRING_DIC.SO..." );
			TSSFile TSS;
			try {
				TSS = new TSSFile( System.IO.File.ReadAllBytes( "STRING_DIC.SO" ) );
			} catch ( System.IO.FileNotFoundException ) {
				Console.WriteLine( "Could not open STRING_DIC.SO, exiting." );
				return;
			}


			if ( args.Length == 1 ) {
				if ( args[0] == "-engorigdmp" ) {
					Console.WriteLine( "Exporting original text (english)..." );
					System.IO.File.WriteAllBytes( "STRING_DIC_original_eng_export.txt", TSS.ExportTextForEnglishDump() );
				} else if ( args[0] == "-origdmp" ) {
					Console.WriteLine( "Exporting original text (all)..." );
					System.IO.File.WriteAllBytes( "STRING_DIC_original_export.txt", TSS.ExportText() );
				}
			}


			Console.WriteLine( "Importing databases..." );
			if ( !TSS.ImportSQL() ) {
				Console.WriteLine( "Could not import all databases! Exiting..." );
				return;
			}


			// Empty unused strings, alter names if wanted
			foreach ( TSSEntry e in TSS.Entries ) {
				if ( e.StringENG != null ) {
					e.StringENG = "";
				}

				if ( UseInsaneNames ) {
					e.StringJPN = HyoutaTools.Tales.Vesperia.TSS.Util.ReplaceWithInsaneNames( e.StringJPN );
				}
			}


			Console.WriteLine( "Writing translated file..." );
			System.IO.File.WriteAllBytes( "STRING_DIC_translated.SO", TSS.Serialize() );


			Console.WriteLine( "Writing text dump..." );
			System.IO.File.WriteAllBytes( "STRING_DIC_translated_export.txt", TSS.ExportText() );


			Console.WriteLine( "Done!" );
			return;
		}
	}
}
