using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HyoutaTools.DanganRonpa.Font {
	static class Util {
		public static String Path = @"../../tov.elf";
		public static int FontInfoOffset = 0x00720860;

		public static String GetText( byte[] File, int Pointer ) {
			if ( Pointer == -1 ) return null;

			try {
				int i = Pointer;
				while ( File[i] != 0x00 ) {
					i++;
				}
				String Text = Encoding.ASCII.GetString( File, Pointer, i - Pointer );
				return Text;
			} catch ( Exception ) {
				return null;
			}
		}

		public static UInt32 SwapEndian( UInt32 x ) {
			return x = ( x >> 24 ) |
					  ( ( x << 8 ) & 0x00FF0000 ) |
					  ( ( x >> 8 ) & 0x0000FF00 ) |
					   ( x << 24 );
		}

		public static Int32 SwapEndian( Int32 y ) {
			UInt32 x = (uint)y;
			x = ( x >> 24 ) |
			  ( ( x << 8 ) & 0x00FF0000 ) |
			  ( ( x >> 8 ) & 0x0000FF00 ) |
			   ( x << 24 );
			return (int)x;
		}

		public static void DisplayException( Exception e ) {
			Console.WriteLine( "Exception occurred:\n" + e.Message );
		}

		public static String RemoveTags( String s ) {
			s = s.Replace( "''", "'" );
			s = s.Replace( "(YUR)", "Yuri" );
			s = s.Replace( "(EST)", "Estellise" );
			s = s.Replace( "(EST_P)", "Estelle" );
			s = s.Replace( "(RIT)", "Rita" );
			s = s.Replace( "(KAR)", "Karol" );
			s = s.Replace( "(RAP)", "Repede" );
			s = s.Replace( "(RAV)", "Raven" );
			s = s.Replace( "(JUD)", "Judith" );
			s = s.Replace( "(FRE)", "Flynn" );
			s = s.Replace( "(PAT)", "Patty" );
			s = s.Replace( "(JUD_P)", "Judy" );
			s = s.Replace( "(BAU)", "Ba'ul" );
			s = s.Replace( ""/*0xFF*/, "\n\n" );
			s = s.Replace( "\r", "" ); // technically I think \r is used for Furigana but I haven't bothered displaying those properly.
			s = Regex.Replace( s, "\t[(][A-Za-z0-9_]+[)]", "" ); // audio/voice commands
			s = Regex.Replace( s, "[(][0-9]+[)]", "" ); // color commands
			return s;
		}

	}
}
