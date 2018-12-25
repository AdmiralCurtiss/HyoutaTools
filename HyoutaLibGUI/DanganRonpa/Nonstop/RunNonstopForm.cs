using HyoutaTools.DanganRonpa.Nonstop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaLibGUI.DanganRonpa.Nonstop {
	class RunNonstopForm {
		public static int Execute( List<string> args ) {
			NonstopFile items = new NonstopFile( args[0] );
			NonstopForm itemForm = new NonstopForm( items );
			itemForm.Show();
			return 0;
		}
	}
}
