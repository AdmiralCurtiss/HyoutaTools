using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTVA {
	public class T8BTVA {
		public T8BTVA( String filename, Util.Endianness endian ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream, endian ) ) {
					throw new Exception( "Loading T8BTVA failed!" );
				}
			}
		}

		public T8BTVA( Stream stream, Util.Endianness endian ) {
			if ( !LoadFile( stream, endian ) ) {
				throw new Exception( "Loading T8BTVA failed!" );
			}
		}

		public List<T8BTVABlock> Blocks;

		private bool LoadFile( Stream stream, Util.Endianness endian ) {
			string magic = stream.ReadAscii( 8 );
			uint blockCount = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );

			Blocks = new List<T8BTVABlock>( (int)blockCount );
			for ( uint i = 0; i < blockCount; ++i ) {
				Blocks.Add( new T8BTVABlock( stream, refStringStart, endian ) );
			}

			return true;
		}
	}

	public class T8BTVABlock {
		public uint Size;
		public uint IndexInArray;
		public uint IndexInGame;
		public uint IdentifierLocation;
		public string Identifier;
		public uint[] CharacterSpecificData;
		public uint EntryCount;

		public List<T8BTVAEntry> Entries;

		public T8BTVABlock( System.IO.Stream stream, uint refStringStart, Util.Endianness endian ) {
			uint size = stream.PeekUInt32().FromEndian( endian );
			if ( size % 4 != 0 ) { throw new Exception( "T8BTVABlock size must be divisible by 4!" ); }
			if ( size / 4 < 14 ) { throw new Exception( "T8BTVABlock must contain at least 14 ints!" ); }
			Size = stream.ReadUInt32().FromEndian( endian );
			IndexInArray = stream.ReadUInt32().FromEndian( endian );
			IndexInGame = stream.ReadUInt32().FromEndian( endian );
			IdentifierLocation = stream.ReadUInt32().FromEndian( endian );
			CharacterSpecificData = new uint[9];
			for ( int i = 0; i < 9; ++i ) {
				CharacterSpecificData[i] = stream.ReadUInt32().FromEndian( endian );
			}
			EntryCount = stream.ReadUInt32().FromEndian( endian );

			Identifier = stream.ReadAsciiNulltermFromLocationAndReset( IdentifierLocation + refStringStart );

			Entries = new List<T8BTVAEntry>( (int)EntryCount );
			for ( uint i = 0; i < EntryCount; ++i ) {
				Entries.Add( new T8BTVAEntry( stream, refStringStart, endian ) );
			}
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append( Identifier );
			sb.Append( ": [ " );
			for ( int i = 0; i < Entries.Count; ++i ) {
				sb.Append( Entries[i].ToString() );
				sb.Append( ", " );
			}
			sb.Append( "]" );
			return sb.ToString();
		}
	}

	public class T8BTVAEntry {
		public uint[] Data;
		public string RefString;
		public uint ScenarioStart;
		public uint ScenarioEnd;
		public uint CharacterBitmask;
		public uint KillCharacterBitmask;

		public T8BTVAEntry( System.IO.Stream stream, uint refStringStart, Util.Endianness endian ) {
			Data = new uint[33];
			for ( int i = 0; i < 33; ++i ) {
				Data[i] = stream.ReadUInt32().FromEndian( endian );
			}

			RefString = stream.ReadAsciiNulltermFromLocationAndReset( refStringStart + Data[22] );
			ScenarioStart = Data[2];
			ScenarioEnd = Data[3];
			CharacterBitmask = Data[5];
			KillCharacterBitmask = Data[6];
		}

		public override string ToString() {
			return RefString;
		}
	}
}
