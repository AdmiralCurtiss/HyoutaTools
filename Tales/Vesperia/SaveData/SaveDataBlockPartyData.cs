using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;
using HyoutaPluginBase;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x49D8 bytes in all versions
	// contains stuff like items, item book, monster book, and so on; basically all non-character-specific menu state
	public class SaveDataBlockPartyData {
		public DuplicatableStream Stream;

		public SaveDataBlockPartyData( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}

		public void PrintData( EndianUtils.Endianness endian, Dictionary<uint, TSS.TSSEntry> inGameDic, List<ItemDat.ItemDatSingle> itemDataSorted, T8BTEMST.T8BTEMST enemies ) {
			using ( DuplicatableStream stream = Stream.Duplicate() ) {
				stream.ReadUInt32().FromEndian( endian ); // ?
				stream.ReadUInt32().FromEndian( endian ); // ?
				stream.ReadUInt32Array( 9, endian );
				stream.ReadUInt32().FromEndian( endian ); // play time in frames, assuming 60 frames = 1 second
				stream.ReadUInt32().FromEndian( endian ); // gald
				stream.DiscardBytes( 4 ); // ?
				uint[] itemCounts = stream.ReadUInt32Array( 3072, endian );
				uint[] itemBookBitfields = stream.ReadUInt32Array( 3072 / 32, endian );
				stream.DiscardBytes( 4 ); // ?
				stream.ReadUInt32Array( 4, endian ); // control modes for the four active party slots
				stream.ReadUInt32Array( 3, endian ); // strategies assigned to dpad directions
				stream.DiscardBytes( 0x40 ); // ??
				for ( int i = 0; i < 8; ++i ) {
					// custom strategy names
					// game seems to read these till null byte so this could totally be abused to buffer overflow...
					stream.ReadAscii( 0x40 );
				}

				stream.DiscardBytes( 0xA84D0 - 0xA7360 ); // ?
				uint[] monsterBookBitfieldsScanned = stream.ReadUInt32Array( 0x48 / 4, endian );
				stream.DiscardBytes( 0xA8680 - 0xA8518 ); // ?
				uint[] monsterBookBitfieldsSeen = stream.ReadUInt32Array( 0x48 / 4, endian );
				stream.DiscardBytes( 0xA8928 - 0xA86C8 ); // ?

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
		}
	}
}
