using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.Tales.Vesperia.TownMap.Viewer {
	static class Program {
		static void Execute() {
			byte[] File = System.IO.File.ReadAllBytes( Util.Path );

			TownMapTable t = new TownMapTable( File );

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			Application.Run( new TownMapDisplay( t ) );
		}
	}
}
