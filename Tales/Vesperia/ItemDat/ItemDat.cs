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



		public static string GetItemDataAsText( ItemDat items, ItemDatSingle item, T8BTSK.T8BTSK skills, TSS.TSSFile tss, Dictionary<uint, TSS.TSSEntry> dict = null ) {
			if ( dict == null ) { dict = tss.GenerateInGameIdDictionary(); }
			var sb = new StringBuilder();

			sb.AppendLine( "[" + item.ItemString.TrimNull() + "]" );
			sb.Append( "[Icon" + item.Data[(int)ItemData.Icon] + "] " );
			var nameEntry = dict[item.NamePointer];
			sb.AppendLine( String.IsNullOrEmpty( nameEntry.StringENG ) ? nameEntry.StringJPN : nameEntry.StringENG );
			var descEntry = dict[item.DescriptionPointer];
			sb.AppendLine( String.IsNullOrEmpty( descEntry.StringENG ) ? descEntry.StringJPN : descEntry.StringENG );

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
				if ( item.Data[(int)ItemData.BuyableIn1] > 0 ) { sb.Append( GetShopFromId( item.Data[(int)ItemData.BuyableIn1] ) ); }
				if ( item.Data[(int)ItemData.BuyableIn2] > 0 ) { sb.Append( "; " + GetShopFromId( item.Data[(int)ItemData.BuyableIn2] ) ); }
				if ( item.Data[(int)ItemData.BuyableIn3] > 0 ) { sb.Append( "; " + GetShopFromId( item.Data[(int)ItemData.BuyableIn3] ) ); }
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
				if ( ( equip & 256 ) == 256 ) { sb.Append( "[PAT]" ); }
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
					string otherItemName = String.IsNullOrEmpty( otherItemNameEntry.StringENG ) ? otherItemNameEntry.StringJPN : otherItemNameEntry.StringENG;
					sb.AppendLine( "  Item " + ( i + 1 ) + ": " + otherItemName + " x" + item.Data[(int)ItemData.Synth1Item1Count + i * 2 + j * 16] );
				}
			}


			switch ( item.Data[(int)ItemData.Category] ) {
				case 2:
				default:
					sb.AppendLine( "~19: " + item.Data[(int)ItemData.PATK] );

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
					if ( item.Data[(int)ItemData.AttrEarth] > 0 ) { sb.AppendLine( "Permanent MDEF increase: " + item.Data[(int)ItemData.AttrEarth] ); }
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
					if ( (int)item.Data[(int)ItemData.AttrEarth] != 0 ) { sb.AppendLine( "Attribute Earth: " + (int)item.Data[(int)ItemData.AttrEarth] ); }
					if ( (int)item.Data[(int)ItemData.AttrWind] != 0 ) { sb.AppendLine( "Attribute Wind: " + (int)item.Data[(int)ItemData.AttrWind] ); }
					if ( (int)item.Data[(int)ItemData.AttrWater] != 0 ) { sb.AppendLine( "Attribute Water: " + (int)item.Data[(int)ItemData.AttrWater] ); }
					if ( (int)item.Data[(int)ItemData.AttrLight] != 0 ) { sb.AppendLine( "Attribute Light: " + (int)item.Data[(int)ItemData.AttrLight] ); }
					if ( (int)item.Data[(int)ItemData.AttrDark] != 0 ) { sb.AppendLine( "Attribute Darkness: " + (int)item.Data[(int)ItemData.AttrDark] ); }

					for ( int i = 0; i < 3; ++i ) {
						uint skillId = item.Data[(int)ItemData.Skill1 + i * 2];
						if ( skillId != 0 ) {
							var skill = skills.SkillIdDict[skillId];
							var skillNameEntry = dict[skill.NameStringDicID];
							var skillDescEntry = dict[skill.DescStringDicID];
							string skillName = String.IsNullOrEmpty( skillNameEntry.StringENG ) ? skillNameEntry.StringJPN : skillNameEntry.StringENG;
							sb.AppendLine( "Skill #" + ( i + 1 ) + " Name: " + skillName );
							sb.AppendLine( "Skill #" + ( i + 1 ) + " Metadata: " + item.Data[(int)ItemData.Skill1Metadata + i * 2] );
						}
					}
					break;
			}



			for ( int i = 0; i < 16; ++i ) {
				if ( item.Data[(int)ItemData.Drop1Enemy + i] != 0 ) {
					sb.AppendLine( "Enemy Drop #" + ( i + 1 ) + ": " + item.Data[(int)ItemData.Drop1Enemy + i] + ", " + item.Data[(int)ItemData.Drop1Chance + i] + "%" );
				}
			}
			for ( int i = 0; i < 16; ++i ) {
				if ( item.Data[(int)ItemData.Steal1Enemy + i] != 0 ) {
					sb.AppendLine( "Enemy Steal #" + ( i + 1 ) + ": " + item.Data[(int)ItemData.Steal1Enemy + i] + ", " + item.Data[(int)ItemData.Steal1Chance + i] + "%" );
				}
			}

			for ( int i = 0; i < 8; ++i ) {
				if ( item.Data[(int)ItemData.UsedInRecipe1 + i] != 0 ) {
					sb.AppendLine( "Used in Recipe #" + ( i + 1 ) + ": " + item.Data[(int)ItemData.UsedInRecipe1 + i] );
				}
			}

			sb.AppendLine( "~3: " + item.Data[3] );
			sb.AppendLine( "~5: " + item.Data[5] );
			sb.AppendLine( "~18: " + item.Data[18] );

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

			if ( item.Data[177] > 0 ) { sb.AppendLine( "Usable in battle" ); };
			if ( item.Data[178] == 0 ) { sb.AppendLine( "Not in Collector's Book" ); }

			return sb.ToString();
		}

		public static string GetShopFromId( uint id ) {
			switch ( id ) {
				case 3: return "The Imperial Capital, Zaphias";
				case 4: return "The City of Blossoms, Halure";
				case 6: return "Deidon Hold";
				case 14: return "The Coliseum City, Nordopolica";
				case 7: return "The Land of Hope, Aurnion";
				case 1: return "The Fount of Warmth, Yumanju";
				case 12: return "Port of Capua Torim";
				case 11: return "Port of Capua Nor";
				case 8: return "The Den of Guilds, Dahngrest";
				case 5: return "The Home of the Kritya, Myorzo";
				case 15: return "The Rising City, Heliord";
				case 10: return "The Desert Oasis, Mantaic";
				case 13: return "The Sealed City of Scholars, Aspio";
				case 16: return "The Heartland Town, Yormgen";
				default: return "UnknownShop" + id;
			}
		}

	}
}
