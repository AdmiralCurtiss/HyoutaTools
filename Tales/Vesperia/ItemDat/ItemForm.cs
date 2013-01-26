using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HyoutaTools.Tales.Vesperia.TSS;

namespace HyoutaTools.Tales.Vesperia.ItemDat
{
    public partial class ItemForm : Form
    {
        ItemDat itemDat;
        TSSFile TSS;

        List<Label> labels;
        List<TextBox> textboxes;

        public ItemForm(ItemDat itemDat, TSSFile TSS)
        {
            InitializeComponent();

            this.itemDat = itemDat;
            this.TSS = TSS;

            labels = new List<Label>();
            textboxes = new List<TextBox>();

            for ( int i = 0; i < ItemDatSingle.size / 4; ++i )
            {
                Label l = new Label();
                l.Text = "";
                l.Size = new System.Drawing.Size(100, 20);
                TextBox b = new TextBox();
				b.Size = new System.Drawing.Size(50, 20);
                b.Text = "";

                labels.Add(l);
                textboxes.Add(b);

                FlowLayoutPanel p = new FlowLayoutPanel();
                p.Size = new System.Drawing.Size(200, 20);
                p.FlowDirection = FlowDirection.LeftToRight;
                p.Controls.Add(l);
                p.Controls.Add(b);

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
						flowLayoutPanel1.Controls.Add(p);
						break;
				}
            }

            foreach (ItemDatSingle i in itemDat.items)
            {
				listBox1.Items.Add( GetEntry(i.UnknownRest[(int)ItemData.NamePointer]).StringENG );
            }

        }

		private TSSEntry GetEntry( uint ptr ) {
			int ItemStartInTss = 0x1A67; // 360
			//int ItemStartInTss = 0x22C1; // PS3
			return TSS.Entries[ptr - 110000 + ItemStartInTss];
		}

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
			ItemDatSingle item = itemDat.items[listBox1.SelectedIndex];
            
            for (int i = 0; i < ItemDatSingle.size / 4; ++i)
            {
                labels[i].Text = ((ItemData)i).ToString();
                textboxes[i].Text = item.UnknownRest[i].ToString();
            }

			TSSEntry entry = GetEntry(item.UnknownRest[(int)ItemData.NamePointer]);
            labelName.Text = String.IsNullOrEmpty(entry.StringENG) ? entry.StringJPN : entry.StringENG;
			entry = GetEntry(item.UnknownRest[(int)ItemData.DescriptionPointer]);
            labelDescription.Text = String.IsNullOrEmpty(entry.StringENG) ? entry.StringJPN : entry.StringENG;
			entry = GetEntry(item.UnknownRest[(int)ItemData.UnknownTextPointer]);
            labelUnknown.Text = String.IsNullOrEmpty(entry.StringENG) ? entry.StringJPN : entry.StringENG;


        }
    }
}
