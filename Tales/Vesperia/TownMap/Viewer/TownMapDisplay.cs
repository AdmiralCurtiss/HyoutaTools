using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.Tales.Vesperia.TownMap.Viewer
{
    public partial class TownMapDisplay : Form
    {
        TownMapTable Table;
        string TownMapImagePath;

        int XCorrection = Int32.MaxValue;
        int YCorrection = Int32.MaxValue;

        public TownMapDisplay()
        {
            InitializeComponent();
        }

        public TownMapDisplay(TownMapTable Table, string imagepath)
        {
            InitializeComponent();
            this.Table = Table;
            this.TownMapImagePath = imagepath;

            UpdateFields();
        }

        public void UpdateFields()
        {
            bool English = checkBoxEnglish.Checked;


            try
            {
                textBoxFilename.Text = Table.TownMapInfos[(int)TownMapIDBox.Value].Filename;
                if (English)
                {
                    labelTiles.Text = "Tile (Max " + Table.TownMapInfos[(int)TownMapIDBox.Value].TileAmountENG + ")";
                }
                else
                {
                    labelTiles.Text = "Tile (Max " + Table.TownMapInfos[(int)TownMapIDBox.Value].TileAmountJPN + ")";
                }


                String ImageFilename;
                TownMapTile tile;
                int TileAmount;
                if (English)
                {
                    ImageFilename = System.IO.Path.Combine( TownMapImagePath, "U_MAP_" + Table.TownMapInfos[(int)TownMapIDBox.Value].Filename + ".png" );
                    tile = Table.TownMapInfos[(int)TownMapIDBox.Value].TownMapTilesENG[(int)numericUpDownTileNumber.Value];
                    TileAmount = Table.TownMapInfos[(int)TownMapIDBox.Value].TileAmountENG;
                    textBoxTileOffLoc.Text = (Table.LocationOffsetTableENG + (((int)TownMapIDBox.Value) * 0x08)).ToString("X");
                }
                else
                {
                    ImageFilename = System.IO.Path.Combine( TownMapImagePath, "U_MAP_E" + Table.TownMapInfos[(int)TownMapIDBox.Value].Filename + ".png" );
                    tile = Table.TownMapInfos[(int)TownMapIDBox.Value].TownMapTilesJPN[(int)numericUpDownTileNumber.Value];
                    TileAmount = Table.TownMapInfos[(int)TownMapIDBox.Value].TileAmountJPN;
                    textBoxTileOffLoc.Text = (Table.LocationOffsetTableJPN + (((int)TownMapIDBox.Value) * 0x08)).ToString("X");
                }
                textBoxInfoLoc.Text = Table.TownMapInfos[(int)TownMapIDBox.Value].InfoLocation.ToString("X");
                textBoxTileLoc.Text = tile.Pointer.ToString("X");

                textBoxDisplayDelay.Text = tile.Time.ToString("X") + " (" + tile.Time + ")";
                textBoxTextureOffset.Text = tile.XTextureOffset.ToString("X") + " / " + tile.YTextureOffset.ToString("X");
                textBoxSize.Text = tile.XSize.ToString("X") + " / " + tile.YSize.ToString("X");
                textBoxDisplayOffset.Text = tile.XDisplayOffset.ToString("X") + " / " + tile.YDisplayOffset.ToString("X");

                System.Drawing.Bitmap tex = new System.Drawing.Bitmap(ImageFilename);
                System.Drawing.Bitmap img = new System.Drawing.Bitmap(1280, 720);

                XCorrection = Int32.MaxValue;
                YCorrection = Int32.MaxValue;

                for (int i = 0; i < TileAmount; i++)
                {
                    TownMapTile t;
                    if (English)
                    {
                        t = Table.TownMapInfos[(int)TownMapIDBox.Value].TownMapTilesENG[i];
                    }
                    else
                    {
                        t = Table.TownMapInfos[(int)TownMapIDBox.Value].TownMapTilesJPN[i];
                    }

                    if (YCorrection > t.YDisplayOffset) YCorrection = t.YDisplayOffset;
                    if (XCorrection > t.XDisplayOffset) XCorrection = t.XDisplayOffset;
                }

                YCorrection = -YCorrection;
                XCorrection = -XCorrection;

                for (int i = 0; i < TileAmount; i++)
                {
                    TownMapTile t;
                    if (English)
                    {
                        t = Table.TownMapInfos[(int)TownMapIDBox.Value].TownMapTilesENG[i];
                    }
                    else
                    {
                        t = Table.TownMapInfos[(int)TownMapIDBox.Value].TownMapTilesJPN[i];
                    }

                    for (int x = 0; x < t.XSize; x++)
                    {
                        for (int y = 0; y < t.YSize; y++)
                        {
                            if (img.GetPixel(t.XDisplayOffset + XCorrection + x, t.YDisplayOffset + YCorrection + y) != Color.FromArgb(0)
                             && img.GetPixel(t.XDisplayOffset + XCorrection + x, t.YDisplayOffset + YCorrection + y) != Color.FromArgb(0x00fff0d3)) continue;

                            img.SetPixel(t.XDisplayOffset + XCorrection + x, t.YDisplayOffset + YCorrection + y,
                                tex.GetPixel(t.XTextureOffset + x, t.YTextureOffset + y));
                        }
                    }
                }


                for (int x = 0; x < tile.XSize; x++)
                {
                    tex.SetPixel(tile.XTextureOffset + x, tile.YTextureOffset, Color.White);
                }
                for (int x = 0; x < tile.XSize; x++)
                {
                    tex.SetPixel(tile.XTextureOffset + x, tile.YTextureOffset + tile.YSize, Color.White);
                }
                for (int y = 0; y < tile.YSize; y++)
                {
                    tex.SetPixel(tile.XTextureOffset, tile.YTextureOffset + y, Color.White);
                }
                for (int y = 0; y < tile.YSize; y++)
                {
                    tex.SetPixel(tile.XTextureOffset + tile.XSize, tile.YTextureOffset + y, Color.White);
                }

                pictureBox1.Image = tex;
                pictureBox2.Image = img;


                

            }
            catch (Exception)
            {
                //throw;
            }
        }

        private void TownMapIDBox_ValueChanged(object sender, EventArgs e)
        {
            UpdateFields();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None) return;
            textBoxClickCoords.Text = (e.X - XCorrection).ToString("X") + " / " + (e.Y - YCorrection).ToString("X");
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None) return;
            textBoxClickCoordsTexture.Text = (e.X).ToString("X") + " / " + (e.Y).ToString("X");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Table = new TownMapTable( Table.Filepath );
            UpdateFields();
        }
    }
}
