using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.Trophy.Viewer
{
    public partial class TrophyForm : Form
    {
        private TrophyConfNode TrophyFile;

        public TrophyForm(TrophyConfNode TrophyFile)
        {
            InitializeComponent();
            this.TrophyFile = TrophyFile;
            this.listBox1.CurrentGame = TrophyFile;
            this.listBox1.DataSource = TrophyFile.Trophies.Values.ToArray();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TrophyNode t = (TrophyNode)listBox1.SelectedItem;

            this.labelName.Text = t.Name;
            
            switch (t.TType)
            {
                case "B":
                    this.labelTType.Text = "Bronze";
                    break;
                case "S":
                    this.labelTType.Text = "Silver";
                    break;
                case "G":
                    this.labelTType.Text = "Gold";
                    break;
                case "P":
                    this.labelTType.Text = "Platinum";
                    break;
                default:
                    this.labelTType.Text = t.TType;
                    break;
            }

            this.labelDescription.Text = t.Detail;

            if (t.Hidden)
            {
                this.labelHidden.Text = "Hidden";
            }
            else
            {
                this.labelHidden.Text = "Visible";
            }

            this.labelTrophyID.Text = t.ID;

            TropUsrSingleTrophy u = TrophyFile.TropUsrFile.TrophyInfos[UInt32.Parse(t.ID)];

            if (u.Unlocked == 1)
            {
                this.labelUnlocked.Text = "Acquired";
				this.labelTimestamp.Text = Util.PS3TimeToDateTime( u.Timestamp1 ).ToString();
				this.labelTimestamp2.Text = Util.PS3TimeToDateTime( u.Timestamp2 ).ToString();
            }
            else
            {
                this.labelUnlocked.Text = "Locked";
                this.labelTimestamp.Text = "";
                this.labelTimestamp2.Text = "";
            }
        }
    }
}
