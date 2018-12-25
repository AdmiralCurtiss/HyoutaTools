using HyoutaTools.Trophy;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HyoutaLibGUI.Trophy.Viewer {
	public class CustomDrawListBox : ListBox {
		public bool IsGameList = false;
		public TrophyConfNode CurrentGame = null;

		public CustomDrawListBox() {
			this.DrawMode = DrawMode.OwnerDrawVariable; // We're using custom drawing.
			this.ItemHeight = 60;
		}

		protected override void OnDrawItem( DrawItemEventArgs e ) {
			// Make sure we're not trying to draw something that isn't there.
			if ( e.Index >= this.Items.Count || e.Index <= -1 )
				return;

			// Get the item object.
			object item = this.Items[e.Index];
			if ( item == null )
				return;


			// Draw the item.
			if ( IsGameList ) {
				if ( ( e.State & DrawItemState.Selected ) == DrawItemState.Selected ) {
					e.Graphics.FillRectangle( new SolidBrush( Color.LightBlue ), e.Bounds );
				} else {
					e.Graphics.FillRectangle( new SolidBrush( Color.White ), e.Bounds );
				}

				TrophyConfNode tc = (TrophyConfNode)item;
				e.Graphics.DrawImage( tc.GameThumbnail, 0, e.Bounds.Y, 160, 88 );

				string text = tc.TitleName;
				SizeF stringSize = e.Graphics.MeasureString( text, this.Font );
				e.Graphics.DrawString( text, this.Font, new SolidBrush( Color.Black ),
					new PointF( 163, e.Bounds.Y + ( e.Bounds.Height - stringSize.Height ) / 2 ) );
			} else {
				TrophyNode t = (TrophyNode)item;

				if ( ( e.State & DrawItemState.Selected ) == DrawItemState.Selected ) {
					e.Graphics.FillRectangle( new SolidBrush( Color.LightBlue ), e.Bounds );
				} else {
					if ( CurrentGame.TropUsrFile.TrophyInfos[UInt32.Parse( t.ID )].Unlocked == 1 ) {
						e.Graphics.FillRectangle( new SolidBrush( Color.LightGreen ), e.Bounds );
					} else {
						e.Graphics.FillRectangle( new SolidBrush( Color.White ), e.Bounds );
					}
				}

				e.Graphics.DrawImage( t.TrophyThumbnail, 0, e.Bounds.Y, 60, 60 );

				string text = item.ToString();
				SizeF stringSize = e.Graphics.MeasureString( text, this.Font );
				e.Graphics.DrawString( text, this.Font, new SolidBrush( Color.Black ),
					new PointF( 63, e.Bounds.Y + ( e.Bounds.Height - stringSize.Height ) / 2 ) );
			}
		}
	}
}
