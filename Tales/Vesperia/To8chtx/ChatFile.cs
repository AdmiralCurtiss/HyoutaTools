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
