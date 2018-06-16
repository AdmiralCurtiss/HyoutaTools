using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	public class SaveDataParser {
		public static int Parse( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: SaveDataParser SAVE" );
				Console.WriteLine( "Save must be decrypted." );
				return -1;
			}

			Util.Endianness endian = Util.Endianness.BigEndian;
			var stringDic = new TSS.TSSFile( System.IO.File.ReadAllBytes( @"c:\Dropbox\ToV\PS3\mod\string.svo.ext\STRING_DIC.SO" ) );
			var inGameDic = stringDic.GenerateInGameIdDictionary();
			var itemData = new ItemDat.ItemDat( @"c:\Dropbox\ToV\PS3\orig\item.svo.ext\ITEM.DAT" );
			var itemDataSorted = itemData.GetSortedByInGameSorting();
			var titles = new FAMEDAT.FAMEDAT( @"c:\Dropbox\ToV\PS3\orig\menu.svo.ext\FAMEDATA.BIN" );
			var enemies = new BTLBDAT.BTLBDAT( @"c:\Dropbox\ToV\PS3\orig\menu.svo.ext\BATTLEBOOKDATA.BIN" );

			using ( Stream file = new FileStream( args[0], System.IO.FileMode.Open ) ) {
				file.DiscardBytes( 0x228 ); // short header, used for save menu on 360 version to display basic info about save
				string magic = file.ReadAscii( 8 );
				if ( magic != "TO8SAVE\0" ) {
					throw new Exception( "Invalid magic byte sequence for ToV save: " + magic );
				}
				uint saveFileSize = file.ReadUInt32().FromEndian( endian );
				if ( saveFileSize != 0xCCAA0 ) {
					throw new Exception( "Unexpected filesize for ToV save: " + magic );
				}
				file.DiscardBytes( 0xA3D14 ); // no idea what all this is
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
				file.DiscardBytes( 0x15C8 ); // ?

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
			}

			return 0;
		}
	}
}
