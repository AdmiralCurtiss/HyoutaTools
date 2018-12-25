using HyoutaTools.Trophy;
using HyoutaTools.Trophy.Viewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaLibGUI.Trophy.Viewer
{
    public partial class GameSelectForm : Form
    {
        private GameFolder Games;

        public GameSelectForm(GameFolder Games)
        {
            this.Games = Games;
            InitializeComponent();
            this.listBox1.ItemHeight = 88;
            this.listBox1.IsGameList = true;
            this.listBox1.DataSource = Games.TrophyLists;
        }

        private void OpenGame(TrophyConfNode TrophyConf)
        {
            TrophyForm tf = new TrophyForm(TrophyConf);
            tf.Show();
        }

        private void OpenSelectedGame()
        {
            try
            {
                TrophyConfNode t = (TrophyConfNode)listBox1.SelectedItem;

                if (radioButtonSortByDate.Checked)
                {
                    t.SortByUnlockedBeforeLocked(TropUsrSingleTrophy.SortByTimestamp, checkBoxDescending.Checked, false, true);
                }
                else
                {
                    t.SortBy(TropUsrSingleTrophy.SortByTrophyID, checkBoxDescending.Checked);
                }

                OpenGame(t);
            }
            catch (Exception) { }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedGame();
        }
    }
}
