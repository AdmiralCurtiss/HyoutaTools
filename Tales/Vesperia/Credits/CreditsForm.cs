using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HyoutaTools.Tales.Vesperia.TSS;

namespace HyoutaTools.Tales.Vesperia.Credits {
	public partial class CreditsForm : Form {
		string itemDatFilename;
		CreditsInfo itemDat;
		TSSFile TSS;

		List<Label> labels;
		List<TextBox> textboxes;

		public CreditsForm( string itemDatFilename, TSSFile TSS ) {
			InitializeComponent();

			LoadCreditsInfoFile( itemDatFilename );
			this.TSS = TSS;

			labels = new List<Label>();
			textboxes = new List<TextBox>();

			for ( int i = 0; i < CreditsInfoSingle.Size / 4; ++i ) {
				Label l = new Label();
				l.Text = "";
				l.Size = new System.Drawing.Size( 80, 20 );
				TextBox b = new TextBox();
				b.Size = new System.Drawing.Size( 80, 20 );
				b.Text = "";

				labels.Add( l );
				textboxes.Add( b );

				FlowLayoutPanel p = new FlowLayoutPanel();
				p.Size = new System.Drawing.Size( 200, 20 );
				p.FlowDirection = FlowDirection.LeftToRight;
				p.Controls.Add( l );
				p.Controls.Add( b );

				flowLayoutPanel1.Controls.Add( p );
			}

			Reload();
		}

		private void LoadCreditsInfoFile( string itemDatFilename ) {
			this.itemDatFilename = itemDatFilename;
			this.itemDat = new CreditsInfo( itemDatFilename );
		}

		private void Reload() {
			listBox1.Items.Clear();
			foreach ( CreditsInfoSingle i in itemDat.items ) {
				listBox1.Items.Add( i.Offset.ToString( "X6" ) + ": [" + i.Data[0] + "] " + GetEntry( i.Data[(int)CreditsData.EntryNumber] ).StringJPN );
			}
		}

		private TSSEntry GetEntry( uint ptr ) {
			try {
				int ItemStartInTss = 17763; // PS3
				return TSS.Entries[ptr - 340000 + ItemStartInTss];
			} catch ( Exception ) {
				return new TSSEntry( new uint[0], "", "", 0, 0 );
			}
		}

		private void listBox1_SelectedIndexChanged( object sender, EventArgs e ) {
			CreditsInfoSingle item = itemDat.items[listBox1.SelectedIndex];

			for ( int i = 0; i < CreditsInfoSingle.Size / 4; ++i ) {
				labels[i].Text = ( (CreditsData)i ).ToString();
				switch ( i ) {
					default:
						textboxes[i].Text = item.Data[i].ToString();
						break;
					case 3:
					case 4:
						uint d = item.Data[i];
						byte[] b = BitConverter.GetBytes(d);
						float f = BitConverter.ToSingle( b, 0 );
						textboxes[i].Text = f.ToString();
						break;
				}
			}

			//TSSEntry entry = GetEntry(item.UnknownRest[(int)ItemData.NamePointer]);
			//labelName.Text = String.IsNullOrEmpty(entry.StringENG) ? entry.StringJPN : entry.StringENG;
			//entry = GetEntry(item.UnknownRest[(int)ItemData.DescriptionPointer]);
			//labelDescription.Text = String.IsNullOrEmpty(entry.StringENG) ? entry.StringJPN : entry.StringENG;
			//entry = GetEntry(item.UnknownRest[(int)ItemData.UnknownTextPointer]);
			//labelUnknown.Text = String.IsNullOrEmpty(entry.StringENG) ? entry.StringJPN : entry.StringENG;


		}

		private void buttonReload_Click( object sender, EventArgs e ) {
			LoadCreditsInfoFile( this.itemDatFilename );
			Reload();
		}
	}
}
