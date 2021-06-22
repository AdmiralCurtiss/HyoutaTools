using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using HyoutaUtils;
using EndianUtils = HyoutaUtils.EndianUtils;

namespace HyoutaTools.Tales.Vesperia.TO8CHTX {
	public struct ChatFileHeader {
		public UInt64 Identify;
		public UInt32 Filesize;
		public UInt32 Lines;
		public UInt32 Unknown;
		public UInt32 TextStart;
		public UInt64 Empty;
	}

	public class ChatFileLine {
		public int Location;

		public ulong NamePointer;
		public ulong[] TextPointers = new ulong[2];
		public uint Unknown;

		public string SName;
		public string[] STexts = new string[2];

		public ulong JPN { get { return TextPointers[0]; } set { TextPointers[0] = value; } }
		public ulong ENG { get { return TextPointers[1]; } set { TextPointers[1] = value; } }
		public string SJPN { get { return STexts[0]; } set { STexts[0] = value; } }
		public string SENG { get { return STexts[1]; } set { STexts[1] = value; } }

		// this field does not actually exist in the game files, used as a hotfix for matching up original and modified files in GenerateWebsite/Database
		public string SNameEnglishNotUsedByGame = null;
	}

	public class ChatFile {
		public ChatFileHeader Header;
		public ChatFileLine[] Lines;

		public ChatFile( string filename, EndianUtils.Endianness endian, TextUtils.GameTextEncoding encoding, BitUtils.Bitness bits, int languageCount ) {
			using ( Stream stream = new FileStream( filename, FileMode.Open, FileAccess.Read ) ) {
				LoadFile( stream, endian, encoding, bits, languageCount );
			}
		}

		public ChatFile( Stream file, EndianUtils.Endianness endian, TextUtils.GameTextEncoding encoding, BitUtils.Bitness bits, int languageCount ) {
			LoadFile( file, endian, encoding, bits, languageCount );
		}

		private void LoadFile( Stream TO8CHTX, EndianUtils.Endianness endian, TextUtils.GameTextEncoding encoding, BitUtils.Bitness bits, int languageCount ) {
			Header = new ChatFileHeader();

			ulong pos = (ulong)TO8CHTX.Position;
			Header.Identify = TO8CHTX.ReadUInt64().FromEndian( endian );
			Header.Filesize = TO8CHTX.ReadUInt32().FromEndian( endian );
			Header.Lines = TO8CHTX.ReadUInt32().FromEndian( endian );
			Header.Unknown = TO8CHTX.ReadUInt32().FromEndian( endian );
			Header.TextStart = TO8CHTX.ReadUInt32().FromEndian( endian );
			Header.Empty = TO8CHTX.ReadUInt64().FromEndian( endian );

			Lines = new ChatFileLine[Header.Lines];

			int entrySize = (int)( 4 + ( languageCount + 1 ) * bits.NumberOfBytes() );
			for ( int i = 0; i < Header.Lines; i++ ) {
				Lines[i] = new ChatFileLine();
				Lines[i].Location = 0x20 + i * entrySize;
				Lines[i].NamePointer = TO8CHTX.ReadUInt( bits, endian );
				Lines[i].TextPointers = new ulong[languageCount];
				for ( int j = 0; j < languageCount; ++j ) {
					Lines[i].TextPointers[j] = TO8CHTX.ReadUInt( bits, endian );
				}
				Lines[i].Unknown = TO8CHTX.ReadUInt32().FromEndian( endian );

				Lines[i].SName = TO8CHTX.ReadNulltermStringFromLocationAndReset( (long)( pos + Lines[i].NamePointer + Header.TextStart ), encoding );
				Lines[i].STexts = new string[languageCount];
				for ( int j = 0; j < languageCount; ++j ) {
					Lines[i].STexts[j] = TO8CHTX.ReadNulltermStringFromLocationAndReset( (long)( pos + Lines[i].TextPointers[j] + Header.TextStart ), encoding ).Replace( '@', ' ' );
				}
			}
		}

		public void GetSQL( String ConnectionString ) {
			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText = "SELECT english, PointerRef FROM Text ORDER BY PointerRef";
				SQLiteDataReader r = Command.ExecuteReader();
				while ( r.Read() ) {
					String SQLText;

					try {
						SQLText = r.GetString( 0 ).Replace( "''", "'" );
					} catch ( System.InvalidCastException ) {
						SQLText = null;
					}

					int PointerRef = r.GetInt32( 1 );

					if ( !String.IsNullOrEmpty( SQLText ) ) {
						if ( PointerRef % 16 == 0 ) {
							int i = ( PointerRef / 16 ) - 2;
							Lines[i].SName = SQLText;
						} else if ( PointerRef % 16 == 4 ) {
							int i = ( ( PointerRef - 4 ) / 16 ) - 2;
							Lines[i].SENG = SQLText;
						}
					}
				}

				Transaction.Rollback();
			}
			return;
		}

		public byte[] Serialize() {
			List<byte> Serialized = new List<byte>( (int)Header.Filesize );

			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( Header.Identify ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( Header.Filesize ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( Header.Lines ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( Header.Unknown ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( Header.TextStart ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( Header.Empty ) ) );

			foreach ( ChatFileLine Line in Lines ) {
				Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( (uint)Line.NamePointer ) ) );
				Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( (uint)Line.ENG ) ) );
				Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( (uint)Line.ENG ) ) );
				Serialized.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( Line.Unknown ) ) );
			}

			byte ByteNull = 0x00;

			foreach ( ChatFileLine Line in Lines ) {
				Serialized.AddRange( TextUtils.StringToBytesShiftJis( Line.SName ) );
				Serialized.Add( ByteNull );
				Serialized.AddRange( TextUtils.StringToBytesShiftJis( Line.SENG ) );
				Serialized.Add( ByteNull );
			}

			return Serialized.ToArray();
		}

		public void RecalculatePointers() {
			uint Size = Header.TextStart;
			for ( int i = 0; i < Lines.Length; i++ ) {
				Lines[i].NamePointer = Size - Header.TextStart;
				Size += (uint)TextUtils.StringToBytesShiftJis( Lines[i].SName ).Length;
				Size++;
				Lines[i].JPN = Size - Header.TextStart;
				Lines[i].ENG = Size - Header.TextStart;
				Size += (uint)TextUtils.StringToBytesShiftJis( Lines[i].SENG ).Length;
				Size++;
			}

			Header.Filesize = Size;
		}
	}
}
