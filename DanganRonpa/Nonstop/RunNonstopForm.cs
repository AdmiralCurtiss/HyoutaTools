using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.DanganRonpa.Nonstop {
	class RunNonstopForm {
		public static int Execute() {
			Nonstop items = new Nonstop(
@"e:\____DANGAN_RONPA_AUDIO_STUFF\__\nonstop\0112_nonstop_06_025.dat" );
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			NonstopForm itemForm = new NonstopForm( items );
			Application.Run( itemForm );
			return 0;
		}
	}
}
