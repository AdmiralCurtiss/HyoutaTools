using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.Tales.Vesperia.To8chtx {
	struct ChatFileHeader {
		public UInt64 Identify;
		public UInt32 Filesize;
		public UInt32 Lines;
		public UInt32 Unknown;
		public UInt32 TextStart;
		public UInt64 Empty;
	}

	struct ChatFileLine {
		public Int32 Location;

		public UInt32 Name;
		public UInt32 JPN;
		public UInt32 ENG;
		public UInt32 Unknown;

		public String SName;
		public String SJPN;
		public String SENG;
	}

	class ChatFile {
		ChatFileHeader Header;
		ChatFileLine[] Lines;

		public ChatFile( byte[] TO8CHTX ) {
			Header = new ChatFileHeader();

			Header.Identify = Util.SwapEndian( BitConverter.ToUInt64( TO8CHTX, 0x00 ) );
			Header.Filesize = Util.SwapEndian( BitConverter.ToUInt32( TO8CHTX, 0x08 ) );
			Header.Lines = Util.SwapEndian( BitConverter.ToUInt32( TO8CHTX, 0x0C ) );
			Header.Unknown = Util.SwapEndian( BitConverter.ToUInt32( TO8CHTX, 0x10 ) );
			Header.TextStart = Util.SwapEndian( BitConverter.ToUInt32( TO8CHTX, 0x14 ) );
			Header.Empty = Util.SwapEndian( BitConverter.ToUInt64( TO8CHTX, 0x18 ) );

			Lines = new ChatFileLine[Header.Lines];

			for ( int i = 0; i < Header.Lines; i++ ) {
				Lines[i] = new ChatFileLine();
				Lines[i].Location = 0x20 + i * 0x10;
				Lines[i].Name = Util.SwapEndian( BitConverter.ToUInt32( TO8CHTX, 0x20 + i * 0x10 ) );
				Lines[i].JPN = Util.SwapEndian( BitConverter.ToUInt32( TO8CHTX, 0x24 + i * 0x10 ) );
				Lines[i].ENG = Util.SwapEndian( BitConverter.ToUInt32( TO8CHTX, 0x28 + i * 0x10 ) );
				Lines[i].Unknown = Util.SwapEndian( BitConverter.ToUInt32( TO8CHTX, 0x2C + i * 0x10 ) );

				Lines[i].SName = GetText( TO8CHTX, Lines[i].Name + Header.TextStart );
				Lines[i].SJPN = GetText( TO8CHTX, Lines[i].JPN + Header.TextStart );
				Lines[i].SENG = GetText( TO8CHTX, Lines[i].ENG + Header.TextStart ).Replace( '@', ' ' );
			}
		}

		public String GetText( byte[] File, uint UPointer ) {
			try {
				int Pointer = (int)UPointer;
				int i = Pointer;

				while ( File[i] != 0x00 ) {
					i++;
				}
				String Text = Util.ShiftJISEncoding.GetString( File, Pointer, i - Pointer );

				return Text;
			} catch ( Exception ) {
				return null;
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
				Serialized.AddRange( Util.StringToBytes( Line.SName ) );
				Serialized.Add( ByteNull );
				Serialized.AddRange( Util.StringToBytes( Line.SENG ) );
				Serialized.Add( ByteNull );
			}

			return Serialized.ToArray();
		}

		public void RecalculatePointers() {
			uint Size = Header.TextStart;
			for ( int i = 0; i < Lines.Length; i++ ) {
				Lines[i].Name = Size - Header.TextStart;
				Size += (uint)Util.StringToBytes( Lines[i].SName ).Length;
				Size++;
				Lines[i].JPN = Size - Header.TextStart;
				Lines[i].ENG = Size - Header.TextStart;
				Size += (uint)Util.StringToBytes( Lines[i].SENG ).Length;
				Size++;
			}

			Header.Filesize = Size;
		}

		public bool InsertSQL( String ConnectionString, String ConnectionStringGracesJapanese ) {
			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			SQLiteConnection ConnectionGracesJapanese = new SQLiteConnection( ConnectionStringGracesJapanese );
			Connection.Open();
			ConnectionGracesJapanese.Open();

			SQLiteCommand EraseCommand = new SQLiteCommand( Connection );
			EraseCommand.CommandText = "DELETE FROM Text";
			EraseCommand.ExecuteNonQuery();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteTransaction TransactionGracesJapanese = ConnectionGracesJapanese.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) )
			using ( SQLiteCommand CommandGracesJapanese = new SQLiteCommand( ConnectionGracesJapanese ) )
			using ( SQLiteCommand CommandJapaneseID = new SQLiteCommand( ConnectionGracesJapanese ) )
			using ( SQLiteCommand CommandSearchJapanese = new SQLiteCommand( ConnectionGracesJapanese ) ) {
				SQLiteParameter JapaneseIDParam = new SQLiteParameter();
				SQLiteParameter JapaneseParam = new SQLiteParameter();
				SQLiteParameter EnglishIDParam = new SQLiteParameter();
				SQLiteParameter StringIDParam = new SQLiteParameter();
				SQLiteParameter EnglishParam = new SQLiteParameter();
				SQLiteParameter LocationParam = new SQLiteParameter();
				SQLiteParameter JapaneseSearchParam = new SQLiteParameter();
				SQLiteParameter EnglishStatusParam = new SQLiteParameter();

				CommandGracesJapanese.CommandText = "INSERT INTO Japanese (ID, string, debug) VALUES (?, ?, 0)";
				CommandGracesJapanese.Parameters.Add( JapaneseIDParam );
				CommandGracesJapanese.Parameters.Add( JapaneseParam );

				Command.CommandText = "INSERT INTO Text (ID, StringID, english, comment, updated, status, PointerRef) VALUES (?, ?, ?, null, 0, ?, ?)";
				Command.Parameters.Add( EnglishIDParam );
				Command.Parameters.Add( StringIDParam );
				Command.Parameters.Add( EnglishParam );  // Line.SENG
				Command.Parameters.Add( EnglishStatusParam );
				Command.Parameters.Add( LocationParam ); // Line.Location

				CommandJapaneseID.CommandText = "SELECT MAX(ID)+1 FROM Japanese";

				CommandSearchJapanese.CommandText = "SELECT ID FROM Japanese WHERE string = ?";
				CommandSearchJapanese.Parameters.Add( JapaneseSearchParam );

				int JPID;
				object JPMaxIDObject = CommandJapaneseID.ExecuteScalar();
				int JPMaxID = Int32.Parse( JPMaxIDObject.ToString() ); // wtf why doesn't this work directly?
				int ENID = 1;

				foreach ( ChatFileLine Line in Lines ) {
					// Name
					JapaneseSearchParam.Value = Line.SName;
					object JPIDobj = CommandSearchJapanese.ExecuteScalar();
					if ( JPIDobj != null ) {
						JPID = (int)JPIDobj;
					} else {
						JPID = JPMaxID++;
						JapaneseIDParam.Value = JPID;
						JapaneseParam.Value = Line.SName;
						CommandGracesJapanese.ExecuteNonQuery();
					}

					EnglishIDParam.Value = ENID;
					StringIDParam.Value = JPID;
					EnglishParam.Value = Line.SName;
					EnglishStatusParam.Value = 1;
					LocationParam.Value = Line.Location;
					Command.ExecuteNonQuery();

					ENID++;

					// Text; also fuck DRY
					JapaneseSearchParam.Value = Line.SJPN;
					JPIDobj = CommandSearchJapanese.ExecuteScalar();
					if ( JPIDobj != null ) {
						JPID = (int)JPIDobj;
					} else {
						JPID = JPMaxID++;
						JapaneseIDParam.Value = JPID;
						JapaneseParam.Value = Line.SJPN;
						CommandGracesJapanese.ExecuteNonQuery();
					}

					EnglishIDParam.Value = ENID;
					StringIDParam.Value = JPID;
					if ( Line.SENG == "Dummy" || Line.SENG == "" ) {
						EnglishParam.Value = null;
						EnglishStatusParam.Value = 0;
					} else {
						EnglishParam.Value = Line.SENG;
						EnglishStatusParam.Value = 1;
					}
					LocationParam.Value = Line.Location + 4;
					Command.ExecuteNonQuery();

					ENID++;
				}
				Transaction.Commit();
				TransactionGracesJapanese.Commit();
			}
			ConnectionGracesJapanese.Close();
			Connection.Close();

			return true;
		}
	}
}
