using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.Website {
	public partial class WebsiteGenerator {
		public GameVersion Version;
		public GameLocale Locale;

		public ItemDat.ItemDat Items;
		public TSS.TSSFile StringDic;
		public T8BTMA.T8BTMA Artes;
		public T8BTSK.T8BTSK Skills;
		public T8BTEMST.T8BTEMST Enemies;
		public T8BTEMGP.T8BTEMGP EnemyGroups;
		public T8BTEMEG.T8BTEMEG EncounterGroups;
		public COOKDAT.COOKDAT Recipes;
		public WRLDDAT.WRLDDAT Locations;
		public SYNPDAT.SYNPDAT Synopsis;
		public FAMEDAT.FAMEDAT Titles;
		public T8BTGR.T8BTGR GradeShop;
		public BTLBDAT.BTLBDAT BattleBook;
		public T8BTTA.T8BTTA Strategy;
		public ShopData.ShopData Shops;
		public TO8CHLI.TO8CHLI Skits;
		public TOVSEAF.TOVSEAF SearchPoints;
		public T8BTVA.T8BTVA BattleVoicesEnd;
		public Trophy.TrophyConfNode TrophyJp;
		public Trophy.TrophyConfNode TrophyEn;
		public Dictionary<string, TO8CHTX.ChatFile> SkitText;
		public List<uint> Records;
		public List<ConfigMenuSetting> Settings;

		public Dictionary<string, ScenarioFile.ScenarioFile> ScenarioFiles;
		public List<List<ScenarioData>> ScenarioGroupsStory;
		public List<List<ScenarioData>> ScenarioGroupsSidequests;
		public List<List<ScenarioData>> ScenarioGroupsMaps;

		public Dictionary<string, SCFOMBIN.SCFOMBIN> BattleTextFiles;

		public T8BTXTM.T8BTXTMA NecropolisFloors;
		public T8BTXTM.T8BTXTMT NecropolisTreasures;
		public IDictionary<string, T8BTXTM.T8BTXTMM> NecropolisMaps;

		public Dictionary<uint, TSS.TSSEntry> InGameIdDict;
		public uint[] IconsWithItems;
	}
}
