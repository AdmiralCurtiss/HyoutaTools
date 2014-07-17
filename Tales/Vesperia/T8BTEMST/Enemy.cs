using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTEMST {
	public class Enemy {
		public uint[] Data;
		public float[] DataFloat;

		public uint NameStringDicID;
		public uint InGameID;
		public string RefString;
		public uint IconID;

		public uint Category;
		public uint Level;

		public uint HP;
		public uint TP;
		public uint PATK;
		public uint PDEF;
		public uint MATK;
		public uint MDEF;
		public uint AGL;

		public int AttrFire;
		public int AttrEarth;
		public int AttrWind;
		public int AttrWater;
		public int AttrLight;
		public int AttrDark;

		public uint EXP;
		public uint LP;
		public uint Gald;

		public uint InMonsterBook;
		public uint Location;
		public int LocationWeather;
		public uint[] DropItems;
		public uint[] DropChances;
		public uint StealItem;
		public uint StealChance;

		public static int[] KnownValues = new int[] { 0, 2, 5, 57, 7, 8, 9, 10, 11, 12, 13, 15, 26, 27, 28, 29, 30, 31, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 60, 61, 6, 24, 59 };

		public Enemy( System.IO.Stream stream, uint refStringStart ) {
			uint entryLength = stream.ReadUInt32().SwapEndian();
			Data = new uint[entryLength / 4];
			DataFloat = new float[entryLength / 4];
			Data[0] = entryLength;

			for ( int i = 1; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
				DataFloat[i] = Data[i].UIntToFloat();
			}

			NameStringDicID = Data[2];
			InGameID = Data[5];
			IconID = Data[57];

			Level = Data[7];
			Category = Data[24];

			HP = Data[8];
			TP = Data[9];
			PATK = Data[10];
			PDEF = Data[11];
			MATK = Data[12];
			MDEF = Data[13];
			AGL = Data[15];

			// > 100 weak, < 100 resist, 0 nullify, negative absorb
			// effectively a damage multiplier in percent
			AttrFire = (int)Data[26];
			AttrEarth = (int)Data[27];
			AttrWind = (int)Data[28];
			AttrWater = (int)Data[29];
			AttrLight = (int)Data[30];
			AttrDark = (int)Data[31];

			EXP = Data[33];
			Gald = Data[34];
			LP = Data[35];

			DropItems = new uint[4];
			DropItems[0] = Data[36];
			DropItems[1] = Data[37];
			DropItems[2] = Data[38];
			DropItems[3] = Data[39];
			DropChances = new uint[4];
			DropChances[0] = Data[40];
			DropChances[1] = Data[41];
			DropChances[2] = Data[42];
			DropChances[3] = Data[43];
			StealItem = Data[44];
			StealChance = Data[45];

			InMonsterBook = Data[59];
			Location = Data[60];
			// -1: None shown, 0: Sun, 1: Rain, 2: Snow, 3: Windy, 4: Night, 5: Cloudy
			LocationWeather = (int)Data[61];

			uint refStringLocation = Data[6];
			long pos = stream.Position;
			stream.Position = refStringStart + refStringLocation;
			RefString = stream.ReadAsciiNullterm();
			stream.Position = pos;
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( ItemDat.ItemDat items, WRLDDAT.WRLDDAT locations, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			var sb = new StringBuilder();

			sb.AppendLine( "<div>" );
			sb.AppendLine( RefString );
			sb.AppendLine( "<br>" );

			var enemyNameEntry = inGameIdDict[NameStringDicID];
			sb.Append( "<img src=\"monster-icons/44px/monster-" + IconID.ToString( "D3" ) + ".png\"><br>" );
			sb.Append( enemyNameEntry.StringEngOrJpn + " / Category: " + Category + "<br>" );

			sb.AppendLine( "Fire: " + AttrFire + " / " );
			sb.AppendLine( "Earth: " + AttrEarth + " / " );
			sb.AppendLine( "Wind: " + AttrWind + " / " );
			sb.AppendLine( "Water: " + AttrWater + " / " );
			sb.AppendLine( "Light: " + AttrLight + " / " );
			sb.AppendLine( "Dark: " + AttrDark + "<br>" );


			if ( Location != 0 ) {
				var loc = locations.LocationIdDict[Location];
				sb.AppendLine( "Location: " + inGameIdDict[loc.DefaultStringDicID].StringEngOrJpn + " / Weather: " + LocationWeather + "<br>" );
			}

			if ( InMonsterBook == 0 ) {
				sb.AppendLine( "Not in Monster Book!<br>" );
			}

			for ( int i = 0; i < DropItems.Length; ++i ) {
				if ( DropItems[i] != 0 ) {
					var item = items.itemIdDict[DropItems[i]];
					sb.AppendLine( inGameIdDict[item.NamePointer].StringEngOrJpn + ", " + DropChances[i] + "%<br>" );
				}
			}
			if ( StealItem != 0 ) {
				var item = items.itemIdDict[StealItem];
				sb.AppendLine( inGameIdDict[item.NamePointer].StringEngOrJpn + ", " + StealChance + "%<br>" );
			}

			/*
			for ( int i = 0; i < Data.Length; ++i ) {
				if ( !KnownValues.Contains( i ) ) {
					sb.AppendLine( i + ": " + Data[i] + " ---- " + DataFloat[i] + "<br>" );
				}
			}
			 */

			sb.AppendLine( "</div>" );


			return sb.ToString();
		}
	}
}
