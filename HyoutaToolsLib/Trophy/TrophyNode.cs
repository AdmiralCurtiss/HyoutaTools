using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HyoutaUtils.Image2D;

namespace HyoutaTools.Trophy {
	public class TrophyNode : IComparable {
		public String ID;
		public bool Hidden;
		public String TType;
		public String PID;

		public String Name;
		public String Detail;

		public String Folder;

		private Bitmap _TrophyImage = null;
		private Bitmap _TrophyThumbnail = null;

		public Bitmap TrophyImage {
			get {
				if ( _TrophyImage == null ) LoadImage();
				return _TrophyImage;
			}
		}
		public Bitmap TrophyThumbnail {
			get {
				if ( _TrophyThumbnail == null ) LoadThumbnail();
				return _TrophyThumbnail;
			}
		}

		private void LoadImage() {
			String pngname = "TROP" + this.ID + ".PNG ";
			_TrophyImage = Bitmap.ReadFromFile(Folder + pngname);
		}

		private void LoadThumbnail() {
			String pngname = "TROP" + this.ID + ".PNG ";
			_TrophyThumbnail = Bitmap.ReadFromFile(Folder + pngname);
			_TrophyThumbnail.Scale(60, 60);
		}

		public TrophyNode( XmlNode Node, String Folder ) {
			ID = Node.Attributes["id"].Value;
			Hidden = Node.Attributes["hidden"].Value == "yes";
			TType = Node.Attributes["ttype"].Value;
			PID = Node.Attributes["pid"].Value;

			Name = Node["name"].InnerText;
			Detail = Node["detail"].InnerText;

			this.Folder = Folder;
		}

		public TrophyNode( String ID, bool Hidden, String TType, String PID, String Name, String Detail, String Folder ) {
			this.ID = ID;
			this.Hidden = Hidden;
			this.TType = TType;
			this.PID = PID;
			this.Name = Name;
			this.Detail = Detail;
			this.Folder = Folder;
		}

		public String ExportTropSFM( bool TropConf ) {
			StringBuilder sb = new StringBuilder();

			sb.Append( " <trophy id=\"" );
			sb.Append( ID );
			sb.Append( "\" hidden=\"" );
			if ( Hidden ) {
				sb.Append( "yes" );
			} else {
				sb.Append( "no" );
			}
			sb.Append( "\" ttype=\"" );
			sb.Append( TType );
			sb.Append( "\" pid=\"" );
			sb.Append( PID );

			if ( TropConf ) {
				sb.Append( "\"/>\n" );
			} else {
				sb.Append( "\">\n" );

				sb.Append( "  <name>" );
				sb.Append( Name );
				sb.Append( "</name>\n" );

				sb.Append( "  <detail>" );
				sb.Append( Detail.Replace( "\x0A", "&#x0a;" ) );
				sb.Append( "</detail>\n" );

				sb.Append( " </trophy>\n" );
			}

			return sb.ToString();
		}


		public override string ToString() {
			return this.Name;
		}


		#region IComparable Members

		public int CompareTo( object obj ) {
			TrophyNode t = (TrophyNode)obj;
			int CurrentID = Int32.Parse( this.ID );
			int OtherID = Int32.Parse( t.ID );

			return CurrentID - OtherID;
		}

		#endregion
	}
}
