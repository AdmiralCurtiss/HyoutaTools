using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaLibGUI.Other.PicrossDS {
	class SaveEditor {
		public static int Execute( List<string> args ) {
			var form = new PuzzleEditorForm();
			if ( !form.IsDisposed ) {
				form.Show();
				return 0;
			}
			return -1;
		}
	}
}
