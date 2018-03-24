using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.Texture {
	public class Decode {
		public static int Execute( List<string> args ) {
			if ( args.Count != 2 ) {
				Console.WriteLine( "Usage: texture.txm texture.txv" );
				return -1;
			}

			String outdir = args[0] + ".ext";
			return Extract( args[0], args[1], outdir ) ? 0 : -1;
		}

		public static bool Extract( string txmpath, string txvpath, string outdir ) {
			TXM txm = new TXM( txmpath );
			TXV txv = new TXV( txm, txvpath );
			Directory.CreateDirectory( outdir );

			int counter = 0;
			foreach ( TXVSingle ts in txv.textures ) {
				foreach ( var tex in ts.Decode() ) {
					using ( var fs = new FileStream( Path.Combine( outdir, counter.ToString( "D4" ) + "_" + tex.name ), FileMode.Create ) ) {
						tex.data.Position = 0;
						Util.CopyStream( tex.data, fs, tex.data.Length );
					}
					++counter;
				}
			}

			return true;
		}
	}
}
