using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaToolsCLI {
	public class Program {
		static int Main( string[] args ) {
			if ( args.Length > 0 ) {
				string ProgramName = args[0];
				if ( ProgramName == "-" ) { PrintUsage(); return -1; }
				List<string> ProgramArguments = new List<string>( args.Length - 1 );
				for ( int i = 1; i < args.Length; ++i ) {
					ProgramArguments.Add( args[i] );
				}

				var kvp = HyoutaTools.ProgramNames.ProgramList.Find( x => x.Key.Equals( ProgramName ) );
				if ( kvp.Value != null ) {
					return kvp.Value( ProgramArguments );
				} else {
					PrintUsage( args[0] );
				}

			} else {
				PrintUsage();
			}
			return -1;
		}

		private static void PrintUsage( string part = null ) {
			bool programFound = false;
			if ( part != null ) { part = part.ToLowerInvariant(); }
			HyoutaTools.ProgramNames.ProgramList.Sort( ( x, y ) => x.Key.CompareTo( y.Key ) );
			foreach ( var p in HyoutaTools.ProgramNames.ProgramList ) {
				if ( part == null || p.Key.Name.ToLowerInvariant().Contains( part ) || p.Key.Shortcut.ToLowerInvariant().Contains( part ) ) {
					Console.WriteLine( String.Format( " {1,-12} {0}", p.Key.Name, p.Key.Shortcut ) );
					programFound = true;
				}
			}

			if ( !programFound ) {
				Console.WriteLine( String.Format( " No tool matching '{0}' found!", part ) );
			}
		}
	}
}
