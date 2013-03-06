using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.DanganRonpa.Nonstop {
	class RunNonstopForm {
		public static int Execute( List<string> args ) {
			Nonstop items = new Nonstop( args[0] );
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			NonstopForm itemForm = new NonstopForm( items );
			Application.Run( itemForm );
			return 0;
		}
	}
}
