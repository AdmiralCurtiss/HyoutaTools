using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HyoutaTools.Trophy {
	class TrophyNode {
		public String ID;
		public bool Hidden;
		public String TType;
		public String PID;

		public String Name;
		public String Detail;

		public TrophyNode( XmlNode Node ) {
			ID = Node.Attributes["id"].Value;
			Hidden = Node.Attributes["hidden"].Value == "yes";
			TType = Node.Attributes["ttype"].Value;
			PID = Node.Attributes["pid"].Value;

			Name = Node["name"].InnerText;
			Detail = Node["detail"].InnerText;
		}

		public TrophyNode( String ID, bool Hidden, String TType, String PID, String Name, String Detail ) {
			this.ID = ID;
			this.Hidden = Hidden;
			this.TType = TType;
			this.PID = PID;
			this.Name = Name;
			this.Detail = Detail;
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
	}
}
