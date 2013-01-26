using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.ItemDat {
	enum ItemData {
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
		Skill1 = 34,
		Skill2 = 36,
		Skill3 = 38,
		EquippableByBitfield = 4,
		Category = 7,
		PATK = 19,
		MATK = 20,
		PDEF = 21,
		_SynthRecipeCount = 168,
		Icon = 6,
		BuyableIn1 = 108,
		BuyableIn2 = 109,

		SearchPointsStuffAroundHereMaybe = 60,

		AttrFire = 28,
		AttrEarth = 29,
		AttrWind = 30,
		AttrWater = 31,
		AttrLight = 32,
		AttrDark = 33,

		ItemDataCount = 0xB9
	}


	public class ItemDatSingle {
		public static int size = 0x2E4;

		public UInt32 NamePointer;
		public string ItemString;
		public UInt32 DescriptionPointer;

		public UInt32[] UnknownRest;

		public ItemDatSingle( int offset, byte[] file ) {
			NamePointer = HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( file, offset + 0x04 ) );
			ItemString = Encoding.ASCII.GetString( file, offset + 0x20, 0x20 );
			DescriptionPointer = HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( file, offset + 0x44 ) );

			int startRest = 0x0;
			UnknownRest = new UInt32[0x2E4 / 4];
			for ( int i = 0; i < 0x2E4 / 4; ++i ) {
				UnknownRest[i] = HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( file, offset + startRest + i * 0x04 ) );
			}

			return;
		}

		public override string ToString() {
			return ItemString;
		}
	}
}
