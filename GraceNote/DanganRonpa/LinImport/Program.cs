using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using HyoutaTools.DanganRonpa.Lin;

namespace HyoutaTools.GraceNote.DanganRonpa.LinImport {
	class Program {
		public static int Execute( string[] args ) {
			if ( args.Length < 1 ) {
				Console.WriteLine( "Usage: DanganRonpaText_GraceNote -dumptxt text.lin out.txt" );
				return -1;
			}

			if ( args[0] == "-check" ) {
				return Check( args[1] );
			} else if ( args[0] == "-dumpinsertcheck" ) {
				int Alignment = 16;
				LinImport.Importer.Import( args[1], args[2], args[3], args[4] );
				LinExport.Exporter.Export( args[1], args[1] + ".new", args[2], Alignment );


				Byte[] OriginalFile = System.IO.File.ReadAllBytes( args[1] );
				LIN lin = new LIN( OriginalFile );
				lin.CreateFile( Alignment );
				return Compare( OriginalFile, System.IO.File.ReadAllBytes( args[1] + ".new" ), lin.UnalignedFilesize, args[1] + ".fail" );
			} else if ( args[0] == "-dumptxt" ) {
				return DumpTxt( args[1], args[2] );
			}

			return -1;
		}

		public static int Compare( Byte[] OriginalFile, Byte[] RecreatedFile, int UnalignedFilesize, string outputOnFail ) {
			try {
				if ( OriginalFile.Length != RecreatedFile.Length ) {
					throw new Exception( "Filesizes don't match!" );
				}

				bool fail = false;
				for ( int i = 0; i < OriginalFile.Length; ++i ) {
					if ( OriginalFile[i] != RecreatedFile[i] ) {
						if ( i >= UnalignedFilesize ) {
							Console.WriteLine( "Acceptable mismatch (stray end bytes) at byte 0x" + i.ToString( "X8" ) );
						} else {
							Console.WriteLine( "Mismatch at byte 0x" + i.ToString( "X8" ) );
							fail = true;
						}
					}
				}
				if ( fail ) { throw new Exception( "Mismatch in file." ); }
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				System.IO.File.WriteAllBytes( outputOnFail, RecreatedFile );
				return -1;
			}

			Console.WriteLine( "Check done." );
			return 0;
		}

		public static int Check( String Filename ) {
			Byte[] OriginalFile = System.IO.File.ReadAllBytes( Filename );
			LIN lin = new LIN( OriginalFile );
			Byte[] RecreatedFile = lin.CreateFile( 1024 );
			LIN linOrig = new LIN( OriginalFile );

			return Compare( OriginalFile, RecreatedFile, lin.UnalignedFilesize, Filename + ".fail" );
		}

		public static int DumpTxt( String Filename, String TxtFilename ) {
			LIN lin;
			try {
				lin = new LIN( Filename );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				Console.WriteLine( "Failed loading text file!" );
				return -1;
			}

			List<string> Output = new List<string>();

			foreach ( ScriptEntry s in lin.ScriptData ) {
				if ( s.Type == 0x02 ) {
					Output.Add( "Text: " + s.Text );
				} else {
					Output.Add( s.FormatForGraceNote() );
				}
			}

			if ( lin.UnreferencedText != null ) {
				foreach ( KeyValuePair<int, string> u in lin.UnreferencedText ) {
					Output.Add( "Unreferenced Text (" + u.Value + "): " + u.Key );
				}
			}

			System.IO.File.WriteAllLines( TxtFilename, Output.ToArray() );
			return 0;
		}
	}
}
