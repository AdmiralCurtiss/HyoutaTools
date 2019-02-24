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
		public bool ExtractImages = false;

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
				ExtractImages = true,
				WorldMapImageOverride = worldmap,
				WebsiteOutputPath = @"c:\Dropbox\ToV\website_out_PS3_with_patch\",
				CompareSite = gens.Where( x => x.Version == GameVersion.X360_EU && x.Locale == GameLocale.UK ).First(),
			} );

			Generate( gens );

			return 0;
		}

		public static void Generate( List<GenerateWebsiteInputOutputData> gens ) {
			foreach ( var g in gens ) {
				if ( g.ExtractImages ) {
					GenerateWebsiteImages( g.GameDataContainer, g.Version, g.VersionPostfix, g.Locale, g.Language, g.Endian, g.Encoding, g.Bits, g.WebsiteOutputPath, g.WorldMapImageOverride );
				}

				WebsiteGenerator site = LoadWebsiteGenerator( g.GameDataContainer, g.Version, g.VersionPostfix, g.Locale, g.Language, g.Endian, g.Encoding, g.Bits );

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

		public static Texture.TXV GetTxmTxv( this IContainer ui, GameVersion version, string name ) {
			string n = name;
			var txm = new Texture.TXM( ui.FindChildByName( n + ".TXM" ).AsFile.DataStream );
			var txv = new Texture.TXV( txm, ui.FindChildByName( n + ".TXV" ).AsFile.DataStream, version == GameVersion.PC );
			return txv;
		}
		public static Bitmap FirstTexture( this Texture.TXV txv ) {
			return txv.textures.First().GetBitmaps().First();
		}
		public static Bitmap FirstTexture( this Texture.TXV txv, string name ) {
			return txv.textures.Where( x => x.TXM.Name == name ).First().GetBitmaps().First();
		}
		public static Bitmap OnlyTexture( this Texture.TXV txv ) {
			if ( txv.textures.Count != 1 ) {
				throw new Exception( "OnlyTexture() called on file with more than one texture" );
			}
			return txv.FirstTexture();
		}
		public static Bitmap Crop( this Bitmap bmp, int fromWidth, int fromHeight, int toXOffset, int toYOffset, int toWidth, int toHeight ) {
			int factorX = fromWidth / bmp.Width;
			int factorY = fromHeight / bmp.Height; 
			int newWidth = toWidth * factorX;
			int newHeight = toHeight * factorY;
			Bitmap n = new Bitmap( newWidth, newHeight );
			for ( int y = 0; y < toHeight * factorY; ++y ) {
				for ( int x = 0; x < toWidth * factorX; ++x ) {
					Color c = bmp.GetPixel( toXOffset * factorX + x, toYOffset * factorY + y );
					n.SetPixel( x, y, c );
				}
			}
			return n;
		}
		public static void SaveAll( this Texture.TXV txv, string path ) {
			foreach ( var tex in txv.textures ) {
				tex.GetBitmaps().First().Save( Path.Combine( path, tex.TXM.Name + ".png" ) );
			}
		}
		public static Bitmap AddBorder( this Bitmap bmp, int oldWidth, int oldHeight, int border, Color borderColor ) {
			int factorX = oldWidth / bmp.Width;
			int factorY = oldHeight / bmp.Height;
			Bitmap n = new Bitmap( bmp.Width + border * 2 * factorX, bmp.Height + border * 2 * factorY );
			// this is a poor way to do this but whatever
			for ( int y = 0; y < n.Height; ++y ) {
				for ( int x = 0; x < n.Width; ++x ) {
					n.SetPixel( x, y, borderColor );
				}
			}
			int offsX = border * factorX;
			int offsY = border * factorY;
			for ( int y = 0; y < bmp.Height; ++y ) {
				for ( int x = 0; x < bmp.Width; ++x ) {
					Color c = bmp.GetPixel( x, y );
					n.SetPixel( offsX + x, offsY + y, c );
				}
			}
			return n;
		}
		public static Bitmap ReplaceColor( this Bitmap bmp, int r, int g, int b ) {
			Bitmap n = new Bitmap( bmp.Width, bmp.Height );
			for ( int y = 0; y < bmp.Width; ++y ) {
				for ( int x = 0; x < bmp.Height; ++x ) {
					Color c = bmp.GetPixel( x, y );
					n.SetPixel( x, y, Color.FromArgb( c.A, r, g, b ) );
				}
			}
			return n;
		}

		public static void GenerateWebsiteImages( IContainer topLevelGameDataContainer, GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, Util.Endianness endian, Util.GameTextEncoding encoding, Util.Bitness bits, string outpath, Bitmap worldmapOverride ) {
			IContainer gameDataPath = FindGameDataDirectory( topLevelGameDataContainer, version );
			if ( gameDataPath == null ) {
				throw new Exception( "Cannot find game data directory from container " + topLevelGameDataContainer.ToString() + "." );
			}

			StringBuilder bat = new StringBuilder();

			Directory.CreateDirectory( Path.Combine( outpath, "chara-art" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "chara-icons" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "element-icons" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "etc" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "item-categories" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "item-icons" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "items" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "map" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "menu-icons" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "monster-categories" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "monster-icons" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "monster-icons", "44px" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "monster-icons", "46px" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "monster-icons", "48px" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "recipes" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "skill-icons" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "synopsis" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "text-icons" ) );
			Directory.CreateDirectory( Path.Combine( outpath, "worldmap" ) );

			if ( version == GameVersion.PS3 ) {
				IContainer trophyDataPath = FindTrophyDataDirectory( topLevelGameDataContainer, version );
				if ( trophyDataPath != null ) {
					Directory.CreateDirectory( Path.Combine( outpath, "trophies" ) );
					var trp = trophyDataPath.FindChildByName( "TROPHY.TRP" ).ToTrophyTrp();
					foreach ( var img in trp.GetChildNames() ) {
						if ( img.EndsWith( ".PNG" ) ) {
							var stream = trp.GetChildByName( img ).AsFile.DataStream;
							using ( var fs = new FileStream( Path.Combine( outpath, "trophies", img ), FileMode.Create ) ) {
								stream.ReStart();
								Util.CopyStream( stream, fs, stream.Length );
								stream.End();
							}
						}
					}
				}
			}

			var ui = gameDataPath.FindChildByName( "UI.svo" ).ToFps4();
			var menu = ui.GetTxmTxv( version, "MENU" );

			{
				foreach ( string name in ui.GetChildNames().Where( x => x.StartsWith( "MENUSTATUS" ) && x.EndsWith( ".TXM" ) ) ) {
					string n = Path.GetFileNameWithoutExtension( name );
					var txv = ui.GetTxmTxv( version, n );
					txv.OnlyTexture().Save( Path.Combine( outpath, "chara-art", txv.textures.First().TXM.Name + ".png" ) );
				}
			}
			{
				var icons = ui.GetTxmTxv( version, "ICONCHARA" ).OnlyTexture();
				icons.Crop( 256, 128, 48 * 0, 64 * 0, 48, 64 ).Save( Path.Combine( outpath, "chara-icons", "YUR.png" ) );
				icons.Crop( 256, 128, 48 * 1, 64 * 0, 48, 64 ).Save( Path.Combine( outpath, "chara-icons", "EST.png" ) );
				icons.Crop( 256, 128, 48 * 2, 64 * 0, 48, 64 ).Save( Path.Combine( outpath, "chara-icons", "KAR.png" ) );
				icons.Crop( 256, 128, 48 * 3, 64 * 0, 48, 64 ).Save( Path.Combine( outpath, "chara-icons", "RIT.png" ) );
				icons.Crop( 256, 128, 48 * 4, 64 * 0, 48, 64 ).Save( Path.Combine( outpath, "chara-icons", "RAV.png" ) );
				icons.Crop( 256, 128, 48 * 0, 64 * 1, 48, 64 ).Save( Path.Combine( outpath, "chara-icons", "JUD.png" ) );
				icons.Crop( 256, 128, 48 * 1, 64 * 1, 48, 64 ).Save( Path.Combine( outpath, "chara-icons", "RAP.png" ) );
				icons.Crop( 256, 128, 48 * 2, 64 * 1, 48, 64 ).Save( Path.Combine( outpath, "chara-icons", "FRE.png" ) );
				icons.Crop( 256, 128, 48 * 3, 64 * 1, 48, 64 ).Save( Path.Combine( outpath, "chara-icons", "PAT.png" ) );
				icons.Crop( 256, 128, 48 * 4, 64 * 1, 48, 64 ).Save( Path.Combine( outpath, "chara-icons", "ESTs.png" ) );
			}
			{
				var icons = ui.GetTxmTxv( version, "ICONATT" ).OnlyTexture();
				icons.Crop( 128, 128, 40 * 0, 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "element-icons", "wind.png" ) );
				icons.Crop( 128, 128, 40 * 1, 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "element-icons", "fire.png" ) );
				icons.Crop( 128, 128, 40 * 2, 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "element-icons", "light.png" ) );
				icons.Crop( 128, 128, 40 * 0, 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "element-icons", "earth.png" ) );
				icons.Crop( 128, 128, 40 * 1, 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "element-icons", "water.png" ) );
				icons.Crop( 128, 128, 40 * 2, 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "element-icons", "dark.png" ) );
				icons.Crop( 128, 128, 40 * 0, 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "element-icons", "phys.png" ) );
			}
			{
				menu.FirstTexture( "U_MENULOAD00" ).Save( Path.Combine( outpath, "etc", "U_MENULOAD00.png" ) );
				menu.FirstTexture( "U_MENULOAD01" ).Save( Path.Combine( outpath, "etc", "U_MENULOAD01.png" ) );
				var right = menu.FirstTexture( "U_MENULIB00" ).Crop( 256, 256, 0, 192, 32, 32 );
				var left = new Bitmap( right );
				var up = new Bitmap( right );
				var down = new Bitmap( right );
				left.RotateFlip( RotateFlipType.RotateNoneFlipX );
				up.RotateFlip( RotateFlipType.Rotate270FlipNone );
				down.RotateFlip( RotateFlipType.Rotate90FlipX );
				right.Save( Path.Combine( outpath, "etc", "right.png" ) );
				left.Save( Path.Combine( outpath, "etc", "left.png" ) );
				up.Save( Path.Combine( outpath, "etc", "up.png" ) );
				down.Save( Path.Combine( outpath, "etc", "down.png" ) );
				//ui.GetTxmTxv( version, "ITEM_IRIKIAGRASS" ).OnlyTexture().Scale( 128, 128, 64, 64 ).Save( Path.Combine( outpath, "etc", "U_ITEM_IRIKIAGRASS-64px.png" ) );
				bat.AppendLine( @"imagemagick items\U_ITEM_IRIKIAGRASS.png -filter Sinc -resize 64x64 etc\U_ITEM_IRIKIAGRASS-64px.png" );
			}
			{
				var iconsort = ui.GetTxmTxv( version, "ICONSORT" );
				var icons = iconsort.FirstTexture( "U_ICONSORT00" );
				var sepia = iconsort.FirstTexture( "U_ICONSORT01" );
				icons.Crop( 256, 256, 72 * 0, 64 * 0, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-11.png" ) );
				icons.Crop( 256, 256, 72 * 1, 64 * 0, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-01.png" ) );
				icons.Crop( 256, 256, 72 * 2, 64 * 0, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-02.png" ) );
				icons.Crop( 256, 256, 72 * 0, 64 * 1, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-03.png" ) );
				icons.Crop( 256, 256, 72 * 1, 64 * 1, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-06.png" ) );
				icons.Crop( 256, 256, 72 * 2, 64 * 1, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-10.png" ) );
				icons.Crop( 256, 256, 72 * 0, 64 * 2, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-07.png" ) );
				icons.Crop( 256, 256, 72 * 1, 64 * 2, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-08.png" ) );
				icons.Crop( 256, 256, 72 * 2, 64 * 2, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-09.png" ) );
				icons.Crop( 256, 256, 72 * 0, 64 * 3, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-04.png" ) );
				icons.Crop( 256, 256, 72 * 1, 64 * 3, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-05.png" ) );
				icons.Crop( 256, 256, 72 * 2, 64 * 3, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-00.png" ) );
				sepia.Crop( 256, 256, 72 * 0, 64 * 0, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-sepia-00.png" ) );
				sepia.Crop( 256, 256, 72 * 1, 64 * 0, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-sepia-01.png" ) );
				sepia.Crop( 256, 256, 72 * 2, 64 * 0, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-sepia-02.png" ) );
				sepia.Crop( 256, 256, 72 * 0, 64 * 1, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-sepia-03.png" ) );
				sepia.Crop( 256, 256, 72 * 1, 64 * 1, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-sepia-06.png" ) );
				sepia.Crop( 256, 256, 72 * 2, 64 * 1, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-sepia-10.png" ) );
				sepia.Crop( 256, 256, 72 * 0, 64 * 2, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-sepia-07.png" ) );
				sepia.Crop( 256, 256, 72 * 1, 64 * 2, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-sepia-08.png" ) );
				sepia.Crop( 256, 256, 72 * 2, 64 * 2, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-sepia-09.png" ) );
				sepia.Crop( 256, 256, 72 * 0, 64 * 3, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-sepia-04.png" ) );
				sepia.Crop( 256, 256, 72 * 1, 64 * 3, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-sepia-05.png" ) );
				sepia.Crop( 256, 256, 72 * 2, 64 * 3, 60, 60 ).Save( Path.Combine( outpath, "item-categories", "cat-sepia-11.png" ) );
			}
			{
				var icons = ui.GetTxmTxv( version, "ICONKIND" ).OnlyTexture();
				for ( int y = 0; y < 8; ++y ) {
					for ( int x = 0; x < 8; ++x ) {
						icons.Crop( 256, 256, 32 * x, 32 * y, 32, 32 ).Save( Path.Combine( outpath, "item-icons", string.Format( "ICON{0}.png", y * 8 + x ) ) );
					}
				}
			}
			foreach ( string name in ui.GetChildNames().Where( x => x.StartsWith( "ITEM_" ) && x.EndsWith( ".TXM" ) ) ) {
				string n = Path.GetFileNameWithoutExtension( name );
				if ( n != "ITEM_GUD22" ) {
					var txv = ui.GetTxmTxv( version, n );
					txv.FirstTexture( "U_" + n ).Save( Path.Combine( outpath, "items", "U_" + n + ".png" ) );
				} else {
					// contains wrong and duplicate image
					Console.WriteLine( "Skipping known bad texture " + n );
				}
			}
			{
				ui.GetTxmTxv( version, "WORLDMAP" ).SaveAll( Path.Combine( outpath, "map" ) );
				ui.GetTxmTxv( version, "WORLDNAVI" ).SaveAll( Path.Combine( outpath, "map" ) );
			}
			{
				var artes = menu.FirstTexture( "U_MENUART00" );
				artes.Crop( 128, 128, 32 * 0, 32 * 0, 30, 30 ).Save( Path.Combine( outpath, "menu-icons", "artes-00.png" ) );
				artes.Crop( 128, 128, 32 * 1, 32 * 0, 30, 30 ).Save( Path.Combine( outpath, "menu-icons", "artes-01.png" ) );
				artes.Crop( 128, 128, 32 * 2, 32 * 0, 30, 30 ).Save( Path.Combine( outpath, "menu-icons", "artes-02.png" ) );
				artes.Crop( 128, 128, 32 * 0, 32 * 1, 30, 30 ).Save( Path.Combine( outpath, "menu-icons", "artes-04.png" ) );
				artes.Crop( 128, 128, 32 * 1, 32 * 1, 30, 30 ).Save( Path.Combine( outpath, "menu-icons", "artes-05.png" ) );
				artes.Crop( 128, 128, 32 * 2, 32 * 1, 30, 30 ).Save( Path.Combine( outpath, "menu-icons", "artes-06.png" ) );
				artes.Crop( 128, 128, 32 * 0, 32 * 2, 30, 30 ).Save( Path.Combine( outpath, "menu-icons", "artes-08.png" ) );
				artes.Crop( 128, 128, 32 * 1, 32 * 2, 30, 30 ).Save( Path.Combine( outpath, "menu-icons", "artes-09.png" ) );
				artes.Crop( 128, 128, 32 * 0, 32 * 3, 30, 30 ).Save( Path.Combine( outpath, "menu-icons", "artes-12.png" ) );
				artes.Crop( 128, 128, 32 * 1, 32 * 3, 32, 32 ).Save( Path.Combine( outpath, "menu-icons", "artes-13.png" ) );
				artes.Crop( 128, 128, 32 * 2, 32 * 3, 32, 32 ).Save( Path.Combine( outpath, "menu-icons", "artes-14.png" ) );
				artes.Crop( 128, 128, 32 * 3, 32 * 3, 32, 32 ).Save( Path.Combine( outpath, "menu-icons", "artes-15.png" ) );

				var weather = menu.FirstTexture( "U_MENUTOP02" );
				for ( int i = 0; i < 8; ++i ) {
					int x = i % 5;
					int y = i / 5;
					var w = weather.Crop( 256, 128, 48 * x, 48 * y, 48, 48 );
					w.Save( Path.Combine( outpath, "menu-icons", string.Format( "weather-{0}.png", i ) ) );
					w.AddBorder( 48, 48, 8, Color.FromArgb( 0, 0, 0, 0 ) ).Save( Path.Combine( outpath, "menu-icons", string.Format( "weather-{0}-64px.png", i ) ) ); ;
					//if ( i == 7 ) {
					//	var a = w.Scale( 48, 48, 32, 32 );
					//	a.Save( Path.Combine( outpath, "menu-icons", "artes-.png" ) );
					//	a.Save( Path.Combine( outpath, "menu-icons", "artes-xx.png" ) );
					//}
				}
				bat.AppendLine( @"imagemagick menu-icons\weather-7.png -filter Sinc -resize 32x32 menu-icons\artes-.png" );
				bat.AppendLine( @"imagemagick menu-icons\weather-7.png -filter Sinc -resize 32x32 menu-icons\artes-xx.png" );

				var main = menu.FirstTexture( "U_MENUTOP00" );
				for ( int i = 1; i < 11; ++i ) {
					int x = i % 3;
					int y = i / 3;
					main.Crop( 256, 512, 80 * x, 80 * y, 72, 72 ).Save( Path.Combine( outpath, "menu-icons", string.Format( "main-{0:D2}.png", i ) ) );
				}
				var sub = menu.FirstTexture( "U_MENUTOP01" );
				for ( int i = 4; i < 15; ++i ) {
					int x = i % 3;
					int y = i / 3;
					sub.Crop( 256, 512, 72 * x, 72 * y, 64, 64 ).Save( Path.Combine( outpath, "menu-icons", string.Format( "sub-{0:D2}.png", i ) ) );
				}

				var btlmenuartes = menu.FirstTexture( "U_MENUBTLINFO06" );
				var btlmenuequip = menu.FirstTexture( "U_MENUBTLINFO07" );
				var btlmenuitems = menu.FirstTexture( "U_MENUBTLINFO08" );
				var btlmenuskill = menu.FirstTexture( "U_MENUBTLINFO09" );
				var btlmenustrat = menu.FirstTexture( "U_MENUBTLINFO10" );
				var btlmenuescape = menu.FirstTexture( "U_MENUBTLINFO11" );
				for ( int x = 0; x < 8; ++x ) {
					btlmenuartes.Crop( 512, 64, 64 * x, 0, 64, 64 ).Save( Path.Combine( outpath, "menu-icons", string.Format( "battle-menu-artes-{0:D2}.png", x ) ) );
					btlmenuequip.Crop( 512, 64, 64 * x, 0, 64, 64 ).Save( Path.Combine( outpath, "menu-icons", string.Format( "battle-menu-equip-{0:D2}.png", x ) ) );
					btlmenuitems.Crop( 512, 64, 64 * x, 0, 64, 64 ).Save( Path.Combine( outpath, "menu-icons", string.Format( "battle-menu-items-{0:D2}.png", x ) ) );
					btlmenustrat.Crop( 512, 64, 64 * x, 0, 64, 64 ).Save( Path.Combine( outpath, "menu-icons", string.Format( "battle-menu-strat-{0:D2}.png", x ) ) );
					btlmenuescape.Crop( 512, 64, 64 * x, 0, 64, 64 ).Save( Path.Combine( outpath, "menu-icons", string.Format( "battle-menu-escape-{0:D2}.png", x ) ) );
				}
				for ( int x = 0; x < 4; ++x ) {
					btlmenuskill.Crop( 256, 64, 64 * x, 0, 64, 64 ).Save( Path.Combine( outpath, "menu-icons", string.Format( "battle-menu-skill-{0:D2}.png", x ) ) );
				}
			}
			{
				var menulib = menu.FirstTexture( "U_MENULIB00" );
				menulib.Crop( 256, 256, 64 * 0, 64 * 0, 56, 56 ).Save( Path.Combine( outpath, "monster-categories", "cat-0.png" ) );
				menulib.Crop( 256, 256, 64 * 1, 64 * 0, 56, 56 ).Save( Path.Combine( outpath, "monster-categories", "cat-1.png" ) );
				menulib.Crop( 256, 256, 64 * 2, 64 * 0, 56, 56 ).Save( Path.Combine( outpath, "monster-categories", "cat-2.png" ) );
				menulib.Crop( 256, 256, 64 * 3, 64 * 0, 56, 56 ).Save( Path.Combine( outpath, "monster-categories", "cat-3.png" ) );
				menulib.Crop( 256, 256, 64 * 0, 64 * 1, 56, 56 ).Save( Path.Combine( outpath, "monster-categories", "cat-4.png" ) );
				menulib.Crop( 256, 256, 64 * 1, 64 * 1, 56, 56 ).Save( Path.Combine( outpath, "monster-categories", "cat-5.png" ) );
				menulib.Crop( 256, 256, 64 * 2, 64 * 1, 56, 56 ).Save( Path.Combine( outpath, "monster-categories", "cat-6.png" ) );
				menulib.Crop( 256, 256, 64 * 3, 64 * 1, 56, 56 ).Save( Path.Combine( outpath, "monster-categories", "cat-7.png" ) );
				menulib.Crop( 256, 256, 64 * 0, 64 * 2, 56, 56 ).Save( Path.Combine( outpath, "monster-categories", "cat-8.png" ) );
			}
			{
				var icons = ui.GetTxmTxv( version, "ICONMONS" ).OnlyTexture();
				Color borderColor = icons.GetPixel( icons.Width - 1, icons.Height - 1 );
				for ( int i = 0; i < 372; ++i ) {
					int x = i % 20;
					int y = i / 20;
					var monster = icons.Crop( 1024, 1024, 50 * x + 2, 50 * y + 2, 44, 44 );
					monster.Save( Path.Combine( outpath, "monster-icons", "44px", string.Format( "monster-{0:D3}.png", i ) ) );
					monster.AddBorder( 44, 44, 1, borderColor ).Save( Path.Combine( outpath, "monster-icons", "46px", string.Format( "monster-{0:D3}.png", i ) ) );
					monster.AddBorder( 44, 44, 2, borderColor ).Save( Path.Combine( outpath, "monster-icons", "48px", string.Format( "monster-{0:D3}.png", i ) ) );
				}
			}
			foreach ( string name in ui.GetChildNames().Where( x => x.StartsWith( "COOK_" ) && x.EndsWith( ".TXM" ) ) ) {
				string n = Path.GetFileNameWithoutExtension( name );
				var txv = ui.GetTxmTxv( version, n );
				txv.FirstTexture( "U_" + n ).Save( Path.Combine( outpath, "recipes", "U_" + n + ".png" ) );
			}
			{
				var sym = ui.GetTxmTxv( version, "ICONSYM" );
				var glyph = sym.FirstTexture( "U_ICONSYM00" );
				for ( int i = 0; i < 12; ++i ) {
					int x = i % 3;
					int y = i / 3;
					glyph.Crop( 128, 256, 40 * x, 40 * y, 40, 40 ).Save( Path.Combine( outpath, "skill-icons", string.Format( "symbol-{0:D2}.png", i ) ) );
				}
				glyph.Crop( 128, 256, 32 * 0, 160, 30, 30 ).Save( Path.Combine( outpath, "skill-icons", "category-borderless-0-nocolor.png" ) );
				glyph.Crop( 128, 256, 32 * 1, 160, 30, 30 ).Save( Path.Combine( outpath, "skill-icons", "category-borderless-1-nocolor.png" ) );
				glyph.Crop( 128, 256, 32 * 2, 160, 30, 30 ).Save( Path.Combine( outpath, "skill-icons", "category-borderless-2-nocolor.png" ) );
				glyph.Crop( 128, 256, 32 * 3, 160, 30, 30 ).Save( Path.Combine( outpath, "skill-icons", "category-borderless-3-nocolor.png" ) );
				var icons = sym.FirstTexture( "U_ICONSYM01" );
				icons.Crop( 128, 128, 32 * 0, 32 * 0, 30, 30 ).Save( Path.Combine( outpath, "skill-icons", "set.png" ) );
				icons.Crop( 128, 128, 32 * 1, 32 * 0, 30, 30 ).Save( Path.Combine( outpath, "skill-icons", "category-0-nocolor.png" ) );
				icons.Crop( 128, 128, 32 * 2, 32 * 0, 30, 30 ).Save( Path.Combine( outpath, "skill-icons", "category-1-nocolor.png" ) );
				icons.Crop( 128, 128, 32 * 3, 32 * 0, 30, 30 ).Save( Path.Combine( outpath, "skill-icons", "category-2-nocolor.png" ) );
				icons.Crop( 128, 128, 32 * 0, 32 * 1, 30, 30 ).Save( Path.Combine( outpath, "skill-icons", "category-3-nocolor.png" ) );
				icons.Crop( 128, 128, 32 * 1, 32 * 0, 30, 30 ).ReplaceColor( 255, 120, 120 ).Save( Path.Combine( outpath, "skill-icons", "category-0.png" ) );
				icons.Crop( 128, 128, 32 * 2, 32 * 0, 30, 30 ).ReplaceColor( 250, 250, 100 ).Save( Path.Combine( outpath, "skill-icons", "category-1.png" ) );
				icons.Crop( 128, 128, 32 * 3, 32 * 0, 30, 30 ).ReplaceColor( 180, 220, 100 ).Save( Path.Combine( outpath, "skill-icons", "category-2.png" ) );
				icons.Crop( 128, 128, 32 * 0, 32 * 1, 30, 30 ).ReplaceColor( 130, 141, 229 ).Save( Path.Combine( outpath, "skill-icons", "category-3.png" ) );
				icons.Crop( 128, 128, 32 * 1, 32 * 1, 30, 30 ).Save( Path.Combine( outpath, "skill-icons", "all.png" ) );
				icons.Crop( 128, 128, 32 * 2, 32 * 1, 30, 30 ).Save( Path.Combine( outpath, "skill-icons", "empty.png" ) );
				icons.Crop( 128, 128, 32 * 3, 32 * 1, 30, 30 ).Save( Path.Combine( outpath, "skill-icons", "equip.png" ) );
			}
			for ( int i = 1; i <= 154; ++i ) {
				string name = string.Format( "SYNOPSIS_{0:D4}", i );
				var txv = ui.GetTxmTxv( version, name );
				txv.FirstTexture( "U_" + name ).Save( Path.Combine( outpath, "synopsis", "U_" + name + ".png" ) );
			}
			{
				// TODO: the actual button icons
				string filenameMainButtonArchive = version == GameVersion.PS3 ? "ICONBTNPS3" : "ICONBTN360";
				var buttons = ui.GetTxmTxv( version, filenameMainButtonArchive ).OnlyTexture();
				buttons.Crop( 512, 512, 40 *  0, 400 + 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-fs-01.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  1, 400 + 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-fs-02.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  2, 400 + 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-fs-03.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  3, 400 + 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-monster-01.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  4, 400 + 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-monster-02.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  5, 400 + 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-monster-03.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  6, 400 + 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-monster-04.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  7, 400 + 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-monster-05.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  8, 400 + 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-monster-06.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  9, 400 + 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-monster-07.png" ) ); ;
				buttons.Crop( 512, 512, 40 * 10, 400 + 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-monster-08.png" ) ); ;
				buttons.Crop( 512, 512, 40 * 11, 400 + 40 * 0, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-monster-09.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  0, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-01.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  1, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-02.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  2, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-03.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  3, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-04.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  4, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-05.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  5, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-06.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  6, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-07.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  7, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-08.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  8, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-09.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  9, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-10.png" ) ); ;
				buttons.Crop( 512, 512, 40 * 10, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-11.png" ) ); ;
				buttons.Crop( 512, 512, 40 * 11, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-12.png" ) ); ;
				buttons.Crop( 512, 512, 40 * 12, 400 + 40 * 1, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-13.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  0, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-14.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  1, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-15.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  2, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-16.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  3, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-17.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  4, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-status-18.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  5, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-element-01.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  6, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-element-02.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  7, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-element-03.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  8, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-element-04.png" ) ); ;
				buttons.Crop( 512, 512, 40 *  9, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-element-05.png" ) ); ;
				buttons.Crop( 512, 512, 40 * 10, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-element-06.png" ) ); ;
				buttons.Crop( 512, 512, 40 * 11, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-element-07.png" ) ); ;
				buttons.Crop( 512, 512, 40 * 12, 400 + 40 * 2, 32, 32 ).Save( Path.Combine( outpath, "text-icons", "icon-costume.png" ) ); ;
			}
			foreach ( string name in ui.GetChildNames().Where( x => x.StartsWith( "MENUWORLDMAP" ) && x.EndsWith( ".TXM" ) ) ) {
				var txv = ui.GetTxmTxv( version, Path.GetFileNameWithoutExtension( name ) );
				txv.OnlyTexture().Crop( 256, 256, 0, 0, 248, 168 ).Save( Path.Combine( outpath, "worldmap", txv.textures.First().TXM.Name + ".png" ) );
			}

			if ( !version.Is360() ) { // 360 version stores search points differently, haven't decoded that
				Bitmap worldMapImage;
				if ( worldmapOverride != null ) {
					worldMapImage = worldmapOverride;
				} else {
					worldMapImage = IntegerScaled( ui.GetTxmTxv( version, "WORLDNAVI" ).FirstTexture( "U_WORLDNAVI00" ), 5, 4 );
				}
				var searchPoints = new TOVSEAF.TOVSEAF( TryGetSearchPoints( gameDataPath, locale, version ), endian );
				string versionDir = version.ToString().ToLowerInvariant();
				Directory.CreateDirectory( Path.Combine( outpath, "etc", versionDir ) );
				searchPoints.GenerateMap( worldMapImage ).Save( Path.Combine( outpath, "etc", versionDir, "SearchPoint.png" ) );
				bat.AppendLine( string.Format( @"imagemagick etc\{0}\SearchPoint.png -filter Hermite -resize 1280x1024 etc\{0}\SearchPoint.jpg", versionDir ) );
			}

			File.WriteAllText( Path.Combine( outpath, "_postprocessing.bat" ), bat.ToString() );
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

		public static WebsiteGenerator LoadWebsiteGenerator( IContainer topLevelGameDataContainer, GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, Util.Endianness endian, Util.GameTextEncoding encoding, Util.Bitness bits ) {
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
