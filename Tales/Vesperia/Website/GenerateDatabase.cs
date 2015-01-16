using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace HyoutaTools.Tales.Vesperia.Website {
	public class GenerateDatabase {
		private GenerateWebsite Site;
		private IDbConnection DB;

		public GenerateDatabase( GenerateWebsite website, IDbConnection databaseConnection ) {
			this.Site = website;
			this.DB = databaseConnection;
		}

		public void ExportAll() {
			Console.WriteLine( "Exporting database" );
			try {
				DB.Open();
				ExportStringDic();
				ExportArtes();
				ExportSkills();
				ExportStrategy();
				ExportRecipes();
				ExportShops();
				ExportTitles();
				ExportSynopsis();
				ExportBattleBook();
				ExportMonsters();
				ExportMonsterGroups();
				ExportEncounterGroups();
				ExportItems();
				ExportWorldMap();
				ExportRecords();
				ExportSettings();
				ExportGradeShop();
				if ( Site.Version != GameVersion.X360 ) {
					ExportNecropolis();
					ExportScenarioDat();
					ExportSkitText();
					ExportScenarioMetadata();
				}
			} finally {
				DB.Close();
			}
		}

		enum TextboxType { Bubble, Information, Subtitle };
		private void ExportScenarioDat() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE ScenarioDat ( id INTEGER PRIMARY KEY AUTOINCREMENT, episodeId VARCHAR(16), displayOrder INT, type INT, jpName TEXT, jpText TEXT, enName TEXT, enText TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX ScenarioDat_EpisodeId_Index ON ScenarioDat ( episodeId )";
					command.ExecuteNonQuery();
				}

				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO ScenarioDat ( episodeId, displayOrder, type, jpName, jpText, enName, enText ) VALUES ( @episodeId, @displayOrder, @type, @jpName, @jpText, @enName, @enText )";
					command.AddParameter( "episodeId" );
					command.AddParameter( "displayOrder" );
					command.AddParameter( "type" );
					command.AddParameter( "jpName" );
					command.AddParameter( "jpText" );
					command.AddParameter( "enName" );
					command.AddParameter( "enText" );

					foreach ( var kvp in Site.ScenarioFiles ) {
						var episodeId = kvp.Key;
						var scenario = kvp.Value;

						for ( int i = 0; i < scenario.EntryList.Count; ++i ) {
							var entry = scenario.EntryList[i];
							TextboxType type = TextboxType.Bubble;
							if ( entry.JpName == "Information" ) { type = TextboxType.Information; }
							if ( entry.EnText == "Yes\nNo" ) { type = TextboxType.Information; }
							if ( entry.EnText.StartsWith( "\x03(2)" ) ) { type = TextboxType.Subtitle; }

							string[] jpTextArray = entry.JpText.Split( '\f' );
							string[] enTextArray = entry.EnText.Split( '\f' );
							int textboxCount = Math.Max( jpTextArray.Length, enTextArray.Length );

							const int maxTextboxCount = 16;
							Util.Assert( textboxCount <= maxTextboxCount );
							for ( int j = 0; j < textboxCount; ++j ) {
								string jpText = j < jpTextArray.Length ? jpTextArray[j] : "";
								string enText = j < enTextArray.Length ? enTextArray[j] : "";
								command.GetParameter( "episodeId" ).Value = episodeId;
								command.GetParameter( "displayOrder" ).Value = i * maxTextboxCount + j;
								command.GetParameter( "type" ).Value = (int)type;
								command.GetParameter( "jpName" ).Value = entry.JpName.ToHtmlJpn( Site.Version );
								command.GetParameter( "jpText" ).Value = jpText.ToHtmlJpn( Site.Version );
								command.GetParameter( "enName" ).Value = entry.EnName.ToHtmlEng( Site.Version );
								command.GetParameter( "enText" ).Value = enText.ToHtmlEng( Site.Version );
								command.ExecuteNonQuery();
							}
						}
					}
				}
				transaction.Commit();
			}
		}

		private void ExportSkitText() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE SkitText ( id INTEGER PRIMARY KEY AUTOINCREMENT, skitId VARCHAR(8), displayOrder INT, character TEXT, jpText TEXT, enText TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX SkitText_SkitId_Index ON SkitText ( skitId )";
					command.ExecuteNonQuery();
				}

				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO SkitText ( skitId, displayOrder, character, jpText, enText ) VALUES ( @skitId, @displayOrder, @character, @jpText, @enText )";
					command.AddParameter( "skitId" );
					command.AddParameter( "displayOrder" );
					command.AddParameter( "character" );
					command.AddParameter( "jpText" );
					command.AddParameter( "enText" );

					foreach ( var kvp in Site.SkitText ) {
						var skitId = kvp.Key;
						var skit = kvp.Value;

						for ( int i = 0; i < skit.Lines.Length; ++i ) {
							string name = skit.Lines[i].SName;
							int idx = name.IndexOf( '(' ) + 1;
							name = name.Substring( idx, name.LastIndexOf( ')' ) - idx );

							command.GetParameter( "skitId" ).Value = skitId;
							command.GetParameter( "displayOrder" ).Value = i;
							command.GetParameter( "character" ).Value = name;
							command.GetParameter( "jpText" ).Value = skit.Lines[i].SJPN.ToHtmlJpn( Site.Version );
							command.GetParameter( "enText" ).Value = skit.Lines[i].SENG.ToHtmlEng( Site.Version );
							command.ExecuteNonQuery();
						}
					}
				}
				transaction.Commit();
			}
		}

		private void ExportScenarioMetadata() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE ScenarioMeta ( id INTEGER PRIMARY KEY AUTOINCREMENT, type INT, sceneGroup INT, parent INT, episodeId VARCHAR(16), description TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX ScenarioMeta_Type_Group_Index ON ScenarioMeta ( type, sceneGroup )";
					command.ExecuteNonQuery();
				}

				ExportScenarioMetadata( transaction, Site.ScenarioGroupsStory, 1 );
				ExportScenarioMetadata( transaction, Site.ScenarioGroupsSidequests, 2 );
				transaction.Commit();
			}
		}

		private void ExportScenarioMetadata( IDbTransaction transaction, List<List<ScenarioData>> groups, int groupType ) {
			using ( var command = DB.CreateCommand() ) {
				command.CommandText = "INSERT INTO ScenarioMeta ( type, sceneGroup, parent, episodeId, description ) VALUES ( @type, @sceneGroup, @parent, @episodeId, @description )";
				command.AddParameter( "type" );
				command.AddParameter( "sceneGroup" );
				command.AddParameter( "parent" );
				command.AddParameter( "episodeId" );
				command.AddParameter( "description" );

				int groupNumber = 0;
				foreach ( var group in groups ) {
					string commonBegin = ScenarioData.FindMostCommonStart( group );
					command.GetParameter( "type" ).Value = groupType;
					command.GetParameter( "sceneGroup" ).Value = groupNumber;
					command.GetParameter( "parent" ).Value = null;
					command.GetParameter( "episodeId" ).Value = null;
					command.GetParameter( "description" ).Value = commonBegin;
					command.ExecuteNonQuery();

					long parentId = GetLastInsertedId();
					foreach ( var scene in group ) {
						command.GetParameter( "type" ).Value = groupType;
						command.GetParameter( "sceneGroup" ).Value = groupNumber;
						command.GetParameter( "parent" ).Value = parentId;
						command.GetParameter( "episodeId" ).Value = scene.EpisodeId;
						command.GetParameter( "description" ).Value = scene.HumanReadableNameWithoutPrefix( commonBegin );
						command.ExecuteNonQuery();

						long sceneId = GetLastInsertedId();
						foreach ( var skit in scene.Skits ) {
							command.GetParameter( "type" ).Value = groupType;
							command.GetParameter( "sceneGroup" ).Value = groupNumber;
							command.GetParameter( "parent" ).Value = sceneId;
							command.GetParameter( "episodeId" ).Value = skit.RefString;
							command.GetParameter( "description" ).Value = Site.InGameIdDict[skit.StringDicIdName].GetStringHtml( 1, GameVersion.PS3 );
							command.ExecuteNonQuery();
						}
					}

					++groupNumber;
				}
			}
		}

		private void ExportStringDic() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE StringDic ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT, language INT, entry TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX StringDic_GameId_Index ON StringDic ( gameId, language )";
					command.ExecuteNonQuery();
				}

				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO StringDic ( gameId, language, entry ) VALUES ( @gameId, @language, @entry )";
					command.AddParameter( "gameId" );
					command.AddParameter( "language" );
					command.AddParameter( "entry" );

					foreach ( var e in Site.StringDic.Entries ) {
						if ( e.inGameStringId > -1 ) {
							string jp = e.StringJpnHtml( Site.Version );
							string en = e.StringEngHtml( Site.Version );

							command.GetParameter( "gameId" ).Value = e.inGameStringId;
							command.GetParameter( "language" ).Value = 0;
							command.GetParameter( "entry" ).Value = jp;
							command.ExecuteNonQuery();

							command.GetParameter( "gameId" ).Value = e.inGameStringId;
							command.GetParameter( "language" ).Value = 1;
							command.GetParameter( "entry" ).Value = en;
							command.ExecuteNonQuery();
						}
					}
				}
				transaction.Commit();
			}
		}

		public void ExportArtes() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Artes ( "
						+ "id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, refString TEXT, strDicName INT, strDicDesc INT, type INT, character INT, tpUsage INT, "
						+ "fatalStrikeType INT, usableInMenu INT, fire INT, earth INT, wind INT, water INT, light INT, dark INT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Artes_LearnReqs ( id INTEGER PRIMARY KEY AUTOINCREMENT, arteId INT, type INT, value INT, useCount INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Artes_AlteredReqs ( id INTEGER PRIMARY KEY AUTOINCREMENT, arteId INT, type INT, value INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX Artes_Character_Type_Index ON Artes ( character, type )";
					command.ExecuteNonQuery();
				}

				using ( var commandArte = DB.CreateCommand() )
				using ( var commandLearnReq = DB.CreateCommand() )
				using ( var commandAlteredReq = DB.CreateCommand() ) {
					commandArte.CommandText = "INSERT INTO Artes ( id, gameId, refString, strDicName, strDicDesc, type, character, tpUsage, fatalStrikeType, "
						+ "usableInMenu, fire, earth, wind, water, light, dark, html ) VALUES ( @id, @gameId, @refString, @strDicName, @strDicDesc, @type, "
						+ "@character, @tpUsage, @fatalStrikeType, @usableInMenu, @fire, @earth, @wind, @water, @light, @dark, @html )";
					commandArte.AddParameter( "id" );
					commandArte.AddParameter( "gameId" );
					commandArte.AddParameter( "refString" );
					commandArte.AddParameter( "strDicName" );
					commandArte.AddParameter( "strDicDesc" );
					commandArte.AddParameter( "type" );
					commandArte.AddParameter( "character" );
					commandArte.AddParameter( "tpUsage" );
					commandArte.AddParameter( "fatalStrikeType" );
					commandArte.AddParameter( "usableInMenu" );
					commandArte.AddParameter( "fire" );
					commandArte.AddParameter( "earth" );
					commandArte.AddParameter( "wind" );
					commandArte.AddParameter( "water" );
					commandArte.AddParameter( "light" );
					commandArte.AddParameter( "dark" );
					commandArte.AddParameter( "html" );

					commandLearnReq.CommandText = "INSERT INTO Artes_LearnReqs ( arteId, type, value, useCount ) VALUES ( @arteId, @type, @value, @useCount )";
					commandLearnReq.AddParameter( "arteId" );
					commandLearnReq.AddParameter( "type" );
					commandLearnReq.AddParameter( "value" );
					commandLearnReq.AddParameter( "useCount" );

					commandAlteredReq.CommandText = "INSERT INTO Artes_AlteredReqs ( arteId, type, value ) VALUES ( @arteId, @type, @value )";
					commandAlteredReq.AddParameter( "arteId" );
					commandAlteredReq.AddParameter( "type" );
					commandAlteredReq.AddParameter( "value" );

					for ( int i = 0; i < Site.Artes.ArteList.Count; ++i ) {
						var arte = Site.Artes.ArteList[i];

						commandArte.GetParameter( "id" ).Value = arte.ID;
						commandArte.GetParameter( "gameId" ).Value = arte.InGameID;
						commandArte.GetParameter( "refString" ).Value = arte.RefString;
						commandArte.GetParameter( "strDicName" ).Value = arte.NameStringDicId;
						commandArte.GetParameter( "strDicDesc" ).Value = arte.DescStringDicId;
						commandArte.GetParameter( "type" ).Value = (int)arte.Type;
						commandArte.GetParameter( "character" ).Value = arte.Character;
						commandArte.GetParameter( "tpUsage" ).Value = arte.TPUsage;
						commandArte.GetParameter( "fatalStrikeType" ).Value = arte.FatalStrikeType;
						commandArte.GetParameter( "usableInMenu" ).Value = arte.UsableInMenu;
						commandArte.GetParameter( "fire" ).Value = arte.ElementFire;
						commandArte.GetParameter( "earth" ).Value = arte.ElementEarth;
						commandArte.GetParameter( "wind" ).Value = arte.ElementWind;
						commandArte.GetParameter( "water" ).Value = arte.ElementWater;
						commandArte.GetParameter( "light" ).Value = arte.ElementLight;
						commandArte.GetParameter( "dark" ).Value = arte.ElementDarkness;
						commandArte.GetParameter( "html" ).Value = arte.GetDataAsHtml( Site.Version, Site.Artes.ArteIdDict, Site.Enemies, Site.Skills, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						commandArte.ExecuteNonQuery();

						for ( int j = 0; j < arte.LearnRequirementsOtherArtesType.Length; ++j ) {
							if ( arte.LearnRequirementsOtherArtesType[j] <= 0 ) { continue; }
							commandLearnReq.GetParameter( "arteId" ).Value = arte.ID;
							commandLearnReq.GetParameter( "type" ).Value = arte.LearnRequirementsOtherArtesType[j];
							commandLearnReq.GetParameter( "value" ).Value = arte.LearnRequirementsOtherArtesId[j];
							commandLearnReq.GetParameter( "useCount" ).Value = arte.LearnRequirementsOtherArtesUsageCount[j];
							commandLearnReq.ExecuteNonQuery();
						}

						for ( int j = 0; j < arte.AlteredArteRequirementType.Length; ++j ) {
							if ( arte.AlteredArteRequirementType[j] <= 0 ) { continue; }
							commandAlteredReq.GetParameter( "arteId" ).Value = arte.ID;
							commandAlteredReq.GetParameter( "type" ).Value = arte.AlteredArteRequirementType[j];
							commandAlteredReq.GetParameter( "value" ).Value = arte.AlteredArteRequirementId[j];
							commandAlteredReq.ExecuteNonQuery();
						}
					}
				}
				transaction.Commit();
			}
		}

		public void ExportSkills() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Skills ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, refString TEXT, strDicName INT, strDicDesc INT, "
						+ "learnableBy INT, equipCost INT, learnCost INT, category INT, symbolValue INT, inactive INT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Skills ( id, gameId, refString, strDicName, strDicDesc, learnableBy, equipCost, learnCost, category, "
						+ "symbolValue, inactive, html ) VALUES ( @id, @gameId, @refString, @strDicName, @strDicDesc, @learnableBy, @equipCost, @learnCost, "
						+ "@category, @symbolValue, @inactive, @html )";
					command.AddParameter( "id" );
					command.AddParameter( "gameId" );
					command.AddParameter( "refString" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "learnableBy" );
					command.AddParameter( "equipCost" );
					command.AddParameter( "learnCost" );
					command.AddParameter( "category" );
					command.AddParameter( "symbolValue" );
					command.AddParameter( "inactive" );
					command.AddParameter( "html" );

					foreach ( var s in Site.Skills.SkillList ) {
						command.GetParameter( "id" ).Value = s.ID;
						command.GetParameter( "gameId" ).Value = s.InGameID;
						command.GetParameter( "refString" ).Value = s.RefString;
						command.GetParameter( "strDicName" ).Value = s.NameStringDicID;
						command.GetParameter( "strDicDesc" ).Value = s.DescStringDicID;
						command.GetParameter( "learnableBy" ).Value = s.LearnableByBitmask;
						command.GetParameter( "equipCost" ).Value = s.EquipCost;
						command.GetParameter( "learnCost" ).Value = s.LearnCost;
						command.GetParameter( "category" ).Value = s.Category;
						command.GetParameter( "symbolValue" ).Value = s.SymbolValue;
						command.GetParameter( "inactive" ).Value = s.Inactive;
						command.GetParameter( "html" ).Value = s.GetDataAsHtml( Site.Version, Site.StringDic, Site.InGameIdDict );
						command.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
		}

		public void ExportStrategy() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE StrategyOptions ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, refString TEXT, strDicName INT, strDicDesc INT, "
						+ "category INT, characters INT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE StrategySet ( id INTEGER PRIMARY KEY AUTOINCREMENT, refString TEXT, strDicName INT, strDicDesc INT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE StrategySetDefaults ( id INTEGER PRIMARY KEY AUTOINCREMENT, strategySetId INT, character INT, category INT, optionId INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO StrategyOptions ( id, gameId, refString, strDicName, strDicDesc, category, characters, html ) "
						+ "VALUES ( @id, @gameId, @refString, @strDicName, @strDicDesc, @category, @characters, @html )";
					command.AddParameter( "id" );
					command.AddParameter( "gameId" );
					command.AddParameter( "refString" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "category" );
					command.AddParameter( "characters" );
					command.AddParameter( "html" );

					foreach ( var so in Site.Strategy.StrategyOptionList ) {
						command.GetParameter( "id" ).Value = so.ID;
						command.GetParameter( "gameId" ).Value = so.InGameID;
						command.GetParameter( "refString" ).Value = so.RefString;
						command.GetParameter( "strDicName" ).Value = so.NameStringDicID;
						command.GetParameter( "strDicDesc" ).Value = so.DescStringDicID;
						command.GetParameter( "category" ).Value = so.Category;
						command.GetParameter( "characters" ).Value = so.Characters;
						command.GetParameter( "html" ).Value = so.GetDataAsHtml( Site.Version, Site.StringDic, Site.InGameIdDict );
						command.ExecuteNonQuery();
					}
				}
				using ( var commandSet = DB.CreateCommand() )
				using ( var commandDefault = DB.CreateCommand() ) {
					commandSet.CommandText = "INSERT INTO StrategySet ( id, refString, strDicName, strDicDesc, html ) "
						+ "VALUES ( @id, @refString, @strDicName, @strDicDesc, @html )";
					commandSet.AddParameter( "id" );
					commandSet.AddParameter( "refString" );
					commandSet.AddParameter( "strDicName" );
					commandSet.AddParameter( "strDicDesc" );
					commandSet.AddParameter( "html" );

					commandDefault.CommandText = "INSERT INTO StrategySetDefaults ( strategySetId, character, category, optionId ) "
						+ "VALUES ( @strategySetId, @character, @category, @optionId )";
					commandDefault.AddParameter( "strategySetId" );
					commandDefault.AddParameter( "character" );
					commandDefault.AddParameter( "category" );
					commandDefault.AddParameter( "optionId" );

					foreach ( var ss in Site.Strategy.StrategySetList ) {
						commandSet.GetParameter( "id" ).Value = ss.ID;
						commandSet.GetParameter( "refString" ).Value = ss.RefString;
						commandSet.GetParameter( "strDicName" ).Value = ss.NameStringDicID;
						commandSet.GetParameter( "strDicDesc" ).Value = ss.DescStringDicID;
						commandSet.GetParameter( "html" ).Value = ss.GetDataAsHtml( Site.Version, Site.Strategy, Site.StringDic, Site.InGameIdDict );
						commandSet.ExecuteNonQuery();

						for ( uint cat = 0; cat < ss.StrategyDefaults.GetLength( 0 ); ++cat ) {
							for ( uint ch = 0; ch < ss.StrategyDefaults.GetLength( 1 ); ++ch ) {
								commandDefault.GetParameter( "strategySetId" ).Value = ss.ID;
								commandDefault.GetParameter( "character" ).Value = ch;
								commandDefault.GetParameter( "category" ).Value = cat;
								commandDefault.GetParameter( "optionId" ).Value = ss.StrategyDefaults[cat, ch];
								commandDefault.ExecuteNonQuery();
							}
						}
					}
				}
				transaction.Commit();
			}
		}

		public void ExportRecipes() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Recipes ( id INTEGER PRIMARY KEY AUTOINCREMENT, refString TEXT, strDicName INT, strDicDesc INT, strDicEffect INT, "
						+ "charLike INT, charHate INT, charGood INT, charBad INT, hp INT, tp INT, death INT, ailment INT, statType INT, statValue INT, statTime INT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Recipes_Ingredients ( id INTEGER PRIMARY KEY AUTOINCREMENT, recipeId INT, type INT, item INT, count INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Recipes_RecipeCreation ( id INTEGER PRIMARY KEY AUTOINCREMENT, recipeId INT, character INT, createdRecipe INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() )
				using ( var commandIngredients = DB.CreateCommand() )
				using ( var commandRecipeCreation = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Recipes ( id, refString, strDicName, strDicDesc, strDicEffect, charLike, charHate, charGood, charBad, hp, tp, "
						+ "death, ailment, statType, statValue, statTime, html ) VALUES ( @id, @refString, @strDicName, @strDicDesc, @strDicEffect, @charLike, @charHate, "
						+ "@charGood, @charBad, @hp, @tp, @death, @ailment, @statType, @statValue, @statTime, @html )";
					command.AddParameter( "id" );
					command.AddParameter( "refString" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "strDicEffect" );
					command.AddParameter( "charLike" );
					command.AddParameter( "charHate" );
					command.AddParameter( "charGood" );
					command.AddParameter( "charBad" );
					command.AddParameter( "hp" );
					command.AddParameter( "tp" );
					command.AddParameter( "death" );
					command.AddParameter( "ailment" );
					command.AddParameter( "statType" );
					command.AddParameter( "statValue" );
					command.AddParameter( "statTime" );
					command.AddParameter( "html" );

					commandIngredients.CommandText = "INSERT INTO Recipes_Ingredients ( recipeId, type, item, count ) VALUES ( @recipeId, @type, @item, @count )";
					commandIngredients.AddParameter( "recipeId" );
					commandIngredients.AddParameter( "type" );
					commandIngredients.AddParameter( "item" );
					commandIngredients.AddParameter( "count" );

					commandRecipeCreation.CommandText = "INSERT INTO Recipes_RecipeCreation ( recipeId, character, createdRecipe ) VALUES ( @recipeId, @character, @createdRecipe )";
					commandRecipeCreation.AddParameter( "recipeId" );
					commandRecipeCreation.AddParameter( "character" );
					commandRecipeCreation.AddParameter( "createdRecipe" );

					foreach ( var r in Site.Recipes.RecipeList ) {
						command.GetParameter( "id" ).Value = r.ID;
						command.GetParameter( "refString" ).Value = r.RefString;
						command.GetParameter( "strDicName" ).Value = r.NameStringDicID;
						command.GetParameter( "strDicDesc" ).Value = r.DescriptionStringDicID;
						command.GetParameter( "strDicEffect" ).Value = r.EffectStringDicID;
						command.GetParameter( "charLike" ).Value = r.CharactersLike;
						command.GetParameter( "charHate" ).Value = r.CharactersDislike;
						command.GetParameter( "charGood" ).Value = r.CharactersGoodAtMaking;
						command.GetParameter( "charBad" ).Value = r.CharactersBadAtMaking;
						command.GetParameter( "hp" ).Value = r.HP;
						command.GetParameter( "tp" ).Value = r.TP;
						command.GetParameter( "death" ).Value = r.DeathHeal;
						command.GetParameter( "ailment" ).Value = r.PhysicalAilmentHeal;
						command.GetParameter( "statType" ).Value = r.StatType;
						command.GetParameter( "statValue" ).Value = r.StatValue;
						command.GetParameter( "statTime" ).Value = r.StatTime;
						command.GetParameter( "html" ).Value = r.GetDataAsHtml( Site.Version, Site.Recipes, Site.Items, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.ExecuteNonQuery();

						for ( int i = 0; i < r.IngredientGroups.Length; ++i ) {
							if ( r.IngredientGroups[i] <= 0 ) { continue; }
							commandIngredients.GetParameter( "recipeId" ).Value = r.ID;
							commandIngredients.GetParameter( "type" ).Value = 1;
							commandIngredients.GetParameter( "item" ).Value = r.IngredientGroups[i];
							commandIngredients.GetParameter( "count" ).Value = r.IngredientGroupCount[i];
							commandIngredients.ExecuteNonQuery();
						}
						for ( int i = 0; i < r.Ingredients.Length; ++i ) {
							if ( r.Ingredients[i] <= 0 ) { continue; }
							commandIngredients.GetParameter( "recipeId" ).Value = r.ID;
							commandIngredients.GetParameter( "type" ).Value = 2;
							commandIngredients.GetParameter( "item" ).Value = r.Ingredients[i];
							commandIngredients.GetParameter( "count" ).Value = r.IngredientCount[i];
							commandIngredients.ExecuteNonQuery();
						}
						for ( int i = 0; i < r.RecipeCreationCharacter.Length; ++i ) {
							if ( r.RecipeCreationCharacter[i] <= 0 ) { continue; }
							commandRecipeCreation.GetParameter( "recipeId" ).Value = r.ID;
							commandRecipeCreation.GetParameter( "character" ).Value = r.RecipeCreationCharacter[i];
							commandRecipeCreation.GetParameter( "createdRecipe" ).Value = r.RecipeCreationRecipe[i];
							commandRecipeCreation.ExecuteNonQuery();
						}
					}
				}
				transaction.Commit();
			}
		}

		public void ExportShops() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Shops ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, strDicName INT, changesToShop INT, onTrigger INT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Shops_Items ( id INTEGER PRIMARY KEY AUTOINCREMENT, shopId INT, itemId INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Shops ( gameId, strDicName, changesToShop, onTrigger, html ) VALUES ( @gameId, @strDicName, @changesToShop, @onTrigger, @html )";
					command.AddParameter( "gameId" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "changesToShop" );
					command.AddParameter( "onTrigger" );
					command.AddParameter( "html" );

					foreach ( var s in Site.Shops.ShopDefinitions ) {
						command.GetParameter( "gameId" ).Value = s.InGameID;
						command.GetParameter( "strDicName" ).Value = s.StringDicID;
						command.GetParameter( "changesToShop" ).Value = s.ChangeToShop;
						command.GetParameter( "onTrigger" ).Value = s.OnTrigger;
						command.GetParameter( "html" ).Value = s.GetDataAsHtml( Site.Version, Site.Items, Site.Shops, Site.InGameIdDict, phpLinks: true );
						command.ExecuteNonQuery();
					}
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Shops_Items ( shopId, itemId ) VALUES ( @shopId, @itemId )";
					command.AddParameter( "shopId" );
					command.AddParameter( "itemId" );

					foreach ( var s in Site.Shops.ShopItems ) {
						command.GetParameter( "shopId" ).Value = s.ShopID;
						command.GetParameter( "itemId" ).Value = s.ItemID;
						command.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
		}

		public void ExportTitles() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Titles ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, strDicName INT, strDicDesc INT, character INT, "
						+ "points INT, model TEXT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Titles ( gameId, strDicName, strDicDesc, character, points, model, html ) "
						+ "VALUES ( @gameId, @strDicName, @strDicDesc, @character, @points, @model, @html )";
					command.AddParameter( "gameId" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "character" );
					command.AddParameter( "points" );
					command.AddParameter( "model" );
					command.AddParameter( "html" );

					foreach ( var t in Site.Titles.TitleList ) {
						command.GetParameter( "gameId" ).Value = t.ID;
						command.GetParameter( "strDicName" ).Value = t.NameStringDicID;
						command.GetParameter( "strDicDesc" ).Value = t.DescStringDicID;
						command.GetParameter( "character" ).Value = t.Character;
						command.GetParameter( "points" ).Value = t.BunnyGuildPointsMaybe;
						command.GetParameter( "model" ).Value = t.CostumeString;
						command.GetParameter( "html" ).Value = t.GetDataAsHtml( Site.Version, Site.StringDic, Site.InGameIdDict );
						command.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
		}

		public void ExportSynopsis() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Synopsis ( id INTEGER PRIMARY KEY AUTOINCREMENT, strDicName INT, strDicDesc INT, storyMin INT, storyMax INT, image TEXT, "
						+ "refString TEXT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Synopsis ( id, strDicName, strDicDesc, storyMin, storyMax, image, refString, html ) "
						+ "VALUES ( @id, @strDicName, @strDicDesc, @storyMin, @storyMax, @image, @refString, @html )";
					command.AddParameter( "id" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "storyMin" );
					command.AddParameter( "storyMax" );
					command.AddParameter( "image" );
					command.AddParameter( "refString" );
					command.AddParameter( "html" );

					foreach ( var s in Site.Synopsis.SynopsisList ) {
						command.GetParameter( "id" ).Value = s.ID;
						command.GetParameter( "strDicName" ).Value = s.NameStringDicId;
						command.GetParameter( "strDicDesc" ).Value = s.TextStringDicId;
						command.GetParameter( "storyMin" ).Value = s.StoryIdMin;
						command.GetParameter( "storyMax" ).Value = s.StoryIdMax;
						command.GetParameter( "image" ).Value = s.RefString1;
						command.GetParameter( "refString" ).Value = s.RefString2;
						command.GetParameter( "html" ).Value = s.GetDataAsHtml( Site.Version, Site.StringDic, Site.InGameIdDict );
						command.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
		}

		public void ExportBattleBook() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE BattleBook ( id INTEGER PRIMARY KEY AUTOINCREMENT, strDicName INT, strDicDesc INT, unlock INT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO BattleBook ( strDicName, strDicDesc, unlock, html ) "
						+ "VALUES ( @strDicName, @strDicDesc, @unlock, @html )";
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "unlock" );
					command.AddParameter( "html" );

					for ( int i = 0; i < Site.BattleBook.BattleBookEntryList.Count; ++i ) {
						var b = Site.BattleBook.BattleBookEntryList[i];
						if ( b.NameStringDicId == 0xFFFFFFFFu || b.NameStringDicId == 0 ) { continue; }
						command.GetParameter( "strDicName" ).Value = b.NameStringDicId;
						command.GetParameter( "strDicDesc" ).Value = b.TextStringDicId;
						command.GetParameter( "unlock" ).Value = b.UnlockReferenceMaybe;
						command.GetParameter( "html" ).Value = b.GetDataAsHtml( Site.Version, Site.StringDic, Site.InGameIdDict );
						command.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
		}

		public void ExportMonsters() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Enemies ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, strDicName INT, refString TEXT, icon INT, category INT, "
						+ "level INT, hp INT, tp INT, pAtk INT, pDef INT, mAtk INT, mDef INT, agl INT, attrFire INT, attrEarth INT, attrWind INT, attrWater INT, attrLight INT, "
						+ "attrDark INT, attrPhys INT, exp INT, lp INT, gald INT, fatalBlueResist FLOAT, fatalRedResist FLOAT, fatalGreenResist FLOAT, inMonsterBook INT, "
						+ "location INT, locationWeather INT, stealItem INT, stealChance INT, killableWithFatal INT, secretMissionDrop INT, secretMissionDropChance INT, "
						+ "fatalExpType INT, fatalExpModifier INT, fatalLpType INT, fatalLpModifier INT, fatalDropType INT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Enemies_Drops ( id INTEGER PRIMARY KEY AUTOINCREMENT, enemyId INT, itemId INT, chance INT, fatalModifier INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX Enemies_Category_Index ON Enemies ( category )";
					command.ExecuteNonQuery();
				}

				using ( var command = DB.CreateCommand() )
				using ( var commandDrop = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Enemies ( id, gameId, strDicName, refString, icon, category, level, hp, tp, pAtk, pDef, mAtk, mDef, agl, attrFire, "
						+ "attrEarth, attrWind, attrWater, attrLight, attrDark, attrPhys, exp, lp, gald, fatalBlueResist, fatalRedResist, fatalGreenResist, inMonsterBook, "
						+ "location, locationWeather, stealItem, stealChance, killableWithFatal, secretMissionDrop, secretMissionDropChance, fatalExpType, fatalExpModifier, "
						+ "fatalLpType, fatalLpModifier, fatalDropType, html ) VALUES ( @id, @gameId, @strDicName, @refString, @icon, @category, @level, @hp, @tp, @pAtk, "
						+ "@pDef, @mAtk, @mDef, @agl, @attrFire, @attrEarth, @attrWind, @attrWater, @attrLight, @attrDark, @attrPhys, @exp, @lp, @gald, @fatalBlueResist, "
						+ "@fatalRedResist, @fatalGreenResist, @inMonsterBook, @location, @locationWeather, @stealItem, @stealChance, @killableWithFatal, @secretMissionDrop, "
						+ "@secretMissionDropChance, @fatalExpType, @fatalExpModifier, @fatalLpType, @fatalLpModifier, @fatalDropType, @html )";
					command.AddParameter( "id" );
					command.AddParameter( "gameId" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "refString" );
					command.AddParameter( "icon" );
					command.AddParameter( "category" );
					command.AddParameter( "level" );
					command.AddParameter( "hp" );
					command.AddParameter( "tp" );
					command.AddParameter( "pAtk" );
					command.AddParameter( "pDef" );
					command.AddParameter( "mAtk" );
					command.AddParameter( "mDef" );
					command.AddParameter( "agl" );
					command.AddParameter( "attrFire" );
					command.AddParameter( "attrEarth" );
					command.AddParameter( "attrWind" );
					command.AddParameter( "attrWater" );
					command.AddParameter( "attrLight" );
					command.AddParameter( "attrDark" );
					command.AddParameter( "attrPhys" );
					command.AddParameter( "exp" );
					command.AddParameter( "lp" );
					command.AddParameter( "gald" );
					command.AddParameter( "fatalBlueResist" );
					command.AddParameter( "fatalRedResist" );
					command.AddParameter( "fatalGreenResist" );
					command.AddParameter( "inMonsterBook" );
					command.AddParameter( "location" );
					command.AddParameter( "locationWeather" );
					command.AddParameter( "stealItem" );
					command.AddParameter( "stealChance" );
					command.AddParameter( "killableWithFatal" );
					command.AddParameter( "secretMissionDrop" );
					command.AddParameter( "secretMissionDropChance" );
					command.AddParameter( "fatalExpType" );
					command.AddParameter( "fatalExpModifier" );
					command.AddParameter( "fatalLpType" );
					command.AddParameter( "fatalLpModifier" );
					command.AddParameter( "fatalDropType" );
					command.AddParameter( "html" );

					commandDrop.CommandText = "INSERT INTO Enemies_Drops ( enemyId, itemId, chance, fatalModifier ) VALUES ( @enemyId, @itemId, @chance, @fatalModifier )";
					commandDrop.AddParameter( "enemyId" );
					commandDrop.AddParameter( "itemId" );
					commandDrop.AddParameter( "chance" );
					commandDrop.AddParameter( "fatalModifier" );

					foreach ( var e in Site.Enemies.EnemyList ) {
						command.GetParameter( "id" ).Value = e.ID;
						command.GetParameter( "gameId" ).Value = e.InGameID;
						command.GetParameter( "strDicName" ).Value = e.NameStringDicID;
						command.GetParameter( "refString" ).Value = e.RefString;
						command.GetParameter( "icon" ).Value = e.IconID;
						command.GetParameter( "category" ).Value = e.Category;
						command.GetParameter( "level" ).Value = e.Level;
						command.GetParameter( "hp" ).Value = e.HP;
						command.GetParameter( "tp" ).Value = e.TP;
						command.GetParameter( "pAtk" ).Value = e.PATK;
						command.GetParameter( "pDef" ).Value = e.PDEF;
						command.GetParameter( "mAtk" ).Value = e.MATK;
						command.GetParameter( "mDef" ).Value = e.MDEF;
						command.GetParameter( "agl" ).Value = e.AGL;
						command.GetParameter( "attrFire" ).Value = e.Attributes[(int)T8BTEMST.Element.Fire];
						command.GetParameter( "attrEarth" ).Value = e.Attributes[(int)T8BTEMST.Element.Earth];
						command.GetParameter( "attrWind" ).Value = e.Attributes[(int)T8BTEMST.Element.Wind];
						command.GetParameter( "attrWater" ).Value = e.Attributes[(int)T8BTEMST.Element.Water];
						command.GetParameter( "attrLight" ).Value = e.Attributes[(int)T8BTEMST.Element.Light];
						command.GetParameter( "attrDark" ).Value = e.Attributes[(int)T8BTEMST.Element.Darkness];
						command.GetParameter( "attrPhys" ).Value = e.Attributes[(int)T8BTEMST.Element.Physical];
						command.GetParameter( "exp" ).Value = e.EXP;
						command.GetParameter( "lp" ).Value = e.LP;
						command.GetParameter( "gald" ).Value = e.Gald;
						command.GetParameter( "fatalBlueResist" ).Value = e.FatalBlue;
						command.GetParameter( "fatalRedResist" ).Value = e.FatalRed;
						command.GetParameter( "fatalGreenResist" ).Value = e.FatalGreen;
						command.GetParameter( "inMonsterBook" ).Value = e.InMonsterBook;
						command.GetParameter( "location" ).Value = e.Location;
						command.GetParameter( "locationWeather" ).Value = e.LocationWeather;
						command.GetParameter( "stealItem" ).Value = e.StealItem;
						command.GetParameter( "stealChance" ).Value = e.StealChance;
						command.GetParameter( "killableWithFatal" ).Value = e.KillableWithFS;
						command.GetParameter( "secretMissionDrop" ).Value = e.SecretMissionDrop;
						command.GetParameter( "secretMissionDropChance" ).Value = e.SecretMissionDropChance;
						command.GetParameter( "fatalExpType" ).Value = e.FatalTypeExp;
						command.GetParameter( "fatalExpModifier" ).Value = e.EXPModifier;
						command.GetParameter( "fatalLpType" ).Value = e.FatalTypeLP;
						command.GetParameter( "fatalLpModifier" ).Value = e.LPModifier;
						command.GetParameter( "fatalDropType" ).Value = e.FatalTypeDrop;
						command.GetParameter( "html" ).Value = e.GetDataAsHtml( Site.Version, Site.Items, Site.Locations, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.ExecuteNonQuery();

						for ( int i = 0; i < e.DropItems.Length; ++i ) {
							if ( e.DropItems[i] <= 0 ) { continue; }
							commandDrop.GetParameter( "enemyId" ).Value = e.ID;
							commandDrop.GetParameter( "itemId" ).Value = e.DropItems[i];
							commandDrop.GetParameter( "chance" ).Value = e.DropChances[i];
							commandDrop.GetParameter( "fatalModifier" ).Value = e.DropModifier[i];
							commandDrop.ExecuteNonQuery();
						}
					}
				}
				transaction.Commit();
			}
		}

		public void ExportMonsterGroups() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE EnemyGroups ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, strDicName INT, refString TEXT, flag INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE EnemyGroups_Enemies ( id INTEGER PRIMARY KEY AUTOINCREMENT, groupId INT, slot INT, enemyId INT, unknown1 FLOAT, "
						+ "posX FLOAT, posY FLOAT, scale FLOAT, unknown2 INT )";
					command.ExecuteNonQuery();
				}

				using ( var command = DB.CreateCommand() )
				using ( var commandEnemy = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO EnemyGroups ( id, gameId, strDicName, refString, flag ) VALUES ( @id, @gameId, @strDicName, @refString, @flag )";
					command.AddParameter( "id" );
					command.AddParameter( "gameId" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "refString" );
					command.AddParameter( "flag" );

					commandEnemy.CommandText = "INSERT INTO EnemyGroups_Enemies ( groupId, slot, enemyId, unknown1, posX, posY, scale, unknown2 ) VALUES "
						+ "( @groupId, @slot, @enemyId, @unknown1, @posX, @posY, @scale, @unknown2 )";
					commandEnemy.AddParameter( "groupId" );
					commandEnemy.AddParameter( "slot" );
					commandEnemy.AddParameter( "enemyId" );
					commandEnemy.AddParameter( "unknown1" );
					commandEnemy.AddParameter( "posX" );
					commandEnemy.AddParameter( "posY" );
					commandEnemy.AddParameter( "scale" );
					commandEnemy.AddParameter( "unknown2" );

					foreach ( var e in Site.EnemyGroups.EnemyGroupList ) {
						command.GetParameter( "id" ).Value = e.ID;
						command.GetParameter( "gameId" ).Value = e.InGameID;
						command.GetParameter( "strDicName" ).Value = e.StringDicID;
						command.GetParameter( "refString" ).Value = e.RefString;
						command.GetParameter( "flag" ).Value = e.SomeFlag;
						command.ExecuteNonQuery();

						for ( int i = 0; i < e.EnemyIDs.Length; ++i ) {
							if ( e.EnemyIDs[i] < 0 ) { continue; }
							commandEnemy.GetParameter( "groupId" ).Value = e.ID;
							commandEnemy.GetParameter( "slot" ).Value = i;
							commandEnemy.GetParameter( "enemyId" ).Value = e.EnemyIDs[i];
							commandEnemy.GetParameter( "unknown1" ).Value = e.UnknownFloats[i];
							commandEnemy.GetParameter( "posX" ).Value = e.PosX[i];
							commandEnemy.GetParameter( "posY" ).Value = e.PosY[i];
							commandEnemy.GetParameter( "scale" ).Value = e.Scale[i];
							commandEnemy.GetParameter( "unknown2" ).Value = e.UnknownInts[i];
							commandEnemy.ExecuteNonQuery();
						}
					}
				}
				transaction.Commit();
			}
		}

		public void ExportEncounterGroups() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Encounters ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, strDicName INT, refString TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Encounters_EnemyGroups ( id INTEGER PRIMARY KEY AUTOINCREMENT, encounterId INT, groupId INT )";
					command.ExecuteNonQuery();
				}

				using ( var command = DB.CreateCommand() )
				using ( var commandGroup = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Encounters ( id, gameId, strDicName, refString ) VALUES ( @id, @gameId, @strDicName, @refString )";
					command.AddParameter( "id" );
					command.AddParameter( "gameId" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "refString" );

					commandGroup.CommandText = "INSERT INTO Encounters_EnemyGroups ( encounterId, groupId ) VALUES ( @encounterId, @groupId )";
					commandGroup.AddParameter( "encounterId" );
					commandGroup.AddParameter( "groupId" );

					foreach ( var e in Site.EncounterGroups.EncounterGroupList ) {
						command.GetParameter( "id" ).Value = e.ID;
						command.GetParameter( "gameId" ).Value = e.InGameID;
						command.GetParameter( "strDicName" ).Value = e.StringDicID;
						command.GetParameter( "refString" ).Value = e.RefString;
						command.ExecuteNonQuery();

						for ( int i = 0; i < e.EnemyGroupIDs.Length; ++i ) {
							if ( e.EnemyGroupIDs[i] == 0xFFFFFFFFu ) { continue; }
							commandGroup.GetParameter( "encounterId" ).Value = e.ID;
							commandGroup.GetParameter( "groupId" ).Value = e.EnemyGroupIDs[i];
							commandGroup.ExecuteNonQuery();
						}
					}
				}
				transaction.Commit();
			}
		}

		public void ExportItems() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Items ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, image TEXT, equipBy INT, strDicName INT, strDicDesc INT, "
						+ "icon INT, usableInBattle INT, inCollectorsBook INT, category INT, hpHeal INT, tpHeal INT, ailmentPhys INT, ailmentMag INT, permaPAtk INT, permaPDef INT, "
						+ "permaMAtk INT, permaMDef INT, permaAgl INT, permaHp INT, permaTp INT, equipPAtk INT, equipMAtk INT, equipPDef INT, equipMDef INT, equipAgl INT, "
						+ "equipLuck INT, attrFire INT, attrWater INT, attrWind INT, attrEarth INT, attrLight INT, attrDark INT, price INT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Items_SynthInfo ( id INTEGER PRIMARY KEY AUTOINCREMENT, itemId INT, level INT, price INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Items_SynthItems ( id INTEGER PRIMARY KEY AUTOINCREMENT, synthInfoId INT, itemId INT, count INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Items_Recipes ( id INTEGER PRIMARY KEY AUTOINCREMENT, itemId INT, recipeId INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Items_Skills ( id INTEGER PRIMARY KEY AUTOINCREMENT, itemId INT, skillId INT, learnRate INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Items_Shops ( id INTEGER PRIMARY KEY AUTOINCREMENT, itemId INT, shopId INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Items_Drops ( id INTEGER PRIMARY KEY AUTOINCREMENT, itemId INT, enemyId INT, chance INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Items_Steals ( id INTEGER PRIMARY KEY AUTOINCREMENT, itemId INT, enemyId INT, chance INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX Items_Category_Index ON Items ( category )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX Items_Icon_Index ON Items ( icon )";
					command.ExecuteNonQuery();
				}

				using ( var command = DB.CreateCommand() )
				using ( var commandSynthInfo = DB.CreateCommand() )
				using ( var commandSynthItem = DB.CreateCommand() )
				using ( var commandRecipe = DB.CreateCommand() )
				using ( var commandSkill = DB.CreateCommand() )
				using ( var commandShop = DB.CreateCommand() )
				using ( var commandDrop = DB.CreateCommand() )
				using ( var commandSteal = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Items ( gameId, image, equipBy, strDicName, strDicDesc, icon, usableInBattle, inCollectorsBook, category, hpHeal, "
						+ "tpHeal, ailmentPhys, ailmentMag, permaPAtk, permaPDef, permaMAtk, permaMDef, permaAgl, permaHp, permaTp, equipPAtk, equipMAtk, equipPDef, "
						+ "equipMDef, equipAgl, equipLuck, attrFire, attrWater, attrWind, attrEarth, attrLight, attrDark, price, html ) VALUES ( @gameId, @image, @equipBy, "
						+ "@strDicName, @strDicDesc, @icon, @usableInBattle, @inCollectorsBook, @category, @hpHeal, @tpHeal, @ailmentPhys, @ailmentMag, @permaPAtk, "
						+ "@permaPDef, @permaMAtk, @permaMDef, @permaAgl, @permaHp, @permaTp, @equipPAtk, @equipMAtk, @equipPDef, @equipMDef, @equipAgl, @equipLuck, "
						+ "@attrFire, @attrWater, @attrWind, @attrEarth, @attrLight, @attrDark, @price, @html )";
					command.AddParameter( "gameId" );
					command.AddParameter( "image" );
					command.AddParameter( "equipBy" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "icon" );
					command.AddParameter( "usableInBattle" );
					command.AddParameter( "inCollectorsBook" );
					command.AddParameter( "category" );
					command.AddParameter( "hpHeal" );
					command.AddParameter( "tpHeal" );
					command.AddParameter( "ailmentPhys" );
					command.AddParameter( "ailmentMag" );
					command.AddParameter( "permaPAtk" );
					command.AddParameter( "permaPDef" );
					command.AddParameter( "permaMAtk" );
					command.AddParameter( "permaMDef" );
					command.AddParameter( "permaAgl" );
					command.AddParameter( "permaHp" );
					command.AddParameter( "permaTp" );
					command.AddParameter( "equipPAtk" );
					command.AddParameter( "equipMAtk" );
					command.AddParameter( "equipPDef" );
					command.AddParameter( "equipMDef" );
					command.AddParameter( "equipAgl" );
					command.AddParameter( "equipLuck" );
					command.AddParameter( "attrFire" );
					command.AddParameter( "attrWater" );
					command.AddParameter( "attrWind" );
					command.AddParameter( "attrEarth" );
					command.AddParameter( "attrLight" );
					command.AddParameter( "attrDark" );
					command.AddParameter( "price" );
					command.AddParameter( "html" );

					commandSynthInfo.CommandText = "INSERT INTO Items_SynthInfo ( itemId, level, price ) VALUES ( @itemId, @level, @price )";
					commandSynthInfo.AddParameter( "itemId" );
					commandSynthInfo.AddParameter( "level" );
					commandSynthInfo.AddParameter( "price" );

					commandSynthItem.CommandText = "INSERT INTO Items_SynthItems ( synthInfoId, itemId, count ) VALUES ( @synthInfoId, @itemId, @count )";
					commandSynthItem.AddParameter( "synthInfoId" );
					commandSynthItem.AddParameter( "itemId" );
					commandSynthItem.AddParameter( "count" );

					commandRecipe.CommandText = "INSERT INTO Items_Recipes ( itemId, recipeId ) VALUES ( @itemId, @recipeId )";
					commandRecipe.AddParameter( "itemId" );
					commandRecipe.AddParameter( "recipeId" );

					commandSkill.CommandText = "INSERT INTO Items_Skills ( itemId, skillId, learnRate ) VALUES ( @itemId, @skillId, @learnRate )";
					commandSkill.AddParameter( "itemId" );
					commandSkill.AddParameter( "skillId" );
					commandSkill.AddParameter( "learnRate" );

					commandShop.CommandText = "INSERT INTO Items_Shops ( itemId, shopId ) VALUES ( @itemId, @shopId )";
					commandShop.AddParameter( "itemId" );
					commandShop.AddParameter( "shopId" );

					commandDrop.CommandText = "INSERT INTO Items_Drops ( itemId, enemyId, chance ) VALUES ( @itemId, @enemyId, @chance )";
					commandDrop.AddParameter( "itemId" );
					commandDrop.AddParameter( "enemyId" );
					commandDrop.AddParameter( "chance" );

					commandSteal.CommandText = "INSERT INTO Items_Steals ( itemId, enemyId, chance ) VALUES ( @itemId, @enemyId, @chance )";
					commandSteal.AddParameter( "itemId" );
					commandSteal.AddParameter( "enemyId" );
					commandSteal.AddParameter( "chance" );

					foreach ( var item in Site.Items.items ) {
						command.GetParameter( "gameId" ).Value = item.Data[(int)ItemDat.ItemData.ID];
						command.GetParameter( "image" ).Value = item.ItemString.TrimNull();
						command.GetParameter( "equipBy" ).Value = item.Data[(int)ItemDat.ItemData.EquippableByBitfield];
						command.GetParameter( "strDicName" ).Value = item.Data[(int)ItemDat.ItemData.NamePointer];
						command.GetParameter( "strDicDesc" ).Value = item.Data[(int)ItemDat.ItemData.DescriptionPointer];
						command.GetParameter( "icon" ).Value = item.Data[(int)ItemDat.ItemData.Icon];
						command.GetParameter( "usableInBattle" ).Value = item.Data[(int)ItemDat.ItemData.UsableInBattle];
						command.GetParameter( "inCollectorsBook" ).Value = item.Data[(int)ItemDat.ItemData.InCollectorsBook];
						command.GetParameter( "price" ).Value = item.Data[(int)ItemDat.ItemData.ShopPrice];
						command.GetParameter( "html" ).Value = ItemDat.ItemDat.GetItemDataAsHtml( Site.Version, Site.Items, item, Site.Skills, Site.Enemies, Site.Recipes, Site.Locations, Site.StringDic, Site.InGameIdDict, phpLinks: true );

						uint category = item.Data[(int)ItemDat.ItemData.Category];
						bool equipType = category >= 3 && category <= 7;
						command.GetParameter( "category" ).Value = category;

						if ( equipType ) {
							command.GetParameter( "equipPAtk" ).Value = (int)item.Data[(int)ItemDat.ItemData.PATK];
							command.GetParameter( "equipMAtk" ).Value = (int)item.Data[(int)ItemDat.ItemData.MATK];
							command.GetParameter( "equipPDef" ).Value = (int)item.Data[(int)ItemDat.ItemData.PDEF];
							command.GetParameter( "equipMDef" ).Value = (int)item.Data[(int)ItemDat.ItemData.MDEF_or_HPHealPercent];
							command.GetParameter( "equipAgl" ).Value = (int)item.Data[(int)ItemDat.ItemData._AGL_Again];
							command.GetParameter( "equipLuck" ).Value = (int)item.Data[(int)ItemDat.ItemData._LUCK];
							command.GetParameter( "attrFire" ).Value = (int)item.Data[(int)ItemDat.ItemData.AttrFire];
							command.GetParameter( "attrWater" ).Value = (int)item.Data[(int)ItemDat.ItemData.AttrWater];
							command.GetParameter( "attrWind" ).Value = (int)item.Data[(int)ItemDat.ItemData.AttrWind];
							command.GetParameter( "attrEarth" ).Value = (int)item.Data[(int)ItemDat.ItemData.AttrEarth];
							command.GetParameter( "attrLight" ).Value = (int)item.Data[(int)ItemDat.ItemData.AttrLight];
							command.GetParameter( "attrDark" ).Value = (int)item.Data[(int)ItemDat.ItemData.AttrDark];

							command.GetParameter( "hpHeal" ).Value = 0;
							command.GetParameter( "tpHeal" ).Value = 0;
							command.GetParameter( "ailmentPhys" ).Value = 0;
							command.GetParameter( "ailmentMag" ).Value = 0;
							command.GetParameter( "permaPAtk" ).Value = 0;
							command.GetParameter( "permaPDef" ).Value = 0;
							command.GetParameter( "permaMAtk" ).Value = 0;
							command.GetParameter( "permaMDef" ).Value = 0;
							command.GetParameter( "permaAgl" ).Value = 0;
							command.GetParameter( "permaHp" ).Value = 0;
							command.GetParameter( "permaTp" ).Value = 0;
						} else {
							command.GetParameter( "hpHeal" ).Value = item.Data[(int)ItemDat.ItemData.MDEF_or_HPHealPercent];
							command.GetParameter( "tpHeal" ).Value = item.Data[(int)ItemDat.ItemData.AGL_TPHealPercent];
							command.GetParameter( "ailmentPhys" ).Value = item.Data[(int)ItemDat.ItemData._LUCK];
							command.GetParameter( "ailmentMag" ).Value = item.Data[(int)ItemDat.ItemData._AGL_Again];
							command.GetParameter( "permaPAtk" ).Value = item.Data[(int)ItemDat.ItemData.PermanentPAtkIncrease];
							command.GetParameter( "permaPDef" ).Value = item.Data[(int)ItemDat.ItemData.PermanentPDefIncrease];
							command.GetParameter( "permaMAtk" ).Value = item.Data[(int)ItemDat.ItemData.AttrFire];
							command.GetParameter( "permaMDef" ).Value = item.Data[(int)ItemDat.ItemData.AttrWater];
							command.GetParameter( "permaAgl" ).Value = item.Data[(int)ItemDat.ItemData.AttrWind];
							command.GetParameter( "permaHp" ).Value = item.Data[(int)ItemDat.ItemData.Skill1];
							command.GetParameter( "permaTp" ).Value = item.Data[(int)ItemDat.ItemData.Skill1Metadata];

							command.GetParameter( "equipPAtk" ).Value = 0;
							command.GetParameter( "equipMAtk" ).Value = 0;
							command.GetParameter( "equipPDef" ).Value = 0;
							command.GetParameter( "equipMDef" ).Value = 0;
							command.GetParameter( "equipAgl" ).Value = 0;
							command.GetParameter( "equipLuck" ).Value = 0;
							command.GetParameter( "attrFire" ).Value = 0;
							command.GetParameter( "attrWater" ).Value = 0;
							command.GetParameter( "attrWind" ).Value = 0;
							command.GetParameter( "attrEarth" ).Value = 0;
							command.GetParameter( "attrLight" ).Value = 0;
							command.GetParameter( "attrDark" ).Value = 0;
						}

						command.ExecuteNonQuery();

						long itemId = GetLastInsertedId();

						uint synthCount = item.Data[(int)ItemDat.ItemData.SynthRecipeCount];
						for ( int j = 0; j < synthCount; ++j ) {
							uint synthItemCount = item.Data[(int)ItemDat.ItemData.Synth1ItemSlotCount + j * 16];
							commandSynthInfo.GetParameter( "itemId" ).Value = itemId;
							commandSynthInfo.GetParameter( "level" ).Value = item.Data[(int)ItemDat.ItemData._Synth1Level + j * 16];
							commandSynthInfo.GetParameter( "price" ).Value = item.Data[(int)ItemDat.ItemData.Synth1Price + j * 16];
							commandSynthInfo.ExecuteNonQuery();

							long synthInfoId = GetLastInsertedId();
							for ( int i = 0; i < synthItemCount; ++i ) {
								commandSynthItem.GetParameter( "synthInfoId" ).Value = synthInfoId;
								commandSynthItem.GetParameter( "itemId" ).Value = item.Data[(int)ItemDat.ItemData.Synth1Item1Type + i * 2 + j * 16];
								commandSynthItem.GetParameter( "count" ).Value = item.Data[(int)ItemDat.ItemData.Synth1Item1Count + i * 2 + j * 16];
								commandSynthItem.ExecuteNonQuery();
							}
						}

						if ( !equipType ) {
							for ( int i = 0; i < 8; ++i ) {
								int recipeId = (int)item.Data[(int)ItemDat.ItemData.UsedInRecipe1 + i];
								if ( recipeId != 0 ) {
									commandRecipe.GetParameter( "itemId" ).Value = itemId;
									commandRecipe.GetParameter( "recipeId" ).Value = recipeId;
									commandRecipe.ExecuteNonQuery();
								}
							}
						}

						if ( equipType ) {
							for ( int i = 0; i < 3; ++i ) {
								uint skillId = item.Data[(int)ItemDat.ItemData.Skill1 + i * 2];
								if ( skillId != 0 ) {
									commandSkill.GetParameter( "itemId" ).Value = itemId;
									commandSkill.GetParameter( "skillId" ).Value = skillId;
									commandSkill.GetParameter( "learnRate" ).Value = item.Data[(int)ItemDat.ItemData.Skill1Metadata + i * 2];
									commandSkill.ExecuteNonQuery();
								}
							}
						}

						for ( int i = 0; i < 3; ++i ) {
							if ( item.Data[(int)ItemDat.ItemData.BuyableIn1 + i] > 0 ) {
								commandShop.GetParameter( "itemId" ).Value = itemId;
								commandShop.GetParameter( "shopId" ).Value = item.Data[(int)ItemDat.ItemData.BuyableIn1 + i];
								commandShop.ExecuteNonQuery();
							}
						}

						for ( int i = 0; i < 16; ++i ) {
							uint enemyId = item.Data[(int)ItemDat.ItemData.Drop1Enemy + i];
							if ( enemyId != 0 ) {
								commandDrop.GetParameter( "itemId" ).Value = itemId;
								commandDrop.GetParameter( "enemyId" ).Value = enemyId;
								commandDrop.GetParameter( "chance" ).Value = item.Data[(int)ItemDat.ItemData.Drop1Chance + i];
								commandDrop.ExecuteNonQuery();
							}
						}

						for ( int i = 0; i < 16; ++i ) {
							uint enemyId = item.Data[(int)ItemDat.ItemData.Steal1Enemy + i];
							if ( enemyId != 0 ) {
								commandSteal.GetParameter( "itemId" ).Value = itemId;
								commandSteal.GetParameter( "enemyId" ).Value = enemyId;
								commandSteal.GetParameter( "chance" ).Value = item.Data[(int)ItemDat.ItemData.Steal1Chance + i];
								commandSteal.ExecuteNonQuery();
							}
						}
					}
				}
				transaction.Commit();
			}
		}

		public void ExportWorldMap() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Locations ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, strDicName INT, category INT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Locations_State ( id INTEGER PRIMARY KEY AUTOINCREMENT, locationId INT, strDicName INT, strDicDesc INT, refString TEXT, trigger INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Locations_Shops ( id INTEGER PRIMARY KEY AUTOINCREMENT, locationId INT, shopId INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Locations_Encounters ( id INTEGER PRIMARY KEY AUTOINCREMENT, locationId INT, encounterId INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() )
				using ( var commandState = DB.CreateCommand() )
				using ( var commandShop = DB.CreateCommand() )
				using ( var commandEncounter = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Locations ( gameId, strDicName, category, html ) VALUES ( @gameId, @strDicName, @category, @html )";
					command.AddParameter( "gameId" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "category" );
					command.AddParameter( "html" );

					commandState.CommandText = "INSERT INTO Locations_State ( locationId, strDicName, strDicDesc, refString, trigger ) "
						+ "VALUES ( @locationId, @strDicName, @strDicDesc, @refString, @trigger )";
					commandState.AddParameter( "locationId" );
					commandState.AddParameter( "strDicName" );
					commandState.AddParameter( "strDicDesc" );
					commandState.AddParameter( "refString" );
					commandState.AddParameter( "trigger" );

					commandShop.CommandText = "INSERT INTO Locations_Shops ( locationId, shopId ) VALUES ( @locationId, @shopId )";
					commandShop.AddParameter( "locationId" );
					commandShop.AddParameter( "shopId" );

					commandEncounter.CommandText = "INSERT INTO Locations_Encounters ( locationId, encounterId ) VALUES ( @locationId, @encounterId )";
					commandEncounter.AddParameter( "locationId" );
					commandEncounter.AddParameter( "encounterId" );

					foreach ( var s in Site.Locations.LocationList ) {
						command.GetParameter( "gameId" ).Value = s.LocationID;
						command.GetParameter( "strDicName" ).Value = s.DefaultStringDicID;
						command.GetParameter( "category" ).Value = s.Category;
						command.GetParameter( "html" ).Value = s.GetDataAsHtml( Site.Version, Site.StringDic, Site.InGameIdDict, Site.EncounterGroups, Site.EnemyGroups, Site.Enemies, Site.Shops, phpLinks: true );
						command.ExecuteNonQuery();

						long insertedId = GetLastInsertedId();

						for ( int i = 0; i < s.NameStringDicIDs.Length; ++i ) {
							commandState.GetParameter( "locationId" ).Value = insertedId;
							commandState.GetParameter( "strDicName" ).Value = s.NameStringDicIDs[i];
							commandState.GetParameter( "strDicDesc" ).Value = s.DescStringDicIDs[i];
							commandState.GetParameter( "refString" ).Value = s.RefStrings[i];
							commandState.GetParameter( "trigger" ).Value = s.ChangeEventTriggers[i];
							commandState.ExecuteNonQuery();
						}

						foreach ( uint v in s.ShopsOrEnemyGroups ) {
							if ( v <= 0 ) { continue; }
							if ( s.Category == 1 ) {
								commandShop.GetParameter( "locationId" ).Value = insertedId;
								commandShop.GetParameter( "shopId" ).Value = v;
								commandShop.ExecuteNonQuery();
							} else {
								commandEncounter.GetParameter( "locationId" ).Value = insertedId;
								commandEncounter.GetParameter( "encounterId" ).Value = v;
								commandEncounter.ExecuteNonQuery();
							}
						}
					}
				}
				transaction.Commit();
			}
		}

		public void ExportRecords() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Records ( id INTEGER PRIMARY KEY AUTOINCREMENT, strDicId INT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Records ( strDicId, html ) "
						+ "VALUES ( @strDicId, @html )";
					command.AddParameter( "strDicId" );
					command.AddParameter( "html" );

					foreach ( uint r in Site.Records ) {
						command.GetParameter( "strDicId" ).Value = r;
						StringBuilder html = new StringBuilder();
						Site.AppendRecord( html, r );
						command.GetParameter( "html" ).Value = html.ToString();
						command.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
		}

		public void ExportSettings() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Settings ( id INTEGER PRIMARY KEY AUTOINCREMENT, strDicName INT, strDicDesc INT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Settings_Options ( id INTEGER PRIMARY KEY AUTOINCREMENT, settingId INT, strDicId INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() )
				using ( var commandOpt = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Settings ( strDicName, strDicDesc, html ) VALUES ( @strDicName, @strDicDesc, @html )";
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "html" );
					commandOpt.CommandText = "INSERT INTO Settings_Options ( settingId, strDicId ) VALUES ( @settingId, @strDicId )";
					commandOpt.AddParameter( "settingId" );
					commandOpt.AddParameter( "strDicId" );

					foreach ( var s in Site.Settings ) {
						command.GetParameter( "strDicName" ).Value = s.NameStringDicId;
						command.GetParameter( "strDicDesc" ).Value = s.DescStringDicId;
						StringBuilder html = new StringBuilder();
						Site.AppendSetting( html, s.NameStringDicId, s.DescStringDicId, s.OptionsStringDicIds[0],
							s.OptionsStringDicIds[1], s.OptionsStringDicIds[2], s.OptionsStringDicIds[3] );
						command.GetParameter( "html" ).Value = html.ToString();
						command.ExecuteNonQuery();

						long lastId = GetLastInsertedId();
						foreach ( uint so in s.OptionsStringDicIds ) {
							if ( so <= 0 ) { continue; }
							commandOpt.GetParameter( "settingId" ).Value = lastId;
							commandOpt.GetParameter( "strDicId" ).Value = so;
							commandOpt.ExecuteNonQuery();
						}
					}
				}
				transaction.Commit();
			}
		}

		public void ExportGradeShop() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE GradeShop ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, strDicName INT, strDicDesc INT, cost INT, "
						+ "refString TEXT, html TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO GradeShop ( id, gameId, strDicName, strDicDesc, cost, refString, html ) "
						+ "VALUES ( @id, @gameId, @strDicName, @strDicDesc, @cost, @refString, @html )";
					command.AddParameter( "id" );
					command.AddParameter( "gameId" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "cost" );
					command.AddParameter( "refString" );
					command.AddParameter( "html" );

					foreach ( var g in Site.GradeShop.GradeShopEntryList ) {
						command.GetParameter( "id" ).Value = g.ID;
						command.GetParameter( "gameId" ).Value = g.InGameID;
						command.GetParameter( "strDicName" ).Value = g.NameStringDicID;
						command.GetParameter( "strDicDesc" ).Value = g.DescStringDicID;
						command.GetParameter( "cost" ).Value = g.GradeCost;
						command.GetParameter( "refString" ).Value = g.RefString;
						command.GetParameter( "html" ).Value = g.GetDataAsHtml( Site.Version, Site.StringDic, Site.InGameIdDict );
						command.ExecuteNonQuery();

					}
				}
				transaction.Commit();
			}
		}

		public void ExportNecropolis() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE NecropolisFloors ( id INTEGER PRIMARY KEY AUTOINCREMENT, floorName VARCHAR(16) UNIQUE, map TEXT, html TEXT, htmlEnemies TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO NecropolisFloors ( floorName, map, html, htmlEnemies ) VALUES ( @floorName, @map, @html, @htmlEnemies )";
					command.AddParameter( "floorName" );
					command.AddParameter( "map" );
					command.AddParameter( "html" );
					command.AddParameter( "htmlEnemies" );

					foreach ( var floor in Site.NecropolisFloors.FloorList ) {
						int floorNumberLong = Int32.Parse( floor.RefString1.Split( '_' ).Last() );
						int floorNumber = ( floorNumberLong - 1 ) % 10 + 1;
						int floorStratumAsNumber = ( floorNumberLong - 1 ) / 10 + 1;
						string floorStratum = ( (char)( floorStratumAsNumber + 64 ) ).ToString();
						string html = Site.NecropolisMaps[floor.RefString2].GetDataAsHtml( floorStratum, floorNumber, null, null, null,
							Site.Version, Site.NecropolisTreasures, Site.Items, Site.InGameIdDict, surroundingTable: false, phpLinks: true );
						string htmlEnemies = Site.NecropolisMaps[floor.RefString2].GetDataAsHtml( floorStratum, floorNumber, Site.Enemies, Site.EnemyGroups,
							Site.EncounterGroups, Site.Version, Site.NecropolisTreasures, Site.Items, Site.InGameIdDict, surroundingTable: false, phpLinks: true );

						command.GetParameter( "floorName" ).Value = floor.RefString1;
						command.GetParameter( "map" ).Value = floor.RefString2;
						command.GetParameter( "html" ).Value = html;
						command.GetParameter( "htmlEnemies" ).Value = htmlEnemies;
						command.ExecuteNonQuery();

					}
				}

				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE NecropolisMaps ( id INTEGER PRIMARY KEY AUTOINCREMENT, mapName VARCHAR(5) UNIQUE, tilesX INT, tilesY INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE NecropolisMaps_Tiles ( id INTEGER PRIMARY KEY AUTOINCREMENT, mapId INT, posX INT, posY INT, type INT, "
						+ "exitDiff INT, encounterId INT, framesToMove INT, regularTresure INT, specialTreasure INT, moveUpAllowed INT, moveDownAllowed INT, "
						+ "moveLeftAllowed INT, moveRightAllowed INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() )
				using ( var commandTile = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO NecropolisMaps ( mapName, tilesX, tilesY ) VALUES ( @mapName, @tilesX, @tilesY )";
					command.AddParameter( "mapName" );
					command.AddParameter( "tilesX" );
					command.AddParameter( "tilesY" );

					commandTile.CommandText = "INSERT INTO NecropolisMaps_Tiles ( mapId, posX, posY, type, exitDiff, encounterId, framesToMove, "
						+ "regularTresure, specialTreasure, moveUpAllowed, moveDownAllowed, moveLeftAllowed, moveRightAllowed ) VALUES ( @mapId, @posX, "
						+ "@posY, @type, @exitDiff, @encounterId, @framesToMove, @regularTresure, @specialTreasure, @moveUpAllowed, @moveDownAllowed, "
						+ "@moveLeftAllowed, @moveRightAllowed )";
					commandTile.AddParameter( "mapId" );
					commandTile.AddParameter( "posX" );
					commandTile.AddParameter( "posY" );
					commandTile.AddParameter( "type" );
					commandTile.AddParameter( "exitDiff" );
					commandTile.AddParameter( "encounterId" );
					commandTile.AddParameter( "framesToMove" );
					commandTile.AddParameter( "regularTresure" );
					commandTile.AddParameter( "specialTreasure" );
					commandTile.AddParameter( "moveUpAllowed" );
					commandTile.AddParameter( "moveDownAllowed" );
					commandTile.AddParameter( "moveLeftAllowed" );
					commandTile.AddParameter( "moveRightAllowed" );

					foreach ( var kvp in Site.NecropolisMaps ) {
						command.GetParameter( "mapName" ).Value = kvp.Key;
						command.GetParameter( "tilesX" ).Value = kvp.Value.HorizontalTiles;
						command.GetParameter( "tilesY" ).Value = kvp.Value.VerticalTiles;
						command.ExecuteNonQuery();

						long insertedId = GetLastInsertedId();

						for ( int y = 0; y < kvp.Value.VerticalTiles; y++ ) {
							for ( int x = 0; x < kvp.Value.HorizontalTiles; x++ ) {
								var tile = kvp.Value.TileList[(int)( y * kvp.Value.HorizontalTiles + x )];
								commandTile.GetParameter( "mapId" ).Value = insertedId;
								commandTile.GetParameter( "posX" ).Value = x;
								commandTile.GetParameter( "posY" ).Value = y;
								commandTile.GetParameter( "type" ).Value = tile.RoomType;
								commandTile.GetParameter( "exitDiff" ).Value = tile.FloorExitDiff;
								commandTile.GetParameter( "encounterId" ).Value = tile.EnemyGroup;
								commandTile.GetParameter( "framesToMove" ).Value = tile.FramesToMove;
								commandTile.GetParameter( "regularTresure" ).Value = tile.RegularTreasure;
								commandTile.GetParameter( "specialTreasure" ).Value = tile.SpecialTreasure;
								commandTile.GetParameter( "moveUpAllowed" ).Value = tile.MoveUpAllowed;
								commandTile.GetParameter( "moveDownAllowed" ).Value = tile.MoveDownAllowed;
								commandTile.GetParameter( "moveLeftAllowed" ).Value = tile.MoveLeftAllowed;
								commandTile.GetParameter( "moveRightAllowed" ).Value = tile.MoveRightAllowed;
								commandTile.ExecuteNonQuery();
							}
						}
					}
				}

				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE NecropolisTreasures ( id INTEGER PRIMARY KEY AUTOINCREMENT, treasureName TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE NecropolisTreasures_Chests ( id INTEGER PRIMARY KEY AUTOINCREMENT, treasureId INT, slot INT, "
						+ "type INT, posX FLOAT, posY FLOAT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE NecropolisTreasures_Items ( id INTEGER PRIMARY KEY AUTOINCREMENT, chestId INT, slot INT, "
						+ "itemId INT, chance INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() )
				using ( var commandChest = DB.CreateCommand() )
				using ( var commandItem = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO NecropolisTreasures ( id, treasureName ) VALUES ( @id, @treasureName )";
					command.AddParameter( "id" );
					command.AddParameter( "treasureName" );

					commandChest.CommandText = "INSERT INTO NecropolisTreasures_Chests ( treasureId, slot, type, posX, posY ) VALUES ( @treasureId, @slot, @type, @posX, @posY )";
					commandChest.AddParameter( "treasureId" );
					commandChest.AddParameter( "slot" );
					commandChest.AddParameter( "type" );
					commandChest.AddParameter( "posX" );
					commandChest.AddParameter( "posY" );

					commandItem.CommandText = "INSERT INTO NecropolisTreasures_Items ( chestId, slot, itemId, chance ) VALUES ( @chestId, @slot, @itemId, @chance )";
					commandItem.AddParameter( "chestId" );
					commandItem.AddParameter( "slot" );
					commandItem.AddParameter( "itemId" );
					commandItem.AddParameter( "chance" );

					foreach ( var t in Site.NecropolisTreasures.TreasureInfoList ) {
						command.GetParameter( "id" ).Value = t.ID;
						command.GetParameter( "treasureName" ).Value = t.RefString;
						command.ExecuteNonQuery();

						for ( int i = 0; i < t.ChestTypes.Length; ++i ) {
							commandChest.GetParameter( "treasureId" ).Value = t.ID;
							commandChest.GetParameter( "slot" ).Value = i;
							commandChest.GetParameter( "type" ).Value = t.ChestTypes[i];
							commandChest.GetParameter( "posX" ).Value = t.ChestPositions[i * 2];
							commandChest.GetParameter( "posY" ).Value = t.ChestPositions[i * 2 + 1];
							commandChest.ExecuteNonQuery();

							long insertedId = GetLastInsertedId();

							int itemSlots = t.Items.Length / t.ChestTypes.Length;
							for ( int j = 0; j < itemSlots; j++ ) {
								commandItem.GetParameter( "chestId" ).Value = insertedId;
								commandItem.GetParameter( "slot" ).Value = j;
								commandItem.GetParameter( "itemId" ).Value = t.Items[itemSlots * i + j];
								commandItem.GetParameter( "chance" ).Value = t.Chances[itemSlots * i + j];
								commandItem.ExecuteNonQuery();
							}
						}
					}
				}

				transaction.Commit();
			}
		}

		private long GetLastInsertedId() {
			using ( var cmd = DB.CreateCommand() ) {
				cmd.CommandText = "SELECT last_insert_rowid()";
				return (long)cmd.ExecuteScalar();
			}
		}
	}

	public static class DatabaseExtensions {
		public static void AddParameter( this IDbCommand command, string name ) {
			IDbDataParameter parameter = command.CreateParameter();
			parameter.ParameterName = name;
			command.Parameters.Add( parameter );
		}
		public static void AddParameter<T>( this IDbCommand command, string name, T value ) {
			IDbDataParameter parameter = command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;
			command.Parameters.Add( parameter );
		}
		public static IDbDataParameter GetParameter( this IDbCommand command, string name ) {
			return (IDbDataParameter)command.Parameters[name];
		}
	}
}
