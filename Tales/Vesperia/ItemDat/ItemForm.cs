using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HyoutaTools.Tales.Vesperia.TSS;
using HyoutaTools.Tales.Vesperia.Website;

namespace HyoutaTools.Tales.Vesperia.ItemDat {
	public partial class ItemForm : Form {
		GameVersion Version;

		ItemDat itemDat;
		TSSFile TSS;
		Dictionary<uint, TSSEntry> InGameIdDict;
		T8BTSK.T8BTSK Skills;
		T8BTEMST.T8BTEMST Enemies;
		COOKDAT.COOKDAT Recipes;
		WRLDDAT.WRLDDAT Locations;

		List<Label> labels;
		List<TextBox> textboxes;

		public ItemForm( GameVersion version, ItemDat itemDat, TSSFile TSS, T8BTSK.T8BTSK skills, T8BTEMST.T8BTEMST enemies, COOKDAT.COOKDAT cookdat, WRLDDAT.WRLDDAT locations ) {
			InitializeComponent();

			this.Version = version;
			this.itemDat = itemDat;
			this.TSS = TSS;
			this.Skills = skills;
			this.Enemies = enemies;
			this.Recipes = cookdat;
			this.Locations = locations;
			this.InGameIdDict = TSS.GenerateInGameIdDictionary();

			labels = new List<Label>();
			textboxes = new List<TextBox>();

			for ( int i = 0; i < ItemDatSingle.size / 4; ++i ) {
				Label l = new Label();
				l.Text = "";
				l.Size = new System.Drawing.Size( 100, 20 );
				TextBox b = new TextBox();
				b.Size = new System.Drawing.Size( 50, 20 );
				b.Text = "";

				labels.Add( l );
				textboxes.Add( b );

				FlowLayoutPanel p = new FlowLayoutPanel();
				p.Size = new System.Drawing.Size( 200, 20 );
				p.FlowDirection = FlowDirection.LeftToRight;
				p.Controls.Add( l );
				p.Controls.Add( b );

				switch ( (ItemData)i ) {
					case ItemData.NamePointer:
					case ItemData.DescriptionPointer:
					case ItemData.UnknownTextPointer:
					case ItemData.TextIDPart1:
					case ItemData.TextIDPart2:
					case ItemData.TextIDPart3:
					case ItemData.TextIDPart4:
					case ItemData.TextIDPart5:
					case ItemData.TextIDPart6:
					case ItemData.TextIDPart7:
					case ItemData.TextIDPart8:
						break;
					default:
						flowLayoutPanel1.Controls.Add( p );
						break;
				}
			}

			foreach ( ItemDatSingle i in itemDat.items ) {
				var entry = GetEntry( i.Data[(int)ItemData.NamePointer] );
				listBox1.Items.Add( entry.StringEngOrJpn );
			}

		}

		private TSSEntry GetEntry( uint ptr ) {
			return InGameIdDict[ptr];
		}

		private void listBox1_SelectedIndexChanged( object sender, EventArgs e ) {
			ItemDatSingle item = itemDat.items[listBox1.SelectedIndex];

			for ( int i = 0; i < ItemDatSingle.size / 4; ++i ) {
				labels[i].Text = ( (ItemData)i ).ToString();
				textboxes[i].Text = item.Data[i].ToString();
			}

			TSSEntry entry = GetEntry( item.Data[(int)ItemData.NamePointer] );
			labelName.Text = entry.StringEngOrJpn;
			entry = GetEntry( item.Data[(int)ItemData.DescriptionPointer] );
			labelDescription.Text = entry.StringEngOrJpn;
			entry = GetEntry( item.Data[(int)ItemData.UnknownTextPointer] );
			labelUnknown.Text = entry.StringEngOrJpn;
			textBoxGeneratedText.Text = ItemDat.GetItemDataAsText( Version, itemDat, item, Skills, Enemies, Recipes, Locations, TSS, InGameIdDict );
		}

		private void buttonGenerateText_Click( object sender, EventArgs e ) {
			var sb = new StringBuilder();
			foreach ( var item in itemDat.items ) {
				sb.AppendLine( ItemDat.GetItemDataAsText( Version, itemDat, item, Skills, Enemies, Recipes, Locations, TSS, InGameIdDict ) );
				sb.AppendLine();
				sb.AppendLine();
			}
			Clipboard.SetText( sb.ToString() );
		}

		private void buttonGenerateHtml_Click( object sender, EventArgs e ) {
			var site = new GenerateWebsite();
			site.Version = Version;
			site.Items = itemDat;
			site.StringDic = TSS;
			site.Skills = Skills;
			site.Enemies = Enemies;
			site.Recipes = Recipes;
			site.InGameIdDict = InGameIdDict;

			Clipboard.SetText( site.GenerateHtmlItems() );
		}
	}
}
