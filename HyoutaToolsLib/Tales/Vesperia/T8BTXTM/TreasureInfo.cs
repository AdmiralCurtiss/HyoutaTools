﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.T8BTXTM {
	public class TreasureInfo {
		public uint EntrySize;
		public uint ID;
		public uint IDAgain;
		public ulong RefStringLocation;

		// treasure chest types?
		public uint[] ChestTypes;

		// treasure chest positions on x/y ground plane?
		public float[] ChestPositions;

		// four slots, 0/1/2 -> possible treasures for slot 1, 3/4/5 -> for slot 2, etc.
		public uint[] Items;
		public uint[] Chances;

		public string RefString;

		public TreasureInfo( System.IO.Stream stream, uint refStringStart, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			EntrySize = stream.ReadUInt32().FromEndian( endian );
			ID = stream.ReadUInt32().FromEndian( endian );
			IDAgain = stream.ReadUInt32().FromEndian( endian );
			RefStringLocation = stream.ReadUInt( bits, endian );

			ChestTypes = new uint[4];
			for ( int i = 0; i < ChestTypes.Length; ++i ) {
				ChestTypes[i] = stream.ReadUInt32().FromEndian( endian );
			}

			ChestPositions = new float[8];
			for ( int i = 0; i < ChestPositions.Length; ++i ) {
				ChestPositions[i] = stream.ReadUInt32().FromEndian( endian ).UIntToFloat();
			}

			Items = new uint[12];
			for ( int i = 0; i < Items.Length; ++i ) {
				Items[i] = stream.ReadUInt32().FromEndian( endian );
			}
			Chances = new uint[12];
			for ( int i = 0; i < Chances.Length; ++i ) {
				Chances[i] = stream.ReadUInt32().FromEndian( endian );
			}

			RefString = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + RefStringLocation ) );
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( ItemDat.ItemDat items, Dictionary<uint, TSS.TSSEntry> inGameIdDict, GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage ) {
			StringBuilder sb = new StringBuilder();

			for ( int i = 0; i < Items.Length; ++i ) {
				if ( Items[i] > 0 ) {
					var item = items.itemIdDict[this.Items[i]];
					sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpnHtml( version, inGameIdDict, websiteLanguage ) );
					sb.Append( " / " );
					sb.Append( Chances[i] );
				}
				sb.Append( ( i % 3 == 2 ) ? "<br>" : " -- " );
			}

			return sb.ToString();
		}
	}
}
