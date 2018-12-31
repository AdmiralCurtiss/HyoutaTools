using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.Website {
	public class ScenarioData : IComparable<ScenarioData> {
		public int ScenarioDatIndex;
		public string EpisodeId;
		public string HumanReadableName;
		public List<TO8CHLI.SkitInfo> Skits = new List<TO8CHLI.SkitInfo>();

		public int CompareTo( ScenarioData other ) {
			return this.EpisodeId.CompareTo( other.EpisodeId );
		}
		public override string ToString() {
			return "[" + ScenarioDatIndex + "] " + EpisodeId + ": " + HumanReadableName;
		}

		public string HumanReadableNameWithoutPrefix( string prefix ) {
			string sceneName = HumanReadableName;
			if ( sceneName.StartsWith( prefix ) ) {
				sceneName = sceneName.Substring( prefix.Length );
			}
			sceneName = sceneName.Trim( new char[] { ' ', '-', ':' } );
			if ( sceneName == "" ) { sceneName = HumanReadableName; }
			return sceneName;
		}

		public static string FindMostCommonStart( List<ScenarioData> group ) {
			Dictionary<string, int> dict = new Dictionary<string, int>();
			foreach ( var scene in group ) {
				string start = scene.HumanReadableName;
				int idx = start.IndexOfAny( new char[] { ':', '-' } );
				if ( idx > -1 ) {
					start = start.Substring( 0, idx );
				}
				start = start.Trim();
				if ( !dict.Keys.Contains( start ) ) {
					dict.Add( start, 0 );
				}
				dict[start] += 1;
			}

			string highestStr = "";
			int highestInt = 0;
			foreach ( var kvp in dict ) {
				if ( kvp.Value > highestInt ) {
					highestInt = kvp.Value;
					highestStr = kvp.Key;
				}
			}

			return highestStr;
		}

		public static List<List<ScenarioData>> ProcessScenesToGroups( List<ScenarioData> scenes ) {
			var digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

			scenes.Sort();
			List<List<ScenarioData>> groups = new List<List<ScenarioData>>();

			List<ScenarioData> group = new List<ScenarioData>();
			group.Add( scenes[0] );
			for ( int i = 1; i < scenes.Count; ++i ) {
				string currId;
				string lastId;

				string currEp = scenes[i].EpisodeId;
				string lastEp = scenes[i - 1].EpisodeId;

				currId = currEp.Contains( '_' ) ? currEp.Split( '_' )[currEp.StartsWith( "EP_" ) ? 1 : 0] : currEp.TrimEnd( digits );
				lastId = lastEp.Contains( '_' ) ? lastEp.Split( '_' )[lastEp.StartsWith( "EP_" ) ? 1 : 0] : lastEp.TrimEnd( digits );

				if ( currId != lastId ) {
					groups.Add( group );
					group = new List<ScenarioData>();
				}

				group.Add( scenes[i] );
			}
			groups.Add( group );

			return groups;
		}
	}
}
