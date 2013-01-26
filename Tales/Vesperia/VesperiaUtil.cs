using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HyoutaTools.Tales.Vesperia {
	class VesperiaUtil {
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

		private static String[][] InsaneNames = null;
		public static String ReplaceWithInsaneNames( String input ) {
			if ( String.IsNullOrEmpty( input ) ) return input;

			if ( InsaneNames == null ) {
				InsaneNames = new String[][] {
					new String[] { "Commandant Alexei", "Emperor Peony Wesker-Dumbledore" } ,
					new String[] { "commandant Alexei", "Emperor Peony Wesker-Dumbledore" } ,
					new String[] { "Commandant\nAlexei", "Emperor Peony\nWesker-Dumbledore" } ,
					new String[] { "commandant\nAlexei", "Emperor Peony\nWesker-Dumbledore" } ,
					new String[] { "Commandant \nAlexei", "Emperor Peony\nWesker-Dumbledore" } ,
					new String[] { "commandant \nAlexei", "Emperor Peony\nWesker-Dumbledore" } ,
					new String[] { "Alexei", "Wesker-Dumbledore" } ,
					new String[] { "Zaude", "Hogwarts" } ,
					new String[] { "Ioder", "Yoda" } ,
					new String[] { "Repede", "MC Weihnachtsdog" } ,
					new String[] { "Tricorne", "Partyhut" } ,
					new String[] { "Technical Diver", "Beach Party Outfit" } ,
					new String[] { "Fireball", "Imperial Fireball" } ,
					new String[] { "fireball", "Imperial Fireball" } ,
					new String[] { "Fire ball", "Imperial Fireball" } ,
					new String[] { "Fire Ball", "Imperial Fireball" } ,
					new String[] { "fire ball", "Imperial Fireball" } ,
					new String[] { "fire Ball", "Imperial Fireball" } ,
					new String[] { "Thunder Blade", "Handymasten" } ,
					new String[] { "Traitor to Heaven", "Tipp dir den Kick" } ,
					new String[] { "Traitor To Heaven", "Tipp dir den Kick" } ,
					new String[] { "Zagi", "Sarkli" } ,
				};
			}

			foreach ( String[] s in InsaneNames ) {
				input = input.Replace( s[0], s[1] );
			}
			return input;
		}

	}
}
