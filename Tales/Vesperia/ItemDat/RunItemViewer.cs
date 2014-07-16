using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HyoutaTools.Tales.Vesperia.TSS;

namespace HyoutaTools.Tales.Vesperia.ItemDat {
	class RunItemViewer {
		public static int Execute( List<string> args ) {
			if ( args.Count < 4 ) {
				Console.WriteLine( "Usage: ITEM.DAT STRING_DIC.SO T8BTSK T8BTEMST COOKDAT" );
				return -1;
			}

			ItemDat items = new ItemDat( args[0] );
			T8BTSK.T8BTSK skills = new T8BTSK.T8BTSK( args[2] );
			T8BTEMST.T8BTEMST enemies = new T8BTEMST.T8BTEMST( args[3] );
			COOKDAT.COOKDAT cookdat = new COOKDAT.COOKDAT( args[4] );

			Console.WriteLine( "Opening STRING_DIC.SO..." );
			TSSFile TSS;
			try {
				TSS = new TSSFile( System.IO.File.ReadAllBytes( args[1] ) );
			} catch ( System.IO.FileNotFoundException ) {
				Console.WriteLine( "Could not open STRING_DIC.SO, exiting." );
				return -1;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			ItemForm itemForm = new ItemForm( items, TSS, skills, enemies, cookdat );
			Application.Run( itemForm );
			return 0;
		}
	}
}
