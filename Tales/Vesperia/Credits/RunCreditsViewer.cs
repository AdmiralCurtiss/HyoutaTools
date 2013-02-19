using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HyoutaTools.Tales.Vesperia.TSS;

namespace HyoutaTools.Tales.Vesperia.Credits {
	public class RunCreditsViewer {
		public static int Execute( string[] args ) {


			Console.WriteLine( "Opening STRING_DIC.SO..." );
			TSSFile TSS;
			try {
				TSS = new TSSFile( System.IO.File.ReadAllBytes( @"e:\__\string.svo.new.ext\STRING_DIC.SO" ) );
			} catch ( System.IO.FileNotFoundException ) {
				Console.WriteLine( "Could not open STRING_DIC.SO, exiting." );
				return -1;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			CreditsForm itemForm = new CreditsForm( args[0], TSS );
			Application.Run( itemForm );
			return 0;
		}
	}
}
