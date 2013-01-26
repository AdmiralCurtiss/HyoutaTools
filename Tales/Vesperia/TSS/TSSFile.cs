using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace HyoutaTools.Tales.Vesperia.TSS {
	public class TSSFile {
		public TSSHeader Header;
		public TSSEntry[] Entries;
		public byte[] File;

		public TSSFile( byte[] File ) {
			this.File = File;
			// set header
			Header = new TSSHeader( File.Take( 0x20 ).ToArray() );

			if ( Header.Magic != 0x54535300 ) {
				Console.WriteLine( "File is not a TSS file!" );
				return;
			}

			// convert all instructions into a List of uint
			int CurrentLocation = 0x20;
			UInt32 EntriesEnd = Header.TextStart;
			List<uint> EntryUIntList = new List<uint>();

			while ( CurrentLocation < EntriesEnd ) {
				uint Instruction = BitConverter.ToUInt32( File, CurrentLocation );
				EntryUIntList.Add( Util.SwapEndian( Instruction ) );
				CurrentLocation += 4;
			}

			// split the full instruction list into blocks seperated by 0xFFFFFFFF ( == TSSEntry )
			// and put them into the Entry list
			// complete with text it's pointing at
			CurrentLocation = 0;
			uint[] EntryUIntArray = EntryUIntList.ToArray();
			int ListSize = EntryUIntArray.Length;
			List<TSSEntry> EntryList = new List<TSSEntry>( ListSize / 10 );
			int i = 0;

			List<uint> OneEntryList = new List<uint>();

			while ( CurrentLocation < ListSize ) {
				//uint[] OneEntry = EntryUIntArray.Skip(CurrentLocation).TakeWhile(subject => subject != 0xFFFFFFFF).ToArray();
				OneEntryList.Clear();

				while ( CurrentLocation < EntryUIntArray.Length && EntryUIntArray[CurrentLocation] != 0xFFFFFFFF ) {
					OneEntryList.Add( EntryUIntArray[CurrentLocation] );
					CurrentLocation++;
				}

				uint[] OneEntry = OneEntryList.ToArray();

				int JPNPointer = -1;
				int ENGPointer = -1;
				int JPNIndex = -1;
				int ENGIndex = -1;

				// get JPN pointer
				for ( i = 0; i < OneEntry.Length; i++ ) {
					if ( OneEntry[i] == 0x02820000 ) {
						break;
					}
				}

				if ( i == OneEntry.Length ) {
					JPNPointer = -1;
					ENGPointer = -1;
				} else {
					JPNIndex = ++i;
					JPNPointer = (int)( OneEntry[JPNIndex] + Header.TextStart );

					// get English pointer
					for ( ; i < OneEntry.Length; i++ ) {
						if ( OneEntry[i] == 0x02820000 ) {
							break;
						}
					}

					if ( i == OneEntry.Length ) {
						ENGPointer = -1;
					} else {
						ENGIndex = i + 1;
						ENGPointer = (int)( OneEntry[ENGIndex] + Header.TextStart );
					}
				}

				EntryList.Add( new TSSEntry( OneEntry, Util.GetTextShiftJis( File, JPNPointer ), Util.GetTextShiftJis( File, ENGPointer ), JPNIndex, ENGIndex ) );
				//CurrentLocation += OneEntry.Length;
				CurrentLocation++;
			}
			Entries = EntryList.ToArray();
		}

		public byte[] ExportText() {
			List<string> Lines = new List<string>( Entries.Length * 3 );

			char[] newline = { '\n' };
			String newlineS = new String( newline );
			//char[] backslashn = { '\\', 'n' };
			char[] backslashn = { '↵' };
			String backslashnS = new String( backslashn );
			byte[] newlinereturn = { 0x0D, 0x0A };

			for ( int i = 0; i < Entries.Length; i++ ) {
				if ( Entries[i].StringJPN != null ) {
					Lines.Add( i.ToString() + "j: " + Entries[i].StringJPN.Replace( newlineS, backslashnS ) );
				}
				if ( Entries[i].StringENG != null ) {
					Lines.Add( i.ToString() + "e: " + Entries[i].StringENG.Replace( newlineS, backslashnS ) );
				}
				Lines.Add( "" );
			}

			List<byte> Serialized = new List<byte>( (int)( Header.TextLength ) );

			foreach ( String s in Lines ) {
				Serialized.AddRange( Encoding.UTF8.GetBytes( s ) );
				Serialized.AddRange( newlinereturn );
			}

			return Serialized.ToArray();
		}


		public byte[] ExportTextForEnglishDump() {
			List<string> Lines = new List<string>( Entries.Length * 3 );

			char[] newline = { '\n' };
			String newlineS = new String( newline );
			char[] backslashn = { '\\', 'n' };
			String backslashnS = new String( backslashn );
			byte[] newlinereturn = { 0x0D, 0x0A };

			for ( int i = 0; i < Entries.Length; i++ ) {
				if ( Entries[i].StringJPN != null ) {
					//Lines.Add(Entries[i].StringJPN);
				}
				if ( Entries[i].StringENG != null ) {
					if (
						( Entries[i].StringENG.Trim() == "" || Entries[i].StringENG.Trim() == "北米専用。右に直接書き込んでください→" )
						&& Entries[i].StringJPN != null ) {
						Lines.Add( RemoveTags( Entries[i].StringJPN ) );
					} else {
						Lines.Add( RemoveTags( Entries[i].StringENG ) );
					}
				}
				Lines.Add( "" );
			}

			List<byte> Serialized = new List<byte>( (int)( Header.TextLength ) );

			foreach ( String s in Lines ) {
				Serialized.AddRange( Encoding.UTF8.GetBytes( s ) );
				Serialized.AddRange( newlinereturn );
			}

			return Serialized.ToArray();
		}


		public void ImportText( byte[] TextFile ) {
			List<string> Lines = new List<string>( Entries.Length * 3 );

			char[] newline = { '\n' };
			String newlineS = new String( newline );
			char[] backslashn = { '\\', 'n' };
			String backslashnS = new String( backslashn );
			byte[] newlinereturn = { 0x0D, 0x0A };

			int LineStart = 0;
			for ( int i = 0; i < TextFile.Length - 1; i++ ) {
				if ( TextFile[i] == 0x0D && TextFile[i + 1] == 0x0A ) {
					try {
						int len = ( i - LineStart );
						if ( len < 1 ) {
							LineStart = i + 2;
							continue;
						}
						String CurrLine = Encoding.UTF8.GetString( TextFile, LineStart, len );
						CurrLine = CurrLine.Replace( backslashnS, newlineS );
						Lines.Add( CurrLine );
						LineStart = i + 2;
					} catch ( Exception ) {

					}
				}
			}
			String CurrLine2 = Encoding.UTF8.GetString( TextFile, LineStart, TextFile.Length - LineStart );
			CurrLine2 = CurrLine2.Replace( backslashnS, newlineS );
			Lines.Add( CurrLine2 );

			foreach ( String s in Lines ) {
				try {
					int location = s.IndexOf( ':' );
					String StringNumber = s.Substring( 0, location - 1 );
					char language = s[location - 1];
					int number = Int32.Parse( StringNumber );
					if ( language == 'j' ) {
						Entries[number].StringJPN = s.Substring( location + 1 );
					} else if ( language == 'e' ) {
						Entries[number].StringENG = s.Substring( location + 1 );
					}
				} catch ( Exception ) { }
			}

		}

		public byte[] Serialize() {
			uint TextStartBuffer = 0x20;

			// recalculate all pointers
			uint CurrentPointer = Header.TextStart + TextStartBuffer;
			foreach ( TSSEntry e in Entries ) {
				uint ptr = CurrentPointer - Header.TextStart;

				if ( e.StringJPN != null ) {
					e.SetJPNPointer( ptr );
					uint StringLength = (uint)Util.StringToBytesShiftJis( e.StringJPN ).Length;
					CurrentPointer += StringLength + 1;
					if ( e.StringENG != null ) {
						e.SetENGPointer( ptr + StringLength );
					}
				}
				// Only exporting JP and putting a pointer to that into both ENG and JPN slots
				/* 
				if (e.StringENG != null)
				{
					e.SetENGPointer(CurrentPointer - Header.TextStart);
					CurrentPointer += (uint)Util.StringToBytes(e.StringENG).Length + 1;
				}
				//*/
			}


			Header.TextLength = CurrentPointer - Header.TextStart;

			List<byte> Serialized = new List<byte>( (int)( Header.TextStart + Header.TextLength ) );
			//serialize
			Serialized.AddRange( Header.Serialize() );
			byte[] delimiter = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
			foreach ( TSSEntry e in Entries ) {
				Serialized.AddRange( e.SerializeScript() );
				Serialized.AddRange( delimiter );
			}
			Serialized.RemoveRange( Serialized.Count - 4, 4 );
			for ( int i = 0; i < TextStartBuffer; i++ ) {
				Serialized.Add( 0x00 );
			}
			foreach ( TSSEntry e in Entries ) {
				if ( e.StringJPN != null ) {
					Serialized.AddRange( Util.StringToBytesShiftJis( e.StringJPN ) );
					Serialized.Add( 0x00 );
				}
				/* Only exporting JP and putting a pointer to that into both ENG and JPN slots
				if (e.StringENG != null)
				{
					Serialized.AddRange(Util.StringToBytes(e.StringENG));
					Serialized.Add(0x00);
				}
				 * */
			}

			return Serialized.ToArray();
		}

		public bool ImportSQL() {
			String[] DBNames = {
                                   "VGeneral", "VMenu", "VArtes", "VSkills",
                                   "VStrategy", "VLocation Names", "VEnemies",
                                   "VGroups", "VItem General", "VItem Offensive",
                                   "VItem Defensive", "VItem Accessories", "VItem Food",
                                   "VItem Synth Ingredients", "VItem Special",
                                   "VItem Attachments", "VDLC", "VShops", "VRecipes",
                                   "VEtc", "VLocations", "VGrade Bonus", "VGrade Shop",
                                   "VTitles", "VLogs", "VMini Game Text", "VSkit Titles",
                                   "VMisc", "VMisc2", "VTutorial", "VAilments", "VSigns",
                                   "VMap Locations", "VCredits", "VOther Text",
                                   "VEvent Jumps", "VSkit Text"
                               };

			char[] bx06 = { '', '(' };
			String x06 = new String( bx06 );

			foreach ( String CurrentDBName in DBNames ) {
				Console.WriteLine( "   Importing " + CurrentDBName + "..." );
				SQLiteConnection Connection = new SQLiteConnection( "Data Source=db/" + CurrentDBName );
				Connection.Open();
				SQLiteCommand Command = new SQLiteCommand( "SELECT english, PointerRef FROM Text", Connection );
				SQLiteDataReader Reader = Command.ExecuteReader();
				while ( Reader.Read() ) {
					String English = Reader.GetString( 0 );
					int PointerRef = Reader.GetInt32( 1 );
					if ( Entries[PointerRef + 1].StringJPN != null
						 && !String.IsNullOrEmpty( English )
					   ) {
						English = English.Replace( "''", "'" );
						/*
						if (English.Contains("<Unk6:"))
						{
							English = English.Replace("<Unk6: ", x06);
							English = English.Replace(">", ")");
						}
						*/
						Entries[PointerRef + 1].StringJPN = English;
					}
				}
				Reader.Close();
				Reader.Dispose();
				Connection.Close();
				Connection.Dispose();
			}

			return true;
		}

		private static String RemoveTags( String s ) {
			s = s.Replace( "''", "'" );
			s = s.Replace( "(YUR)", "Yuri" );
			s = s.Replace( "(EST)", "Estellise" );
			s = s.Replace( "(EST_P)", "Estelle" );
			s = s.Replace( "(RIT)", "Rita" );
			s = s.Replace( "(KAR)", "Karol" );
			s = s.Replace( "(RAP)", "Repede" );
			s = s.Replace( "(RAV)", "Raven" );
			s = s.Replace( "(JUD)", "Judith" );
			s = s.Replace( "(FRE)", "Flynn" );
			s = s.Replace( "(PAT)", "Patty" );
			s = s.Replace( "(JUD_P)", "Judy" );
			s = s.Replace( "", " " );
			s = Regex.Replace( s, "\t[(][A-Za-z0-9_]+[)]", "" ); // voice command
			return s;
		}
	}
}
