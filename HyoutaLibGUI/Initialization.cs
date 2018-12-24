using HyoutaTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HyoutaTools.ProgramNames;

namespace HyoutaLibGUI {
	public class Initialization {
		public static void RegisterGuiOnlyTools() {
			HyoutaTools.Initialization.RegisterTool( new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "DanganRonpa.Font.Viewer",                 "DrFont"      ), DanganRonpa.Font.Viewer.Program.Execute ) );
		}
	}
}
