using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.ItemDat;

namespace HyoutaTools.Tales.Vesperia.T8BTEMST {
	public class Enemy {
		public uint[] Data;
		public float[] DataFloat;

		private uint ID;
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

		public float FatalBlue;
		public float FatalGreen;
		public float FatalRed;
		public float FatalBlueRelated;
		public float FatalGreenRelated;
		public float FatalRedRelated;

		public uint InMonsterBook;
		public uint Location;
		public int LocationWeather;
		public uint[] DropItems;
		public uint[] DropChances;
		public uint StealItem;
		public uint StealChance;

		public static int[] KnownValues = new int[] { 14, 1, 20, 21, 22, 0, 2, 5, 57, 7, 8, 9, 10, 11, 12, 13, 15, 26, 27, 28, 29, 30, 31, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 60, 61, 6, 24, 59, 17, 18, 19 };
		// 14 is always zero
		// 16 is zero except on the new giganto monsters and spiral draco
		// 32 is 100 for most, probably some % chance?
		// 46, 48, 50 identical across category, possibly related to FS bonuses?
		// 55, 58 -> stringrefs?
		// 56 -> 0 on bosses, 1 otherwise -- maybe a "can be killed with FS?" flag
		// 76 is on PS3 only, StringDicID for the dummy description

		public Enemy( System.IO.Stream stream, uint refStringStart ) {
			uint entryLength = stream.ReadUInt32().SwapEndian();
			Data = new uint[entryLength / 4];
			DataFloat = new float[entryLength / 4];
			Data[0] = entryLength;

			for ( int i = 1; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
				DataFloat[i] = Data[i].UIntToFloat();
			}

			ID = Data[1];
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

			// this is a total guess
			FatalBlue = DataFloat[17];
			FatalGreen = DataFloat[18];
			FatalRed = DataFloat[19];
			FatalBlueRelated = DataFloat[20];
			FatalGreenRelated = DataFloat[21];
			FatalRedRelated = DataFloat[22];

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

		public string GetDataAsHtml( GameVersion version, ItemDat.ItemDat items, WRLDDAT.WRLDDAT locations, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			var sb = new StringBuilder();

			sb.AppendLine( "<div id=\"enemy" + InGameID + "\">" );

			var enemyNameEntry = inGameIdDict[NameStringDicID];
			sb.Append( "<img src=\"monster-icons/44px/monster-" + IconID.ToString( "D3" ) + ".png\" title=\"" + RefString + "\"><br>" );
			sb.Append( "<span class=\"itemname\">" + VesperiaUtil.RemoveTags( enemyNameEntry.StringJPN, true, true ) + "</span><br>" );
			sb.Append( "<span class=\"itemname\">" + enemyNameEntry.StringENG + "</span><br>" );
			//sb.Append( " / Category: " + Category + "<br>" );

			sb.Append( "Level: " + Level + "<br>" );
			sb.Append( "HP: " + HP + "<br>" );
			sb.Append( "TP: " + TP + "<br>" );
			sb.Append( "PATK: " + PATK + "<br>" );
			sb.Append( "PDEF: " + PDEF + "<br>" );
			sb.Append( "MATK: " + MATK + "<br>" );
			sb.Append( "MDEF: " + MDEF + "<br>" );
			sb.Append( "AGL: " + AGL + "<br>" );
			sb.Append( "EXP: " + EXP + "<br>" );
			sb.Append( "LP: " + LP + "<br>" );
			sb.Append( "Gald: " + Gald + "<br>" );

			sb.Append( "<img src=\"menu-icons/artes-13.png\" width=\"16\" height=\"16\">" + FatalBlue );
			sb.Append( " <img src=\"menu-icons/artes-14.png\" width=\"16\" height=\"16\">" + FatalGreen );
			sb.Append( " <img src=\"menu-icons/artes-15.png\" width=\"16\" height=\"16\">" + FatalRed );
			sb.Append( "<br>" );

			if ( AttrFire != 100 ) { sb.Append( "<td><img src=\"text-icons/icon-element-02.png\"></td>: " + AttrFire + " " ); }
			if ( AttrEarth != 100 ) { sb.Append( "<td><img src=\"text-icons/icon-element-04.png\"></td>: " + AttrEarth + " " ); }
			if ( AttrWind != 100 ) { sb.Append( "<td><img src=\"text-icons/icon-element-01.png\"></td>: " + AttrWind + " " ); }
			if ( AttrWater != 100 ) { sb.Append( "<td><img src=\"text-icons/icon-element-05.png\"></td>: " + AttrWater + " " ); }
			if ( AttrLight != 100 ) { sb.Append( "<td><img src=\"text-icons/icon-element-03.png\"></td>: " + AttrLight + " " ); }
			if ( AttrDark != 100 ) { sb.Append( "<td><img src=\"text-icons/icon-element-06.png\"></td>: " + AttrDark + " " ); }
			sb.Append( "<br>" );

			if ( Location != 0 ) {
				var loc = locations.LocationIdDict[Location];
				sb.Append( "<a href=\"locations-" + version + ".html#location" + loc.LocationID + "\">" );
				if ( LocationWeather > -1 ) {
					sb.AppendLine( "<img src=\"menu-icons/weather-" + LocationWeather + ".png\" width=\"16\" height=\"16\">" );
				}
				sb.AppendLine( loc.GetLastValidName( inGameIdDict ).StringEngOrJpn + "</a><br>" );
			}

			if ( InMonsterBook == 0 ) {
				sb.AppendLine( "Not in Monster Book!<br>" );
			}

			sb.AppendLine( "Drops:<br>" );
			for ( int i = 0; i < DropItems.Length; ++i ) {
				if ( DropItems[i] != 0 ) {
					var item = items.itemIdDict[DropItems[i]];
					sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
					sb.Append( "<a href=\"items-" + version + ".html#item" + item.Data[(int)ItemData.ID] + "\">" );
					sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpn + "</a>, " + DropChances[i] + "%<br>" );
				}
			}
			sb.AppendLine( "Steal:<br>" );
			if ( StealItem != 0 ) {
				var item = items.itemIdDict[StealItem];
				sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
				sb.Append( "<a href=\"items-" + version + ".html#item" + item.Data[(int)ItemData.ID] + "\">" );
				sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpn + "</a>, " + StealChance + "%<br>" );
			}

			/*
			sb.AppendLine();
			for ( int i = 0; i < Data.Length; ++i ) {
				if ( !KnownValues.Contains( i ) ) {
					sb.Append( "~" + i + ": " + Data[i] + " ---- " + DataFloat[i] );
					sb.Append( " [" + Category + "/" + enemyNameEntry.StringEngOrJpn + "]" );
					sb.AppendLine( "<br>" );
				}
			}
			// */

			sb.AppendLine( "</div>" );


			return sb.ToString();
		}
	}
}
