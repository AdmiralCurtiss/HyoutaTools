using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.ItemDat;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.Website {
	public class GenerateWebsite {
		public static int Generate( List<string> args ) {
			string dir = @"d:\Dropbox\ToV\website\";


			var site = new GenerateWebsite();
			site.Version = GameVersion.X360;
			site.Items = new ItemDat.ItemDat( @"d:\Dropbox\ToV\360\item.svo.ext\ITEM.DAT" );
			site.StringDic = new TSS.TSSFile( System.IO.File.ReadAllBytes( @"d:\Dropbox\ToV\360\string_dic_uk.so" ), true );
			site.Artes = new T8BTMA.T8BTMA( @"d:\Dropbox\ToV\360\btl.svo.ext\BTL_PACK_UK.DAT.ext\0004.ext\ALL.0000" );
			site.Skills = new T8BTSK.T8BTSK( @"d:\Dropbox\ToV\360\btl.svo.ext\BTL_PACK_UK.DAT.ext\0010.ext\ALL.0000" );
			site.Enemies = new T8BTEMST.T8BTEMST( @"d:\Dropbox\ToV\360\btl.svo.ext\BTL_PACK_UK.DAT.ext\0005.ext\ALL.0000" );
			site.Recipes = new COOKDAT.COOKDAT( @"d:\Dropbox\ToV\360\cook.svo.ext\COOKDATA.BIN" );
			site.Locations = new WRLDDAT.WRLDDAT( @"d:\Dropbox\ToV\360\menu.svo.ext\WORLDDATA.BIN" );
			site.Synopsis = new SYNPDAT.SYNPDAT( @"d:\Dropbox\ToV\360\menu.svo.ext\SYNOPSISDATA.BIN" );
			site.Titles = new FAMEDAT.FAMEDAT( @"d:\Dropbox\ToV\360\menu.svo.ext\FAMEDATA.BIN" );
			site.GradeShop = new T8BTGR.T8BTGR( @"d:\Dropbox\ToV\360\btl.svo.ext\BTL_PACK_UK.DAT.ext\0016.ext\ALL.0000" );
			site.BattleBook = new BTLBDAT.BTLBDAT( @"d:\Dropbox\ToV\PS3\orig\menu.svo.ext\BATTLEBOOKDATA.BIN" );
			site.Strategy = new T8BTTA.T8BTTA( @"d:\Dropbox\ToV\360\btl.svo.ext\BTL_PACK_UK.DAT.ext\0011.ext\ALL.0000" );
			site.InGameIdDict = site.StringDic.GenerateInGameIdDictionary();

			// copy over Japanese stuff into UK StringDic
			var StringDicUs = new TSS.TSSFile( System.IO.File.ReadAllBytes( @"d:\Dropbox\ToV\360\string_dic_us.so" ), true );
			var IdDictUs = StringDicUs.GenerateInGameIdDictionary();

			foreach ( var kvp in IdDictUs ) {
				site.InGameIdDict[kvp.Key].StringJPN = kvp.Value.StringJPN;
			}

			System.IO.File.WriteAllText( Path.Combine( dir, "items-" + site.Version + ".html" ), site.GenerateHtmlItems(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "enemies-" + site.Version + ".html" ), site.GenerateHtmlEnemies(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "skills-" + site.Version + ".html" ), site.GenerateHtmlSkills(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "artes-" + site.Version + ".html" ), site.GenerateHtmlArtes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "synopsis-" + site.Version + ".html" ), site.GenerateHtmlSynopsis(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "recipes-" + site.Version + ".html" ), site.GenerateHtmlRecipes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "locations-" + site.Version + ".html" ), site.GenerateHtmlLocations(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "strategy-" + site.Version + ".html" ), site.GenerateHtmlStrategy(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "titles-" + site.Version + ".html" ), site.GenerateHtmlTitles(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "battlebook-" + site.Version + ".html" ), site.GenerateHtmlBattleBook(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "records-" + site.Version + ".html" ), site.GenerateHtmlRecords(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "settings-" + site.Version + ".html" ), site.GenerateHtmlSettings(), Encoding.UTF8 );

			site.Version = GameVersion.PS3;
			site.Items = new ItemDat.ItemDat( @"d:\Dropbox\ToV\PS3\orig\item.svo.ext\ITEM.DAT" );
			site.StringDic = new TSS.TSSFile( System.IO.File.ReadAllBytes( @"d:\Dropbox\ToV\PS3\mod\string.svo.ext\STRING_DIC.SO" ) );
			site.Artes = new T8BTMA.T8BTMA( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0004.ext\ALL.0000" );
			site.Skills = new T8BTSK.T8BTSK( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0010.ext\ALL.0000" );
			site.Enemies = new T8BTEMST.T8BTEMST( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0005.ext\ALL.0000" );
			site.Recipes = new COOKDAT.COOKDAT( @"d:\Dropbox\ToV\PS3\orig\menu.svo.ext\COOKDATA.BIN" );
			site.Locations = new WRLDDAT.WRLDDAT( @"d:\Dropbox\ToV\PS3\orig\menu.svo.ext\WORLDDATA.BIN" );
			site.Synopsis = new SYNPDAT.SYNPDAT( @"d:\Dropbox\ToV\PS3\orig\menu.svo.ext\SYNOPSISDATA.BIN" );
			site.Titles = new FAMEDAT.FAMEDAT( @"d:\Dropbox\ToV\PS3\orig\menu.svo.ext\FAMEDATA.BIN" );
			site.GradeShop = new T8BTGR.T8BTGR( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0016.ext\ALL.0000" );
			site.BattleBook = new BTLBDAT.BTLBDAT( @"d:\Dropbox\ToV\PS3\orig\menu.svo.ext\BATTLEBOOKDATA.BIN" );
			site.Strategy = new T8BTTA.T8BTTA( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0011.ext\ALL.0000" );
			site.NecropolisFloors = new T8BTXTM.T8BTXTMA( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0021.ext\ALL.0000" );
			site.NecropolisTreasures = new T8BTXTM.T8BTXTMT( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0022.ext\ALL.0000" );
			var filenames = System.IO.Directory.GetFiles( @"d:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0023.ext\" );
			site.NecropolisMaps = new Dictionary<string, T8BTXTM.T8BTXTMM>( filenames.Length );
			for ( int i = 0; i < filenames.Length; ++i ) {
				site.NecropolisMaps.Add( System.IO.Path.GetFileNameWithoutExtension( filenames[i] ), new T8BTXTM.T8BTXTMM( filenames[i] ) );
			}
			site.InGameIdDict = site.StringDic.GenerateInGameIdDictionary();

			System.IO.File.WriteAllText( Path.Combine( dir, "items-" + site.Version + ".html" ), site.GenerateHtmlItems(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "enemies-" + site.Version + ".html" ), site.GenerateHtmlEnemies(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "skills-" + site.Version + ".html" ), site.GenerateHtmlSkills(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "artes-" + site.Version + ".html" ), site.GenerateHtmlArtes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "synopsis-" + site.Version + ".html" ), site.GenerateHtmlSynopsis(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "recipes-" + site.Version + ".html" ), site.GenerateHtmlRecipes(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "locations-" + site.Version + ".html" ), site.GenerateHtmlLocations(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "strategy-" + site.Version + ".html" ), site.GenerateHtmlStrategy(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "titles-" + site.Version + ".html" ), site.GenerateHtmlTitles(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "battlebook-" + site.Version + ".html" ), site.GenerateHtmlBattleBook(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "records-" + site.Version + ".html" ), site.GenerateHtmlRecords(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "settings-" + site.Version + ".html" ), site.GenerateHtmlSettings(), Encoding.UTF8 );
			System.IO.File.WriteAllText( Path.Combine( dir, "necropolis-" + site.Version + ".html" ), site.GenerateHtmlNecropolis(), Encoding.UTF8 );

			return 0;
		}

		public GameVersion Version;
		public ItemDat.ItemDat Items;
		public TSS.TSSFile StringDic;
		public T8BTMA.T8BTMA Artes;
		public T8BTSK.T8BTSK Skills;
		public T8BTEMST.T8BTEMST Enemies;
		public COOKDAT.COOKDAT Recipes;
		public WRLDDAT.WRLDDAT Locations;
		public SYNPDAT.SYNPDAT Synopsis;
		public FAMEDAT.FAMEDAT Titles;
		public T8BTGR.T8BTGR GradeShop;
		public BTLBDAT.BTLBDAT BattleBook;
		public T8BTTA.T8BTTA Strategy;

		public T8BTXTM.T8BTXTMA NecropolisFloors;
		public T8BTXTM.T8BTXTMT NecropolisTreasures;
		public Dictionary<string, T8BTXTM.T8BTXTMM> NecropolisMaps;

		public Dictionary<uint, TSS.TSSEntry> InGameIdDict;

		public string GenerateHtmlItems() {
			var sb = new StringBuilder();
			AddHeader( sb, "Items" );
			sb.AppendLine( "<body><table>" );
			AddMenuBar( sb );
			foreach ( var item in Items.items ) {
				if ( item.Data[(int)ItemData.Category] == 0 ) { continue; }
				sb.AppendLine( ItemDat.ItemDat.GetItemDataAsHtml( Version, Items, item, Skills, Enemies, Recipes, Locations, StringDic, InGameIdDict ) );
				sb.AppendLine( "<tr><td colspan=\"5\"><hr></td></tr>" );
			}
			sb.AppendLine( "</table></body></html>" );
			return FixInGameStrings( sb );
		}
		public string GenerateHtmlEnemies() {
			var sb = new StringBuilder();
			AddHeader( sb, "Enemies" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			foreach ( var enemy in Enemies.EnemyList ) {
				if ( enemy.InGameID == 0 ) { continue; }
				sb.AppendLine( enemy.GetDataAsHtml( Version, Items, Locations, StringDic, InGameIdDict ) );
				sb.AppendLine( "<hr>" );
			}
			sb.AppendLine( "</body></html>" );
			return FixInGameStrings( sb );
		}
		public string GenerateHtmlSkills() {
			var sb = new StringBuilder();
			AddHeader( sb, "Enemies" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			foreach ( var skill in Skills.SkillList ) {
				sb.AppendLine( skill.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				sb.AppendLine( "<hr>" );
			}
			sb.AppendLine( "</body></html>" );
			return FixInGameStrings( sb );
		}
		public string GenerateHtmlArtes() {
			var sb = new StringBuilder();
			AddHeader( sb, "Enemies" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			foreach ( var arte in Artes.ArteList ) {
				if ( arte.Type == T8BTMA.Arte.ArteType.Generic ) {
					continue;
				}
				sb.AppendLine( arte.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				sb.AppendLine( "<hr>" );
			}
			sb.AppendLine( "</body></html>" );
			return FixInGameStrings( sb );
		}
		public string GenerateHtmlSynopsis() {
			var sb = new StringBuilder();
			AddHeader( sb, "Enemies" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			foreach ( var entry in Synopsis.SynopsisList ) {
				if ( InGameIdDict[entry.NameStringDicId].StringEngOrJpn == "" ) { continue; }
				sb.AppendLine( entry.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				sb.AppendLine( "<hr>" );
			}
			sb.AppendLine( "</body></html>" );
			return FixInGameStrings( sb );
		}
		public string GenerateHtmlRecipes() {
			var sb = new StringBuilder();
			AddHeader( sb, "Enemies" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			for ( int i = 1; i < Recipes.RecipeList.Count; ++i ) {
				var recipe = Recipes.RecipeList[i];
				sb.AppendLine( recipe.GetDataAsHtml( Version, Recipes, Items, StringDic, InGameIdDict ) );
				sb.AppendLine( "<hr>" );
			}
			sb.AppendLine( "</body></html>" );
			return FixInGameStrings( sb );
		}
		public string GenerateHtmlLocations() {
			var sb = new StringBuilder();
			AddHeader( sb, "Enemies" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			for ( int i = 1; i < Locations.LocationList.Count; ++i ) {
				var location = Locations.LocationList[i];
				sb.AppendLine( location.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				sb.AppendLine( "<hr>" );
			}
			sb.AppendLine( "</body></html>" );
			return FixInGameStrings( sb );
		}
		public string GenerateHtmlStrategy() {
			var sb = new StringBuilder();
			AddHeader( sb, "Strategy" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			foreach ( var entry in Strategy.StrategySetList ) {
				sb.Append( entry.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				sb.Append( "<hr>" );
			}
			foreach ( var entry in Strategy.StrategyOptionList ) {
				sb.Append( entry.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				sb.Append( "<hr>" );
			}
			sb.AppendLine( "</body></html>" );
			return FixInGameStrings( sb );
		}
		public string GenerateHtmlTitles() {
			var sb = new StringBuilder();
			AddHeader( sb, "Titles" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			foreach ( var entry in Titles.TitleList ) {
				sb.Append( entry.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				sb.Append( "<hr>" );
			}
			sb.AppendLine( "</body></html>" );
			return FixInGameStrings( sb );
		}
		public string GenerateHtmlBattleBook() {
			var sb = new StringBuilder();
			AddHeader( sb, "Battle Book" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );

			foreach ( var entry in BattleBook.BattleBookEntryList ) {
				try {
					sb.Append( entry.GetDataAsHtml( Version, StringDic, InGameIdDict ) );
				} catch ( KeyNotFoundException ) {
					continue;
				}
				sb.Append( "<hr>" );
			}

			sb.AppendLine( "</body></html>" );
			return FixInGameStrings( sb );
		}
		public string GenerateHtmlRecords() {
			var sb = new StringBuilder();
			AddHeader( sb, "Records" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			sb.AppendLine( "</body></html>" );
			return FixInGameStrings( sb );
		}
		public string GenerateHtmlSettings() {
			var sb = new StringBuilder();
			AddHeader( sb, "Settings" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );
			sb.AppendLine( "</body></html>" );
			return FixInGameStrings( sb );
		}
		public string GenerateHtmlNecropolis() {
			var sb = new StringBuilder();
			AddHeader( sb, "Necropolis of Nostalgia" );
			sb.AppendLine( "<body>" );
			AddMenuBar( sb );

			foreach ( var map in NecropolisMaps ) {
				sb.Append( "<hr>" );
				sb.Append( map.Key );
				sb.Append( map.Value.GetDataAsHtml() );
			}

			sb.AppendLine( "</body></html>" );
			return FixInGameStrings( sb );
		}

		public void AddHeader( StringBuilder sb, string name ) {
			sb.AppendLine( "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">" );
			sb.AppendLine( "<title>Tales of Vesperia (" + Version.ToString() + ") - " + name + "</title>" );
			sb.AppendLine( "<style>" );
			sb.AppendLine( "body { background-color: #68504F; color: #EFD1AE; font-size: 16; }" );
			sb.AppendLine( ".itemname { color: #FFEBD2; font-size: 20; }" );
			sb.AppendLine( ".itemdesc { }" );
			sb.AppendLine( ".equip { text-align: right; float: right; }" );
			sb.AppendLine( ".special { text-align: right; float: right; }" );
			sb.AppendLine( "table, tr, td, th { padding: 0px 4px 0px 0px; border-spacing: 0px; }" );
			sb.AppendLine( "td, td > a { vertical-align: top; }" );
			sb.AppendLine( "a:link, a:visited, a:hover, a:active { color: #FFEBD2; }" );
			sb.AppendLine( "table.element { display: inline-block; }" );
			sb.AppendLine( "table.element td { text-align: center; }" );
			sb.AppendLine( "table.synopsis { margin: 0px auto; }" );
			sb.AppendLine( "table.synopsis td { padding: 0px 10px 0px 10px; border-spacing: 10px; }" );
			sb.AppendLine( ".synopsistitle { text-align: center; color: #FFEBD2; font-size: 20; }" );
			sb.AppendLine( "</style>" );
			sb.AppendLine( "</head>" );
		}
		public void AddMenuBar( StringBuilder sb ) {
			sb.AppendLine( "<div>" );
			sb.AppendLine( "<a href=\"artes-" + Version + ".html\"><img src=\"menu-icons/main-01.png\" title=\"Artes\"></a>" );
			//sb.AppendLine( "<a href=\"equip-" + Version + ".html\"><img src=\"menu-icons/main-02.png\" title=\"Equipment\"></a>" );
			sb.AppendLine( "<a href=\"items-" + Version + ".html\"><img src=\"menu-icons/main-03.png\" title=\"Items\"></a>" );
			sb.AppendLine( "<a href=\"skills-" + Version + ".html\"><img src=\"menu-icons/main-04.png\" title=\"Skills\"></a>" );
			sb.AppendLine( "<a href=\"strategy-" + Version + ".html\"><img src=\"menu-icons/main-05.png\" title=\"Strategy\"></a>" );
			sb.AppendLine( "<a href=\"recipes-" + Version + ".html\"><img src=\"menu-icons/main-06.png\" title=\"Recipes\"></a>" );
			sb.AppendLine( "<a href=\"titles-" + Version + ".html\"><img src=\"menu-icons/main-07.png\" title=\"Titles\"></a>" );
			//sb.AppendLine( "<img src=\"menu-icons/main-08.png\" title=\"Library\">" );
			sb.AppendLine( "<a href=\"synopsis-" + Version + ".html\"><img src=\"menu-icons/sub-09.png\" title=\"Synopsis\"></a>" );
			sb.AppendLine( "<a href=\"battlebook-" + Version + ".html\"><img src=\"menu-icons/sub-14.png\" title=\"Battle Book\"></a>" );
			sb.AppendLine( "<a href=\"enemies-" + Version + ".html\"><img src=\"menu-icons/sub-13.png\" title=\"Monster Book\"></a>" );
			//sb.AppendLine( "<a href=\"items-" + Version + ".html\"><img src=\"menu-icons/sub-11.png\" title=\"Collector's Book\"></a>" );
			sb.AppendLine( "<a href=\"locations-" + Version + ".html\"><img src=\"menu-icons/sub-10.png\" title=\"World Map\"></a>" );
			sb.AppendLine( "<a href=\"records-" + Version + ".html\"><img src=\"menu-icons/sub-08.png\" title=\"Records\"></a>" );
			//sb.AppendLine( "<img src=\"menu-icons/main-09.png\" title=\"Save & Load\">" );
			//sb.AppendLine( "<img src=\"menu-icons/sub-06.png\" title=\"Save\">" );
			//sb.AppendLine( "<img src=\"menu-icons/sub-05.png\" title=\"Load\">" );
			sb.AppendLine( "<a href=\"settings-" + Version + ".html\"><img src=\"menu-icons/sub-07.png\" title=\"Settings\"></a>" );
			if ( Version == GameVersion.PS3 ) {
				sb.AppendLine( "<a href=\"necropolis-" + Version + ".html\"><img src=\"menu-icons/weather-7-64px.png\" title=\"Necropolis of Nostalgia Maps\"></a>" );
			}
			sb.AppendLine( "</div>" );
			sb.AppendLine( "<hr>" );
		}
		public string FixInGameStrings( StringBuilder sb ) {
			sb.Replace( "\x06(STA)", "<img src=\"text-icons/" + Version.ToString() + "/button-Start.png\" height=\"16\">" );
			sb.Replace( "\x06(L3)", "<img src=\"text-icons/" + Version.ToString() + "/ls-push.png\" height=\"16\">" );
			sb.Replace( "\x06(ST1)", "<img src=\"text-icons/icon-status-01.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST2)", "<img src=\"text-icons/icon-status-02.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST3)", "<img src=\"text-icons/icon-status-03.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST4)", "<img src=\"text-icons/icon-status-04.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST5)", "<img src=\"text-icons/icon-status-05.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST6)", "<img src=\"text-icons/icon-status-06.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST7)", "<img src=\"text-icons/icon-status-07.png\" height=\"16\" width=\"16\">" );
			return VesperiaUtil.RemoveTags( sb.ToString() );
		}
		public static void AppendCharacterBitfieldAsImageString( StringBuilder sb, GameVersion version, uint equip ) {
			if ( ( equip & 1 ) == 1 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_YUR.gif\" height=\"32\" width=\"24\">" ); }
			if ( ( equip & 2 ) == 2 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_EST.gif\" height=\"32\" width=\"24\">" ); }
			if ( ( equip & 4 ) == 4 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_KAR.gif\" height=\"32\" width=\"24\">" ); }
			if ( ( equip & 8 ) == 8 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_RIT.gif\" height=\"32\" width=\"24\">" ); }
			if ( ( equip & 16 ) == 16 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_RAV.gif\" height=\"32\" width=\"24\">" ); }
			if ( ( equip & 32 ) == 32 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_JUD.gif\" height=\"32\" width=\"24\">" ); }
			if ( ( equip & 64 ) == 64 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_RAP.gif\" height=\"32\" width=\"24\">" ); }
			if ( ( equip & 128 ) == 128 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_FRE.gif\" height=\"32\" width=\"24\">" ); }
			if ( version == GameVersion.PS3 && ( equip & 256 ) == 256 ) { sb.Append( "<img src=\"chara-icons/StatusIcon_PAT.gif\" height=\"32\" width=\"24\">" ); }
		}
		public static void AppendPhysicalAilmentBitfieldAsImageString( StringBuilder sb, uint physAil ) {
			if ( ( physAil & 1 ) == 1 ) { sb.Append( "<img src=\"text-icons/icon-status-13.png\" height=\"32\" width=\"32\">" ); }
			if ( ( physAil & 2 ) == 2 ) { sb.Append( "<img src=\"text-icons/icon-status-01.png\" height=\"32\" width=\"32\">" ); }
			if ( ( physAil & 4 ) == 4 ) { sb.Append( "<img src=\"text-icons/icon-status-02.png\" height=\"32\" width=\"32\">" ); }
			if ( ( physAil & 8 ) == 8 ) { sb.Append( "<img src=\"text-icons/icon-status-03.png\" height=\"32\" width=\"32\">" ); }
			if ( ( physAil & 16 ) == 16 ) { sb.Append( "<img src=\"text-icons/icon-status-04.png\" height=\"32\" width=\"32\">" ); }
			if ( ( physAil & 32 ) == 32 ) { sb.Append( "<img src=\"text-icons/icon-status-05.png\" height=\"32\" width=\"32\">" ); }
			if ( ( physAil & 64 ) == 64 ) { sb.Append( "<img src=\"text-icons/icon-status-06.png\" height=\"32\" width=\"32\">" ); }
			if ( ( physAil & 128 ) == 128 ) { sb.Append( "<img src=\"text-icons/icon-status-07.png\" height=\"32\" width=\"32\">" ); }
		}
	}
}
