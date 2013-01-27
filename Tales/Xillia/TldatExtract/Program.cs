using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Xillia.TldatExtract {
	class Program {
		public static int Execute( string[] args ) {
			if ( args.Length != 4 ) {
				Console.WriteLine( "Usage: TLDATextract TLDAT TOFHDB ExtractFolder" );
				return -1;
			}

			String TLDAT = @"c:\Users\Georg\Downloads\XilliaStuff\USRDIR\TLFILE.TLDAT";
			String TOFHDB = @"c:\Users\Georg\Downloads\XilliaStuff\USRDIR\FILEHEADER.TOFHDB";
			String ExtractFolder = @"c:\Users\Georg\Downloads\XilliaStuff\USRDIR\ex";
			if ( !( ExtractFolder.EndsWith( "/" ) || ExtractFolder.EndsWith( "\\" ) ) ) {
				ExtractFolder = ExtractFolder + '/';
			}

			Directory.CreateDirectory( ExtractFolder );

			TOFHDBheader header = new TOFHDBheader( TOFHDB );

			UInt64 biggestFile = header.BiggestFile();
			Byte[] buffer = new byte[biggestFile];
			FileStream fs = File.Open( TLDAT, FileMode.Open, FileAccess.Read );

			int counter = 1;

			foreach ( TOFHDBEntry e in header.EntryList ) {
				fs.Position = (long)e.Offset;
				fs.Read( buffer, 0, (int)e.CompressedSize );
				String Path = ExtractFolder + counter.ToString( "D8" ) + '.' + e.Extension;

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
