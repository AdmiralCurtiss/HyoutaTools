using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TSS {
	static class Util {
		private static String[][] InsaneNames = new String[][] {
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

		public static String ReplaceWithInsaneNames( String input ) {
			if ( String.IsNullOrEmpty( input ) ) return input;

			foreach ( String[] s in InsaneNames ) {
				input = input.Replace( s[0], s[1] );
			}
			return input;
		}
	}
}
