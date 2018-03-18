using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using HyoutaTools.Other.LuxPain;

namespace HyoutaTools.GraceNote.LuxPainEvtExport {
	class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 3 ) {
				Console.WriteLine( "Usage: GN_LPE Infile Outfile DB" );
				return -1;
			}

			String InFilename = args[0];
			String OutFilename = args[1];
			String DB = args[2];

			LuxPainEvt Evt;
			byte[] EvtFile;
			try {
				EvtFile = System.IO.File.ReadAllBytes( InFilename );
				Evt = new LuxPainEvt( EvtFile );
				Evt.GetSQL( "Data Source=" + DB );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				Console.WriteLine( "Failed loading menu file!" );
				return -1;
			}

			LuxPainEvt Evt2 = new LuxPainEvt( EvtFile );
			Evt.FormatTextForGameInserting();
			byte[] newtextblock = Evt.CreateTextBlock( 256 * 256 * 2 );

			List<byte> newEvtFile = new List<byte>( (int)Evt.Header.TextOffsetsLocation + newtextblock.Length );

			for ( uint i = 0; i < Evt.Header.TextOffsetsLocation; ++i ) {
				newEvtFile.Add( EvtFile[i] );
			}
			newEvtFile.AddRange( newtextblock );

			// replicating the weird behavoir that if the last entry ends with 0x8144, there's an additional 0x0000 after it, sometimes.
			// No idea if that's actually relevant but better be safe than sorry!
			if ( newEvtFile[newEvtFile.Count - 4] == 0x81 && newEvtFile[newEvtFile.Count - 3] == 0x44 ) {
				newEvtFile.Add( 0x00 );
				newEvtFile.Add( 0x00 );
			}

			System.IO.File.WriteAllBytes( OutFilename, newEvtFile.ToArray() );

			return 0;
		}
	}
}
