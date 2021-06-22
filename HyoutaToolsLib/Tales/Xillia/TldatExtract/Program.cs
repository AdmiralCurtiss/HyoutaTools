using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Xillia.TldatExtract {
	class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 3 ) {
				Console.WriteLine( "Usage: TLDAT TOFHDB ExtractFolder" );
				return -1;
			}

			String TLDAT = args[0];
			String TOFHDB = args[1];
			String ExtractFolder = args[2];
			if ( !( ExtractFolder.EndsWith( "/" ) || ExtractFolder.EndsWith( "\\" ) ) ) {
				ExtractFolder = ExtractFolder + '/';
			}

			Directory.CreateDirectory( ExtractFolder );

			TOFHDBheader header = new TOFHDBheader( TOFHDB );

			UInt64 biggestFile = header.BiggestFile();
			Byte[] buffer = new byte[biggestFile];
			FileStream fs = File.Open( TLDAT, FileMode.Open, FileAccess.Read );

			int counter = 1;

			foreach ( TOFHDBfile e in header.FileArray ) {
				fs.Position = (long)e.Offset;
				fs.Read( buffer, 0, (int)e.CompressedSize );
				String ExtensionFolder = ExtractFolder + e.Extension + '/';
				Directory.CreateDirectory( ExtensionFolder );
				String Path = ExtensionFolder + counter.ToString( "D8" ) + '.' + e.Extension;

				FileStream os = File.OpenWrite( Path );
				os.Write( buffer, 0, (int)e.CompressedSize );
				os.Close();

				counter++;
			}

			fs.Close();

			return 0;
		}
	}
}
