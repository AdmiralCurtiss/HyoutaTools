using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HyoutaTools.Other.InvokeGimConv {
	class Program {

		static bool RunProgram( String prog, String args, bool displayCommandLine ) {
			if ( displayCommandLine ) {
				Console.Write( prog );
				Console.Write( " " );
				Console.WriteLine( args );
			}

			// Use ProcessStartInfo class
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.CreateNoWindow = false;
			startInfo.UseShellExecute = false;
			startInfo.FileName = prog;
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.Arguments = args;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;

			using ( Process exeProcess = Process.Start( startInfo ) ) {
				exeProcess.WaitForExit();
				string output = exeProcess.StandardOutput.ReadToEnd();

				if ( exeProcess.ExitCode != 0 ) {
					Console.WriteLine( prog + " returned nonzero:" );
					Console.WriteLine( output );
					throw new Exception( output );
					//return false;
				}

				return true;
			}
		}

		static void ReplaceInList( List<String> gimconvargs, string orig, string repl ) {
			for ( int i = 0; i < gimconvargs.Count; ++i ) {
				if ( gimconvargs[i] == orig ) {
					gimconvargs[i] = repl;
				}
			}
		}

		public static int Execute( string[] args ) {
			List<String> gimconvargs = new List<string>();

			string OriginalFile = null;
			int OffsetInOriFile = 0;

			foreach ( string arg in args ) {
				if ( arg.StartsWith( "----" ) ) {
					if ( arg.StartsWith( "----FILE" ) ) {
						OriginalFile = arg.Substring( 8 );
						// THIS IS A VERY UGLY HACK but windows command line batch file SUCK so here
						if ( OriginalFile.EndsWith( "-big.png" ) ) { return 0; }
					} else if ( arg.StartsWith( "----OFFS" ) ) {
						string suboffs = arg.Substring( 8 );
						if ( suboffs.EndsWith( "-big" ) ) { return 0; }
						OffsetInOriFile = Int32.Parse( suboffs, System.Globalization.NumberStyles.AllowHexSpecifier );
					}
				} else {
					gimconvargs.Add( "\"" + arg + "\"" );
				}
			}

			byte[] orig = System.IO.File.ReadAllBytes( OriginalFile );
			byte[] GimIdent = new byte[] { 0x4D, 0x49, 0x47, 0x2E, 0x30, 0x30, 0x2E, 0x31, 0x50, 0x53, 0x50, 0x00, 0x00, 0x00, 0x00, 0x00 };
			for ( int j = 0; j < GimIdent.Length; ++j ) {
				if ( orig[OffsetInOriFile + j] != GimIdent[j] ) {
					throw new Exception(
						"Could not find GIM header at requested location!\n" +
						"File: " + OriginalFile + "\n" +
						"Offs: " + OffsetInOriFile + "\n"  
					);
				}
			}

			int layers = BitConverter.ToInt16( orig, OffsetInOriFile + 0x6A );
			switch ( layers ) {
				case 3:
					// yeah this is expected, ok
					break;
				case 1:
					// also acceptable, just remove the half/quarter params
					gimconvargs.Remove( @"assets\h.png" );
					gimconvargs.Remove( @"assets\q.png" );
					gimconvargs.Remove( "\"assets\\h.png\"" );
					gimconvargs.Remove( "\"assets\\q.png\"" );
					break;
				default:
					throw new Exception( "Unsupported Layercount: " + layers );
			}

			int bitperpixel = BitConverter.ToInt16( orig, OffsetInOriFile + 0x4C );
			gimconvargs.Add( "--image_format" );
			switch ( bitperpixel ) {
				case 8:
					ReplaceInList( gimconvargs, @"assets\h.png", @"assets\h8.png" );
					ReplaceInList( gimconvargs, @"assets\q.png", @"assets\q8.png" );
					ReplaceInList( gimconvargs, "\"assets\\h.png\"", @"assets\h8.png" );
					ReplaceInList( gimconvargs, "\"assets\\q.png\"", @"assets\q8.png" );
					gimconvargs.Add( "index8" );
					break;
				case 4:
					ReplaceInList( gimconvargs, @"assets\h.png", @"assets\h4.png" );
					ReplaceInList( gimconvargs, @"assets\q.png", @"assets\q4.png" );
					ReplaceInList( gimconvargs, "\"assets\\h.png\"", @"assets\h4.png" );
					ReplaceInList( gimconvargs, "\"assets\\q.png\"", @"assets\q4.png" );
					gimconvargs.Add( "index4" );
					break;
				//case 16:
				//	gimconvargs.Add( "index16" );
				//	break;
				default:
					throw new Exception( "Unknown Bit Per Pixel!" );
			}




			StringBuilder ga = new StringBuilder();
			foreach ( string a in gimconvargs ) {
				ga.Append( a ).Append( ' ' );
			}
			bool success = RunProgram( "GimConv", ga.ToString(), displayCommandLine: false );
			return success ? 0 : -1;
		}
	}
}
