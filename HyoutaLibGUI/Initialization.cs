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
			HyoutaTools.Initialization.RegisterTool( new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.PicrossDS.SaveEditor",              "-"           ), Other.PicrossDS.SaveEditor.Execute ) );
			HyoutaTools.Initialization.RegisterTool( new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "DanganRonpa.Nonstop.Viewer",              "DrNonstop"   ), DanganRonpa.Nonstop.RunNonstopForm.Execute ) );
			HyoutaTools.Initialization.RegisterTool( new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Vesperia.Credits.Viewer",           "ToVcredits"  ), Tales.Vesperia.Credits.RunCreditsViewer.Execute ) );
			HyoutaTools.Initialization.RegisterTool( new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Vesperia.Font.Viewer",              "ToVfont"     ), Tales.Vesperia.Font.Viewer.Program.Execute ) );
		}
	}
}
