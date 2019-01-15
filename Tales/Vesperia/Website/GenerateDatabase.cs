using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using HyoutaTools.Tales.Vesperia.ScenarioFile;
using System.Text.RegularExpressions;

namespace HyoutaTools.Tales.Vesperia.Website {
	public class GenerateDatabase {
		private WebsiteGenerator Site;
		private WebsiteGenerator SiteCompare;
		private IDbConnection DB;

		public GenerateDatabase( WebsiteGenerator website, IDbConnection databaseConnection, WebsiteGenerator websiteForComparison = null ) {
			this.Site = website;
			this.DB = databaseConnection;
			this.SiteCompare = websiteForComparison;
		}

		enum ChangeStatus { NoComparison, SameLine, ChangedOrAddedLine, NewFile }

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
				if ( Site.Version.HasPS3Content() ) {
					ExportSearchPoints();
					ExportNecropolis();
				}
				if ( Site.Version == GameVersion.PS3 ) {
					ExportTrophies();
				}
				ExportScenarioDat();
				ExportSkitText();
				ExportScenarioMetadata();
				ExportSkitMetadata();
			} finally {
				DB.Close();
			}
		}

		private void ExportScenarioDat() {
			AddBattleStringsToScenario();
			if ( Site.Version == GameVersion.PS3 ) {
				CleanScenarioStrings( Site.ScenarioFiles, Site.InGameIdDict );
			}

			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE ScenarioDat ( id INTEGER PRIMARY KEY AUTOINCREMENT, episodeId VARCHAR(20), displayOrder INT, type INT, jpName TEXT, jpText TEXT, enName TEXT, enText TEXT, "
						+ "jpSearchKanji TEXT, jpSearchFuri TEXT, enSearch TEXT, changeStatus INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX ScenarioDat_EpisodeId_Index ON ScenarioDat ( episodeId )";
					command.ExecuteNonQuery();
				}

				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO ScenarioDat ( episodeId, displayOrder, type, jpName, jpText, enName, enText, jpSearchKanji, jpSearchFuri, enSearch, changeStatus ) "
						+ "VALUES ( @episodeId, @displayOrder, @type, @jpName, @jpText, @enName, @enText, @jpSearchKanji, @jpSearchFuri, @enSearch, @changeStatus )";
					command.AddParameter( "episodeId" );
					command.AddParameter( "displayOrder" );
					command.AddParameter( "type" );
					command.AddParameter( "jpName" );
					command.AddParameter( "jpText" );
					command.AddParameter( "enName" );
					command.AddParameter( "enText" );
					command.AddParameter( "jpSearchKanji" );
					command.AddParameter( "jpSearchFuri" );
					command.AddParameter( "enSearch" );
					command.AddParameter( "changeStatus" );

					foreach ( var kvp in Site.ScenarioFiles ) {
						var episodeId = kvp.Key;
						var scenario = kvp.Value;

						Tales.Vesperia.ScenarioFile.ScenarioFile sceCmp = null;
						if ( SiteCompare != null ) {
							if ( SiteCompare.ScenarioFiles.ContainsKey( kvp.Value.Metadata.EpisodeId ) ) {
								sceCmp = SiteCompare.ScenarioFiles[kvp.Value.Metadata.EpisodeId];
							}
						}

						for ( int i = 0; i < scenario.EntryList.Count; ++i ) {
							var entry = scenario.EntryList[i];
							if ( entry.Type == TextboxType.Unknown ) {
								entry.Type = TextboxType.Bubble;
								if ( entry.JpName == "Information"
								  || entry.JpName.Contains( "：" )
								  || entry.JpName.EndsWith( "？" )
								  || entry.EnName.EndsWith( "?" )
								  || entry.EnName.StartsWith( "Select" )
								  || entry.EnText == "Yes\nNo"
								   ) {
									entry.Type = TextboxType.Information;
								}
								if ( entry.EnText.StartsWith( "\x03(2)" ) ) { entry.Type = TextboxType.Subtitle; }
							}

							string[] jpTextArray = entry.JpText.Split( '\f' );
							string[] enTextArray = entry.EnText.Split( '\f' );
							int textboxCount = Math.Max( jpTextArray.Length, enTextArray.Length );

							const int maxTextboxCount = 16;
							Util.Assert( textboxCount <= maxTextboxCount );
							for ( int j = 0; j < textboxCount; ++j ) {
								string jpText = j < jpTextArray.Length ? jpTextArray[j] : "";
								string enText = j < enTextArray.Length ? enTextArray[j] : "";
								string jpTextSearchKanji = CleanStringForSearch( jpText, true, false );
								command.GetParameter( "episodeId" ).Value = episodeId;
								command.GetParameter( "displayOrder" ).Value = i * maxTextboxCount + j;
								command.GetParameter( "type" ).Value = (int)entry.Type;
								command.GetParameter( "jpName" ).Value = entry.JpName.ToHtmlJpn( Site.InGameIdDict, Site.Version );
								command.GetParameter( "jpText" ).Value = jpText.ToHtmlJpn( Site.InGameIdDict, Site.Version );
								command.GetParameter( "enName" ).Value = entry.EnName.ToHtmlEng( Site.InGameIdDict, Site.Version );
								command.GetParameter( "enText" ).Value = enText.ToHtmlEng( Site.InGameIdDict, Site.Version );
								command.GetParameter( "jpSearchKanji" ).Value = jpTextSearchKanji;
								command.GetParameter( "jpSearchFuri" ).Value = CleanStringForSearch( jpText, true, true );
								command.GetParameter( "enSearch" ).Value = CleanStringForSearch( enText, false, false );

								ChangeStatus changeStatus = ChangeStatus.NoComparison;
								if ( SiteCompare != null ) {
									if ( sceCmp != null ) {
										string textJpCleanCmp = CleanCleanedStringForVersionComparision( jpTextSearchKanji );
										changeStatus = ChangeStatus.ChangedOrAddedLine;
										foreach ( var entry360 in sceCmp.EntryList ) {
											foreach ( var lineCmp in entry360.JpText.Split( '\f' ) ) {
												string cmpclean = CleanStringForSearch( lineCmp, true, false );
												cmpclean = CleanCleanedStringForVersionComparision( cmpclean );
												if ( textJpCleanCmp.Like( cmpclean ) ) {
													changeStatus = ChangeStatus.SameLine;
													break;
												}
											}
										}
									} else {
										changeStatus = ChangeStatus.NewFile;
									}
								}
								command.GetParameter( "changeStatus" ).Value = (int)changeStatus;

								command.ExecuteNonQuery();
							}
						}
					}
				}
				transaction.Commit();
			}
		}

		private string CleanStringForSearch( string s, bool useJapaneseNames, bool removeKanjiWithFurigana ) {
			s = VesperiaUtil.RemoveTags( s, Site.InGameIdDict, useJapaneseNames: useJapaneseNames, removeKanjiWithFurigana: removeKanjiWithFurigana );
			s = s.Replace( '\n', ' ' ); // remove newlines
			s = Regex.Replace( s, "\\s+", " " ); // remove excessive whitespace
			s = s.Trim();
			return s;
		}

		private string CleanCleanedStringForVersionComparision( string s ) {
			s = s.Replace( " ", "" );
			s = s.Replace( "!", "" );
			s = s.Replace( "?", "" );
			s = s.Replace( "*", "" );
			s = s.Replace( "@", "" );
			s = s.Replace( "[", "" );
			s = s.Replace( "]", "" );
			s = s.Replace( "(", "" );
			s = s.Replace( ")", "" );
			s = s.Replace( "{", "" );
			s = s.Replace( "}", "" );
			s = s.Replace( "`", "" );
			s = s.Replace( "´", "" );
			s = s.Replace( "'", "" );
			s = s.Replace( "…", "" );
			s = s.Replace( "∀", "" );
			s = s.Replace( "♪", "" );
			s = s.Replace( "、", "" );
			s = s.Replace( "。", "" );
			s = s.Replace( "『", "" );
			s = s.Replace( "』", "" );
			s = s.Replace( "　", "" );
			s = s.Replace( "！", "" );
			s = s.Replace( "？", "" );
			s = s.Replace( "（", "" );
			s = s.Replace( "）", "" );
			s = s.Replace( "~", "" );
			s = s.Replace( "〜", "" );
			s = s.Replace( "～", "" );
			for ( int i = 0; i < 26; ++i ) {
				char ascii = Convert.ToChar( Convert.ToInt32( 'A' ) + i );
				char fullwidth = Convert.ToChar( Convert.ToInt32( 'Ａ' ) + i );
				s = s.Replace( fullwidth, ascii );
			}
			for ( int i = 0; i < 26; ++i ) {
				char ascii = Convert.ToChar( Convert.ToInt32( 'a' ) + i );
				char fullwidth = Convert.ToChar( Convert.ToInt32( 'ａ' ) + i );
				s = s.Replace( fullwidth, ascii );
			}
			for ( int i = 0; i < 10; ++i ) {
				char ascii = Convert.ToChar( Convert.ToInt32( '0' ) + i );
				char fullwidth = Convert.ToChar( Convert.ToInt32( '０' ) + i );
				s = s.Replace( fullwidth, ascii );
			}
			// some 360 JP strings contain stray ・ where there should be kanji, maybe a shiftjis->utf8 conversion problem
			// that was never caught since JP text is unused in EU game anyway, so do some wildcard comparsion for those
			s = s.Replace( "・", "*" );
			return s;
		}

		private void AddBattleStringsToScenario() {
			foreach ( var kvp in Site.BattleTextFiles ) {
				var fakeScenario = new ScenarioFile.ScenarioFile();
				fakeScenario.EntryList = kvp.Value.EntryList;
				fakeScenario.Metadata = new ScenarioData() { EpisodeId = kvp.Key, HumanReadableName = kvp.Key };
				Site.ScenarioFiles.Add( kvp.Key, fakeScenario );
			}
		}

		private void ExportSkitText() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE SkitText ( id INTEGER PRIMARY KEY AUTOINCREMENT, skitId VARCHAR(8), displayOrder INT, jpChar TEXT, enChar TEXT, jpText TEXT, enText TEXT, "
						+ "jpSearchKanji TEXT, jpSearchFuri TEXT, enSearch TEXT, changeStatus INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX SkitText_SkitId_Index ON SkitText ( skitId )";
					command.ExecuteNonQuery();
				}

				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO SkitText ( skitId, displayOrder, jpChar, enChar, jpText, enText, jpSearchKanji, jpSearchFuri, enSearch, changeStatus ) "
						+ "VALUES ( @skitId, @displayOrder, @jpChar, @enChar, @jpText, @enText, @jpSearchKanji, @jpSearchFuri, @enSearch, @changeStatus )";
					command.AddParameter( "skitId" );
					command.AddParameter( "displayOrder" );
					command.AddParameter( "jpChar" );
					command.AddParameter( "enChar" );
					command.AddParameter( "jpText" );
					command.AddParameter( "enText" );
					command.AddParameter( "jpSearchKanji" );
					command.AddParameter( "jpSearchFuri" );
					command.AddParameter( "enSearch" );
					command.AddParameter( "changeStatus" );

					foreach ( var kvp in Site.SkitText ) {
						var skitId = kvp.Key;
						var skit = kvp.Value;
						Tales.Vesperia.TO8CHTX.ChatFile skitCmp = null;
						if ( SiteCompare != null ) {
							if ( SiteCompare.SkitText.ContainsKey( skitId ) ) {
								skitCmp = SiteCompare.SkitText[skitId];
							}
						}

						for ( int i = 0; i < skit.Lines.Length; ++i ) {
							string nameJp = skit.Lines[i].SName;
							int idx = nameJp.IndexOf( '(' ) + 1;
							nameJp = nameJp.Substring( idx, nameJp.LastIndexOf( ')' ) - idx );
							string nameEn = skit.Lines[i].SNameEnglishNotUsedByGame;
							if ( nameEn != null ) {
								idx = nameEn.IndexOf( '(' ) + 1;
								nameEn = nameEn.Substring( idx, nameEn.LastIndexOf( ')' ) - idx );
							} else {
								nameEn = nameJp;
							}

							string textJp = skit.Lines[i].SJPN;
							string textEn = skit.Lines[i].SENG;
							string textJpSearchKanji = CleanStringForSearch( textJp, true, false );

							command.GetParameter( "skitId" ).Value = skitId;
							command.GetParameter( "displayOrder" ).Value = i;
							command.GetParameter( "jpChar" ).Value = nameJp;
							command.GetParameter( "enChar" ).Value = nameEn;
							command.GetParameter( "jpText" ).Value = textJp.ToHtmlJpn( Site.InGameIdDict, Site.Version );
							command.GetParameter( "enText" ).Value = textEn.ToHtmlEng( Site.InGameIdDict, Site.Version );
							command.GetParameter( "jpSearchKanji" ).Value = textJpSearchKanji;
							command.GetParameter( "jpSearchFuri" ).Value = CleanStringForSearch( textJp, true, true );
							command.GetParameter( "enSearch" ).Value = CleanStringForSearch( textEn, false, false );

							ChangeStatus changeStatus = ChangeStatus.NoComparison;
							if ( SiteCompare != null ) {
								if ( skitCmp != null ) {
									string textJpCleanCmp = CleanCleanedStringForVersionComparision( textJpSearchKanji );
									changeStatus = ChangeStatus.ChangedOrAddedLine;
									foreach ( var lineCmp in skitCmp.Lines ) {
										string cmpclean = CleanStringForSearch( lineCmp.SJPN, true, false );
										cmpclean = CleanCleanedStringForVersionComparision( cmpclean );
										if ( textJpCleanCmp.Like( cmpclean ) ) {
											changeStatus = ChangeStatus.SameLine;
											break;
										}
									}
								} else {
									changeStatus = ChangeStatus.NewFile;
								}
							}
							command.GetParameter( "changeStatus" ).Value = (int)changeStatus;

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
					command.CommandText = "CREATE TABLE ScenarioMeta ( id INTEGER PRIMARY KEY AUTOINCREMENT, type INT, sceneGroup INT, parent INT, episodeId VARCHAR(20), descriptionJ TEXT, descriptionE TEXT, changeStatus INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX ScenarioMeta_Type_Group_Index ON ScenarioMeta ( type, sceneGroup )";
					command.ExecuteNonQuery();
				}

				if ( Site.Version == GameVersion.PS3 ) {
					CleanScenarioMetadata( Site.ScenarioGroupsStory, true );
					CleanScenarioMetadata( Site.ScenarioGroupsSidequests, false );
				}

				ExportScenarioMetadata( transaction, Site.ScenarioGroupsStory, 1, Site.Version );
				ExportScenarioMetadata( transaction, Site.ScenarioGroupsSidequests, 2, Site.Version );
				transaction.Commit();
			}
		}

		// removes files from the scenario index that aren't useful for the website user
		// stuff like scenes without dialogue, duplicated scenes
		// also move skits around a bit to make them fit better
		// as well as add our custom split final boss form 3 file
		private void CleanScenarioMetadata( List<List<ScenarioData>> groups, bool sortSkits ) {
			foreach ( var group in groups ) {
				// remove scenes that the user shouldn't see
				group.RemoveAll( x => x.EpisodeId == "EP_000_020" ); // no text
				group.RemoveAll( x => x.EpisodeId == "EP_060_060" ); // no text
				group.RemoveAll( x => x.EpisodeId == "EP_140_041" ); // duplicate of EP_140_040
				group.RemoveAll( x => x.EpisodeId == "EP_150_021" ); // partial duplicate of EP_150_020
				group.RemoveAll( x => x.EpisodeId == "EP_200_080" ); // partial duplicate of EP_200_070
				group.RemoveAll( x => x.EpisodeId == "EP_200_090" ); // partial duplicate of EP_200_070 without new PS3 lines
				group.RemoveAll( x => x.EpisodeId == "EP_210_010" ); // FMV subtitles only for a FMV without dialogue
				group.RemoveAll( x => x.EpisodeId == "EP_260_080_0" ); // partial duplicate of EP_260_080
				group.RemoveAll( x => x.EpisodeId == "EP_260_080_1" ); // no text
				group.RemoveAll( x => x.EpisodeId == "EP_260_080_2" ); // partial duplicate of EP_260_080
				group.RemoveAll( x => x.EpisodeId == "EP_270_030" ); // scene with tokunaga in nordopolica, presumably not in PS3 version
				group.RemoveAll( x => x.EpisodeId == "EP_320_065" ); // no text
				group.RemoveAll( x => x.EpisodeId == "EP_370_070" ); // no text
				group.RemoveAll( x => x.EpisodeId == "EP_420_011" ); // effectively duplicate of EP_420_010
				group.RemoveAll( x => x.EpisodeId == "EP_420_090_1" ); // no text
				group.RemoveAll( x => x.EpisodeId == "EP_420_111" ); // duplicate of EP_420_110
				group.RemoveAll( x => x.EpisodeId == "EP_650_053" ); // english 360 FMV subs
				group.RemoveAll( x => x.EpisodeId == "EP_0017_050" ); // scene no longer in PS3 version
				group.RemoveAll( x => x.EpisodeId == "EP_0017_060" ); // scene no longer in PS3 version
				group.RemoveAll( x => x.EpisodeId == "EP_0030_080" ); // scene no longer in PS3 version
				group.RemoveAll( x => x.EpisodeId == "EP_0030_090" ); // unused variant of radiant winged one
				group.RemoveAll( x => x.EpisodeId == "EP_0070_010" ); // battle tutorial copy
				group.RemoveAll( x => x.EpisodeId == "EP_0080_010" ); // skill tutorial copy
				group.RemoveAll( x => x.EpisodeId == "EP_0090_010" ); // cooking tutorial copy
				group.RemoveAll( x => x.EpisodeId == "EP_0121_021" ); // duplicate of EP_0121_020
				group.RemoveAll( x => x.EpisodeId == "EP_0170_011" ); // duplicate of EP_0170_010
				group.RemoveAll( x => x.EpisodeId == "EP_0170_021" ); // duplicate of EP_0170_020
				group.RemoveAll( x => x.EpisodeId == "EP_0170_031" ); // duplicate of EP_0170_030
				group.RemoveAll( x => x.EpisodeId == "EP_0271_011" ); // duplicate of EP_0271_010
				group.RemoveAll( x => x.EpisodeId == "EP_0271_021" ); // duplicate of EP_0271_020
				group.RemoveAll( x => x.EpisodeId == "EP_0271_031" ); // duplicate of EP_0271_030
				group.RemoveAll( x => x.EpisodeId == "EP_0300_030" ); // partial duplicate of EP_0300_020
				group.RemoveAll( x => x.EpisodeId == "EP_0741_020" ); // scene no longer in PS3 version (has been adapted into a skit)
				group.RemoveAll( x => x.EpisodeId == "EP_0920_010" ); // synthesis tutorial copy (old revision?)
				group.RemoveAll( x => x.EpisodeId == "EP_0960_020" ); // no text
				group.RemoveAll( x => x.EpisodeId == "EP_0990_010" ); // boat tutorial, game seems to use the one in EP_250_090 instead
				group.RemoveAll( x => x.EpisodeId == "EP_1040_020" ); // burst arte tutorial, game should use the Battle one instead
				group.RemoveAll( x => x.EpisodeId == "EP_0960_010" ); // fatal strike scene in caer bocram, copy of EP_170_030
				group.RemoveAll( x => x.EpisodeId == "EP_1530_060" ); // unused variation of BTL_XTM_EVENT

				// stuff that's reinserted at the proper place later
				group.RemoveAll( x => x.EpisodeId == "EP_1040_010" ); // part of burst arte tutorial
				group.RemoveAll( x => x.EpisodeId == "EP_1040_030" ); // part of burst arte tutorial
				group.RemoveAll( x => x.EpisodeId == "EP_0100_010" ); // airship tutorial
				group.RemoveAll( x => x.EpisodeId == "EP_0910_010" ); // surprise encounter tutorial
				group.RemoveAll( x => x.EpisodeId == "EP_0980_010" ); // ovl lv 2
				group.RemoveAll( x => x.EpisodeId == "EP_1010_010" ); // weather tutorial
				group.RemoveAll( x => x.EpisodeId == "EP_1070_010" ); // camping tutorial
				group.RemoveAll( x => x.EpisodeId == "EP_1270_010" ); // encounter link tutorial
				group.RemoveAll( x => x.EpisodeId == "EP_1560_010" ); // abysmal hollow first visit
				group.RemoveAll( x => x.EpisodeId == "EP_1160_010" ); // ghasfarost gear puzzle
				group.RemoveAll( x => x.EpisodeId == "EP_1160_020" ); // ghasfarost gear puzzle
				group.RemoveAll( x => x.EpisodeId == "EP_1160_030" ); // ghasfarost gear puzzle
				group.RemoveAll( x => x.EpisodeId == "EP_1160_040" ); // ghasfarost gear puzzle
				group.RemoveAll( x => x.EpisodeId == "EP_1030_010" ); // caer bocram warp blastia
				group.RemoveAll( x => x.EpisodeId == "EP_1030_020" ); // caer bocram warp blastia
				group.RemoveAll( x => x.EpisodeId == "EP_1030_021" ); // caer bocram warp blastia
				group.RemoveAll( x => x.EpisodeId == "EP_1030_022" ); // caer bocram warp blastia
				group.RemoveAll( x => x.EpisodeId == "EP_1030_023" ); // caer bocram warp blastia
				group.RemoveAll( x => x.EpisodeId == "EP_0110_010" ); // undine minigame
			}

			if ( sortSkits ) {
				string[] skitsToRemove = new string[] {
					"VC911", "VC912", "VC913", "VC906", "VC907", "VC914", "VC964", "VC967", "VC915", "VC1756", // move skits in the 4 spirits events to fitting places
					"VC921", // also for fetching Harry late game one skit seems misplaced
					"VC927", "VC960", // final zagi skits should be one scene higher
				};
				Dictionary<string, TO8CHLI.SkitInfo> removedSkits = new Dictionary<string, TO8CHLI.SkitInfo>();

				// grab skits and remove them from their files
				foreach ( var group in groups ) {
					foreach ( var scene in group ) {
						foreach ( string skitId in skitsToRemove ) {
							var skit = scene.Skits.Find( x => x.RefString == skitId );
							if ( skit != null ) {
								removedSkits.Add( skitId, skit );
								scene.Skits.Remove( skit );
							}
						}
					}
				}

				Util.Assert( skitsToRemove.Length == removedSkits.Count );

				// insert them in other places instead
				InsertSkitAt( groups, "EP_570_020", removedSkits["VC911"] );
				InsertSkitAt( groups, "EP_580_030", removedSkits["VC912"] );
				InsertSkitAt( groups, "EP_580_030", removedSkits["VC913"] );
				InsertSkitAt( groups, "EP_560_040", removedSkits["VC906"] );
				InsertSkitAt( groups, "EP_560_040", removedSkits["VC907"] );
				InsertSkitAt( groups, "EP_590_040", removedSkits["VC914"] );
				InsertSkitAt( groups, "EP_580_030", removedSkits["VC964"] );
				InsertSkitAt( groups, "EP_580_030", removedSkits["VC967"] );
				InsertSkitAt( groups, "EP_600_010", removedSkits["VC915"] );
				InsertSkitAt( groups, "EP_600_010", removedSkits["VC1756"] );
				InsertSkitAt( groups, "EP_620_010", removedSkits["VC921"] );
				InsertSkitAt( groups, "EP_650_030", removedSkits["VC927"] );
				InsertSkitAt( groups, "EP_650_030", removedSkits["VC960"] );
			}

			// add our custom final boss form 3 file
			InsertScenarioAt( groups, "EP_0030_070", new ScenarioData() { EpisodeId = "EP_650_051b", HumanReadableName = "Radiant Winged One" } );

			// add the battle stuff and the split text from that
			InsertScenarioAt( groups, "EP_020_040", new ScenarioData() { EpisodeId = "EP_020_040b", HumanReadableName = "Cumore" } );
			InsertScenarioAt( groups, "EP_020_040", new ScenarioData() { EpisodeId = "BTL_EP_0070_010", HumanReadableName = "Battle Tutorial" } );
			InsertScenarioAt( groups, "EP_030_040", new ScenarioData() { EpisodeId = "EP_030_040b", HumanReadableName = "Mysterious Girl (2)" } );
			InsertScenarioAt( groups, "EP_030_040", new ScenarioData() { EpisodeId = "BTL_EP_030_040", HumanReadableName = "Knights Battle" } );
			InsertScenarioAt( groups, "EP_030_080", new ScenarioData() { EpisodeId = "EP_030_080b", HumanReadableName = "Flynn's Room (2)" } );
			InsertScenarioAt( groups, "EP_030_080", new ScenarioData() { EpisodeId = "BTL_EP_030_080", HumanReadableName = "Zagi Battle" } );
			InsertScenarioAt( groups, "EP_130_040", new ScenarioData() { EpisodeId = "EP_130_040b", HumanReadableName = "Adecor/Boccos (2)" } );
			InsertScenarioAt( groups, "EP_130_040", new ScenarioData() { EpisodeId = "BTL_EP_0950_010", HumanReadableName = "Over Limit Tutorial" } );
			InsertScenarioAt( groups, "EP_170_030", new ScenarioData() { EpisodeId = "BTL_EP_0960_020", HumanReadableName = "Fatal Strike Tutorial" } );
			InsertScenarioAt( groups, "EP_180_020", new ScenarioData() { EpisodeId = "EP_1040_030", HumanReadableName = "Adecor/Boccos (2)" } );
			InsertScenarioAt( groups, "EP_180_020", new ScenarioData() { EpisodeId = "BTL_EP_1040_020", HumanReadableName = "Burst Arte Tutorial" } );
			InsertScenarioAt( groups, "EP_180_020", new ScenarioData() { EpisodeId = "EP_1040_010", HumanReadableName = "Adecor/Boccos (1)" } );
			InsertScenarioAt( groups, "EP_150_170", new ScenarioData() { EpisodeId = "BTL_EP_150_170", HumanReadableName = "Zagi (Secret Mission)" } );
			InsertScenarioAt( groups, "EP_170_050", new ScenarioData() { EpisodeId = "EP_170_050b", HumanReadableName = "Dreaded Giant (2)" } );
			InsertScenarioAt( groups, "EP_170_050", new ScenarioData() { EpisodeId = "BTL_EP_170_050", HumanReadableName = "Dreaded Giant (Battle)" } );
			InsertScenarioAt( groups, "EP_210_090", new ScenarioData() { EpisodeId = "BTL_EP_210_090", HumanReadableName = "Barbos (Secret Mission)" } );
			InsertScenarioAt( groups, "EP_270_110", new ScenarioData() { EpisodeId = "BTL_EP_270_110b", HumanReadableName = "Zagi (Secret Mission)" } );
			InsertScenarioAt( groups, "EP_270_110", new ScenarioData() { EpisodeId = "EP_270_110b", HumanReadableName = "Coliseum: Zagi" } );
			InsertScenarioAt( groups, "EP_270_110", new ScenarioData() { EpisodeId = "BTL_EP_270_110", HumanReadableName = "Coliseum: Finals (Battle)" } );
			InsertScenarioAt( groups, "EP_340_080", new ScenarioData() { EpisodeId = "BTL_EP_340_070", HumanReadableName = "Belius (Secret Mission)" } );
			InsertScenarioAt( groups, "EP_370_050", new ScenarioData() { EpisodeId = "BTL_EP_370_050", HumanReadableName = "Tison/Nan (Secret Mission)" } );
			InsertScenarioAt( groups, "EP_420_080", new ScenarioData() { EpisodeId = "BTL_EP_420_080", HumanReadableName = "Schwann (Secret Mission)" } );
			InsertScenarioAt( groups, "EP_440_040", new ScenarioData() { EpisodeId = "BTL_EP_440_040", HumanReadableName = "Zagi (Secret Mission)" } );
			InsertScenarioAt( groups, "EP_470_030", new ScenarioData() { EpisodeId = "EP_470_030b", HumanReadableName = "Aer Krene (2)" } );
			InsertScenarioAt( groups, "EP_470_030", new ScenarioData() { EpisodeId = "BTL_EP_470_030", HumanReadableName = "Boss Battle (Karol)" } );
			InsertScenarioAt( groups, "EP_490_060", new ScenarioData() { EpisodeId = "BTL_EP_490_060_1", HumanReadableName = "Estelle (Secret Mission)" } );
			InsertScenarioAt( groups, "EP_510_050", new ScenarioData() { EpisodeId = "EP_510_050b", HumanReadableName = "Yeager (2)" } );
			InsertScenarioAt( groups, "EP_510_050", new ScenarioData() { EpisodeId = "BTL_EP_510_050b", HumanReadableName = "Yeager (Secret Mission)" } );
			InsertScenarioAt( groups, "EP_510_050", new ScenarioData() { EpisodeId = "BTL_EP_510_050", HumanReadableName = "Yeager (Battle)" } );
			InsertScenarioAt( groups, "EP_510_070", new ScenarioData() { EpisodeId = "BTL_EP_510_080b", HumanReadableName = "Alexei (Secret Mission)" } );
			InsertScenarioAt( groups, "EP_510_070", new ScenarioData() { EpisodeId = "BTL_EP_510_080", HumanReadableName = "Alexei (Battle)" } );
			InsertScenarioAt( groups, "EP_640_050", new ScenarioData() { EpisodeId = "BTL_EP_640_050", HumanReadableName = "Flynn (Secret Mission)" } );
			InsertScenarioAt( groups, "EP_650_030", new ScenarioData() { EpisodeId = "BTL_EP_650_030", HumanReadableName = "Zagi (Secret Mission)" } );
			InsertScenarioAt( groups, "EP_1530_050", new ScenarioData() { EpisodeId = "BTL_XTM_EVENT", HumanReadableName = "Dungeon Tutorial" } );
			InsertScenarioAt( groups, "EP_0601_030", new ScenarioData() { EpisodeId = "BTL_LL_MONSTER", HumanReadableName = "Giganto Monster (First Encounter)" } );

			InsertScenarioAt( groups, "EP_370_140", new ScenarioData() { EpisodeId = "EP_0100_010", HumanReadableName = "Airship Tutorial" } );
			InsertScenarioAt( groups, "EP_110_030", new ScenarioData() { EpisodeId = "EP_0910_010", HumanReadableName = "Surprise Encounter Tutorial" } );
			InsertScenarioAt( groups, "EP_210_055", new ScenarioData() { EpisodeId = "EP_0980_010", HumanReadableName = "Over Limit Lv. 2 Tutorial" } );
			InsertScenarioAt( groups, "EP_150_052", new ScenarioData() { EpisodeId = "EP_1010_010", HumanReadableName = "Weather Tutorial" } );
			InsertScenarioAt( groups, "EP_140_060", new ScenarioData() { EpisodeId = "EP_1070_010", HumanReadableName = "Camping Tutorial" } );
			InsertScenarioAt( groups, "EP_030_110_1", new ScenarioData() { EpisodeId = "EP_1270_010", HumanReadableName = "Encounter Link Tutorial" } );
			InsertScenarioAt( groups, "EP_1420_010", new ScenarioData() { EpisodeId = "EP_1560_010", HumanReadableName = "Abysmal Hollow" } );
			InsertScenarioAt( groups, "EP_210_080", new ScenarioData() { EpisodeId = "EP_1160_040", HumanReadableName = "Gear Puzzle (4)" } );
			InsertScenarioAt( groups, "EP_210_080", new ScenarioData() { EpisodeId = "EP_1160_030", HumanReadableName = "Gear Puzzle (3)" } );
			InsertScenarioAt( groups, "EP_210_080", new ScenarioData() { EpisodeId = "EP_1160_020", HumanReadableName = "Gear Puzzle (2)" } );
			InsertScenarioAt( groups, "EP_210_080", new ScenarioData() { EpisodeId = "EP_1160_010", HumanReadableName = "Gear Puzzle (1)" } );
			InsertScenarioAt( groups, "EP_170_020", new ScenarioData() { EpisodeId = "EP_1030_023", HumanReadableName = "Warp Blastia (5)" } );
			InsertScenarioAt( groups, "EP_170_020", new ScenarioData() { EpisodeId = "EP_1030_022", HumanReadableName = "Warp Blastia (4)" } );
			InsertScenarioAt( groups, "EP_170_020", new ScenarioData() { EpisodeId = "EP_1030_021", HumanReadableName = "Warp Blastia (3)" } );
			InsertScenarioAt( groups, "EP_170_020", new ScenarioData() { EpisodeId = "EP_1030_020", HumanReadableName = "Warp Blastia (2)" } );
			InsertScenarioAt( groups, "EP_170_010", new ScenarioData() { EpisodeId = "EP_1030_010", HumanReadableName = "Warp Blastia (1)" } );
			InsertScenarioAt( groups, "EP_110_030", new ScenarioData() { EpisodeId = "rui_d01", HumanReadableName = "Sorcerer's Ring" } );
			InsertScenarioAt( groups, "EP_550_030", new ScenarioData() { EpisodeId = "EP_550_030c", HumanReadableName = "Success" } );
			InsertScenarioAt( groups, "EP_550_030", new ScenarioData() { EpisodeId = "EP_550_030b", HumanReadableName = "Minigame" } );

			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050pat2", HumanReadableName = "Final Boss (Patty 2)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050fre2", HumanReadableName = "Final Boss (Flynn 2)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050rap2", HumanReadableName = "Final Boss (Repede 2)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050jud2", HumanReadableName = "Final Boss (Judith 2)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050rav2", HumanReadableName = "Final Boss (Raven 2)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050rit2", HumanReadableName = "Final Boss (Rita 2)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050kar2", HumanReadableName = "Final Boss (Karol 2)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050est2", HumanReadableName = "Final Boss (Estelle 2)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050yur2", HumanReadableName = "Final Boss (Yuri 2)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050pat1", HumanReadableName = "Final Boss (Patty 1)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050fre1", HumanReadableName = "Final Boss (Flynn 1)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050rap1", HumanReadableName = "Final Boss (Repede 1)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050jud1", HumanReadableName = "Final Boss (Judith 1)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050rav1", HumanReadableName = "Final Boss (Raven 1)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050rit1", HumanReadableName = "Final Boss (Rita 1)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050kar1", HumanReadableName = "Final Boss (Karol 1)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050est1", HumanReadableName = "Final Boss (Estelle 1)" } );
			InsertScenarioAt( groups, "EP_650_050", new ScenarioData() { EpisodeId = "EP_650_050yur1", HumanReadableName = "Final Boss (Yuri 1)" } );
		}

		private bool InsertScenarioAt( List<List<ScenarioData>> groups, string episodeId, ScenarioData newScene ) {
			foreach ( var group in groups ) {
				var insertAfterScene = group.Find( x => x.EpisodeId == episodeId );
				if ( insertAfterScene != null ) {
					group.Insert( group.IndexOf( insertAfterScene ) + 1, newScene );
					return true;
				}
			}

			return false;
		}

		private bool InsertSkitAt( List<List<ScenarioData>> groups, string episodeId, TO8CHLI.SkitInfo skit ) {
			foreach ( var group in groups ) {
				var scene = group.Find( x => x.EpisodeId == episodeId );
				if ( scene != null ) {
					scene.Skits.Add( skit );
					return true;
				}
			}
			return false;
		}

		// removes strings from scenario files that shouldn't be shown to the website user
		// stuff like spoilery FMV subtitles or debug text
		private void CleanScenarioStrings( Dictionary<string, ScenarioFile.ScenarioFile> files, Dictionary<uint, TSS.TSSEntry> stringDicInGame ) {
			{
				// remove FMV subtitles from other FMVs, sort the "start game?" question to the front
				var intro = files["EP_000_010"];
				var pleaseAdjustSettings = intro.EntryList[76];
				var startTheGame = intro.EntryList[77];
				intro.EntryList.RemoveRange( 15, intro.EntryList.Count - 15 );
				intro.EntryList.Insert( 0, pleaseAdjustSettings );
				intro.EntryList.Insert( 1, startTheGame );
			}

			{
				// sort the skill tutorial text to the middle of the scene where it belongs
				// cooking tutorial at EP_060_040 doesn't need reordering, it's fine as-is
				var skillTutorial = files["EP_050_010"];
				var sorted = new List<ScenarioFileEntry>( skillTutorial.EntryList.Count );
				for ( int i = 0; i <= 11; ++i ) { sorted.Add( skillTutorial.EntryList[i] ); }
				for ( int i = 27; i <= 32; ++i ) { sorted.Add( skillTutorial.EntryList[i] ); }
				for ( int i = 12; i <= 26; ++i ) { sorted.Add( skillTutorial.EntryList[i] ); }
				skillTutorial.EntryList = sorted;
			}

			{
				// remove FMV subs belonging to other scenes, and sort the remaining ones to the correct in-scene position
				var revivingTheTree = files["EP_090_020"];
				for ( int i = 15; i < 19; ++i ) {
					revivingTheTree.EntryList.Add( revivingTheTree.EntryList[i] );
				}
				revivingTheTree.EntryList.RemoveRange( 0, 76 );

				var dahngrestBridge = files["EP_230_040"];
				for ( int i = 19; i <= 20; ++i ) {
					dahngrestBridge.EntryList.Add( dahngrestBridge.EntryList[i] );
				}
				dahngrestBridge.EntryList.RemoveRange( 0, 76 );

				var alexeiInZaphias = files["EP_450_010"];
				for ( int i = 21; i <= 33; ++i ) {
					alexeiInZaphias.EntryList.Add( alexeiInZaphias.EntryList[i] );
				}
				alexeiInZaphias.EntryList.RemoveRange( 0, 76 );

				var swordStair = files["EP_490_060"];
				for ( int i = 0; i <= 38 - 34; ++i ) {
					swordStair.EntryList.Insert( 123 + i, swordStair.EntryList[i + 34] );
				}
				swordStair.EntryList.RemoveRange( 0, 76 );

				var adephagos = files["EP_510_080"];
				for ( int i = 39; i <= 47; ++i ) {
					adephagos.EntryList.Add( adephagos.EntryList[i] );
				}
				adephagos.EntryList.RemoveRange( 0, 76 );

				var ending = files["EP_650_052"];
				ending.EntryList.RemoveAt( 85 );
				ending.EntryList.RemoveRange( 0, 54 );

				var movieViewer = files["EP_0700_010"];
				for ( int i = 0; i <= 18 - 15; ++i ) {
					movieViewer.EntryList.Insert( 88 + i, movieViewer.EntryList[i + 15] );
				}
				movieViewer.EntryList.RemoveRange( 0, 76 );
			}

			{
				// remove all FMV subs, none of these FMVs have dialogue
				var subLessFmv = new string[] { "EP_420_010", "EP_420_011", "EP_430_010", "EP_440_080", "EP_510_010", "EP_550_040", "EP_600_050", "EP_610_040" };
				foreach ( string episodeId in subLessFmv ) {
					files[episodeId].EntryList.RemoveRange( 0, 76 );
				}
			}

			{
				// remove map text that somehow ended up here
				var raven = files["EP_500_030"];
				raven.EntryList.RemoveRange( 0, 5 );
			}

			{
				// lines for the first part of the final boss scene are not in correct order and have a bunch of duplicates, fix
				var form12 = files["EP_650_050"];
				form12.EntryList.RemoveRange( 163, 7 );
				form12.EntryList.RemoveRange( 102, 5 );
				form12.EntryList.Insert( 102, form12.EntryList[7] );
				form12.EntryList.Insert( 96, form12.EntryList[8] );
				form12.EntryList.Insert( 91, form12.EntryList[6] );
				form12.EntryList.Insert( 85, form12.EntryList[5] );
				form12.EntryList.Insert( 77, form12.EntryList[4] );
				form12.EntryList.Insert( 73, form12.EntryList[3] );
				form12.EntryList.Insert( 68, form12.EntryList[2] );
				form12.EntryList.Insert( 63, form12.EntryList[1] );
				form12.EntryList.Insert( 57, form12.EntryList[0] );
				form12.EntryList.RemoveRange( 0, 9 );

				// split off each character's conversation parts into separate files for ease of navigation
				SplitScenarioFile( files, "EP_650_050", "EP_650_050pat2", 152 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050fre2", 147 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050rap2", 143 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050jud2", 138 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050rav2", 133 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050rit2", 123 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050kar2", 116 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050est2", 109 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050yur2", 102 );

				SplitScenarioFile( files, "EP_650_050", "EP_650_050pat1", 95 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050fre1", 89 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050rap1", 82 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050jud1", 73 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050rav1", 68 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050rit1", 62 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050kar1", 56 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050est1", 49 );
				SplitScenarioFile( files, "EP_650_050", "EP_650_050yur1", 44 );

				// split second part of the scene into two files so you can actually read the ending without getting spoiled on the optional 3rd form
				SplitScenarioFile( files, "EP_650_051", "EP_650_051b", -14 );
			}

			{
				// sorcerer's ring tutorial is only in a map file for some reason, make that presentable
				var sorcRing = files["rui_d01"];
				var pressSquareToShoot = sorcRing.EntryList[31];
				sorcRing.EntryList.RemoveAt( 31 );
				sorcRing.EntryList.Insert( 9, pressSquareToShoot );
				SplitScenarioFile( files, "rui_d01", "rui_d01b", 24 );
			}

			// line is in EP_1040_030 as well and makes more sense there
			files["EP_1040_010"].EntryList.RemoveAt( 14 );

			// split a few files that have in-battle dialogue in the middle of them
			SplitScenarioFile( files, "EP_020_040", "EP_020_040b", 11 ); // battle tutorial
			SplitScenarioFile( files, "EP_030_040", "EP_030_040b", 18 ); // battle with knights when meeting estelle
			SplitScenarioFile( files, "EP_030_080", "EP_030_080b", 19 ); // zagi 1
			SplitScenarioFile( files, "EP_130_040", "EP_130_040b", 12 ); // over limit tutorial
			SplitScenarioFile( files, "EP_170_050", "EP_170_050b", 45 ); // dreaded giant
			SplitScenarioFile( files, "BTL_EP_210_090", "BTL_EP_210_090b", -9 ); // barbos battle
			SplitScenarioFile( files, "EP_270_110", "EP_270_110b", 13 ); // flynn coliseum scene
			SplitScenarioFile( files, "BTL_EP_270_110", "BTL_EP_270_110b", 16 ); // flynn coliseum battle & zagi 3
			SplitScenarioFile( files, "EP_470_030", "EP_470_030b", 25 ); // blade drifts boss
			SplitScenarioFile( files, "BTL_EP_490_060_1", "BTL_EP_490_060_1b", -8 ); // estelle battle
			SplitScenarioFile( files, "EP_510_050", "EP_510_050b", 17 ); // yeager battle
			SplitScenarioFile( files, "BTL_EP_510_050", "BTL_EP_510_050b", -1 ); // yeager secret mission
			SplitScenarioFile( files, "BTL_EP_510_080", "BTL_EP_510_080c", 32 ); // alexei battle
			SplitScenarioFile( files, "BTL_EP_510_080", "BTL_EP_510_080b", -1 ); // alexei secret mission
			SplitScenarioFile( files, "EP_550_030", "EP_550_030c", 30 ); // undine scene

			{
				// Undine minigame tutorial
				var undineMinigame = new ScenarioFile.ScenarioFile();
				undineMinigame.EntryList = new List<ScenarioFileEntry>();
				undineMinigame.EntryList.Add( new ScenarioFileEntry( stringDicInGame[240168], stringDicInGame[240169] ) { Type = TextboxType.Information } ); // read the rules?
				undineMinigame.EntryList.Add( new ScenarioFileEntry( stringDicInGame[240168], stringDicInGame[240165] ) { Type = TextboxType.Information } ); // rules
				undineMinigame.EntryList.Add( new ScenarioFileEntry( stringDicInGame[33892179], stringDicInGame[240173] ) { Type = TextboxType.Information } ); // success
				undineMinigame.EntryList.Add( new ScenarioFileEntry( stringDicInGame[33892179], stringDicInGame[240167] ) { Type = TextboxType.Information } ); // failure
				undineMinigame.EntryList.Add( new ScenarioFileEntry( stringDicInGame[240170], stringDicInGame[240171] ) { Type = TextboxType.Information } ); // ask rita?
				undineMinigame.EntryList.Add( new ScenarioFileEntry( stringDicInGame[33892179], stringDicInGame[240172] ) { Type = TextboxType.Information } ); // asked rita
				undineMinigame.Metadata = new ScenarioData() { EpisodeId = "EP_550_030", HumanReadableName = "Minigame" };
				files.Add( "EP_550_030b", undineMinigame );
			}
		}

		// sign of splitAt decides which file gets to keep which half, positive => new file gets bottom half, negative => new file gets top half
		private void SplitScenarioFile( Dictionary<string, ScenarioFile.ScenarioFile> files, string originalName, string newName, int splitAt ) {
			var orig = files[originalName];
			var clone = orig.CloneShallow();
			int totalFiles = clone.EntryList.Count;

			if ( splitAt < 0 ) {
				orig.EntryList.RemoveRange( 0, -splitAt );
				clone.EntryList.RemoveRange( -splitAt, totalFiles - -splitAt );
			} else {
				orig.EntryList.RemoveRange( splitAt, totalFiles - splitAt );
				clone.EntryList.RemoveRange( 0, splitAt );
			}

			files.Add( newName, clone );
		}

		private void ExportScenarioMetadata( IDbTransaction transaction, List<List<ScenarioData>> groups, int groupType, GameVersion version ) {
			using ( var command = DB.CreateCommand() ) {
				command.CommandText = "INSERT INTO ScenarioMeta ( type, sceneGroup, parent, episodeId, descriptionJ, descriptionE, changeStatus ) VALUES ( @type, @sceneGroup, @parent, @episodeId, @descriptionJ, @descriptionE, @changeStatus )";
				command.AddParameter( "type" );
				command.AddParameter( "sceneGroup" );
				command.AddParameter( "parent" );
				command.AddParameter( "episodeId" );
				command.AddParameter( "descriptionJ" );
				command.AddParameter( "descriptionE" );
				command.AddParameter( "changeStatus" );

				int groupNumber = 0;
				foreach ( var group in groups ) {
					if ( group.Count == 0 ) { continue; }

					string commonBegin = ScenarioData.FindMostCommonStart( group );
					command.GetParameter( "type" ).Value = groupType;
					command.GetParameter( "sceneGroup" ).Value = groupNumber;
					command.GetParameter( "parent" ).Value = null;
					command.GetParameter( "episodeId" ).Value = null;
					command.GetParameter( "descriptionJ" ).Value = commonBegin;
					command.GetParameter( "descriptionE" ).Value = commonBegin;
					command.GetParameter( "changeStatus" ).Value = -1;
					command.ExecuteNonQuery();

					long parentId = GetLastInsertedId();
					foreach ( var scene in group ) {
						command.GetParameter( "type" ).Value = groupType;
						command.GetParameter( "sceneGroup" ).Value = groupNumber;
						command.GetParameter( "parent" ).Value = parentId;
						command.GetParameter( "episodeId" ).Value = scene.EpisodeId;
						command.GetParameter( "descriptionJ" ).Value = scene.HumanReadableNameWithoutPrefix( commonBegin );
						command.GetParameter( "descriptionE" ).Value = scene.HumanReadableNameWithoutPrefix( commonBegin );
						object chst = SqliteUtil.SelectScalar( transaction, "SELECT MAX(changeStatus) FROM ScenarioDat WHERE episodeId = ? AND type != ?", new object[2] { scene.EpisodeId, (int)TextboxType.Information } );
						command.GetParameter( "changeStatus" ).Value = chst == null ? -1L : chst == System.DBNull.Value ? -1L : chst;
						command.ExecuteNonQuery();

						long sceneId = GetLastInsertedId();
						foreach ( var skit in scene.Skits ) {
							var name = Site.InGameIdDict[skit.StringDicIdName];
							command.GetParameter( "type" ).Value = groupType;
							command.GetParameter( "sceneGroup" ).Value = groupNumber;
							command.GetParameter( "parent" ).Value = sceneId;
							command.GetParameter( "episodeId" ).Value = skit.RefString;
							command.GetParameter( "descriptionJ" ).Value = name.GetStringHtml( 0, version, Site.InGameIdDict );
							command.GetParameter( "descriptionE" ).Value = name.GetStringHtml( 1, version, Site.InGameIdDict );
							chst = SqliteUtil.SelectScalar( transaction, "SELECT MAX(changeStatus) FROM SkitText WHERE skitId = ?", new object[1] { skit.RefString } );
							command.GetParameter( "changeStatus" ).Value = chst == null ? -1L : chst == System.DBNull.Value ? -1L : chst;
							command.ExecuteNonQuery();
						}
					}

					++groupNumber;
				}
			}
		}

		private void ExportSkitMetadata() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE SkitMeta ( id INTEGER PRIMARY KEY AUTOINCREMENT, skitId VARCHAR(8), flagTrigger INT, flagCancel INT, category INT, categoryStr TEXT, "
						+ "skitFlag INT, skitFlagUnique INT, characterBitmask INT, jpName TEXT, enName TEXT, jpCond TEXT, enCond TEXT, changeStatus INT, charHtml TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX SkitMeta_SkitId_Index ON SkitMeta ( skitId )";
					command.ExecuteNonQuery();
				}

				Dictionary<string, long> changeStatus = new Dictionary<string, long>();
				List<TO8CHLI.SkitInfo> skits = new List<TO8CHLI.SkitInfo>( Site.Skits.SkitInfoList );
				skits.Sort();
				foreach ( var skit in skits ) {
					object o = SqliteUtil.SelectScalar( transaction, "SELECT MAX(changeStatus) FROM SkitText WHERE skitId = ?", new object[1] { skit.RefString } );
					if ( o == null || o == System.DBNull.Value ) { o = -1L; }
					changeStatus.Add( skit.RefString, (long)o );
				}

				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO SkitMeta ( skitId, flagTrigger, flagCancel, category, categoryStr, skitFlag, skitFlagUnique, characterBitmask, jpName,"
						+ " enName, jpCond, enCond, changeStatus, charHtml ) VALUES ( @skitId, @flagTrigger, @flagCancel, @category, @categoryStr, @skitFlag, @skitFlagUnique,"
						+ " @characterBitmask, @jpName, @enName, @jpCond, @enCond, @changeStatus, @charHtml )";

					command.AddParameter( "skitId" );
					command.AddParameter( "flagTrigger" );
					command.AddParameter( "flagCancel" );
					command.AddParameter( "category" );
					command.AddParameter( "categoryStr" );
					command.AddParameter( "skitFlag" );
					command.AddParameter( "skitFlagUnique" );
					command.AddParameter( "characterBitmask" );
					command.AddParameter( "jpName" );
					command.AddParameter( "enName" );
					command.AddParameter( "jpCond" );
					command.AddParameter( "enCond" );
					command.AddParameter( "changeStatus" );
					command.AddParameter( "charHtml" );

					foreach ( var skit in skits ) {
						command.GetParameter( "skitId" ).Value = skit.RefString;
						command.GetParameter( "flagTrigger" ).Value = skit.FlagTrigger;
						command.GetParameter( "flagCancel" ).Value = skit.FlagCancel;
						command.GetParameter( "category" ).Value = skit.Category;
						command.GetParameter( "categoryStr" ).Value = skit.CategoryString;
						command.GetParameter( "skitFlag" ).Value = skit.SkitFlag;
						command.GetParameter( "skitFlagUnique" ).Value = skit.SkitFlagUnique;
						command.GetParameter( "characterBitmask" ).Value = skit.CharacterBitmask;
						command.GetParameter( "jpName" ).Value = Site.InGameIdDict[skit.StringDicIdName].StringJpnHtml( Site.Version, Site.InGameIdDict );
						command.GetParameter( "jpCond" ).Value = Site.InGameIdDict[skit.StringDicIdCondition].StringJpnHtml( Site.Version, Site.InGameIdDict );
						command.GetParameter( "enName" ).Value = Site.InGameIdDict[skit.StringDicIdName].StringEngHtml( Site.Version, Site.InGameIdDict );
						command.GetParameter( "enCond" ).Value = Site.InGameIdDict[skit.StringDicIdCondition].StringEngHtml( Site.Version, Site.InGameIdDict );
						command.GetParameter( "changeStatus" ).Value = changeStatus[skit.RefString];
						StringBuilder sb = new StringBuilder();
						Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, Site.InGameIdDict, Site.Version, skit.CharacterBitmask );
						command.GetParameter( "charHtml" ).Value = sb.ToString();
						command.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
		}

		private void ExportStringDic() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE StringDic ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT, jpText TEXT, enText TEXT, jpSearchKanji TEXT, jpSearchFuri TEXT, enSearch TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE INDEX StringDic_GameId_Index ON StringDic ( gameId )";
					command.ExecuteNonQuery();
				}

				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO StringDic ( gameId, jpText, enText, jpSearchKanji, jpSearchFuri, enSearch ) "
						+ "VALUES ( @gameId, @jpText, @enText, @jpSearchKanji, @jpSearchFuri, @enSearch )";
					command.AddParameter( "gameId" );
					command.AddParameter( "jpText" );
					command.AddParameter( "enText" );
					command.AddParameter( "jpSearchKanji" );
					command.AddParameter( "jpSearchFuri" );
					command.AddParameter( "enSearch" );

					foreach ( var e in Site.StringDic.Entries ) {
						if ( e.inGameStringId > -1 && e.StringJpn != null ) {
							string jp = e.StringJpnHtml( Site.Version, Site.InGameIdDict );
							string en = e.StringEngHtml( Site.Version, Site.InGameIdDict );

							command.GetParameter( "gameId" ).Value = e.inGameStringId;
							command.GetParameter( "jpText" ).Value = jp;
							command.GetParameter( "enText" ).Value = en;
							command.GetParameter( "jpSearchKanji" ).Value = CleanStringForSearch( e.StringJpn, true, false );
							command.GetParameter( "jpSearchFuri" ).Value = CleanStringForSearch( e.StringJpn, true, true );
							command.GetParameter( "enSearch" ).Value = CleanStringForSearch( e.StringEng, false, false );
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
						+ "fatalStrikeType INT, usableInMenu INT, fire INT, earth INT, wind INT, water INT, light INT, dark INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT, jpSearchKanji TEXT, jpSearchFuri TEXT, enSearch TEXT )";
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
						+ "usableInMenu, fire, earth, wind, water, light, dark, htmlJ, htmlE, htmlCJ, htmlCE, jpSearchKanji, jpSearchFuri, enSearch ) "
						+ "VALUES ( @id, @gameId, @refString, @strDicName, @strDicDesc, @type, @character, @tpUsage, @fatalStrikeType, "
						+ "@usableInMenu, @fire, @earth, @wind, @water, @light, @dark, @htmlJ, @htmlE, @htmlCJ, @htmlCE, @jpSearchKanji, @jpSearchFuri, @enSearch )";
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
					commandArte.AddParameter( "htmlJ" );
					commandArte.AddParameter( "htmlE" );
					commandArte.AddParameter( "htmlCJ" );
					commandArte.AddParameter( "htmlCE" );
					commandArte.AddParameter( "jpSearchKanji" );
					commandArte.AddParameter( "jpSearchFuri" );
					commandArte.AddParameter( "enSearch" );

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
						commandArte.GetParameter( "htmlJ" ).Value = arte.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp, Site.Artes.ArteIdDict, Site.Enemies, Site.Skills, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						commandArte.GetParameter( "htmlE" ).Value = arte.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En, Site.Artes.ArteIdDict, Site.Enemies, Site.Skills, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						commandArte.GetParameter( "htmlCJ" ).Value = arte.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.Artes.ArteIdDict, Site.Enemies, Site.Skills, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						commandArte.GetParameter( "htmlCE" ).Value = arte.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.Artes.ArteIdDict, Site.Enemies, Site.Skills, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						commandArte.GetParameter( "jpSearchKanji" ).Value = CleanStringForSearch( Site.InGameIdDict[arte.NameStringDicId].StringJpn, true, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[arte.DescStringDicId].StringJpn, true, false );
						commandArte.GetParameter( "jpSearchFuri" ).Value = CleanStringForSearch( Site.InGameIdDict[arte.NameStringDicId].StringJpn, true, true ) + "\n" + CleanStringForSearch( Site.InGameIdDict[arte.DescStringDicId].StringJpn, true, true );
						commandArte.GetParameter( "enSearch" ).Value = CleanStringForSearch( Site.InGameIdDict[arte.NameStringDicId].StringEng, false, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[arte.DescStringDicId].StringEng, false, false );
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
						+ "learnableBy INT, equipCost INT, learnCost INT, category INT, symbolValue INT, inactive INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT, jpSearchKanji TEXT, jpSearchFuri TEXT, enSearch TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Skills ( id, gameId, refString, strDicName, strDicDesc, learnableBy, equipCost, learnCost, category, "
						+ "symbolValue, inactive, htmlJ, htmlE, htmlCJ, htmlCE, jpSearchKanji, jpSearchFuri, enSearch ) VALUES ( @id, @gameId, @refString, @strDicName, @strDicDesc, @learnableBy, @equipCost, @learnCost, "
						+ "@category, @symbolValue, @inactive, @htmlJ, @htmlE, @htmlCJ, @htmlCE, @jpSearchKanji, @jpSearchFuri, @enSearch )";
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
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );
					command.AddParameter( "jpSearchKanji" );
					command.AddParameter( "jpSearchFuri" );
					command.AddParameter( "enSearch" );

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
						command.GetParameter( "htmlJ"  ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             ,  Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlE"  ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlCJ" ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlCE" ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "jpSearchKanji" ).Value = CleanStringForSearch( Site.InGameIdDict[s.NameStringDicID].StringJpn, true, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[s.DescStringDicID].StringJpn, true, false );
						command.GetParameter( "jpSearchFuri" ).Value = CleanStringForSearch( Site.InGameIdDict[s.NameStringDicID].StringJpn, true, true ) + "\n" + CleanStringForSearch( Site.InGameIdDict[s.DescStringDicID].StringJpn, true, true );
						command.GetParameter( "enSearch" ).Value = CleanStringForSearch( Site.InGameIdDict[s.NameStringDicID].StringEng, false, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[s.DescStringDicID].StringEng, false, false );
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
						+ "category INT, characters INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE StrategySet ( id INTEGER PRIMARY KEY AUTOINCREMENT, refString TEXT, strDicName INT, strDicDesc INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE StrategySetDefaults ( id INTEGER PRIMARY KEY AUTOINCREMENT, strategySetId INT, character INT, category INT, optionId INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO StrategyOptions ( id, gameId, refString, strDicName, strDicDesc, category, characters, htmlJ, htmlE, htmlCJ, htmlCE ) "
						+ "VALUES ( @id, @gameId, @refString, @strDicName, @strDicDesc, @category, @characters, @htmlJ, @htmlE, @htmlCJ, @htmlCE )";
					command.AddParameter( "id" );
					command.AddParameter( "gameId" );
					command.AddParameter( "refString" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "category" );
					command.AddParameter( "characters" );
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );

					foreach ( var so in Site.Strategy.StrategyOptionList ) {
						command.GetParameter( "id" ).Value = so.ID;
						command.GetParameter( "gameId" ).Value = so.InGameID;
						command.GetParameter( "refString" ).Value = so.RefString;
						command.GetParameter( "strDicName" ).Value = so.NameStringDicID;
						command.GetParameter( "strDicDesc" ).Value = so.DescStringDicID;
						command.GetParameter( "category" ).Value = so.Category;
						command.GetParameter( "characters" ).Value = so.Characters;
						command.GetParameter( "htmlJ"  ).Value = so.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlE"  ).Value = so.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlCJ" ).Value = so.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlCE" ).Value = so.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.StringDic, Site.InGameIdDict );
						command.ExecuteNonQuery();
					}
				}
				using ( var commandSet = DB.CreateCommand() )
				using ( var commandDefault = DB.CreateCommand() ) {
					commandSet.CommandText = "INSERT INTO StrategySet ( id, refString, strDicName, strDicDesc, htmlJ, htmlE, htmlCJ, htmlCE ) "
						+ "VALUES ( @id, @refString, @strDicName, @strDicDesc, @htmlJ, @htmlE, @htmlCJ, @htmlCE )";
					commandSet.AddParameter( "id" );
					commandSet.AddParameter( "refString" );
					commandSet.AddParameter( "strDicName" );
					commandSet.AddParameter( "strDicDesc" );
					commandSet.AddParameter( "htmlJ" );
					commandSet.AddParameter( "htmlE" );
					commandSet.AddParameter( "htmlCJ" );
					commandSet.AddParameter( "htmlCE" );

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
						commandSet.GetParameter( "htmlJ"  ).Value = ss.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , Site.Strategy, Site.StringDic, Site.InGameIdDict );
						commandSet.GetParameter( "htmlE"  ).Value = ss.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.Strategy, Site.StringDic, Site.InGameIdDict );
						commandSet.GetParameter( "htmlCJ" ).Value = ss.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.Strategy, Site.StringDic, Site.InGameIdDict );
						commandSet.GetParameter( "htmlCE" ).Value = ss.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.Strategy, Site.StringDic, Site.InGameIdDict );
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
						+ "charLike INT, charHate INT, charGood INT, charBad INT, hp INT, tp INT, death INT, ailment INT, statType INT, statValue INT, statTime INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT, jpSearchKanji TEXT, jpSearchFuri TEXT, enSearch TEXT )";
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
						+ "death, ailment, statType, statValue, statTime, htmlJ, htmlE, htmlCJ, htmlCE, jpSearchKanji, jpSearchFuri, enSearch ) VALUES ( @id, @refString, @strDicName, @strDicDesc, @strDicEffect, @charLike, @charHate, "
						+ "@charGood, @charBad, @hp, @tp, @death, @ailment, @statType, @statValue, @statTime, @htmlJ, @htmlE, @htmlCJ, @htmlCE, @jpSearchKanji, @jpSearchFuri, @enSearch )";
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
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );
					command.AddParameter( "jpSearchKanji" );
					command.AddParameter( "jpSearchFuri" );
					command.AddParameter( "enSearch" );

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
						command.GetParameter( "htmlJ"  ).Value = r.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , Site.Recipes, Site.Items, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "htmlE"  ).Value = r.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.Recipes, Site.Items, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "htmlCJ" ).Value = r.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.Recipes, Site.Items, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "htmlCE" ).Value = r.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.Recipes, Site.Items, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "jpSearchKanji" ).Value = CleanStringForSearch( Site.InGameIdDict[r.NameStringDicID].StringJpn, true, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[r.DescriptionStringDicID].StringJpn, true, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[r.EffectStringDicID].StringJpn, true, false );
						command.GetParameter( "jpSearchFuri" ).Value = CleanStringForSearch( Site.InGameIdDict[r.NameStringDicID].StringJpn, true, true ) + "\n" + CleanStringForSearch( Site.InGameIdDict[r.DescriptionStringDicID].StringJpn, true, true ) + "\n" + CleanStringForSearch( Site.InGameIdDict[r.EffectStringDicID].StringJpn, true, true );
						command.GetParameter( "enSearch" ).Value = CleanStringForSearch( Site.InGameIdDict[r.NameStringDicID].StringEng, false, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[r.DescriptionStringDicID].StringEng, false, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[r.EffectStringDicID].StringEng, false, false );
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
					command.CommandText = "CREATE TABLE Shops ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, strDicName INT, changesToShop INT, onTrigger INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Shops_Items ( id INTEGER PRIMARY KEY AUTOINCREMENT, shopId INT, itemId INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Shops ( gameId, strDicName, changesToShop, onTrigger, htmlJ, htmlE, htmlCJ, htmlCE ) VALUES ( @gameId, @strDicName, @changesToShop, @onTrigger, @htmlJ, @htmlE, @htmlCJ, @htmlCE )";
					command.AddParameter( "gameId" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "changesToShop" );
					command.AddParameter( "onTrigger" );
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );

					foreach ( var s in Site.Shops.ShopDefinitions ) {
						command.GetParameter( "gameId" ).Value = s.InGameID;
						command.GetParameter( "strDicName" ).Value = s.StringDicID;
						command.GetParameter( "changesToShop" ).Value = s.ChangeToShop;
						command.GetParameter( "onTrigger" ).Value = s.OnTrigger;
						command.GetParameter( "htmlJ"  ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , Site.Items, Site.Shops, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "htmlE"  ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.Items, Site.Shops, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "htmlCJ" ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.Items, Site.Shops, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "htmlCE" ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.Items, Site.Shops, Site.InGameIdDict, phpLinks: true );
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
						+ "points INT, model TEXT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT, jpSearchKanji TEXT, jpSearchFuri TEXT, enSearch TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Titles ( gameId, strDicName, strDicDesc, character, points, model, htmlJ, htmlE, htmlCJ, htmlCE, jpSearchKanji, jpSearchFuri, enSearch ) "
						+ "VALUES ( @gameId, @strDicName, @strDicDesc, @character, @points, @model, @htmlJ, @htmlE, @htmlCJ, @htmlCE, @jpSearchKanji, @jpSearchFuri, @enSearch )";
					command.AddParameter( "gameId" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "character" );
					command.AddParameter( "points" );
					command.AddParameter( "model" );
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );
					command.AddParameter( "jpSearchKanji" );
					command.AddParameter( "jpSearchFuri" );
					command.AddParameter( "enSearch" );

					foreach ( var t in Site.Titles.TitleList ) {
						command.GetParameter( "gameId" ).Value = t.ID;
						command.GetParameter( "strDicName" ).Value = t.NameStringDicID;
						command.GetParameter( "strDicDesc" ).Value = t.DescStringDicID;
						command.GetParameter( "character" ).Value = t.Character;
						command.GetParameter( "points" ).Value = t.BunnyGuildPointsMaybe;
						command.GetParameter( "model" ).Value = t.CostumeString;
						command.GetParameter( "htmlJ"  ).Value = t.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlE"  ).Value = t.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlCJ" ).Value = t.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlCE" ).Value = t.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "jpSearchKanji" ).Value = CleanStringForSearch( Site.InGameIdDict[t.NameStringDicID].StringJpn, true, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[t.DescStringDicID].StringJpn, true, false );
						command.GetParameter( "jpSearchFuri" ).Value = CleanStringForSearch( Site.InGameIdDict[t.NameStringDicID].StringJpn, true, true ) + "\n" + CleanStringForSearch( Site.InGameIdDict[t.DescStringDicID].StringJpn, true, true );
						command.GetParameter( "enSearch" ).Value = CleanStringForSearch( Site.InGameIdDict[t.NameStringDicID].StringEng, false, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[t.DescStringDicID].StringEng, false, false );
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
						+ "refString TEXT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT, jpSearchKanji TEXT, jpSearchFuri TEXT, enSearch TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Synopsis ( id, strDicName, strDicDesc, storyMin, storyMax, image, refString, htmlJ, htmlE, htmlCJ, htmlCE, jpSearchKanji, jpSearchFuri, enSearch ) "
						+ "VALUES ( @id, @strDicName, @strDicDesc, @storyMin, @storyMax, @image, @refString, @htmlJ, @htmlE, @htmlCJ, @htmlCE, @jpSearchKanji, @jpSearchFuri, @enSearch )";
					command.AddParameter( "id" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "storyMin" );
					command.AddParameter( "storyMax" );
					command.AddParameter( "image" );
					command.AddParameter( "refString" );
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );
					command.AddParameter( "jpSearchKanji" );
					command.AddParameter( "jpSearchFuri" );
					command.AddParameter( "enSearch" );

					foreach ( var s in Site.Synopsis.SynopsisList ) {
						command.GetParameter( "id" ).Value = s.ID;
						command.GetParameter( "strDicName" ).Value = s.NameStringDicId;
						command.GetParameter( "strDicDesc" ).Value = s.TextStringDicId;
						command.GetParameter( "storyMin" ).Value = s.StoryIdMin;
						command.GetParameter( "storyMax" ).Value = s.StoryIdMax;
						command.GetParameter( "image" ).Value = s.RefString1;
						command.GetParameter( "refString" ).Value = s.RefString2;
						command.GetParameter( "htmlJ"  ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlE"  ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlCJ" ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlCE" ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "jpSearchKanji" ).Value = CleanStringForSearch( Site.InGameIdDict[s.NameStringDicId].StringJpn, true, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[s.TextStringDicId].StringJpn, true, false );
						command.GetParameter( "jpSearchFuri" ).Value = CleanStringForSearch( Site.InGameIdDict[s.NameStringDicId].StringJpn, true, true ) + "\n" + CleanStringForSearch( Site.InGameIdDict[s.TextStringDicId].StringJpn, true, true );
						command.GetParameter( "enSearch" ).Value = CleanStringForSearch( Site.InGameIdDict[s.NameStringDicId].StringEng, false, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[s.TextStringDicId].StringEng, false, false );
						command.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
		}

		public void ExportBattleBook() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE BattleBook ( id INTEGER PRIMARY KEY AUTOINCREMENT, strDicName INT, strDicDesc INT, unlock INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT, jpSearchKanji TEXT, jpSearchFuri TEXT, enSearch TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO BattleBook ( strDicName, strDicDesc, unlock, htmlJ, htmlE, htmlCJ, htmlCE, jpSearchKanji, jpSearchFuri, enSearch ) "
						+ "VALUES ( @strDicName, @strDicDesc, @unlock, @htmlJ, @htmlE, @htmlCJ, @htmlCE, @jpSearchKanji, @jpSearchFuri, @enSearch )";
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "unlock" );
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );
					command.AddParameter( "jpSearchKanji" );
					command.AddParameter( "jpSearchFuri" );
					command.AddParameter( "enSearch" );

					for ( int i = 0; i < Site.BattleBook.BattleBookEntryList.Count; ++i ) {
						var b = Site.BattleBook.BattleBookEntryList[i];
						command.GetParameter( "strDicName" ).Value = b.NameStringDicId;
						command.GetParameter( "strDicDesc" ).Value = b.TextStringDicId;
						command.GetParameter( "unlock" ).Value = b.UnlockReferenceMaybe;
						command.GetParameter( "htmlJ"  ).Value = b.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlE"  ).Value = b.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlCJ" ).Value = b.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlCE" ).Value = b.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "jpSearchKanji" ).Value = CleanStringForSearch( Site.InGameIdDict[b.NameStringDicId].StringJpn, true, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[b.TextStringDicId].StringJpn, true, false );
						command.GetParameter( "jpSearchFuri" ).Value = CleanStringForSearch( Site.InGameIdDict[b.NameStringDicId].StringJpn, true, true ) + "\n" + CleanStringForSearch( Site.InGameIdDict[b.TextStringDicId].StringJpn, true, true );
						command.GetParameter( "enSearch" ).Value = CleanStringForSearch( Site.InGameIdDict[b.NameStringDicId].StringEng, false, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[b.TextStringDicId].StringEng, false, false );
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
						+ "fatalExpType INT, fatalExpModifier INT, fatalLpType INT, fatalLpModifier INT, fatalDropType INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT, jpSearchKanji TEXT, jpSearchFuri TEXT, enSearch TEXT )";
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
						+ "fatalLpType, fatalLpModifier, fatalDropType, htmlJ, htmlE, htmlCJ, htmlCE, jpSearchKanji, jpSearchFuri, enSearch ) VALUES ( @id, @gameId, @strDicName, @refString, @icon, @category, @level, @hp, @tp, @pAtk, "
						+ "@pDef, @mAtk, @mDef, @agl, @attrFire, @attrEarth, @attrWind, @attrWater, @attrLight, @attrDark, @attrPhys, @exp, @lp, @gald, @fatalBlueResist, "
						+ "@fatalRedResist, @fatalGreenResist, @inMonsterBook, @location, @locationWeather, @stealItem, @stealChance, @killableWithFatal, @secretMissionDrop, "
						+ "@secretMissionDropChance, @fatalExpType, @fatalExpModifier, @fatalLpType, @fatalLpModifier, @fatalDropType, @htmlJ, @htmlE, @htmlCJ, @htmlCE, @jpSearchKanji, @jpSearchFuri, @enSearch )";
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
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );
					command.AddParameter( "jpSearchKanji" );
					command.AddParameter( "jpSearchFuri" );
					command.AddParameter( "enSearch" );

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
						command.GetParameter( "htmlJ"  ).Value = e.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , Site.Items, Site.Locations, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "htmlE"  ).Value = e.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.Items, Site.Locations, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "htmlCJ" ).Value = e.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.Items, Site.Locations, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "htmlCE" ).Value = e.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.Items, Site.Locations, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "jpSearchKanji" ).Value = CleanStringForSearch( Site.InGameIdDict[e.NameStringDicID].StringJpn, true, false );
						command.GetParameter( "jpSearchFuri" ).Value = CleanStringForSearch( Site.InGameIdDict[e.NameStringDicID].StringJpn, true, true );
						command.GetParameter( "enSearch" ).Value = CleanStringForSearch( Site.InGameIdDict[e.NameStringDicID].StringEng, false, false );
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
						+ "equipLuck INT, attrFire INT, attrWater INT, attrWind INT, attrEarth INT, attrLight INT, attrDark INT, price INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT, jpSearchKanji TEXT, jpSearchFuri TEXT, enSearch TEXT )";
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
						+ "equipMDef, equipAgl, equipLuck, attrFire, attrWater, attrWind, attrEarth, attrLight, attrDark, price, htmlJ, htmlE, htmlCJ, htmlCE, jpSearchKanji, jpSearchFuri, enSearch ) VALUES ( @gameId, @image, @equipBy, "
						+ "@strDicName, @strDicDesc, @icon, @usableInBattle, @inCollectorsBook, @category, @hpHeal, @tpHeal, @ailmentPhys, @ailmentMag, @permaPAtk, "
						+ "@permaPDef, @permaMAtk, @permaMDef, @permaAgl, @permaHp, @permaTp, @equipPAtk, @equipMAtk, @equipPDef, @equipMDef, @equipAgl, @equipLuck, "
						+ "@attrFire, @attrWater, @attrWind, @attrEarth, @attrLight, @attrDark, @price, @htmlJ, @htmlE, @htmlCJ, @htmlCE, @jpSearchKanji, @jpSearchFuri, @enSearch )";
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
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );
					command.AddParameter( "jpSearchKanji" );
					command.AddParameter( "jpSearchFuri" );
					command.AddParameter( "enSearch" );

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
						command.GetParameter( "htmlJ"  ).Value = ItemDat.ItemDat.GetItemDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , Site.Items, item, Site.Skills, Site.Enemies, Site.Recipes, Site.Locations, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "htmlE"  ).Value = ItemDat.ItemDat.GetItemDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.Items, item, Site.Skills, Site.Enemies, Site.Recipes, Site.Locations, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "htmlCJ" ).Value = ItemDat.ItemDat.GetItemDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.Items, item, Site.Skills, Site.Enemies, Site.Recipes, Site.Locations, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "htmlCE" ).Value = ItemDat.ItemDat.GetItemDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.Items, item, Site.Skills, Site.Enemies, Site.Recipes, Site.Locations, Site.StringDic, Site.InGameIdDict, phpLinks: true );
						command.GetParameter( "jpSearchKanji" ).Value = CleanStringForSearch( Site.InGameIdDict[item.Data[(int)ItemDat.ItemData.NamePointer]].StringJpn, true, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[item.Data[(int)ItemDat.ItemData.DescriptionPointer]].StringJpn, true, false );
						command.GetParameter( "jpSearchFuri" ).Value = CleanStringForSearch( Site.InGameIdDict[item.Data[(int)ItemDat.ItemData.NamePointer]].StringJpn, true, true ) + "\n" + CleanStringForSearch( Site.InGameIdDict[item.Data[(int)ItemDat.ItemData.DescriptionPointer]].StringJpn, true, true );
						command.GetParameter( "enSearch" ).Value = CleanStringForSearch( Site.InGameIdDict[item.Data[(int)ItemDat.ItemData.NamePointer]].StringEng, false, false ) + "\n" + CleanStringForSearch( Site.InGameIdDict[item.Data[(int)ItemDat.ItemData.DescriptionPointer]].StringEng, false, false );

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
					// TODO: Make searchable by splitting into individual panels sensibly...?
					command.CommandText = "CREATE TABLE Locations ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, strDicName INT, category INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT )";
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
					command.CommandText = "INSERT INTO Locations ( gameId, strDicName, category, htmlJ, htmlE, htmlCJ, htmlCE ) VALUES ( @gameId, @strDicName, @category, @htmlJ, @htmlE, @htmlCJ, @htmlCE )";
					command.AddParameter( "gameId" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "category" );
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );

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
						command.GetParameter( "htmlJ"  ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , Site.StringDic, Site.InGameIdDict, Site.EncounterGroups, Site.EnemyGroups, Site.Enemies, Site.Shops, phpLinks: true );
						command.GetParameter( "htmlE"  ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.StringDic, Site.InGameIdDict, Site.EncounterGroups, Site.EnemyGroups, Site.Enemies, Site.Shops, phpLinks: true );
						command.GetParameter( "htmlCJ" ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.StringDic, Site.InGameIdDict, Site.EncounterGroups, Site.EnemyGroups, Site.Enemies, Site.Shops, phpLinks: true );
						command.GetParameter( "htmlCE" ).Value = s.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.StringDic, Site.InGameIdDict, Site.EncounterGroups, Site.EnemyGroups, Site.Enemies, Site.Shops, phpLinks: true );
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
					command.CommandText = "CREATE TABLE Records ( id INTEGER PRIMARY KEY AUTOINCREMENT, strDicId INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Records ( strDicId, htmlJ, htmlE, htmlCJ, htmlCE ) "
						+ "VALUES ( @strDicId, @htmlJ, @htmlE, @htmlCJ, @htmlCE )";
					command.AddParameter( "strDicId" );
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );

					foreach ( uint r in Site.Records ) {
						command.GetParameter( "strDicId" ).Value = r;

						StringBuilder htmlJ = new StringBuilder();
						StringBuilder htmlE = new StringBuilder();
						StringBuilder htmlCJ = new StringBuilder();
						StringBuilder htmlCE = new StringBuilder();
						WebsiteGenerator.AppendRecord( htmlJ,  Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp, Site.InGameIdDict, r );
						WebsiteGenerator.AppendRecord( htmlE,  Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En, Site.InGameIdDict, r );
						WebsiteGenerator.AppendRecord( htmlCJ, Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.InGameIdDict, r );
						WebsiteGenerator.AppendRecord( htmlCE, Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.InGameIdDict, r );

						command.GetParameter( "htmlJ" ).Value = htmlJ.ToString();
						command.GetParameter( "htmlE" ).Value = htmlE.ToString();
						command.GetParameter( "htmlCJ" ).Value = htmlCJ.ToString();
						command.GetParameter( "htmlCE" ).Value = htmlCE.ToString();
						command.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
		}

		public void ExportSettings() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Settings ( id INTEGER PRIMARY KEY AUTOINCREMENT, strDicName INT, strDicDesc INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Settings_Options ( id INTEGER PRIMARY KEY AUTOINCREMENT, settingId INT, strDicId INT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() )
				using ( var commandOpt = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Settings ( strDicName, strDicDesc, htmlJ, htmlE, htmlCJ, htmlCE ) VALUES ( @strDicName, @strDicDesc, @htmlJ, @htmlE, @htmlCJ, @htmlCE )";
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );
					commandOpt.CommandText = "INSERT INTO Settings_Options ( settingId, strDicId ) VALUES ( @settingId, @strDicId )";
					commandOpt.AddParameter( "settingId" );
					commandOpt.AddParameter( "strDicId" );

					foreach ( var s in Site.Settings ) {
						command.GetParameter( "strDicName" ).Value = s.NameStringDicId;
						command.GetParameter( "strDicDesc" ).Value = s.DescStringDicId;

						StringBuilder htmlJ = new StringBuilder();
						StringBuilder htmlE = new StringBuilder();
						StringBuilder htmlCJ = new StringBuilder();
						StringBuilder htmlCE = new StringBuilder();

						WebsiteGenerator.AppendSetting( htmlJ, Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp, Site.InGameIdDict, s.NameStringDicId, s.DescStringDicId, s.OptionsStringDicIds[0], s.OptionsStringDicIds[1], s.OptionsStringDicIds[2], s.OptionsStringDicIds[3] );
						WebsiteGenerator.AppendSetting( htmlE, Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En, Site.InGameIdDict, s.NameStringDicId, s.DescStringDicId, s.OptionsStringDicIds[0], s.OptionsStringDicIds[1], s.OptionsStringDicIds[2], s.OptionsStringDicIds[3] );
						WebsiteGenerator.AppendSetting( htmlCJ, Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.InGameIdDict, s.NameStringDicId, s.DescStringDicId, s.OptionsStringDicIds[0], s.OptionsStringDicIds[1], s.OptionsStringDicIds[2], s.OptionsStringDicIds[3] );
						WebsiteGenerator.AppendSetting( htmlCE, Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.InGameIdDict, s.NameStringDicId, s.DescStringDicId, s.OptionsStringDicIds[0], s.OptionsStringDicIds[1], s.OptionsStringDicIds[2], s.OptionsStringDicIds[3] );

						command.GetParameter( "htmlJ" ).Value = htmlJ.ToString();
						command.GetParameter( "htmlE" ).Value = htmlE.ToString();
						command.GetParameter( "htmlCJ" ).Value = htmlCJ.ToString();
						command.GetParameter( "htmlCE" ).Value = htmlCE.ToString();
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
						+ "refString TEXT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO GradeShop ( id, gameId, strDicName, strDicDesc, cost, refString, htmlJ, htmlE, htmlCJ, htmlCE ) "
						+ "VALUES ( @id, @gameId, @strDicName, @strDicDesc, @cost, @refString, @htmlJ, @htmlE, @htmlCJ, @htmlCE )";
					command.AddParameter( "id" );
					command.AddParameter( "gameId" );
					command.AddParameter( "strDicName" );
					command.AddParameter( "strDicDesc" );
					command.AddParameter( "cost" );
					command.AddParameter( "refString" );
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );

					foreach ( var g in Site.GradeShop.GradeShopEntryList ) {
						command.GetParameter( "id" ).Value = g.ID;
						command.GetParameter( "gameId" ).Value = g.InGameID;
						command.GetParameter( "strDicName" ).Value = g.NameStringDicID;
						command.GetParameter( "strDicDesc" ).Value = g.DescStringDicID;
						command.GetParameter( "cost" ).Value = g.GradeCost;
						command.GetParameter( "refString" ).Value = g.RefString;
						command.GetParameter( "htmlJ"  ).Value = g.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlE"  ).Value = g.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlCJ" ).Value = g.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.StringDic, Site.InGameIdDict );
						command.GetParameter( "htmlCE" ).Value = g.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.StringDic, Site.InGameIdDict );
						command.ExecuteNonQuery();

					}
				}
				transaction.Commit();
			}
		}

		public void ExportSearchPoints() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE SearchPoints ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId INT UNIQUE, displayId INT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO SearchPoints ( gameId, displayId, htmlJ, htmlE, htmlCJ, htmlCE ) VALUES ( @gameId, @displayId, @htmlJ, @htmlE, @htmlCJ, @htmlCE )";
					command.AddParameter( "gameId" );
					command.AddParameter( "displayId" );
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );

					int idx = 1;
					foreach ( var sp in Site.SearchPoints.SearchPointDefinitions ) {
						command.GetParameter( "gameId" ).Value = sp.Index;
						command.GetParameter( "displayId" ).Value = sp.Unknown11 == 1 ? idx : -1;
						command.GetParameter( "htmlJ"  ).Value = sp.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , Site.Items, Site.StringDic, Site.InGameIdDict, Site.SearchPoints.SearchPointContents, Site.SearchPoints.SearchPointItems, sp.Unknown11 == 1 ? idx : -1, phpLinks: true );
						command.GetParameter( "htmlE"  ).Value = sp.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , Site.Items, Site.StringDic, Site.InGameIdDict, Site.SearchPoints.SearchPointContents, Site.SearchPoints.SearchPointItems, sp.Unknown11 == 1 ? idx : -1, phpLinks: true );
						command.GetParameter( "htmlCJ" ).Value = sp.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.Items, Site.StringDic, Site.InGameIdDict, Site.SearchPoints.SearchPointContents, Site.SearchPoints.SearchPointItems, sp.Unknown11 == 1 ? idx : -1, phpLinks: true );
						command.GetParameter( "htmlCE" ).Value = sp.GetDataAsHtml( Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.Items, Site.StringDic, Site.InGameIdDict, Site.SearchPoints.SearchPointContents, Site.SearchPoints.SearchPointItems, sp.Unknown11 == 1 ? idx : -1, phpLinks: true );
						command.ExecuteNonQuery();
						if ( sp.Unknown11 == 1 ) {
							++idx;
						}
					}
				}
				transaction.Commit();
			}
		}

		public void ExportTrophies() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE Trophies ( id INTEGER PRIMARY KEY AUTOINCREMENT, gameId TEXT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO Trophies ( id, gameId, htmlJ, htmlE, htmlCJ, htmlCE ) "
						+ "VALUES ( @id, @gameId, @htmlJ, @htmlE, @htmlCJ, @htmlCE )";
					command.AddParameter( "id" );
					command.AddParameter( "gameID" );
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );

					foreach ( var kvp in Site.TrophyJp.Trophies ) {
						var jp = Site.TrophyJp.Trophies[kvp.Key];
						var en = Site.TrophyEn?.Trophies[kvp.Key];

						command.GetParameter( "id" ).Value = kvp.Key;
						command.GetParameter( "gameId" ).Value = jp.ID;
						command.GetParameter( "htmlJ"  ).Value = WebsiteGenerator.TrophyNodeToHtml( Site.Version, Site.InGameIdDict, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp             , jp, en );
						command.GetParameter( "htmlE"  ).Value = WebsiteGenerator.TrophyNodeToHtml( Site.Version, Site.InGameIdDict, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En             , jp, en );
						command.GetParameter( "htmlCJ" ).Value = WebsiteGenerator.TrophyNodeToHtml( Site.Version, Site.InGameIdDict, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, jp, en );
						command.GetParameter( "htmlCE" ).Value = WebsiteGenerator.TrophyNodeToHtml( Site.Version, Site.InGameIdDict, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, jp, en );
						command.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
		}

		public void ExportNecropolis() {
			using ( var transaction = DB.BeginTransaction() ) {
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "CREATE TABLE NecropolisFloors ( id INTEGER PRIMARY KEY AUTOINCREMENT, floorName VARCHAR(16) UNIQUE, map TEXT, htmlJ TEXT, htmlE TEXT, htmlCJ TEXT, htmlCE TEXT, htmlEnemyJ TEXT, htmlEnemyE TEXT, htmlEnemyCJ TEXT, htmlEnemyCE TEXT )";
					command.ExecuteNonQuery();
				}
				using ( var command = DB.CreateCommand() ) {
					command.CommandText = "INSERT INTO NecropolisFloors ( floorName, map, htmlJ, htmlE, htmlCJ, htmlCE, htmlEnemyJ, htmlEnemyE, htmlEnemyCJ, htmlEnemyCE ) VALUES ( @floorName, @map, @htmlJ, @htmlE, @htmlCJ, @htmlCE, @htmlEnemyJ, @htmlEnemyE, @htmlEnemyCJ, @htmlEnemyCE )";
					command.AddParameter( "floorName" );
					command.AddParameter( "map" );
					command.AddParameter( "htmlJ" );
					command.AddParameter( "htmlE" );
					command.AddParameter( "htmlCJ" );
					command.AddParameter( "htmlCE" );
					command.AddParameter( "htmlEnemyJ" );
					command.AddParameter( "htmlEnemyE" );
					command.AddParameter( "htmlEnemyCJ" );
					command.AddParameter( "htmlEnemyCE" );

					foreach ( var floor in Site.NecropolisFloors.FloorList ) {
						int floorNumberLong = Int32.Parse( floor.RefString1.Split( '_' ).Last() );
						int floorNumber = ( floorNumberLong - 1 ) % 10 + 1;
						int floorStratumAsNumber = ( floorNumberLong - 1 ) / 10 + 1;
						string floorStratum = ( (char)( floorStratumAsNumber + 64 ) ).ToString();

						command.GetParameter( "floorName" ).Value = floor.RefString1;
						command.GetParameter( "map" ).Value = floor.RefString2;
						command.GetParameter( "htmlJ"  ).Value      = Site.NecropolisMaps[floor.RefString2].GetDataAsHtml( floorStratum, floorNumber, null,         null,             null,                 Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp,              Site.NecropolisTreasures, Site.Items, Site.InGameIdDict, surroundingTable: false, phpLinks: true );
						command.GetParameter( "htmlE"  ).Value      = Site.NecropolisMaps[floor.RefString2].GetDataAsHtml( floorStratum, floorNumber, null,         null,             null,                 Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En,              Site.NecropolisTreasures, Site.Items, Site.InGameIdDict, surroundingTable: false, phpLinks: true );
						command.GetParameter( "htmlCJ" ).Value      = Site.NecropolisMaps[floor.RefString2].GetDataAsHtml( floorStratum, floorNumber, null,         null,             null,                 Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.NecropolisTreasures, Site.Items, Site.InGameIdDict, surroundingTable: false, phpLinks: true );
						command.GetParameter( "htmlCE" ).Value      = Site.NecropolisMaps[floor.RefString2].GetDataAsHtml( floorStratum, floorNumber, null,         null,             null,                 Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.NecropolisTreasures, Site.Items, Site.InGameIdDict, surroundingTable: false, phpLinks: true );
						command.GetParameter( "htmlEnemyJ"  ).Value = Site.NecropolisMaps[floor.RefString2].GetDataAsHtml( floorStratum, floorNumber, Site.Enemies, Site.EnemyGroups, Site.EncounterGroups, Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.Jp,              Site.NecropolisTreasures, Site.Items, Site.InGameIdDict, surroundingTable: false, phpLinks: true );
						command.GetParameter( "htmlEnemyE"  ).Value = Site.NecropolisMaps[floor.RefString2].GetDataAsHtml( floorStratum, floorNumber, Site.Enemies, Site.EnemyGroups, Site.EncounterGroups, Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.En,              Site.NecropolisTreasures, Site.Items, Site.InGameIdDict, surroundingTable: false, phpLinks: true );
						command.GetParameter( "htmlEnemyCJ" ).Value = Site.NecropolisMaps[floor.RefString2].GetDataAsHtml( floorStratum, floorNumber, Site.Enemies, Site.EnemyGroups, Site.EncounterGroups, Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithJpLinks, Site.NecropolisTreasures, Site.Items, Site.InGameIdDict, surroundingTable: false, phpLinks: true );
						command.GetParameter( "htmlEnemyCE" ).Value = Site.NecropolisMaps[floor.RefString2].GetDataAsHtml( floorStratum, floorNumber, Site.Enemies, Site.EnemyGroups, Site.EncounterGroups, Site.Version, Site.VersionPostfix, Site.Locale, WebsiteLanguage.BothWithEnLinks, Site.NecropolisTreasures, Site.Items, Site.InGameIdDict, surroundingTable: false, phpLinks: true );
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
