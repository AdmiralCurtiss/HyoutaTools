using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HyoutaTools.Tales.Vesperia.TSS;

namespace HyoutaTools.Tales.Vesperia.ItemDat {
	public partial class ItemForm : Form {
		ItemDat itemDat;
		TSSFile TSS;
		Dictionary<uint, TSSEntry> InGameIdDict;
		T8BTSK.T8BTSK Skills;
		T8BTEMST.T8BTEMST Enemies;

		List<Label> labels;
		List<TextBox> textboxes;

		public ItemForm( ItemDat itemDat, TSSFile TSS, T8BTSK.T8BTSK skills, T8BTEMST.T8BTEMST enemies ) {
			InitializeComponent();

			this.itemDat = itemDat;
			this.TSS = TSS;
			this.Skills = skills;
			this.Enemies = enemies;
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
				listBox1.Items.Add( String.IsNullOrEmpty( entry.StringENG ) ? entry.StringJPN : entry.StringENG );
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
			labelName.Text = String.IsNullOrEmpty( entry.StringENG ) ? entry.StringJPN : entry.StringENG;
			entry = GetEntry( item.Data[(int)ItemData.DescriptionPointer] );
			labelDescription.Text = String.IsNullOrEmpty( entry.StringENG ) ? entry.StringJPN : entry.StringENG;
			entry = GetEntry( item.Data[(int)ItemData.UnknownTextPointer] );
			labelUnknown.Text = String.IsNullOrEmpty( entry.StringENG ) ? entry.StringJPN : entry.StringENG;
			textBoxGeneratedText.Text = ItemDat.GetItemDataAsText( itemDat, item, Skills, Enemies, TSS, InGameIdDict );
		}

		private void buttonGenerateText_Click( object sender, EventArgs e ) {
			var sb = new StringBuilder();
			foreach ( var item in itemDat.items ) {
				sb.AppendLine( ItemDat.GetItemDataAsText( itemDat, item, Skills, Enemies, TSS, InGameIdDict ) );
				sb.AppendLine();
				sb.AppendLine();
			}
			Clipboard.SetText( sb.ToString() );
		}

		private void buttonGenerateHtml_Click( object sender, EventArgs e ) {
			var sb = new StringBuilder();
			sb.AppendLine( "<html><head>" );
			sb.AppendLine( "<style>" );
			sb.AppendLine( "body { background-color: #68504F; color: #EFD1AE; font-size: 16; }" );
			sb.AppendLine( ".itemname { color: #FFEBD2; font-size: 20; }" );
			sb.AppendLine( ".itemdesc { }" );
			sb.AppendLine( ".equip { text-align: right; float: right; }" );
			sb.AppendLine( ".special { text-align: right; float: right; }" );
			sb.AppendLine( "table, tr, td, th { padding: 0px 4px 0px 0px; border-spacing: 0px; }" );
			sb.AppendLine( "a:link, a:visited, a:hover, a:active { color: #FFEBD2; }" );
			sb.AppendLine( "</style>" );
			sb.AppendLine( "</head><body><table>" );
			foreach ( var item in itemDat.items ) {
				if ( item.Data[(int)ItemData.Category] == 0 ) { continue; }
				sb.AppendLine( ItemDat.GetItemDataAsHtml( itemDat, item, Skills, Enemies, TSS, InGameIdDict ) );
				sb.AppendLine( "<tr><td colspan=\"5\"><hr></td></tr>" );
			}
			sb.AppendLine( "</table></body></html>" );

			string html = sb.ToString();
			html = VesperiaUtil.RemoveTags( html );
			html = html.Replace( "\x06(STA)", "[START]" );
			html = html.Replace( "\x06(L3)", "[LEFT STICK]" );
			html = html.Replace( "\x06(ST1)", "<img src=\"text-icons/icon-status-01.png\" height=\"16\" width=\"16\">" );
			html = html.Replace( "\x06(ST2)", "<img src=\"text-icons/icon-status-02.png\" height=\"16\" width=\"16\">" );
			html = html.Replace( "\x06(ST3)", "<img src=\"text-icons/icon-status-03.png\" height=\"16\" width=\"16\">" );
			html = html.Replace( "\x06(ST4)", "<img src=\"text-icons/icon-status-04.png\" height=\"16\" width=\"16\">" );
			html = html.Replace( "\x06(ST5)", "<img src=\"text-icons/icon-status-05.png\" height=\"16\" width=\"16\">" );
			html = html.Replace( "\x06(ST6)", "<img src=\"text-icons/icon-status-06.png\" height=\"16\" width=\"16\">" );
			html = html.Replace( "\x06(ST7)", "<img src=\"text-icons/icon-status-07.png\" height=\"16\" width=\"16\">" );
			Clipboard.SetText( html );
		}
	}
}
