using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Pokemon.Gen5.PWT {
	class Program {
		public static int ExecuteDownloadChecksumRecalc( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: input.bin output.bin" );
				return -1;
			}

			string inFilename = args[0];
			string outFilename = args[1];

			var downloadFile = new DownloadFile( inFilename );
			var outFile = downloadFile.GetFile();

			byte language = downloadFile.Tournaments[0].Language;
			int i = 0;
			foreach ( var t in downloadFile.Tournaments ) {
				++i;
				t.Language = language;
				Console.WriteLine( "Tournament #" + i + ", ID " + t.ID + ":" );
				Console.WriteLine( "Valid Language:     " + t.Language );

				Console.WriteLine( "Valid Versions:     " + t.Versions );
				Console.WriteLine( "Unknown byte 0x01:  " + t.Unknown0x01 );
				Console.WriteLine( "Unknown byte 0x02:  " + t.Unknown0x02 );
				Console.WriteLine( "Unknown byte 0x03:  " + t.Unknown0x03 );
				Console.WriteLine( "Unknown byte 0x04:  " + t.Unknown0x04 );
				Console.WriteLine( "Unknown byte 0x05:  " + t.Unknown0x05 );
				Console.WriteLine( "Unknown byte 0x06:  " + t.Unknown0x06 );

				Console.WriteLine( t.Name );
				Console.WriteLine( t.Description );
				Console.WriteLine( t.Rules );

				Console.WriteLine( "Minimum Pokemon:    " + t.PokemonCountMin );
				Console.WriteLine( "Maximum Pokemon:    " + t.PokemonCountMax );
				Console.WriteLine( "Enemy Level:        " + t.Level );
				Console.WriteLine( "Player Level:       " + t.LevelStyle );
				Console.WriteLine( "Player Level Total: " + t.LevelTotal );
				Console.WriteLine( "Allow dupl. Pkmn:   " + t.AllowDuplicatePokemon );
				Console.WriteLine( "Allow dupl. Items:  " + t.AllowDuplicateItems );
				Console.WriteLine( "Battle Style:       " + t.BattleStyle );
				Console.WriteLine( "Music Modifier:     " + t.MusicModifier );
				Console.WriteLine( "Allowed Types:      " + t.AllowedPokemonType );
				Console.WriteLine( "Banned Move:        " + t.BannedMove );

				Console.WriteLine();
			}

			System.IO.File.WriteAllBytes( outFilename, outFile );

			return 0;
		}
	}
}
