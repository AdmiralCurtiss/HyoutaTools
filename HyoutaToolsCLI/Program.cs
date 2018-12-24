using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaToolsCLI {
	public class Program {
		static int Main( string[] args ) {
			HyoutaTools.Initialization.Initialize();

			var tool = HyoutaTools.Initialization.ParseToolFromCommandLineArgs( args );
			if ( tool != null ) {
				return tool.Execute( tool.Arguments );
			} else {
				PrintUsage( HyoutaTools.Initialization.GetRegisteredTools(), args.Length > 0 ? args[0] : null );
				return -1;
			}
		}

		private static void PrintUsage( IEnumerable<KeyValuePair<HyoutaTools.ProgramName, HyoutaTools.ProgramNames.ExecuteProgramDelegate>> tools, string part = null ) {
			bool programFound = false;
			if ( part != null ) { part = part.ToLowerInvariant(); }
			foreach ( var p in tools.OrderBy( x => x.Key ) ) {
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
