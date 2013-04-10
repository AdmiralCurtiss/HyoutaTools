using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.Other.N64.OoTSaveEditor {
	class Program {
		public static int Execute( List<string> args ) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			Application.Run( new OoTSaveEditForm() );
			return 0;
		}
	}
}
