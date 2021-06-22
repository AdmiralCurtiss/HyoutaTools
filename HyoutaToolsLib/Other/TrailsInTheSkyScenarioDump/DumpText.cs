using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.TrailsInTheSkyScenarioDump {
	class DumpText {
		public static int Execute( List<string> args ) {
			new ScenarioBin( args[0] );
			return -1;
		}
	}
}
