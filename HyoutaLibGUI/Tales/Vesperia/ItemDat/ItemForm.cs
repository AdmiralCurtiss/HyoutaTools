using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HyoutaTools.Tales.Vesperia;
using HyoutaTools.Tales.Vesperia.TSS;
using HyoutaTools.Tales.Vesperia.Website;

namespace HyoutaLibGUI.Tales.Vesperia.ItemDat {
	public partial class ItemForm : Form {
		GameVersion Version;
		int Locale;

		HyoutaTools.Tales.Vesperia.ItemDat.ItemDat itemDat;
		TSSFile TSS;
		Dictionary<uint, TSSEntry> InGameIdDict;
		HyoutaTools.Tales.Vesperia.T8BTSK.T8BTSK Skills;
		HyoutaTools.Tales.Vesperia.T8BTEMST.T8BTEMST Enemies;
		HyoutaTools.Tales.Vesperia.COOKDAT.COOKDAT Recipes;
		HyoutaTools.Tales.Vesperia.WRLDDAT.WRLDDAT Locations;

		List<Label> labels;
		List<TextBox> textboxes;

		public ItemForm( GameVersion version, int locale, HyoutaTools.Tales.Vesperia.ItemDat.ItemDat itemDat, TSSFile TSS, HyoutaTools.Tales.Vesperia.T8BTSK.T8BTSK skills, HyoutaTools.Tales.Vesperia.T8BTEMST.T8BTEMST enemies, HyoutaTools.Tales.Vesperia.COOKDAT.COOKDAT cookdat, HyoutaTools.Tales.Vesperia.WRLDDAT.WRLDDAT locations ) {
			InitializeComponent();

			this.Version = version;
			this.Locale = locale;
			this.itemDat = itemDat;
			this.TSS = TSS;
			this.Skills = skills;
			this.Enemies = enemies;
			this.Recipes = cookdat;
			this.Locations = locations;
			this.InGameIdDict = TSS.GenerateInGameIdDictionary();

			labels = new List<Label>();
			textboxes = new List<TextBox>();

			for ( int i = 0; i < HyoutaTools.Tales.Vesperia.ItemDat.ItemDatSingle.size / 4; ++i ) {
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

				switch ( (HyoutaTools.Tales.Vesperia.ItemDat.ItemData)i ) {
					case HyoutaTools.Tales.Vesperia.ItemDat.ItemData.NamePointer:
					case HyoutaTools.Tales.Vesperia.ItemDat.ItemData.DescriptionPointer:
					case HyoutaTools.Tales.Vesperia.ItemDat.ItemData.UnknownTextPointer:
					case HyoutaTools.Tales.Vesperia.ItemDat.ItemData.TextIDPart1:
					case HyoutaTools.Tales.Vesperia.ItemDat.ItemData.TextIDPart2:
					case HyoutaTools.Tales.Vesperia.ItemDat.ItemData.TextIDPart3:
					case HyoutaTools.Tales.Vesperia.ItemDat.ItemData.TextIDPart4:
					case HyoutaTools.Tales.Vesperia.ItemDat.ItemData.TextIDPart5:
					case HyoutaTools.Tales.Vesperia.ItemDat.ItemData.TextIDPart6:
					case HyoutaTools.Tales.Vesperia.ItemDat.ItemData.TextIDPart7:
					case HyoutaTools.Tales.Vesperia.ItemDat.ItemData.TextIDPart8:
						break;
					default:
						flowLayoutPanel1.Controls.Add( p );
						break;
				}
			}

			foreach ( HyoutaTools.Tales.Vesperia.ItemDat.ItemDatSingle i in itemDat.items ) {
				var entry = GetEntry( i.Data[(int)HyoutaTools.Tales.Vesperia.ItemDat.ItemData.NamePointer] );
				listBox1.Items.Add( entry.GetString(locale) );
			}

		}

		private TSSEntry GetEntry( uint ptr ) {
			return InGameIdDict[ptr];
		}

		private void listBox1_SelectedIndexChanged( object sender, EventArgs e ) {
			HyoutaTools.Tales.Vesperia.ItemDat.ItemDatSingle item = itemDat.items[listBox1.SelectedIndex];

			for ( int i = 0; i < HyoutaTools.Tales.Vesperia.ItemDat.ItemDatSingle.size / 4; ++i ) {
				labels[i].Text = ( (HyoutaTools.Tales.Vesperia.ItemDat.ItemData)i ).ToString();
				textboxes[i].Text = item.Data[i].ToString();
			}

			TSSEntry entry = GetEntry( item.Data[(int)HyoutaTools.Tales.Vesperia.ItemDat.ItemData.NamePointer] );
			labelName.Text = entry.GetString(Locale);
			entry = GetEntry( item.Data[(int)HyoutaTools.Tales.Vesperia.ItemDat.ItemData.DescriptionPointer] );
			labelDescription.Text = entry.GetString(Locale);
			entry = GetEntry( item.Data[(int)HyoutaTools.Tales.Vesperia.ItemDat.ItemData.UnknownTextPointer] );
			labelUnknown.Text = entry.GetString(Locale);
			textBoxGeneratedText.Text = HyoutaTools.Tales.Vesperia.ItemDat.ItemDat.GetItemDataAsText(Version, Locale, itemDat, item, Skills, Enemies, Recipes, Locations, TSS, InGameIdDict);
		}

		private void buttonGenerateText_Click( object sender, EventArgs e ) {
			var sb = new StringBuilder();
			foreach ( var item in itemDat.items ) {
				sb.AppendLine(HyoutaTools.Tales.Vesperia.ItemDat.ItemDat.GetItemDataAsText(Version, Locale, itemDat, item, Skills, Enemies, Recipes, Locations, TSS, InGameIdDict));
				sb.AppendLine();
				sb.AppendLine();
			}
			Clipboard.SetText( sb.ToString() );
		}

		private void buttonGenerateHtml_Click( object sender, EventArgs e ) {
			var site = new WebsiteGenerator();
			site.Version = Version;
			site.Items = itemDat;
			site.StringDic = TSS;
			site.Skills = Skills;
			site.Enemies = Enemies;
			site.Recipes = Recipes;
			site.InGameIdDict = InGameIdDict;
			site.Version = Version;
			site.Language = Locale == 0 ? WebsiteLanguage.Jp : WebsiteLanguage.En;

			Clipboard.SetText( site.GenerateHtmlItems() );
		}
	}
}
