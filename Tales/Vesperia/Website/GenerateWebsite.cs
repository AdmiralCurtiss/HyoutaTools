using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.ItemDat;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.Website {
	public class GenerateWebsite {
		public static int Generate( List<string> args ) {
			string dir = @"c:\Dropbox\ToV\website\"; // output directory for generated files
			string dir360 = @"c:\Dropbox\ToV\360\";
			string dirPS3 = @"c:\Dropbox\ToV\PS3\";
			string databasePath;

			Console.WriteLine( "Initializing 360" );

			var site = new WebsiteGenerator();

			site.Version = GameVersion.X360;
			site.Items = new ItemDat.ItemDat( dir360 + @"item.svo.ext\ITEM.DAT" );
			site.StringDic = new TSS.TSSFile( dir360 + @"string_dic_uk.so", Util.GameTextEncoding.UTF8 );
			site.Artes = new T8BTMA.T8BTMA( dir360 + @"btl.svo.ext\BTL_PACK_UK.DAT.ext\0004.ext\ALL.0000" );
			site.Skills = new T8BTSK.T8BTSK( dir360 + @"btl.svo.ext\BTL_PACK_UK.DAT.ext\0010.ext\ALL.0000" );
			site.Enemies = new T8BTEMST.T8BTEMST( dir360 + @"btl.svo.ext\BTL_PACK_UK.DAT.ext\0005.ext\ALL.0000" );
			site.EnemyGroups = new T8BTEMGP.T8BTEMGP( dir360 + @"btl.svo.ext\BTL_PACK_UK.DAT.ext\0006.ext\ALL.0000" );
			site.EncounterGroups = new T8BTEMEG.T8BTEMEG( dir360 + @"btl.svo.ext\BTL_PACK_UK.DAT.ext\0007.ext\ALL.0000" );
			site.Recipes = new COOKDAT.COOKDAT( dir360 + @"cook.svo.ext\COOKDATA.BIN" );
			site.Locations = new WRLDDAT.WRLDDAT( dir360 + @"menu.svo.ext\WORLDDATA.BIN" );
			site.Synopsis = new SYNPDAT.SYNPDAT( dir360 + @"menu.svo.ext\SYNOPSISDATA.BIN" );
			site.Titles = new FAMEDAT.FAMEDAT( dir360 + @"menu.svo.ext\FAMEDATA.BIN" );
			site.GradeShop = new T8BTGR.T8BTGR( dir360 + @"btl.svo.ext\BTL_PACK_UK.DAT.ext\0016.ext\ALL.0000" );
			site.BattleBook = new BTLBDAT.BTLBDAT( dir360 + @"menu.svo.ext\BATTLEBOOKDATA.BIN" );
			site.Strategy = new T8BTTA.T8BTTA( dir360 + @"btl.svo.ext\BTL_PACK_UK.DAT.ext\0011.ext\ALL.0000" );
			site.BattleVoicesEnd = new T8BTVA.T8BTVA( dir360 + @"btl.svo.ext\BTL_PACK_UK.DAT.ext\0019.ext\END.0000" );
			site.Skits = new TO8CHLI.TO8CHLI( dir360 + @"chat.svo.ext\CHAT.DAT.dec" );
			site.SkitText = new Dictionary<string, TO8CHTX.ChatFile>();
			for ( int i = 0; i < site.Skits.SkitInfoList.Count; ++i ) {
				string name = site.Skits.SkitInfoList[i].RefString;
				try {
					bool isUtf8 = name != "VC084";
					TO8CHTX.ChatFile chatFile = new TO8CHTX.ChatFile( dir360 + @"chat.svo.ext\" + name + @"UK.DAT.dec.ext\0003", isUtf8 ? Util.GameTextEncoding.UTF8 : Util.GameTextEncoding.ShiftJIS );
					site.SkitText.Add( name, chatFile );
				} catch ( DirectoryNotFoundException ) {
					Console.WriteLine( "Couldn't find 360 chat file " + name + "!" );
				}
			}
			site.Shops = new ShopData.ShopData( dir360 + @"scenario0", 0x1A780, 0x420 / 32, 0x8F8, 0x13780 / 56 );
			site.InGameIdDict = site.StringDic.GenerateInGameIdDictionary();
			site.IconsWithItems = new uint[] { 35, 36, 37, 60, 38, 1, 4, 12, 6, 5, 13, 14, 15, 7, 52, 51, 9, 16, 18, 2, 17, 19, 10, 20, 21, 22, 23, 24, 25, 26, 27, 56, 30, 28, 32, 31, 33, 29, 34, 41, 42, 43, 44, 45, 57, 61, 63, 39, 3, 40 };
			site.Records = WebsiteGenerator.GenerateRecordsStringDicList( site.Version );
			site.Settings = WebsiteGenerator.GenerateSettingsStringDicList( site.Version );
			site.BattleTextFiles = WebsiteGenerator.LoadBattleTextTSS( dir360 + @"btl.svo.ext\BTL_PACK_UK.DAT.ext\0003.ext\", Util.GameTextEncoding.UTF8 );

			site.ScenarioFiles = new Dictionary<string, ScenarioFile.ScenarioFile>();
			site.ScenarioGroupsStory = site.CreateScenarioIndexGroups( ScenarioType.Story, dir360 + @"scenarioDB", dir360 + @"scenario_uk.dat.ext\", encoding: Util.GameTextEncoding.UTF8 );
			site.ScenarioGroupsSidequests = site.CreateScenarioIndexGroups( ScenarioType.Sidequests, dir360 + @"scenarioDB", dir360 + @"scenario_uk.dat.ext\", encoding: Util.GameTextEncoding.UTF8 );
			site.ScenarioGroupsMaps = site.CreateScenarioIndexGroups( ScenarioType.Maps, dir360 + @"scenarioDB", dir360 + @"scenario_uk.dat.ext\", encoding: Util.GameTextEncoding.UTF8 );
			site.ScenarioAddSkits( site.ScenarioGroupsStory );

			// copy over Japanese stuff into UK StringDic
			var StringDicUs = new TSS.TSSFile( dir360 + @"string_dic_us.so", Util.GameTextEncoding.UTF8 );
			var IdDictUs = StringDicUs.GenerateInGameIdDictionary();
			foreach ( var kvp in IdDictUs ) {
				site.InGameIdDict[kvp.Key].StringJpn = kvp.Value.StringJpn;
			}

			databasePath = Path.Combine( dir, "_db-" + site.Version + ".sqlite" );
			System.IO.File.Delete( databasePath );
			new GenerateDatabase( site, new System.Data.SQLite.SQLiteConnection( "Data Source=" + databasePath ) ).ExportAll();

			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Item, site.Version, false ) ), site.GenerateHtmlItems(), Encoding.UTF8 );
			foreach ( uint i in site.IconsWithItems ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Item, site.Version, false, icon: (int)i ) ), site.GenerateHtmlItems( icon: i ), Encoding.UTF8 );
			}
			for ( uint i = 2; i < 12; ++i ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Item, site.Version, false, category: (int)i ) ), site.GenerateHtmlItems( category: i ), Encoding.UTF8 );
			}
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Enemy, site.Version, false ) ), site.GenerateHtmlEnemies(), Encoding.UTF8 );
			for ( int i = 0; i < 9; ++i ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Enemy, site.Version, false, category: (int)i ) ), site.GenerateHtmlEnemies( category: i ), Encoding.UTF8 );
			}
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.EnemyGroup, site.Version, false ) ), site.GenerateHtmlEnemyGroups(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.EncounterGroup, site.Version, false ) ), site.GenerateHtmlEncounterGroups(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Skill, site.Version, false ) ), site.GenerateHtmlSkills(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Arte, site.Version, false ) ), site.GenerateHtmlArtes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Synopsis, site.Version, false ) ), site.GenerateHtmlSynopsis(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Recipe, site.Version, false ) ), site.GenerateHtmlRecipes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Location, site.Version, false ) ), site.GenerateHtmlLocations(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Strategy, site.Version, false ) ), site.GenerateHtmlStrategy(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Shop, site.Version, false ) ), site.GenerateHtmlShops(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Title, site.Version, false ) ), site.GenerateHtmlTitles(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.BattleBook, site.Version, false ) ), site.GenerateHtmlBattleBook(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Record, site.Version, false ) ), site.GenerateHtmlRecords(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Settings, site.Version, false ) ), site.GenerateHtmlSettings(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.GradeShop, site.Version, false ) ), site.GenerateHtmlGradeShop(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.PostBattleVoices, site.Version, false ) ), site.GenerateHtmlBattleVoicesEnd(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.SkitInfo, site.Version, false ) ), site.GenerateHtmlSkitInfo(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.SkitIndex, site.Version, false ) ), site.GenerateHtmlSkitIndex(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.ScenarioStoryIndex, site.Version, false ) ), site.ScenarioProcessGroupsToHtml( site.ScenarioGroupsStory, ScenarioType.Story, site.Version ), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.ScenarioSidequestIndex, site.Version, false ) ), site.ScenarioProcessGroupsToHtml( site.ScenarioGroupsSidequests, ScenarioType.Sidequests, site.Version ), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.ScenarioMapIndex, site.Version, false ) ), site.ScenarioProcessGroupsToHtml( site.ScenarioGroupsMaps, ScenarioType.Maps, site.Version ), Encoding.UTF8 );

			WebsiteGenerator site360 = site;
			site = new WebsiteGenerator();

			Console.WriteLine( "Initializing PS3" );

			site.Version = GameVersion.PS3;
			var PS3StringDic = new TSS.TSSFile( dirPS3 + @"mod\string.svo.ext\STRING_DIC.SO", Util.GameTextEncoding.ShiftJIS );
			site.StringDic = PS3StringDic;
			site.Items = new ItemDat.ItemDat( dirPS3 + @"orig\item.svo.ext\ITEM.DAT" );
			site.Artes = new T8BTMA.T8BTMA( dirPS3 + @"orig\btl.svo.ext\BTL_PACK.DAT.ext\0004.ext\ALL.0000" );
			site.Skills = new T8BTSK.T8BTSK( dirPS3 + @"orig\btl.svo.ext\BTL_PACK.DAT.ext\0010.ext\ALL.0000" );
			site.Enemies = new T8BTEMST.T8BTEMST( dirPS3 + @"orig\btl.svo.ext\BTL_PACK.DAT.ext\0005.ext\ALL.0000" );
			site.EnemyGroups = new T8BTEMGP.T8BTEMGP( dirPS3 + @"orig\btl.svo.ext\BTL_PACK.DAT.ext\0006.ext\ALL.0000" );
			site.EncounterGroups = new T8BTEMEG.T8BTEMEG( dirPS3 + @"orig\btl.svo.ext\BTL_PACK.DAT.ext\0007.ext\ALL.0000" );
			site.Recipes = new COOKDAT.COOKDAT( dirPS3 + @"orig\menu.svo.ext\COOKDATA.BIN" );
			site.Locations = new WRLDDAT.WRLDDAT( dirPS3 + @"orig\menu.svo.ext\WORLDDATA.BIN" );
			site.Synopsis = new SYNPDAT.SYNPDAT( dirPS3 + @"orig\menu.svo.ext\SYNOPSISDATA.BIN" );
			site.Titles = new FAMEDAT.FAMEDAT( dirPS3 + @"orig\menu.svo.ext\FAMEDATA.BIN" );
			site.GradeShop = new T8BTGR.T8BTGR( dirPS3 + @"orig\btl.svo.ext\BTL_PACK.DAT.ext\0016.ext\ALL.0000" );
			site.BattleBook = new BTLBDAT.BTLBDAT( dirPS3 + @"orig\menu.svo.ext\BATTLEBOOKDATA.BIN" );
			site.Strategy = new T8BTTA.T8BTTA( dirPS3 + @"orig\btl.svo.ext\BTL_PACK.DAT.ext\0011.ext\ALL.0000" );
			site.Skits = new TO8CHLI.TO8CHLI( dirPS3 + @"orig\chat.svo.ext\CHAT.DAT.dec" );
			site.SearchPoints = new TOVSEAF.TOVSEAF( dirPS3 + @"orig\npc.svo.ext\FIELD.DAT.dec.ext\0005.dec" );
			site.SkitText = new Dictionary<string, TO8CHTX.ChatFile>();
			for ( int i = 0; i < site.Skits.SkitInfoList.Count; ++i ) {
				string name = site.Skits.SkitInfoList[i].RefString;
				string filenameOrig = dirPS3 + @"orig\chat.svo.ext\" + name + @"J.DAT.dec.ext\0003";
				string filenameMod = dirPS3 + @"mod\chat.svo.ext\" + name + @"J.DAT.dec.ext\0003";
				var chatFile = new TO8CHTX.ChatFile( filenameOrig, Util.GameTextEncoding.ShiftJIS );
				var chatFileMod = new TO8CHTX.ChatFile( filenameMod, Util.GameTextEncoding.ShiftJIS );
				Util.Assert( chatFile.Lines.Length == chatFileMod.Lines.Length );
				for ( int j = 0; j < chatFile.Lines.Length; ++j ) {
					chatFile.Lines[j].SENG = chatFileMod.Lines[j].SJPN;
					chatFile.Lines[j].SNameEnglishNotUsedByGame = chatFileMod.Lines[j].SName;
				}
				site.SkitText.Add( name, chatFile );
			}
			site.Shops = new ShopData.ShopData( dirPS3 + @"mod\scenario0", 0x1C9BC, 0x460 / 32, 0x980, 0x14CB8 / 56 );
			site.NecropolisFloors = new T8BTXTM.T8BTXTMA( dirPS3 + @"orig\btl.svo.ext\BTL_PACK.DAT.ext\0021.ext\ALL.0000" );
			site.NecropolisTreasures = new T8BTXTM.T8BTXTMT( dirPS3 + @"orig\btl.svo.ext\BTL_PACK.DAT.ext\0022.ext\ALL.0000" );
			var filenames = System.IO.Directory.GetFiles( dirPS3 + @"orig\btl.svo.ext\BTL_PACK.DAT.ext\0023.ext\" );
			site.NecropolisMaps = new Dictionary<string, T8BTXTM.T8BTXTMM>( filenames.Length );
			for ( int i = 0; i < filenames.Length; ++i ) {
				site.NecropolisMaps.Add( System.IO.Path.GetFileNameWithoutExtension( filenames[i] ), new T8BTXTM.T8BTXTMM( filenames[i] ) );
			}
			site.TrophyJp = HyoutaTools.Trophy.TrophyConfNode.ReadTropSfmWithTropConf( dirPS3 + @"orig\TROPHY.TRP.ext\TROP.SFM", dirPS3 + @"orig\TROPHY.TRP.ext\TROPCONF.SFM" );
			site.TrophyEn = HyoutaTools.Trophy.TrophyConfNode.ReadTropSfmWithTropConf( dirPS3 + @"mod\TROPHY.TRP.ext\TROP.SFM", dirPS3 + @"mod\TROPHY.TRP.ext\TROPCONF.SFM" );
			site.ScenarioFiles = new Dictionary<string, ScenarioFile.ScenarioFile>();
			site.InGameIdDict = site.StringDic.GenerateInGameIdDictionary();
			site.IconsWithItems = new uint[] { 35, 36, 37, 60, 38, 1, 4, 12, 6, 5, 13, 14, 15, 7, 52, 51, 53, 9, 16, 18, 2, 17, 19, 10, 54, 20, 21, 22, 23, 24, 25, 26, 27, 56, 30, 28, 32, 31, 33, 29, 34, 41, 42, 43, 44, 45, 57, 61, 63, 39, 3, 40 };
			site.Records = WebsiteGenerator.GenerateRecordsStringDicList( site.Version );
			site.Settings = WebsiteGenerator.GenerateSettingsStringDicList( site.Version );
			site.BattleTextFiles = WebsiteGenerator.LoadBattleTextScfombin( dirPS3 + @"orig\btl.svo.ext\BTL_PACK.DAT.ext\0003.ext\", dirPS3 + @"mod\btl.svo.ext\BTL_PACK.DAT.ext\0003.ext\" );

			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Item, site.Version, false ) ), site.GenerateHtmlItems(), Encoding.UTF8 );
			foreach ( uint i in site.IconsWithItems ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Item, site.Version, false, icon: (int)i ) ), site.GenerateHtmlItems( icon: i ), Encoding.UTF8 );
			}
			for ( uint i = 2; i < 12; ++i ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Item, site.Version, false, category: (int)i ) ), site.GenerateHtmlItems( category: i ), Encoding.UTF8 );
			}
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Enemy, site.Version, false ) ), site.GenerateHtmlEnemies(), Encoding.UTF8 );
			for ( int i = 0; i < 9; ++i ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Enemy, site.Version, false, category: (int)i ) ), site.GenerateHtmlEnemies( category: i ), Encoding.UTF8 );
			}
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.EnemyGroup, site.Version, false ) ), site.GenerateHtmlEnemyGroups(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.EncounterGroup, site.Version, false ) ), site.GenerateHtmlEncounterGroups(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Skill, site.Version, false ) ), site.GenerateHtmlSkills(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Arte, site.Version, false ) ), site.GenerateHtmlArtes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Synopsis, site.Version, false ) ), site.GenerateHtmlSynopsis(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Recipe, site.Version, false ) ), site.GenerateHtmlRecipes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Location, site.Version, false ) ), site.GenerateHtmlLocations(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Strategy, site.Version, false ) ), site.GenerateHtmlStrategy(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Shop, site.Version, false ) ), site.GenerateHtmlShops(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Title, site.Version, false ) ), site.GenerateHtmlTitles(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.BattleBook, site.Version, false ) ), site.GenerateHtmlBattleBook(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Record, site.Version, false ) ), site.GenerateHtmlRecords(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Settings, site.Version, false ) ), site.GenerateHtmlSettings(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.GradeShop, site.Version, false ) ), site.GenerateHtmlGradeShop(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Trophy, site.Version, false ) ), site.GenerateHtmlTrophies(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.SkitInfo, site.Version, false ) ), site.GenerateHtmlSkitInfo(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.SkitIndex, site.Version, false ) ), site.GenerateHtmlSkitIndex(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.SearchPoint, site.Version, false ) ), site.GenerateHtmlSearchPoints(), Encoding.UTF8 );
			site.SearchPoints.GenerateMap( new System.Drawing.Bitmap( dir + @"map\U_WORLDNAVI00_5120x4096_point.png" ) ).Save( dir + @"PS3-SearchPoint.png" );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.NecropolisMap, site.Version, false ) ), site.GenerateHtmlNecropolis( dir, false ), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.NecropolisEnemy, site.Version, false ) ), site.GenerateHtmlNecropolis( dir, true ), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.StringDic, site.Version, false ) ), site.GenerateHtmlNpc( dirPS3 ), Encoding.UTF8 );

			site.ScenarioGroupsStory = site.CreateScenarioIndexGroups( ScenarioType.Story, dirPS3 + @"scenarioDB", dirPS3 + @"orig\scenario.dat.ext\", dirPS3 + @"mod\scenario.dat.ext\" );
			site.ScenarioGroupsSidequests = site.CreateScenarioIndexGroups( ScenarioType.Sidequests, dirPS3 + @"scenarioDB", dirPS3 + @"orig\scenario.dat.ext\", dirPS3 + @"mod\scenario.dat.ext\" );
			site.ScenarioGroupsMaps = site.CreateScenarioIndexGroups( ScenarioType.Maps, dirPS3 + @"scenarioDB", dirPS3 + @"orig\scenario.dat.ext\", dirPS3 + @"mod\scenario.dat.ext\" );
			site.ScenarioAddSkits( site.ScenarioGroupsStory );

			databasePath = Path.Combine( dir, "_db-" + site.Version + ".sqlite" );
			System.IO.File.Delete( databasePath );
			new GenerateDatabase( site, new System.Data.SQLite.SQLiteConnection( "Data Source=" + databasePath ), site360 ).ExportAll();

			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.ScenarioStoryIndex, site.Version, false ) ), site.ScenarioProcessGroupsToHtml( site.ScenarioGroupsStory, ScenarioType.Story, site.Version ), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.ScenarioSidequestIndex, site.Version, false ) ), site.ScenarioProcessGroupsToHtml( site.ScenarioGroupsSidequests, ScenarioType.Sidequests, site.Version ), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.ScenarioMapIndex, site.Version, false ) ), site.ScenarioProcessGroupsToHtml( site.ScenarioGroupsMaps, ScenarioType.Maps, site.Version ), Encoding.UTF8 );

			return 0;
		}
	}
}
