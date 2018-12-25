using HyoutaTools.DanganRonpa.Nonstop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaLibGUI.DanganRonpa.Nonstop
{
    public partial class NonstopForm : Form
    {
		NonstopFile itemDat;

        List<Label> labels;
        List<TextBox> textboxes;

		public NonstopForm( NonstopFile itemDat )
        {
            InitializeComponent();

            this.itemDat = itemDat;

            labels = new List<Label>();
            textboxes = new List<TextBox>();

            for ( int i = 0; i < itemDat.BytesPerEntry / 2; ++i )
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

				switch ( (NonstopSingleStructure)i ) {
					default:
						flowLayoutPanel1.Controls.Add(p);
						break;
				}
            }

            foreach (NonstopSingle i in itemDat.items)
            {
				listBox1.Items.Add( i.ToString() );
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
			NonstopSingle item = itemDat.items[listBox1.SelectedIndex];

			for ( int i = 0; i < itemDat.BytesPerEntry / 2; ++i )
            {
                labels[i].Text = ((NonstopSingleStructure)i).ToString();
				textboxes[i].Text = item.data[i].ToString();
            }

			/*
			TSSEntry entry = GetEntry( item.data[(int)NonstopSingleStructure.NamePointer] );
            labelName.Text = String.IsNullOrEmpty(entry.StringENG) ? entry.StringJPN : entry.StringENG;
			entry = GetEntry( item.data[(int)NonstopSingleStructure.DescriptionPointer] );
            labelDescription.Text = String.IsNullOrEmpty(entry.StringENG) ? entry.StringJPN : entry.StringENG;
			entry = GetEntry( item.data[(int)NonstopSingleStructure.UnknownTextPointer] );
            labelUnknown.Text = String.IsNullOrEmpty(entry.StringENG) ? entry.StringJPN : entry.StringENG;
			  */

        }
    }
}
