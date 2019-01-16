using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.Website {
	public partial class WebsiteGenerator {
		public string GenerateHtmlItems( uint? icon = null, uint? category = null ) {
			Console.WriteLine( "Generating Website: Items" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Items" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			foreach ( var item in Items.items ) {
				int itemCat = (int)item.Data[(int)ItemDat.ItemData.Category];
				int itemIcon = (int)item.Data[(int)ItemDat.ItemData.Icon];

				if ( itemCat == 0 ) { continue; }
				if ( category != null && category != itemCat ) { continue; }
				if ( icon != null && icon != itemIcon ) { continue; }

				sb.AppendLine( ItemDat.ItemDat.GetItemDataAsHtml( Version, VersionPostfix, Locale, Language, Items, item, Skills, Enemies, Recipes, Locations, Shops, SearchPoints, StringDic, InGameIdDict ) );
				sb.AppendLine( "<tr><td colspan=\"5\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlEnemies( int? category = null ) {
			Console.WriteLine( "Generating Website: Enemies" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Enemies" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			foreach ( var enemy in Enemies.EnemyList ) {
				if ( enemy.InGameID == 0 ) { continue; }
				if ( category != null && category != enemy.Category ) { continue; }
				sb.AppendLine( enemy.GetDataAsHtml( Version, VersionPostfix, Locale, Language, Items, Locations, StringDic, InGameIdDict ) );
				sb.AppendLine( "<tr><td colspan=\"7\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlEnemyGroups() {
			Console.WriteLine( "Generating Website: Enemy Groups" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Enemy Groups" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			foreach ( var group in EnemyGroups.EnemyGroupList ) {
				sb.AppendLine( group.GetDataAsHtml( Enemies, InGameIdDict, Version, VersionPostfix, Locale, Language ) );
				sb.AppendLine( "<tr><td colspan=\"9\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlEncounterGroups() {
			Console.WriteLine( "Generating Website: Encounters" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Encounter Groups" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			foreach ( var group in EncounterGroups.EncounterGroupList ) {
				sb.AppendLine( group.GetDataAsHtml( EnemyGroups, Enemies, InGameIdDict, Version, VersionPostfix, Locale, Language ) );
				sb.AppendLine( "<tr><td colspan=\"9\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlSkills() {
			Console.WriteLine( "Generating Website: Skills" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Skills" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			foreach ( var skill in Skills.SkillList ) {
				if ( skill.ID == 0 ) { continue; }
				sb.AppendLine( skill.GetDataAsHtml( Version, VersionPostfix, Locale, Language, StringDic, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"4\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlArtes() {
			Console.WriteLine( "Generating Website: Artes" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Artes" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
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
				sb.AppendLine( arte.GetDataAsHtml( Version, VersionPostfix, Locale, Language, Artes.ArteIdDict, Enemies, Skills, StringDic, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"4\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlSynopsis() {
			Console.WriteLine( "Generating Website: Synopsis" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Synopsis" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			foreach ( var entry in Synopsis.SynopsisList ) {
				if ( InGameIdDict[entry.NameStringDicId].StringEngOrJpn == "" ) { continue; }
				sb.AppendLine( entry.GetDataAsHtml( Version, VersionPostfix, Locale, Language, StringDic, InGameIdDict ) );
				sb.AppendLine( "<hr>" );
			}
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlSkitInfo() {
			Console.WriteLine( "Generating Website: Skit Info" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Skits" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			foreach ( var entry in Skits.SkitInfoList ) {
				sb.AppendLine( entry.GetDataAsHtml( Version, VersionPostfix, Locale, Language, Skits, InGameIdDict ) );
				sb.AppendLine( "<hr>" );
			}
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlSkitIndex() {
			Console.WriteLine( "Generating Website: Skit Index" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Skit Index" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			foreach ( var entry in Skits.SkitInfoList ) {
				sb.AppendLine( entry.GetIndexDataAsHtml( Version, VersionPostfix, Locale, Language, Skits, InGameIdDict ) );
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlBattleVoicesEnd() {
			Console.WriteLine( "Generating Website: Battle Voices (End)" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Battle Voices (End)" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			for ( int i = 0; i < BattleVoicesEnd.Blocks.Count; ++i ) {
				{
					sb.Append( "<tr>" );
					var v = BattleVoicesEnd.Blocks[i];
					sb.Append( "<td>" ).Append( v.Identifier ).Append( "</td>" );
					sb.Append( "<td>" ).Append( v.Entries[0].ScenarioStart ).Append( "</td>" );
					sb.Append( "<td>" ).Append( v.Entries[0].ScenarioEnd ).Append( "</td>" );
					sb.Append( "<td>" );
					Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, InGameIdDict, Version, v.Entries[0].CharacterBitmask );
					sb.Append( "</td>" );
					sb.Append( "<td>" );
					sb.Append( "Kill: " );
					Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, InGameIdDict, Version, v.Entries[0].KillCharacterBitmask );
					sb.Append( "</td>" );
					for ( int j = 0; j < v.CharacterSpecificData.Length; ++j ) {
						if ( v.CharacterSpecificData[j] != 0 ) {
							sb.Append( "<td>" );
							Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, InGameIdDict, Version, (uint)( 1 << j ) );
							sb.Append( v.CharacterSpecificData[j] );
							sb.Append( "</td>" );
						}
					}
					sb.Append( "</tr>" );

					sb.Append( "<tr>" );
					foreach ( var e in v.Entries ) {
						sb.Append( "<td style=\"min-width:60px\">" );
						if ( !String.IsNullOrWhiteSpace( e.RefString ) ) {
							sb.Append( e.RefString ).Append( "<br>" );
						}
						for ( int j = 0; j < e.Data.Length; ++j ) {
							switch ( j ) {
								case 1:
									if ( e.Data[j] != 0 ) {
										sb.Append( "Chance: " ).Append( e.Data[j] ).Append( "%" ).Append( "<br>" );
									}
									break;
								case 2:
								case 3:
								case 5:
								case 6:
								case 22:
									break;
								case 7:
								case 8:
								case 9:
								case 10:
								case 11:
								case 12:
								case 13:
								case 14:
								case 15:
									if ( e.Data[j] != 16 ) {
										sb.Append( j ).Append( ": " ).Append( e.Data[j] );
										Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, InGameIdDict, Version, (uint)( 1 << (int)( j - 7 ) ) );
										sb.Append( "<br>" );
									}
									break;
								case 16:
									if ( e.Data[j] != 0 ) {
										sb.Append( "Arte Type (?): " ).Append( ( (T8BTMA.Arte.ArteType)e.Data[j] - 1 ).ToString() ).Append( "<br>" );
									}
									break;
								case 19:
									if ( e.Data[j] != 0 ) {
										sb.Append( "Max Battle Time: " ).Append( e.Data[j] ).Append( " frames" ).Append( "<br>" );
									}
									break;
								case 20:
									if ( e.Data[j] != 0 ) {
										sb.Append( "HP: " );
										switch ( e.Data[j] ) {
											case 1: sb.Append( "Full" ); break;
											case 3: sb.Append( "Low" ); break;
											default: sb.Append( "? (" ).Append( e.Data[j] ).Append( ")" ); break;
										}
										sb.Append( "<br>" );
									}
									break;
								case 9999:
									if ( e.Data[j] != 0 ) {
										sb.Append( j ).Append( ": " );
										Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, InGameIdDict, Version, e.Data[j] );
										sb.Append( "<br>" );
									}
									break;
								default:
									if ( e.Data[j] != 0 ) {
										sb.Append( j ).Append( ": " ).Append( e.Data[j] ).Append( "<br>" );
									}
									break;
							}
						}
						sb.Append( "</td>" );
					}
					sb.Append( "</tr>" );
				}
				sb.Append( "<tr><td colspan=\"5\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlSearchPoints() {
			Console.WriteLine( "Generating Website: Search Points" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Search Points" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<img src=\"PS3-SearchPoint.png\" width=\"1280\" height=\"1024\">" );
			sb.Append( "<hr>" );
			sb.Append( "<table>" );
			int idx = 1;
			for ( int i = 0; i < SearchPoints.SearchPointDefinitions.Count; ++i ) {
				var sp = SearchPoints.SearchPointDefinitions[i];
				if ( sp.Unknown11 != 1 ) { continue; } // not sure what these mean exactly but only the ones with an '1' here show up in game
				sb.Append( sp.GetDataAsHtml( Version, VersionPostfix, Locale, Language, Items, StringDic, InGameIdDict, SearchPoints.SearchPointContents, SearchPoints.SearchPointItems, idx ) );
				sb.Append( "<tr><td colspan=\"5\"><hr></td></tr>" );
				++idx;
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlRecipes() {
			Console.WriteLine( "Generating Website: Recipes" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Recipes" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			for ( int i = 1; i < Recipes.RecipeList.Count; ++i ) {
				var recipe = Recipes.RecipeList[i];
				sb.Append( "<tr>" );
				sb.Append( recipe.GetDataAsHtml( Version, VersionPostfix, Locale, Language, Recipes, Items, StringDic, InGameIdDict ) );
				sb.Append( "</tr>" );
				sb.Append( "<tr><td colspan=\"4\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlLocations() {
			Console.WriteLine( "Generating Website: Locations" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Locations" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.AppendLine( "<table>" );
			for ( int i = 1; i < Locations.LocationList.Count; ++i ) {
				var location = Locations.LocationList[i];
				sb.AppendLine( location.GetDataAsHtml( Version, VersionPostfix, Locale, Language, StringDic, InGameIdDict, EncounterGroups, EnemyGroups, Enemies, Shops ) );
				sb.Append( "<tr><td colspan=\"3\"><hr></td></tr>" );
			}
			sb.AppendLine( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlStrategy() {
			Console.WriteLine( "Generating Website: Strategy" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Strategy" );
			sb.Append( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			foreach ( var entry in Strategy.StrategySetList ) {
				sb.Append( entry.GetDataAsHtml( Version, VersionPostfix, Locale, Language, Strategy, StringDic, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"10\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.Append( "<table>" );
			foreach ( var entry in Strategy.StrategyOptionList ) {
				sb.Append( entry.GetDataAsHtml( Version, VersionPostfix, Locale, Language, StringDic, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"4\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.Append( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlShops() {
			Console.WriteLine( "Generating Website: Shops" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Shops" );
			sb.Append( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			foreach ( var entry in Shops.ShopDefinitions ) {
				if ( entry.InGameID == 1 ) { continue; } // dummy shop
				sb.Append( entry.GetDataAsHtml( Version, VersionPostfix, Locale, Language, Items, Shops, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"6\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.Append( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlTitles() {
			Console.WriteLine( "Generating Website: Titles" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Titles" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			foreach ( var entry in Titles.TitleList ) {
				if ( entry.Character == 0 ) { continue; }
				sb.Append( entry.GetDataAsHtml( Version, VersionPostfix, Locale, Language, StringDic, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"4\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlBattleBook() {
			Console.WriteLine( "Generating Website: Battle Book" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Battle Book" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );

			sb.Append( "<table>" );

			foreach ( var entry in BattleBook.BattleBookEntryList ) {
				string data;
				if ( InGameIdDict[entry.NameStringDicId].StringEngOrJpn == "" ) { continue; }
				data = entry.GetDataAsHtml( Version, VersionPostfix, Locale, Language, StringDic, InGameIdDict );
				sb.Append( "<tr>" );
				sb.Append( data );
				sb.Append( "</tr>" );
				sb.Append( "<tr><td colspan=\"2\"><hr></td></tr>" );
			}

			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlRecords() {
			Console.WriteLine( "Generating Website: Records" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Records" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );

			sb.Append( "<table>" );
			foreach ( var i in Records ) {
				AppendRecord( sb, Version, VersionPostfix, Locale, Language, InGameIdDict, i );
			}
			sb.Append( "</table>" );

			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlSettings() {
			Console.WriteLine( "Generating Website: Settings" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Settings" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );

			sb.Append( "<table class=\"settings\">" );
			foreach ( var s in Settings ) {
				AppendSetting( sb, Version, VersionPostfix, Locale, Language, InGameIdDict, s.NameStringDicId, s.DescStringDicId, s.OptionsStringDicIds[0],
					s.OptionsStringDicIds[1], s.OptionsStringDicIds[2], s.OptionsStringDicIds[3] );
				sb.Append( "<tr><td colspan=\"5\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );

			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlTrophies() {
			Console.WriteLine( "Generating Website: Trophies" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Trophies" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			foreach ( var kvp in TrophyJp.Trophies ) {
				var jp = kvp.Value;
				var en = TrophyEn?.Trophies[kvp.Key];

				sb.Append( TrophyNodeToHtml( Version, InGameIdDict, VersionPostfix, Locale, Language, jp, en ) );
				sb.Append( "<tr><td colspan=\"3\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlGradeShop() {
			Console.WriteLine( "Generating Website: Grade Shop" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Grade Shop" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table>" );
			foreach ( var entry in GradeShop.GradeShopEntryList ) {
				if ( entry.GradeCost == 0 ) { continue; }
				sb.Append( entry.GetDataAsHtml( Version, VersionPostfix, Locale, Language, StringDic, InGameIdDict ) );
				sb.Append( "<tr><td colspan=\"3\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlNecropolis( string dir, bool showEnemies ) {
			Console.WriteLine( "Generating Website: Necropolis" );
			var sb = new StringBuilder();
			AddHeader( sb, Version, "Necropolis of Nostalgia" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );

			foreach ( var floor in NecropolisFloors.FloorList ) {
				int floorNumberLong = Int32.Parse( floor.RefString1.Split( '_' ).Last() );
				int floorNumber = ( floorNumberLong - 1 ) % 10 + 1;
				int floorStratumAsNumber = ( floorNumberLong - 1 ) / 10 + 1;
				string floorStratum = ( (char)( floorStratumAsNumber + 64 ) ).ToString();
				string html = NecropolisMaps[floor.RefString2].GetDataAsHtml( floorStratum, floorNumber, showEnemies ? Enemies : null, showEnemies ? EnemyGroups : null, showEnemies ? EncounterGroups : null, Version, VersionPostfix, Locale, Language, NecropolisTreasures, Items, InGameIdDict );
				sb.Append( html );
				sb.Append( "<hr>" );

				StringBuilder sb2 = new StringBuilder();
				AddHeader( sb2, Version, floorStratum + "-" + floorNumber + " - Necropolis of Nostalgia" );
				sb2.Append( "<body>" );
				sb2.Append( html );
				AddFooter( sb2 );
				sb2.Append( "</body></html>" );
				System.IO.File.WriteAllText( Path.Combine( dir, WebsiteGenerator.GetUrl( showEnemies ? WebsiteSection.NecropolisEnemy : WebsiteSection.NecropolisMap, Version, VersionPostfix, Locale, Language, false, extra: floorStratum + floorNumber ) ), sb2.ToString(), Encoding.UTF8 );
			}

			/*
			foreach ( var treasureLayout in NecropolisTreasures.TreasureInfoList ) {
				sb.Append( "<hr>" );
				sb.Append( treasureLayout.GetDataAsHtml( Items, InGameIdDict ) );
			}
			//*/

			AddFooter( sb );
			sb.AppendLine( "</body></html>" );
			return sb.ToString();
		}
		public string GenerateHtmlNpc() {
			Console.WriteLine( "Generating Website: NPCs" );

			StringBuilder sb = new StringBuilder();
			AddHeader( sb, Version, "NPC Dialogue" );
			sb.AppendLine( "<body>" );
			//AddMenuBar( sb, Version, VersionPostfix, Locale, Language, IconsWithItems, InGameIdDict );
			sb.Append( "<table class=\"npcdiff\">" );
			foreach ( var kvp in NpcDefs ) {
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
					sb.Append( InGameIdDict[d.StringDicId].StringJpnHtml( Version, InGameIdDict ) );
					sb.Append( "</td>" );

					sb.Append( "<td>" );
					sb.Append( InGameIdDict[d.StringDicId].StringEngHtml( Version, InGameIdDict ) );
					sb.Append( "</td>" );

					sb.Append( "</tr>" );

					sb.Append( "<tr><td colspan=\"3\"><hr></td></tr>" );
				}
			}
			sb.Append( "</table>" );
			sb.Append( "</body></html>" );

			return sb.ToString();
		}

		public static string ScenarioProcessGroupsToHtml( List<List<ScenarioData>> groups, ScenarioType type, GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, Dictionary<uint, TSS.TSSEntry> inGameIdDict, uint[] iconsWithItems, bool phpLinks = false ) {
			var sb = new StringBuilder();

			AddHeader( sb, version, "Scenario Index (" + type.ToString() + ")" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb, version, versionPostfix, locale, websiteLanguage, iconsWithItems, inGameIdDict );
			sb.Append( "<div class=\"scenario-index\">" );

			sb.Append( "<ul>" );
			for ( int i = 0; i < groups.Count; ++i ) {
				var group = groups[i];

				string commonBegin = ScenarioData.FindMostCommonStart( group );
				sb.Append( "<li>" );
				if ( type == ScenarioType.Maps ) {
					sb.Append( commonBegin.Split( new char[] { '_', '0' } )[0] );
				} else {
					if ( commonBegin != "" ) {
						sb.Append( commonBegin );
					} else {
						sb.Append( "Intro" );
					}
				}

				sb.Append( "<ul>" );
				for ( int j = 0; j < group.Count; ++j ) {
					var scene = group[j];

					sb.Append( "<li>" );
					sb.Append( "<a href=\"" );
					sb.Append( WebsiteGenerator.GetUrl( WebsiteSection.Scenario, version, versionPostfix, locale, websiteLanguage, phpLinks, extra: scene.EpisodeId ) );
					sb.Append( "\">" );
					sb.Append( scene.HumanReadableNameWithoutPrefix( commonBegin ) );
					sb.Append( "</a>" );

					foreach ( var skit in scene.Skits ) {
						sb.Append( "<ul>" );
						sb.Append( "<li>" );
						sb.Append( "<a href=\"" );
						sb.Append( WebsiteGenerator.GetUrl( WebsiteSection.Skit, version, versionPostfix, locale, websiteLanguage, phpLinks, extra: skit.RefString ) );
						sb.Append( "\">" );
						if ( websiteLanguage.WantsJp() ) {
							sb.Append( inGameIdDict[skit.StringDicIdName].GetStringHtml( 0, version, inGameIdDict ) );
						}
						if ( websiteLanguage.WantsBoth() ) {
							sb.Append( " (" );
						}
						if ( websiteLanguage.WantsEn() ) {
							sb.Append( inGameIdDict[skit.StringDicIdName].GetStringHtml( 1, version, inGameIdDict ) );
						}
						if ( websiteLanguage.WantsBoth() ) {
							sb.Append( ")" );
						}
						sb.Append( "</a>" );
						sb.Append( "</li>" );
						sb.Append( "</ul>" );
					}

					sb.Append( "</li>" );
				}
				sb.Append( "</ul>" );

				sb.Append( "</li>" );
			}
			sb.Append( "</ul>" );
			sb.Append( "</div>" );

			AddFooter( sb );
			sb.AppendLine( "</body></html>" );

			return sb.ToString();
		}
	}
}
