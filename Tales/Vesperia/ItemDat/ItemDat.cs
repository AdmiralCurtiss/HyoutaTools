using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.ItemDat {
	public class ItemDat {
		public List<ItemDatSingle> items;
		public Dictionary<uint, ItemDatSingle> itemIdDict;

		public ItemDat( string Filename ) {
			Initialize( System.IO.File.ReadAllBytes( Filename ) );
		}

		private void Initialize( byte[] file ) {
			items = new List<ItemDatSingle>( file.Length / 0x2E4 );

			for ( int i = 0; i < file.Length; i += 0x2E4 ) {
				items.Add( new ItemDatSingle( i, file ) );
			}

			itemIdDict = new Dictionary<uint, ItemDatSingle>();
			foreach ( var item in items ) {
				itemIdDict.Add( item.Data[(int)ItemData.ID], item );
			}
		}

		public List<ItemDatSingle> GetSortedByInGameSorting() {
			return new List<ItemDatSingle>( items ).OrderBy( x => x.Data[(int)ItemData.SortByIdInteger] ).ToList();
		}



		public static string GetItemDataAsText( GameVersion version, ItemDat items, ItemDatSingle item, T8BTSK.T8BTSK skills, T8BTEMST.T8BTEMST enemies, COOKDAT.COOKDAT Recipes, WRLDDAT.WRLDDAT Locations, TSS.TSSFile tss, Dictionary<uint, TSS.TSSEntry> dict = null ) {
			if ( dict == null ) { dict = tss.GenerateInGameIdDictionary(); }
			var sb = new StringBuilder();

			sb.AppendLine( "[" + item.ItemString.TrimNull() + "]" );
			sb.Append( "[Icon" + item.Data[(int)ItemData.Icon] + "] " );
			var nameEntry = dict[item.NamePointer];
			sb.AppendLine( nameEntry.StringEngOrJpn );
			var descEntry = dict[item.DescriptionPointer];
			sb.AppendLine( descEntry.StringEngOrJpn );

			switch ( item.Data[(int)ItemData.Category] ) {
				case 2: sb.AppendLine( "<Tools>" ); break;
				case 3: sb.AppendLine( "<Main>" ); break;
				case 4: sb.AppendLine( "<Sub>" ); break;
				case 5: sb.AppendLine( "<Head>" ); break;
				case 6: sb.AppendLine( "<Body>" ); break;
				case 7: sb.AppendLine( "<Accessories>" ); break;
				case 8: sb.AppendLine( "<Ingredients>" ); break;
				case 9: sb.AppendLine( "<Synthesis Materials>" ); break;
				case 10: sb.AppendLine( "<Valuables>" ); break;
				case 11: sb.AppendLine( "<DLC>" ); break;
				default: sb.AppendLine( "<UNKNOWN>" ); break;
			}

			sb.AppendLine( "Price in shops: " + item.Data[(int)ItemData.ShopPrice] + " Gald" );

			if ( item.Data[(int)ItemData.BuyableIn1] > 0 || item.Data[(int)ItemData.BuyableIn2] > 0 || item.Data[(int)ItemData.BuyableIn3] > 0 ) {
				sb.Append( "Available at shops in: " );
				if ( item.Data[(int)ItemData.BuyableIn1] > 0 ) { sb.Append( Locations.LocationIdDict[item.Data[(int)ItemData.BuyableIn1]].GetLastValidName( dict ).StringEngOrJpn ); }
				if ( item.Data[(int)ItemData.BuyableIn2] > 0 ) { sb.Append( "; " + Locations.LocationIdDict[item.Data[(int)ItemData.BuyableIn2]].GetLastValidName( dict ).StringEngOrJpn ); }
				if ( item.Data[(int)ItemData.BuyableIn3] > 0 ) { sb.Append( "; " + Locations.LocationIdDict[item.Data[(int)ItemData.BuyableIn3]].GetLastValidName( dict ).StringEngOrJpn ); }
				sb.AppendLine();
			}

			uint equip = item.Data[(int)ItemData.EquippableByBitfield];
			if ( equip > 0 ) {
				sb.Append( "Equippable by: " );
				if ( ( equip & 1 ) == 1 ) { sb.Append( "[YUR]" ); }
				if ( ( equip & 2 ) == 2 ) { sb.Append( "[EST]" ); }
				if ( ( equip & 4 ) == 4 ) { sb.Append( "[KAR]" ); }
				if ( ( equip & 8 ) == 8 ) { sb.Append( "[RIT]" ); }
				if ( ( equip & 16 ) == 16 ) { sb.Append( "[RAV]" ); }
				if ( ( equip & 32 ) == 32 ) { sb.Append( "[JUD]" ); }
				if ( ( equip & 64 ) == 64 ) { sb.Append( "[RAP]" ); }
				if ( ( equip & 128 ) == 128 ) { sb.Append( "[FRE]" ); }
				if ( version == GameVersion.PS3 && ( equip & 256 ) == 256 ) { sb.Append( "[PAT]" ); }
				sb.AppendLine();
			}

			uint synthCount = item.Data[(int)ItemData.SynthRecipeCount];
			switch ( synthCount ) {
				case 0: sb.AppendLine( "Can't be synthesized." ); break;
				case 1: sb.AppendLine( "Can be synthesized in 1 way." ); break;
				default: sb.AppendLine( "Can be synthesized in " + synthCount + " ways." ); break;
			}
			for ( int j = 0; j < synthCount; ++j ) {
				uint synthItemCount = item.Data[(int)ItemData.Synth1ItemSlotCount + j * 16];
				sb.AppendLine( "Synthesis method #" + ( j + 1 ) );
				sb.AppendLine( " Required Synthesis Level: " + item.Data[(int)ItemData._Synth1Level + j * 16] );
				sb.AppendLine( " Price: " + item.Data[(int)ItemData.Synth1Price + j * 16] + " Gald" );
				sb.AppendLine( " Requires " + synthItemCount + " items:" );
				for ( int i = 0; i < synthItemCount; ++i ) {
					var otherItem = items.itemIdDict[item.Data[(int)ItemData.Synth1Item1Type + i * 2 + j * 16]];
					var otherItemNameEntry = dict[otherItem.NamePointer];
					string otherItemName = otherItemNameEntry.StringEngOrJpn;
					sb.AppendLine( "  Item " + ( i + 1 ) + ": " + otherItemName + " x" + item.Data[(int)ItemData.Synth1Item1Count + i * 2 + j * 16] );
				}
			}


			switch ( item.Data[(int)ItemData.Category] ) {
				case 2:
				default:
					// seems to be some kind of singletarget/multitarget flag maybe?
					//sb.AppendLine( "~19: " + item.Data[(int)ItemData.PATK] );

					// seems to be a bitfield regarding what stuff it heals, 1 == death, 2 = magical ailment, 4 == physical ailment
					// this is already covered below so don't print it
					//sb.AppendLine( "~20: " + item.Data[(int)ItemData.MATK] );

					if ( item.Data[(int)ItemData.MDEF_or_HPHealPercent] > 0 ) { sb.AppendLine( "HP Heal %: " + item.Data[(int)ItemData.MDEF_or_HPHealPercent] ); }
					if ( item.Data[(int)ItemData.AGL_TPHealPercent] > 0 ) { sb.AppendLine( "TP Heal %: " + item.Data[(int)ItemData.AGL_TPHealPercent] ); }

					// why is this here twice?
					uint physAilAlt = item.Data[(int)ItemData.PDEF];
					uint physAil = item.Data[(int)ItemData._LUCK];
					if ( physAil != physAilAlt ) { throw new Exception(); }

					if ( physAil > 0 ) {
						sb.Append( "Cures physical ailments: " );
						if ( ( physAil & 1 ) == 1 ) { sb.Append( "Death " ); }
						if ( ( physAil & 2 ) == 2 ) { sb.Append( "Poison " ); }
						if ( ( physAil & 4 ) == 4 ) { sb.Append( "Paralysis " ); }
						if ( ( physAil & 8 ) == 8 ) { sb.Append( "Petrification " ); }
						if ( ( physAil & 16 ) == 16 ) { sb.Append( "Weak " ); }
						if ( ( physAil & 32 ) == 32 ) { sb.Append( "SealedArtes " ); }
						if ( ( physAil & 64 ) == 64 ) { sb.Append( "SealedSkills " ); }
						if ( ( physAil & 128 ) == 128 ) { sb.Append( "Contamination " ); }
						sb.AppendLine();
					}

					if ( item.Data[(int)ItemData._AGL_Again] > 0 ) {
						sb.AppendLine( "Cures magical ailments" );
					}

					if ( item.Data[26] > 0 ) { sb.AppendLine( "Permanent PATK increase: " + item.Data[26] ); }
					if ( item.Data[27] > 0 ) { sb.AppendLine( "Permanent PDEF increase: " + item.Data[27] ); }
					if ( item.Data[(int)ItemData.AttrFire] > 0 ) { sb.AppendLine( "Permanent MATK increase: " + item.Data[(int)ItemData.AttrFire] ); }
					if ( item.Data[(int)ItemData.AttrWater] > 0 ) { sb.AppendLine( "Permanent MDEF increase: " + item.Data[(int)ItemData.AttrWater] ); }
					if ( item.Data[(int)ItemData.AttrWind] > 0 ) { sb.AppendLine( "Permanent AGL increase: " + item.Data[(int)ItemData.AttrWind] ); }
					if ( item.Data[(int)ItemData.Skill1] > 0 ) { sb.AppendLine( "Max HP increase: " + item.Data[(int)ItemData.Skill1] ); }
					if ( item.Data[(int)ItemData.Skill1Metadata] > 0 ) { sb.AppendLine( "Max TP increase: " + item.Data[(int)ItemData.Skill1Metadata] ); }
					break;

				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
					if ( (int)item.Data[(int)ItemData.PATK] > 0 ) { sb.AppendLine( "PATK: " + (int)item.Data[(int)ItemData.PATK] ); }
					if ( (int)item.Data[(int)ItemData.MATK] > 0 ) { sb.AppendLine( "MATK: " + (int)item.Data[(int)ItemData.MATK] ); }
					if ( (int)item.Data[(int)ItemData.PDEF] > 0 ) { sb.AppendLine( "PDEF: " + (int)item.Data[(int)ItemData.PDEF] ); }
					if ( (int)item.Data[(int)ItemData.MDEF_or_HPHealPercent] > 0 ) { sb.AppendLine( "MDEF: " + (int)item.Data[(int)ItemData.MDEF_or_HPHealPercent] ); }

					int agl1 = (int)item.Data[(int)ItemData.AGL_TPHealPercent];
					int agl2 = (int)item.Data[(int)ItemData._AGL_Again];

					if ( agl1 != agl2 ) {
						sb.AppendLine( "!!! AGL1: " + agl1 + " / AGL2: " + agl2 );
					} else {
						if ( agl1 > 0 ) { sb.AppendLine( "AGL: " + agl1 ); }
					}

					if ( (int)item.Data[(int)ItemData._LUCK] > 0 ) { sb.AppendLine( "LUCK: " + (int)item.Data[(int)ItemData._LUCK] ); }

					if ( (int)item.Data[(int)ItemData.AttrFire] != 0 ) { sb.AppendLine( "Attribute Fire: " + (int)item.Data[(int)ItemData.AttrFire] ); }
					if ( (int)item.Data[(int)ItemData.AttrWater] != 0 ) { sb.AppendLine( "Attribute Water: " + (int)item.Data[(int)ItemData.AttrWater] ); }
					if ( (int)item.Data[(int)ItemData.AttrWind] != 0 ) { sb.AppendLine( "Attribute Wind: " + (int)item.Data[(int)ItemData.AttrWind] ); }
					if ( (int)item.Data[(int)ItemData.AttrEarth] != 0 ) { sb.AppendLine( "Attribute Earth: " + (int)item.Data[(int)ItemData.AttrEarth] ); }
					if ( (int)item.Data[(int)ItemData.AttrLight] != 0 ) { sb.AppendLine( "Attribute Light: " + (int)item.Data[(int)ItemData.AttrLight] ); }
					if ( (int)item.Data[(int)ItemData.AttrDark] != 0 ) { sb.AppendLine( "Attribute Darkness: " + (int)item.Data[(int)ItemData.AttrDark] ); }

					for ( int i = 0; i < 3; ++i ) {
						uint skillId = item.Data[(int)ItemData.Skill1 + i * 2];
						if ( skillId != 0 ) {
							var skill = skills.SkillIdDict[skillId];
							var skillNameEntry = dict[skill.NameStringDicID];
							string skillName = skillNameEntry.StringEngOrJpn;
							sb.AppendLine( "Skill #" + ( i + 1 ) + " Name: " + skillName );
							sb.AppendLine( "Skill #" + ( i + 1 ) + " Metadata: " + item.Data[(int)ItemData.Skill1Metadata + i * 2] );
						}
					}
					break;
			}



			for ( int j = 0; j < 2; ++j ) {
				for ( int i = 0; i < 16; ++i ) {
					uint enemyId = item.Data[(int)ItemData.Drop1Enemy + i + j * 32];
					if ( enemyId != 0 ) {
						var enemy = enemies.EnemyIdDict[enemyId];
						var enemyNameEntry = dict[enemy.NameStringDicID];
						string enemyName = enemyNameEntry.StringEngOrJpn;
						sb.AppendLine( "Enemy " + ( j == 0 ? "Drop" : "Steal" ) + " #" + ( i + 1 ) + ": " + enemyName + ", " + item.Data[(int)ItemData.Drop1Chance + i + j * 32] + "%" );
					}
				}
			}

			for ( int i = 0; i < 8; ++i ) {
				if ( item.Data[(int)ItemData.UsedInRecipe1 + i] != 0 ) {
					sb.AppendLine( "Used in Recipe #" + ( i + 1 ) + ": " + item.Data[(int)ItemData.UsedInRecipe1 + i] );
				}
			}

			//sb.AppendLine( "~3: " + item.Data[3] );
			//sb.AppendLine( "~5: " + item.Data[5] );
			//sb.AppendLine( "~18: " + item.Data[18] );

			/* all of these values make no sense to me, probably useless for the reader
			sb.AppendLine( "~169: " + item.Data[169] );
			sb.AppendLine( "~170: " + item.Data[170] );
			sb.AppendLine( "~171: " + item.Data[171] );
			sb.AppendLine( "~172: " + item.Data[172] );
			sb.AppendLine( "~173: " + item.Data[173] );
			sb.AppendLine( "~174: " + item.Data[174] );
			 */

			// no idea, maybe related to what shows up on the character model?
			//sb.AppendLine( "~175: " + (int)item.Data[175] );

			// seems to be some sort of ID, useless for the reader
			//sb.AppendLine( "~176: " + item.Data[176] );

			if ( item.Data[(int)ItemData.UsableInBattle] > 0 ) { sb.AppendLine( "Usable in battle" ); };
			if ( item.Data[(int)ItemData.InCollectorsBook] == 0 ) { sb.AppendLine( "Not in Collector's Book" ); }

			return sb.ToString();
		}

		public static string GetItemDataAsHtml( GameVersion version, ItemDat items, ItemDatSingle item, T8BTSK.T8BTSK skills, T8BTEMST.T8BTEMST enemies, COOKDAT.COOKDAT Recipes, WRLDDAT.WRLDDAT Locations, TSS.TSSFile tss, Dictionary<uint, TSS.TSSEntry> dict = null, bool phpLinks = false ) {
			if ( dict == null ) { dict = tss.GenerateInGameIdDictionary(); }
			var sb = new StringBuilder();

			sb.Append( "<tr id=\"item" + item.Data[(int)ItemData.ID] + "\">" );
			sb.Append( "<td rowspan=\"3\">" );

			sb.Append( "<img src=\"items/U_" + item.ItemString.TrimNull() + ".png\" height=\"128\" width=\"128\">" );
			sb.Append( "</td><td colspan=\"2\">" );

			uint equip = item.Data[(int)ItemData.EquippableByBitfield];
			if ( equip > 0 ) {
				sb.Append( "<span class=\"equip\">" );
				Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, version, equip );
				sb.Append( "</span>" );
			}


			var nameEntry = dict[item.NamePointer];
			var descEntry = dict[item.DescriptionPointer];

			sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
			sb.Append( "<span class=\"itemname\">" );
			sb.Append( nameEntry.StringJpnHtml( version ) );
			sb.Append( "</span>" );
			sb.Append( "<br>" );
			sb.Append( "<span class=\"itemdesc\">" );
			sb.Append( descEntry.StringJpnHtml( version ) );
			sb.Append( "</span>" );
			sb.Append( "<br>" );
			sb.Append( "<br>" );

			sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
			sb.Append( "<span class=\"itemname\">" );
			sb.Append( nameEntry.StringEngHtml( version ) );
			sb.Append( "</span>" );
			sb.Append( "<br>" );
			sb.Append( "<span class=\"itemdesc\">" );
			sb.Append( descEntry.StringEngHtml( version ) );
			sb.Append( "</span>" );

			sb.Append( "<span class=\"special\">" );
			if ( item.Data[(int)ItemData.UsableInBattle] > 0 ) { sb.Append( "Usable in battle" ); };
			if ( item.Data[(int)ItemData.InCollectorsBook] == 0 ) { sb.Append( "Not in Collector's Book" ); }
			sb.Append( "</span>" );
			sb.Append( "</td>" );

			uint synthCount = item.Data[(int)ItemData.SynthRecipeCount];
			switch ( synthCount ) {
				case 0: break;
				case 1: sb.Append( "<td colspan=\"2\">" ); break;
				default: sb.Append( "<td>" ); break;
			}
			for ( int j = 0; j < synthCount; ++j ) {
				uint synthItemCount = item.Data[(int)ItemData.Synth1ItemSlotCount + j * 16];
				sb.Append( "Synthesis Level: " + item.Data[(int)ItemData._Synth1Level + j * 16] );
				sb.Append( "<br>" );
				sb.Append( "Price: " + item.Data[(int)ItemData.Synth1Price + j * 16] + " Gald" );
				for ( int i = 0; i < synthItemCount; ++i ) {
					sb.Append( "<br>" );
					var otherItem = items.itemIdDict[item.Data[(int)ItemData.Synth1Item1Type + i * 2 + j * 16]];
					var otherItemNameEntry = dict[otherItem.NamePointer];
					string otherItemName = otherItemNameEntry.StringEngOrJpnHtml( version );
					sb.Append( "<img src=\"item-icons/ICON" + otherItem.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
					sb.Append( "<a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Item, version, phpLinks, id: (int)otherItem.Data[(int)ItemData.ID], icon: (int)otherItem.Data[(int)ItemData.Icon] ) + "\">" );
					sb.Append( otherItemName + "</a> x" + item.Data[(int)ItemData.Synth1Item1Count + i * 2 + j * 16] );
				}
				if ( synthCount > 1 && j == 0 ) { sb.Append( "</td><td>" ); }
			}

			sb.Append( "</td></tr><tr>" );

			uint category = item.Data[(int)ItemData.Category];
			switch ( category ) {
				case 2:
				default:
					sb.Append( "<td colspan=\"2\">" );
					if ( item.Data[(int)ItemData.MDEF_or_HPHealPercent] > 0 ) { sb.Append( "HP Heal %: " + item.Data[(int)ItemData.MDEF_or_HPHealPercent] + "<br>" ); }
					if ( item.Data[(int)ItemData.AGL_TPHealPercent] > 0 ) { sb.Append( "TP Heal %: " + item.Data[(int)ItemData.AGL_TPHealPercent] + "<br>" ); }

					// why is this here twice?
					uint physAilAlt = item.Data[(int)ItemData.PDEF];
					uint physAil = item.Data[(int)ItemData._LUCK];
					if ( physAil != physAilAlt ) { throw new Exception(); }

					if ( physAil > 0 ) {
						sb.Append( "Cures physical ailments: " );
						if ( ( physAil & 1 ) == 1 ) { sb.Append( "<img src=\"text-icons/icon-status-13.png\" height=\"32\" width=\"32\">" ); }
						if ( ( physAil & 2 ) == 2 ) { sb.Append( "<img src=\"text-icons/icon-status-01.png\" height=\"32\" width=\"32\">" ); }
						if ( ( physAil & 4 ) == 4 ) { sb.Append( "<img src=\"text-icons/icon-status-02.png\" height=\"32\" width=\"32\">" ); }
						if ( ( physAil & 8 ) == 8 ) { sb.Append( "<img src=\"text-icons/icon-status-03.png\" height=\"32\" width=\"32\">" ); }
						if ( ( physAil & 16 ) == 16 ) { sb.Append( "<img src=\"text-icons/icon-status-04.png\" height=\"32\" width=\"32\">" ); }
						if ( ( physAil & 32 ) == 32 ) { sb.Append( "<img src=\"text-icons/icon-status-05.png\" height=\"32\" width=\"32\">" ); }
						if ( ( physAil & 64 ) == 64 ) { sb.Append( "<img src=\"text-icons/icon-status-06.png\" height=\"32\" width=\"32\">" ); }
						if ( ( physAil & 128 ) == 128 ) { sb.Append( "<img src=\"text-icons/icon-status-07.png\" height=\"32\" width=\"32\">" ); }
						sb.Append( "<br>" );
					}

					if ( item.Data[(int)ItemData._AGL_Again] > 0 ) {
						sb.Append( "Cures magical ailments<br>" );
					}

					if ( item.Data[(int)ItemData.PermanentPAtkIncrease] > 0 ) { sb.Append( "Permanent PATK increase: " + item.Data[(int)ItemData.PermanentPAtkIncrease] + "<br>" ); }
					if ( item.Data[(int)ItemData.PermanentPDefIncrease] > 0 ) { sb.Append( "Permanent PDEF increase: " + item.Data[(int)ItemData.PermanentPDefIncrease] + "<br>" ); }
					if ( item.Data[(int)ItemData.AttrFire] > 0 ) { sb.Append( "Permanent MATK increase: " + item.Data[(int)ItemData.AttrFire] + "<br>" ); }
					if ( item.Data[(int)ItemData.AttrWater] > 0 ) { sb.Append( "Permanent MDEF increase: " + item.Data[(int)ItemData.AttrWater] + "<br>" ); }
					if ( item.Data[(int)ItemData.AttrWind] > 0 ) { sb.Append( "Permanent AGL increase: " + item.Data[(int)ItemData.AttrWind] + "<br>" ); }
					if ( item.Data[(int)ItemData.Skill1] > 0 ) { sb.Append( "Max HP increase: " + item.Data[(int)ItemData.Skill1] + "<br>" ); }
					if ( item.Data[(int)ItemData.Skill1Metadata] > 0 ) { sb.Append( "Max TP increase: " + item.Data[(int)ItemData.Skill1Metadata] + "<br>" ); }

					for ( int i = 0; i < 8; ++i ) {
						int recipeId = (int)item.Data[(int)ItemData.UsedInRecipe1 + i];
						if ( recipeId != 0 ) {
							var recipe = Recipes.RecipeList[recipeId];
							var recipeNameEntry = dict[recipe.NameStringDicID];
							sb.Append( "<a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Recipe, version, phpLinks, id: (int)recipe.ID ) + "\">" + recipeNameEntry.StringEngOrJpnHtml( version ) + "</a><br>" );
						}
					}

					sb.Append( "</td>" );
					break;

				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
					sb.Append( "<td>" );
					if ( (int)item.Data[(int)ItemData.PATK] > 0 ) { sb.Append( "PATK: " + (int)item.Data[(int)ItemData.PATK] + "<br>" ); }
					if ( (int)item.Data[(int)ItemData.MATK] > 0 ) { sb.Append( "MATK: " + (int)item.Data[(int)ItemData.MATK] + "<br>" ); }
					if ( (int)item.Data[(int)ItemData.PDEF] > 0 ) { sb.Append( "PDEF: " + (int)item.Data[(int)ItemData.PDEF] + "<br>" ); }
					if ( (int)item.Data[(int)ItemData.MDEF_or_HPHealPercent] > 0 ) { sb.Append( "MDEF: " + (int)item.Data[(int)ItemData.MDEF_or_HPHealPercent] + "<br>" ); }

					int agl1 = (int)item.Data[(int)ItemData.AGL_TPHealPercent];
					int agl2 = (int)item.Data[(int)ItemData._AGL_Again];

					if ( agl2 > 0 ) { sb.Append( "AGL: " + agl2 + "<br>" ); }

					if ( (int)item.Data[(int)ItemData._LUCK] > 0 ) { sb.Append( "LUCK: " + (int)item.Data[(int)ItemData._LUCK] + "<br>" ); }


					int attackElementCount = 0;
					int defenseElementCount = 0;
					for ( int i = 0; i < 6; ++i ) {
						if ( (int)item.Data[(int)ItemData.AttrFire + i] > 0 ) { attackElementCount++; }
						if ( (int)item.Data[(int)ItemData.AttrFire + i] < 0 ) { defenseElementCount++; }
					}

					if ( attackElementCount > 0 || defenseElementCount > 0 ) {
						int fire = (int)item.Data[(int)ItemData.AttrFire];
						int watr = (int)item.Data[(int)ItemData.AttrWater];
						int wind = (int)item.Data[(int)ItemData.AttrWind];
						int eart = (int)item.Data[(int)ItemData.AttrEarth];
						int lght = (int)item.Data[(int)ItemData.AttrLight];
						int dark = (int)item.Data[(int)ItemData.AttrDark];
						if ( defenseElementCount > 0 ) {
							sb.Append( "<table class=\"element\"><tr>" );
							sb.Append( "<td colspan=\"" + defenseElementCount + "\">Resistance</td>" );
							sb.Append( "</tr><tr>" );
							if ( fire < 0 ) { sb.Append( "<td><img src=\"text-icons/icon-element-02.png\"></td>" ); }
							if ( eart < 0 ) { sb.Append( "<td><img src=\"text-icons/icon-element-04.png\"></td>" ); }
							if ( wind < 0 ) { sb.Append( "<td><img src=\"text-icons/icon-element-01.png\"></td>" ); }
							if ( watr < 0 ) { sb.Append( "<td><img src=\"text-icons/icon-element-05.png\"></td>" ); }
							if ( lght < 0 ) { sb.Append( "<td><img src=\"text-icons/icon-element-03.png\"></td>" ); }
							if ( dark < 0 ) { sb.Append( "<td><img src=\"text-icons/icon-element-06.png\"></td>" ); }
							sb.Append( "</tr><tr>" );
							if ( fire < 0 ) { sb.Append( "<td>" + -fire + "%</td>" ); }
							if ( eart < 0 ) { sb.Append( "<td>" + -eart + "%</td>" ); }
							if ( wind < 0 ) { sb.Append( "<td>" + -wind + "%</td>" ); }
							if ( watr < 0 ) { sb.Append( "<td>" + -watr + "%</td>" ); }
							if ( lght < 0 ) { sb.Append( "<td>" + -lght + "%</td>" ); }
							if ( dark < 0 ) { sb.Append( "<td>" + -dark + "%</td>" ); }
							sb.Append( "</tr></table>" );
						}
						if ( attackElementCount > 0 ) {
							sb.Append( "<table class=\"element\"><tr>" );
							if ( category == 3 || category == 4 ) {
								// weapons and sub-weapons add elemental attributes to your attack
								sb.Append( "<td colspan=\"" + attackElementCount + "\">Attack Element</td>" );
							} else {
								// defensive equipment instead uses this field as a weak/resist, with weak as positive and resist as negative values
								sb.Append( "<td colspan=\"" + attackElementCount + "\">Weakness</td>" );
							}
							sb.Append( "</tr><tr>" );
							if ( fire > 0 ) { sb.Append( "<td><img src=\"text-icons/icon-element-02.png\"></td>" ); }
							if ( eart > 0 ) { sb.Append( "<td><img src=\"text-icons/icon-element-04.png\"></td>" ); }
							if ( wind > 0 ) { sb.Append( "<td><img src=\"text-icons/icon-element-01.png\"></td>" ); }
							if ( watr > 0 ) { sb.Append( "<td><img src=\"text-icons/icon-element-05.png\"></td>" ); }
							if ( lght > 0 ) { sb.Append( "<td><img src=\"text-icons/icon-element-03.png\"></td>" ); }
							if ( dark > 0 ) { sb.Append( "<td><img src=\"text-icons/icon-element-06.png\"></td>" ); }
							if ( !( category == 3 || category == 4 ) ) {
								// weapons always have a "1" here, don't print that, it's not useful
								sb.Append( "</tr><tr>" );
								if ( fire > 0 ) { sb.Append( "<td>" + fire + "%</td>" ); }
								if ( eart > 0 ) { sb.Append( "<td>" + eart + "%</td>" ); }
								if ( wind > 0 ) { sb.Append( "<td>" + wind + "%</td>" ); }
								if ( watr > 0 ) { sb.Append( "<td>" + watr + "%</td>" ); }
								if ( lght > 0 ) { sb.Append( "<td>" + lght + "%</td>" ); }
								if ( dark > 0 ) { sb.Append( "<td>" + dark + "%</td>" ); }
							}
							sb.Append( "</tr></table>" );
						}
					}

					sb.Append( "</td><td>" );

					for ( int i = 0; i < 3; ++i ) {
						uint skillId = item.Data[(int)ItemData.Skill1 + i * 2];
						if ( skillId != 0 ) {
							var skill = skills.SkillIdDict[skillId];
							var skillNameEntry = dict[skill.NameStringDicID];
							string skillName = skillNameEntry.StringEngOrJpnHtml( version );
							string skillCat = "<img src=\"skill-icons/category-" + skill.Category.ToString() + ".png\" height=\"16\" width=\"16\">";
							sb.Append( skillCat );
							sb.Append( "<a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Skill, version, phpLinks, id: (int)skill.InGameID ) + "\">" );
							sb.Append( skillName );
							sb.Append( "</a>, " + item.Data[(int)ItemData.Skill1Metadata + i * 2] + "<br>" );
						}
					}
					sb.Append( "</td>" );
					break;
			}



			sb.Append( "<td colspan=\"2\">" );


			sb.Append( item.Data[(int)ItemData.ShopPrice] + " Gald" );

			if ( item.Data[(int)ItemData.BuyableIn1] > 0 || item.Data[(int)ItemData.BuyableIn2] > 0 || item.Data[(int)ItemData.BuyableIn3] > 0 ) {
				//sb.Append( "<br>Available at shops in:" );
				for ( int i = 0; i < 3; ++i ) {
					if ( item.Data[(int)ItemData.BuyableIn1 + i] > 0 ) {
						var loc = Locations.LocationIdDict[item.Data[(int)ItemData.BuyableIn1 + i]];
						sb.Append( "<br><a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Location, version, phpLinks, id: (int)loc.LocationID ) + "\">" );
						sb.Append( loc.GetLastValidName( dict ).StringEngOrJpnHtml( version ) + "</a>" );
					}
				}
				sb.AppendLine();
			}

			sb.Append( "</td></tr><tr>" );



			// read how many drops and steals this item lists
			int[] dropStealCount = new int[2];
			for ( int j = 0; j < 2; ++j ) {
				dropStealCount[j] = 0;
				for ( int i = 0; i < 16; ++i ) {
					uint enemyId = item.Data[(int)ItemData.Drop1Enemy + i + j * 32];
					if ( enemyId != 0 ) { dropStealCount[j]++; }
				}
			}

			for ( int j = 0; j < 2; ++j ) {
				sb.Append( "<td colspan=\"2\">" );
				if ( dropStealCount[j] > 0 ) {
					int colCount = Math.Min( 4, dropStealCount[j] );
					int rowCount = ( dropStealCount[j] - 1 ) / 4 + 1;

					sb.Append( "<table class=\"element\">" );
					sb.Append( "<tr><td colspan=\"" + colCount + "\">" );
					sb.Append( j == 0 ? "Drop" : "Steal" );
					sb.Append( "</td></tr>" );

					int cellCount = 0;
					for ( int i = 0; i < 16; ++i ) {
						uint enemyId = item.Data[(int)ItemData.Drop1Enemy + i + j * 32];

						if ( enemyId != 0 ) {
							if ( cellCount % 4 == 0 ) {
								sb.Append( "<tr>" );
							}
							sb.Append( "<td>" );
							var enemy = enemies.EnemyIdDict[enemyId];
							var enemyNameEntry = dict[enemy.NameStringDicID];
							string enemyName = enemyNameEntry.StringEngOrJpnHtml( version );
							sb.Append( "<img src=\"monster-icons/44px/monster-" + enemy.IconID.ToString( "D3" ) + ".png\"><br>" );
							sb.Append( "<a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Enemy, version, phpLinks, category: (int)enemy.Category, id: (int)enemy.InGameID ) + "\">" );
							sb.Append( enemyName + "</a><br>" + item.Data[(int)ItemData.Drop1Chance + i + j * 32] + "%" );
							sb.Append( "</td>" );
							if ( cellCount % 4 == 3 ) {
								sb.Append( "</tr>" );
							}
							cellCount++;
						}
					}
					if ( cellCount % 4 != 0 ) {
						if ( cellCount > 4 ) {
							for ( int i = cellCount % 4; i < 4; ++i ) {
								sb.Append( "<td></td>" );
							}
						}
						sb.Append( "</tr>" );
					}
					sb.Append( "</table>" );
				}
				sb.Append( "</td>" );
			}

			sb.Append( "</tr>" );
			return sb.ToString();
		}
	}
}
