using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTMA {
	public class Arte {
		public enum ArteType {
			Generic = 0,
			NoviceSpell = 1,
			IntermediateSpell = 2,
			AdvancedSpell = 3,
			Base = 4,
			Arcane = 5,
			BurstSpell = 6,
			Burst = 7,
			AlteredSpell = 8,
			Altered = 9,
			AlteredBurstSpell = 10,
			AlteredBurst = 11,
			FatalStrike = 12,
			Mystic = 13,
			OverLimit = 14,
			SkillAutomatic = 15
		}

		public uint[] Data;
		public uint NameStringDicId;
		public uint DescStringDicId;
		public ArteType Type;

		public uint ID;
		public uint InGameID;

		public string RefString;

		public uint[] LearnRequirementsOtherArtesType;
		public uint[] LearnRequirementsOtherArtesId;
		public uint[] LearnRequirementsOtherArtesUsageCount;
		public uint CharacterRelatedField;
		public uint Character;

		public uint[] AlteredArteRequirementType;
		public uint[] AlteredArteRequirementId;

		public uint ElementFire;
		public uint ElementEarth;
		public uint ElementWind;
		public uint ElementWater;
		public uint ElementLight;
		public uint ElementDarkness;
		public uint FatalStrikeType; // green = 2, blue = 0, red = 1, none = 3

		public uint TPUsage;
		public uint UsableInMenu;

		public Arte( byte[] Bytes, uint Location, uint Size, uint refStringStart ) {
			Data = new uint[Size / 4];
			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = BitConverter.ToUInt32( Bytes, (int)( Location + i * 4 ) ).SwapEndian();
			}

			ID = Data[1];
			InGameID = Data[2];

			uint refStringLocaton = Data[3]; // seems to be identical with Data[4]
			RefString = Util.GetTextAscii( Bytes, (int)( refStringStart + refStringLocaton ) );

			NameStringDicId = Data[5];
			DescStringDicId = Data[6];
			Type = (ArteType)Data[7];

			TPUsage = Data[8];
			ElementFire = Data[10];
			ElementEarth = Data[11];
			ElementWind = Data[12];
			ElementWater = Data[13];
			ElementLight = Data[14];
			ElementDarkness = Data[15];


			// for some reason the data order different between versions?
			if ( Size == 876 ) {
				LearnRequirementsOtherArtesType = new uint[6];
				for ( int i = 0; i < LearnRequirementsOtherArtesType.Length; ++i ) {
					LearnRequirementsOtherArtesType[i] = Data[128 + 22 + i];
				}
				LearnRequirementsOtherArtesId = new uint[6];
				for ( int i = 0; i < LearnRequirementsOtherArtesId.Length; ++i ) {
					LearnRequirementsOtherArtesId[i] = Data[128 + 28 + i];
				}
				LearnRequirementsOtherArtesUsageCount = new uint[6];
				for ( int i = 0; i < LearnRequirementsOtherArtesUsageCount.Length; ++i ) {
					LearnRequirementsOtherArtesUsageCount[i] = Data[128 + 34 + i];
				}

				AlteredArteRequirementType = new uint[5];
				for ( int i = 0; i < AlteredArteRequirementType.Length; ++i ) {
					AlteredArteRequirementType[i] = Data[127 + 69 + i];
				}
				AlteredArteRequirementId = new uint[5];
				for ( int i = 0; i < AlteredArteRequirementId.Length; ++i ) {
					AlteredArteRequirementId[i] = Data[127 + 74 + i];
				}
				//CharacterRelatedField = Data[?];
				Character = Data[21];

				FatalStrikeType = Data[210];

				UsableInMenu = Data[128 + 44];
			} else {
				LearnRequirementsOtherArtesType = new uint[6];
				for ( int i = 0; i < LearnRequirementsOtherArtesType.Length; ++i ) {
					LearnRequirementsOtherArtesType[i] = Data[22 + i];
				}
				LearnRequirementsOtherArtesId = new uint[6];
				for ( int i = 0; i < LearnRequirementsOtherArtesId.Length; ++i ) {
					LearnRequirementsOtherArtesId[i] = Data[28 + i];
				}
				LearnRequirementsOtherArtesUsageCount = new uint[6];
				for ( int i = 0; i < LearnRequirementsOtherArtesUsageCount.Length; ++i ) {
					LearnRequirementsOtherArtesUsageCount[i] = Data[34 + i];
				}

				AlteredArteRequirementType = new uint[5];
				for ( int i = 0; i < AlteredArteRequirementType.Length; ++i ) {
					AlteredArteRequirementType[i] = Data[69 + i];
				}
				AlteredArteRequirementId = new uint[5];
				for ( int i = 0; i < AlteredArteRequirementId.Length; ++i ) {
					AlteredArteRequirementId[i] = Data[74 + i];
				}
				CharacterRelatedField = Data[95];
				Character = Data[96];

				FatalStrikeType = Data[83];

				UsableInMenu = Data[44];
			}

			// always identical: 16, 17
			// 59 is some sort of status effect field
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( GameVersion version, Dictionary<uint, Arte> arteIdDict, T8BTEMST.T8BTEMST enemies, T8BTSK.T8BTSK skills, TSS.TSSFile stringDic, Dictionary<uint, TSS.TSSEntry> inGameIdDict, bool phpLinks = false ) {
			StringBuilder sb = new StringBuilder();
			sb.Append( "<tr id=\"arte" + InGameID + "\">" );
			sb.Append( "<td style=\"text-align: right;\">" );
			//sb.Append( RefString + "<br>" );
			if ( Character > 0 && Character <= 9 ) {
				Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, version, 1u << (int)( Character - 1 ) );
			}
			if ( Character > 9 ) {
				var enemy = enemies.EnemyIdDict[Character];
				sb.Append( "<a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Enemy, version, phpLinks, category: (int)enemy.Category, id: (int)enemy.InGameID ) + "\">" );
				sb.Append( "<img src=\"monster-icons/48px/monster-" + enemy.IconID.ToString( "D3" ) + ".png\" width=\"32\" height=\"32\" title=\"" + inGameIdDict[enemy.NameStringDicID].StringEngOrJpnHtml( version ) + "\">" );
				sb.Append( "</a>" );
			}
			sb.Append( "<img src=\"menu-icons/artes-" );
			switch ( Type ) {
				case ArteType.Base: sb.Append( "00" ); break;
				case ArteType.Arcane: sb.Append( "01" ); break;
				case ArteType.Altered: sb.Append( "12" ); break;
				case ArteType.AlteredSpell: sb.Append( "12" ); break;
				case ArteType.NoviceSpell: sb.Append( "04" ); break;
				case ArteType.IntermediateSpell: sb.Append( "05" ); break;
				case ArteType.AdvancedSpell: sb.Append( "06" ); break;
				case ArteType.Burst: sb.Append( "02" ); break;
				case ArteType.BurstSpell: sb.Append( "02" ); break;
				case ArteType.AlteredBurst: sb.Append( "02" ); break;
				case ArteType.AlteredBurstSpell: sb.Append( "02" ); break;
				case ArteType.Mystic: sb.Append( "02" ); break;
			}
			sb.Append( ".png\" width=\"32\" height=\"32\">" );
			sb.Append( "</td>" );
			sb.Append( "<td>" );
			sb.Append( "<span class=\"itemname\">" );
			sb.Append( inGameIdDict[NameStringDicId].StringJpnHtml( version ) + "</span><br>" );
			sb.Append( inGameIdDict[DescStringDicId].StringJpnHtml( version ) );
			sb.Append( "</td>" );
			sb.Append( "<td>" );
			sb.Append( "<span class=\"itemname\">" );
			sb.Append( inGameIdDict[NameStringDicId].StringEngHtml( version ) + "</span><br>" );
			sb.Append( inGameIdDict[DescStringDicId].StringEngHtml( version ) );
			sb.Append( "</td>" );
			sb.Append( "<td>" );
			//sb.Append( Type + "<br>" );

			bool iconsInserted = false;
			if ( Character <= 9 ) {
				switch ( FatalStrikeType ) {
					case 0: sb.Append( "<img src=\"menu-icons/artes-13.png\" width=\"16\" height=\"16\">" ); iconsInserted = true; break;
					case 1: sb.Append( "<img src=\"menu-icons/artes-15.png\" width=\"16\" height=\"16\">" ); iconsInserted = true; break;
					case 2: sb.Append( "<img src=\"menu-icons/artes-14.png\" width=\"16\" height=\"16\">" ); iconsInserted = true; break;
				}
			}
			if ( ElementFire > 0 ) { sb.Append( "<img src=\"text-icons/icon-element-02.png\" width=\"16\" height=\"16\">" ); iconsInserted = true; }
			if ( ElementEarth > 0 ) { sb.Append( "<img src=\"text-icons/icon-element-04.png\" width=\"16\" height=\"16\">" ); iconsInserted = true; }
			if ( ElementWind > 0 ) { sb.Append( "<img src=\"text-icons/icon-element-01.png\" width=\"16\" height=\"16\">" ); iconsInserted = true; }
			if ( ElementWater > 0 ) { sb.Append( "<img src=\"text-icons/icon-element-05.png\" width=\"16\" height=\"16\">" ); iconsInserted = true; }
			if ( ElementLight > 0 ) { sb.Append( "<img src=\"text-icons/icon-element-03.png\" width=\"16\" height=\"16\">" ); iconsInserted = true; }
			if ( ElementDarkness > 0 ) { sb.Append( "<img src=\"text-icons/icon-element-06.png\" width=\"16\" height=\"16\">" ); iconsInserted = true; }
			if ( iconsInserted ) { sb.Append( "<br>" ); }
			if ( TPUsage > 0 ) { sb.Append( "TP usage: " + TPUsage + "<br>" ); }

			for ( int i = 0; i < LearnRequirementsOtherArtesType.Length; ++i ) {
				if ( LearnRequirementsOtherArtesType[i] > 0 ) {
					switch ( LearnRequirementsOtherArtesType[i] ) {
						case 1: // Level
							if ( LearnRequirementsOtherArtesId[i] == 300 ) {
								sb.Append( "Learn via Event" );
							} else {
								sb.Append( "Level " + LearnRequirementsOtherArtesId[i] );
							}
							break;
						case 2: // Other Arte
							var otherArte = arteIdDict[LearnRequirementsOtherArtesId[i]];
							if ( otherArte.ID != this.ID ) {
								sb.Append( "<img src=\"menu-icons/artes-" + otherArte.GetIconNumber() + ".png\" width=\"16\" height=\"16\">" );
								sb.Append( "<a href=\"#arte" + otherArte.InGameID + "\">" + inGameIdDict[otherArte.NameStringDicId].StringEngOrJpnHtml( version ) );
								sb.Append( "</a>, " );
							} else {
								sb.Append( "Learn with " );
							}
							sb.Append( LearnRequirementsOtherArtesUsageCount[i] + " uses" );

							break;
						case 3: // appears on Rita and Repede's Burst Artes in 360 only, appears to be unused
							break;
						default:
							sb.Append( "##Unknown Learn Type: " + LearnRequirementsOtherArtesType[i] + "<br>" );
							sb.Append( "##Value 1: " + LearnRequirementsOtherArtesId[i] + "<br>" );
							sb.Append( "##Value 2: " + LearnRequirementsOtherArtesUsageCount[i] + "<br>" );
							break;
					}
					sb.Append( "<br>" );
				}
			}
			for ( int i = 0; i < AlteredArteRequirementType.Length; ++i ) {
				if ( AlteredArteRequirementType[i] > 0 ) {
					switch ( AlteredArteRequirementType[i] ) {
						case 1: // original arte
							var otherArte = arteIdDict[AlteredArteRequirementId[i]];
							sb.Append( "Alters from " );
							sb.Append( "<img src=\"menu-icons/artes-" + otherArte.GetIconNumber() + ".png\" width=\"16\" height=\"16\">" );
							sb.Append( "<a href=\"#arte" + otherArte.InGameID + "\">" + inGameIdDict[otherArte.NameStringDicId].StringEngOrJpnHtml( version ) + "</a>" );
							break;
						case 3: // skill
							var skill = skills.SkillIdDict[AlteredArteRequirementId[i]];
							sb.Append( "<img src=\"skill-icons/category-" + skill.Category + ".png\" width=\"16\" height=\"16\">" );
							sb.Append( "<a href=\"" + Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Skill, version, phpLinks, id: (int)skill.InGameID ) + "\">" + inGameIdDict[skill.NameStringDicID].StringEngOrJpnHtml( version ) + "</a>" );
							break;
						default:
							sb.Append( "##Unknown Altered Type: " + AlteredArteRequirementType[i] + "<br>" );
							sb.Append( "##Value: " + AlteredArteRequirementId[i] + "<br>" );
							break;
					}
					sb.Append( "<br>" );
				}
			}

			if ( UsableInMenu > 0 ) { sb.Append( "Usable outside of battle" ); }

			sb.Append( "</td>" );

			/*
			sb.Append( "<td>" );
			sb.AppendLine();
			sb.AppendLine( "~" + 9 + ": " + Data[9] + "<br>" );
			for ( int i = 16; i < 22; ++i ) {
				sb.AppendLine( "~" + i + ": " + Data[i] + "<br>" );
			}
			for ( int i = 40; i < 69; ++i ) {
				if ( i == 44 ) continue;
				if ( i == 42 || i == 66 ) {
					sb.AppendLine( "~" + i + ": " + Data[i].UIntToFloat() + "<br>" );
				} else {
					sb.AppendLine( "~" + i + ": " + Data[i] + "<br>" );
				}
			}
			for ( int i = 79; i < 95; ++i ) {
				if ( i == 83 ) continue;
				sb.AppendLine( "~" + i + ": " + Data[i] + "<br>" );
			}
			sb.Append( "</td>" );
			//*/

			sb.Append( "</tr>" );
			return sb.ToString();
		}

		public string GetIconNumber() {
			switch ( Type ) {
				case ArteType.Base: return "00";
				case ArteType.Arcane: return "01";
				case ArteType.Altered: return "12";
				case ArteType.AlteredSpell: return "12";
				case ArteType.NoviceSpell: return "04";
				case ArteType.IntermediateSpell: return "05";
				case ArteType.AdvancedSpell: return "06";
				case ArteType.Burst: return "02";
				case ArteType.BurstSpell: return "02";
				case ArteType.AlteredBurst: return "02";
				case ArteType.AlteredBurstSpell: return "02";
				case ArteType.Mystic: return "02";
			}
			return "xx";
		}
	}
}
