using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using HyoutaTools.Trophy;

namespace HyoutaTools.GraceNote.Trophy.TropSfmImport {
	class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 4 ) {
				Console.WriteLine( "Usage: TropSFM_GraceNote TROP.SFM TROPCONF.SFM NewDBFile GracesJapanese" );
				return -1;
			}

			String Filename = args[0];
			String FilenameTropConf = args[1];
			String NewDB = args[2];
			String GracesDB = args[3];

			TrophyConfNode TROPSFM = TrophyConfNode.ReadTropSfmWithTropConf( Filename, FilenameTropConf );
			GraceNoteUtil.GenerateEmptyDatabase( NewDB );

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>( TROPSFM.Trophies.Values.Count * 2 );
			foreach ( TrophyNode Trophy in TROPSFM.Trophies.Values ) {
				string Ident = "<ID: " + Trophy.ID + "> "
							 + "<Hidden: " + Trophy.Hidden.ToString() + "> "
							 + "<TType: " + Trophy.TType + "> "
							 + "<PID: " + Trophy.PID + ">";
				Entries.Add( new GraceNoteDatabaseEntry( Trophy.Name, Trophy.Name, "", 0, 0, Ident, 0 ) );
				Entries.Add( new GraceNoteDatabaseEntry( Trophy.Detail, Trophy.Detail, "", 0, 1, Ident, 0 ) );
			}

			GraceNoteDatabaseEntry.InsertSQL( Entries.ToArray(), "Data Source=" + NewDB, "Data Source=" + GracesDB );

			//System.IO.File.WriteAllBytes(@"C:\TROPHY\newTrophyConf.trp", Encoding.UTF8.GetBytes(TROPSFM.ExportTropSFM(true)));
			//System.IO.File.WriteAllBytes(@"C:\TROPHY\newTrophy.trp", Encoding.UTF8.GetBytes(TROPSFM.ExportTropSFM(false)));

			return 0;
		}
	}
}
