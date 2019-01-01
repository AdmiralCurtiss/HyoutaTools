using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.ItemDat;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.Website {
	public class GenerateWebsite {
		public static Stream TryCreateStreamFromPath( string path ) {
			try {
				return new FileStream( path, FileMode.Open, FileAccess.Read );
			} catch ( FileNotFoundException ) {
				return null;
			} catch ( DirectoryNotFoundException ) {
				return null;
			}
		}

		public static Stream TryGetItemDat( string basepath, GameLocale locale, GameVersion version ) {
			return TryCreateStreamFromPath( Path.Combine( basepath, "item.svo.ext", "ITEM.DAT" ) );
		}
		public static Stream TryGetStringDic( string basepath, GameLocale locale, GameVersion version ) {
			if ( version == GameVersion.X360 ) {
				return TryCreateStreamFromPath( Path.Combine( basepath, "string_dic_" + locale.ToString().ToLowerInvariant() + ".so" ) );
			} else {
				return TryCreateStreamFromPath( Path.Combine( basepath, "string.svo.ext", "STRING_DIC.SO" ) );
			}
		}
		public static string ConstructBtlPackPath( string basepath, GameLocale locale, GameVersion version ) {
			if ( version == GameVersion.X360 ) {
				return Path.Combine( basepath, "btl.svo.ext", "BTL_PACK_" + locale.ToString().ToUpperInvariant() + ".DAT.ext" );
			} else {
				return Path.Combine( basepath, "btl.svo.ext", "BTL_PACK.DAT.ext" );
			}
		}
		public static Stream TryGetBtlPack( string basepath, int folderIndex, string filename, GameLocale locale, GameVersion version ) {
			return TryCreateStreamFromPath( Path.Combine( ConstructBtlPackPath( basepath, locale, version ), folderIndex.ToString( "D4" ) + ".ext", filename ) );
		}
		public static Stream TryGetArtes( string basepath, GameLocale locale, GameVersion version ) {
			return TryGetBtlPack( basepath, 4, "ALL.0000", locale, version );
		}
		public static Stream TryGetSkills( string basepath, GameLocale locale, GameVersion version ) {
			return TryGetBtlPack( basepath, 10, "ALL.0000", locale, version );
		}
		public static Stream TryGetEnemies( string basepath, GameLocale locale, GameVersion version ) {
			return TryGetBtlPack( basepath, 5, "ALL.0000", locale, version );
		}
		public static Stream TryGetEnemyGroups( string basepath, GameLocale locale, GameVersion version ) {
			return TryGetBtlPack( basepath, 6, "ALL.0000", locale, version );
		}
		public static Stream TryGetEncounterGroups( string basepath, GameLocale locale, GameVersion version ) {
			return TryGetBtlPack( basepath, 7, "ALL.0000", locale, version );
		}
		public static Stream TryGetGradeShop( string basepath, GameLocale locale, GameVersion version ) {
			return TryGetBtlPack( basepath, 16, "ALL.0000", locale, version );
		}
		public static Stream TryGetStrategy( string basepath, GameLocale locale, GameVersion version ) {
			return TryGetBtlPack( basepath, 11, "ALL.0000", locale, version );
		}
		public static Stream TryGetBattleVoicesEnd( string basepath, GameLocale locale, GameVersion version ) {
			return TryGetBtlPack( basepath, 19, "END.0000", locale, version );
		}
		public static Stream TryGetNecropolisFloors( string basepath, GameLocale locale, GameVersion version ) {
			return TryGetBtlPack( basepath, 21, "ALL.0000", locale, version );
		}
		public static Stream TryGetNecropolisTreasures( string basepath, GameLocale locale, GameVersion version ) {
			return TryGetBtlPack( basepath, 22, "ALL.0000", locale, version );
		}
		public static Stream TryGetNecropolisMap( string basepath, string mapname, GameLocale locale, GameVersion version ) {
			string folder = Path.Combine( ConstructBtlPackPath( basepath, locale, version ), 23.ToString( "D4" ) + ".ext" );
			var files = System.IO.Directory.GetFiles( folder, mapname + ".*", SearchOption.TopDirectoryOnly );
			Util.Assert( files.Length == 1 );
			return TryCreateStreamFromPath( files[0] );
		}
		public static List<string> GetBattleScenarioFileNames( string basepath, GameLocale locale, GameVersion version ) {
			string folder = Path.Combine( ConstructBtlPackPath( basepath, locale, version ), 3.ToString( "D4" ) + ".ext" );
			List<string> names = new List<string>();
			foreach ( string f in System.IO.Directory.GetFiles( folder, "BTL_*", SearchOption.TopDirectoryOnly ) ) {
				names.Add( Path.GetFileName( f ).Split( '.' )[0] );
			}
			return names;
		}
		public static Stream TryGetBattleScenarioFile( string basepath, string epname, GameLocale locale, GameVersion version ) {
			string folder = Path.Combine( ConstructBtlPackPath( basepath, locale, version ), 3.ToString( "D4" ) + ".ext" );
			var files = System.IO.Directory.GetFiles( folder, epname + ".*", SearchOption.TopDirectoryOnly );
			Util.Assert( files.Length == 1 );
			return TryCreateStreamFromPath( files[0] );
		}
		public static Stream TryGetRecipes( string basepath, GameLocale locale, GameVersion version ) {
			if ( version == GameVersion.X360 ) {
				return TryCreateStreamFromPath( Path.Combine( basepath, "cook.svo.ext", "COOKDATA.BIN" ) );
			} else {
				return TryCreateStreamFromPath( Path.Combine( basepath, "menu.svo.ext", "COOKDATA.BIN" ) );
			}
		}
		public static Stream TryGetLocations( string basepath, GameLocale locale, GameVersion version ) {
			return TryCreateStreamFromPath( Path.Combine( basepath, "menu.svo.ext", "WORLDDATA.BIN" ) );
		}
		public static Stream TryGetSynopsis( string basepath, GameLocale locale, GameVersion version ) {
			return TryCreateStreamFromPath( Path.Combine( basepath, "menu.svo.ext", "SYNOPSISDATA.BIN" ) );
		}
		public static Stream TryGetTitles( string basepath, GameLocale locale, GameVersion version ) {
			return TryCreateStreamFromPath( Path.Combine( basepath, "menu.svo.ext", "FAMEDATA.BIN" ) );
		}
		public static Stream TryGetBattleBook( string basepath, GameLocale locale, GameVersion version ) {
			return TryCreateStreamFromPath( Path.Combine( basepath, "menu.svo.ext", "BATTLEBOOKDATA.BIN" ) );
		}
		public static Stream TryGetSkitMetadata( string basepath, GameLocale locale, GameVersion version ) {
			return TryCreateStreamFromPath( Path.Combine( basepath, "chat.svo.ext", "CHAT.DAT.dec" ) );
		}
		public static Stream TryGetSkitText( string basepath, string skit, GameLocale locale, GameVersion version ) {
			return TryCreateStreamFromPath( Path.Combine( basepath, "chat.svo.ext", skit + locale.ToString().ToUpperInvariant() + ".DAT.dec.ext", "0003" ) );
		}
		public static Stream TryGetSearchPoints( string basepath, GameLocale locale, GameVersion version ) {
			return TryCreateStreamFromPath( Path.Combine( basepath, "npc.svo.ext", "FIELD.DAT.dec.ext", "0005.dec" ) );
		}
		public static Stream TryGetScenarioFile( string basepath, int fileIndex, GameLocale locale, GameVersion version ) {
			if ( version == GameVersion.X360 ) {
				return TryCreateStreamFromPath( Path.Combine( basepath, "scenario_" + locale.ToString().ToLowerInvariant() + ".dat.ext", fileIndex.ToString( "D1" ) + ".d" ) );
			} else {
				return TryCreateStreamFromPath( Path.Combine( basepath, "scenario.dat.ext", fileIndex.ToString( "D1" ) + ".d" ) );
			}
		}
		public static Stream TryGetMaplist( string basepath, GameLocale locale, GameVersion version ) {
			return TryCreateStreamFromPath( Path.Combine( basepath, "map.svo.ext", "MAPLIST.DAT" ) );
		}

		public static int Generate( List<string> args ) {
			string dir = @"c:\Dropbox\ToV\website\"; // output directory for generated files
			string dir360 = @"c:\Dropbox\ToV\360\";
			string dirPS3 = @"c:\Dropbox\ToV\PS3\";
			string dirPS3orig = @"c:\Dropbox\ToV\PS3\orig\";
			string dirPS3mod = @"c:\Dropbox\ToV\PS3\mod\";

			Console.WriteLine( "Initializing 360" );

			Util.Endianness endian = Util.Endianness.BigEndian;
			WebsiteGenerator site = LoadWebsiteGenerator( dir360, GameVersion.X360, GameLocale.UK, endian, Util.GameTextEncoding.UTF8 );

			site.InGameIdDict = site.StringDic.GenerateInGameIdDictionary();

			// copy over Japanese stuff into UK StringDic
			var StringDicUs = new TSS.TSSFile( TryGetStringDic( dir360, GameLocale.US, GameVersion.X360 ), Util.GameTextEncoding.UTF8 );
			var IdDictUs = StringDicUs.GenerateInGameIdDictionary();
			foreach ( var kvp in IdDictUs ) {
				site.InGameIdDict[kvp.Key].StringJpn = kvp.Value.StringJpn;
			}

			ExportToWebsite( site, dir );

			WebsiteGenerator site360 = site;
			site = new WebsiteGenerator();

			Console.WriteLine( "Initializing PS3" );

			site = LoadWebsiteGenerator( dirPS3orig, GameVersion.PS3, GameLocale.J, endian, Util.GameTextEncoding.ShiftJIS );

			// patch original PS3 data with fantranslation
			var stringDicTranslated = new TSS.TSSFile( TryGetStringDic( dirPS3mod, site.Locale, site.Version ), Util.GameTextEncoding.ShiftJIS );
			Util.Assert( site.StringDic.Entries.Length == stringDicTranslated.Entries.Length );
			for ( int i = 0; i < site.StringDic.Entries.Length; ++i ) {
				Util.Assert( site.StringDic.Entries[i].inGameStringId == stringDicTranslated.Entries[i].inGameStringId );
				site.StringDic.Entries[i].StringEng = stringDicTranslated.Entries[i].StringJpn;
			}
			foreach ( var kvp in site.ScenarioFiles ) {
				if ( kvp.Value.EntryList.Count > 0 && kvp.Value.Metadata.ScenarioDatIndex >= 0 ) {
					Stream streamMod = TryGetScenarioFile( dirPS3mod, kvp.Value.Metadata.ScenarioDatIndex, GameLocale.J, GameVersion.PS3 );
					if ( streamMod != null ) {
						var scenarioMod = new ScenarioFile.ScenarioFile( streamMod, Util.GameTextEncoding.ShiftJIS );
						Util.Assert( kvp.Value.EntryList.Count == scenarioMod.EntryList.Count );
						for ( int i = 0; i < kvp.Value.EntryList.Count; ++i ) {
							kvp.Value.EntryList[i].EnName = scenarioMod.EntryList[i].JpName;
							kvp.Value.EntryList[i].EnText = scenarioMod.EntryList[i].JpText;
						}
					}
				}
			}
			foreach ( var kvp in site.BattleTextFiles ) {
				if ( kvp.Value.EntryList.Count > 0 ) {
					Stream streamMod = TryGetBattleScenarioFile( dirPS3mod, kvp.Key, GameLocale.J, GameVersion.PS3 );
					if ( streamMod != null ) {
						var scenarioMod = new SCFOMBIN.SCFOMBIN( streamMod, Util.Endianness.BigEndian, Util.GameTextEncoding.ShiftJIS, kvp.Value.TextPointerLocationDiff );
						Util.Assert( kvp.Value.EntryList.Count == scenarioMod.EntryList.Count );
						for ( int i = 0; i < kvp.Value.EntryList.Count; ++i ) {
							kvp.Value.EntryList[i].EnName = scenarioMod.EntryList[i].JpName;
							kvp.Value.EntryList[i].EnText = scenarioMod.EntryList[i].JpText;
						}
					}
				}
			}
			foreach ( var kvp in site.SkitText ) {
				var chatFile = kvp.Value;
				Stream streamMod = TryGetSkitText( dirPS3mod, kvp.Key, site.Locale, site.Version );
				var chatFileMod = new TO8CHTX.ChatFile( streamMod, endian, Util.GameTextEncoding.ShiftJIS );
				Util.Assert( chatFile.Lines.Length == chatFileMod.Lines.Length );
				for ( int j = 0; j < chatFile.Lines.Length; ++j ) {
					chatFile.Lines[j].SENG = chatFileMod.Lines[j].SJPN;
					chatFile.Lines[j].SNameEnglishNotUsedByGame = chatFileMod.Lines[j].SName;
				}
			}
			site.TrophyEn = HyoutaTools.Trophy.TrophyConfNode.ReadTropSfmWithTropConf( dirPS3 + @"mod\TROPHY.TRP.ext\TROP.SFM", dirPS3 + @"mod\TROPHY.TRP.ext\TROPCONF.SFM" );
			site.InGameIdDict = site.StringDic.GenerateInGameIdDictionary();

			ExportToWebsite( site, dir, site360 );

			return 0;
		}

		public static WebsiteGenerator LoadWebsiteGenerator( string gameDataPath, GameVersion version, GameLocale locale, Util.Endianness endian, Util.GameTextEncoding encoding ) {
			WebsiteGenerator site = new WebsiteGenerator();
			site.Locale = locale;
			site.Version = version;

			site.Items = new ItemDat.ItemDat( TryGetItemDat( gameDataPath, site.Locale, site.Version ), endian );
			site.StringDic = new TSS.TSSFile( TryGetStringDic( gameDataPath, site.Locale, site.Version ), encoding );
			site.Artes = new T8BTMA.T8BTMA( TryGetArtes( gameDataPath, site.Locale, site.Version ), endian );
			site.Skills = new T8BTSK.T8BTSK( TryGetSkills( gameDataPath, site.Locale, site.Version ), endian );
			site.Enemies = new T8BTEMST.T8BTEMST( TryGetEnemies( gameDataPath, site.Locale, site.Version ), endian );
			site.EnemyGroups = new T8BTEMGP.T8BTEMGP( TryGetEnemyGroups( gameDataPath, site.Locale, site.Version ), endian );
			site.EncounterGroups = new T8BTEMEG.T8BTEMEG( TryGetEncounterGroups( gameDataPath, site.Locale, site.Version ), endian );
			site.Recipes = new COOKDAT.COOKDAT( TryGetRecipes( gameDataPath, site.Locale, site.Version ), endian );
			site.Locations = new WRLDDAT.WRLDDAT( TryGetLocations( gameDataPath, site.Locale, site.Version ), endian );
			site.Synopsis = new SYNPDAT.SYNPDAT( TryGetSynopsis( gameDataPath, site.Locale, site.Version ), endian );
			site.Titles = new FAMEDAT.FAMEDAT( TryGetTitles( gameDataPath, site.Locale, site.Version ), endian );
			site.GradeShop = new T8BTGR.T8BTGR( TryGetGradeShop( gameDataPath, site.Locale, site.Version ), endian );
			site.BattleBook = new BTLBDAT.BTLBDAT( TryGetBattleBook( gameDataPath, site.Locale, site.Version ), endian );
			site.Strategy = new T8BTTA.T8BTTA( TryGetStrategy( gameDataPath, site.Locale, site.Version ), endian );
			if ( site.Version == GameVersion.X360 ) {
				site.BattleVoicesEnd = new T8BTVA.T8BTVA( TryGetBattleVoicesEnd( gameDataPath, site.Locale, site.Version ), endian );
			}
			if ( site.Version == GameVersion.PS3 ) {
				site.SearchPoints = new TOVSEAF.TOVSEAF( TryGetSearchPoints( gameDataPath, site.Locale, site.Version ), endian );
			}
			site.Skits = new TO8CHLI.TO8CHLI( TryGetSkitMetadata( gameDataPath, site.Locale, site.Version ), endian );
			site.SkitText = new Dictionary<string, TO8CHTX.ChatFile>();
			for ( int i = 0; i < site.Skits.SkitInfoList.Count; ++i ) {
				string name = site.Skits.SkitInfoList[i].RefString;
				Stream stream = TryGetSkitText( gameDataPath, name, site.Locale, site.Version );
				if ( stream != null ) {
					bool forceShiftJis = name == "VC084" && version == GameVersion.X360 && ( locale == GameLocale.UK || locale == GameLocale.US );
					TO8CHTX.ChatFile chatFile = new TO8CHTX.ChatFile( stream, endian, forceShiftJis ? Util.GameTextEncoding.ShiftJIS : encoding );
					site.SkitText.Add( name, chatFile );
				} else {
					Console.WriteLine( "Couldn't find chat file " + name + "! (" + version + ", " + locale + ")" );
				}
			}

			site.Records = WebsiteGenerator.GenerateRecordsStringDicList( site.Version );
			site.Settings = WebsiteGenerator.GenerateSettingsStringDicList( site.Version );

			site.ScenarioFiles = new Dictionary<string, ScenarioFile.ScenarioFile>();

			if ( version == GameVersion.X360 ) {
				site.Shops = new ShopData.ShopData( TryGetScenarioFile( gameDataPath, 0, site.Locale, site.Version ), 0x1A780, 0x420 / 32, 0x8F8, 0x13780 / 56, endian );
				site.IconsWithItems = new uint[] { 35, 36, 37, 60, 38, 1, 4, 12, 6, 5, 13, 14, 15, 7, 52, 51,     9, 16, 18, 2, 17, 19, 10,     20, 21, 22, 23, 24, 25, 26, 27, 56, 30, 28, 32, 31, 33, 29, 34, 41, 42, 43, 44, 45, 57, 61, 63, 39, 3, 40 };
				site.BattleTextFiles = WebsiteGenerator.LoadBattleTextTSS( gameDataPath, site.Locale, site.Version, endian, encoding );
			} else {
				site.Shops = new ShopData.ShopData( TryGetScenarioFile( gameDataPath, 0, site.Locale, site.Version ), 0x1C9BC, 0x460 / 32, 0x980, 0x14CB8 / 56, endian );
				site.IconsWithItems = new uint[] { 35, 36, 37, 60, 38, 1, 4, 12, 6, 5, 13, 14, 15, 7, 52, 51, 53, 9, 16, 18, 2, 17, 19, 10, 54, 20, 21, 22, 23, 24, 25, 26, 27, 56, 30, 28, 32, 31, 33, 29, 34, 41, 42, 43, 44, 45, 57, 61, 63, 39, 3, 40 };
				site.BattleTextFiles = WebsiteGenerator.LoadBattleTextScfombin( gameDataPath, site.Locale, site.Version, endian, encoding );
			}

			if ( version == GameVersion.PS3 ) {
				site.NecropolisFloors = new T8BTXTM.T8BTXTMA( TryGetNecropolisFloors( gameDataPath, site.Locale, site.Version ), endian );
				site.NecropolisTreasures = new T8BTXTM.T8BTXTMT( TryGetNecropolisTreasures( gameDataPath, site.Locale, site.Version ), endian );
				site.NecropolisMaps = new SortedDictionary<string, T8BTXTM.T8BTXTMM>();
				foreach ( T8BTXTM.FloorInfo floor in site.NecropolisFloors.FloorList ) {
					if ( !site.NecropolisMaps.ContainsKey( floor.RefString2 ) ) {
						site.NecropolisMaps.Add( floor.RefString2, new T8BTXTM.T8BTXTMM( TryGetNecropolisMap( gameDataPath, floor.RefString2, site.Locale, site.Version ), endian ) );
					}
				}
				site.TrophyJp = HyoutaTools.Trophy.TrophyConfNode.ReadTropSfmWithTropConf( gameDataPath + @"TROPHY.TRP.ext\TROP.SFM", gameDataPath + @"TROPHY.TRP.ext\TROPCONF.SFM" );

				site.NpcList = new TOVNPC.TOVNPCL( gameDataPath + @"npc.svo.ext\NPC.DAT.dec.ext\0000.dec", endian );
				site.NpcDefs = new Dictionary<string, TOVNPC.TOVNPCT>();
				foreach ( var f in site.NpcList.NpcFileList ) {
					string filename = gameDataPath + @"npc.svo.ext\" + f.Filename + @".dec.ext\0001.dec";
					if ( File.Exists( filename ) ) {
						var d = new TOVNPC.TOVNPCT( filename, endian );
						site.NpcDefs.Add( f.Map, d );
					}
				}
			}

			var maplist = new MapList.MapList( TryGetMaplist( gameDataPath, site.Locale, site.Version ), endian );
			site.ScenarioGroupsStory = site.CreateScenarioIndexGroups( ScenarioType.Story, maplist, gameDataPath, encoding );
			site.ScenarioGroupsSidequests = site.CreateScenarioIndexGroups( ScenarioType.Sidequests, maplist, gameDataPath, encoding );
			site.ScenarioGroupsMaps = site.CreateScenarioIndexGroups( ScenarioType.Maps, maplist, gameDataPath, encoding );
			site.ScenarioAddSkits( site.ScenarioGroupsStory );

			return site;
		}

		public static void ExportToWebsite( WebsiteGenerator site, string dir, WebsiteGenerator siteComparison = null ) {
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
			if ( site.BattleVoicesEnd != null ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.PostBattleVoices, site.Version, false ) ), site.GenerateHtmlBattleVoicesEnd(), Encoding.UTF8 );
			}
			if ( site.TrophyEn != null && site.TrophyJp != null ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Trophy, site.Version, false ) ), site.GenerateHtmlTrophies(), Encoding.UTF8 );
			}
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.SkitInfo, site.Version, false ) ), site.GenerateHtmlSkitInfo(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.SkitIndex, site.Version, false ) ), site.GenerateHtmlSkitIndex(), Encoding.UTF8 );
			if ( site.SearchPoints != null ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.SearchPoint, site.Version, false ) ), site.GenerateHtmlSearchPoints(), Encoding.UTF8 );
				site.SearchPoints.GenerateMap( new System.Drawing.Bitmap( dir + @"map\U_WORLDNAVI00_5120x4096_point.png" ) ).Save( dir + site.Version + @"-SearchPoint.png" );
			}
			if ( site.NecropolisFloors != null && site.NecropolisTreasures != null && site.NecropolisMaps != null ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.NecropolisMap, site.Version, false ) ), site.GenerateHtmlNecropolis( dir, false ), Encoding.UTF8 );
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.NecropolisEnemy, site.Version, false ) ), site.GenerateHtmlNecropolis( dir, true ), Encoding.UTF8 );
			}
			if ( site.NpcDefs != null ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.StringDic, site.Version, false ) ), site.GenerateHtmlNpc(), Encoding.UTF8 );
			}

			string databasePath = Path.Combine( dir, "_db-" + site.Version + ".sqlite" );
			System.IO.File.Delete( databasePath );
			new GenerateDatabase( site, new System.Data.SQLite.SQLiteConnection( "Data Source=" + databasePath ), siteComparison ).ExportAll();

			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.ScenarioStoryIndex, site.Version, false ) ), site.ScenarioProcessGroupsToHtml( site.ScenarioGroupsStory, ScenarioType.Story, site.Version ), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.ScenarioSidequestIndex, site.Version, false ) ), site.ScenarioProcessGroupsToHtml( site.ScenarioGroupsSidequests, ScenarioType.Sidequests, site.Version ), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.ScenarioMapIndex, site.Version, false ) ), site.ScenarioProcessGroupsToHtml( site.ScenarioGroupsMaps, ScenarioType.Maps, site.Version ), Encoding.UTF8 );
		}
	}
}
