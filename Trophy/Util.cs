using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HyoutaTools.Trophy {
	static class Util {
		public static Encoding ShiftJISEncoding = Encoding.GetEncoding( "shift-jis" );

		public static TrophyConfNode ReadTropSfmWithTropConf( String Filename, String FilenameTropConf ) {
			String XMLFile = System.IO.File.ReadAllText( Filename, Encoding.UTF8 );
			String Signature;
			try {
				Signature = XMLFile.Substring( XMLFile.IndexOf( "<!--Sce-Np-Trophy-Signature: " ) + "<!--Sce-Np-Trophy-Signature: ".Length );
				Signature = Signature.Substring( 0, Signature.IndexOf( "-->" ) );
			} catch ( Exception ) {
				Signature = "";
			}

			String SignatureTropConf;
			try {
				String TropConf = System.IO.File.ReadAllText( FilenameTropConf, Encoding.UTF8 );
				SignatureTropConf = TropConf.Substring( TropConf.IndexOf( "<!--Sce-Np-Trophy-Signature: " ) + "<!--Sce-Np-Trophy-Signature: ".Length );
				SignatureTropConf = SignatureTropConf.Substring( 0, SignatureTropConf.IndexOf( "-->" ) );
			} catch ( Exception ) {
				SignatureTropConf = "";
			}

			XmlDocument doc = new XmlDocument();
			doc.Load( Filename );

			XmlElement root = doc.DocumentElement;
			return new TrophyConfNode( root, Signature, SignatureTropConf, null );
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


		public static DateTime UnixTimeToDateTime( uint unixTime ) {
			return new DateTime( 1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc ).AddSeconds( unixTime ).ToLocalTime();
		}

		internal static object PS3TimeToDateTime( ulong PS3Time ) {
			return new DateTime( 1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc ).AddMilliseconds( PS3Time / 1000 ).ToLocalTime();
		}
	}
}
