using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HyoutaTools.Trophy {
	class TrophyConfNode {
		public String SceNpTrophySignature;
		public String SceNpTrophySignature_TropConf;
		public String version;

		public String npcommid;
		public String trophysetversion;
		public String parentallevel;
		public String parentallevel_licensearea;
		public String TitleName;
		public String TitleDetail;

		public List<TrophyNode> Trophies;

		public TrophyConfNode( XmlNode Node, String TrophySignature, String TrophyConfigSignature ) {
			SceNpTrophySignature = TrophySignature;
			SceNpTrophySignature_TropConf = TrophyConfigSignature;

			version = Node.Attributes["version"].Value;

			npcommid = Node["npcommid"].InnerText;
			trophysetversion = Node["trophyset-version"].InnerText;
			parentallevel = Node["parental-level"].InnerText;
			parentallevel_licensearea = Node["parental-level"].Attributes["license-area"].Value;
			TitleName = Node["title-name"].InnerText;
			TitleDetail = Node["title-detail"].InnerText;

			XmlNodeList TrophyNodes = Node.SelectNodes( "trophy" );

			Trophies = new List<TrophyNode>();
			foreach ( XmlNode Trophy in TrophyNodes ) {
				Trophies.Add( new TrophyNode( Trophy ) );
			}
		}

		public TrophyConfNode( String SceNpTrophySignature, String SceNpTrophySignature_TropConf, String version, String npcommid, String trophysetversion,
							  String parentallevel, String parentallevel_licensearea, String TitleName, String TitleDetail ) {
			this.SceNpTrophySignature = SceNpTrophySignature;
			this.SceNpTrophySignature_TropConf = SceNpTrophySignature_TropConf;
			this.version = version;
			this.npcommid = npcommid;
			this.trophysetversion = trophysetversion;
			this.parentallevel = parentallevel;
			this.parentallevel_licensearea = parentallevel_licensearea;
			this.TitleName = TitleName;
			this.TitleDetail = TitleDetail;
		}

		public String ExportTropSFM( bool TropConf ) {
			StringBuilder sb = new StringBuilder();

			sb.Append( "<!--Sce-Np-Trophy-Signature: " );
			if ( TropConf ) {
				sb.Append( SceNpTrophySignature_TropConf );
			} else {
				sb.Append( SceNpTrophySignature );
			}
			sb.Append( "-->\n" );

			sb.Append( "<trophyconf version=\"" );
			sb.Append( version );
			sb.Append( "\">\n" );

			sb.Append( " <npcommid>" );
			sb.Append( npcommid );
			sb.Append( "</npcommid>\n" );

			sb.Append( " <trophyset-version>" );
			sb.Append( trophysetversion );
			sb.Append( "</trophyset-version>\n" );

			sb.Append( " <parental-level license-area=\"" );
			sb.Append( parentallevel_licensearea );
			sb.Append( "\">" );
			sb.Append( parentallevel );
			sb.Append( "</parental-level>\n" );

			if ( !TropConf ) {
				sb.Append( " <title-name>" );
				sb.Append( TitleName );
				sb.Append( "</title-name>\n" );

				sb.Append( " <title-detail>" );
				sb.Append( TitleDetail );
				sb.Append( "</title-detail>\n" );
			}

			foreach ( TrophyNode Trophy in Trophies ) {
				sb.Append( Trophy.ExportTropSFM( TropConf ) );
			}

			sb.Append( "</trophyconf>\n" );

			return sb.ToString();
		}
	}
}