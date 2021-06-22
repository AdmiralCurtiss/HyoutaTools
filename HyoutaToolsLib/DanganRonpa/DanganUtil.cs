using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.DanganRonpa {
	class DanganUtil {
		public static String CharacterIdToName( byte id ) {
			switch ( GameVersion ) {
				case 1:
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
				case 2:
					switch ( id ) {
						case 0: return "SDR2-Hinata";
						case 1: return "SDR2-Nagito";
						case 2: return "SDR2-Togami-Fat";
						case 3: return "SDR2-Gundam";
						case 4: return "SDR2-Souda";
						case 5: return "SDR2-Hanamura";
						case 6: return "SDR2-Nidai";
						case 7: return "SDR2-Kuzuryuu";
						case 8: return "SDR2-Akane";
						case 9: return "SDR2-Nanami";
						case 10: return "SDR2-Sonia";
						case 11: return "SDR2-Saionji";
						case 12: return "SDR2-Mahiru";
						case 13: return "SDR2-Mikan";
						case 14: return "SDR2-Ibuki";
						case 15: return "SDR2-Pekoyama";
						case 16: return "SDR2-Monobear";
						case 17: return "SDR2-Monomi";
						case 18: return "SDR2-Junko";
						case 19: return "SDR2-Nidai-Robot";
						case 20: return "SDR2-Naegi";
						case 21: return "SDR2-Kirigiri";
						case 22: return "SDR2-Togami";
						case 23: return "SDR2-Hanamura-Mother";
						case 24: return "SDR2-AlterEgo";
						case 25: return "SDR2-Mini-Nidai";
						case 26: return "SDR2-Monobear/Monomi";
						case 27: return "SDR2-Narrator";
						case 39: return "SDR2-Usami";
						case 40: return "SDR2-Kirakira";
						case 41: return "SDR2-???";
						case 48: return "SDR2-Junko-2";
						case 50: return "SDR2-Twilight-Syndrome-Child-A";
						case 51: return "SDR2-Twilight-Syndrome-Child-B";
						case 52: return "SDR2-Twilight-Syndrome-Child-C";
						case 53: return "SDR2-Twilight-Syndrome-Child-D";
						case 54: return "SDR2-Twilight-Syndrome-Child-E";
						case 55: return "SDR2-Twilight-Syndrome-Man-F";
						case 56: return "SDR2-Twilight-Syndrome-???";
						case 63: return "SDR2-Empty";
						default: return "Unknown-" + id.ToString( "X2" );
					}
				default:
					return "Unknown-" + id.ToString( "X2" );
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
				case "SDR2-Hinata": return 0;
				case "SDR2-Nagito": return 1;
				case "SDR2-Togami-Fat": return 2;
				case "SDR2-Gundam": return 3;
				case "SDR2-Souda": return 4;
				case "SDR2-Hanamura": return 5;
				case "SDR2-Nidai": return 6;
				case "SDR2-Kuzuryuu": return 7;
				case "SDR2-Akane": return 8;
				case "SDR2-Nanami": return 9;
				case "SDR2-Sonia": return 10;
				case "SDR2-Saionji": return 11;
				case "SDR2-Mahiru": return 12;
				case "SDR2-Mikan": return 13;
				case "SDR2-Ibuki": return 14;
				case "SDR2-Pekoyama": return 15;
				case "SDR2-Monobear": return 16;
				case "SDR2-Monomi": return 17;
				case "SDR2-Junko": return 18;
				case "SDR2-Nidai-Robot": return 19;
				case "SDR2-Naegi": return 20;
				case "SDR2-Kirigiri": return 21;
				case "SDR2-Togami": return 22;
				case "SDR2-Hanamura-Mother": return 23;
				case "SDR2-AlterEgo": return 24;
				case "SDR2-Mini-Nidai": return 25;
				case "SDR2-Monobear/Monomi": return 26;
				case "SDR2-Narrator": return 27;
				case "SDR2-Usami": return 39;
				case "SDR2-Kirakira": return 40;
				case "SDR2-???": return 41;
				case "SDR2-Junko-2": return 48;
				case "SDR2-Twilight-Syndrome-Child-A": return 50;
				case "SDR2-Twilight-Syndrome-Child-B": return 51;
				case "SDR2-Twilight-Syndrome-Child-C": return 52;
				case "SDR2-Twilight-Syndrome-Child-D": return 53;
				case "SDR2-Twilight-Syndrome-Child-E": return 54;
				case "SDR2-Twilight-Syndrome-Man-F": return 55;
				case "SDR2-Twilight-Syndrome-???": return 56;
				case "SDR2-Empty": return 63;
				default:
					if ( name.StartsWith( "Unknown-" ) ) {
						string idstr = name.Substring( "Unknown-".Length );
						return Byte.Parse( idstr, System.Globalization.NumberStyles.AllowHexSpecifier );
					} else {
						throw new Exception( "Character name not recognized: " + name );
					}
			}
		}

		public static uint GameVersion = 1; // 1 == DR1, 2 == SDR2
	}
}
