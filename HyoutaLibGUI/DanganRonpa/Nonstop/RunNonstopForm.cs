using HyoutaTools.DanganRonpa.Nonstop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaLibGUI.DanganRonpa.Nonstop {
	class RunNonstopForm {
		public static int Execute( List<string> args ) {
			if (args.Count < 1) {
				System.Windows.Forms.MessageBox.Show("Requires 1 argument, path to nonstop data file.");
				return -1;
			}

			NonstopFile items = new NonstopFile( args[0] );
			NonstopForm itemForm = new NonstopForm( items );
			itemForm.Show();
			return 0;
		}
	}
}
