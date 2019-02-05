using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HyoutaToolsGUI {
	public partial class ToolSelectionForm : Form {
		private class ToolInList {
			public HyoutaTools.IProgram Program;
			public override string ToString() { return Program.Identifiers().First(); }
		}

		public ToolSelectionForm() {
			InitializeComponent();

			HyoutaTools.Initialization.Initialize();
			HyoutaLibGUI.Initialization.RegisterGuiOnlyTools();
			foreach ( var tool in HyoutaTools.Initialization.GetRegisteredTools().OrderBy( x => x.Identifiers().First() ) ) {
				listBoxTools.Items.Add( new ToolInList() { Program = tool } );
			}
		}

		private void buttonRun_Click( object sender, EventArgs e ) {
			ToolInList t = listBoxTools.SelectedItem as ToolInList;
			if ( t != null ) {
				try {
					// TODO: Better args handling or whatever.
					t.Program.Execute( textBoxArgs.Text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries ).ToList() );
				} catch ( Exception ex ) {
					Console.WriteLine( "Error during execution: " + ex.ToString() );
				}
			}
		}
	}
}
