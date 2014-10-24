using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.ItemDat;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.Website {
	public class Setting {
		public uint NameStringDicId;
		public uint DescStringDicId;
		public uint[] OptionsStringDicIds;

		public Setting( uint idName, uint idDesc, uint option1 = 0, uint option2 = 0, uint option3 = 0, uint option4 = 0 ) {
			NameStringDicId = idName;
			DescStringDicId = idDesc;
			OptionsStringDicIds = new uint[4];
			OptionsStringDicIds[0] = option1;
			OptionsStringDicIds[1] = option2;
			OptionsStringDicIds[2] = option3;
			OptionsStringDicIds[3] = option4;
		}
	}

	public class GenerateWebsite {
		public static int Generate( List<string> args ) {
			string dir = @"d:\Dropbox\ToV\website\";
			string databasePath;

			Console.WriteLine( "Initializing 360" );

			var site = new GenerateWebsite();

			site.Version = GameVersion.X360;
			site.Items = new ItemDat.ItemDat( @"d:\Dropbox\ToV\360\item.svo.ext\ITEM.DAT" );
			site.StringDic = new TSS.TSSFile( System.IO.File.ReadAllBytes( @"d:\Dropbox\ToV\360\string_dic_uk.so" ), true );
			site.Artes = new T8BTMA.T8BTMA( @"d:\Dropbox\ToV\360\btl.svo.ext\BTL_PACK_UK.DAT.ext\0004.ext\ALL.0000" );
			site.Skills = new T8BTSK.T8BTSK( @"d:\Dropbox\ToV\360\btl.svo.ext\BTL_PACK_UK.DAT.ext\0010.ext\ALL.0000" );
			site.Enemies = new T8BTEMST.T8BTEMST( @"d:\Dropbox\ToV\360\btl.svo.ext\BTL_PACK_UK.DAT.ext\0005.ext\ALL.0000" );
			site.EnemyGroups = new T8BTEMGP.T8BTEMGP( @"d:\Dropbox\ToV\360\btl.svo.ext\BTL_PACK_UK.DAT.ext\0006.ext\ALL.0000" );
			site.EncounterGroups = new T8BTEMEG.T8BTEMEG( @"d:\Dropbox\ToV\360\btl.svo.ext\BTL_PACK_UK.DAT.ext\0007.ext\ALL.0000" );
			site.Recipes = new COOKDAT.COOKDAT( @"d:\Dropbox\ToV\360\cook.svo.ext\COOKDATA.BIN" );
			site.Locations = new WRLDDAT.WRLDDAT( @"d:\Dropbox\ToV\360\menu.svo.ext\WORLDDATA.BIN" );
			site.Synopsis = new SYNPDAT.SYNPDAT( @"d:\Dropbox\ToV\360\menu.svo.ext\SYNOPSISDATA.BIN" );
			site.Titles = new FAMEDAT.FAMEDAT( @"d:\Dropbox\ToV\360\menu.svo.ext\FAMEDATA.BIN" );
			site.GradeShop = new T8BTGR.T8BTGR( @"d:\Dropbox\ToV\360\btl.svo.ext\BTL_PACK_UK.DAT.ext\0016.ext\ALL.0000" );
			site.BattleBook = new BTLBDAT.BTLBDAT( @"d:\Dropbox\ToV\360\menu.svo.ext\BATTLEBOOKDATA.BIN" );
			site.Strategy = new T8BTTA.T8BTTA( @"d:\Dropbox\ToV\360\btl.svo.ext\BTL_PACK_UK.DAT.ext\0011.ext\ALL.0000" );
			site.Skits = new TO8CHLI.TO8CHLI( @"d:\Dropbox\ToV\360\chat.svo.ext\CHAT.DAT.dec" );
			site.Shops = new ShopData.ShopData( @"d:\Dropbox\ToV\360\scenario0", 0x1A780, 0x420 / 32, 0x8F8, 0x13780 / 56 );
			site.InGameIdDict = site.StringDic.GenerateInGameIdDictionary();
			site.IconsWithItems = new uint[] { 35, 36, 37, 60, 38, 1, 4, 12, 6, 5, 13, 14, 15, 7, 52, 51, 9, 16, 18, 2, 17, 19, 10, 20, 21, 22, 23, 24, 25, 26, 27, 56, 30, 28, 32, 31, 33, 29, 34, 41, 42, 43, 44, 45, 57, 61, 63, 39, 3, 40 };
			site.Records = site.GenerateRecordsStringDicList();
			site.Settings = site.GenerateSettingsStringDicList();

			// copy over Japanese stuff into UK StringDic
			var StringDicUs = new TSS.TSSFile( System.IO.File.ReadAllBytes( @"d:\Dropbox\ToV\360\string_dic_us.so" ), true );
			var IdDictUs = StringDicUs.GenerateInGameIdDictionary();
			foreach ( var kvp in IdDictUs ) {
				site.InGameIdDict[kvp.Key].StringJpn = kvp.Value.StringJpn;
			}

			databasePath = Path.Combine( dir, "_db-" + site.Version + ".sqlite" );
			System.IO.File.Delete( databasePath );
			new GenerateDatabase( site, new System.Data.SQLite.SQLiteConnection( "Data Source=" + databasePath ) ).ExportAll();

			System.IO.File.WriteAllText( Path.Combine( dir, "items-" + site.Version + ".html" ), site.GenerateHtmlItems(), Encoding.UTF8 );
			foreach ( uint i in site.IconsWithItems ) {
				System.IO.File.WriteAllText( Path.Combine( dir, "items-i" + i + "-" + site.Version + ".html" ), site.GenerateHtmlItems( icon: i ), Encoding.UTF8 );
			}
			for ( uint i = 2; i < 12; ++i ) {
				System.IO.File.WriteAllText( Path.Combine( dir, "items-c" + i + "-" + site.Version + ".html" ), site.GenerateHtmlItems( category: i ), Encoding.UTF8 );
			}
			System.IO.File.WriteAllText( Path.Combine( dir, "enemies-" + site.Version + ".html" ), site.GenerateHtmlEnemies(), Encoding.UTF8 );
			for ( int i = 0; i < 9; ++i ) {
				System.IO.File.WriteAllText( Path.Combine( dir, "enemies-c" + i + "-" + site.Version + ".html" ), site.GenerateHtmlEnemies( category: i ), Encoding.UTF8 );
			}
			System.IO.File.WriteAllText( Path.Combine( dir, "enemygroups-" + site.Version + ".html" ), site.GenerateHtmlEnemyGroups(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "encountergroups-" + site.Version + ".html" ), site.GenerateHtmlEncounterGroups(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "skills-" + site.Version + ".html" ), site.GenerateHtmlSkills(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "artes-" + site.Version + ".html" ), site.GenerateHtmlArtes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "synopsis-" + site.Version + ".html" ), site.GenerateHtmlSynopsis(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "recipes-" + site.Version + ".html" ), site.GenerateHtmlRecipes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "locations-" + site.Version + ".html" ), site.GenerateHtmlLocations(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "strategy-" + site.Version + ".html" ), site.GenerateHtmlStrategy(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "shops-" + site.Version + ".html" ), site.GenerateHtmlShops(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "titles-" + site.Version + ".html" ), site.GenerateHtmlTitles(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "battlebook-" + site.Version + ".html" ), site.GenerateHtmlBattleBook(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "records-" + site.Version + ".html" ), site.GenerateHtmlRecords(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "settings-" + site.Version + ".html" ), site.GenerateHtmlSettings(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "gradeshop-" + site.Version + ".html" ), site.GenerateHtmlGradeShop(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "skits-" + site.Version + ".html" ), site.GenerateHtmlSkitInfo(), Encoding.UTF8 );

			Console.WriteLine( "Initializing PS3" );

			site.Version = GameVersion.PS3;
			var PS3StringDic = new TSS.TSSFile( System.IO.File.ReadAllBytes( @"d:\Dropbox\ToV\PS3\mod\string.svo.ext\STRING_DIC.SO" ) );
			site.StringDic = PS3StringDic;
			site.Items = new ItemDat.ItemDat( @"d:\Dropbox\ToV\PS3\orig\item.svo.ext\ITEM.DAT" );
			site.Artes = new T8BTMA.T8BTMA( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0004.ext\ALL.0000" );
			site.Skills = new T8BTSK.T8BTSK( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0010.ext\ALL.0000" );
			site.Enemies = new T8BTEMST.T8BTEMST( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0005.ext\ALL.0000" );
			site.EnemyGroups = new T8BTEMGP.T8BTEMGP( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0006.ext\ALL.0000" );
			site.EncounterGroups = new T8BTEMEG.T8BTEMEG( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0007.ext\ALL.0000" );
			site.Recipes = new COOKDAT.COOKDAT( @"d:\Dropbox\ToV\PS3\orig\menu.svo.ext\COOKDATA.BIN" );
			site.Locations = new WRLDDAT.WRLDDAT( @"d:\Dropbox\ToV\PS3\orig\menu.svo.ext\WORLDDATA.BIN" );
			site.Synopsis = new SYNPDAT.SYNPDAT( @"d:\Dropbox\ToV\PS3\orig\menu.svo.ext\SYNOPSISDATA.BIN" );
			site.Titles = new FAMEDAT.FAMEDAT( @"d:\Dropbox\ToV\PS3\orig\menu.svo.ext\FAMEDATA.BIN" );
			site.GradeShop = new T8BTGR.T8BTGR( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0016.ext\ALL.0000" );
			site.BattleBook = new BTLBDAT.BTLBDAT( @"d:\Dropbox\ToV\PS3\orig\menu.svo.ext\BATTLEBOOKDATA.BIN" );
			site.Strategy = new T8BTTA.T8BTTA( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0011.ext\ALL.0000" );
			site.Skits = new TO8CHLI.TO8CHLI( @"d:\Dropbox\ToV\PS3\orig\chat.svo.ext\CHAT.DAT.dec" );
			site.Shops = new ShopData.ShopData( @"d:\Dropbox\ToV\PS3\mod\scenario0", 0x1C9BC, 0x460 / 32, 0x980, 0x14CB8 / 56 );
			site.NecropolisFloors = new T8BTXTM.T8BTXTMA( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0021.ext\ALL.0000" );
			site.NecropolisTreasures = new T8BTXTM.T8BTXTMT( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0022.ext\ALL.0000" );
			var filenames = System.IO.Directory.GetFiles( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0023.ext\" );
			site.NecropolisMaps = new Dictionary<string, T8BTXTM.T8BTXTMM>( filenames.Length );
			for ( int i = 0; i < filenames.Length; ++i ) {
				site.NecropolisMaps.Add( System.IO.Path.GetFileNameWithoutExtension( filenames[i] ), new T8BTXTM.T8BTXTMM( filenames[i] ) );
			}
			site.InGameIdDict = site.StringDic.GenerateInGameIdDictionary();
			site.IconsWithItems = new uint[] { 35, 36, 37, 60, 38, 1, 4, 12, 6, 5, 13, 14, 15, 7, 52, 51, 53, 9, 16, 18, 2, 17, 19, 10, 54, 20, 21, 22, 23, 24, 25, 26, 27, 56, 30, 28, 32, 31, 33, 29, 34, 41, 42, 43, 44, 45, 57, 61, 63, 39, 3, 40 };
			site.Records = site.GenerateRecordsStringDicList();
			site.Settings = site.GenerateSettingsStringDicList();

			databasePath = Path.Combine( dir, "_db-" + site.Version + ".sqlite" );
			System.IO.File.Delete( databasePath );
			new GenerateDatabase( site, new System.Data.SQLite.SQLiteConnection( "Data Source=" + databasePath ) ).ExportAll();

			System.IO.File.WriteAllText( Path.Combine( dir, "items-" + site.Version + ".html" ), site.GenerateHtmlItems(), Encoding.UTF8 );
			foreach ( uint i in site.IconsWithItems ) {
				System.IO.File.WriteAllText( Path.Combine( dir, "items-i" + i + "-" + site.Version + ".html" ), site.GenerateHtmlItems( icon: i ), Encoding.UTF8 );
			}
			for ( uint i = 2; i < 12; ++i ) {
				System.IO.File.WriteAllText( Path.Combine( dir, "items-c" + i + "-" + site.Version + ".html" ), site.GenerateHtmlItems( category: i ), Encoding.UTF8 );
			}
			System.IO.File.WriteAllText( Path.Combine( dir, "enemies-" + site.Version + ".html" ), site.GenerateHtmlEnemies(), Encoding.UTF8 );
			for ( int i = 0; i < 9; ++i ) {
				System.IO.File.WriteAllText( Path.Combine( dir, "enemies-c" + i + "-" + site.Version + ".html" ), site.GenerateHtmlEnemies( category: i ), Encoding.UTF8 );
			}
			System.IO.File.WriteAllText( Path.Combine( dir, "enemygroups-" + site.Version + ".html" ), site.GenerateHtmlEnemyGroups(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "encountergroups-" + site.Version + ".html" ), site.GenerateHtmlEncounterGroups(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "skills-" + site.Version + ".html" ), site.GenerateHtmlSkills(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "artes-" + site.Version + ".html" ), site.GenerateHtmlArtes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "synopsis-" + site.Version + ".html" ), site.GenerateHtmlSynopsis(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "recipes-" + site.Version + ".html" ), site.GenerateHtmlRecipes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "locations-" + site.Version + ".html" ), site.GenerateHtmlLocations(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "strategy-" + site.Version + ".html" ), site.GenerateHtmlStrategy(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "shops-" + site.Version + ".html" ), site.GenerateHtmlShops(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "titles-" + site.Version + ".html" ), site.GenerateHtmlTitles(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "battlebook-" + site.Version + ".html" ), site.GenerateHtmlBattleBook(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "records-" + site.Version + ".html" ), site.GenerateHtmlRecords(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "settings-" + site.Version + ".html" ), site.GenerateHtmlSettings(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "gradeshop-" + site.Version + ".html" ), site.GenerateHtmlGradeShop(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "skits-" + site.Version + ".html" ), site.GenerateHtmlSkitInfo(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "necropolis-" + site.Version + ".html" ), site.GenerateHtmlNecropolis( false ), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "necropolis-enemies-" + site.Version + ".html" ), site.GenerateHtmlNecropolis( true ), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "npc-" + site.Version + ".html" ), site.GenerateHtmlNpc(), Encoding.UTF8 );

			return 0;
		}

		public GameVersion Version;
		public ItemDat.ItemDat Items;
		public TSS.TSSFile StringDic;
		public T8BTMA.T8BTMA Artes;
		public T8BTSK.T8BTSK Skills;
		public T8BTEMST.T8BTEMST Enemies;
		public T8BTEMGP.T8BTEMGP EnemyGroups;
		public T8BTEMEG.T8BTEMEG EncounterGroups;
		public COOKDAT.COOKDAT Recipes;
		public WRLDDAT.WRLDDAT Locations;
		public SYNPDAT.SYNPDAT Synopsis;
		public FAMEDAT.FAMEDAT Titles;
		public T8BTGR.T8BTGR GradeShop;
		public BTLBDAT.BTLBDAT BattleBook;
		public T8BTTA.T8BTTA Strategy;
		public ShopData.ShopData Shops;
		public TO8CHLI.TO8CHLI Skits;
		public List<uint> Records;
		public List<Setting> Settings;

		public T8BTXTM.T8BTXTMA NecropolisFloors;
		public T8BTXTM.T8BTXTMT NecropolisTreasures;
		public Dictionary<string, T8BTXTM.T8BTXTMM> NecropolisMaps;

		public Dictionary<uint, TSS.TSSEntry> InGameIdDict;
		public uint[] IconsWithItems;

		public string GenerateHtmlItems( uint? icon = null, uint? category = null ) {
			Console.WriteLine( "Generating Website: Items" );
			var sb = new StringBuilder();
			AddHeader( sb, "Items" );
			sb.AppendLine( "<body><table>" );
			AddMenuBar( sb );
			foreach ( var item in Items.items ) {
				int itemCat = (int)item.Data[(int)ItemData.Category];
				int itemIcon = (int)item.Data[(int)ItemData.Icon];

				if ( itemCat == 0 ) { continue; }
				if ( category != null && category != itemCat ) { continue; }
				if ( icon != null && icon != itemIcon ) { continue; }

				sb.AppendLine( ItemDat.ItemDat.GetItemDataAsHtml( Version, Items, item, Skills, Enemies, Recipes, Locations, StringDic, InGameIdDict ) );
				sb.AppendLine( "<tr><td colspan=\"5\"><hr></td></tr>" );
			}
			sb.AppendLine( "</table></body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlEnemies( int? category = null ) {
			Console.WriteLine( "Generating Website: Enemies" );
			var sb = new StringBuilder();
			AddHeader( sb, "Enemies" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			sb.Append( "<table>" );
			foreach ( var enemy in Enemies.EnemyList ) {
				if ( enemy.InGameID == 0 ) { continue; }
				if ( category != null && category != enemy.Category ) { continue; }
				sb.AppendLine( enemy.GetDataAsHtml( Version, Items, Locations, StringDic, InGameIdDict ) );
				sb.AppendLine( "<tr><td colspan=\"7\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlEnemyGroups() {
			Console.WriteLine( "Generating Website: Enemy Groups" );
			var sb = new StringBuilder();
			AddHeader( sb, "Enemy Groups" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			sb.Append( "<table>" );
			foreach ( var group in EnemyGroups.EnemyGroupList ) {
				sb.AppendLine( group.GetDataAsHtml( Enemies, InGameIdDict ) );
				sb.AppendLine( "<tr><td colspan=\"9\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlEncounterGroups() {
			Console.WriteLine( "Generating Website: Encounters" );
			var sb = new StringBuilder();
			AddHeader( sb, "Encounter Groups" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			sb.Append( "<table>" );
			foreach ( var group in EncounterGroups.EncounterGroupList ) {
				sb.AppendLine( group.GetDataAsHtml( EnemyGroups, Enemies, InGameIdDict ) );
				sb.AppendLine( "<tr><td colspan=\"9\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlSkills() {
			Console.WriteLine( "Generating Website: Skills" );
			var sb = new StringBuilder();
			AddHeader( sb, "Skills" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			sb.Append( "<table>" );
			foreach ( var skill in Skills.SkillList ) {
				if ( skill.ID == 0 ) { continue; }
				sb.AppendLine( skill.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"4\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlArtes() {
			Console.WriteLine( "Generating Website: Artes" );
			var sb = new StringBuilder();
			AddHeader( sb, "Artes" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			sb.Append( "<table>" );
			foreach ( var arte in Artes.ArteList ) {
				switch ( arte.Type ) {
					case T8BTMA.Arte.ArteType.Generic:
					case T8BTMA.Arte.ArteType.SkillAutomatic:
					case T8BTMA.Arte.ArteType.FatalStrike:
					case T8BTMA.Arte.ArteType.OverLimit:
						//break;
						continue;
				}
				sb.AppendLine( arte.GetDataAsHtml( Version, Artes.ArteIdDict, Enemies, Skills, StringDic, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"4\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlSynopsis() {
			Console.WriteLine( "Generating Website: Synopsis" );
			var sb = new StringBuilder();
			AddHeader( sb, "Synopsis" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			foreach ( var entry in Synopsis.SynopsisList ) {
				if ( InGameIdDict[entry.NameStringDicId].StringEngOrJpn == "" ) { continue; }
				sb.AppendLine( entry.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				sb.AppendLine( "<hr>" );
			}
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlSkitInfo() {
			Console.WriteLine( "Generating Website: Skit Info" );
			var sb = new StringBuilder();
			AddHeader( sb, "Skits" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			foreach ( var entry in Skits.SkitInfoList ) {
				sb.AppendLine( entry.GetDataAsHtml( Version, Skits, InGameIdDict ) );
				sb.AppendLine( "<hr>" );
			}
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlRecipes() {
			Console.WriteLine( "Generating Website: Recipes" );
			var sb = new StringBuilder();
			AddHeader( sb, "Recipes" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			sb.Append( "<table>" );
			for ( int i = 1; i < Recipes.RecipeList.Count; ++i ) {
				var recipe = Recipes.RecipeList[i];
				sb.Append( "<tr>" );
				sb.Append( recipe.GetDataAsHtml( Version, Recipes, Items, StringDic, InGameIdDict ) );
				sb.Append( "</tr>" );
				sb.Append( "<tr><td colspan=\"4\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlLocations() {
			Console.WriteLine( "Generating Website: Locations" );
			var sb = new StringBuilder();
			AddHeader( sb, "Locations" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			sb.AppendLine( "<table>" );
			for ( int i = 1; i < Locations.LocationList.Count; ++i ) {
				var location = Locations.LocationList[i];
				sb.AppendLine( location.GetDataAsHtml( Version, StringDic, InGameIdDict, EncounterGroups, EnemyGroups, Enemies, Shops ) );
				sb.Append( "<tr><td colspan=\"3\"><hr></td></tr>" );
			}
			sb.AppendLine( "</table>" );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlStrategy() {
			Console.WriteLine( "Generating Website: Strategy" );
			var sb = new StringBuilder();
			AddHeader( sb, "Strategy" );
			sb.Append( "<body>" );
			AddMenuBar( sb );
			sb.Append( "<table>" );
			foreach ( var entry in Strategy.StrategySetList ) {
				sb.Append( entry.GetDataAsHtml( Version, Strategy, StringDic, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"10\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.Append( "<table>" );
			foreach ( var entry in Strategy.StrategyOptionList ) {
				sb.Append( entry.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"4\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.Append( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlShops() {
			Console.WriteLine( "Generating Website: Shops" );
			var sb = new StringBuilder();
			AddHeader( sb, "Shops" );
			sb.Append( "<body>" );
			AddMenuBar( sb );
			sb.Append( "<table>" );
			foreach ( var entry in Shops.ShopDefinitions ) {
				if ( entry.InGameID == 1 ) { continue; } // dummy shop
				sb.Append( entry.GetDataAsHtml( Version, Items, Shops, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"6\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.Append( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlTitles() {
			Console.WriteLine( "Generating Website: Titles" );
			var sb = new StringBuilder();
			AddHeader( sb, "Titles" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			sb.Append( "<table>" );
			foreach ( var entry in Titles.TitleList ) {
				if ( entry.Character == 0 ) { continue; }
				sb.Append( entry.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"4\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlBattleBook() {
			Console.WriteLine( "Generating Website: Battle Book" );
			var sb = new StringBuilder();
			AddHeader( sb, "Battle Book" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );

			sb.Append( "<table>" );

			foreach ( var entry in BattleBook.BattleBookEntryList ) {
				string data;
				try {
					if ( InGameIdDict[entry.NameStringDicId].StringEngOrJpn == "" ) { continue; }
					data = entry.GetDataAsHtml( Version, StringDic, InGameIdDict );
				} catch ( KeyNotFoundException ) {
					continue;
				}
				sb.Append( "<tr>" );
				sb.Append( data );
				sb.Append( "</tr>" );
				sb.Append( "<tr><td colspan=\"2\"><hr></td></tr>" );
			}

			sb.Append( "</table>" );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public List<uint> GenerateRecordsStringDicList() {
			List<uint> records = new List<uint>();

			for ( uint i = 33912371; i < 33912385; ++i ) {
				if ( i == 33912376 ) { continue; }
				records.Add( i );
			}
			records.Add( 33912570u );
			for ( uint i = 33912385; i < 33912392; ++i ) {
				records.Add( i );
			}
			records.Add( 33912571u );
			records.Add( 33912572u );
			records.Add( 33912585u );
			records.Add( 33912586u );
			records.Add( 33912587u );
			records.Add( 33912588u );

			if ( Version == GameVersion.PS3 ) {
				// repede snowboarding 1 - 8, team melee, 30 man per character
				for ( uint i = 33912733; i < 33912751; ++i ) {
					records.Add( i );
				}
			} else {
				records.Add( 33912621u ); // 30 man melee generic
			}

			for ( uint i = 33912392; i < 33912399; ++i ) {
				records.Add( i );
			}
			if ( Version == GameVersion.PS3 ) {
				// usage flynn, patty
				records.Add( 33912399u );
				records.Add( 33912400u );
			}

			return records;
		}
		public string GenerateHtmlRecords() {
			Console.WriteLine( "Generating Website: Records" );
			var sb = new StringBuilder();
			AddHeader( sb, "Records" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );

			sb.Append( "<table>" );
			foreach ( var i in Records ) {
				AppendRecord( sb, i );
			}
			sb.Append( "</table>" );

			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public void AppendRecord( StringBuilder sb, uint id ) {
			sb.Append( "<tr>" );
			sb.Append( "<td>" );
			sb.Append( InGameIdDict[id].StringJpnHtml( Version ) );
			sb.Append( "</td>" );
			sb.Append( "<td>" );
			sb.Append( InGameIdDict[id].StringEngHtml( Version ) );
			sb.Append( "</td>" );
			sb.Append( "</tr>" );
			//sb.Append( "<tr><td colspan=\"2\"><hr></td></tr>" );
		}
		public List<Setting> GenerateSettingsStringDicList() {
			List<Setting> settings = new List<Setting>();

			settings.Add( new Setting( 33912401u, 33912401u + 46u, 33912427u, 33912426u, 33912425u, 33912424u ) ); // msg speed
			settings.Add( new Setting( 33912402u, 33912402u + 46u, 33912428u, 33912429u, 33912430u, 33912431u ) ); // difficulty
			if ( Version == GameVersion.X360 ) {
				settings.Add( new Setting( 33912403u, 33912403u + 46u, 33912438u, 33912437u ) ); // x360 vibration
			} else {
				settings.Add( new Setting( 33912679u, 33912681u, 33912438u, 33912437u ) ); // console-neutral vibration
			}
			settings.Add( new Setting( 33912404u, 33912404u + 46u, 33912432u, 33912433u ) ); // camera controls
			if ( Version == GameVersion.PS3 ) {
				settings.Add( new Setting( 33912751u, 33912752u, 33912443u, 33912444u ) ); // stick/dpad controls
			}
			settings.Add( new Setting( 33912405u, 33912405u + 46u, 33912439u ) ); // button config
			settings.Add( new Setting( 33912406u, 33912406u + 46u, 33912436u, 33912435u, 33912434u ) ); // sound
			settings.Add( new Setting( 33912407u, 33912407u + 46u ) ); // bgm
			settings.Add( new Setting( 33912408u, 33912408u + 46u ) ); // se
			settings.Add( new Setting( 33912409u, 33912409u + 46u ) ); // battle se
			settings.Add( new Setting( 33912413u, 33912413u + 46u ) ); // battle voice
			settings.Add( new Setting( 33912414u, 33912414u + 46u ) ); // event voice
			settings.Add( new Setting( 33912422u, 33912422u + 46u ) ); // skit
			settings.Add( new Setting( 33912423u, 33912423u + 46u ) ); // movie
			if ( Version == GameVersion.PS3 ) {
				settings.Add( new Setting( 33912656u, 33912657u, 33912658u, 33912659u ) ); // item request type
			}
			settings.Add( new Setting( 33912410u, 33912410u + 46u, 33912438u, 33912437u ) ); // engage cam
			settings.Add( new Setting( 33912411u, 33912411u + 46u, 33912438u, 33912437u ) ); // dynamic cam
			settings.Add( new Setting( 33912412u, 33912412u + 46u, 33912438u, 33912437u ) ); // field boundary
			settings.Add( new Setting( 33912415u, 33912415u + 46u, 33912438u, 33912437u ) ); // location names
			settings.Add( new Setting( 33912416u, 33912416u + 46u, 33912438u, 33912437u ) ); // skit titles
			settings.Add( new Setting( 33912417u, 33912417u + 46u, 33912438u, 33912437u ) ); // skit subs
			settings.Add( new Setting( 33912418u, 33912418u + 46u, 33912438u, 33912437u ) ); // movie subs
			settings.Add( new Setting( 33912420u, 33912420u + 46u, 33912440u, 33912441u, 33912442u ) ); // font
			if ( Version == GameVersion.X360 ) {
				settings.Add( new Setting( 33912419u, 33912419u + 46u, 33912439u ) ); // brightness
				settings.Add( new Setting( 33912421u, 33912421u + 46u, 33912439u ) ); // marketplace
			} else {
				settings.Add( new Setting( 33912713u, 33912714u, 33912439u ) ); // brightness & screen pos
			}
			settings.Add( new Setting( 33912595u, 33912596u, 33912597u ) ); // reset to default

			return settings;
		}
		public string GenerateHtmlSettings() {
			Console.WriteLine( "Generating Website: Settings" );
			var sb = new StringBuilder();
			AddHeader( sb, "Settings" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );

			sb.Append( "<table class=\"settings\">" );
			foreach ( var s in Settings ) {
				AppendSetting( sb, s.NameStringDicId, s.DescStringDicId, s.OptionsStringDicIds[0],
					s.OptionsStringDicIds[1], s.OptionsStringDicIds[2], s.OptionsStringDicIds[3] );
			}
			sb.Append( "</table>" );

			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public void AppendSetting( StringBuilder sb, uint idName, uint idDesc, uint option1 = 0, uint option2 = 0, uint option3 = 0, uint option4 = 0 ) {
			for ( int i = 0; i < 2; ++i ) {
				sb.Append( "<tr>" );
				sb.Append( "<td>" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( InGameIdDict[idName].GetStringHtml( i, Version ) );
				sb.Append( "</span>" );
				sb.Append( "</td>" );

				int optionCount = 0;
				if ( option1 > 0 ) { ++optionCount; }
				if ( option2 > 0 ) { ++optionCount; }
				if ( option3 > 0 ) { ++optionCount; }
				if ( option4 > 0 ) { ++optionCount; }

				if ( optionCount == 0 ) {
					sb.Append( "<td colspan=\"4\">" );
					sb.Append( "</td>" );
				} else {
					if ( option1 > 0 ) {
						if ( optionCount == 1 ) {
							sb.Append( "<td colspan=\"4\">" );
						} else {
							sb.Append( "<td>" );
						}
						sb.Append( InGameIdDict[option1].GetStringHtml( i, Version ) );
						sb.Append( "</td>" );
					}
					if ( option2 > 0 ) {
						if ( optionCount == 2 ) {
							sb.Append( "<td colspan=\"3\">" );
						} else {
							sb.Append( "<td>" );
						}
						sb.Append( InGameIdDict[option2].GetStringHtml( i, Version ) );
						sb.Append( "</td>" );
					}
					if ( option3 > 0 ) {
						if ( optionCount == 3 ) {
							sb.Append( "<td colspan=\"2\">" );
						} else {
							sb.Append( "<td>" );
						}
						sb.Append( InGameIdDict[option3].GetStringHtml( i, Version ) );
						sb.Append( "</td>" );
					}
					if ( option4 > 0 ) {
						sb.Append( "<td>" );
						sb.Append( InGameIdDict[option4].GetStringHtml( i, Version ) );
						sb.Append( "</td>" );
					}
				}
				sb.Append( "</tr>" );

				sb.Append( "<tr>" );
				sb.Append( "<td colspan=\"5\">" );
				sb.Append( InGameIdDict[idDesc].GetStringHtml( i, Version ) );
				sb.Append( "</td>" );
				sb.Append( "</tr>" );
			}

			sb.Append( "<tr><td colspan=\"5\"><hr></td></tr>" );
		}
		public string GenerateHtmlGradeShop() {
			Console.WriteLine( "Generating Website: Grade Shop" );
			var sb = new StringBuilder();
			AddHeader( sb, "Grade Shop" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			sb.Append( "<table>" );
			foreach ( var entry in GradeShop.GradeShopEntryList ) {
				if ( entry.GradeCost == 0 ) { continue; }
				sb.Append( entry.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"3\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlNecropolis( bool showEnemies ) {
			Console.WriteLine( "Generating Website: Necropolis" );
			var sb = new StringBuilder();
			AddHeader( sb, "Necropolis of Nostalgia" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );

			foreach ( var floor in NecropolisFloors.FloorList ) {
				int floorNumberLong = Int32.Parse( floor.RefString1.Split( '_' ).Last() );
				int floorNumber = ( floorNumberLong - 1 ) % 10 + 1;
				int floorStratumAsNumber = ( floorNumberLong - 1 ) / 10 + 1;
				string floorStratum = ( (char)( floorStratumAsNumber + 64 ) ).ToString();
				string html = NecropolisMaps[floor.RefString2].GetDataAsHtml( floorStratum, floorNumber, showEnemies ? Enemies : null, showEnemies ? EnemyGroups : null, showEnemies ? EncounterGroups : null, Version, NecropolisTreasures, Items, InGameIdDict );
				sb.Append( html );
				sb.Append( "<hr>" );

				string dir = @"d:\Dropbox\ToV\website\";
				StringBuilder sb2 = new StringBuilder();
				AddHeader( sb2, floorStratum + "-" + floorNumber + " - Necropolis of Nostalgia" );
				sb2.Append( "<body>" );
				sb2.Append( html );
				sb2.Append( "</body></html>" );
				System.IO.File.WriteAllText( Path.Combine( dir, "necropolis-single-" + floorStratum + floorNumber + "-" + ( showEnemies ? "enemies-" : "" ) + Version + ".html" ), sb2.ToString(), Encoding.UTF8 );
			}

			/*
			foreach ( var treasureLayout in NecropolisTreasures.TreasureInfoList ) {
				sb.Append( "<hr>" );
				sb.Append( treasureLayout.GetDataAsHtml( Items, InGameIdDict ) );
			}
			//*/

			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlNpc() {
			Console.WriteLine( "Generating Website: NPCs" );
			var npcListPS3 = new TOVNPC.TOVNPCL( @"d:\Dropbox\ToV\PS3\orig\npc.svo.ext\NPC.DAT.dec.ext\0000.dec" );
			Dictionary<string, TOVNPC.TOVNPCT> npcDefs = new Dictionary<string, TOVNPC.TOVNPCT>();
			foreach ( var f in npcListPS3.NpcFileList ) {
				string filename = @"d:\Dropbox\ToV\PS3\orig\npc.svo.ext\" + f.Filename + @".dec.ext\0001.dec";
				if ( File.Exists( filename ) ) {
					var d = new TOVNPC.TOVNPCT( filename );
					npcDefs.Add( f.Map, d );
				}
			}

			StringBuilder sb = new StringBuilder();
			AddHeader( sb, "NPC Dialogue" );
			sb.AppendLine( "<body>" );
			//AddMenuBar( sb );
			sb.Append( "<table class=\"npcdiff\">" );
			foreach ( var kvp in npcDefs ) {
				for ( int i = 0; i < kvp.Value.NpcDefList.Count; ++i ) {
					var d = kvp.Value.NpcDefList[i];
					sb.Append( "<tr>" );

					sb.Append( "<td>" );
					sb.Append( kvp.Key );
					sb.Append( "<br>" );
					sb.Append( d.StringDicId );
					//sb.Append( "<br>" );
					//sb.Append( d.RefString1 );
					//sb.Append( "<br>" );
					//sb.Append( d.RefString2 );
					sb.Append( "</td>" );

					sb.Append( "<td>" );
					sb.Append( InGameIdDict[d.StringDicId].StringJpnHtml( Version ) );
					sb.Append( "</td>" );

					sb.Append( "<td>" );
					sb.Append( InGameIdDict[d.StringDicId].StringEngHtml( Version ) );
					sb.Append( "</td>" );

					sb.Append( "</tr>" );

					sb.Append( "<tr><td colspan=\"3\"><hr></td></tr>" );
				}
			}
			sb.Append( "</table>" );
			sb.Append( "</body></html>" );

			return sb.ToString();
		}

		public void AddHeader( StringBuilder sb, string name ) {
			sb.AppendLine( "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">" );
			sb.AppendLine( "<title>" + name + " - Tales of Vesperia (" + Version.ToString() + ")</title>" );
			sb.AppendLine( "<style>" );
			sb.AppendLine( "body { background-color: #68504F; color: #EFD1AE; font-size: 16; }" );
			sb.AppendLine( ".itemname { color: #FFEBD2; font-size: 20; }" );
			sb.AppendLine( ".itemdesc { }" );
			sb.AppendLine( ".equip { text-align: right; float: right; }" );
			sb.AppendLine( ".special { text-align: right; float: right; }" );
			sb.AppendLine( "table, tr, td, th { padding: 0px 4px 0px 0px; border-spacing: 0px; }" );
			sb.AppendLine( "td, td > a { vertical-align: top; }" );
			sb.AppendLine( "a:link, a:visited, a:hover, a:active { color: #FFEBD2; }" );
			sb.AppendLine( "table.element { display: inline-block; }" );
			sb.AppendLine( "table.element td { text-align: center; padding: 0px 8px 0px 8px; border-spacing: 0px; }" );
			sb.AppendLine( "table.synopsis { margin: 0px auto; }" );
			sb.AppendLine( "table.synopsis td { padding: 0px 10px 0px 10px; border-spacing: 10px; }" );
			sb.AppendLine( ".synopsistitle { text-align: center; color: #FFEBD2; font-size: 20; }" );
			sb.AppendLine( "table.necropolisfloor td { width: 200px; height: 150px; padding: 0; }" );
			sb.AppendLine( "td.necropolistile1 div.necropolis-data { background-color: black; }" );
			sb.AppendLine( "td.necropolistile2 div.necropolis-data { background-color: #701010; }" );
			sb.AppendLine( "td.necropolistile3 div.necropolis-data { background-color: #203860; }" );
			sb.AppendLine( "td.necropolistile4 div.necropolis-data { background-color: #204040; }" );
			sb.AppendLine( "td.necropolistile5 div.necropolis-data { background-color: #701010; }" );
			sb.AppendLine( "div.necropolis-arrow-up    { float: left;  width: 200px; height: 16px;  text-align: center; }" );
			sb.AppendLine( "div.necropolis-arrow-down  { float: left;  width: 200px; height: 16px;  text-align: center; }" );
			sb.AppendLine( "div.necropolis-arrow-side  { float: left;  width: 16px; height: 118px;  text-align: center; line-height: 118px; position:relative; min-height: 16px; }" );
			sb.AppendLine( "div.necropolis-arrow-side img { vertical-align: middle; max-height: 118px; position: absolute; top: 0; bottom: 0; left: 0; margin: auto; }" );
			sb.AppendLine( "div.necropolis-data        { float: left;  width: 166px; height: 116px; text-align: center; border: 1px solid black; }" );
			sb.AppendLine( "div.necropolis-data table  { margin: 0px auto; }" );
			sb.AppendLine( "div.necropolis-data td     { width: auto; height: auto; text-align: center; padding: 0px 4px 0px 4px; }" );
			sb.AppendLine( "td.skilljpn { white-space: nowrap; }" );
			sb.AppendLine( "td.skilldata { width: 275px; }" );
			sb.AppendLine( ".strategycat { color: #FFEBD2; }" );
			sb.AppendLine( ".strategychar { text-align: center; }" );
			sb.AppendLine( "table.settings td { vertical-align: middle; min-width: 90px; }" );
			sb.AppendLine( ".difficultyname { font-weight: bold; text-decoration: underline; }" );
			sb.AppendLine( "</style>" );
			sb.AppendLine( "</head>" );
		}
		public void AddMenuBar( StringBuilder sb ) {
			sb.AppendLine( "<div>" );
			sb.AppendLine( "<a href=\"artes-" + Version + ".html\"><img src=\"menu-icons/main-01.png\" title=\"Artes\"></a>" );
			//sb.AppendLine( "<a href=\"equip-" + Version + ".html\"><img src=\"menu-icons/main-02.png\" title=\"Equipment\"></a>" );
			//sb.AppendLine( "<a href=\"items-" + Version + ".html\"><img src=\"menu-icons/main-03.png\" title=\"Items\"></a>" );
			sb.AppendLine( "<a href=\"skills-" + Version + ".html\"><img src=\"menu-icons/main-04.png\" title=\"Skills\"></a>" );
			sb.AppendLine( "<a href=\"strategy-" + Version + ".html\"><img src=\"menu-icons/main-05.png\" title=\"Strategy\"></a>" );
			sb.AppendLine( "<a href=\"recipes-" + Version + ".html\"><img src=\"menu-icons/main-06.png\" title=\"Recipes\"></a>" );
			sb.AppendLine( "<a href=\"shops-" + Version + ".html\"><img src=\"menu-icons/main-02.png\" title=\"Shops\"></a>" );
			sb.AppendLine( "<a href=\"titles-" + Version + ".html\"><img src=\"menu-icons/main-07.png\" title=\"Titles\"></a>" );
			//sb.AppendLine( "<img src=\"menu-icons/main-08.png\" title=\"Library\">" );
			sb.AppendLine( "<a href=\"synopsis-" + Version + ".html\"><img src=\"menu-icons/sub-09.png\" title=\"Synopsis\"></a>" );
			sb.AppendLine( "<a href=\"battlebook-" + Version + ".html\"><img src=\"menu-icons/sub-14.png\" title=\"Battle Book\"></a>" );
			sb.AppendLine( "<a href=\"enemies-" + Version + ".html\"><img src=\"menu-icons/sub-13.png\" title=\"Monster Book\"></a>" );
			sb.AppendLine( "<a href=\"items-" + Version + ".html\"><img src=\"menu-icons/sub-11.png\" title=\"Collector's Book\"></a>" );
			sb.AppendLine( "<a href=\"locations-" + Version + ".html\"><img src=\"menu-icons/sub-10.png\" title=\"World Map\"></a>" );
			sb.AppendLine( "<a href=\"records-" + Version + ".html\"><img src=\"menu-icons/sub-08.png\" title=\"Records\"></a>" );
			//sb.AppendLine( "<img src=\"menu-icons/main-09.png\" title=\"Save & Load\">" );
			//sb.AppendLine( "<img src=\"menu-icons/sub-06.png\" title=\"Save\">" );
			//sb.AppendLine( "<img src=\"menu-icons/sub-05.png\" title=\"Load\">" );
			sb.AppendLine( "<a href=\"settings-" + Version + ".html\"><img src=\"menu-icons/sub-07.png\" title=\"Settings\"></a>" );
			sb.AppendLine( "<a href=\"gradeshop-" + Version + ".html\"><img src=\"item-categories/cat-01.png\" title=\"Grade Shop\"></a>" );
			if ( Version == GameVersion.PS3 ) {
				sb.AppendLine( "<a href=\"necropolis-" + Version + ".html\"><img src=\"menu-icons/weather-4-64px.png\" title=\"Necropolis of Nostalgia Maps\"></a>" );
			}
			sb.AppendLine( "<br>" );
			for ( uint i = 2; i < 12; ++i ) {
				sb.Append( "<a href=\"items-c" + i + "-" + Version + ".html\">" );
				sb.Append( "<img src=\"item-categories/cat-" + i.ToString( "D2" ) + ".png\" title=\"" + InGameIdDict[33912572u + i].StringEngOrJpnHtml( Version ) + "\" height=\"32\">" );
				sb.Append( "</a>" );
			}
			sb.AppendLine();
			for ( uint i = 0; i < 9; ++i ) {
				sb.Append( "<a href=\"enemies-c" + i + "-" + Version + ".html\">" );
				sb.Append( "<img src=\"monster-categories/cat-" + i + ".png\" title=\"" + InGameIdDict[33912323u + i].StringEngOrJpnHtml( Version ) + "\" height=\"32\">" );
				sb.Append( "</a>" );
			}
			sb.AppendLine( "<br>" );
			foreach ( uint i in IconsWithItems ) {
				sb.Append( "<a href=\"items-i" + i + "-" + Version + ".html\">" );
				sb.Append( "<img src=\"item-icons/ICON" + i + ".png\" height=\"16\" width=\"16\">" );
				sb.Append( "</a>" );
			}

			sb.AppendLine( "</div>" );
			sb.AppendLine( "<hr>" );
		}
		public static StringBuilder ReplaceIconsWithHtml( StringBuilder sb, GameVersion Version ) {
			sb.Replace( "\x06(ST1)", "<img src=\"text-icons/icon-status-01.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST2)", "<img src=\"text-icons/icon-status-02.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST3)", "<img src=\"text-icons/icon-status-03.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST4)", "<img src=\"text-icons/icon-status-04.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST5)", "<img src=\"text-icons/icon-status-05.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST6)", "<img src=\"text-icons/icon-status-06.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST7)", "<img src=\"text-icons/icon-status-07.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC1)", "<img src=\"text-icons/icon-status-08.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC2)", "<img src=\"text-icons/icon-status-09.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC3)", "<img src=\"text-icons/icon-status-10.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC4)", "<img src=\"text-icons/icon-status-11.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC5)", "<img src=\"text-icons/icon-status-17.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC6)", "<img src=\"text-icons/icon-status-13.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC7)", "<img src=\"text-icons/icon-status-18.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL1)", "<img src=\"text-icons/icon-element-02.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL2)", "<img src=\"text-icons/icon-element-04.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL3)", "<img src=\"text-icons/icon-element-01.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL4)", "<img src=\"text-icons/icon-element-05.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL5)", "<img src=\"text-icons/icon-element-03.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL6)", "<img src=\"text-icons/icon-element-06.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL7)", "<img src=\"text-icons/icon-element-07.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(FS1)", "<img src=\"text-icons/icon-fs-01.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(FS2)", "<img src=\"text-icons/icon-fs-02.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(FS3)", "<img src=\"text-icons/icon-fs-03.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC1)", "<img src=\"text-icons/icon-monster-01.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC2)", "<img src=\"text-icons/icon-monster-02.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC3)", "<img src=\"text-icons/icon-monster-03.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC4)", "<img src=\"text-icons/icon-monster-04.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC5)", "<img src=\"text-icons/icon-monster-05.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC6)", "<img src=\"text-icons/icon-monster-06.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC7)", "<img src=\"text-icons/icon-monster-07.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC8)", "<img src=\"text-icons/icon-monster-08.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC9)", "<img src=\"text-icons/icon-monster-09.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(COS)", "<img src=\"text-icons/icon-costume.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(STA)", "<img src=\"text-icons/" + Version.ToString() + "/button-Start.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.Start ) + "\">" );
			sb.Replace( "\x06(SEL)", "<img src=\"text-icons/" + Version.ToString() + "/button-Select.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.Select ) + "\">" );
			sb.Replace( "\x06(L3)", "<img src=\"text-icons/" + Version.ToString() + "/ls-push.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.L3 ) + "\">" );
			sb.Replace( "\x06(R3)", "<img src=\"text-icons/" + Version.ToString() + "/rs-push.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.R3 ) + "\">" );
			sb.Replace( "\x06(MVN)", "<img src=\"text-icons/" + Version.ToString() + "/ls.png\" height=\"16\" title=\"Character Movement\">" );
			sb.Replace( "\x06(MVR)", "<img src=\"text-icons/" + Version.ToString() + "/ls-right.png\" height=\"16\" title=\"Character Movement Right\">" );
			sb.Replace( "\x06(MVL)", "<img src=\"text-icons/" + Version.ToString() + "/ls-left.png\" height=\"16\" title=\"Character Movement Left\">" );
			sb.Replace( "\x06(MVU)", "<img src=\"text-icons/" + Version.ToString() + "/ls-up.png\" height=\"16\" title=\"Character Movement Up\">" );
			sb.Replace( "\x06(MVD)", "<img src=\"text-icons/" + Version.ToString() + "/ls-down.png\" height=\"16\" title=\"Character Movement Down\">" );
			sb.Replace( "\x06(MVH)", "<img src=\"text-icons/" + Version.ToString() + "/ls-side.png\" height=\"16\" title=\"Character Movement Side\">" );
			sb.Replace( "\x06(LTN)", "<img src=\"text-icons/" + Version.ToString() + "/ls.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftStick ) + "\">" );
			sb.Replace( "\x06(LTR)", "<img src=\"text-icons/" + Version.ToString() + "/ls-right.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftStick ) + " Right\">" );
			sb.Replace( "\x06(LTL)", "<img src=\"text-icons/" + Version.ToString() + "/ls-left.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftStick ) + " Left\">" );
			sb.Replace( "\x06(LTU)", "<img src=\"text-icons/" + Version.ToString() + "/ls-up.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftStick ) + " Up\">" );
			sb.Replace( "\x06(LTD)", "<img src=\"text-icons/" + Version.ToString() + "/ls-down.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftStick ) + " Down\">" );
			sb.Replace( "\x06(LTH)", "<img src=\"text-icons/" + Version.ToString() + "/ls-side.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftStick ) + " Side\">" );
			sb.Replace( "\x06(RTN)", "<img src=\"text-icons/" + Version.ToString() + "/rs.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightStick ) + "\">" );
			sb.Replace( "\x06(RTR)", "<img src=\"text-icons/" + Version.ToString() + "/rs-right.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightStick ) + " Right\">" );
			sb.Replace( "\x06(RTL)", "<img src=\"text-icons/" + Version.ToString() + "/rs-left.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightStick ) + " Left\">" );
			sb.Replace( "\x06(RTU)", "<img src=\"text-icons/" + Version.ToString() + "/rs-up.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightStick ) + " Up\">" );
			sb.Replace( "\x06(RTD)", "<img src=\"text-icons/" + Version.ToString() + "/rs-down.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightStick ) + " Down\">" );
			sb.Replace( "\x06(RTH)", "<img src=\"text-icons/" + Version.ToString() + "/rs-side.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightStick ) + " Side\">" );
			sb.Replace( "\x06(LBN)", "<img src=\"text-icons/" + Version.ToString() + "/dpad.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.DPad ) + "\">" );
			sb.Replace( "\x06(LBR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-right.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.DPad ) + " Right\">" );
			sb.Replace( "\x06(LBL)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-left.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.DPad ) + " Left\">" );
			sb.Replace( "\x06(LBU)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-up.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.DPad ) + " Up\">" );
			sb.Replace( "\x06(LBD)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-down.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.DPad ) + " Down\">" );
			sb.Replace( "\x06(LBH)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-side.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.DPad ) + " Side\">" );
			sb.Replace( "\x06(MVNR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad.png\" height=\"16\" title=\"Non-Character Movement\">" );
			sb.Replace( "\x06(MVRR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-right.png\" height=\"16\" title=\"Non-Character Movement Right\">" );
			sb.Replace( "\x06(MVLR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-left.png\" height=\"16\" title=\"Non-Character Movement Left\">" );
			sb.Replace( "\x06(MVUR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-up.png\" height=\"16\" title=\"Non-Character Movement Up\">" );
			sb.Replace( "\x06(MVDR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-down.png\" height=\"16\" title=\"Non-Character Movement Down\">" );
			sb.Replace( "\x06(MVHR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-side.png\" height=\"16\" title=\"Non-Character Movement Side\">" );
			sb.Replace( "\x06(RBL)", "<img src=\"text-icons/" + Version.ToString() + "/button-action.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftButton ) + "\">" );
			sb.Replace( "\x06(RBU)", "<img src=\"text-icons/" + Version.ToString() + "/button-menu.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.UpperButton ) + "\">" );
			sb.Replace( "\x06(RBR)", "<img src=\"text-icons/" + Version.ToString() + "/button-cancel.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightButton ) + "\">" );
			sb.Replace( "\x06(RBD)", "<img src=\"text-icons/" + Version.ToString() + "/button-confirm.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LowerButton ) + "\">" );
			sb.Replace( "\x06(CCL)", "<img src=\"text-icons/" + Version.ToString() + "/button-cancel.png\" height=\"16\" title=\"Cancel\">" );
			sb.Replace( "\x06(ETR)", "<img src=\"text-icons/" + Version.ToString() + "/button-confirm.png\" height=\"16\" title=\"Confirm\">" );
			sb.Replace( "\x06(ATK)", "<img src=\"text-icons/" + Version.ToString() + "/button-cancel.png\" height=\"16\" title=\"Attack\">" );
			sb.Replace( "\x06(ART)", "<img src=\"text-icons/" + Version.ToString() + "/button-confirm.png\" height=\"16\" title=\"Arte\">" );
			sb.Replace( "\x06(GUD)", "<img src=\"text-icons/" + Version.ToString() + "/button-action.png\" height=\"16\" title=\"Guard\">" );
			sb.Replace( "\x06(MEN)", "<img src=\"text-icons/" + Version.ToString() + "/button-menu.png\" height=\"16\" title=\"Menu\">" );
			sb.Replace( "\x06(CBR1)", "<img src=\"text-icons/" + Version.ToString() + "/button-R1.png\" height=\"16\" title=\"Enemy Target Switch\">" );
			sb.Replace( "\x06(CBR2)", "<img src=\"text-icons/" + Version.ToString() + "/button-R2.png\" height=\"16\" title=\"Fatal Strike\">" );
			sb.Replace( "\x06(CBL1)", "<img src=\"text-icons/" + Version.ToString() + "/button-L1.png\" height=\"16\" title=\"Alternate Attack\">" );
			sb.Replace( "\x06(CBL2)", "<img src=\"text-icons/" + Version.ToString() + "/button-L2.png\" height=\"16\" title=\"Free Run\">" );
			sb.Replace( "\x06(R1)", "<img src=\"text-icons/" + Version.ToString() + "/button-R1.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.R1 ) + "\">" );
			sb.Replace( "\x06(R2)", "<img src=\"text-icons/" + Version.ToString() + "/button-R2.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.R2 ) + "\">" );
			sb.Replace( "\x06(L1)", "<img src=\"text-icons/" + Version.ToString() + "/button-L1.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.L1 ) + "\">" );
			sb.Replace( "\x06(L2)", "<img src=\"text-icons/" + Version.ToString() + "/button-L2.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.L2 ) + "\">" );
			sb.Replace( '∀', '♥' );
			sb.Replace( '‡', 'é' );
			sb.Replace( '†', 'í' );
			return sb;
		}
		public string FixInGameStrings( StringBuilder sb ) {
			return VesperiaUtil.RemoveTags( ReplaceIconsWithHtml( sb, this.Version ).ToString() );
		}
		public static void AppendCharacterBitfieldAsImageString( StringBuilder sb, GameVersion version, uint equip ) {
			if ( ( equip & 1 ) == 1 ) { sb.Append( "<img src=\"chara-icons/YUR.png\" height=\"32\" width=\"24\" title=\"Yuri\">" ); }
			if ( ( equip & 2 ) == 2 ) { sb.Append( "<img src=\"chara-icons/EST.png\" height=\"32\" width=\"24\" title=\"Estelle\">" ); }
			if ( ( equip & 4 ) == 4 ) { sb.Append( "<img src=\"chara-icons/KAR.png\" height=\"32\" width=\"24\" title=\"Karol\">" ); }
			if ( ( equip & 8 ) == 8 ) { sb.Append( "<img src=\"chara-icons/RIT.png\" height=\"32\" width=\"24\" title=\"Rita\">" ); }
			if ( ( equip & 16 ) == 16 ) { sb.Append( "<img src=\"chara-icons/RAV.png\" height=\"32\" width=\"24\" title=\"Raven\">" ); }
			if ( ( equip & 32 ) == 32 ) { sb.Append( "<img src=\"chara-icons/JUD.png\" height=\"32\" width=\"24\" title=\"Judith\">" ); }
			if ( ( equip & 64 ) == 64 ) { sb.Append( "<img src=\"chara-icons/RAP.png\" height=\"32\" width=\"24\" title=\"Repede\">" ); }
			if ( ( equip & 128 ) == 128 ) { sb.Append( "<img src=\"chara-icons/FRE.png\" height=\"32\" width=\"24\" title=\"Flynn\">" ); }
			if ( version == GameVersion.PS3 && ( equip & 256 ) == 256 ) { sb.Append( "<img src=\"chara-icons/PAT.png\" height=\"32\" width=\"24\" title=\"Patty\">" ); }
		}
		public static void AppendPhysicalAilmentBitfieldAsImageString( StringBuilder sb, uint physAil ) {
			if ( ( physAil & 1 ) == 1 ) { sb.Append( "<img src=\"text-icons/icon-status-13.png\" height=\"16\" width=\"16\" title=\"Death\">" ); }
			if ( ( physAil & 2 ) == 2 ) { sb.Append( "<img src=\"text-icons/icon-status-01.png\" height=\"16\" width=\"16\" title=\"Poison\">" ); }
			if ( ( physAil & 4 ) == 4 ) { sb.Append( "<img src=\"text-icons/icon-status-02.png\" height=\"16\" width=\"16\" title=\"Paralysis\">" ); }
			if ( ( physAil & 8 ) == 8 ) { sb.Append( "<img src=\"text-icons/icon-status-03.png\" height=\"16\" width=\"16\" title=\"Petrification\">" ); }
			if ( ( physAil & 16 ) == 16 ) { sb.Append( "<img src=\"text-icons/icon-status-04.png\" height=\"16\" width=\"16\" title=\"Weakness\">" ); }
			if ( ( physAil & 32 ) == 32 ) { sb.Append( "<img src=\"text-icons/icon-status-05.png\" height=\"16\" width=\"16\" title=\"Sealed Artes\">" ); }
			if ( ( physAil & 64 ) == 64 ) { sb.Append( "<img src=\"text-icons/icon-status-06.png\" height=\"16\" width=\"16\" title=\"Sealed Skills\">" ); }
			if ( ( physAil & 128 ) == 128 ) { sb.Append( "<img src=\"text-icons/icon-status-07.png\" height=\"16\" width=\"16\" title=\"Contamination\">" ); }
		}
		public static void AppendFatalStrikeIcon( StringBuilder sb, uint fstype ) {
			switch ( fstype ) {
				case 0: sb.Append( "<img src=\"menu-icons/artes-13.png\" width=\"16\" height=\"16\">" ); break;
				case 1: sb.Append( "<img src=\"menu-icons/artes-15.png\" width=\"16\" height=\"16\">" ); break;
				case 2: sb.Append( "<img src=\"menu-icons/artes-14.png\" width=\"16\" height=\"16\">" ); break;
				default: sb.Append( "[Unknown Fatal Strike Type]" ); break;
			}
		}
		public static void AppendElementIcon( StringBuilder sb, T8BTEMST.Element element ) {
			switch ( element ) {
				case T8BTEMST.Element.Fire: sb.Append( "<img src=\"text-icons/icon-element-02.png\" width=\"16\" height=\"16\">" ); break;
				case T8BTEMST.Element.Earth: sb.Append( "<img src=\"text-icons/icon-element-04.png\" width=\"16\" height=\"16\">" ); break;
				case T8BTEMST.Element.Wind: sb.Append( "<img src=\"text-icons/icon-element-01.png\" width=\"16\" height=\"16\">" ); break;
				case T8BTEMST.Element.Water: sb.Append( "<img src=\"text-icons/icon-element-05.png\" width=\"16\" height=\"16\">" ); break;
				case T8BTEMST.Element.Light: sb.Append( "<img src=\"text-icons/icon-element-03.png\" width=\"16\" height=\"16\">" ); break;
				case T8BTEMST.Element.Darkness: sb.Append( "<img src=\"text-icons/icon-element-06.png\" width=\"16\" height=\"16\">" ); break;
				case T8BTEMST.Element.Physical: sb.Append( "<img src=\"text-icons/icon-element-07.png\" width=\"16\" height=\"16\">" ); break;
				default: sb.Append( "[Unknown Element]" ); break;
			}
		}
	}
}
