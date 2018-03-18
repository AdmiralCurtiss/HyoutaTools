using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace HyoutaTools.Generic.ByteHotfix {
	class ByteHotfix {
		public static int Execute( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: ByteHotfix [filename] [location-byte] [location-byte] etc." );
				Console.WriteLine( "example: ByteHotfix file.ext 77D0-20 77D1-45 77D2-46 77D3-FF" );
				return -1;
			}

			try {
				string inFilename = args[0];

				using ( var fi = new System.IO.FileStream( inFilename, System.IO.FileMode.Open ) ) {
					for ( int i = 1; i < args.Count; i++ ) {
						String[] v = args[i].Split( new char[] { '-' } );
						int location = int.Parse( v[0], NumberStyles.AllowHexSpecifier );
						byte value = byte.Parse( v[1], NumberStyles.AllowHexSpecifier );
						fi.Position = location;
						fi.WriteByte( value );
					}
					fi.Close();
				}

				return 0;

			} catch ( Exception ex ) {
				Console.WriteLine( "Exception: " + ex.Message );
				return -1;
			}
		}
	}
}
