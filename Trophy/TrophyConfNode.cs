using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HyoutaTools.Trophy.Viewer;

namespace HyoutaTools.Trophy {
	public class TrophyConfNode {
		public String SceNpTrophySignature;
		public String SceNpTrophySignature_TropConf;
		public String version;

		public String npcommid;
		public String trophysetversion;
		public String parentallevel;
		public String parentallevel_licensearea;
		public String TitleName;
		public String TitleDetail;

		public Dictionary<uint, TrophyNode> Trophies;

		public String Folder;

		public TropUsr TropUsrFile;


		private System.Drawing.Image _GameImage = null;
		private System.Drawing.Image _GameThumbnail = null;

		public System.Drawing.Image GameImage {
			get {
				if ( _GameImage == null ) LoadImage();
				return _GameImage;
			}
		}
		public System.Drawing.Image GameThumbnail {
			get {
				if ( _GameThumbnail == null ) LoadThumbnail();
				return _GameThumbnail;
			}
		}

		private void LoadImage() {
			String pngname = "ICON0.PNG";
			_GameImage = System.Drawing.Image.FromFile( Folder + pngname ); // 320x176
		}

		private void LoadThumbnail() {
			String pngname = "ICON0.PNG";
			_GameThumbnail = System.Drawing.Image.FromFile( Folder + pngname ).GetThumbnailImage( 160, 88, delegate { return false; }, System.IntPtr.Zero );
		}


		public TrophyConfNode( XmlNode Node, String TrophySignature, String TrophyConfigSignature, String Folder ) {
			SceNpTrophySignature = TrophySignature;
			SceNpTrophySignature_TropConf = TrophyConfigSignature;

			version = Node.Attributes["version"].Value;

			npcommid = Node["npcommid"].InnerText;
			trophysetversion = Node["trophyset-version"].InnerText;
			parentallevel = Node["parental-level"].InnerText;
			parentallevel_licensearea = Node["parental-level"].Attributes["license-area"].Value;
			TitleName = Node["title-name"].InnerText;
			TitleDetail = Node["title-detail"].InnerText;
			this.Folder = Folder;

			XmlNodeList TrophyNodes = Node.SelectNodes( "trophy" );

			if ( Folder != null ) {
				TropUsrFile = new TropUsr( System.IO.File.ReadAllBytes( Folder + "TROPUSR.DAT" ) );
			}

			Trophies = new Dictionary<uint, TrophyNode>();
			foreach ( XmlNode Trophy in TrophyNodes ) {
				TrophyNode t = new TrophyNode( Trophy, Folder );
				Trophies.Add( UInt32.Parse( t.ID ), t );
			}
		}

		public TrophyConfNode( String SceNpTrophySignature, String SceNpTrophySignature_TropConf, String version, String npcommid, String trophysetversion,
							  String parentallevel, String parentallevel_licensearea, String TitleName, String TitleDetail, String Folder ) {
			this.SceNpTrophySignature = SceNpTrophySignature;
			this.SceNpTrophySignature_TropConf = SceNpTrophySignature_TropConf;
			this.version = version;
			this.npcommid = npcommid;
			this.trophysetversion = trophysetversion;
			this.parentallevel = parentallevel;
			this.parentallevel_licensearea = parentallevel_licensearea;
			this.TitleName = TitleName;
			this.TitleDetail = TitleDetail;
			this.Folder = Folder;
		}

		public void SortBy( Comparison<TropUsrSingleTrophy> SortType, bool Descending ) {
			List<TropUsrSingleTrophy> TrophyInfoList = new List<TropUsrSingleTrophy>( TropUsrFile.TrophyInfos.Values.Count );
			foreach ( TropUsrSingleTrophy t in TropUsrFile.TrophyInfos.Values ) {
				TrophyInfoList.Add( t );
			}

			TrophyInfoList.Sort( SortType );

			Dictionary<uint, TropUsrSingleTrophy> NewDict = new Dictionary<uint, TropUsrSingleTrophy>( TropUsrFile.TrophyInfos.Count );
			if ( Descending ) {
				for ( int i = TrophyInfoList.Count - 1; i >= 0; i-- ) NewDict.Add( TrophyInfoList[i].TrophyID, TrophyInfoList[i] );
			} else {
				foreach ( TropUsrSingleTrophy t in TrophyInfoList ) NewDict.Add( t.TrophyID, t );
			}
			TropUsrFile.TrophyInfos = NewDict;

			Dictionary<uint, TrophyNode> TrophyNodeNewDict = new Dictionary<uint, TrophyNode>( NewDict.Count );
			foreach ( uint key in NewDict.Keys ) {
				TrophyNodeNewDict.Add( key, Trophies[key] );
			}
			Trophies = TrophyNodeNewDict;
		}

		public void SortByUnlockedBeforeLocked( Comparison<TropUsrSingleTrophy> SortType, bool DescendingUnlocked, bool DescendingLocked, bool SortLockedByID ) {
			List<TropUsrSingleTrophy> TrophyInfoListLocked = new List<TropUsrSingleTrophy>( TropUsrFile.TrophyInfos.Values.Count );
			List<TropUsrSingleTrophy> TrophyInfoListUnlocked = new List<TropUsrSingleTrophy>( TropUsrFile.TrophyInfos.Values.Count );
			foreach ( TropUsrSingleTrophy t in TropUsrFile.TrophyInfos.Values ) {
				if ( t.Unlocked == 1 ) {
					TrophyInfoListUnlocked.Add( t );
				} else {
					TrophyInfoListLocked.Add( t );
				}
			}

			if ( SortLockedByID ) {
				TrophyInfoListLocked.Sort( TropUsrSingleTrophy.SortByTrophyID );
			} else {
				TrophyInfoListLocked.Sort( SortType );
			}
			TrophyInfoListUnlocked.Sort( SortType );

			Dictionary<uint, TropUsrSingleTrophy> NewDict = new Dictionary<uint, TropUsrSingleTrophy>( TropUsrFile.TrophyInfos.Count );
			if ( DescendingUnlocked ) {
				for ( int i = TrophyInfoListUnlocked.Count - 1; i >= 0; i-- ) NewDict.Add( TrophyInfoListUnlocked[i].TrophyID, TrophyInfoListUnlocked[i] );
			} else {
				foreach ( TropUsrSingleTrophy t in TrophyInfoListUnlocked ) NewDict.Add( t.TrophyID, t );
			}
			if ( DescendingLocked ) {
				for ( int i = TrophyInfoListLocked.Count - 1; i >= 0; i-- ) NewDict.Add( TrophyInfoListLocked[i].TrophyID, TrophyInfoListLocked[i] );
			} else {
				foreach ( TropUsrSingleTrophy t in TrophyInfoListLocked ) NewDict.Add( t.TrophyID, t );
			}
			TropUsrFile.TrophyInfos = NewDict;

			Dictionary<uint, TrophyNode> TrophyNodeNewDict = new Dictionary<uint, TrophyNode>( NewDict.Count );
			foreach ( uint key in NewDict.Keys ) {
				TrophyNodeNewDict.Add( key, Trophies[key] );
			}
			Trophies = TrophyNodeNewDict;
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

			foreach ( TrophyNode Trophy in Trophies.Values ) {
				sb.Append( Trophy.ExportTropSFM( TropConf ) );
			}

			sb.Append( "</trophyconf>\n" );

			return sb.ToString();
		}

		public static TrophyConfNode ReadTropSfmWithTropConfFromString( String XMLFile, String TropConf ) {
			String Signature;
			try {
				Signature = XMLFile.Substring( XMLFile.IndexOf( "<!--Sce-Np-Trophy-Signature: " ) + "<!--Sce-Np-Trophy-Signature: ".Length );
				Signature = Signature.Substring( 0, Signature.IndexOf( "-->" ) );
			} catch ( Exception ) {
				Signature = "";
			}

			String SignatureTropConf;
			try {
				SignatureTropConf = TropConf.Substring( TropConf.IndexOf( "<!--Sce-Np-Trophy-Signature: " ) + "<!--Sce-Np-Trophy-Signature: ".Length );
				SignatureTropConf = SignatureTropConf.Substring( 0, SignatureTropConf.IndexOf( "-->" ) );
			} catch ( Exception ) {
				SignatureTropConf = "";
			}

			XmlDocument doc = new XmlDocument();
			doc.LoadXml( XMLFile );

			XmlElement root = doc.DocumentElement;
			return new TrophyConfNode( root, Signature, SignatureTropConf, null );
		}

		public static TrophyConfNode ReadTropSfmWithTropConf( String Filename, String FilenameTropConf ) {
			return ReadTropSfmWithTropConfFromString( System.IO.File.ReadAllText( Filename, Encoding.UTF8 ), System.IO.File.ReadAllText( FilenameTropConf, Encoding.UTF8 ) );
		}

		public static TrophyConfNode ReadTropSfmWithTropConf( System.IO.Stream trop, System.IO.Stream tropconf ) {
			string t = trop.ReadSizedString( trop.Length - trop.Position, Util.GameTextEncoding.UTF8 );
			string c = tropconf.ReadSizedString( tropconf.Length - tropconf.Position, Util.GameTextEncoding.UTF8 );
			return ReadTropSfmWithTropConfFromString( t, c );
		}

		public static TrophyConfNode ReadTropSfmWithFolder( String Folder, String Filename ) {
			if ( !Folder.EndsWith( "/" ) && !Folder.EndsWith( "\\" ) ) {
				Folder = Folder + "/";
			}
			String XMLFile = System.IO.File.ReadAllText( Folder + Filename, Encoding.UTF8 );
			XMLFile = XMLFile.Substring( 0x40 ).TrimEnd( '\0' );
			String Signature;
			try {
				Signature = XMLFile.Substring( XMLFile.IndexOf( "<!--Sce-Np-Trophy-Signature: " ) + "<!--Sce-Np-Trophy-Signature: ".Length );
				Signature = Signature.Substring( 0, Signature.IndexOf( "-->" ) );
			} catch ( Exception ) {
				Signature = "";
			}

			XmlDocument doc = new XmlDocument();
			doc.LoadXml( XMLFile );

			XmlElement root = doc.DocumentElement;
			return new TrophyConfNode( root, Signature, Signature, Folder );
		}


	}
}
