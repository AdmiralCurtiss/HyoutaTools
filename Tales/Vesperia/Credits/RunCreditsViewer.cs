using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HyoutaTools.Tales.Vesperia.TSS;

namespace HyoutaTools.Tales.Vesperia.Credits {
	class RunCreditsViewer {
		public static int Execute( string[] args ) {

			CreditsInfo items = new CreditsInfo( @"d:\_svn\Tales of Vesperia Translation\Repack\GNDB_ToV_Repack\assets\E_A102_CREDIT-2-mod.DAT" );

			Console.WriteLine( "Opening STRING_DIC.SO..." );
			TSSFile TSS;
			try {
				TSS = new TSSFile( System.IO.File.ReadAllBytes( @"d:\Dropbox\ToV\360\string.svo.ext\STRING_DIC.SO" ) );
			} catch ( System.IO.FileNotFoundException ) {
				Console.WriteLine( "Could not open STRING_DIC.SO, exiting." );
				return -1;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			//ItemForm itemForm = new ItemForm( items, TSS );
			//Application.Run( itemForm );
			return 0;
		}
	}
}
