using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using HyoutaTools.FileContainer;

namespace HyoutaTools.Tales.Vesperia.Website {
	public class GenerateWebsiteInputOutputData {
		public IContainer GameDataContainer;
		public IContainer GamePatchContainer = null;
		public GameVersion Version;
		public string VersionPostfix = "";
		public GameLocale Locale;
		public WebsiteLanguage Language;
		public GameLocale? ImportJpInGameDictLocale = null;
		public Util.Endianness Endian;
		public Util.GameTextEncoding Encoding;
		public Util.Bitness Bits = Util.Bitness.B32;
		public string WebsiteOutputPath;
		public Bitmap WorldMapImageOverride = null;

		public WebsiteGenerator Generator = null;
		public GenerateWebsiteInputOutputData CompareSite = null;
	}

	public static class GenerateWebsite {
		private static INode FindChildByName( this IContainer node, string name ) {
			INode n = node.GetChildByName( name );
			if ( n != null ) {
				return n;
			}
			INode o = node.GetChildByName( name + ".ext" );
			if ( o != null ) {
				return o;
			}
			INode p = node.GetChildByName( name + ".dec" );
			if ( p != null ) {
				return p;
			}
			INode q = node.GetChildByName( name + ".dec.ext" );
			if ( q != null ) {
				return q;
			}
			IEnumerable<string> names = node.GetChildNames();
			string probablyName = names.Where( x => x.StartsWith( name + "." ) ).FirstOrDefault();
			if ( probablyName != null ) {
				return node.GetChildByName( probablyName );
			}
			return null;
		}
		private static IContainer ToFps4( this INode node ) {
			if ( node.IsFile ) {
				return new FPS4.FPS4( node.AsFile.DataStream );
			}
			return node.AsContainer;
		}
		private static IContainer ToIndexedFps4( this INode node ) {
			if ( node.IsFile ) {
				return new FPS4.FPS4( node.AsFile.DataStream );
			} else {
				return new ContainerIndexAsStringWrapper( node.AsContainer, "{0:D4}" );
			}
		}
		private static IContainer ToScenarioDat( this INode node ) {
			if ( node.IsFile ) {
				return new Scenario.ScenarioDat( node.AsFile.DataStream );
			} else {
				return new ContainerIndexAsStringWrapper( node.AsContainer );
			}
		}
		private static INode TryDecompress( this INode node ) {
			if ( node.IsFile ) {
				IFile f = node.AsFile;
				try {
					f.DataStream.ReStart();
					foreach ( IDecompressor d in DecompressorManager.Instance.GetDecompressors() ) {
						if ( d.CanDecompress( f.DataStream ) == CanDecompressAnswer.Yes ) {
							return new FileFromStream( d.Decompress( f.DataStream ).ToDuplicatableStream() );
						}
					}
				} finally {
					f.DataStream.End();
				}
			}
			return node;
		}
		private static Streams.DuplicatableStream ToDuplicatableStream( this Stream stream ) {
			if ( stream is Streams.DuplicatableStream ) {
				return (Streams.DuplicatableStream)stream;
			}

			stream.Position = 0;
			byte[] data = new byte[stream.Length];
			stream.Read( data, 0, data.Length );
			return new Streams.DuplicatableByteArrayStream( data );
		}

		public static Stream TryGetItemDat( IContainer basepath, GameLocale locale, GameVersion version ) {
			return basepath?.FindChildByName( "item.svo" )?.ToFps4()?.FindChildByName( "ITEM.DAT" )?.AsFile?.DataStream;
		}
		public static Stream TryGetStringDic( IContainer basepath, GameLocale locale, GameVersion version ) {
			if ( version == GameVersion.X360_EU || version == GameVersion.PC ) {
				return basepath?.FindChildByName( "language" )?.AsContainer?.FindChildByName( "string_dic_" + ( version == GameVersion.X360_EU ? locale.ToString().ToLowerInvariant() : locale.ToString().ToUpperInvariant() ) + ".so" )?.AsFile?.DataStream;
			} else {
				return basepath?.FindChildByName( "string.svo" )?.ToFps4()?.FindChildByName( "STRING_DIC.SO" )?.AsFile?.DataStream;
			}
		}
		public static IContainer TryOpenBtlPack( IContainer basepath, GameLocale locale, GameVersion version ) {
			string btlPackName = version == GameVersion.X360_EU ? "BTL_PACK_" + locale.ToString().ToUpperInvariant() + ".DAT" : "BTL_PACK.DAT";
			return basepath?.FindChildByName( "btl.svo" )?.ToFps4()?.FindChildByName( btlPackName )?.ToIndexedFps4();
		}
		public static Stream TryGetArtes( IContainer basepath, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 4 )?.ToFps4()?.FindChildByName( "ALL.0000" )?.AsFile?.DataStream;
		}
		public static Stream TryGetSkills( IContainer basepath, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 10 )?.ToFps4()?.FindChildByName( "ALL.0000" )?.AsFile?.DataStream;
		}
		public static Stream TryGetEnemies( IContainer basepath, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 5 )?.ToFps4()?.FindChildByName( "ALL.0000" )?.AsFile?.DataStream;
		}
		public static Stream TryGetEnemyGroups( IContainer basepath, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 6 )?.ToFps4()?.FindChildByName( "ALL.0000" )?.AsFile?.DataStream;
		}
		public static Stream TryGetEncounterGroups( IContainer basepath, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 7 )?.ToFps4()?.FindChildByName( "ALL.0000" )?.AsFile?.DataStream;
		}
		public static Stream TryGetGradeShop( IContainer basepath, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 16 )?.ToFps4()?.FindChildByName( "ALL.0000" )?.AsFile?.DataStream;
		}
		public static Stream TryGetStrategy( IContainer basepath, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 11 )?.ToFps4()?.FindChildByName( "ALL.0000" )?.AsFile?.DataStream;
		}
		public static Stream TryGetBattleVoicesEnd( IContainer basepath, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 19 )?.ToFps4()?.FindChildByName( "END.0000" )?.AsFile?.DataStream;
		}
		public static Stream TryGetNecropolisFloors( IContainer basepath, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 21 )?.ToFps4()?.FindChildByName( "ALL.0000" )?.AsFile?.DataStream;
		}
		public static Stream TryGetNecropolisTreasures( IContainer basepath, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 22 )?.ToFps4()?.FindChildByName( "ALL.0000" )?.AsFile?.DataStream;
		}
		public static Stream TryGetNecropolisMap( IContainer basepath, string mapname, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 23 )?.ToFps4()?.FindChildByName( mapname )?.AsFile?.DataStream;
		}
		public static List<string> GetBattleScenarioFileNames( IContainer basepath, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 3 )?.ToFps4()?.GetChildNames().Where( x => x.StartsWith( "BTL_" ) ).Select( x => x.Split( new char[] { '.' } ).First() ).ToList();
		}
		public static Stream TryGetBattleScenarioFile( IContainer basepath, string epname, GameLocale locale, GameVersion version ) {
			return TryOpenBtlPack( basepath, locale, version )?.GetChildByIndex( 3 )?.ToFps4()?.FindChildByName( epname )?.TryDecompress()?.AsFile?.DataStream;
		}
		public static Stream TryGetRecipes( IContainer basepath, GameLocale locale, GameVersion version ) {
			if ( version.Is360() ) {
				return basepath?.FindChildByName( "cook.svo" )?.ToFps4()?.FindChildByName( "COOKDATA.BIN" )?.AsFile?.DataStream;
			} else {
				return basepath?.FindChildByName( "menu.svo" )?.ToFps4()?.FindChildByName( "COOKDATA.BIN" )?.AsFile?.DataStream;
			}
		}
		public static Stream TryGetLocations( IContainer basepath, GameLocale locale, GameVersion version ) {
			return basepath?.FindChildByName( "menu.svo" )?.ToFps4()?.FindChildByName( "WORLDDATA.BIN" )?.AsFile?.DataStream;
		}
		public static Stream TryGetSynopsis( IContainer basepath, GameLocale locale, GameVersion version ) {
			return basepath?.FindChildByName( "menu.svo" )?.ToFps4()?.FindChildByName( "SYNOPSISDATA.BIN" )?.AsFile?.DataStream;
		}
		public static Stream TryGetTitles( IContainer basepath, GameLocale locale, GameVersion version ) {
			return basepath?.FindChildByName( "menu.svo" )?.ToFps4()?.FindChildByName( "FAMEDATA.BIN" )?.AsFile?.DataStream;
		}
		public static Stream TryGetBattleBook( IContainer basepath, GameLocale locale, GameVersion version ) {
			return basepath?.FindChildByName( "menu.svo" )?.ToFps4()?.FindChildByName( "BATTLEBOOKDATA.BIN" )?.AsFile?.DataStream;
		}
		public static Stream TryGetSkitMetadata( IContainer basepath, GameLocale locale, GameVersion version ) {
			return basepath?.FindChildByName( "chat.svo" )?.ToFps4()?.FindChildByName( "CHAT.DAT" )?.TryDecompress()?.AsFile?.DataStream;
		}
		public static Stream TryGetSkitText( IContainer basepath, string skitname, GameLocale locale, GameVersion version ) {
			var chatsvo = basepath?.FindChildByName( "chat.svo" )?.ToFps4();
			var skit = chatsvo?.FindChildByName( skitname + ( version == GameVersion.PC ? "J" : locale.ToString().ToUpperInvariant() ) + ".DAT" )?.TryDecompress();
			return skit?.ToIndexedFps4()?.GetChildByIndex( 3 )?.AsFile?.DataStream;
		}
		public static Stream TryGetSearchPoints( IContainer basepath, GameLocale locale, GameVersion version ) {
			var svo = basepath?.FindChildByName( "npc.svo" )?.ToFps4();
			var field = svo?.FindChildByName( "FIELD.DAT" )?.TryDecompress()?.ToIndexedFps4();
			return field?.GetChildByIndex( 5 )?.TryDecompress()?.AsFile?.DataStream;
		}
		public static Stream TryGetScenarioFile( IContainer basefolder, int fileIndex, GameLocale locale, GameVersion version ) {
			INode scenariodat;
			if ( version == GameVersion.X360_EU || version == GameVersion.PC ) {
				scenariodat = basefolder?.FindChildByName( "language" )?.AsContainer?.FindChildByName( "scenario_" + ( version == GameVersion.X360_EU ? locale.ToString().ToLowerInvariant() : locale.ToString().ToUpperInvariant() ) + ".dat" );
			} else {
				scenariodat = basefolder?.FindChildByName( "scenario.dat" );
			}
			var f = scenariodat?.ToScenarioDat()?.GetChildByIndex( fileIndex )?.TryDecompress()?.AsFile?.DataStream;
			if ( f != null ) {
				return f;
			}
			return scenariodat?.ToScenarioDat()?.FindChildByName( fileIndex.ToString( "D1" ) )?.TryDecompress()?.AsFile?.DataStream;
		}
		public static Stream TryGetMaplist( IContainer basepath, GameLocale locale, GameVersion version ) {
			return basepath?.FindChildByName( "map.svo" )?.ToFps4()?.FindChildByName( "MAPLIST.DAT" )?.AsFile?.DataStream;
		}
		private static IContainer ToTrophyTrp( this INode node ) {
			if ( node.IsFile ) {
				return new Trophy.TrophyTrp( node.AsFile.DataStream );
			} else {
				return node.AsContainer;
			}
		}
		public static Trophy.TrophyConfNode GetTrophy( IContainer trophyDataPath ) {
			var trp = trophyDataPath.FindChildByName( "TROPHY.TRP" ).ToTrophyTrp();
			return Trophy.TrophyConfNode.ReadTropSfmWithTropConf( trp.FindChildByName( "TROP.SFM" ).AsFile.DataStream, trp.FindChildByName( "TROPCONF.SFM" ).AsFile.DataStream );
		}
		public static int Generate( List<string> args ) {
			var worldmap = IntegerScaled( new Bitmap( @"c:\Dropbox\ToV\U_WORLDNAVI00.png" ), 5, 4 );

			List<GenerateWebsiteInputOutputData> gens = new List<GenerateWebsiteInputOutputData>();
			gens.Add( new GenerateWebsiteInputOutputData() {
				GameDataContainer = new DirectoryOnDisk( @"c:\Dropbox\ToV\360_EU\" ),
				Version = GameVersion.X360_EU,
				Locale = GameLocale.UK,
				Language = WebsiteLanguage.BothWithEnLinks,
				ImportJpInGameDictLocale = GameLocale.US,
				Endian = Util.Endianness.BigEndian,
				Encoding = Util.GameTextEncoding.UTF8,
				WorldMapImageOverride = worldmap,
				WebsiteOutputPath = @"c:\Dropbox\ToV\website_out_360_EU_UK\",
			} );
			gens.Add( new GenerateWebsiteInputOutputData() {
				GameDataContainer = new DirectoryOnDisk( @"c:\Dropbox\ToV\PS3\orig\" ),
				GamePatchContainer = new DirectoryOnDisk( @"c:\Dropbox\ToV\PS3\mod\" ),
				Version = GameVersion.PS3,
				VersionPostfix = "p",
				Locale = GameLocale.J,
				Language = WebsiteLanguage.BothWithEnLinks,
				Endian = Util.Endianness.BigEndian,
				Encoding = Util.GameTextEncoding.ShiftJIS,
				WorldMapImageOverride = worldmap,
				WebsiteOutputPath = @"c:\Dropbox\ToV\website_out_PS3_with_patch\",
				CompareSite = gens.Where( x => x.Version == GameVersion.X360_EU && x.Locale == GameLocale.UK ).First(),
			} );

			Generate( gens );

			return 0;
		}

		public static void Generate( List<GenerateWebsiteInputOutputData> gens ) {
			foreach ( var g in gens ) {
				WebsiteGenerator site = LoadWebsiteGenerator( g.GameDataContainer, g.Version, g.VersionPostfix, g.Locale, g.Language, g.Endian, g.Encoding, g.Bits, g.WorldMapImageOverride );

				if ( g.GamePatchContainer != null ) {
					IContainer patchDataContainer = FindGameDataDirectory( g.GamePatchContainer, g.Version );

					// patch original PS3 data with fantranslation
					{
						// STRING_DIC
						var stringDicTranslated = new TSS.TSSFile( TryGetStringDic( patchDataContainer, g.Locale, g.Version ), g.Encoding, g.Endian );
						Util.Assert( site.StringDic.Entries.Length == stringDicTranslated.Entries.Length );
						for ( int i = 0; i < site.StringDic.Entries.Length; ++i ) {
							Util.Assert( site.StringDic.Entries[i].inGameStringId == stringDicTranslated.Entries[i].inGameStringId );
							site.StringDic.Entries[i].StringEng = stringDicTranslated.Entries[i].StringJpn;
						}
					}
					foreach ( var kvp in site.ScenarioFiles ) {
						// scenario.dat
						if ( kvp.Value.EntryList.Count > 0 && kvp.Value.Metadata.ScenarioDatIndex >= 0 ) {
							Stream streamMod = TryGetScenarioFile( patchDataContainer, kvp.Value.Metadata.ScenarioDatIndex, g.Locale, g.Version );
							if ( streamMod != null ) {
								var scenarioMod = new ScenarioFile.ScenarioFile( streamMod, g.Encoding, g.Endian, g.Bits );
								Util.Assert( kvp.Value.EntryList.Count == scenarioMod.EntryList.Count );
								for ( int i = 0; i < kvp.Value.EntryList.Count; ++i ) {
									kvp.Value.EntryList[i].EnName = scenarioMod.EntryList[i].JpName;
									kvp.Value.EntryList[i].EnText = scenarioMod.EntryList[i].JpText;
								}
							}
						}
					}
					foreach ( var kvp in site.BattleTextFiles ) {
						// btl.svo/BATTLE_PACK
						if ( kvp.Value.EntryList.Count > 0 ) {
							var scenarioMod = WebsiteGenerator.LoadBattleTextFile( patchDataContainer, kvp.Key, g.Locale, g.Version, g.Endian, g.Encoding, g.Bits );
							Util.Assert( kvp.Value.EntryList.Count == scenarioMod.EntryList.Count );
							for ( int i = 0; i < kvp.Value.EntryList.Count; ++i ) {
								kvp.Value.EntryList[i].EnName = scenarioMod.EntryList[i].JpName;
								kvp.Value.EntryList[i].EnText = scenarioMod.EntryList[i].JpText;
							}
						}
					}
					foreach ( var kvp in site.SkitText ) {
						// chat.svo
						var chatFile = kvp.Value;
						Stream streamMod = TryGetSkitText( patchDataContainer, kvp.Key, g.Locale, g.Version );
						var chatFileMod = new TO8CHTX.ChatFile( streamMod, g.Endian, g.Encoding, g.Bits, 2 );
						Util.Assert( chatFile.Lines.Length == chatFileMod.Lines.Length );
						for ( int j = 0; j < chatFile.Lines.Length; ++j ) {
							chatFile.Lines[j].SENG = chatFileMod.Lines[j].SJPN;
							chatFile.Lines[j].SNameEnglishNotUsedByGame = chatFileMod.Lines[j].SName;
						}
					}

					IContainer patchTrophyContainer = FindTrophyDataDirectory( g.GamePatchContainer, g.Version );
					if ( patchTrophyContainer != null ) {
						site.TrophyEn = GetTrophy( patchTrophyContainer );
					}
				}

				site.InGameIdDict = site.StringDic.GenerateInGameIdDictionary();

				if ( g.ImportJpInGameDictLocale != null ) {
					// copy over Japanese stuff into StringDic from other locale
					var StringDicUs = new TSS.TSSFile( TryGetStringDic( FindGameDataDirectory( g.GameDataContainer, g.Version ), g.ImportJpInGameDictLocale.Value, g.Version ), g.Encoding, g.Endian );
					var IdDictUs = StringDicUs.GenerateInGameIdDictionary();
					foreach ( var kvp in IdDictUs ) {
						site.InGameIdDict[kvp.Key].StringJpn = kvp.Value.StringJpn;
					}
				}

				ExportToWebsite( site, WebsiteLanguage.BothWithEnLinks, g.WebsiteOutputPath, g.CompareSite?.Generator );

				g.Generator = site;
			}
		}

		private static IContainer FindGameDataDirectory( IContainer topLevelGameDataContainer, GameVersion version ) {
			return FindDirectoryWithFile( topLevelGameDataContainer, version, "menu.svo", "USRDIR" );
		}
		private static IContainer FindTrophyDataDirectory( IContainer topLevelGameDataContainer, GameVersion version ) {
			return FindDirectoryWithFile( topLevelGameDataContainer, version, "TROPHY.TRP", "TROPDIR" );
		}
		private static IContainer FindDirectoryWithFile( IContainer topLevelGameDataContainer, GameVersion version, string markerFile, string downstreamHint ) {
			// if the container contains the markerFile we probably have the right one
			if ( topLevelGameDataContainer.FindChildByName( markerFile ) != null ) {
				return topLevelGameDataContainer;
			}

			if ( version == GameVersion.PS3 ) {
				// could be up one or two levels in the directory structure
				var usrdir = topLevelGameDataContainer.GetChildByName( downstreamHint )?.AsContainer;
				if ( usrdir != null ) {
					return FindDirectoryWithFile( usrdir, version, markerFile, downstreamHint );
				}
				var ps3game = topLevelGameDataContainer.GetChildByName( "PS3_GAME" )?.AsContainer;
				if ( ps3game != null ) {
					return FindDirectoryWithFile( ps3game, version, markerFile, downstreamHint );
				}
			}

			if ( version == GameVersion.PC ) {
				// could be one level up
				var data64 = topLevelGameDataContainer.GetChildByName( "Data64" )?.AsContainer;
				if ( data64 != null ) {
					return FindDirectoryWithFile( data64, version, markerFile, downstreamHint );
				}
			}

			if ( topLevelGameDataContainer.GetChildNames().Count() == 1 ) {
				return FindDirectoryWithFile( topLevelGameDataContainer.GetChildByName( topLevelGameDataContainer.GetChildNames().First() )?.AsContainer, version, markerFile, downstreamHint );
			}

			return null;
		}

		public static WebsiteGenerator LoadWebsiteGenerator( IContainer topLevelGameDataContainer, GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, Util.Endianness endian, Util.GameTextEncoding encoding, Util.Bitness bits, Bitmap worldmapOverride ) {
			IContainer gameDataPath = FindGameDataDirectory( topLevelGameDataContainer, version );
			if ( gameDataPath == null ) {
				throw new Exception( "Cannot find game data directory from container " + topLevelGameDataContainer.ToString() + "." );
			}

			IContainer trophyDataPath = null;
			if ( version == GameVersion.PS3 ) {
				trophyDataPath = FindTrophyDataDirectory( topLevelGameDataContainer, version );
				if ( trophyDataPath == null ) {
					Console.WriteLine( "Cannot find trophy file from container " + topLevelGameDataContainer.ToString() + "." );
				}
			}

			WebsiteGenerator site = new WebsiteGenerator();
			site.Locale = locale;
			site.Version = version;
			site.VersionPostfix = versionPostfix;
			site.Language = websiteLanguage;

			site.Items = new ItemDat.ItemDat( TryGetItemDat( gameDataPath, site.Locale, site.Version ), Util.Endianness.BigEndian );
			site.StringDic = new TSS.TSSFile( TryGetStringDic( gameDataPath, site.Locale, site.Version ), encoding, endian );
			site.Artes = new T8BTMA.T8BTMA( TryGetArtes( gameDataPath, site.Locale, site.Version ), endian, bits );
			site.Skills = new T8BTSK.T8BTSK( TryGetSkills( gameDataPath, site.Locale, site.Version ), endian, bits );
			site.Enemies = new T8BTEMST.T8BTEMST( TryGetEnemies( gameDataPath, site.Locale, site.Version ), endian, bits );
			site.EnemyGroups = new T8BTEMGP.T8BTEMGP( TryGetEnemyGroups( gameDataPath, site.Locale, site.Version ), endian, bits );
			site.EncounterGroups = new T8BTEMEG.T8BTEMEG( TryGetEncounterGroups( gameDataPath, site.Locale, site.Version ), endian, bits );
			site.Recipes = new COOKDAT.COOKDAT( TryGetRecipes( gameDataPath, site.Locale, site.Version ), endian );
			site.Locations = new WRLDDAT.WRLDDAT( TryGetLocations( gameDataPath, site.Locale, site.Version ), endian );
			site.Synopsis = new SYNPDAT.SYNPDAT( TryGetSynopsis( gameDataPath, site.Locale, site.Version ), endian );
			site.Titles = new FAMEDAT.FAMEDAT( TryGetTitles( gameDataPath, site.Locale, site.Version ), endian );
			site.GradeShop = new T8BTGR.T8BTGR( TryGetGradeShop( gameDataPath, site.Locale, site.Version ), endian, bits );
			site.BattleBook = new BTLBDAT.BTLBDAT( TryGetBattleBook( gameDataPath, site.Locale, site.Version ), endian );
			site.Strategy = new T8BTTA.T8BTTA( TryGetStrategy( gameDataPath, site.Locale, site.Version ), endian, bits );
			if ( site.Version != GameVersion.PC ) { // TODO
				site.BattleVoicesEnd = new T8BTVA.T8BTVA( TryGetBattleVoicesEnd( gameDataPath, site.Locale, site.Version ), endian );
			}
			if ( !site.Version.Is360() ) { // 360 version stores search points differently, haven't decoded that
				if ( worldmapOverride != null ) {
					site.WorldMapImage = worldmapOverride;
				} else {
					var ui = gameDataPath.FindChildByName( "UI.svo" ).ToFps4();
					var txm = new Texture.TXM( ui.FindChildByName( "WORLDNAVI.TXM" ).AsFile.DataStream );
					var txv = new Texture.TXV( txm, ui.FindChildByName( "WORLDNAVI.TXV" ).AsFile.DataStream, site.Version == GameVersion.PC );
					var tex = txv.textures.Where( x => x.TXM.Name == "U_WORLDNAVI00" ).First();
					site.WorldMapImage = IntegerScaled( tex.GetBitmaps().First(), 5, 4 );
				}
				site.SearchPoints = new TOVSEAF.TOVSEAF( TryGetSearchPoints( gameDataPath, site.Locale, site.Version ), endian );
			}
			site.Skits = new TO8CHLI.TO8CHLI( TryGetSkitMetadata( gameDataPath, site.Locale, site.Version ), endian, bits );
			site.SkitText = new Dictionary<string, TO8CHTX.ChatFile>();
			ISet<string> forceShiftJisSkits = version == GameVersion.X360_EU && ( locale == GameLocale.UK || locale == GameLocale.US ) ?
				new HashSet<string> { "VC051", "VC052", "VC053", "VC054", "VC055", "VC056", "VC057", "VC084", "VC719", "VC954" } : new HashSet<string>();
			for ( int i = 0; i < site.Skits.SkitInfoList.Count; ++i ) {
				string name = site.Skits.SkitInfoList[i].RefString;
				Stream stream = TryGetSkitText( gameDataPath, name, site.Locale, site.Version );
				if ( stream != null ) {
					bool forceShiftJis = forceShiftJisSkits.Contains( name );
					TO8CHTX.ChatFile chatFile = new TO8CHTX.ChatFile( stream, endian, forceShiftJis ? Util.GameTextEncoding.ShiftJIS : encoding, bits, version == GameVersion.PC ? 11 : 2 );
					site.SkitText.Add( name, chatFile );
				} else {
					Console.WriteLine( "Couldn't find chat file " + name + "! (" + version + ", " + locale + ")" );
				}
			}

			site.Records = WebsiteGenerator.GenerateRecordsStringDicList( site.Version );
			site.Settings = WebsiteGenerator.GenerateSettingsStringDicList( site.Version );

			site.ScenarioFiles = new Dictionary<string, ScenarioFile.ScenarioFile>();

			switch ( version ) {
				case GameVersion.X360_US:
					site.Shops = new ShopData.ShopData( TryGetScenarioFile( gameDataPath, 0, site.Locale, site.Version ), 0x1A718, 0x420 / 32, 0x8F8, 0x13780 / 56, endian, bits );
					break;
				case GameVersion.X360_EU:
					site.Shops = new ShopData.ShopData( TryGetScenarioFile( gameDataPath, 0, site.Locale, site.Version ), 0x1A780, 0x420 / 32, 0x8F8, 0x13780 / 56, endian, bits );
					break;
				case GameVersion.PS3:
				case GameVersion.PC:
					site.Shops = new ShopData.ShopData( TryGetScenarioFile( gameDataPath, 0, site.Locale, site.Version ), 0x1C9BC, 0x460 / 32, 0x980, 0x14CB8 / 56, endian, bits );
					break;
				default:
					throw new Exception( "Don't know shop data location for version " + version );
			}

			if ( version.HasPS3Content() ) {
				site.IconsWithItems = new uint[] { 35, 36, 37, 60, 38, 1, 4, 12, 6, 5, 13, 14, 15, 7, 52, 51, 53, 9, 16, 18, 2, 17, 19, 10, 54, 20, 21, 22, 23, 24, 25, 26, 27, 56, 30, 28, 32, 31, 33, 29, 34, 41, 42, 43, 44, 45, 57, 61, 63, 39, 3, 40 };
			} else { 
				site.IconsWithItems = new uint[] { 35, 36, 37, 60, 38, 1, 4, 12, 6, 5, 13, 14, 15, 7, 52, 51,     9, 16, 18, 2, 17, 19, 10,     20, 21, 22, 23, 24, 25, 26, 27, 56, 30, 28, 32, 31, 33, 29, 34, 41, 42, 43, 44, 45, 57, 61, 63, 39, 3, 40 };
			}

			site.BattleTextFiles = WebsiteGenerator.LoadBattleText( gameDataPath, site.Locale, site.Version, endian, encoding, bits );

			if ( version.HasPS3Content() ) {
				site.NecropolisFloors = new T8BTXTM.T8BTXTMA( TryGetNecropolisFloors( gameDataPath, site.Locale, site.Version ), endian, bits );
				site.NecropolisTreasures = new T8BTXTM.T8BTXTMT( TryGetNecropolisTreasures( gameDataPath, site.Locale, site.Version ), endian, bits );
				site.NecropolisMaps = new SortedDictionary<string, T8BTXTM.T8BTXTMM>();
				foreach ( T8BTXTM.FloorInfo floor in site.NecropolisFloors.FloorList ) {
					if ( !site.NecropolisMaps.ContainsKey( floor.RefString2 ) ) {
						site.NecropolisMaps.Add( floor.RefString2, new T8BTXTM.T8BTXTMM( TryGetNecropolisMap( gameDataPath, floor.RefString2, site.Locale, site.Version ), endian ) );
					}
				}
			}

			if ( version == GameVersion.PS3 && trophyDataPath != null ) {
				site.TrophyJp = GetTrophy( trophyDataPath );
			}

			if ( version.HasPS3Content() ) {
				var npcsvo = gameDataPath.FindChildByName( "npc.svo" ).ToFps4();
				site.NpcList = new TOVNPC.TOVNPCL( npcsvo.FindChildByName( "NPC.DAT" ).TryDecompress().ToIndexedFps4().GetChildByIndex( 0 ).TryDecompress().AsFile.DataStream, endian, bits );
				site.NpcDefs = new Dictionary<string, TOVNPC.TOVNPCT>();
				foreach ( var f in site.NpcList.NpcFileList ) {
					var s = npcsvo.FindChildByName( f.Filename.ToUpperInvariant() ).TryDecompress().ToIndexedFps4().GetChildByIndex( 1 )?.TryDecompress()?.AsFile?.DataStream;
					if ( s != null ) {
						var d = new TOVNPC.TOVNPCT( s, endian, bits );
						site.NpcDefs.Add( f.Map, d );
					}
				}
			}

			var maplist = new MapList.MapList( TryGetMaplist( gameDataPath, site.Locale, site.Version ), endian, bits );
			site.ScenarioGroupsStory = site.CreateScenarioIndexGroups( ScenarioType.Story, maplist, gameDataPath, encoding, endian, bits );
			site.ScenarioGroupsSidequests = site.CreateScenarioIndexGroups( ScenarioType.Sidequests, maplist, gameDataPath, encoding, endian, bits );
			site.ScenarioGroupsMaps = site.CreateScenarioIndexGroups( ScenarioType.Maps, maplist, gameDataPath, encoding, endian, bits );
			site.ScenarioAddSkits( site.ScenarioGroupsStory );

			return site;
		}

		private static Bitmap IntegerScaled( Bitmap bmp, int factorX, int factorY ) {
			int newWidth = bmp.Width * factorX;
			int newHeight = bmp.Height * factorY;
			Bitmap n = new Bitmap( newWidth, newHeight );
			for ( int y = 0; y < bmp.Height; ++y ) {
				for ( int x = 0; x < bmp.Width; ++x ) {
					Color c = bmp.GetPixel( x, y );
					for ( int ny = 0; ny < factorY; ++ny ) {
						for ( int nx = 0; nx < factorX; ++nx ) {
							n.SetPixel( x * factorX + nx, y * factorY + ny, c );
						}
					}
				}
			}
			return n;
		}

		public static void ExportToWebsite( WebsiteGenerator site, WebsiteLanguage lang, string dir, WebsiteGenerator siteComparison = null ) {
			Directory.CreateDirectory( dir );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Item, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlItems(), Encoding.UTF8 );
			foreach ( uint i in site.IconsWithItems ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Item, site.Version, site.VersionPostfix, site.Locale, lang, false, icon: (int)i ) ), site.GenerateHtmlItems( icon: i ), Encoding.UTF8 );
			}
			for ( uint i = 2; i < 12; ++i ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Item, site.Version, site.VersionPostfix, site.Locale, lang, false, category: (int)i ) ), site.GenerateHtmlItems( category: i ), Encoding.UTF8 );
			}
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Enemy, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlEnemies(), Encoding.UTF8 );
			for ( int i = 0; i < 9; ++i ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Enemy, site.Version, site.VersionPostfix, site.Locale, lang, false, category: (int)i ) ), site.GenerateHtmlEnemies( category: i ), Encoding.UTF8 );
			}
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.EnemyGroup, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlEnemyGroups(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.EncounterGroup, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlEncounterGroups(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Skill, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlSkills(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Arte, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlArtes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Synopsis, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlSynopsis(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Recipe, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlRecipes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Location, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlLocations(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Strategy, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlStrategy(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Shop, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlShops(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Title, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlTitles(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.BattleBook, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlBattleBook(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Record, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlRecords(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Settings, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlSettings(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.GradeShop, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlGradeShop(), Encoding.UTF8 );
			if ( site.BattleVoicesEnd != null ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.PostBattleVoices, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlBattleVoicesEnd(), Encoding.UTF8 );
			}
			if ( site.TrophyEn != null && site.TrophyJp != null ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.Trophy, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlTrophies(), Encoding.UTF8 );
			}
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.SkitInfo, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlSkitInfo(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.SkitIndex, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlSkitIndex(), Encoding.UTF8 );
			if ( site.SearchPoints != null ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.SearchPoint, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlSearchPoints(), Encoding.UTF8 );
				site.SearchPoints.GenerateMap( site.WorldMapImage ).Save( dir + site.Version + @"-SearchPoint.png" );
			}
			if ( site.NecropolisFloors != null && site.NecropolisTreasures != null && site.NecropolisMaps != null ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.NecropolisMap, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlNecropolis( dir, false ), Encoding.UTF8 );
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.NecropolisEnemy, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlNecropolis( dir, true ), Encoding.UTF8 );
			}
			if ( site.NpcDefs != null ) {
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.StringDic, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), site.GenerateHtmlNpc(), Encoding.UTF8 );
			}

			string databasePath = Path.Combine( dir, "_db-" + site.Version + ".sqlite" );
			System.IO.File.Delete( databasePath );
			new GenerateDatabase( site, new System.Data.SQLite.SQLiteConnection( "Data Source=" + databasePath ), siteComparison ).ExportAll();

			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.ScenarioStoryIndex, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), WebsiteGenerator.ScenarioProcessGroupsToHtml( site.ScenarioGroupsStory, ScenarioType.Story, site.Version, site.VersionPostfix, site.Locale, lang, site.InGameIdDict, site.IconsWithItems ), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.ScenarioSidequestIndex, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), WebsiteGenerator.ScenarioProcessGroupsToHtml( site.ScenarioGroupsSidequests, ScenarioType.Sidequests, site.Version, site.VersionPostfix, site.Locale, lang, site.InGameIdDict, site.IconsWithItems ), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( WebsiteSection.ScenarioMapIndex, site.Version, site.VersionPostfix, site.Locale, lang, false ) ), WebsiteGenerator.ScenarioProcessGroupsToHtml( site.ScenarioGroupsMaps, ScenarioType.Maps, site.Version, site.VersionPostfix, site.Locale, lang, site.InGameIdDict, site.IconsWithItems ), Encoding.UTF8 );
		}
	}
}
