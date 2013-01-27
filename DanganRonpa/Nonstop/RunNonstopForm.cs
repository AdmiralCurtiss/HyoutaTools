using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.DanganRonpa.Nonstop {
	class RunNonstopForm {
		public static int Execute() {
			Nonstop items = new Nonstop( @"d:\_svn\GraceNote\GraceNote\DanganRonpaBestOfRebuild\umdimage.dat.ex\0052_nonstop_02_002.dat" );
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			NonstopForm itemForm = new NonstopForm( items );
			Application.Run( itemForm );
			return 0;
		}
	}
}
