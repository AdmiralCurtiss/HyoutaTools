using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	public class SaveDataBlock {
		public string BlockName;
		public Streams.DuplicatableStream BlockStream;

		public SaveDataBlock( Streams.DuplicatableStream saveDataStream, uint headerPosition, uint dataStart, uint refStringStart, Util.Endianness endian ) {
			saveDataStream.Position = headerPosition;
			uint refStringPos = saveDataStream.ReadUInt32().FromEndian( endian );
			uint offset = saveDataStream.ReadUInt32().FromEndian( endian );
			uint size = saveDataStream.ReadUInt32().FromEndian( endian );
			BlockStream = new Streams.PartialStream( saveDataStream, dataStart + offset, size );
			BlockName = saveDataStream.ReadAsciiNulltermFromLocationAndReset( refStringStart + refStringPos );
		}

		public override string ToString() {
			return BlockStream.Length + " bytes; " + BlockName;
		}
	}

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
			var enemies = new T8BTEMST.T8BTEMST( @"c:\Dropbox\ToV\PS3\orig\btl.svo.ext\BTL_PACK.DAT.ext\0005.ext\ALL.0000", endian, Util.Bitness.B32 );

			using ( Streams.DuplicatableFileStream file = new Streams.DuplicatableFileStream( args[0] ) ) {
				Streams.DuplicatableStream saveMenuStream = new Streams.PartialStream( file, 0, 0x228 ); // short header, used for save menu on non-PS3 versions to display basic info about save
				Streams.DuplicatableStream saveDataStream = new Streams.PartialStream( file, 0x228, file.Length - 0x228 ); // actual save file
				string magic = saveDataStream.ReadAscii( 8 );
				if ( magic != "TO8SAVE\0" ) {
					throw new Exception( "Invalid magic byte sequence for ToV save: " + magic );
				}
				uint saveFileSize = saveDataStream.ReadUInt32().FromEndian( endian );
				saveDataStream.DiscardBytes( 0x14 ); // seemingly unused
				uint sectionMetadataBlockStart = saveDataStream.ReadUInt32().FromEndian( endian );
				uint sectionCount = saveDataStream.ReadUInt32().FromEndian( endian );
				uint dataStart = saveDataStream.ReadUInt32().FromEndian( endian );
				uint refStringStart = saveDataStream.ReadUInt32().FromEndian( endian );

				List<SaveDataBlock> blocks = new List<SaveDataBlock>();
				for ( uint i = 0; i < sectionCount; ++i ) {
					blocks.Add( new SaveDataBlock( saveDataStream, sectionMetadataBlockStart + i * 0x20, dataStart, refStringStart, endian ) );
				}

				// save point flags, one byte each, 0x00 not visted 0x01 visited
				SaveDataBlock savePointFlagBlock = blocks.Where( x => x.BlockName == "SavePoint" ).First();
				{
					savePointFlagBlock.BlockStream.ReStart();
					byte[] savePointFlags = savePointFlagBlock.BlockStream.ReadUInt8Array( 0x59 );
					savePointFlagBlock.BlockStream.End();
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

				SaveDataBlock partyDataBlock = blocks.Where( x => x.BlockName == "PARTY_DATA" ).First();
				partyDataBlock.BlockStream.ReStart();
				partyDataBlock.BlockStream.ReadUInt32().FromEndian( endian ); // ?
				partyDataBlock.BlockStream.ReadUInt32().FromEndian( endian ); // ?
				partyDataBlock.BlockStream.ReadUInt32Array( 9, endian );
				partyDataBlock.BlockStream.ReadUInt32().FromEndian( endian ); // play time in frames, assuming 60 frames = 1 second
				partyDataBlock.BlockStream.ReadUInt32().FromEndian( endian ); // gald
				partyDataBlock.BlockStream.DiscardBytes( 4 ); // ?
				uint[] itemCounts = partyDataBlock.BlockStream.ReadUInt32Array( 3072, endian );
				uint[] itemBookBitfields = partyDataBlock.BlockStream.ReadUInt32Array( 3072 / 32, endian );
				partyDataBlock.BlockStream.DiscardBytes( 4 ); // ?
				partyDataBlock.BlockStream.ReadUInt32Array( 4, endian ); // control modes for the four active party slots
				partyDataBlock.BlockStream.ReadUInt32Array( 3, endian ); // strategies assigned to dpad directions
				partyDataBlock.BlockStream.DiscardBytes( 0x40 ); // ??
				for ( int i = 0; i < 8; ++i ) {
					// custom strategy names
					// game seems to read these till null byte so this could totally be abused to buffer overflow...
					partyDataBlock.BlockStream.ReadAscii( 0x40 );
				}

				partyDataBlock.BlockStream.DiscardBytes( 0xA84D0 - 0xA7360 ); // ?
				uint[] monsterBookBitfieldsScanned = partyDataBlock.BlockStream.ReadUInt32Array( 0x48 / 4, endian );
				partyDataBlock.BlockStream.DiscardBytes( 0xA8680 - 0xA8518 ); // ?
				uint[] monsterBookBitfieldsSeen = partyDataBlock.BlockStream.ReadUInt32Array( 0x48 / 4, endian );
				partyDataBlock.BlockStream.DiscardBytes( 0xA8928 - 0xA86C8 ); // ?
				partyDataBlock.BlockStream.End();

				// 9 character blocks, each 0x4010 bytes
				for ( int character = 0; character < 9; ++character ) {
					SaveDataBlock characterDataBlock = blocks.Where( x => x.BlockName == "PC_STATUS" + ( character + 1 ).ToString( "D1" ) ).First();
					var characterDataStream = characterDataBlock.BlockStream;
					characterDataStream.ReadUInt32().FromEndian( endian ); // ?
					characterDataStream.ReadAscii( 0x40 ); // custom character name
					characterDataStream.ReadUInt32().FromEndian( endian ); // character ID
					characterDataStream.ReadUInt32().FromEndian( endian ); // level
					characterDataStream.ReadUInt32().FromEndian( endian ); // current HP
					characterDataStream.ReadUInt32().FromEndian( endian ); // current TP
					characterDataStream.ReadUInt32().FromEndian( endian ); // max HP
					characterDataStream.ReadUInt32().FromEndian( endian ); // max TP
					characterDataStream.ReadUInt32().FromEndian( endian ); // ?
					characterDataStream.ReadUInt32().FromEndian( endian ); // EXP
					characterDataStream.ReadUInt32().FromEndian( endian ); // ?
					characterDataStream.ReadUInt32().FromEndian( endian ); // ?
					characterDataStream.ReadUInt32().FromEndian( endian ); // base attack
					characterDataStream.ReadUInt32().FromEndian( endian ); // base magic attack
					characterDataStream.ReadUInt32().FromEndian( endian ); // base def
					characterDataStream.ReadUInt32().FromEndian( endian ); // base mdef
					characterDataStream.ReadUInt32().FromEndian( endian ); // ?
					characterDataStream.ReadUInt32().FromEndian( endian ); // base agility
					characterDataStream.ReadUInt32().FromEndian( endian ); // luck
					characterDataStream.ReadUInt32().FromEndian( endian ); // ?
					characterDataStream.ReadUInt32().FromEndian( endian ); // ?
					characterDataStream.ReadUInt32().FromEndian( endian ); // base attack attribute fire
					characterDataStream.ReadUInt32().FromEndian( endian ); // base attack attribute earth
					characterDataStream.ReadUInt32().FromEndian( endian ); // base attack attribute wind
					characterDataStream.ReadUInt32().FromEndian( endian ); // base attack attribute water
					characterDataStream.ReadUInt32().FromEndian( endian ); // base attack attribute light
					characterDataStream.ReadUInt32().FromEndian( endian ); // base attack attribute dark
					characterDataStream.ReadUInt32().FromEndian( endian ); // base attack attribute physical...?
					characterDataStream.ReadUInt32().FromEndian( endian ); // base damage multiplier fire
					characterDataStream.ReadUInt32().FromEndian( endian ); // base damage multiplier earth
					characterDataStream.ReadUInt32().FromEndian( endian ); // base damage multiplier wind
					characterDataStream.ReadUInt32().FromEndian( endian ); // base damage multiplier water
					characterDataStream.ReadUInt32().FromEndian( endian ); // base damage multiplier light
					characterDataStream.ReadUInt32().FromEndian( endian ); // base damage multiplier dark
					characterDataStream.ReadUInt32().FromEndian( endian ); // base damage multiplier physical?
					characterDataStream.DiscardBytes( 0xA8A60 - 0xA89F0 ); // ?
					characterDataStream.ReadUInt32().FromEndian( endian ); // modified attack (base + from equipment)
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod def
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod matk
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod mdef
					characterDataStream.ReadUInt32().FromEndian( endian ); // ?
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod agility
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod luck
					characterDataStream.ReadUInt32().FromEndian( endian ); // ?
					characterDataStream.ReadUInt32().FromEndian( endian ); // ?
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod attack attribute fire
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod attack attribute earth
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod attack attribute wind
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod attack attribute water
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod attack attribute light
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod attack attribute dark
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod attack attribute physical...?
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod damage multiplier fire
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod damage multiplier earth
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod damage multiplier wind
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod damage multiplier water
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod damage multiplier light
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod damage multiplier dark
					characterDataStream.ReadUInt32().FromEndian( endian ); // mod damage multiplier physical?
					characterDataStream.DiscardBytes( 0xA8E04 - 0xA8ABC ); // ?
					characterDataStream.ReadUInt32().FromEndian( endian ); // enemy kill counter (?)
					characterDataStream.DiscardBytes( 0xAAE28 - 0xA8E08 ); // ?

					// skill equipment is stored around here

					characterDataStream.DiscardBytes( 0xAC5B8 - 0xAAE28 ); // ?
					uint[] titlesUnlockedBitfield = characterDataStream.ReadUInt32Array( 15, endian );

					foreach ( var title in titles.TitleList ) {
						bool haveTitle = ( ( titlesUnlockedBitfield[title.ID / 32] >> (int)( title.ID % 32 ) ) & 1 ) > 0;
						if ( haveTitle || ( ( title.BunnyGuildPointsMaybe > 0 || title.ID == 67 ) && title.Character == ( character + 1 ) ) ) {
							Console.WriteLine( ( haveTitle ? "Y" : "N" ) + ": " + inGameDic[title.NameStringDicID].StringEngOrJpn );
						}
					}
					Console.WriteLine( "===" );

					characterDataStream.DiscardBytes( 0xAC938 - 0xAC5F4 ); // ?
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
