using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools {
	public class ToolExecution {
		public ProgramNames.ExecuteProgramDelegate Execute { get; private set; }
		public List<string> Arguments { get; private set; }

		public ToolExecution( ProgramNames.ExecuteProgramDelegate execute, List<string> arguments ) {
			Execute = execute;
			Arguments = arguments;
		}
	}

	public class Initialization {
		private static List<KeyValuePair<ProgramName, ProgramNames.ExecuteProgramDelegate>> KnownTools = new List<KeyValuePair<ProgramName, ProgramNames.ExecuteProgramDelegate>>();

		public static void Initialize() {
			KnownTools.Clear();
			KnownTools.AddRange( ProgramNames.BuiltInTools );
		}

		public static IEnumerable<KeyValuePair<ProgramName, ProgramNames.ExecuteProgramDelegate>> GetRegisteredTools() {
			return KnownTools;
		}

		public static void RegisterTool( KeyValuePair<ProgramName, ProgramNames.ExecuteProgramDelegate> tool ) {
			KnownTools.Add( tool );
		}

		public static ToolExecution ParseToolFromCommandLineArgs( string[] args ) {
			if ( args.Length > 0 ) {
				string ProgramName = args[0];
				if ( ProgramName == "-" ) { return null; }
				List<string> ProgramArguments = new List<string>( args.Length - 1 );
				for ( int i = 1; i < args.Length; ++i ) {
					ProgramArguments.Add( args[i] );
				}

				var kvp = KnownTools.Find( x => x.Key.Equals( ProgramName ) );
				if ( kvp.Value != null ) {
					return new ToolExecution( kvp.Value, ProgramArguments );
				}
			}
			return null;
		}
	}
}
