using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HyoutaTools.Tales.Vesperia.Font {
	static class Util {
		public static String Path = @"../../tov.elf";
		public static int FontInfoOffset = 0x00720860;

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
