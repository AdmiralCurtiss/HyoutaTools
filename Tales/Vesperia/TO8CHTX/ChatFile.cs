using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;

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
		public Int32 Location;

		public UInt32 Name;
		public UInt32 JPN;
		public UInt32 ENG;
		public UInt32 Unknown;

		public String SName;
		public String SJPN;
		public String SENG;

		// this field does not actually exist in the game files, used as a hotfix for matching up original and modified files in GenerateWebsite/Database
		public String SNameEnglishNotUsedByGame = null;
	}

	public class ChatFile {
		public ChatFileHeader Header;
		public ChatFileLine[] Lines;

		public ChatFile( string filename, Util.GameTextEncoding encoding ) {
			using ( Stream stream = new FileStream( filename, FileMode.Open ) ) {
				LoadFile( stream, encoding );
			}
		}

		public ChatFile( Stream file, Util.GameTextEncoding encoding ) {
			LoadFile( file, encoding );
		}

		private void LoadFile( Stream TO8CHTX, Util.GameTextEncoding encoding ) {
			Header = new ChatFileHeader();

			long pos = TO8CHTX.Position;
			Header.Identify = TO8CHTX.ReadUInt64().SwapEndian();
			Header.Filesize = TO8CHTX.ReadUInt32().SwapEndian();
			Header.Lines = TO8CHTX.ReadUInt32().SwapEndian();
			Header.Unknown = TO8CHTX.ReadUInt32().SwapEndian();
			Header.TextStart = TO8CHTX.ReadUInt32().SwapEndian();
			Header.Empty = TO8CHTX.ReadUInt64().SwapEndian();

			Lines = new ChatFileLine[Header.Lines];

			for ( int i = 0; i < Header.Lines; i++ ) {
				Lines[i] = new ChatFileLine();
				Lines[i].Location = 0x20 + i * 0x10;
				Lines[i].Name = TO8CHTX.ReadUInt32().SwapEndian();
				Lines[i].JPN = TO8CHTX.ReadUInt32().SwapEndian();
				Lines[i].ENG = TO8CHTX.ReadUInt32().SwapEndian();
				Lines[i].Unknown = TO8CHTX.ReadUInt32().SwapEndian();

				Lines[i].SName = TO8CHTX.ReadNulltermStringFromLocationAndReset( pos + Lines[i].Name + Header.TextStart, encoding );
				Lines[i].SJPN = TO8CHTX.ReadNulltermStringFromLocationAndReset( pos + Lines[i].JPN + Header.TextStart, encoding );
				Lines[i].SENG = TO8CHTX.ReadNulltermStringFromLocationAndReset( pos + Lines[i].ENG + Header.TextStart, encoding ).Replace( '@', ' ' );
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

			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( Header.Identify ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( Header.Filesize ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( Header.Lines ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( Header.Unknown ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( Header.TextStart ) ) );
			Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( Header.Empty ) ) );

			foreach ( ChatFileLine Line in Lines ) {
				Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( Line.Name ) ) );
				Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( Line.ENG ) ) );
				Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( Line.ENG ) ) );
				Serialized.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( Line.Unknown ) ) );
			}

			byte ByteNull = 0x00;

			foreach ( ChatFileLine Line in Lines ) {
				Serialized.AddRange( Util.StringToBytesShiftJis( Line.SName ) );
				Serialized.Add( ByteNull );
				Serialized.AddRange( Util.StringToBytesShiftJis( Line.SENG ) );
				Serialized.Add( ByteNull );
			}

			return Serialized.ToArray();
		}

		public void RecalculatePointers() {
			uint Size = Header.TextStart;
			for ( int i = 0; i < Lines.Length; i++ ) {
				Lines[i].Name = Size - Header.TextStart;
				Size += (uint)Util.StringToBytesShiftJis( Lines[i].SName ).Length;
				Size++;
				Lines[i].JPN = Size - Header.TextStart;
				Lines[i].ENG = Size - Header.TextStart;
				Size += (uint)Util.StringToBytesShiftJis( Lines[i].SENG ).Length;
				Size++;
			}

			Header.Filesize = Size;
		}
	}
}
