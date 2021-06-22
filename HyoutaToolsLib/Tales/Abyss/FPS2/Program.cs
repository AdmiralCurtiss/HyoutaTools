using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Abyss.FPS2 {
	public class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "usage: fps2 something.dat.dec" );
				return -1;
			}

			string file = args[0];

			byte[] b;
			try {
				b = System.IO.File.ReadAllBytes( file );
			} catch ( Exception ) {
				Console.WriteLine( "ERROR: can't open " + file );
				return -1;
			}

			uint filecount = ( BitConverter.ToUInt32( b, 0x04 ) );
			uint headersize = 0x8;
			uint entrysize = 0xC;

			string dirname = file + ".ext";
			System.IO.Directory.CreateDirectory( dirname );

			for ( uint i = 0; i < filecount; ++i ) {
				uint fileloc = ( BitConverter.ToUInt32( b, (int)( headersize + ( i * entrysize ) + 0x00 ) ) );
				uint filesize = fileloc + ( BitConverter.ToUInt32( b, (int)( headersize + ( i * entrysize ) + 0x04 ) ) );
				uint extensionAsInt = ( BitConverter.ToUInt32( b, (int)( headersize + ( i * entrysize ) + 0x08 ) ) );
				if ( fileloc == 0 || filesize == 0 || extensionAsInt == 0 ) continue;

				string extension =
					Encoding.ASCII.GetString( b, (int)( headersize + ( i * entrysize ) + 0x08 ), 4 ).Trim( '\0' );

				FileStream outfile;
				try {
					outfile = new FileStream( dirname + '/' + i.ToString( "D4" ) + "." + extension, FileMode.Create );
				} catch ( Exception ) {
					outfile = new FileStream( dirname + '/' + i.ToString( "D4" ), FileMode.Create );
				}

				try {
					outfile.Write( b, (int)fileloc, (int)( filesize - fileloc ) );
				} catch ( Exception ) {
					Console.WriteLine( "ERROR on file " + outfile.Name );
					outfile.Close();
					return -1;
				}
				outfile.Close();
			}

			return 0;
		}
	}
}
