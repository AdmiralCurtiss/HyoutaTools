using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Other.PSP.GIM.GimToPng {
	public class GimToPng {
		public static int Execute( List<string> args ) {
			if ( args.Count == 0 ) {
				Console.WriteLine( "Usage: GimToPng file.gim" );
				return -1;
			}

			string filename = args[0];

			GIM gim = new GIM( filename );
			int filenum = 0;
			foreach ( Bitmap bmp in gim.ConvertToBitmaps() ) {
				bmp.Save( filename + "." + filenum + ".png" );
			}

			return 0;
		}
	}
}
