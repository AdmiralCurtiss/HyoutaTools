﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.ItemDat;

namespace HyoutaTools.Tales.Vesperia.COOKDAT {
	public class Recipe {
		public uint[] Data;

		public uint ID;
		public uint NameStringDicID;
		public uint DescriptionStringDicID;
		public uint EffectStringDicID;
		public string RefString;

		public uint[] Ingredients;
		public uint[] IngredientCount;
		public uint[] IngredientGroups;
		public uint[] IngredientGroupCount;
		public uint[] RecipeCreationCharacter;
		public uint[] RecipeCreationRecipe;

		public uint CharactersLike;
		public uint CharactersDislike;
		public uint CharactersGoodAtMaking;
		public uint CharactersBadAtMaking;

		public uint HP;
		public uint TP;
		public uint DeathHeal;
		public uint PhysicalAilmentHeal;
		public uint StatType;
		public uint StatValue;
		public uint StatTime;

		// not the best way to handle it but I can't find a direct reference to the entries in question
		private static Dictionary<uint, uint> _IngredientGroupDictX360 = null;
		private static Dictionary<uint, uint> IngredientGroupDictX360 {
			get {
				if ( _IngredientGroupDictX360 == null ) {
					_IngredientGroupDictX360 = new Dictionary<uint, uint>() {
						{ 41, 410 }, { 42, 411 }, { 43, 412 }, { 44, 414 },
					};
				}
				return _IngredientGroupDictX360;
			}
		}
		private static Dictionary<uint, uint> _IngredientGroupDictPS3 = null;
		private static Dictionary<uint, uint> IngredientGroupDictPS3 {
			get {
				if ( _IngredientGroupDictPS3 == null ) {
					_IngredientGroupDictPS3 = new Dictionary<uint, uint>() {
						{ 41, 578 }, { 42, 579 }, { 43, 580 }, { 44, 582 },
					};
				}
				return _IngredientGroupDictPS3;
			}
		}

		public Recipe( System.IO.Stream stream ) {
			Data = new uint[0xCC / 4]; // + 0x20

			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
			}
			long pos = stream.Position;
			RefString = stream.ReadAsciiNullterm();
			stream.Position = pos + 0x20;

			ID = Data[0];
			NameStringDicID = Data[1];
			DescriptionStringDicID = Data[2];
			EffectStringDicID = Data[3];

			HP = Data[4];
			TP = Data[5];
			DeathHeal = Data[6];
			PhysicalAilmentHeal = Data[7];

			Ingredients = new uint[6];
			for ( int i = 0; i < 6; ++i ) {
				Ingredients[i] = Data[30 + i];
			}
			IngredientCount = new uint[6];
			for ( int i = 0; i < 6; ++i ) {
				IngredientCount[i] = Data[42 + i];
			}
			IngredientGroups = new uint[6];
			for ( int i = 0; i < 6; ++i ) {
				IngredientGroups[i] = Data[24 + i];
			}
			IngredientGroupCount = new uint[6];
			for ( int i = 0; i < 6; ++i ) {
				IngredientGroupCount[i] = Data[36 + i];
			}

			CharactersLike = Data[8];
			CharactersDislike = Data[9];
			CharactersGoodAtMaking = Data[10];
			CharactersBadAtMaking = Data[11];

			RecipeCreationCharacter = new uint[6];
			for ( int i = 0; i < 6; ++i ) {
				RecipeCreationCharacter[i] = Data[12 + i];
			}
			RecipeCreationRecipe = new uint[6];
			for ( int i = 0; i < 6; ++i ) {
				RecipeCreationRecipe[i] = Data[18 + i];
			}
			StatType = Data[48];
			StatValue = Data[49];
			StatTime = Data[50];
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( GameVersion version, COOKDAT recipes, ItemDat.ItemDat items, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();
			sb.Append( "<div id=\"recipe" + ID + "\">" );
			sb.Append( "<table><tr><td>" );
			sb.Append( "<img src=\"recipes/U_" + RefString + ".png\">" );
			sb.Append( "</td><td>" );
			sb.Append( "<span class=\"itemname\">" + inGameIdDict[NameStringDicID].StringEngOrJpn + "</span><br>" );
			sb.Append( inGameIdDict[DescriptionStringDicID].StringEngOrJpn.Replace( "\n", "<br>" ) + "<br>" );
			sb.Append( "<br>" + inGameIdDict[EffectStringDicID].StringEngOrJpn + "<br>" );

			sb.Append( "</td><td>" );
			for ( int i = 0; i < IngredientGroups.Length; ++i ) {
				if ( IngredientGroups[i] != 0 ) {
					uint stringDicEntryId;
					if ( version == GameVersion.PS3 ) { stringDicEntryId = IngredientGroupDictPS3[IngredientGroups[i]]; } else { stringDicEntryId = IngredientGroupDictX360[IngredientGroups[i]]; }
					var entry = stringDic.Entries[stringDicEntryId];
					sb.Append( "<img src=\"item-icons/ICON" + IngredientGroups[i] + ".png\" height=\"16\" width=\"16\"> " );
					sb.Append( entry.StringEngOrJpn + " x" + IngredientGroupCount[i] + "<br>" );
				}
			}
			for ( int i = 0; i < Ingredients.Length; ++i ) {
				if ( Ingredients[i] != 0 ) {
					var item = items.itemIdDict[Ingredients[i]];
					sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
					sb.Append( "<a href=\"items-" + version + ".html#item" + item.Data[(int)ItemData.ID] + "\">" );
					sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpn + "</a> x" + IngredientCount[i] + "<br>" );
				}
			}

			sb.Append( "</td><td>" );
			if ( HP > 0 ) { sb.Append( "HP Heal: " + HP + "%<br>" ); }
			if ( TP > 0 ) { sb.Append( "HP Heal: " + TP + "%<br>" ); }

			if ( PhysicalAilmentHeal > 0 || DeathHeal > 0 ) {
				sb.Append( "Cures Ailments: " );
				if ( DeathHeal > 0 ) { sb.Append( "<img src=\"text-icons/icon-status-13.png\" height=\"32\" width=\"32\">" ); }
				if ( ( PhysicalAilmentHeal & 1 ) == 1 ) { sb.Append( "<img src=\"text-icons/icon-status-01.png\" height=\"32\" width=\"32\">" ); }
				if ( ( PhysicalAilmentHeal & 2 ) == 2 ) { sb.Append( "<img src=\"text-icons/icon-status-07.png\" height=\"32\" width=\"32\">" ); }
				if ( ( PhysicalAilmentHeal & 4 ) == 4 ) { sb.Append( "<img src=\"text-icons/icon-status-02.png\" height=\"32\" width=\"32\">" ); }
				if ( ( PhysicalAilmentHeal & 8 ) == 8 ) { sb.Append( "<img src=\"text-icons/icon-status-03.png\" height=\"32\" width=\"32\">" ); }
				if ( ( PhysicalAilmentHeal & 16 ) == 16 ) { sb.Append( "<img src=\"text-icons/icon-status-04.png\" height=\"32\" width=\"32\">" ); }
				if ( ( PhysicalAilmentHeal & 32 ) == 32 ) { sb.Append( "<img src=\"text-icons/icon-status-05.png\" height=\"32\" width=\"32\">" ); }
				if ( ( PhysicalAilmentHeal & 64 ) == 64 ) { sb.Append( "<img src=\"text-icons/icon-status-06.png\" height=\"32\" width=\"32\">" ); }
				sb.Append( "<br>" );
			}

			if ( StatValue > 0 ) {
				sb.Append( "Stat Type: " + StatType + "<br>" );
				switch ( StatType ) {
					case 1: sb.Append( "PATK" ); break;
					case 2: sb.Append( "PDEF" ); break;
					case 3: sb.Append( "MATK" ); break;
					case 4: sb.Append( "MDEF" ); break;
					case 5: sb.Append( "AGL" ); break;
					case 11: sb.Append( "Over Limit" ); break;
				}
				sb.Append( " Increase: " + StatValue + "%<br>" );
				sb.Append( "Duration: " + StatTime + " seconds<br>" );
			}

			for ( int i = 0; i < RecipeCreationCharacter.Length; ++i ) {
				if ( RecipeCreationCharacter[i] != 0 ) {
					Website.GenerateWebsite.AppendCharacterBitfieldAsImageString( sb, version, (uint)( 1 << (int)( RecipeCreationCharacter[i] - 1 ) ) );
					sb.Append( " - " + inGameIdDict[recipes.RecipeList[(int)RecipeCreationRecipe[i]].NameStringDicID].StringEngOrJpn + "<br>" );
				}
			}

			sb.Append( "Likes: " );
			Website.GenerateWebsite.AppendCharacterBitfieldAsImageString( sb, version, CharactersLike );
			sb.Append( "<br>" );
			sb.Append( "Dislikes: " );
			Website.GenerateWebsite.AppendCharacterBitfieldAsImageString( sb, version, CharactersDislike );
			sb.Append( "<br>" );
			sb.Append( "Good at making: " );
			Website.GenerateWebsite.AppendCharacterBitfieldAsImageString( sb, version, CharactersGoodAtMaking );
			sb.Append( "<br>" );
			sb.Append( "Bad at making: " );
			Website.GenerateWebsite.AppendCharacterBitfieldAsImageString( sb, version, CharactersBadAtMaking );
			sb.Append( "<br>" );

			sb.Append( "</td></tr></table>" );
			sb.Append( "</div>" );
			return sb.ToString();
		}
	}
}
