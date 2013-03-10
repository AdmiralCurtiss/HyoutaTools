using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Abyss.FPS3 {
	public class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "usage: fps3 something.dat.dec" );
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

			uint filecount = BitConverter.ToUInt32( b, 0x04 );
			uint headersize = BitConverter.ToUInt32( b, 0x08 );
			uint filesstartloc = BitConverter.ToUInt32( b, 0x0C );
			ushort entrysize = BitConverter.ToUInt16( b, 0x10 );
			ushort metaBitmask = BitConverter.ToUInt16( b, 0x12 );

			bool hasFilestart = ( metaBitmask & 0x0001 ) == 0x0001;
			bool hasFileend = ( metaBitmask & 0x0002 ) == 0x0002;
			bool hasUnknown0004 = ( metaBitmask & 0x0004 ) == 0x0004;
			bool hasFilename = ( metaBitmask & 0x0008 ) == 0x0008;

			bool hasExtension = ( metaBitmask & 0x0010 ) == 0x0010;
			bool hasUnknown0020 = ( metaBitmask & 0x0020 ) == 0x0020;
			bool hasUnknown0040 = ( metaBitmask & 0x0040 ) == 0x0040;
			bool hasUnknown0080 = ( metaBitmask & 0x0080 ) == 0x0080;

			bool hasUnknown0100 = ( metaBitmask & 0x0100 ) == 0x0100;
			bool hasUnknown0200 = ( metaBitmask & 0x0200 ) == 0x0200;
			bool hasUnknown0400 = ( metaBitmask & 0x0400 ) == 0x0400;
			bool hasUnknown0800 = ( metaBitmask & 0x0800 ) == 0x0800;

			bool hasUnknown1000 = ( metaBitmask & 0x1000 ) == 0x1000;
			bool hasUnknown2000 = ( metaBitmask & 0x2000 ) == 0x2000;
			bool hasUnknown4000 = ( metaBitmask & 0x4000 ) == 0x4000;
			bool hasUnknown8000 = ( metaBitmask & 0x8000 ) == 0x8000;

			uint Unknown1 = BitConverter.ToUInt16( b, 0x14 );
			uint Unknown2 = BitConverter.ToUInt16( b, 0x18 );


			if ( hasUnknown0004 ||
								  hasUnknown0020 || hasUnknown0040 || hasUnknown0080 ||
				hasUnknown0100 || hasUnknown0200 || hasUnknown0400 || hasUnknown0800 ||
				hasUnknown1000 || hasUnknown2000 || hasUnknown4000 || hasUnknown8000 ) {
				Console.WriteLine( "UNKNOWN META BIT in " + file );
				return -1;
			}

			if ( !hasFilestart ) {
				Console.WriteLine( "Welp this file doesn't have file starts, can't do anything." );
				return -1;
			}

			string dirname = file + ".ext";
			System.IO.Directory.CreateDirectory( dirname );

			for ( uint i = 0; i < filecount; ++i ) {
				uint entryLoc = headersize + ( i * entrysize );
				uint inEntryLoc = 0;

				uint fileloc = BitConverter.ToUInt32( b, (int)( entryLoc + inEntryLoc ) );
				inEntryLoc += 4;

				uint fileend;
				if ( hasFileend ) {
					fileend = BitConverter.ToUInt32( b, (int)( entryLoc + inEntryLoc ) );
					inEntryLoc += 4;
				} else {
					// grab end via start of next file
					fileend = BitConverter.ToUInt32( b, (int)( entryLoc + entrysize ) );
				}

				if ( fileloc == 0 || fileend == 0 || fileloc == 0xFFFFFFFF || fileend == 0xFFFFFFFF ) continue;

				string filename;
				if ( hasFilename ) {
					filename = Encoding.ASCII.GetString( b, (int)( entryLoc + inEntryLoc ), 0x20 ).Trim( '\0' );
					inEntryLoc += 0x20;

					if ( String.IsNullOrEmpty( filename ) ) continue;
				} else {
					filename = i.ToString( "D4" );
				}

				if ( hasExtension ) {
					string extension =
						Encoding.ASCII.GetString( b, (int)( entryLoc + inEntryLoc ), 4 ).Trim( '\0' );
					filename += "." + extension;
					inEntryLoc += 4;
				}


				// write file
				FileStream outfile;
				outfile = new FileStream( dirname + '/' + filename, FileMode.Create );
				try {
					outfile.Write( b, (int)fileloc, (int)( fileend - fileloc ) );
				} catch ( Exception ) {
					try {
						outfile.Write( b, (int)fileloc, (int)( fileend ) );
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
