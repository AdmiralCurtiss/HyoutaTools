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
			public HyoutaPluginBase.IProgram Program;
			public override string ToString() { return Program.Identifiers().First(); }
		}

		public ToolSelectionForm(string[] args) {
			InitializeComponent();

			HyoutaTools.Initialization.Initialize();
			HyoutaLibGUI.Initialization.RegisterGuiOnlyTools();
			foreach (var tool in HyoutaTools.Initialization.GetRegisteredTools().OrderBy(x => x.Identifiers().First())) {
				listBoxTools.Items.Add(new ToolInList() { Program = tool });
			}

			if (args != null && args.Length > 0) {
				string a = args[0];
				foreach (var tool in listBoxTools.Items) {
					ToolInList t = tool as ToolInList;
					if (t != null && t.Program.Identifiers().Contains(a)) {
						try {
							Exec(t, args.Skip(1).ToList());
						} catch (Exception ex) {
							MessageBox.Show("Error during execution: " + ex.ToString());
						}
						break;
					}
				}
			}
		}

		private void buttonRun_Click(object sender, EventArgs e) {
			ToolInList t = listBoxTools.SelectedItem as ToolInList;
			if (t != null) {
				try {
					// TODO: Better args handling or whatever.
					Exec(t, textBoxArgs.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList());
				} catch (Exception ex) {
					MessageBox.Show("Error during execution: " + ex.ToString());
				}
			}
		}

		private void Exec(ToolInList t, List<string> args) {
			t.Program.Execute(args);
		}
	}
}
