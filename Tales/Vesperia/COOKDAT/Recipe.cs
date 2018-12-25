using System;
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
		private static Dictionary<uint, uint> _IngredientGroupDict = null;
		private static Dictionary<uint, uint> IngredientGroupDict {
			get {
				if ( _IngredientGroupDict == null ) {
					_IngredientGroupDict = new Dictionary<uint, uint>() {
						{ 41, 33912200 }, { 42, 33912201 }, { 43, 33912202 }, { 44, 33912204 },
					};
				}
				return _IngredientGroupDict;
			}
		}

		public Recipe( System.IO.Stream stream, Util.Endianness endian ) {
			Data = new uint[0xCC / 4]; // + 0x20

			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().FromEndian( endian );
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

		public string GetDataAsHtml( GameVersion version, COOKDAT recipes, ItemDat.ItemDat items, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict, bool phpLinks = false ) {
			StringBuilder sb = new StringBuilder();
			sb.Append( "<tr id=\"recipe" + ID + "\"><td>" );
			sb.Append( "<img src=\"recipes/U_" + RefString + ".png\">" );
			sb.Append( "</td><td>" );
			sb.Append( "<span class=\"itemname\">" + inGameIdDict[NameStringDicID].StringJpnHtml( version ) + "</span><br>" );
			sb.Append( inGameIdDict[DescriptionStringDicID].StringJpnHtml( version ) + "<br>" );
			sb.Append( inGameIdDict[EffectStringDicID].StringJpnHtml( version ) + "<br>" );
			sb.Append( "<br>" );
			sb.Append( "<span class=\"itemname\">" + inGameIdDict[NameStringDicID].StringEngHtml( version ) + "</span><br>" );
			sb.Append( inGameIdDict[DescriptionStringDicID].StringEngHtml( version ) + "<br>" );
			sb.Append( inGameIdDict[EffectStringDicID].StringEngHtml( version ) + "<br>" );

			sb.Append( "</td><td>" );
			for ( int i = 0; i < IngredientGroups.Length; ++i ) {
				if ( IngredientGroups[i] != 0 ) {
					uint stringDicId;
					stringDicId = IngredientGroupDict[IngredientGroups[i]];
					var entry = inGameIdDict[stringDicId];
					sb.Append( "<img src=\"item-icons/ICON" + IngredientGroups[i] + ".png\" height=\"16\" width=\"16\"> " );
					sb.Append( entry.StringEngOrJpnHtml( version ) + " x" + IngredientGroupCount[i] + "<br>" );
				}
			}
			for ( int i = 0; i < Ingredients.Length; ++i ) {
				if ( Ingredients[i] != 0 ) {
					var item = items.itemIdDict[Ingredients[i]];
					sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
					sb.Append( "<a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Item, version, phpLinks, id: (int)item.Data[(int)ItemData.ID], icon: (int)item.Data[(int)ItemData.Icon] ) + "\">" );
					sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpnHtml( version ) + "</a> x" + IngredientCount[i] + "<br>" );
				}
			}

			sb.Append( "</td><td>" );
			if ( HP > 0 ) { sb.Append( "HP Heal: " + HP + "%<br>" ); }
			if ( TP > 0 ) { sb.Append( "TP Heal: " + TP + "%<br>" ); }

			if ( PhysicalAilmentHeal > 0 || DeathHeal > 0 ) {
				sb.Append( "Cures Ailments: " );
				if ( DeathHeal > 0 ) { sb.Append( "<img src=\"text-icons/icon-status-13.png\" height=\"16\" width=\"16\" title=\"Death\">" ); }
				if ( ( PhysicalAilmentHeal & 1 ) == 1 ) { sb.Append( "<img src=\"text-icons/icon-status-01.png\" height=\"16\" width=\"16\" title=\"Poison\">" ); }
				if ( ( PhysicalAilmentHeal & 2 ) == 2 ) { sb.Append( "<img src=\"text-icons/icon-status-07.png\" height=\"16\" width=\"16\" title=\"Contamination\">" ); }
				if ( ( PhysicalAilmentHeal & 4 ) == 4 ) { sb.Append( "<img src=\"text-icons/icon-status-02.png\" height=\"16\" width=\"16\" title=\"Paralysis\">" ); }
				if ( ( PhysicalAilmentHeal & 8 ) == 8 ) { sb.Append( "<img src=\"text-icons/icon-status-03.png\" height=\"16\" width=\"16\" title=\"Petrification\">" ); }
				if ( ( PhysicalAilmentHeal & 16 ) == 16 ) { sb.Append( "<img src=\"text-icons/icon-status-04.png\" height=\"16\" width=\"16\" title=\"Weakness\">" ); }
				if ( ( PhysicalAilmentHeal & 32 ) == 32 ) { sb.Append( "<img src=\"text-icons/icon-status-05.png\" height=\"16\" width=\"16\" title=\"Sealed Artes\">" ); }
				if ( ( PhysicalAilmentHeal & 64 ) == 64 ) { sb.Append( "<img src=\"text-icons/icon-status-06.png\" height=\"16\" width=\"16\" title=\"Sealed Skills\">" ); }
				sb.Append( "<br>" );
			}

			if ( StatValue > 0 ) {
				//sb.Append( "Stat Type: " + StatType + "<br>" );
				switch ( StatType ) {
					case 1: sb.Append( "P. ATK" ); break;
					case 2: sb.Append( "P. DEF" ); break;
					case 3: sb.Append( "M. ATK" ); break;
					case 4: sb.Append( "M. DEF" ); break;
					case 5: sb.Append( "AGL" ); break;
					case 11: sb.Append( "Over Limit gauge increases<br>" ); break;
				}
				if ( StatType != 11 ) {
					sb.Append( " +" + StatValue + ( StatType < 5 ? "%" : "" ) + "<br>" );
					sb.Append( "Duration: " + StatTime + " seconds<br>" );
				}
			}

			if ( RecipeCreationCharacter.Count( x => x != 0 ) > 0 ) {
				sb.Append( "Recipe Evolutions:<br>" );
				for ( int i = 0; i < RecipeCreationCharacter.Length; ++i ) {
					if ( RecipeCreationCharacter[i] != 0 ) {
						var otherRecipe = recipes.RecipeList[(int)RecipeCreationRecipe[i]];
						Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, version, (uint)( 1 << (int)( RecipeCreationCharacter[i] - 1 ) ) );
						sb.Append( " <a href=\"#recipe" + otherRecipe.ID + "\">" );
						sb.Append( inGameIdDict[otherRecipe.NameStringDicID].StringEngOrJpnHtml( version ) );
						sb.Append( "</a>" );
						sb.Append( "<br>" );
					}
				}
			}

			sb.Append( "<table class=\"element\">" );
			sb.Append( "<tr>" );
			sb.Append( "<td>Likes</td>" );
			sb.Append( "<td>Dislikes</td>" );
			sb.Append( "</tr>" );

			sb.Append( "<tr>" );
			sb.Append( "<td>" );
			Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, version, CharactersLike );
			sb.Append( "</td>" );
			sb.Append( "<td>" );
			Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, version, CharactersDislike );
			sb.Append( "</td>" );
			sb.Append( "</tr>" );
			sb.Append( "<tr>" );
			sb.Append( "<td>Good at</td>" );
			sb.Append( "<td>Bad at</td>" );
			sb.Append( "</tr>" );
			sb.Append( "<tr>" );
			sb.Append( "<td>" );
			Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, version, CharactersGoodAtMaking );
			sb.Append( "</td>" );
			sb.Append( "<td>" );
			Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, version, CharactersBadAtMaking );
			sb.Append( "</td>" );
			sb.Append( "</tr>" );

			sb.Append( "</table>" );

			sb.Append( "</td></tr>" );
			return sb.ToString();
		}
	}
}
