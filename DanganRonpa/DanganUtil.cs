using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.DanganRonpa {
	class DanganUtil {
		public static String CharacterIdToName( byte id ) {
			switch ( id ) {
				case 0x00: return "Naegi";
				case 0x01: return "Ishimaru";
				case 0x02: return "Togami";
				case 0x03: return "Mondo";
				case 0x04: return "Leon";
				case 0x05: return "Yamada";
				case 0x06: return "Hagakure";
				case 0x07: return "Maizono";
				case 0x08: return "Kirigiri";
				case 0x09: return "Aoi";
				case 0x0A: return "Fukawa";
				case 0x0B: return "Sakura";
				case 0x0C: return "Celes";
				case 0x0D: return "Mukuro";
				case 0x0E: return "Chihiro";
				case 0x0F: return "Monobear";
				case 0x10: return "Junko";
				case 0x11: return "AlterEgo";
				case 0x12: return "Syo";
				case 0x13: return "Headmaster";
				case 0x14: return "Mom";
				case 0x15: return "Dad";
				case 0x16: return "Sis";
				case 0x18: return "Mondo/Ishimaru";
				case 0x19: return "Daiya";
				case 0x1E: return "???";
				case 0x1F: return "None";
				default: return "Unknown-" + id.ToString( "X2" );
			}
		}

		public static byte NameToCharacterId( string name ) {
			switch ( name ) {
				case "Neagi": return 0x00;
				case "Naegi": return 0x00;
				case "Ishimaru": return 0x01;
				case "Togami": return 0x02;
				case "Mondo": return 0x03;
				case "Leon": return 0x04;
				case "Yamada": return 0x05;
				case "Hagakure": return 0x06;
				case "Maizono": return 0x07;
				case "Kirigiri": return 0x08;
				case "Aoi": return 0x09;
				case "Fukawa": return 0x0A;
				case "Sakura": return 0x0B;
				case "Celes": return 0x0C;
				case "Mukuro": return 0x0D;
				case "Chihiro": return 0x0E;
				case "Monobear": return 0x0F;
				case "Junko": return 0x10;
				case "AlterEgo": return 0x11;
				case "Syo": return 0x12;
				case "Headmaster": return 0x13;
				case "Mom": return 0x14;
				case "Dad": return 0x15;
				case "Sis": return 0x16;
				case "Mondo/Ishimaru": return 0x18;
				case "Daiya": return 0x19;
				case "???": return 0x1E;
				case "None": return 0x1F;
				default:
					if ( name.StartsWith( "Unknown-" ) ) {
						string idstr = name.Substring( "Unknown-".Length );
						return Byte.Parse( idstr, System.Globalization.NumberStyles.AllowHexSpecifier );
					} else {
						throw new Exception( "Character name not recognized: " + name );
					}
			}
		}

		public static uint GameVersion = 2; // 1 == DR1, 2 == SDR2
	}
}
