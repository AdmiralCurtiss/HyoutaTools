using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Trophy.Viewer {
	class Program {
		public static int Execute( string[] args ) {
			if ( args.Length < 1 ) {
				Console.WriteLine( "Usage: TrophyViewer [folder]" );
				return -1;
			}

			String path = args[0];
			GameFolder GF = new GameFolder( path );
			GameSelectForm GameForm = new GameSelectForm( GF );
			System.Windows.Forms.Application.Run( GameForm );

			return 0;
		}
	}
}
