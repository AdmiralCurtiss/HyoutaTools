using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Trophy.Viewer {
	public class GameFolder {
		public List<TrophyConfNode> TrophyLists;

		public GameFolder( String Folder ) {
			String[] Games = System.IO.Directory.GetDirectories( Folder );

			TrophyLists = new List<TrophyConfNode>();

			foreach ( String Game in Games ) {
				String TropConf = Game + "/TROPCONF.SFM";
				String TropUsr = Game + "/TROPUSR.DAT";
				if ( System.IO.File.Exists( TropConf ) && System.IO.File.Exists( TropUsr ) ) {
					TrophyConfNode TROPSFM = TrophyConfNode.ReadTropSfmWithFolder( Game, "TROPCONF.SFM" );
					TrophyLists.Add( TROPSFM );
				}
			}

			TrophyLists.TrimExcess();

			return;
		}
	}
}
