using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	public class SaveDataParser {
		private static void PrintSavePoint( byte[] flags, int index, string where ) {
			Console.WriteLine( "Save Point 0x" + index.ToString( "X2" ) + " " + ( flags[index] > 0 ? "[ ok ]" : "[MISS]" ) + ": " + where );
		}

		public static int Parse( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: SaveDataParser SAVE" );
				Console.WriteLine( "Save must be decrypted." );
				return -1;
			}

			Util.Endianness endian = Util.Endianness.BigEndian;
			var stringDic = new TSS.TSSFile( @"c:\Dropbox\ToV\PS3\mod\string.svo.ext\STRING_DIC.SO", Util.GameTextEncoding.ShiftJIS, Util.Endianness.BigEndian );
			var inGameDic = stringDic.GenerateInGameIdDictionary();
			var itemData = new ItemDat.ItemDat( @"c:\Dropbox\ToV\PS3\orig\item.svo.ext\ITEM.DAT", endian );
			var itemDataSorted = itemData.GetSortedByInGameSorting();
			var titles = new FAMEDAT.FAMEDAT( @"c:\Dropbox\ToV\PS3\orig\menu.svo.ext\FAMEDATA.BIN", endian );
			var enemies = new T8BTEMST.T8BTEMST( @"c:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0005.ext\ALL.0000", endian );

			using ( Stream file = new FileStream( args[0], FileMode.Open, FileAccess.Read ) ) {
				file.DiscardBytes( 0x228 ); // short header, used for save menu on 360 version to display basic info about save
				string magic = file.ReadAscii( 8 );
				if ( magic != "TO8SAVE\0" ) {
					throw new Exception( "Invalid magic byte sequence for ToV save: " + magic );
				}
				uint saveFileSize = file.ReadUInt32().FromEndian( endian );
				if ( saveFileSize != 0xCCAA0 ) {
					throw new Exception( "Unexpected filesize for ToV save: " + saveFileSize );
				}
				file.DiscardBytes( 0x3AC8 - 0x234 ); // no idea what all this is

				// save point flags, one byte each, 0x00 not visted 0x01 visited
				byte[] savePointFlags = file.ReadUInt8Array( 0x59 );
				{
					PrintSavePoint( savePointFlags, 0x00, "Fiertia Deck (Docked at Atherum)" );
					PrintSavePoint( savePointFlags, 0x01, "Atherum" );
					PrintSavePoint( savePointFlags, 0x02, "Fiertia Hold" ); // same flag for both opportunities?
					PrintSavePoint( savePointFlags, 0x03, "Keiv Moc (Middle)" );
					PrintSavePoint( savePointFlags, 0x04, "Keiv Moc (Boss)" );
					PrintSavePoint( savePointFlags, 0x05, "Zaphias (Lower Quarter)" );
					PrintSavePoint( savePointFlags, 0x06, "Zaphias (Royal Quarter)" );
					PrintSavePoint( savePointFlags, 0x07, "Zaphias Castle (Prison)" );
					PrintSavePoint( savePointFlags, 0x08, "Zaphias Castle (Kitchen)" ); // 2nd visit only
					PrintSavePoint( savePointFlags, 0x09, "Zaphias Castle (Hallways)" ); // before zagi fight
					PrintSavePoint( savePointFlags, 0x0A, "Zaphias Castle (Sword Stair)" );
					PrintSavePoint( savePointFlags, 0x0B, "Zaphias Castle (Big Hall)" ); // 2nd visit only, that big room that leads to the sword stair
					PrintSavePoint( savePointFlags, 0x0C, "Weasand of Cados (Middle)" );
					PrintSavePoint( savePointFlags, 0x0D, "Weasand of Cados (Exit)" );
					PrintSavePoint( savePointFlags, 0x0E, "Halure (Inn)" );
					PrintSavePoint( savePointFlags, 0x0F, "Ghasfarost (Bottom)" );
					PrintSavePoint( savePointFlags, 0x10, "Ghasfarost (Top)" );
					PrintSavePoint( savePointFlags, 0x11, "Myorzo (Vacant House)" );
					PrintSavePoint( savePointFlags, 0x12, "Mt. Temza (Middle)" );
					PrintSavePoint( savePointFlags, 0x13, "Mt. Temza (Boss)" );
					PrintSavePoint( savePointFlags, 0x14, "Deidon Hold" );
					PrintSavePoint( savePointFlags, 0x15, "Northeastern Hypionia" ); // aurnion before it's built
					PrintSavePoint( savePointFlags, 0x16, "Aurnion (Developing)" );
					PrintSavePoint( savePointFlags, 0x17, "Aurnion (Developed)" );
					PrintSavePoint( savePointFlags, 0x18, "Caer Bocram" );
					PrintSavePoint( savePointFlags, 0x19, "Quoi Woods" );
					PrintSavePoint( savePointFlags, 0x1A, "Dahngrest (Inn)" );
					PrintSavePoint( savePointFlags, 0x1B, "Ehmead Hill" );
					PrintSavePoint( savePointFlags, 0x1C, "Erealumen (Middle)" );
					PrintSavePoint( savePointFlags, 0x1D, "Erealumen (Boss)" );
					PrintSavePoint( savePointFlags, 0x1E, "Heracles (Near Engine Room)" );
					PrintSavePoint( savePointFlags, 0x1F, "Heracles (Near Control Room)" ); // zagi fight
					PrintSavePoint( savePointFlags, 0x20, "Zopheir (Boss)" ); // 1st visit only
					PrintSavePoint( savePointFlags, 0x21, "Zopheir (Near Aer Krene)" ); // 2nd visit only
					PrintSavePoint( savePointFlags, 0x22, "Manor of the Wicked" );
					PrintSavePoint( savePointFlags, 0x23, "Tarqaron (Middle)" );
					PrintSavePoint( savePointFlags, 0x24, "Tarqaron (Top)" );
					PrintSavePoint( savePointFlags, 0x25, "Baction B1F" );
					PrintSavePoint( savePointFlags, 0x26, "Baction B2F" ); // both save points on B2F share this flag...?
					PrintSavePoint( savePointFlags, 0x27, "Mantaic (Inn)" );
					PrintSavePoint( savePointFlags, 0x28, "Relewiese (Middle)" );
					PrintSavePoint( savePointFlags, 0x29, "Relewiese (Boss)" );
					PrintSavePoint( savePointFlags, 0x2A, "Capua Nor (Outside Ragou's Mansion)" );
					PrintSavePoint( savePointFlags, 0x2B, "Capua Nor (Inn)" );
					PrintSavePoint( savePointFlags, 0x2C, "Capua Torim (Inn)" );
					PrintSavePoint( savePointFlags, 0x2D, "Shaikos Ruins" );
					PrintSavePoint( savePointFlags, 0x2E, "Zaude (Side Entrance)" );
					PrintSavePoint( savePointFlags, 0x2F, "Zaude (Alexei)" );
					PrintSavePoint( savePointFlags, 0x30, "Zaude (Yeager)" );
					PrintSavePoint( savePointFlags, 0x31, "Aspio (Inn)" );
					PrintSavePoint( savePointFlags, 0x32, "Nordopolica (Inn)" );
					PrintSavePoint( savePointFlags, 0x33, "Heliord (Inn)" );
					PrintSavePoint( savePointFlags, 0x34, "Yormgen (Inn)" );
					PrintSavePoint( savePointFlags, 0x35, "Weasand of Kogorh (Oasis)" );
					PrintSavePoint( savePointFlags, 0x36, "Weasand of Kogorh (Exit)" );
					PrintSavePoint( savePointFlags, 0x37, "Egothor Forest" );
					PrintSavePoint( savePointFlags, 0x38, "Dahngrest Underpass (Oath)" );
					PrintSavePoint( savePointFlags, 0x39, "Ragou's Mansion" ); // basement dungeon midpoint
					PrintSavePoint( savePointFlags, 0x3A, "Dahngrest Underpass (Exit)" );
					PrintSavePoint( savePointFlags, 0x3B, "Abysmal Hollow (Aer Krene near Yumanju)" );
					PrintSavePoint( savePointFlags, 0x3C, "? Abysmal Hollow (Aer Krene near Zaphias)" );
					PrintSavePoint( savePointFlags, 0x3D, "Abysmal Hollow (Aer Krene near Heliord)" );
					PrintSavePoint( savePointFlags, 0x3E, "Abysmal Hollow (Aer Krene near Nordopolica)" );
					PrintSavePoint( savePointFlags, 0x3F, "? Abysmal Hollow (Center)" );
					PrintSavePoint( savePointFlags, 0x40, "City of the Waning Moon" );
					PrintSavePoint( savePointFlags, 0x41, "Necropolis of Nostalgia A3" );
					PrintSavePoint( savePointFlags, 0x42, "Necropolis of Nostalgia A6" );
					PrintSavePoint( savePointFlags, 0x43, "Necropolis of Nostalgia A9" );
					PrintSavePoint( savePointFlags, 0x44, "Necropolis of Nostalgia A Bottom" );
					PrintSavePoint( savePointFlags, 0x45, "Necropolis of Nostalgia B2" );
					PrintSavePoint( savePointFlags, 0x46, "Necropolis of Nostalgia B5" );
					PrintSavePoint( savePointFlags, 0x47, "Necropolis of Nostalgia B8" );
					PrintSavePoint( savePointFlags, 0x48, "Necropolis of Nostalgia B Bottom" );
					PrintSavePoint( savePointFlags, 0x49, "Necropolis of Nostalgia C3" );
					PrintSavePoint( savePointFlags, 0x4A, "Necropolis of Nostalgia C6" );
					PrintSavePoint( savePointFlags, 0x4B, "Necropolis of Nostalgia C9" );
					PrintSavePoint( savePointFlags, 0x4C, "Necropolis of Nostalgia C Bottom" );
					PrintSavePoint( savePointFlags, 0x4D, "Necropolis of Nostalgia D3" );
					PrintSavePoint( savePointFlags, 0x4E, "Necropolis of Nostalgia D6" );
					PrintSavePoint( savePointFlags, 0x4F, "Necropolis of Nostalgia D9" );
					PrintSavePoint( savePointFlags, 0x50, "Necropolis of Nostalgia D Bottom" );
					PrintSavePoint( savePointFlags, 0x51, "Necropolis of Nostalgia E3" );
					PrintSavePoint( savePointFlags, 0x52, "Necropolis of Nostalgia E6" );
					PrintSavePoint( savePointFlags, 0x53, "Necropolis of Nostalgia E9" );
					PrintSavePoint( savePointFlags, 0x54, "Necropolis of Nostalgia E Bottom" );
					PrintSavePoint( savePointFlags, 0x55, "Necropolis of Nostalgia F3" );
					PrintSavePoint( savePointFlags, 0x56, "Necropolis of Nostalgia F6" );
					PrintSavePoint( savePointFlags, 0x57, "Necropolis of Nostalgia F9" );
					PrintSavePoint( savePointFlags, 0x58, "Necropolis of Nostalgia F Bottom" );
				}

				file.DiscardBytes( 0xA3F48 - 0x3B21 ); // no idea what all this is
				file.ReadUInt32().FromEndian( endian ); // ?
				file.ReadUInt32().FromEndian( endian ); // ?
				file.ReadUInt32Array( 9, endian );
				file.ReadUInt32().FromEndian( endian ); // play time in frames, assuming 60 frames = 1 second
				file.ReadUInt32().FromEndian( endian ); // gald
				file.DiscardBytes( 4 ); // ?
				uint[] itemCounts = file.ReadUInt32Array( 3072, endian );
				uint[] itemBookBitfields = file.ReadUInt32Array( 3072 / 32, endian );
				file.DiscardBytes( 4 ); // ?
				file.ReadUInt32Array( 4, endian ); // control modes for the four active party slots
				file.ReadUInt32Array( 3, endian ); // strategies assigned to dpad directions
				file.DiscardBytes( 0x40 ); // ??
				for ( int i = 0; i < 8; ++i ) {
					// custom strategy names
					// game seems to read these till null byte so this could totally be abused to buffer overflow...
					file.ReadAscii( 0x40 );
				}

				file.DiscardBytes( 0xA84D0 - 0xA7360 ); // ?
				uint[] monsterBookBitfieldsScanned = file.ReadUInt32Array( 0x48 / 4, endian );
				file.DiscardBytes( 0xA8680 - 0xA8518 ); // ?
				uint[] monsterBookBitfieldsSeen = file.ReadUInt32Array( 0x48 / 4, endian );
				file.DiscardBytes( 0xA8928 - 0xA86C8 ); // ?

				// 9 character blocks, each 0x4010 bytes
				for ( int character = 0; character < 9; ++character ) {
					file.ReadUInt32().FromEndian( endian ); // ?
					file.ReadAscii( 0x40 ); // custom character name
					file.ReadUInt32().FromEndian( endian ); // character ID
					file.ReadUInt32().FromEndian( endian ); // level
					file.ReadUInt32().FromEndian( endian ); // current HP
					file.ReadUInt32().FromEndian( endian ); // current TP
					file.ReadUInt32().FromEndian( endian ); // max HP
					file.ReadUInt32().FromEndian( endian ); // max TP
					file.ReadUInt32().FromEndian( endian ); // ?
					file.ReadUInt32().FromEndian( endian ); // EXP
					file.ReadUInt32().FromEndian( endian ); // ?
					file.ReadUInt32().FromEndian( endian ); // ?
					file.ReadUInt32().FromEndian( endian ); // base attack
					file.ReadUInt32().FromEndian( endian ); // base magic attack
					file.ReadUInt32().FromEndian( endian ); // base def
					file.ReadUInt32().FromEndian( endian ); // base mdef
					file.ReadUInt32().FromEndian( endian ); // ?
					file.ReadUInt32().FromEndian( endian ); // base agility
					file.ReadUInt32().FromEndian( endian ); // luck
					file.ReadUInt32().FromEndian( endian ); // ?
					file.ReadUInt32().FromEndian( endian ); // ?
					file.ReadUInt32().FromEndian( endian ); // base attack attribute fire
					file.ReadUInt32().FromEndian( endian ); // base attack attribute earth
					file.ReadUInt32().FromEndian( endian ); // base attack attribute wind
					file.ReadUInt32().FromEndian( endian ); // base attack attribute water
					file.ReadUInt32().FromEndian( endian ); // base attack attribute light
					file.ReadUInt32().FromEndian( endian ); // base attack attribute dark
					file.ReadUInt32().FromEndian( endian ); // base attack attribute physical...?
					file.ReadUInt32().FromEndian( endian ); // base damage multiplier fire
					file.ReadUInt32().FromEndian( endian ); // base damage multiplier earth
					file.ReadUInt32().FromEndian( endian ); // base damage multiplier wind
					file.ReadUInt32().FromEndian( endian ); // base damage multiplier water
					file.ReadUInt32().FromEndian( endian ); // base damage multiplier light
					file.ReadUInt32().FromEndian( endian ); // base damage multiplier dark
					file.ReadUInt32().FromEndian( endian ); // base damage multiplier physical?
					file.DiscardBytes( 0xA8A60 - 0xA89F0 ); // ?
					file.ReadUInt32().FromEndian( endian ); // modified attack (base + from equipment)
					file.ReadUInt32().FromEndian( endian ); // mod def
					file.ReadUInt32().FromEndian( endian ); // mod matk
					file.ReadUInt32().FromEndian( endian ); // mod mdef
					file.ReadUInt32().FromEndian( endian ); // ?
					file.ReadUInt32().FromEndian( endian ); // mod agility
					file.ReadUInt32().FromEndian( endian ); // mod luck
					file.ReadUInt32().FromEndian( endian ); // ?
					file.ReadUInt32().FromEndian( endian ); // ?
					file.ReadUInt32().FromEndian( endian ); // mod attack attribute fire
					file.ReadUInt32().FromEndian( endian ); // mod attack attribute earth
					file.ReadUInt32().FromEndian( endian ); // mod attack attribute wind
					file.ReadUInt32().FromEndian( endian ); // mod attack attribute water
					file.ReadUInt32().FromEndian( endian ); // mod attack attribute light
					file.ReadUInt32().FromEndian( endian ); // mod attack attribute dark
					file.ReadUInt32().FromEndian( endian ); // mod attack attribute physical...?
					file.ReadUInt32().FromEndian( endian ); // mod damage multiplier fire
					file.ReadUInt32().FromEndian( endian ); // mod damage multiplier earth
					file.ReadUInt32().FromEndian( endian ); // mod damage multiplier wind
					file.ReadUInt32().FromEndian( endian ); // mod damage multiplier water
					file.ReadUInt32().FromEndian( endian ); // mod damage multiplier light
					file.ReadUInt32().FromEndian( endian ); // mod damage multiplier dark
					file.ReadUInt32().FromEndian( endian ); // mod damage multiplier physical?
					file.DiscardBytes( 0xA8E04 - 0xA8ABC ); // ?
					file.ReadUInt32().FromEndian( endian ); // enemy kill counter (?)
					file.DiscardBytes( 0xAAE28 - 0xA8E08 ); // ?

					// skill equipment is stored around here

					file.DiscardBytes( 0xAC5B8 - 0xAAE28 ); // ?
					uint[] titlesUnlockedBitfield = file.ReadUInt32Array( 15, endian );

					foreach ( var title in titles.TitleList ) {
						bool haveTitle = ( ( titlesUnlockedBitfield[title.ID / 32] >> (int)( title.ID % 32 ) ) & 1 ) > 0;
						if ( haveTitle || ( ( title.BunnyGuildPointsMaybe > 0 || title.ID == 67 ) && title.Character == ( character + 1 ) ) ) {
							Console.WriteLine( ( haveTitle ? "Y" : "N" ) + ": " + inGameDic[title.NameStringDicID].StringEngOrJpn );
						}
					}
					Console.WriteLine( "===" );

					file.DiscardBytes( 0xAC938 - 0xAC5F4 ); // ?
				}


				uint collectorsBookIndex = 0;
				foreach ( var item in itemDataSorted ) {
					uint i = item.Data[(int)ItemDat.ItemData.ID];
					if ( item.Data[(int)ItemDat.ItemData.InCollectorsBook] > 0 ) {
						bool haveItem = ( ( itemBookBitfields[i / 32] >> (int)( i % 32 ) ) & 1 ) > 0;
						Console.WriteLine( ( haveItem ? "Y" : "N" ) + ( collectorsBookIndex ) + ": " + inGameDic[item.NamePointer].StringEngOrJpn );
						++collectorsBookIndex;
					}
				}


				uint monsterBookIndex = 0;
				foreach ( var enemy in enemies.EnemyList ) {
					uint i = enemy.InGameID;
					if ( enemy.InMonsterBook > 0 ) {
						bool haveSeen = ( ( monsterBookBitfieldsSeen[i / 32] >> (int)( i % 32 ) ) & 1 ) > 0;
						bool haveScanned = ( ( monsterBookBitfieldsScanned[i / 32] >> (int)( i % 32 ) ) & 1 ) > 0;
						Console.WriteLine( ( haveSeen ? "Y" : "N" ) + ( haveScanned ? "Y" : "N" ) + ( monsterBookIndex ) + ": " + inGameDic[enemy.NameStringDicID].StringEngOrJpn );
						++monsterBookIndex;
					}
				}
			}

			return 0;
		}
	}
}
