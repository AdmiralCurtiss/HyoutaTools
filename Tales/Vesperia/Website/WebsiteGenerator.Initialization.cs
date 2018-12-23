using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.Website {
	public partial class WebsiteGenerator {
		public static Dictionary<string, SCFOMBIN.SCFOMBIN> LoadBattleTextTSS( string dir, Util.GameTextEncoding encoding ) {
			var BattleTextFiles = new Dictionary<string, SCFOMBIN.SCFOMBIN>();

			var files = new System.IO.DirectoryInfo( dir ).GetFiles();
			foreach ( var file in files ) {
				if ( file.Name.StartsWith( "BTL_" ) ) {
					var bin = new ScenarioFile.ScenarioFile( System.IO.Path.Combine( dir, file.Name ), encoding );
					var name = file.Name.Split( '.' )[0];

					var btl = new SCFOMBIN.SCFOMBIN();
					btl.EntryList = bin.EntryList;
					BattleTextFiles.Add( name, btl );
				}
			}

			return BattleTextFiles;
		}

		public static Dictionary<string, SCFOMBIN.SCFOMBIN> LoadBattleTextScfombin( string dir, string modDir = null ) {
			var BattleTextFiles = new Dictionary<string, SCFOMBIN.SCFOMBIN>();

			var files = new System.IO.DirectoryInfo( dir ).GetFiles();
			foreach ( var file in files ) {
				if ( file.Name.StartsWith( "BTL_" ) ) {
					uint ptrDiff = 0x1888;
					if ( file.Name.StartsWith( "BTL_XTM" ) ) { ptrDiff = 0x1B4C; }

					var bin = new SCFOMBIN.SCFOMBIN( System.IO.Path.Combine( dir, file.Name ), ptrDiff );
					var name = file.Name.Split( '.' )[0];

					if ( modDir != null ) {
						var modBin = new SCFOMBIN.SCFOMBIN( System.IO.Path.Combine( modDir, file.Name ), ptrDiff );
						for ( int i = 0; i < bin.EntryList.Count; ++i ) {
							bin.EntryList[i].EnName = modBin.EntryList[i].JpName;
							bin.EntryList[i].EnText = modBin.EntryList[i].JpText;
						}
					}

					BattleTextFiles.Add( name, bin );
				}
			}

			return BattleTextFiles;
		}

		public static List<uint> GenerateRecordsStringDicList( GameVersion version ) {
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

			if ( version == GameVersion.PS3 ) {
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
			if ( version == GameVersion.PS3 ) {
				// usage flynn, patty
				records.Add( 33912399u );
				records.Add( 33912400u );
			}

			return records;
		}

		public static List<ConfigMenuSetting> GenerateSettingsStringDicList( GameVersion version ) {
			List<ConfigMenuSetting> settings = new List<ConfigMenuSetting>();

			settings.Add( new ConfigMenuSetting( 33912401u, 33912401u + 46u, 33912427u, 33912426u, 33912425u, 33912424u ) ); // msg speed
			settings.Add( new ConfigMenuSetting( 33912402u, 33912402u + 46u, 33912428u, 33912429u, 33912430u, 33912431u ) ); // difficulty
			if ( version == GameVersion.X360 ) {
				settings.Add( new ConfigMenuSetting( 33912403u, 33912403u + 46u, 33912438u, 33912437u ) ); // x360 vibration
			} else {
				settings.Add( new ConfigMenuSetting( 33912679u, 33912681u, 33912438u, 33912437u ) ); // console-neutral vibration
			}
			settings.Add( new ConfigMenuSetting( 33912404u, 33912404u + 46u, 33912432u, 33912433u ) ); // camera controls
			if ( version == GameVersion.PS3 ) {
				settings.Add( new ConfigMenuSetting( 33912751u, 33912752u, 33912443u, 33912444u ) ); // stick/dpad controls
			}
			settings.Add( new ConfigMenuSetting( 33912405u, 33912405u + 46u, 33912439u ) ); // button config
			settings.Add( new ConfigMenuSetting( 33912406u, 33912406u + 46u, 33912436u, 33912435u, 33912434u ) ); // sound
			settings.Add( new ConfigMenuSetting( 33912407u, 33912407u + 46u ) ); // bgm
			settings.Add( new ConfigMenuSetting( 33912408u, 33912408u + 46u ) ); // se
			settings.Add( new ConfigMenuSetting( 33912409u, 33912409u + 46u ) ); // battle se
			settings.Add( new ConfigMenuSetting( 33912413u, 33912413u + 46u ) ); // battle voice
			settings.Add( new ConfigMenuSetting( 33912414u, 33912414u + 46u ) ); // event voice
			settings.Add( new ConfigMenuSetting( 33912422u, 33912422u + 46u ) ); // skit
			settings.Add( new ConfigMenuSetting( 33912423u, 33912423u + 46u ) ); // movie
			if ( version == GameVersion.PS3 ) {
				settings.Add( new ConfigMenuSetting( 33912656u, 33912657u, 33912658u, 33912659u ) ); // item request type
			}
			settings.Add( new ConfigMenuSetting( 33912410u, 33912410u + 46u, 33912438u, 33912437u ) ); // engage cam
			settings.Add( new ConfigMenuSetting( 33912411u, 33912411u + 46u, 33912438u, 33912437u ) ); // dynamic cam
			settings.Add( new ConfigMenuSetting( 33912412u, 33912412u + 46u, 33912438u, 33912437u ) ); // field boundary
			settings.Add( new ConfigMenuSetting( 33912415u, 33912415u + 46u, 33912438u, 33912437u ) ); // location names
			settings.Add( new ConfigMenuSetting( 33912416u, 33912416u + 46u, 33912438u, 33912437u ) ); // skit titles
			settings.Add( new ConfigMenuSetting( 33912417u, 33912417u + 46u, 33912438u, 33912437u ) ); // skit subs
			settings.Add( new ConfigMenuSetting( 33912418u, 33912418u + 46u, 33912438u, 33912437u ) ); // movie subs
			settings.Add( new ConfigMenuSetting( 33912420u, 33912420u + 46u, 33912440u, 33912441u, 33912442u ) ); // font
			if ( version == GameVersion.X360 ) {
				settings.Add( new ConfigMenuSetting( 33912419u, 33912419u + 46u, 33912439u ) ); // brightness
				settings.Add( new ConfigMenuSetting( 33912421u, 33912421u + 46u, 33912439u ) ); // marketplace
			} else {
				settings.Add( new ConfigMenuSetting( 33912713u, 33912714u, 33912439u ) ); // brightness & screen pos
			}
			settings.Add( new ConfigMenuSetting( 33912595u, 33912596u, 33912597u ) ); // reset to default

			return settings;
		}

		public List<List<ScenarioData>> CreateScenarioIndexGroups( ScenarioType type, string database, string scenarioDatFolder, string scenarioDatFolderMod = null, Util.GameTextEncoding encoding = Util.GameTextEncoding.ShiftJIS ) {
			var data = SqliteUtil.SelectArray( "Data Source=" + database, "SELECT filename, shortdesc, desc FROM descriptions ORDER BY desc" );

			List<ScenarioData> scenes = new List<ScenarioData>();
			foreach ( var d in data ) {
				string filename = (string)d[0];
				string humanReadableName = (string)d[1];
				string episodeID = (string)d[2];

				int idx = humanReadableName.LastIndexOfAny( new char[] { ']', '}' } );
				if ( idx > -1 ) {
					humanReadableName = humanReadableName.Substring( idx + 1 );
				}
				humanReadableName = humanReadableName.Trim();

				if ( filename.StartsWith( "VScenario" ) ) {
					string group;
					int dummy;
					bool isStory;
					bool isScenario = episodeID.StartsWith( "EP_" );

					if ( isScenario ) {
						group = episodeID.Split( '_' )[1];
						isStory = group.Length == 3 && Int32.TryParse( group, out dummy );
					} else {
						group = "";
						isStory = false;
					}

					bool exportAsScenario = isScenario && ( ( type == ScenarioType.Story && isStory ) || ( type == ScenarioType.Sidequests && !isStory ) );
					bool exportAsMap = !isScenario && type == ScenarioType.Maps;
					if ( exportAsScenario || exportAsMap ) {
						if ( !ScenarioFiles.ContainsKey( episodeID ) ) {
							string num = filename.Substring( "VScenario".Length );
							try {
								var orig = new ScenarioFile.ScenarioFile( System.IO.Path.Combine( scenarioDatFolder, num + ".d" ), encoding );
								if ( scenarioDatFolderMod != null ) {
									var mod = new ScenarioFile.ScenarioFile( System.IO.Path.Combine( scenarioDatFolderMod, num + ".d" ), encoding );
									Util.Assert( orig.EntryList.Count == mod.EntryList.Count );
									for ( int i = 0; i < orig.EntryList.Count; ++i ) {
										orig.EntryList[i].EnName = mod.EntryList[i].JpName;
										orig.EntryList[i].EnText = mod.EntryList[i].JpText;
									}
								}
								orig.EpisodeID = episodeID;
								this.ScenarioFiles.Add( episodeID, orig );
								scenes.Add( new ScenarioData() { EpisodeId = episodeID, HumanReadableName = humanReadableName, DatabaseName = filename } );
							} catch ( System.IO.FileNotFoundException ) { }
						}
					}
				}
			}

			return ScenarioData.ProcessScenesToGroups( scenes );
		}

		public void ScenarioAddSkits( List<List<ScenarioData>> groups ) {
			Skits.SkitInfoList.Sort();
			List<TO8CHLI.SkitInfo> skitsToProcess = new List<TO8CHLI.SkitInfo>();
			foreach ( var skit in Skits.SkitInfoList ) {
				if ( skit.Category == 0 ) {
					skitsToProcess.Add( skit );
				}
			}

			for ( int i = 0; i < groups.Count; ++i ) {
				var group = groups[i];
				for ( int j = 0; j < group.Count; ++j ) {
					var scene = group[j];

					ScenarioData nextScene = null;
					if ( j != group.Count - 1 ) {
						nextScene = group[j + 1];
					} else {
						if ( i != groups.Count - 1 ) {
							nextScene = groups[i + 1][0];
						}
					}

					uint nextScenarioId = 1000000u;
					if ( nextScene != null ) {
						string scenarioIdStr = nextScene.EpisodeId.Substring( 3, 7 ).Replace( "_", "" );
						nextScenarioId = UInt32.Parse( scenarioIdStr.TrimStart( '0' ) );
					}

					List<TO8CHLI.SkitInfo> skitsToRemove = new List<TO8CHLI.SkitInfo>();
					foreach ( var skit in skitsToProcess ) {
						uint skitTrigger = skit.FlagTrigger % 1000000u;
						if ( skitTrigger < nextScenarioId ) {
							skitsToRemove.Add( skit );

							if ( !scene.Skits.Contains( skit ) ) {
								scene.Skits.Add( skit );
							}
						}
					}
					foreach ( var skit in skitsToRemove ) {
						skitsToProcess.Remove( skit );
					}
				}
			}
		}
	}
}
