using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HyoutaTools.Trophy {
	static class Util {
		public static Encoding ShiftJISEncoding = Encoding.GetEncoding( "shift-jis" );

		public static UInt32 SwapEndian( UInt32 x ) {
			return x = ( x >> 24 ) |
					  ( ( x << 8 ) & 0x00FF0000 ) |
					  ( ( x >> 8 ) & 0x0000FF00 ) |
					   ( x << 24 );
		}

		public static UInt64 SwapEndian( UInt64 x ) {
			return x = ( x >> 56 ) |
						( ( x << 40 ) & 0x00FF000000000000 ) |
						( ( x << 24 ) & 0x0000FF0000000000 ) |
						( ( x << 8 ) & 0x000000FF00000000 ) |
						( ( x >> 8 ) & 0x00000000FF000000 ) |
						( ( x >> 24 ) & 0x0000000000FF0000 ) |
						( ( x >> 40 ) & 0x000000000000FF00 ) |
						 ( x << 56 );
		}



		public static byte[] StringToBytes( String s ) {
			//byte[] bytes = ShiftJISEncoding.GetBytes(s);
			//return bytes.TakeWhile(subject => subject != 0x00).ToArray();
			return ShiftJISEncoding.GetBytes( s );
		}

		public static void DisplayException( Exception e ) {
			Console.WriteLine( "Exception occurred:" );
			Console.WriteLine( e.Message );
		}

		public static String GetText( int Pointer, byte[] File ) {
			if ( Pointer == -1 ) return null;

			try {
				int i = Pointer;
				while ( File[i] != 0x00 ) {
					i++;
				}
				String Text = Util.ShiftJISEncoding.GetString( File, Pointer, i - Pointer );
				//String Text = Util.ShiftJISEncoding.GetString(File, Pointer, 0x400);
				return Text;
			} catch ( Exception ) {
				return null;
			}
		}


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
