using HyoutaTools.FileContainer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaPluginBase.FileContainer;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	public class SaveDataParser {
		public static int Parse( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: SaveDataParser savefile gamedatapath" );
				Console.WriteLine( "Save must be decrypted." );
				Console.WriteLine( "Game data path should point to a directory or other container with the game files, which is needed to parse things like item, title, and enemy data correctly." );
				return -1;
			}

			IContainer gameContainer = Website.GenerateWebsite.ContainerFromPath( args[1] );
			if ( gameContainer == null ) {
				Console.WriteLine( "Invalid game data path given." );
				return -1;
			}

			GameVersion? maybeVersion = Website.GenerateWebsite.GuessGameVersionFromContainer( gameContainer );
			if ( !maybeVersion.HasValue ) {
				Console.WriteLine( "Failed to determine game version from given data path." );
				return -1;
			}

			GameVersion version = maybeVersion.Value;
			IContainer gameDir = Website.GenerateWebsite.FindGameDataDirectory( gameContainer, version );
			if ( gameDir == null ) {
				Console.WriteLine( "Failed to find correct file container -- is your game dump incomplete?" );
				return -1;
			}

			GameLocale locale = VesperiaUtil.GetValidLocales( version ).First();
			Util.Endianness endian = VesperiaUtil.GetEndian( version );
			Util.GameTextEncoding encoding = VesperiaUtil.GetEncoding( version );
			Util.Bitness bits = VesperiaUtil.GetBitness( version );

			TSS.TSSFile stringDic = new TSS.TSSFile( Website.GenerateWebsite.TryGetStringDic( gameDir, locale, version ), encoding, endian );
			Dictionary<uint, TSS.TSSEntry> inGameDic = stringDic.GenerateInGameIdDictionary();
			ItemDat.ItemDat itemData = new ItemDat.ItemDat( Website.GenerateWebsite.TryGetItemDat( gameDir, locale, version ), Website.GenerateWebsite.TryGetItemSortDat( gameDir, locale, version ), Util.Endianness.BigEndian );
			List<ItemDat.ItemDatSingle> itemDataSorted = itemData.GetSortedByInGameSorting();
			FAMEDAT.FAMEDAT titles = new FAMEDAT.FAMEDAT( Website.GenerateWebsite.TryGetTitles( gameDir, locale, version ), endian );
			T8BTEMST.T8BTEMST enemies = new T8BTEMST.T8BTEMST( Website.GenerateWebsite.TryGetEnemies( gameDir, locale, version ), endian, bits );

			using ( Streams.DuplicatableFileStream file = new Streams.DuplicatableFileStream( args[0] ) ) {
				var savedata = new SaveData( file, endian );
				savedata.SavePoint.PrintData();
				savedata.PartyData.PrintData( endian, inGameDic, itemDataSorted, enemies );
				foreach ( var characterBlock in savedata.CharacterData ) {
					characterBlock.PrintData( endian, version, inGameDic, titles );
					Console.WriteLine( "=====" );
				}

				//savedata.ExportTo( args[0] + ".ext" );
			}

			return 0;
		}
	}
}
