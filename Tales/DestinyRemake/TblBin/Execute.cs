using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.DestinyRemake.TblBin {
	class Execute {
		public static int Extract( List<string> args ) {

			string tblpath = args[0];
			string binpath = args[1];

			TBL tbl = new TBL();
			tbl.Load( System.IO.File.ReadAllBytes( tblpath ) );
			System.IO.FileStream fs = new System.IO.FileStream( binpath, System.IO.FileMode.Open );
			tbl.Extract( fs, binpath + ".ex" );

			return 0;
		}
	}
}
