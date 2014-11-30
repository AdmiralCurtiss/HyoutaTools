using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Gust.g1t {
	class DDSConverter {
		public static int Execute( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: infile.g1t" );
				return -1;
			}

			string infile = args[0];
			string outdir = infile + ".ext";

			var g1t = new g1t( infile );
			System.IO.Directory.CreateDirectory( outdir );

			for ( int i = 0; i < g1t.Textures.Count; ++i ) {
				var tex = g1t.Textures[i];
				string path = System.IO.Path.Combine( outdir, i.ToString( "D4" ) + ".dds" );

				using ( var fs = new System.IO.FileStream( path, System.IO.FileMode.Create ) ) {
					fs.Write( Textures.DDSHeader.Generate( tex.Width, tex.Height, tex.Mipmaps, tex.Format ) );
					fs.Write( tex.Data );
					fs.Close();
				}
			}

			return 0;
		}
	}
}
