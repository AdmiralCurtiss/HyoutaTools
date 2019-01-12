using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.TO8CHTX;

namespace HyoutaTools.GraceNote.Vesperia.TO8CHTXImport {
	class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 3 ) {
				Console.WriteLine( "Usage: TO8CHTX_GraceNote ChatFilename NewDBFilename GracesJapanese" );
				return -1;
			}

			String Filename = args[0];
			String NewDB = args[1];
			String GracesDB = args[2];

			ChatFile c = new ChatFile( Filename, Util.Endianness.BigEndian, Util.GameTextEncoding.ShiftJIS, Util.Bitness.B32, 2 );

			GraceNoteUtil.GenerateEmptyDatabase( NewDB );

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>( c.Lines.Length * 2 );
			foreach ( ChatFileLine Line in c.Lines ) {

				String EnglishText;
				int EnglishStatus;
				if ( Line.SENG == "Dummy" || Line.SENG == "" ) {
					EnglishText = Line.SJPN;
					EnglishStatus = 0;
				} else {
					EnglishText = Line.SENG;
					EnglishStatus = 1;
				}

				Entries.Add( new GraceNoteDatabaseEntry( Line.SName, Line.SName, "", 1, Line.Location, "", 0 ) );
				Entries.Add( new GraceNoteDatabaseEntry( Line.SJPN, EnglishText, "", EnglishStatus, Line.Location + 4, "", 0 ) );
			}

			GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + NewDB, "Data Source=" + GracesDB );
			return 0;
		}
	}
}
