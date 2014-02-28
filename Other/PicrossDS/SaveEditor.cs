using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.Other.PicrossDS {
	class SaveEditor {
		public static int Execute( List<string> args ) {

			new SaveFile( args[0] ).RecalculateChecksum();

			
			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault( false );
			//Application.Run( new OoTSaveEditForm() );
			return 0;
		}
	}
}
