using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HyoutaTools;
using HyoutaTools.Tales.Vesperia;
using HyoutaTools.Tales.Vesperia.TSS;

namespace HyoutaLibGUI.Tales.Vesperia.ItemDat {
	class RunItemViewer {
		public static int Execute( List<string> args ) {
			if ( args.Count < 7 ) {
				Console.WriteLine( "Usage: [360/PS3] ITEM.DAT STRING_DIC.SO T8BTSK T8BTEMST COOKDAT WRLDDAT" );
				return -1;
			}

			GameVersion? version = null;
			switch ( args[0].ToUpperInvariant() ) {
				case "360":
					version = GameVersion.X360;
					break;
				case "PS3":
					version = GameVersion.PS3;
					break;
			}

			if ( version == null ) {
				Console.WriteLine( "First parameter must indicate game version!" );
				return -1;
			}

			HyoutaTools.Tales.Vesperia.ItemDat.ItemDat items = new HyoutaTools.Tales.Vesperia.ItemDat.ItemDat( args[1] );

			TSSFile TSS;
			try {
				TSS = new TSSFile( args[2], Util.GameTextEncoding.ShiftJIS );
			} catch ( System.IO.FileNotFoundException ) {
				Console.WriteLine( "Could not open STRING_DIC.SO, exiting." );
				return -1;
			}

			HyoutaTools.Tales.Vesperia.T8BTSK.T8BTSK skills = new HyoutaTools.Tales.Vesperia.T8BTSK.T8BTSK( args[3] );
			HyoutaTools.Tales.Vesperia.T8BTEMST.T8BTEMST enemies = new HyoutaTools.Tales.Vesperia.T8BTEMST.T8BTEMST( args[4] );
			HyoutaTools.Tales.Vesperia.COOKDAT.COOKDAT cookdat = new HyoutaTools.Tales.Vesperia.COOKDAT.COOKDAT( args[5] );
			HyoutaTools.Tales.Vesperia.WRLDDAT.WRLDDAT locations = new HyoutaTools.Tales.Vesperia.WRLDDAT.WRLDDAT( args[6] );

			Console.WriteLine( "Initializing GUI..." );
			ItemForm itemForm = new ItemForm( version.Value, items, TSS, skills, enemies, cookdat, locations );
			itemForm.Show();
			return 0;
		}
	}
}
