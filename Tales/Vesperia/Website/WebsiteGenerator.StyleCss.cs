using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.Website {
	public partial class WebsiteGenerator {
		private static readonly string StyleCss = @"
body { background-color: #68504F; color: #EFD1AE; font-size: 16; }
.itemname { color: #FFEBD2; font-size: 20; }
.itemdesc { }
.equip { text-align: right; float: right; }
.special { text-align: right; float: right; }
table, tr, td, th { padding: 0px 4px 0px 0px; border-spacing: 0px; }
td, td > a { vertical-align: top; }
a:link, a:visited, a:hover, a:active { color: #FFEBD2; }
table.element { display: inline-block; }
table.element td { text-align: center; padding: 0px 8px 0px 8px; border-spacing: 0px; }
table.synopsis { margin: 0px auto; }
table.synopsis td { padding: 0px 10px 0px 10px; border-spacing: 10px; }
.synopsistitle { text-align: center; color: #FFEBD2; font-size: 20; }
table.necropolisfloor td { width: 200px; height: 150px; padding: 0; }
td.necropolistile1 div.necropolis-data { background-color: black; }
td.necropolistile2 div.necropolis-data { background-color: #701010; }
td.necropolistile3 div.necropolis-data { background-color: #203860; }
td.necropolistile4 div.necropolis-data { background-color: #204040; }
td.necropolistile5 div.necropolis-data { background-color: #701010; }
div.necropolis-arrow-up    { float: left;  width: 200px; height: 16px;  text-align: center; }
div.necropolis-arrow-down  { float: left;  width: 200px; height: 16px;  text-align: center; }
div.necropolis-arrow-side  { float: left;  width: 16px; height: 118px;  text-align: center; line-height: 118px; position:relative; min-height: 16px; }
div.necropolis-arrow-side img { vertical-align: middle; max-height: 118px; position: absolute; top: 0; bottom: 0; left: 0; margin: auto; }
div.necropolis-data        { float: left;  width: 166px; height: 116px; text-align: center; border: 1px solid black; }
div.necropolis-data table  { margin: 0px auto; }
div.necropolis-data td     { width: auto; height: auto; text-align: center; padding: 0px 4px 0px 4px; }
td.skilljpn { white-space: nowrap; }
td.skilldata { width: 275px; }
.strategycat { color: #FFEBD2; }
.strategychar { text-align: center; }
table.settings td { vertical-align: middle; min-width: 90px; }
.difficultyname { font-weight: bold; text-decoration: underline; }
div.necropolis-select { width: 100%; }
div.necropolis-select table { text-align: center; margin: 0px auto; font-size: 20; }
div.scenario-index { float: left; text-align: left; margin-right: 14px; }
div.scenario-index-sub { width: 220px; }
span.scenario-selected { font-weight: bold; }
div.character-select { margin-bottom: 16px; }
#footer_time { display: none; }
#header-name { text-align: center; }
#topmenu { text-align: center; font-size: 22; }
#content { text-align: center; }
#content table { margin-left: auto; margin-right: auto; }
#footer { text-align: center; font-size: 20; margin-top: 24px; clear: both; }
#search { text-align: center; margin-top: 8px; }
form { margin: 0; }
";
	}
}
