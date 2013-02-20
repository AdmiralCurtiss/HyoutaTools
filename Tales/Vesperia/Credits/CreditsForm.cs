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
			SuspendLayout();
			listBox1.Items.Clear();
			foreach ( CreditsInfoSingle i in itemDat.items ) {
				//if ( i.Data[0] != 2 ) continue;
				i.Form = this;
				listBox1.Items.Add( i );
			}
			ResumeLayout();
		}

		public TSSEntry GetEntry( uint ptr ) {
			try {
				int ItemStartInTss = 17763; // PS3
				//ItemStartInTss = 17763 + 216; // 360 with PS3 string_dic
				return TSS.Entries[ptr - 340000 + ItemStartInTss];
			} catch ( Exception ) {
				return new TSSEntry( new uint[0], "", "", 0, 0 );
			}
		}

		private void listBox1_SelectedIndexChanged( object sender, EventArgs e ) {
			CreditsInfoSingle item = (CreditsInfoSingle)listBox1.SelectedItem;

			for ( int i = 0; i < CreditsInfoSingle.Size / 4; ++i ) {
				labels[i].Text = ( (CreditsData)i ).ToString();
				switch ( i ) {
					default:
						textboxes[i].Text = item.Data[i].ToString();
						break;
					case 1:
					case 2:
						textboxes[i].Text = item.Data[i].ToString( "X" );
						break;
					case 3:
					case 4:
						textboxes[i].Text = Util.UIntToFloat( item.Data[i] ).ToString();
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
