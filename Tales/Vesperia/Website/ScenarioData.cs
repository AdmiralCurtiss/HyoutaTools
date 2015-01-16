using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.Website {
	public class ScenarioData : IComparable<ScenarioData> {
		public string EpisodeId;
		public string HumanReadableName;
		public string DatabaseName;
		public List<TO8CHLI.SkitInfo> Skits = new List<TO8CHLI.SkitInfo>();

		public int CompareTo( ScenarioData other ) {
			return this.EpisodeId.CompareTo( other.EpisodeId );
		}
		public override string ToString() {
			return EpisodeId + ": " + HumanReadableName;
		}

		public string HumanReadableNameWithoutPrefix( string prefix ) {
			string sceneName = HumanReadableName;
			if ( sceneName.StartsWith( prefix ) ) {
				sceneName = sceneName.Substring( prefix.Length );
			}
			sceneName = sceneName.Trim( new char[] { ' ', '-', ':' } );
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
			scenes.Sort();
			List<List<ScenarioData>> groups = new List<List<ScenarioData>>();

			List<ScenarioData> group = new List<ScenarioData>();
			group.Add( scenes[0] );
			for ( int i = 1; i < scenes.Count; ++i ) {
				string currentId = scenes[i].EpisodeId.Split( '_' )[1];
				string lastId = scenes[i - 1].EpisodeId.Split( '_' )[1];

				if ( currentId != lastId ) {
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
