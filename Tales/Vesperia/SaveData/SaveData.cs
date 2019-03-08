using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	public class SaveData {
		public SaveMenuBlock MenuBlock;
		public SaveDataBlockScenario Scenario;
		public SaveDataBlockFieldCamera FieldCamera;
		public SaveDataBlockFieldArea FieldArea;
		public SaveDataBlockFieldCar FieldCar;
		public SaveDataBlockCamp Camp;
		public SaveDataBlockFieldSave FieldSave;
		public SaveDataBlockStandbyEnemy StandbyEnemy;
		public SaveDataBlockTerasureSave TerasureSave;
		public SaveDataBlockTreasureSaveData TreasureSaveData;
		public SaveDataBlockCustomData CustomData;
		public SaveDataBlockSoundTheater SoundTheater;
		public SaveDataBlockSavePoint SavePoint;
		public SaveDataBlockMG2Poker MG2Poker;
		public SaveDataBlockSnowBoard SnowBoard;
		public SaveDataBlockPartyData PartyData;
		public SaveDataBlockPCStatus[] CharacterData = new SaveDataBlockPCStatus[9];
		public SaveDataBlockFieldGadget FieldGadget;

		public SaveData( Streams.DuplicatableStream stream, Util.Endianness endian ) {
			MenuBlock = new SaveMenuBlock( new Streams.PartialStream( stream, 0, 0x228 ) );

			// actual save file
			using ( Streams.DuplicatableStream saveDataStream = new Streams.PartialStream( stream, 0x228, stream.Length - 0x228 ) ) {
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

				for ( uint i = 0; i < sectionCount; ++i ) {
					uint headerPosition = sectionMetadataBlockStart + i * 0x20;
					saveDataStream.Position = headerPosition;
					uint refStringPos = saveDataStream.ReadUInt32().FromEndian( endian );
					uint offset = saveDataStream.ReadUInt32().FromEndian( endian );
					uint size = saveDataStream.ReadUInt32().FromEndian( endian );
					var blockStream = new Streams.PartialStream( saveDataStream, dataStart + offset, size );
					string blockName = saveDataStream.ReadAsciiNulltermFromLocationAndReset( refStringStart + refStringPos );

					if ( blockName.StartsWith( "PC_STATUS" ) ) {
						int idx = int.Parse( blockName.Substring( "PC_STATUS".Length ) ) - 1;
						CharacterData[idx] = new SaveDataBlockPCStatus( blockStream );
					} else {
						switch ( blockName ) {
							case "Scenario": Scenario = new SaveDataBlockScenario( blockStream ); break;
							case "FieldCamera": FieldCamera = new SaveDataBlockFieldCamera( blockStream ); break;
							case "FieldArea": FieldArea = new SaveDataBlockFieldArea( blockStream ); break;
							case "FieldCar": FieldCar = new SaveDataBlockFieldCar( blockStream ); break;
							case "Camp": Camp = new SaveDataBlockCamp( blockStream ); break;
							case "FIELD_SAVE": FieldSave = new SaveDataBlockFieldSave( blockStream ); break;
							case "STANDBYENEMY": StandbyEnemy = new SaveDataBlockStandbyEnemy( blockStream ); break;
							case "TERASURE_SAVE": TerasureSave = new SaveDataBlockTerasureSave( blockStream ); break;
							case "TreasureSaveData": TreasureSaveData = new SaveDataBlockTreasureSaveData( blockStream ); break;
							case "CUSTOM_DATA": CustomData = new SaveDataBlockCustomData( blockStream ); break;
							case "SoundTheater": SoundTheater = new SaveDataBlockSoundTheater( blockStream ); break;
							case "SavePoint": SavePoint = new SaveDataBlockSavePoint( blockStream ); break;
							case "MG2Poker": MG2Poker = new SaveDataBlockMG2Poker( blockStream ); break;
							case "SnowBoard": SnowBoard = new SaveDataBlockSnowBoard( blockStream ); break;
							case "PARTY_DATA": PartyData = new SaveDataBlockPartyData( blockStream ); break;
							case "FieldGadget": FieldGadget = new SaveDataBlockFieldGadget( blockStream ); break;
							default: Console.WriteLine( "Unknown save data block '" + blockName + "'" ); break;
						}
					}
				}
			}
		}

		public void ExportTo( string path ) {
			Directory.CreateDirectory( path );
			ExportSingle( MenuBlock?.Stream, Path.Combine( path, "_SaveMenu" ) );
			ExportSingle( Scenario?.Stream, Path.Combine( path, "Scenario" ) );
			ExportSingle( FieldCamera?.Stream, Path.Combine( path, "FieldCamera" ) );
			ExportSingle( FieldArea?.Stream, Path.Combine( path, "FieldArea" ) );
			ExportSingle( FieldCar?.Stream, Path.Combine( path, "FieldCar" ) );
			ExportSingle( Camp?.Stream, Path.Combine( path, "Camp" ) );
			ExportSingle( FieldSave?.Stream, Path.Combine( path, "FIELD_SAVE" ) );
			ExportSingle( StandbyEnemy?.Stream, Path.Combine( path, "STANDBYENEMY" ) );
			ExportSingle( TerasureSave?.Stream, Path.Combine( path, "TERASURE_SAVE" ) );
			ExportSingle( TreasureSaveData?.Stream, Path.Combine( path, "TreasureSaveData" ) );
			ExportSingle( CustomData?.Stream, Path.Combine( path, "CUSTOM_DATA" ) );
			ExportSingle( SoundTheater?.Stream, Path.Combine( path, "SoundTheater" ) );
			ExportSingle( SavePoint?.Stream, Path.Combine( path, "SavePoint" ) );
			ExportSingle( MG2Poker?.Stream, Path.Combine( path, "MG2Poker" ) );
			ExportSingle( SnowBoard?.Stream, Path.Combine( path, "SnowBoard" ) );
			ExportSingle( PartyData?.Stream, Path.Combine( path, "PARTY_DATA" ) );
			for ( int i = 0; i < CharacterData.Length; ++i ) {
				ExportSingle( CharacterData[i]?.Stream, Path.Combine( path, "PC_STATUS" + i.ToString( "D1" ) ) );
			}
			ExportSingle( FieldGadget?.Stream, Path.Combine( path, "FieldGadget" ) );
		}

		private static void ExportSingle( Streams.DuplicatableStream stream, string path ) {
			if ( stream != null ) {
				using ( var s = stream.Duplicate() )
				using ( var f = new FileStream( path, FileMode.Create ) ) {
					Util.CopyStream( s, f, s.Length );
				}
			}
		}
	}
}
