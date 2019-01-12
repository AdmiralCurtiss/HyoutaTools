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

		// 14 is always zero
		// 16 is zero except on the new giganto monsters and spiral draco
		// 76 is on PS3 only, StringDicID for the dummy description

		public Enemy( System.IO.Stream stream, uint refStringStart, Util.Endianness endian, Util.Bitness bits ) {
			long pos = stream.Position;

			uint entryLength = stream.ReadUInt32().FromEndian( endian ); // 0

			ID = stream.ReadUInt32().FromEndian( endian ); // 1
			NameStringDicID = stream.ReadUInt32().FromEndian( endian ); // 2
			ulong ref3 = stream.ReadUInt( bits, endian ); // 3
			ulong ref4 = stream.ReadUInt( bits, endian ); // 4
			InGameID = stream.ReadUInt32().FromEndian( endian ); // 5
			ulong ref6 = stream.ReadUInt( bits, endian ); // 6
			Level = stream.ReadUInt32().FromEndian( endian ); // 7
			HP = stream.ReadUInt32().FromEndian( endian ); // 8
			TP = stream.ReadUInt32().FromEndian( endian ); // 9
			PATK = stream.ReadUInt32().FromEndian( endian ); // 10
			PDEF = stream.ReadUInt32().FromEndian( endian ); // 11
			MATK = stream.ReadUInt32().FromEndian( endian ); // 12
			MDEF = stream.ReadUInt32().FromEndian( endian ); // 13
			stream.ReadUInt32().FromEndian( endian ); // 14
			AGL = stream.ReadUInt32().FromEndian( endian ); // 15
			stream.ReadUInt32().FromEndian( endian ); // 16

			FatalBlue = stream.ReadUInt32().FromEndian( endian ).UIntToFloat(); // 17
			FatalRed = stream.ReadUInt32().FromEndian( endian ).UIntToFloat(); // 18
			FatalGreen = stream.ReadUInt32().FromEndian( endian ).UIntToFloat(); // 19
			FatalBlueRelated = stream.ReadUInt32().FromEndian( endian ).UIntToFloat(); // 20
			FatalRedRelated = stream.ReadUInt32().FromEndian( endian ).UIntToFloat(); // 21
			FatalGreenRelated = stream.ReadUInt32().FromEndian( endian ).UIntToFloat(); // 22

			stream.ReadUInt32().FromEndian( endian ); // 23

			Category = stream.ReadUInt32().FromEndian( endian ); // 24

			stream.ReadUInt32().FromEndian( endian ); // 25

			// > 100 weak, < 100 resist, 0 nullify, negative absorb
			// effectively a damage multiplier in percent
			Attributes = new int[7];
			Attributes[0] = (int)stream.ReadUInt32().FromEndian( endian ); // 26
			Attributes[1] = (int)stream.ReadUInt32().FromEndian( endian ); // 27
			Attributes[2] = (int)stream.ReadUInt32().FromEndian( endian ); // 28
			Attributes[3] = (int)stream.ReadUInt32().FromEndian( endian ); // 29
			Attributes[4] = (int)stream.ReadUInt32().FromEndian( endian ); // 30
			Attributes[5] = (int)stream.ReadUInt32().FromEndian( endian ); // 31
			Attributes[6] = (int)stream.ReadUInt32().FromEndian( endian ); // 32

			EXP = stream.ReadUInt32().FromEndian( endian ); // 33
			Gald = stream.ReadUInt32().FromEndian( endian ); // 34
			LP = stream.ReadUInt32().FromEndian( endian ); // 35

			DropItems = new uint[4];
			DropItems[0] = stream.ReadUInt32().FromEndian( endian ); // 36
			DropItems[1] = stream.ReadUInt32().FromEndian( endian ); // 37
			DropItems[2] = stream.ReadUInt32().FromEndian( endian ); // 38
			DropItems[3] = stream.ReadUInt32().FromEndian( endian ); // 39
			DropChances = new uint[4];
			DropChances[0] = stream.ReadUInt32().FromEndian( endian ); // 40
			DropChances[1] = stream.ReadUInt32().FromEndian( endian ); // 41
			DropChances[2] = stream.ReadUInt32().FromEndian( endian ); // 42
			DropChances[3] = stream.ReadUInt32().FromEndian( endian ); // 43
			StealItem = stream.ReadUInt32().FromEndian( endian ); // 44
			StealChance = stream.ReadUInt32().FromEndian( endian ); // 45

			FatalTypeExp = stream.ReadUInt32().FromEndian( endian ); // 46
			EXPModifier = stream.ReadUInt32().FromEndian( endian ); // 47
			FatalTypeLP = stream.ReadUInt32().FromEndian( endian ); // 48
			LPModifier = stream.ReadUInt32().FromEndian( endian ); // 49
			FatalTypeDrop = stream.ReadUInt32().FromEndian( endian ); // 50
			DropModifier = new uint[4];
			DropModifier[0] = stream.ReadUInt32().FromEndian( endian ); // 51
			DropModifier[1] = stream.ReadUInt32().FromEndian( endian ); // 52
			DropModifier[2] = stream.ReadUInt32().FromEndian( endian ); // 53
			DropModifier[3] = stream.ReadUInt32().FromEndian( endian ); // 54

			ulong ref55 = stream.ReadUInt( bits, endian ); // 55

			KillableWithFS = stream.ReadUInt32().FromEndian( endian ); // 56

			IconID = stream.ReadUInt32().FromEndian( endian ); // 57

			ulong ref58 = stream.ReadUInt( bits, endian ); // 58

			InMonsterBook = stream.ReadUInt32().FromEndian( endian ); // 59
			Location = stream.ReadUInt32().FromEndian( endian ); // 60
			// -1: None shown, 0: Sun, 1: Rain, 2: Snow, 3: Windy, 4: Night, 5: Cloudy
			LocationWeather = (int)stream.ReadUInt32().FromEndian( endian ); // 61

			stream.ReadUInt32().FromEndian( endian ); // 62
			stream.ReadUInt32().FromEndian( endian ); // 63
			stream.ReadUInt32().FromEndian( endian ); // 64
			stream.ReadUInt32().FromEndian( endian ); // 65
			stream.ReadUInt32().FromEndian( endian ); // 66
			stream.ReadUInt32().FromEndian( endian ); // 67

			SecretMissionDrop = stream.ReadUInt32().FromEndian( endian ); // 68
			SecretMissionDropChance = stream.ReadUInt32().FromEndian( endian ); // 69

			long bytesleft = ( pos + entryLength ) - stream.Position;
			if ( bytesleft > 0 ) {
				stream.DiscardBytes( (uint)bytesleft );
			}

			RefString = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + ref6 ) );
			RefString2 = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + ref3 ) );
			RefString3 = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + ref4 ) );
			RefString4 = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + ref55 ) );
			RefString5 = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + ref58 ) );
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, ItemDat.ItemDat items, WRLDDAT.WRLDDAT locations, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict, bool phpLinks = false ) {
			var sb = new StringBuilder();

			sb.Append( "<tr id=\"enemy" + InGameID + "\">" );

			sb.Append( "<td style=\"height: 46px; width: 46px;\">" );
			var enemyNameEntry = inGameIdDict[NameStringDicID];
			sb.Append( "<img src=\"monster-icons/44px/monster-" + IconID.ToString( "D3" ) + ".png\" title=\"" + RefString + "\">" );
			sb.Append( "</td>" );
			sb.Append( "<td style=\"height: 46px;\">" );
			if ( websiteLanguage.WantsJp() ) {
				sb.Append( "<span class=\"itemname\">" + enemyNameEntry.StringJpnHtml( version ) + "</span>" );
			}
			if ( websiteLanguage.WantsBoth() ) {
				sb.Append( "<br>" );
			}
			if ( websiteLanguage.WantsEn() ) {
				sb.Append( "<span class=\"itemname\">" + enemyNameEntry.StringEngHtml( version ) + "</span>" );
			}
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
			Website.WebsiteGenerator.AppendFatalStrikeIcon( sb, FatalTypeExp );
			sb.Append( " +" + EXPModifier + "<br>" );
			sb.Append( "LP: " + LP + ", " );
			Website.WebsiteGenerator.AppendFatalStrikeIcon( sb, FatalTypeLP );
			sb.Append( " +" + LPModifier + "<br>" );
			sb.Append( "Gald: " + Gald + "<br>" );

			sb.Append( "Fatal Strike Resistances:<br>" );
			Website.WebsiteGenerator.AppendFatalStrikeIcon( sb, 0 );
			sb.Append( FatalBlue );
			sb.Append( " " );
			Website.WebsiteGenerator.AppendFatalStrikeIcon( sb, 1 );
			sb.Append( FatalRed );
			sb.Append( " " );
			Website.WebsiteGenerator.AppendFatalStrikeIcon( sb, 2 );
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
						Website.WebsiteGenerator.AppendElementIcon( sb, (Element)i );
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
						Website.WebsiteGenerator.AppendElementIcon( sb, (Element)i );
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
						Website.WebsiteGenerator.AppendElementIcon( sb, (Element)i );
					}
				}
				sb.Append( "<br>" );
			}
			if ( absorbCount > 0 ) {
				sb.Append( "Absorb: " );
				for ( int i = 0; i < Attributes.Length; ++i ) {
					if ( Attributes[i] < 0 ) {
						Website.WebsiteGenerator.AppendElementIcon( sb, (Element)i );
						sb.Append( -Attributes[i] );
						sb.Append( "% " );
					}
				}
				sb.Append( "<br>" );
			}

			if ( KillableWithFS == 0 ) {
				sb.Append( "Immune to Fatal Strike.<br>" );
			}

			sb.Append( "</td>" );

			sb.Append( "</tr>" );

			sb.Append( "<tr>" );
			sb.Append( "<td colspan=\"2\">" );

			if ( InMonsterBook == 0 ) {
				sb.Append( "Not in Monster Book.<br>" );
			}

			if ( Location != 0 ) {
				var loc = locations.LocationIdDict[Location];
				sb.Append( "<a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Location, version, versionPostfix, locale, websiteLanguage, phpLinks, id: (int)loc.LocationID ) + "\">" );
				if ( LocationWeather > -1 ) {
					sb.Append( "<img src=\"menu-icons/weather-" + LocationWeather + ".png\" width=\"16\" height=\"16\">" );
				}
				sb.Append( loc.GetLastValidName( inGameIdDict ).StringEngOrJpnHtml( version, websiteLanguage ) + "</a><br>" );
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
					sb.Append( "<a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Item, version, versionPostfix, locale, websiteLanguage, phpLinks, id: (int)item.Data[(int)ItemData.ID], icon: (int)item.Data[(int)ItemData.Icon] ) + "\">" );
					sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpnHtml( version, websiteLanguage ) + "</a>" );
					sb.Append( ", " + DropChances[i] + "%" );
					if ( DropChances[i] < 100 ) {
						sb.Append( ", " );
						Website.WebsiteGenerator.AppendFatalStrikeIcon( sb, FatalTypeDrop );
						sb.Append( " +" + DropModifier[i] + "%" );
					}
					sb.Append( "<br>" );
				}
			}
			if ( SecretMissionDrop != 0 && SecretMissionDropChance > 0 ) {
				sb.Append( "Secret Mission Reward:<br>" );
				var item = items.itemIdDict[SecretMissionDrop];
				sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
				sb.Append( "<a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Item, version, versionPostfix, locale, websiteLanguage, phpLinks, id: (int)item.Data[(int)ItemData.ID], icon: (int)item.Data[(int)ItemData.Icon] ) + "\">" );
				sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpnHtml( version, websiteLanguage ) + "</a>, " + SecretMissionDropChance + "%<br>" );
			}
			if ( StealItem != 0 ) {
				sb.Append( "Steal:<br>" );
				var item = items.itemIdDict[StealItem];
				sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
				sb.Append( "<a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Item, version, versionPostfix, locale, websiteLanguage, phpLinks, id: (int)item.Data[(int)ItemData.ID], icon: (int)item.Data[(int)ItemData.Icon] ) + "\">" );
				sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpnHtml( version, websiteLanguage ) + "</a>, " + StealChance + "%<br>" );
			}
			sb.Append( "</td>" );


			sb.Append( "</tr>" );


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
