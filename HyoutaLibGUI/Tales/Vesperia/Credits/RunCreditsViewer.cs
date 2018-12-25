using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HyoutaTools;
using HyoutaTools.Tales.Vesperia.TSS;

namespace HyoutaLibGUI.Tales.Vesperia.Credits {
	public class RunCreditsViewer {
		public static int Execute( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: credits.dat STRING_DIC.SO" );
				return -1;
			}

			Console.WriteLine( "Opening STRING_DIC.SO..." );
			TSSFile TSS;
			try {
				TSS = new TSSFile( args[1], Util.GameTextEncoding.ShiftJIS );
			} catch ( System.IO.FileNotFoundException ) {
				Console.WriteLine( "Could not open STRING_DIC.SO, exiting." );
				return -1;
			}

			CreditsForm itemForm = new CreditsForm( args[0], TSS );
			itemForm.Show();
			return 0;
		}
	}
}
