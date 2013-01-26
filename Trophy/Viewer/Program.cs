using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Trophy.Viewer {
	class Program {
		static void Execute( string[] args ) {
			if ( args.Length > 1 ) {
				Console.WriteLine( "Usage: TrophyViewer [folder]" );
				Console.WriteLine( "       TrophyViewer [TROPUSR.DAT]" );
				return;
			}

			//String Folder = @"c:\Dokumente und Einstellungen\GStrof\Eigene Dateien\Visual Studio 2008\Projects\trophy\NPWR00642_00\"; // Tales of Vesperia
			//String Folder = @"c:\Dokumente und Einstellungen\GStrof\Eigene Dateien\Visual Studio 2008\Projects\trophy\NPWR01678_00\"; // Ar tonelico 3
			//String Folder = @"c:\Dokumente und Einstellungen\GStrof\Eigene Dateien\Visual Studio 2008\Projects\trophy\NPWR00234_00\"; // Burnout Paradise
			//String Folder = @"c:\Dokumente und Einstellungen\GStrof\Eigene Dateien\Visual Studio 2008\Projects\trophy\NPWR01097_00\"; // 3D Dot Game
			//String Folder = @"c:\Dokumente und Einstellungen\GStrof\Eigene Dateien\Visual Studio 2008\Projects\trophy\NPWR01103_00\"; // Resonance of Fate
			//String Folder = @"c:\Dokumente und Einstellungen\GStrof\Eigene Dateien\Visual Studio 2008\Projects\trophy\"; // parent folder
			//String Folder = @"c:\Dokumente und Einstellungen\GStrof\Eigene Dateien\Visual Studio 2008\Projects\trophy\NPWR00187_00"; // Wipeout HD
			//String Folder = @"c:\Users\Georg\Documents\___PS3\00000007\trophy\NPWR00774_00\"; // FF13
			//String Folder = @"c:\Users\Georg\Documents\___PS3\00000007\trophy\NPWR01678_00\"; // AT3
			String Folder = @"c:\Users\Georg\Documents\___PS3\00000007\trophy\";
			/*
			TrophyConfNode TROPSFM = new TrophyConfNode(
				"4c39b98c0100000000000000b4bb7de046f205e74b24eabf731497511b639be57ca1ae0a3efd0519adba789cb32b91d97d3e11a7bf8302544fab919062647245e95796f11dd01c8db63f391756e0a1ddf6ddc7d64b0b8c86ec962e4ea2ef3b4caf82d178afb91a0a6fe3a082299c4a2fb1e3764ac6d42a7a11f52980a2e5149e9a8256fbc9dc438a4490ddace9a96c4d23f4652bbbb1cee1819a26e390724d7c",
				"4c39b98c010000000000000074abfed402a679fdff35652737f80e2368488f2f524b8721ab07366434ba133c4b545d55130ce41e31c07f8dc7c166cef530cd5f340c489b55626fd20a5d6b4b3851a3cb1ba04fc353619307033fc3b208e1e08d1de80c2c5e4be2ff131f22ed1f31e22f93697e9bd91b1e1e5285a1e2f50f8c324a03e515e006a85322f19c246d538c08177fcd9d302e34bb3b250ab40b704c61",
				"1.0", "NPWR00642_00", "01.00", "0", "default", "Tales of Vesperia", "Tales of Vesperia Trophy Set"
				);
			TROPSFM.Trophies = EnglishTrophies;
			System.IO.File.WriteAllBytes(@"newTrophyConf.SFM", Encoding.UTF8.GetBytes(TROPSFM.ExportTropSFM(true)));
			System.IO.File.WriteAllBytes(@"newTrophy.SFM", Encoding.UTF8.GetBytes(TROPSFM.ExportTropSFM(false)));
			*/


			//*
			GameFolder GF = new GameFolder( Folder );
			GameSelectForm GameForm = new GameSelectForm( GF );
			System.Windows.Forms.Application.Run( GameForm );

			/*
			TrophyConfNode TROPSFM = Util.ReadTropSfm(Folder, "TROPCONF.SFM");
			TROPSFM.SortBy(TropUsrSingleTrophy.SortByTimestamp, false);
			TrophyForm TForm = new TrophyForm(TROPSFM);
			System.Windows.Forms.Application.Run(TForm);
			*/


		}
	}
}
