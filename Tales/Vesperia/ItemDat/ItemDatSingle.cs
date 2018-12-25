using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.ItemDat {
	public enum ItemData {
		ID = 0,
		NamePointer = 1,
		ShopPrice = 2,
		DescriptionPointer = 0x11,
		UnknownTextPointer = 43,
		TextIDPart1 = 8,
		TextIDPart2 = 9,
		TextIDPart3 = 10,
		TextIDPart4 = 11,
		TextIDPart5 = 12,
		TextIDPart6 = 13,
		TextIDPart7 = 14,
		TextIDPart8 = 15,
		MDEF_or_HPHealPercent = 22,
		AGL_TPHealPercent = 23,
		_AGL_Again = 25,
		_LUCK = 24,

		_Synth1Level = 120,
		Synth1Price = 121,
		Synth1Item1Type = 123,
		Synth1Item1Count = 124,
		Synth1Item2Type = 125,
		Synth1Item2Count = 126,
		Synth1Item3Type = 127,
		Synth1Item3Count = 128,
		Synth1Item4Type = 129,
		Synth1Item4Count = 130,
		Synth1Item5Type = 131,
		Synth1Item5Count = 132,
		Synth1Item6Type = 133,
		Synth1Item6Count = 134,
		Synth1ItemSlotCount = 135,

		_Synth2Level = 136,
		Synth2Price = 137,
		Synth2Item1Type = 139,
		Synth2Item1Count = 140,
		Synth2Item2Type = 141,
		Synth2Item2Count = 142,
		Synth2Item3Type = 143,
		Synth2Item3Count = 144,
		Synth2Item4Type = 145,
		Synth2Item4Count = 146,
		Synth2Item5Type = 147,
		Synth2Item5Count = 148,
		Synth2Item6Type = 149,
		Synth2Item6Count = 150,
		Synth2ItemSlotCount = 151,

		_Synth3Level = 152,
		Synth3Price = 153,
		Synth3Item1Type = 155,
		Synth3Item1Count = 156,
		Synth3Item2Type = 157,
		Synth3Item2Count = 158,
		Synth3Item3Type = 159,
		Synth3Item3Count = 160,
		Synth3Item4Type = 161,
		Synth3Item4Count = 162,
		Synth3Item5Type = 163,
		Synth3Item5Count = 164,
		Synth3Item6Type = 165,
		Synth3Item6Count = 166,
		Synth3ItemSlotCount = 167,

		Skill1 = 34,
		Skill2 = 36,
		Skill3 = 38,
		Skill1Metadata = 35,
		Skill2Metadata = 37,
		Skill3Metadata = 39,

		EquippableByBitfield = 4,
		Category = 7,
		PATK = 19,
		MATK = 20,
		PDEF = 21,
		SynthRecipeCount = 168,
		Icon = 6,
		BuyableIn1 = 108,
		BuyableIn2 = 109,
		BuyableIn3 = 110,

		Drop1Enemy = 44,
		Drop2Enemy = 45,
		Drop3Enemy = 46,
		Drop4Enemy = 47,
		Drop5Enemy = 48,
		Drop6Enemy = 49,
		Drop7Enemy = 50,
		Drop8Enemy = 51,
		Drop9Enemy = 52,
		Drop10Enemy = 53,
		Drop11Enemy = 54,
		Drop12Enemy = 55,
		Drop13Enemy = 56,
		Drop14Enemy = 57,
		Drop15Enemy = 58,
		Drop16Enemy = 59,

		Drop1Chance = 60,
		Drop2Chance = 61,
		Drop3Chance = 62,
		Drop4Chance = 63,
		Drop5Chance = 64,
		Drop6Chance = 65,
		Drop7Chance = 66,
		Drop8Chance = 67,
		Drop9Chance = 68,
		Drop10Chance = 69,
		Drop11Chance = 70,
		Drop12Chance = 71,
		Drop13Chance = 72,
		Drop14Chance = 73,
		Drop15Chance = 74,
		Drop16Chance = 75,

		Steal1Enemy = 76,
		Steal2Enemy = 77,
		Steal3Enemy = 78,
		Steal4Enemy = 79,
		Steal5Enemy = 80,
		Steal6Enemy = 81,
		Steal7Enemy = 82,
		Steal8Enemy = 83,
		Steal9Enemy = 84,
		Steal10Enemy = 85,
		Steal11Enemy = 86,
		Steal12Enemy = 87,
		Steal13Enemy = 88,
		Steal14Enemy = 89,
		Steal15Enemy = 90,
		Steal16Enemy = 91,

		Steal1Chance = 92,
		Steal2Chance = 93,
		Steal3Chance = 94,
		Steal4Chance = 95,
		Steal5Chance = 96,
		Steal6Chance = 97,
		Steal7Chance = 98,
		Steal8Chance = 99,
		Steal9Chance = 100,
		Steal10Chance = 101,
		Steal11Chance = 102,
		Steal12Chance = 103,
		Steal13Chance = 104,
		Steal14Chance = 105,
		Steal15Chance = 106,
		Steal16Chance = 107,

		UsedInRecipe1 = 111,
		UsedInRecipe2 = 112,
		UsedInRecipe3 = 113,
		UsedInRecipe4 = 114,

		AttrFire = 28,
		AttrWater = 29,
		AttrWind = 30,
		AttrEarth = 31,
		AttrLight = 32,
		AttrDark = 33,

		Always35 = 16,

		SortByIdInteger = 176, // this seems to be used to sort the items in the item menu
		UsableInBattle = 177,
		InCollectorsBook = 178,

		PermanentPAtkIncrease = 26,
		PermanentPDefIncrease = 27,

		ItemDataCount = 0xB9
	}


	public class ItemDatSingle {
		public static int size = 0x2E4;

		public UInt32 NamePointer;
		public string ItemString;
		public UInt32 DescriptionPointer;

		public UInt32[] Data;

		public ItemDatSingle( int offset, byte[] file, Util.Endianness endian ) {
			NamePointer = BitConverter.ToUInt32( file, offset + 0x04 ).FromEndian( endian );
			ItemString = Encoding.ASCII.GetString( file, offset + 0x20, 0x20 );
			DescriptionPointer = BitConverter.ToUInt32( file, offset + 0x44 ).FromEndian( endian );

			int startRest = 0x0;
			Data = new UInt32[0x2E4 / 4];
			for ( int i = 0; i < 0x2E4 / 4; ++i ) {
				Data[i] = BitConverter.ToUInt32( file, offset + startRest + i * 0x04 ).FromEndian( endian );
			}

			return;
		}

		public override string ToString() {
			return ItemString;
		}

	}
}
