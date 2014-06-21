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

			new FPS4( file ).Extract( file + ".ext" );

			return 0;
		}
	}

	public class FPS4Entry {
		public FPS4Entry() { }
	}
	public class FPS4 {
		public FPS4() { }
		public FPS4( string inFilename ) {
			if ( !LoadFile( inFilename ) ) {
				throw new Exception( "Failed loading FPS4: " + inFilename );
			}
		}
		~FPS4() {
			if ( infile != null ) {
				infile.Close();
			}
		}

		FileStream infile = null;
		uint FileCount;
		uint HeaderSize;
		uint FirstFileStart;
		ushort EntrySize;
		ushort ContentBitmask;
		uint Unknown2;
		uint ArchiveNameLocation;

		// -- bitmask examples --
		// 0x000F -> loc, end, size, name
		// 0x0007 -> loc, end, size
		// 0x0047 -> loc, end, size, ptr to path? attributes? something like that
		public bool ContainsStartPointers { get { return ( ContentBitmask & 0x0001 ) == 0x0001; } }
		public bool ContainsEndPointers { get { return ( ContentBitmask & 0x0002 ) == 0x0002; } }
		public bool ContainsFileSizes { get { return ( ContentBitmask & 0x0004 ) == 0x0004; } }
		public bool ContainsFilenames { get { return ( ContentBitmask & 0x0008 ) == 0x0008; } }
		public bool ContainsFiletypes { get { return ( ContentBitmask & 0x0020 ) == 0x0020; } }
		public bool ContainsFilepaths { get { return ( ContentBitmask & 0x0040 ) == 0x0040; } }

		private bool LoadFile( string inFilename ) {
			try {
				infile = new FileStream( inFilename, FileMode.Open );
			} catch ( Exception ) {
				Console.WriteLine( "ERROR: can't open " + inFilename );
				return false;
			}

			infile.Seek( 0x00, SeekOrigin.Begin );
			string magic = infile.ReadAscii( 4 );
			if ( magic != "FPS4" ) {
				Console.WriteLine( "Not an FPS4 file!" );
				return false;
			}

			FileCount = infile.ReadUInt32().SwapEndian();
			HeaderSize = infile.ReadUInt32().SwapEndian();
			FirstFileStart = infile.ReadUInt32().SwapEndian();
			EntrySize = infile.ReadUInt16().SwapEndian();
			ContentBitmask = infile.ReadUInt16().SwapEndian();
			Unknown2 = infile.ReadUInt32().SwapEndian();
			ArchiveNameLocation = infile.ReadUInt32().SwapEndian();

			Console.WriteLine( "Content Bitmask: 0x" + ContentBitmask.ToString( "X4" ) );

			return true;
		}

		public void Extract( string dirname ) {
			System.IO.Directory.CreateDirectory( dirname );

			for ( uint i = 0; i < FileCount - 1; ++i ) {
				infile.Seek( HeaderSize + ( i * EntrySize ), SeekOrigin.Begin );

				uint fileloc = infile.ReadUInt32().SwapEndian();
				uint fileend = fileloc + infile.ReadUInt32().SwapEndian();
				uint filesize = infile.ReadUInt32().SwapEndian();
				if ( fileloc == 0xFFFFFFFF || fileloc == 0x00 ) {
					Console.WriteLine( "Skipped #" + i.ToString( "D4" ) + ", can't find file" );
					continue;
				}

				string filename = "";
				if ( ContainsFilenames ) {
					filename = infile.ReadAsciiNullterm();
				}

				string extension = "";
				if ( ContainsFiletypes ) {
					extension = '.' + infile.ReadAscii( 4 ).TrimNull();
				}

				string path = "";
				if ( ContainsFilepaths ) {
					uint pathLocation = infile.ReadUInt32().SwapEndian();
					if ( pathLocation != 0x00 ) {
						long tmp = infile.Position;
						infile.Seek( pathLocation, SeekOrigin.Begin );
						path = infile.ReadAsciiNullterm();
						infile.Seek( tmp, SeekOrigin.Begin );

						if ( path.StartsWith( "name=" ) ) {
							path = path.Substring( 5 );
						}

						int finaldir = path.LastIndexOf( '/' );
						if ( finaldir == -1 ) { finaldir = 0; }
						if ( filename == "" ) {
							filename = path.Substring( finaldir + 1 ) + '.' + i.ToString( "D4" );
						} else {
							filename = path.Substring( finaldir + 1 ) + '.' + filename;
						}
						path = path.Substring( 0, finaldir );
					}
				}
				//string description = Util.GetText((int)(Util.SwapEndian(BitConverter.ToUInt32(b, (int)(headersize + (i * entrysize) + 0x2C)))), b);

				if ( filename == "" ) {
					filename = i.ToString( "D4" );
				}

				FileStream outfile;
				try {
					System.IO.Directory.CreateDirectory( dirname + '/' + path );
					outfile = new FileStream( dirname + '/' + path + '/' + filename + extension, FileMode.Create );
				} catch ( Exception ) {
					try {
						outfile = new FileStream( dirname + '/' + path + '/' + i.ToString( "D4" ) + extension, FileMode.Create );
					} catch ( Exception ) {
						try {
							outfile = new FileStream( dirname + '/' + i.ToString( "D4" ) + extension, FileMode.Create );
						} catch ( Exception ) {
							outfile = new FileStream( dirname + '/' + i.ToString( "D4" ), FileMode.Create );
						}
					}
				}

				Console.WriteLine( "Extracting #" + i.ToString( "D4" ) + ": " + path + '/' + System.IO.Path.GetFileName( outfile.Name ) );

				try {
					infile.Seek( fileloc, SeekOrigin.Begin );
					Util.CopyStream( infile, outfile, (int)filesize );
				} catch ( Exception ) {
					try {
						infile.Seek( fileloc, SeekOrigin.Begin );
						Util.CopyStream( infile, outfile, (int)( fileend - fileloc ) );
					} catch ( Exception ) {
						Console.WriteLine( "ERROR on file " + outfile.Name );
					}
				}
				outfile.Close();
			}
		}
	}
}
