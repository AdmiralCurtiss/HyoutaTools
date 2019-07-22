using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaPluginBase;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x3F38 bytes in 360, 0x4010 bytes in PS3
	// 9 blocks of this in each save file, one for each party member
	// contains character-specific state like stats, artes, skills, etc.
	public class SaveDataBlockPCStatus {
		public DuplicatableStream Stream;

		public SaveDataBlockPCStatus( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}

		public void PrintData( EndianUtils.Endianness endian, GameVersion version, Dictionary<uint, TSS.TSSEntry> inGameDic, FAMEDAT.FAMEDAT titles ) {
			using ( var characterDataStream = Stream.Duplicate() ) {
				characterDataStream.ReadUInt32().FromEndian( endian ); // ?
				characterDataStream.ReadAscii( 0x40 ); // custom character name
				uint character = characterDataStream.ReadUInt32().FromEndian( endian ); // character ID
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
					if ( haveTitle || ( ( title.BunnyGuildPointsMaybe > 0 || ( !version.Is360() && title.ID == 67 ) ) && title.Character == character ) ) {
						Console.WriteLine( ( haveTitle ? "Y" : "N" ) + ": " + inGameDic[title.NameStringDicID].StringEngOrJpn );
					}
				}

				characterDataStream.DiscardBytes( 0xAC938 - 0xAC5F4 ); // ?
			}
		}
	}
}
