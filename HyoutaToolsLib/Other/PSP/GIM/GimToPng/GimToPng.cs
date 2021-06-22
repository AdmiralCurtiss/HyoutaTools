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
			List<string> convertedFilenames = ConvertGimFileToPngFiles( filename );
			return ( convertedFilenames != null && convertedFilenames.Count > 0 ) ? 0 : -1;
		}

		public static List<string> ConvertGimFileToPngFiles( string filename ) {
			GIM gim = new GIM( filename );
			int filenum = 0;
			List<string> names = new List<string>();
			foreach ( Bitmap bmp in gim.ConvertToBitmaps() ) {
				string newname = filename + "." + filenum + ".png";
				bmp.Save( newname );
				names.Add( newname );
			}
			return names;
		}
	}
}
