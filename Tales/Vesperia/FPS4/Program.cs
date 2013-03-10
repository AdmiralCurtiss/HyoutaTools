using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.FPS4 {
	public class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "usage: fps4hack something.dat.dec" );
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

			uint filecount = Util.SwapEndian( BitConverter.ToUInt32( b, 0x04 ) );
			uint headersize = Util.SwapEndian( BitConverter.ToUInt32( b, 0x08 ) );
			uint entrysize = Util.SwapEndian( BitConverter.ToUInt16( b, 0x10 ) );

			string dirname = file + ".ext";
			System.IO.Directory.CreateDirectory( dirname );

			for ( uint i = 0; i < filecount; ++i ) {
				uint fileloc = Util.SwapEndian( BitConverter.ToUInt32( b, (int)( headersize + ( i * entrysize ) + 0x00 ) ) );
				uint fileend = fileloc + Util.SwapEndian( BitConverter.ToUInt32( b, (int)( headersize + ( i * entrysize ) + 0x04 ) ) );
				uint filesize = Util.SwapEndian( BitConverter.ToUInt32( b, (int)( headersize + ( i * entrysize ) + 0x08 ) ) );
				string filename = Util.GetTextAscii( b, (int)( headersize + ( i * entrysize ) + 0x0C ) );
				if ( fileloc == 0xFFFFFFFF || fileend < fileloc || filesize == 0 ) continue;

				//string description = Util.GetText((int)(Util.SwapEndian(BitConverter.ToUInt32(b, (int)(headersize + (i * entrysize) + 0x2C)))), b);


				if ( filename == "DMY" || filename == "SCR" || filename == "MDL" || filename == "TEX" ) {
					filename = i.ToString( "D4" );
				}


				FileStream outfile;
				try {
					outfile = new FileStream( dirname + '/' + filename, FileMode.Create );
				} catch ( Exception ) {
					outfile = new FileStream( dirname + '/' + i.ToString( "D4" ), FileMode.Create );
				}

				try {
					outfile.Write( b, (int)fileloc, (int)filesize );
				} catch ( Exception ) {
					try {
						outfile.Write( b, (int)fileloc, (int)( fileend - fileloc ) );
					} catch ( Exception ) {
						Console.WriteLine( "ERROR on file " + outfile.Name );
						outfile.Close();
						return -1;
					}
				}
				outfile.Close();
			}

			return 0;
		}
	}
}
