using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.ItemDat;

namespace HyoutaTools.Tales.Vesperia.T8BTEMST {
	public enum Element {
		Fire = 0,
		Earth = 1,
		Wind = 2,
		Water = 3,
		Light = 4,
		Darkness = 5,
		Physical = 6
	}
	public class Enemy {
		public uint[] Data;
		public float[] DataFloat;

		public uint ID;
		public uint NameStringDicID;
		public uint InGameID;
		public string RefString;
		public string RefString2;
		public string RefString3;
		public string RefString4;
		public string RefString5;
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

		public int[] Attributes; // fire, earth, wind, water, light, dark, phys

		public uint EXP;
		public uint LP;
		public uint Gald;

		public float FatalBlue;
		public float FatalRed;
		public float FatalGreen;
		public float FatalBlueRelated;
		public float FatalRedRelated;
		public float FatalGreenRelated;

		public uint InMonsterBook;
		public uint Location;
		public int LocationWeather;
		public uint[] DropItems;
		public uint[] DropChances;
		public uint StealItem;
		public uint StealChance;
		public uint KillableWithFS;

		public uint SecretMissionDrop;
		public uint SecretMissionDropChance;

		public uint FatalTypeExp;
		public uint EXPModifier;
		public uint FatalTypeLP;
		public uint LPModifier;
		public uint FatalTypeDrop;
		public uint[] DropModifier;

		/*
		public static int[] KnownValues = new int[] { 32, 46, 47, 48, 49, 50, 55, 58, 3, 4, 68, 69, 56, 51, 52, 53, 54, 14, 1, 20, 21, 22, 0, 2, 5, 57, 7, 8, 9, 10, 11, 12, 13, 15, 26, 27, 28, 29, 30, 31, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 60, 61, 6, 24, 59, 17, 18, 19 };
		//*/
		// 14 is always zero
		// 16 is zero except on the new giganto monsters and spiral draco
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

			FatalBlue = DataFloat[17];
			FatalRed = DataFloat[18];
			FatalGreen = DataFloat[19];
			FatalBlueRelated = DataFloat[20];
			FatalRedRelated = DataFloat[21];
			FatalGreenRelated = DataFloat[22];

			// > 100 weak, < 100 resist, 0 nullify, negative absorb
			// effectively a damage multiplier in percent
			Attributes = new int[7];
			Attributes[0] = (int)Data[26];
			Attributes[1] = (int)Data[27];
			Attributes[2] = (int)Data[28];
			Attributes[3] = (int)Data[29];
			Attributes[4] = (int)Data[30];
			Attributes[5] = (int)Data[31];
			Attributes[6] = (int)Data[32];

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

			FatalTypeExp = Data[46];
			EXPModifier = Data[47];
			FatalTypeLP = Data[48];
			LPModifier = Data[49];
			FatalTypeDrop = Data[50];
			DropModifier = new uint[4];
			DropModifier[0] = Data[51];
			DropModifier[1] = Data[52];
			DropModifier[2] = Data[53];
			DropModifier[3] = Data[54];

			KillableWithFS = Data[56];

			InMonsterBook = Data[59];
			Location = Data[60];
			// -1: None shown, 0: Sun, 1: Rain, 2: Snow, 3: Windy, 4: Night, 5: Cloudy
			LocationWeather = (int)Data[61];

			SecretMissionDrop = Data[68];
			SecretMissionDropChance = Data[69];

			long pos = stream.Position;

			stream.Position = refStringStart + Data[6];
			RefString = stream.ReadAsciiNullterm();
			stream.Position = refStringStart + Data[3];
			RefString2 = stream.ReadAsciiNullterm();
			stream.Position = refStringStart + Data[4];
			RefString3 = stream.ReadAsciiNullterm();
			stream.Position = refStringStart + Data[55];
			RefString4 = stream.ReadAsciiNullterm();
			stream.Position = refStringStart + Data[58];
			RefString5 = stream.ReadAsciiNullterm();

			stream.Position = pos;
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( GameVersion version, ItemDat.ItemDat items, WRLDDAT.WRLDDAT locations, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			var sb = new StringBuilder();

			sb.Append( "<tr id=\"enemy" + InGameID + "\">" );

			sb.Append( "<td style=\"height: 46px;\">" );
			var enemyNameEntry = inGameIdDict[NameStringDicID];
			sb.Append( "<img src=\"monster-icons/44px/monster-" + IconID.ToString( "D3" ) + ".png\" title=\"" + RefString + "\">" );
			sb.Append( "</td>" );
			sb.Append( "<td style=\"height: 46px;\">" );
			sb.Append( "<span class=\"itemname\">" + enemyNameEntry.StringJpnHtml( version ) + "</span><br>" );
			sb.Append( "<span class=\"itemname\">" + enemyNameEntry.StringEngHtml( version ) + "</span><br>" );
			//sb.Append( RefString2 + "<br>" );
			//sb.Append( RefString3 + "<br>" );
			//sb.Append( RefString4 + "<br>" );
			//sb.Append( RefString5 + "<br>" );
			sb.Append( "</td>" );
			//sb.Append( " / Category: " + Category + "<br>" );

			AppendStats( sb, "Easy", 0 );
			AppendStats( sb, "Normal", 1 );
			AppendStats( sb, "Hard", 2 );
			AppendStats( sb, "Unknown", 3 );

			sb.Append( "<td rowspan=\"2\">" );
			sb.Append( "EXP: " + EXP + ", " );
			Website.GenerateWebsite.AppendFatalStrikeIcon( sb, FatalTypeExp );
			sb.Append( " +" + EXPModifier + "<br>" );
			sb.Append( "LP: " + LP + ", " );
			Website.GenerateWebsite.AppendFatalStrikeIcon( sb, FatalTypeLP );
			sb.Append( " +" + LPModifier + "<br>" );
			sb.Append( "Gald: " + Gald + "<br>" );

			sb.Append( "Fatal Strike Resistances:<br>" );
			Website.GenerateWebsite.AppendFatalStrikeIcon( sb, 0 );
			sb.Append( FatalBlue );
			sb.Append( " " );
			Website.GenerateWebsite.AppendFatalStrikeIcon( sb, 1 );
			sb.Append( FatalRed );
			sb.Append( " " );
			Website.GenerateWebsite.AppendFatalStrikeIcon( sb, 2 );
			sb.Append( FatalGreen );
			sb.Append( "<br>" );

			int weakCount = 0;
			int strongCount = 0;
			int immuneCount = 0;
			int absorbCount = 0;
			for ( int i = 0; i < Attributes.Length; ++i ) {
				if ( Attributes[i] > 100 ) { weakCount++; }
				if ( Attributes[i] > 0 && Attributes[i] < 100 ) { strongCount++; }
				if ( Attributes[i] == 0 ) { immuneCount++; }
				if ( Attributes[i] < 0 ) { absorbCount++; }
			}

			if ( weakCount > 0 ) {
				sb.Append( "Weak: " );
				for ( int i = 0; i < Attributes.Length; ++i ) {
					if ( Attributes[i] > 100 ) {
						Website.GenerateWebsite.AppendElementIcon( sb, (Element)i );
						sb.Append( Attributes[i] - 100 );
						sb.Append( "% " );
					}
				}
				sb.Append( "<br>" );
			}
			if ( strongCount > 0 ) {
				sb.Append( "Strong: " );
				for ( int i = 0; i < Attributes.Length; ++i ) {
					if ( Attributes[i] > 0 && Attributes[i] < 100 ) {
						Website.GenerateWebsite.AppendElementIcon( sb, (Element)i );
						sb.Append( Attributes[i] );
						sb.Append( "% " );
					}
				}
				sb.Append( "<br>" );
			}
			if ( immuneCount > 0 ) {
				sb.Append( "Immune: " );
				for ( int i = 0; i < Attributes.Length; ++i ) {
					if ( Attributes[i] == 0 ) {
						Website.GenerateWebsite.AppendElementIcon( sb, (Element)i );
					}
				}
				sb.Append( "<br>" );
			}
			if ( absorbCount > 0 ) {
				sb.Append( "Absorb: " );
				for ( int i = 0; i < Attributes.Length; ++i ) {
					if ( Attributes[i] < 0 ) {
						Website.GenerateWebsite.AppendElementIcon( sb, (Element)i );
						sb.Append( -Attributes[i] );
						sb.Append( "% " );
					}
				}
				sb.Append( "<br>" );
			}

			if ( KillableWithFS == 0 ) {
				sb.AppendLine( "Immune to Fatal Strike.<br>" );
			}

			sb.Append( "</td>" );


			/*
			sb.Append( "<td rowspan=\"2\">" );
			sb.AppendLine();
			for ( int i = 0; i < 62; ++i ) {
				if ( !KnownValues.Contains( i ) ) {
					sb.Append( "~" + i + ": " + Data[i] );
					sb.Append( " [" + Category + "/" + enemyNameEntry.StringEngOrJpn + "]" );
					sb.AppendLine( "<br>" );
				}
			}
			for ( int i = 62; i < Data.Length; ++i ) {
				if ( !KnownValues.Contains( i ) ) {
					sb.Append( "~" + i + ": " + DataFloat[i] );
					sb.Append( " [" + Category + "/" + enemyNameEntry.StringEngOrJpn + "]" );
					sb.AppendLine( "<br>" );
				}
			}
			sb.Append( "</td>" );
			//*/

			sb.Append( "</tr>" );

			sb.Append( "<tr>" );
			sb.Append( "<td colspan=\"2\">" );

			if ( InMonsterBook == 0 ) {
				sb.AppendLine( "Not in Monster Book.<br>" );
			}

			if ( Location != 0 ) {
				var loc = locations.LocationIdDict[Location];
				sb.Append( "<a href=\"locations-" + version + ".html#location" + loc.LocationID + "\">" );
				if ( LocationWeather > -1 ) {
					sb.AppendLine( "<img src=\"menu-icons/weather-" + LocationWeather + ".png\" width=\"16\" height=\"16\">" );
				}
				sb.AppendLine( loc.GetLastValidName( inGameIdDict ).StringEngOrJpnHtml( version ) + "</a><br>" );
			}

			for ( int i = 0; i < DropItems.Length; ++i ) {
				if ( DropItems[i] != 0 ) {
					sb.Append( "Drops:<br>" );
					break;
				}
			}
			for ( int i = 0; i < DropItems.Length; ++i ) {
				if ( DropItems[i] != 0 ) {
					var item = items.itemIdDict[DropItems[i]];
					sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
					sb.Append( "<a href=\"items-i" + item.Data[(int)ItemData.Icon] + "-" + version + ".html#item" + item.Data[(int)ItemData.ID] + "\">" );
					sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpnHtml( version ) + "</a>" );
					sb.Append( ", " + DropChances[i] + "%" );
					if ( DropChances[i] < 100 ) {
						sb.Append( ", " );
						Website.GenerateWebsite.AppendFatalStrikeIcon( sb, FatalTypeDrop );
						sb.Append( " +" + DropModifier[i] + "%" );
					}
					sb.Append( "<br>" );
				}
			}
			if ( SecretMissionDrop != 0 && SecretMissionDropChance > 0 ) {
				sb.Append( "Secret Mission Reward:<br>" );
				var item = items.itemIdDict[SecretMissionDrop];
				sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
				sb.Append( "<a href=\"items-i" + item.Data[(int)ItemData.Icon] + "-" + version + ".html#item" + item.Data[(int)ItemData.ID] + "\">" );
				sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpnHtml( version ) + "</a>, " + SecretMissionDropChance + "%<br>" );
			}
			if ( StealItem != 0 ) {
				sb.Append( "Steal:<br>" );
				var item = items.itemIdDict[StealItem];
				sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
				sb.Append( "<a href=\"items-i" + item.Data[(int)ItemData.Icon] + "-" + version + ".html#item" + item.Data[(int)ItemData.ID] + "\">" );
				sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpnHtml( version ) + "</a>, " + StealChance + "%<br>" );
			}
			sb.Append( "</td>" );


			sb.AppendLine( "</tr>" );


			return sb.ToString();
		}

		public void AppendStats( StringBuilder sb, string difficultyName, int difficulty ) {
			double hpMod = 1.0;
			double tpMod = 1.0;
			double atkMod = 1.0;
			double defMod = 1.0;
			double mAtkMod = 1.0;
			double mDefMod = 1.0;
			double speedMod = 1.0;
			switch ( difficulty ) {
				case 0: hpMod = 0.7; tpMod = 0.7; atkMod = 0.7; defMod = 0.7; mAtkMod = 0.7; mDefMod = 0.7; speedMod = 0.7; break;
				case 1: hpMod = 1.0; tpMod = 1.0; atkMod = 1.0; defMod = 1.0; mAtkMod = 1.0; mDefMod = 1.0; speedMod = 1.0; break;
				case 2: hpMod = 2.5; tpMod = 1.0; atkMod = 1.2; defMod = 1.0; mAtkMod = 1.2; mDefMod = 1.0; speedMod = 1.0; break;
				case 3: hpMod = 5.0; tpMod = 1.0; atkMod = 3.5; defMod = 2.5; mAtkMod = 3.0; mDefMod = 2.5; speedMod = 1.0; break;
			}

			sb.Append( "<td rowspan=\"2\">" );
			sb.Append( "<span class=\"difficultyname\">" + difficultyName + "</span><br>" );
			sb.Append( "Level: " + Level + "<br>" );
			sb.Append( "HP: " + Math.Floor( HP * hpMod ) + "<br>" );
			sb.Append( "TP: " + Math.Floor( TP * tpMod ) + "<br>" );
			sb.Append( "PATK: " + Math.Floor( PATK * atkMod ) + "<br>" );
			sb.Append( "PDEF: " + Math.Floor( PDEF * defMod ) + "<br>" );
			sb.Append( "MATK: " + Math.Floor( MATK * mAtkMod ) + "<br>" );
			sb.Append( "MDEF: " + Math.Floor( MDEF * mDefMod ) + "<br>" );
			sb.Append( "AGL: " + Math.Floor( AGL * speedMod ) + "<br>" );
			sb.Append( "</td>" );
		}

	}
}
