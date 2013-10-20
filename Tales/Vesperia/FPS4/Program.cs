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

			FileStream infile;
			try {
				infile = new FileStream( file, FileMode.Open );
			} catch ( Exception ) {
				Console.WriteLine( "ERROR: can't open " + file );
				return -1;
			}

			infile.Seek( 0x04, SeekOrigin.Begin );
			uint filecount = infile.ReadUInt32().SwapEndian();
			uint headersize = infile.ReadUInt32().SwapEndian();
			uint unknown = infile.ReadUInt32().SwapEndian();
			ushort entrysize = infile.ReadUInt16().SwapEndian();

			string dirname = file + ".ext";
			System.IO.Directory.CreateDirectory( dirname );

			for ( uint i = 0; i < filecount; ++i ) {
				infile.Seek( headersize + ( i * entrysize ), SeekOrigin.Begin );

				uint fileloc = infile.ReadUInt32().SwapEndian();
				uint fileend = fileloc + infile.ReadUInt32().SwapEndian();
				uint filesize = infile.ReadUInt32().SwapEndian();
				string filename = infile.ReadAsciiNullterm();
				if ( fileloc == 0xFFFFFFFF || fileend < fileloc || filesize == 0 ) continue;

				//string description = Util.GetText((int)(Util.SwapEndian(BitConverter.ToUInt32(b, (int)(headersize + (i * entrysize) + 0x2C)))), b);


				if ( filename == "DMY" || filename == "SCR" || filename == "MDL" || filename == "TEX" ) {
					filename = i.ToString( "D4" ) + '.' + filename;
				}


				FileStream outfile;
				try {
					outfile = new FileStream( dirname + '/' + filename, FileMode.Create );
				} catch ( Exception ) {
					outfile = new FileStream( dirname + '/' + i.ToString( "D4" ), FileMode.Create );
				}

				Console.WriteLine( "Extracting: " + System.IO.Path.GetFileName( outfile.Name ) );

				try {
					infile.Seek( fileloc, SeekOrigin.Begin );
					Util.CopyStream( infile, outfile, (int)filesize );
				} catch ( Exception ) {
					try {
						infile.Seek( fileloc, SeekOrigin.Begin );
						Util.CopyStream( infile, outfile, (int)( fileend - fileloc ) );
					} catch ( Exception ) {
						Console.WriteLine( "ERROR on file " + outfile.Name );
						outfile.Close();
						return -1;
					}
				}
				outfile.Close();
			}
			infile.Close();

			return 0;
		}
	}
}
