using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.Website {
	public partial class WebsiteGenerator {
		public static StringBuilder ReplaceIconsWithHtml( StringBuilder sb, Dictionary<uint, TSS.TSSEntry> inGameIdDict, GameVersion Version, bool japaneseStyle ) {
			sb.Replace( "\x06(ST1)", "<img src=\"text-icons/icon-status-01.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST2)", "<img src=\"text-icons/icon-status-02.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST3)", "<img src=\"text-icons/icon-status-03.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST4)", "<img src=\"text-icons/icon-status-04.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST5)", "<img src=\"text-icons/icon-status-05.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST6)", "<img src=\"text-icons/icon-status-06.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(ST7)", "<img src=\"text-icons/icon-status-07.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC1)", "<img src=\"text-icons/icon-status-08.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC2)", "<img src=\"text-icons/icon-status-09.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC3)", "<img src=\"text-icons/icon-status-10.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC4)", "<img src=\"text-icons/icon-status-11.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC5)", "<img src=\"text-icons/icon-status-17.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC6)", "<img src=\"text-icons/icon-status-13.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(SC7)", "<img src=\"text-icons/icon-status-18.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL1)", "<img src=\"text-icons/icon-element-02.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL2)", "<img src=\"text-icons/icon-element-04.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL3)", "<img src=\"text-icons/icon-element-01.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL4)", "<img src=\"text-icons/icon-element-05.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL5)", "<img src=\"text-icons/icon-element-03.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL6)", "<img src=\"text-icons/icon-element-06.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(EL7)", "<img src=\"text-icons/icon-element-07.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(FS1)", "<img src=\"text-icons/icon-fs-01.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(FS2)", "<img src=\"text-icons/icon-fs-02.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(FS3)", "<img src=\"text-icons/icon-fs-03.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC1)", "<img src=\"text-icons/icon-monster-01.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC2)", "<img src=\"text-icons/icon-monster-02.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC3)", "<img src=\"text-icons/icon-monster-03.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC4)", "<img src=\"text-icons/icon-monster-04.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC5)", "<img src=\"text-icons/icon-monster-05.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC6)", "<img src=\"text-icons/icon-monster-06.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC7)", "<img src=\"text-icons/icon-monster-07.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC8)", "<img src=\"text-icons/icon-monster-08.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(MC9)", "<img src=\"text-icons/icon-monster-09.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(COS)", "<img src=\"text-icons/icon-costume.png\" height=\"16\" width=\"16\">" );
			sb.Replace( "\x06(STA)", "<img src=\"text-icons/" + Version.ToString() + "/button-Start.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.Start ) + "\">" );
			sb.Replace( "\x06(SEL)", "<img src=\"text-icons/" + Version.ToString() + "/button-Select.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.Select ) + "\">" );
			sb.Replace( "\x06(L3)", "<img src=\"text-icons/" + Version.ToString() + "/ls-push.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.L3 ) + "\">" );
			sb.Replace( "\x06(R3)", "<img src=\"text-icons/" + Version.ToString() + "/rs-push.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.R3 ) + "\">" );
			sb.Replace( "\x06(MVN)", "<img src=\"text-icons/" + Version.ToString() + "/ls.png\" height=\"16\" title=\"Character Movement\">" );
			sb.Replace( "\x06(MVR)", "<img src=\"text-icons/" + Version.ToString() + "/ls-right.png\" height=\"16\" title=\"Character Movement Right\">" );
			sb.Replace( "\x06(MVL)", "<img src=\"text-icons/" + Version.ToString() + "/ls-left.png\" height=\"16\" title=\"Character Movement Left\">" );
			sb.Replace( "\x06(MVU)", "<img src=\"text-icons/" + Version.ToString() + "/ls-up.png\" height=\"16\" title=\"Character Movement Up\">" );
			sb.Replace( "\x06(MVD)", "<img src=\"text-icons/" + Version.ToString() + "/ls-down.png\" height=\"16\" title=\"Character Movement Down\">" );
			sb.Replace( "\x06(MVH)", "<img src=\"text-icons/" + Version.ToString() + "/ls-side.png\" height=\"16\" title=\"Character Movement Side\">" );
			sb.Replace( "\x06(LTN)", "<img src=\"text-icons/" + Version.ToString() + "/ls.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftStick ) + "\">" );
			sb.Replace( "\x06(LTR)", "<img src=\"text-icons/" + Version.ToString() + "/ls-right.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftStick ) + " Right\">" );
			sb.Replace( "\x06(LTL)", "<img src=\"text-icons/" + Version.ToString() + "/ls-left.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftStick ) + " Left\">" );
			sb.Replace( "\x06(LTU)", "<img src=\"text-icons/" + Version.ToString() + "/ls-up.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftStick ) + " Up\">" );
			sb.Replace( "\x06(LTD)", "<img src=\"text-icons/" + Version.ToString() + "/ls-down.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftStick ) + " Down\">" );
			sb.Replace( "\x06(LTH)", "<img src=\"text-icons/" + Version.ToString() + "/ls-side.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftStick ) + " Side\">" );
			sb.Replace( "\x06(RTN)", "<img src=\"text-icons/" + Version.ToString() + "/rs.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightStick ) + "\">" );
			sb.Replace( "\x06(RTR)", "<img src=\"text-icons/" + Version.ToString() + "/rs-right.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightStick ) + " Right\">" );
			sb.Replace( "\x06(RTL)", "<img src=\"text-icons/" + Version.ToString() + "/rs-left.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightStick ) + " Left\">" );
			sb.Replace( "\x06(RTU)", "<img src=\"text-icons/" + Version.ToString() + "/rs-up.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightStick ) + " Up\">" );
			sb.Replace( "\x06(RTD)", "<img src=\"text-icons/" + Version.ToString() + "/rs-down.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightStick ) + " Down\">" );
			sb.Replace( "\x06(RTH)", "<img src=\"text-icons/" + Version.ToString() + "/rs-side.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightStick ) + " Side\">" );
			sb.Replace( "\x06(LBN)", "<img src=\"text-icons/" + Version.ToString() + "/dpad.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.DPad ) + "\">" );
			sb.Replace( "\x06(LBR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-right.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.DPad ) + " Right\">" );
			sb.Replace( "\x06(LBL)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-left.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.DPad ) + " Left\">" );
			sb.Replace( "\x06(LBU)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-up.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.DPad ) + " Up\">" );
			sb.Replace( "\x06(LBD)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-down.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.DPad ) + " Down\">" );
			sb.Replace( "\x06(LBH)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-side.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.DPad ) + " Side\">" );
			sb.Replace( "\x06(MVNR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad.png\" height=\"16\" title=\"Non-Character Movement\">" );
			sb.Replace( "\x06(MVRR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-right.png\" height=\"16\" title=\"Non-Character Movement Right\">" );
			sb.Replace( "\x06(MVLR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-left.png\" height=\"16\" title=\"Non-Character Movement Left\">" );
			sb.Replace( "\x06(MVUR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-up.png\" height=\"16\" title=\"Non-Character Movement Up\">" );
			sb.Replace( "\x06(MVDR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-down.png\" height=\"16\" title=\"Non-Character Movement Down\">" );
			sb.Replace( "\x06(MVHR)", "<img src=\"text-icons/" + Version.ToString() + "/dpad-side.png\" height=\"16\" title=\"Non-Character Movement Side\">" );
			sb.Replace( "\x06(RBL)", "<img src=\"text-icons/" + Version.ToString() + "/button-action.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LeftButton ) + "\">" );
			sb.Replace( "\x06(RBU)", "<img src=\"text-icons/" + Version.ToString() + "/button-menu.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.UpperButton ) + "\">" );
			sb.Replace( "\x06(RBR)", "<img src=\"text-icons/" + Version.ToString() + "/button-cancel.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.RightButton ) + "\">" );
			sb.Replace( "\x06(RBD)", "<img src=\"text-icons/" + Version.ToString() + "/button-confirm.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.LowerButton ) + "\">" );

			if ( japaneseStyle && Version.SwapsConfirmAndCancelDependingOnRegion() ) {
				// in JP PS3 version, swap circle/cross for confirm/cancel
				sb.Replace( "\x06(CCL)", "<img src=\"text-icons/" + Version.ToString() + "/button-confirm.png\" height=\"16\" title=\"Cancel\">" );
				sb.Replace( "\x06(ETR)", "<img src=\"text-icons/" + Version.ToString() + "/button-cancel.png\" height=\"16\" title=\"Confirm\">" );
			} else {
				sb.Replace( "\x06(CCL)", "<img src=\"text-icons/" + Version.ToString() + "/button-cancel.png\" height=\"16\" title=\"Cancel\">" );
				sb.Replace( "\x06(ETR)", "<img src=\"text-icons/" + Version.ToString() + "/button-confirm.png\" height=\"16\" title=\"Confirm\">" );
			}

			sb.Replace( "\x06(ATK)", "<img src=\"text-icons/" + Version.ToString() + "/button-cancel.png\" height=\"16\" title=\"Attack\">" );
			sb.Replace( "\x06(ART)", "<img src=\"text-icons/" + Version.ToString() + "/button-confirm.png\" height=\"16\" title=\"Arte\">" );
			sb.Replace( "\x06(GUD)", "<img src=\"text-icons/" + Version.ToString() + "/button-action.png\" height=\"16\" title=\"Guard\">" );
			sb.Replace( "\x06(MEN)", "<img src=\"text-icons/" + Version.ToString() + "/button-menu.png\" height=\"16\" title=\"Menu\">" );
			sb.Replace( "\x06(CBR1)", "<img src=\"text-icons/" + Version.ToString() + "/button-R1.png\" height=\"16\" title=\"Enemy Target Switch\">" );
			sb.Replace( "\x06(CBR2)", "<img src=\"text-icons/" + Version.ToString() + "/button-R2.png\" height=\"16\" title=\"Fatal Strike\">" );
			sb.Replace( "\x06(CBL1)", "<img src=\"text-icons/" + Version.ToString() + "/button-L1.png\" height=\"16\" title=\"Alternate Attack\">" );
			sb.Replace( "\x06(CBL2)", "<img src=\"text-icons/" + Version.ToString() + "/button-L2.png\" height=\"16\" title=\"Free Run\">" );
			sb.Replace( "\x06(R1)", "<img src=\"text-icons/" + Version.ToString() + "/button-R1.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.R1 ) + "\">" );
			sb.Replace( "\x06(R2)", "<img src=\"text-icons/" + Version.ToString() + "/button-R2.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.R2 ) + "\">" );
			sb.Replace( "\x06(L1)", "<img src=\"text-icons/" + Version.ToString() + "/button-L1.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.L1 ) + "\">" );
			sb.Replace( "\x06(L2)", "<img src=\"text-icons/" + Version.ToString() + "/button-L2.png\" height=\"16\" title=\"" + VesperiaUtil.GetButtonName( Version, ControllerButton.L2 ) + "\">" );
			sb.Replace( '∀', '♥' );
			sb.Replace( '‡', 'é' );
			sb.Replace( '†', 'í' );
			return sb;
		}

		public static void AddHeader( StringBuilder sb, GameVersion Version, string name ) {
			sb.AppendLine( "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">" );
			sb.AppendLine( "<title>" + name + " - Tales of Vesperia (" + Version.ToString() + ")</title>" );
			sb.AppendLine( "<style>" );
			sb.AppendLine( HyoutaTools.Properties.Resources.vesperia_website_general_css );
			sb.AppendLine( HyoutaTools.Properties.Resources.vesperia_website_scenario_css );
			sb.AppendLine( "</style>" );
			sb.AppendLine( "</head>" );
		}
		public static void AddMenuBar( StringBuilder sb, GameVersion Version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, IEnumerable<uint> IconsWithItems, Dictionary<uint, TSS.TSSEntry> InGameIdDict ) {
			sb.AppendLine( "<div id=\"header-name\">" );
			sb.AppendLine( "<a href=\"index.html\">Tales of Vesperia - Data &amp; Translation Guide</a>" );
			sb.AppendLine( "</div>" );

			sb.AppendLine( "<div id=\"topmenu\">" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Arte, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/main-01.png\" title=\"Artes\"></a>" );
			//sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Equipment, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/main-02.png\" title=\"Equipment\"></a>" );
			//sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Item, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/main-03.png\" title=\"Items\"></a>" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Skill, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/main-04.png\" title=\"Skills\"></a>" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Strategy, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/main-05.png\" title=\"Strategy\"></a>" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Recipe, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/main-06.png\" title=\"Recipes\"></a>" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Shop, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/main-02.png\" title=\"Shops\"></a>" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Title, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/main-07.png\" title=\"Titles\"></a>" );
			//sb.AppendLine( "<img src=\"menu-icons/main-08.png\" title=\"Library\">" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Synopsis, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/sub-09.png\" title=\"Synopsis\"></a>" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.BattleBook, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/sub-14.png\" title=\"Battle Book\"></a>" );
			//sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Enemy, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/sub-13.png\" title=\"Monster Book\"></a>" );
			//sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Item, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/sub-11.png\" title=\"Collector's Book\"></a>" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Location, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/sub-10.png\" title=\"World Map\"></a>" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.SearchPoint, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"etc/U_ITEM_IRIKIAGRASS-64px.png\" title=\"Search Points\"></a>" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Record, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/sub-08.png\" title=\"Records\"></a>" );
			//sb.AppendLine( "<img src=\"menu-icons/main-09.png\" title=\"Save & Load\">" );
			//sb.AppendLine( "<img src=\"menu-icons/sub-06.png\" title=\"Save\">" );
			//sb.AppendLine( "<img src=\"menu-icons/sub-05.png\" title=\"Load\">" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Settings, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/sub-07.png\" title=\"Settings\"></a>" );
			sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.GradeShop, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"item-categories/cat-01.png\" title=\"Grade Shop\"></a>" );
			if ( Version.HasPS3Content() ) {
				sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.NecropolisMap, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"menu-icons/weather-4-64px.png\" title=\"Necropolis of Nostalgia Maps\"></a>" );
			}
			if ( Version == GameVersion.PS3 ) {
				sb.AppendLine( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Trophy, Version, versionPostfix, locale, websiteLanguage, false ) + "\"><img src=\"trophies/gold.png\" title=\"Trophies\"></a>" );
			}
			sb.AppendLine( "<br>" );
			for ( uint i = 2; i < 12; ++i ) {
				sb.Append( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Item, Version, versionPostfix, locale, websiteLanguage, false, category: (int)i ) + "\">" );
				sb.Append( "<img src=\"item-categories/cat-" + i.ToString( "D2" ) + ".png\" title=\"" + InGameIdDict[33912572u + i].StringEngOrJpnHtml( Version, InGameIdDict, websiteLanguage ) + "\" height=\"32\">" );
				sb.Append( "</a>" );
			}
			sb.AppendLine();
			for ( uint i = 0; i < 9; ++i ) {
				sb.Append( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Enemy, Version, versionPostfix, locale, websiteLanguage, false, category: (int)i ) + "\">" );
				sb.Append( "<img src=\"monster-categories/cat-" + i + ".png\" title=\"" + InGameIdDict[33912323u + i].StringEngOrJpnHtml( Version, InGameIdDict, websiteLanguage ) + "\" height=\"32\">" );
				sb.Append( "</a>" );
			}
			sb.AppendLine( "<br>" );
			foreach ( uint i in IconsWithItems ) {
				sb.Append( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.Item, Version, versionPostfix, locale, websiteLanguage, false, icon: (int)i ) + "\">" );
				sb.Append( "<img src=\"item-icons/ICON" + i + ".png\" height=\"16\" width=\"16\">" );
				sb.Append( "</a>" );
			}
			sb.AppendLine( "<br>" );
			sb.Append( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.ScenarioStoryIndex, Version, versionPostfix, locale, websiteLanguage, false ) + "\">Story</a> / " );
			sb.Append( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.ScenarioSidequestIndex, Version, versionPostfix, locale, websiteLanguage, false ) + "\">Sidequests</a> / " );
			sb.Append( "<a href=\"" + WebsiteGenerator.GetUrl( WebsiteSection.SkitIndex, Version, versionPostfix, locale, websiteLanguage, false ) + "\">Skits</a>" );
			sb.AppendLine();
			sb.AppendLine( "</div>" );
			sb.AppendLine( "<hr>" );
			sb.AppendLine( "<div id=\"content\">" );
		}
		public static void AddFooter( StringBuilder sb ) {
			sb.AppendLine( "</div>" );
			sb.AppendLine( "<div id=\"footer\">All Tales of Vesperia game content © 2008/2009 Bandai Namco Games Inc.</div>" );
		}

		public static void AppendCharacterBitfieldAsImageString( StringBuilder sb, Dictionary<uint, TSS.TSSEntry> inGameIdDict, GameVersion version, uint equip ) {
			if ( ( equip & 1 ) == 1 ) { sb.Append( "<img src=\"chara-icons/YUR.png\" height=\"32\" width=\"24\" title=\"Yuri\">" ); }
			if ( ( equip & 2 ) == 2 ) { sb.Append( "<img src=\"chara-icons/EST.png\" height=\"32\" width=\"24\" title=\"Estelle\">" ); }
			if ( ( equip & 4 ) == 4 ) { sb.Append( "<img src=\"chara-icons/KAR.png\" height=\"32\" width=\"24\" title=\"Karol\">" ); }
			if ( ( equip & 8 ) == 8 ) { sb.Append( "<img src=\"chara-icons/RIT.png\" height=\"32\" width=\"24\" title=\"Rita\">" ); }
			if ( ( equip & 16 ) == 16 ) { sb.Append( "<img src=\"chara-icons/RAV.png\" height=\"32\" width=\"24\" title=\"Raven\">" ); }
			if ( ( equip & 32 ) == 32 ) { sb.Append( "<img src=\"chara-icons/JUD.png\" height=\"32\" width=\"24\" title=\"Judith\">" ); }
			if ( ( equip & 64 ) == 64 ) { sb.Append( "<img src=\"chara-icons/RAP.png\" height=\"32\" width=\"24\" title=\"Repede\">" ); }
			if ( ( equip & 128 ) == 128 ) { sb.Append( "<img src=\"chara-icons/FRE.png\" height=\"32\" width=\"24\" title=\"Flynn\">" ); }
			if ( version.HasPS3Content() && ( equip & 256 ) == 256 ) { sb.Append( "<img src=\"chara-icons/PAT.png\" height=\"32\" width=\"24\" title=\"Patty\">" ); }
		}
		public static void AppendPhysicalAilmentBitfieldAsImageString( StringBuilder sb, Dictionary<uint, TSS.TSSEntry> inGameIdDict, uint physAil ) {
			if ( ( physAil & 1 ) == 1 ) { sb.Append( "<img src=\"text-icons/icon-status-13.png\" height=\"16\" width=\"16\" title=\"Death\">" ); }
			if ( ( physAil & 2 ) == 2 ) { sb.Append( "<img src=\"text-icons/icon-status-01.png\" height=\"16\" width=\"16\" title=\"Poison\">" ); }
			if ( ( physAil & 4 ) == 4 ) { sb.Append( "<img src=\"text-icons/icon-status-02.png\" height=\"16\" width=\"16\" title=\"Paralysis\">" ); }
			if ( ( physAil & 8 ) == 8 ) { sb.Append( "<img src=\"text-icons/icon-status-03.png\" height=\"16\" width=\"16\" title=\"Petrification\">" ); }
			if ( ( physAil & 16 ) == 16 ) { sb.Append( "<img src=\"text-icons/icon-status-04.png\" height=\"16\" width=\"16\" title=\"Weakness\">" ); }
			if ( ( physAil & 32 ) == 32 ) { sb.Append( "<img src=\"text-icons/icon-status-05.png\" height=\"16\" width=\"16\" title=\"Sealed Artes\">" ); }
			if ( ( physAil & 64 ) == 64 ) { sb.Append( "<img src=\"text-icons/icon-status-06.png\" height=\"16\" width=\"16\" title=\"Sealed Skills\">" ); }
			if ( ( physAil & 128 ) == 128 ) { sb.Append( "<img src=\"text-icons/icon-status-07.png\" height=\"16\" width=\"16\" title=\"Contamination\">" ); }
		}
		public static void AppendFatalStrikeIcon( StringBuilder sb, Dictionary<uint, TSS.TSSEntry> inGameIdDict, uint fstype ) {
			switch ( fstype ) {
				case 0: sb.Append( "<img src=\"menu-icons/artes-13.png\" width=\"16\" height=\"16\">" ); break;
				case 1: sb.Append( "<img src=\"menu-icons/artes-15.png\" width=\"16\" height=\"16\">" ); break;
				case 2: sb.Append( "<img src=\"menu-icons/artes-14.png\" width=\"16\" height=\"16\">" ); break;
				default: sb.Append( "[Unknown Fatal Strike Type]" ); break;
			}
		}
		public static void AppendElementIcon( StringBuilder sb, Dictionary<uint, TSS.TSSEntry> inGameIdDict, T8BTEMST.Element element ) {
			switch ( element ) {
				case T8BTEMST.Element.Fire: sb.Append( "<img src=\"text-icons/icon-element-02.png\" width=\"16\" height=\"16\">" ); break;
				case T8BTEMST.Element.Earth: sb.Append( "<img src=\"text-icons/icon-element-04.png\" width=\"16\" height=\"16\">" ); break;
				case T8BTEMST.Element.Wind: sb.Append( "<img src=\"text-icons/icon-element-01.png\" width=\"16\" height=\"16\">" ); break;
				case T8BTEMST.Element.Water: sb.Append( "<img src=\"text-icons/icon-element-05.png\" width=\"16\" height=\"16\">" ); break;
				case T8BTEMST.Element.Light: sb.Append( "<img src=\"text-icons/icon-element-03.png\" width=\"16\" height=\"16\">" ); break;
				case T8BTEMST.Element.Darkness: sb.Append( "<img src=\"text-icons/icon-element-06.png\" width=\"16\" height=\"16\">" ); break;
				case T8BTEMST.Element.Physical: sb.Append( "<img src=\"text-icons/icon-element-07.png\" width=\"16\" height=\"16\">" ); break;
				default: sb.Append( "[Unknown Element]" ); break;
			}
		}
		public static void AppendRecord( StringBuilder sb, GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, Dictionary<uint, TSS.TSSEntry> InGameIdDict, uint id ) {
			sb.Append( "<tr>" );
			if ( websiteLanguage.WantsJp() ) {
				sb.Append( "<td>" );
				sb.Append( InGameIdDict[id].StringJpnHtml( version, InGameIdDict ) );
				sb.Append( "</td>" );
			}
			if ( websiteLanguage.WantsEn() ) {
				sb.Append( "<td>" );
				sb.Append( InGameIdDict[id].StringEngHtml( version, InGameIdDict ) );
				sb.Append( "</td>" );
			}
			sb.Append( "</tr>" );
			//sb.Append( "<tr><td colspan=\"2\"><hr></td></tr>" );
		}
		public static void AppendSetting( StringBuilder sb, GameVersion Version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, Dictionary<uint, TSS.TSSEntry> InGameIdDict, uint idName, uint idDesc, uint option1 = 0, uint option2 = 0, uint option3 = 0, uint option4 = 0 ) {
			for ( int i = 0; i < 2; ++i ) {
				if ( !websiteLanguage.WantsJp() && i == 0 ) { continue; }
				if ( !websiteLanguage.WantsEn() && i == 1 ) { continue; }

				sb.Append( "<tr>" );
				sb.Append( "<td>" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( InGameIdDict[idName].GetStringHtml( i, Version, InGameIdDict ) );
				sb.Append( "</span>" );
				sb.Append( "</td>" );

				int optionCount = 0;
				if ( option1 > 0 ) { ++optionCount; }
				if ( option2 > 0 ) { ++optionCount; }
				if ( option3 > 0 ) { ++optionCount; }
				if ( option4 > 0 ) { ++optionCount; }

				if ( optionCount == 0 ) {
					sb.Append( "<td colspan=\"4\">" );
					sb.Append( "</td>" );
				} else {
					if ( option1 > 0 ) {
						if ( optionCount == 1 ) {
							sb.Append( "<td colspan=\"4\">" );
						} else {
							sb.Append( "<td>" );
						}
						sb.Append( InGameIdDict[option1].GetStringHtml( i, Version, InGameIdDict ) );
						sb.Append( "</td>" );
					}
					if ( option2 > 0 ) {
						if ( optionCount == 2 ) {
							sb.Append( "<td colspan=\"3\">" );
						} else {
							sb.Append( "<td>" );
						}
						sb.Append( InGameIdDict[option2].GetStringHtml( i, Version, InGameIdDict ) );
						sb.Append( "</td>" );
					}
					if ( option3 > 0 ) {
						if ( optionCount == 3 ) {
							sb.Append( "<td colspan=\"2\">" );
						} else {
							sb.Append( "<td>" );
						}
						sb.Append( InGameIdDict[option3].GetStringHtml( i, Version, InGameIdDict ) );
						sb.Append( "</td>" );
					}
					if ( option4 > 0 ) {
						sb.Append( "<td>" );
						sb.Append( InGameIdDict[option4].GetStringHtml( i, Version, InGameIdDict ) );
						sb.Append( "</td>" );
					}
				}
				sb.Append( "</tr>" );

				sb.Append( "<tr>" );
				sb.Append( "<td colspan=\"5\">" );
				sb.Append( InGameIdDict[idDesc].GetStringHtml( i, Version, InGameIdDict ) );
				sb.Append( "</td>" );
				sb.Append( "</tr>" );
			}
		}

		public static string TrophyNodeToHtml( GameVersion version, Dictionary<uint, TSS.TSSEntry> inGameIdDict, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, HyoutaTools.Trophy.TrophyNode jp, HyoutaTools.Trophy.TrophyNode en ) {
			var sb = new StringBuilder();
			bool wantJp = websiteLanguage.WantsJp() && jp != null;
			bool wantEn = websiteLanguage.WantsEn() && en != null;
			bool wantBoth = wantJp && wantEn;

			sb.Append( "<tr>" );

			sb.Append( "<td>" );
			sb.Append( "<img width=\"60\" height=\"60\" src=\"trophies/TROP" + jp.ID + ".PNG\"/>" );
			sb.Append( "</td>" );

			int colspan = wantBoth ? 1 : 2;

			if ( wantJp ) {
				sb.Append( "<td colspan=\"" + colspan + "\">" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( jp.Name );
				sb.Append( "</span>" );
				sb.Append( "<br/>" );
				sb.Append( jp.Detail.ToHtmlJpn( inGameIdDict, version ) );
				sb.Append( "</td>" );
			}

			if ( wantEn ) {
				sb.Append( "<td colspan=\"" + colspan + "\">" );
				sb.Append( "<span class=\"itemname\">" );
				sb.Append( en.Name );
				sb.Append( "</span>" );
				sb.Append( "<br/>" );
				sb.Append( en.Detail.ToHtmlEng( inGameIdDict, version ) );
				sb.Append( "</td>" );
			}

			sb.Append( "</tr>" );

			return sb.ToString();
		}

		public static string GetPhpUrlGameVersion( GameVersion version ) {
			switch ( version ) {
				case GameVersion.X360_US: return "360u";
				case GameVersion.X360_EU: return "360e";
				case GameVersion.PS3: return "ps3";
				case GameVersion.PC: return "pc";
				default: throw new Exception( "Unknown version " + version );
			}
		}
		public static string GetPhpUrlGameLocale( GameLocale locale ) {
			switch ( locale ) {
				case GameLocale.J: return "jp";
				default: return locale.ToString().ToLowerInvariant();
			}
		}
		public static string GetPhpUrlWebsiteLanguage( WebsiteLanguage lang ) {
			switch ( lang ) {
				case WebsiteLanguage.Jp: return "1";
				case WebsiteLanguage.En: return "2";
				case WebsiteLanguage.BothWithJpLinks: return "c1";
				case WebsiteLanguage.BothWithEnLinks: return "c2";
				default: throw new Exception( "Unknown website language " + lang );
			}
		}
		public static string GetUrl( WebsiteSection section, GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, bool phpLink, int? id = null, int? category = null, int? icon = null, string extra = null ) {
			if ( phpLink ) {
				string v = GetPhpUrlGameVersion( version );
				string l = GetPhpUrlGameLocale( locale );
				string w = GetPhpUrlWebsiteLanguage( websiteLanguage );
				string begin = "?version=" + v + versionPostfix + "&locale=" + l + "&compare=" + w;
				switch ( section ) {
					case WebsiteSection.Enemy: return begin + "&section=enemies&category=" + category + ( id != null ? "&id=" + id : "" );
					case WebsiteSection.Item: return begin + "&section=items&icon=" + icon + ( id != null ? "&id=" + id : "" );
					case WebsiteSection.Recipe: return begin + "&section=recipes" + ( id != null ? "&id=" + id : "" );
					case WebsiteSection.Skill: return begin + "&section=skills" + ( id != null ? "&id=" + id : "" );
					case WebsiteSection.Location: return begin + "&section=locations" + ( id != null ? "&id=" + id : "" );
					case WebsiteSection.Shop: return begin + "&section=shops" + ( id != null ? "&id=" + id : "" );
					case WebsiteSection.SearchPoint: return begin + "&section=searchpoint" + ( id != null ? "#searchpoint" + id : "" );
					case WebsiteSection.Skit: return begin + "&section=skit" + ( extra != null ? "&name=" + extra : "" );
					case WebsiteSection.Scenario: return begin + "&section=scenario" + ( extra != null ? "&name=" + extra : "" );
					case WebsiteSection.NecropolisMap: return begin + "&section=necropolis" + ( extra != null ? "&map=" + extra : "" );
					default: throw new Exception( "Unsupported PHP URL requested." );
				}
			} else {
				StringBuilder sb = new StringBuilder();
				sb.Append( version.ToString() );
				sb.Append( "-" );
				sb.Append( section.ToString() );
				if ( category != null ) {
					sb.Append( "-c" );
					sb.Append( category );
				}
				if ( icon != null ) {
					sb.Append( "-i" );
					sb.Append( icon );
				}
				if ( extra != null ) {
					sb.Append( "-" );
					sb.Append( extra );
				}
				sb.Append( ".html" );
				if ( id != null ) {
					sb.Append( "#" );
					sb.Append( section.ToString().ToLowerInvariant() );
					sb.Append( id );
				}
				return sb.ToString();
			}
		}
	}
}
