using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace HyoutaTools.Other.GZip {
	class GZipHandler {
		public static void Extract( FileStream fs, string outpath ) {
			fs.Position = 0;
			var outfile = new FileStream( outpath, System.IO.FileMode.Create );
			using ( var decompressed = new GZipStream( fs, CompressionMode.Decompress ) ) {
				byte[] buffer = new byte[4096];
				int numRead;
				while ( ( numRead = decompressed.Read( buffer, 0, buffer.Length ) ) != 0 ) {
					outfile.Write( buffer, 0, numRead );
				}
			}
			outfile.Close();
		}
	}
}
