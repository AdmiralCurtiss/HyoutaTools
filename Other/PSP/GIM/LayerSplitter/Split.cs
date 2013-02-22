using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PSP.GIM.LayerSplitter {
	class Splitter {
		public static int Split( string[] args ) {
			string Filename = @"e:\__\test-mine.gim";
			GIM[] gims = new GIM[3];
			gims[0] = new GIM( Filename ); ;
			gims[1] = new GIM( Filename ); ;
			gims[2] = new GIM( Filename ); ;

			for ( int i = 0; i < gims.Length; ++i ) {
				GIM gim = gims[i];
				gim.ReduceToOneImage( i );
				System.IO.File.WriteAllBytes(
					System.IO.Path.GetDirectoryName(Filename) + System.IO.Path.DirectorySeparatorChar + System.IO.Path.GetFileNameWithoutExtension(Filename) + i.ToString() + System.IO.Path.GetExtension(Filename),
					gim.Serialize() );
			}

			return 0;
		}
	}
}
