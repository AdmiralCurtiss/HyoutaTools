using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.GraceNote.Narisokonai {
	class scrImport {
		public static int Import( List<string> args ) {
			if ( args.Count < 3 ) {
				Console.WriteLine( "Usage: scr Database GracesJapanese" );
				return -1;
			}

			string InFile = args[0];
			string OutDatabase = args[1];
			string GracesJapanese = args[2];
			var bscrFile = new HyoutaTools.Narisokonai.scr( System.IO.File.ReadAllBytes( InFile ) );
			GraceNoteUtil.GenerateEmptyDatabase( OutDatabase );
			if ( !System.IO.File.Exists( GracesJapanese ) ) { GraceNoteUtil.GenerateEmptyGracesJapanese( GracesJapanese ); }

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>();
			foreach ( var sec in bscrFile.Sections ) {
				if ( sec.Elements == null ) continue; // dummy section
				GraceNoteDatabaseEntry gn;

				foreach ( var e in sec.Elements ) {
					switch ( e.Type ) {
						case HyoutaTools.Narisokonai.scrElement.scrElementType.Code:
							StringBuilder sb = new StringBuilder();
							for ( int i = 0; i < e.Code.Length; ++i ) {
								sb.Append( '<' ).Append( e.Code[i].ToString( "X2" ) ).Append( "> " );
							}
							gn = new GraceNoteDatabaseEntry( sb.ToString(), sb.ToString(), "", -1, (int)sec.Location, e.Type.ToString(), sec.PointerIndex );
							break;
						case HyoutaTools.Narisokonai.scrElement.scrElementType.Text:
							gn = new GraceNoteDatabaseEntry( e.Text, e.Text, "", 0, (int)sec.Location, e.Type.ToString(), sec.PointerIndex );
							break;
						default:
							throw new Exception("scrImport: Unknown Element Type!");
					}
					Entries.Add( gn );
				}
			}

			GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + OutDatabase, "Data Source=" + GracesJapanese );

			return 0;
		}
	}
}
