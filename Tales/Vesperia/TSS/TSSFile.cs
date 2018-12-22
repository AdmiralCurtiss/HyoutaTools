using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.TSS {
	public class TSSFile {
		public TSSHeader Header;
		public TSSEntry[] Entries;

		public TSSFile( string filename, bool isUtf8 = false ) {
			using ( Stream stream = new FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream, isUtf8 ) ) {
					throw new Exception( "Loading TSSFile failed!" );
				}
			}
		}

		public TSSFile( Stream stream, bool isUtf8 = false ) {
			if ( !LoadFile( stream, isUtf8 ) ) {
				throw new Exception( "Loading TSSFile failed!" );
			}
		}

		private bool LoadFile( Stream File, bool isUtf8 = false ) {
			Util.GameTextEncoding encoding = isUtf8 ? Util.GameTextEncoding.UTF8 : Util.GameTextEncoding.ShiftJIS;
			long pos = File.Position;

			// set header
			Header = new TSSHeader( File );

			if ( Header.Magic != 0x54535300 ) {
				Console.WriteLine( "File is not a TSS file!" );
				return false;
			}

			// convert all instructions into a List of uint
			File.Position = pos + 0x20;
			int CurrentLocation = 0x20;
			UInt32 EntriesEnd = Header.TextStart;
			List<uint> EntryUIntList = new List<uint>();

			while ( CurrentLocation < EntriesEnd ) {
				uint Instruction = File.ReadUInt32().SwapEndian();
				EntryUIntList.Add( Instruction );
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
				int inGameStringId = -1;

				// get in-game string id
				for ( i = 0; i < OneEntry.Length; i++ ) {
					if ( OneEntry[i] == 0x0E000007 ) {
						inGameStringId = (int)OneEntry[i - 2];
						break;
					}
				}

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

				string jpn = JPNPointer == -1 ? null : File.ReadNulltermStringFromLocationAndReset( pos + JPNPointer, encoding );
				string eng = ENGPointer == -1 ? null : File.ReadNulltermStringFromLocationAndReset( pos + ENGPointer, encoding );
				EntryList.Add( new TSSEntry( OneEntry, jpn, eng, JPNIndex, ENGIndex, inGameStringId ) );
				//CurrentLocation += OneEntry.Length;
				CurrentLocation++;
			}
			Entries = EntryList.ToArray();

			return true;
		}

		public Dictionary<uint, TSSEntry> GenerateInGameIdDictionary() {
			Dictionary<uint, TSSEntry> stringIdDict = new Dictionary<uint, TSSEntry>();
			foreach ( var e in Entries ) {
				if ( e.inGameStringId > -1 ) {
					stringIdDict.Add( (uint)e.inGameStringId, e );
				}
			}
			return stringIdDict;
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
				if ( Entries[i].StringJpn != null ) {
					Lines.Add( i.ToString() + "j: " + Entries[i].StringJpn.Replace( newlineS, backslashnS ) );
				}
				if ( Entries[i].StringEng != null ) {
					Lines.Add( i.ToString() + "e: " + Entries[i].StringEng.Replace( newlineS, backslashnS ) );
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
				if ( Entries[i].StringJpn != null ) {
					//Lines.Add(Entries[i].StringJPN);
				}
				if ( Entries[i].StringEng != null ) {
					if (
						( Entries[i].StringEng.Trim() == "" || Entries[i].StringEng.Trim() == "北米専用。右に直接書き込んでください→" )
						&& Entries[i].StringJpn != null ) {
						Lines.Add( RemoveTags( Entries[i].StringJpn ) );
					} else {
						Lines.Add( RemoveTags( Entries[i].StringEng ) );
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
						Entries[number].StringJpn = s.Substring( location + 1 );
					} else if ( language == 'e' ) {
						Entries[number].StringEng = s.Substring( location + 1 );
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

				if ( e.StringJpn != null ) {
					e.SetJPNPointer( ptr );
					uint StringLength = (uint)Util.StringToBytesShiftJis( e.StringJpn ).Length;
					CurrentPointer += StringLength + 1;
					if ( e.StringEng != null ) {
						e.SetENGPointer( ptr + StringLength );
					}
				}
				if ( e.StringEng != null && e.StringEng != "" ) {
					e.SetENGPointer( CurrentPointer - Header.TextStart );
					CurrentPointer += (uint)Util.StringToBytesShiftJis( e.StringEng ).Length + 1;
				}
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
				if ( e.StringJpn != null ) {
					Serialized.AddRange( Util.StringToBytesShiftJis( e.StringJpn ) );
					Serialized.Add( 0x00 );
				}
				if ( e.StringEng != null && e.StringEng != "" ) {
					Serialized.AddRange( Util.StringToBytesShiftJis( e.StringEng ) );
					Serialized.Add( 0x00 );
				}
			}

			return Serialized.ToArray();
		}

		public bool ImportSQL( bool placeEnglishInJpnEntry = true, bool updateDatabaseWithInGameStringId = false, bool generateGracesEnglish = false ) {
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
			SQLiteConnection connectionGracesEnglish = null;
			SQLiteTransaction transactionGracesEnglish = null;
			if ( generateGracesEnglish ) {
				connectionGracesEnglish = new SQLiteConnection( "Data Source=db/GracesEnglish" );
				connectionGracesEnglish.Open();
				transactionGracesEnglish = connectionGracesEnglish.BeginTransaction();
			}

			// stuff to import the NPC text area names into GN
			Dictionary<uint, string> npcInGameIdMapDict = null;
			if ( updateDatabaseWithInGameStringId ) {
				string npcListFilename = @"d:\Dropbox\ToV\PS3\orig\npc.svo.ext\NPC.DAT.dec.ext\0000.dec";
				if ( System.IO.File.Exists( npcListFilename ) ) {
					var npcListPS3 = new TOVNPC.TOVNPCL( npcListFilename );
					Dictionary<string, TOVNPC.TOVNPCT> npcDefs = new Dictionary<string, TOVNPC.TOVNPCT>();
					foreach ( var f in npcListPS3.NpcFileList ) {
						string filename = @"d:\Dropbox\ToV\PS3\orig\npc.svo.ext\" + f.Filename + @".dec.ext\0001.dec";
						if ( System.IO.File.Exists( filename ) ) {
							var d = new TOVNPC.TOVNPCT( filename );
							npcDefs.Add( f.Map, d );
						}
					}
					npcInGameIdMapDict = new Dictionary<uint, string>();
					foreach ( var kvp in npcDefs ) {
						foreach ( var npcText in kvp.Value.NpcDefList ) {
							npcInGameIdMapDict.Add( npcText.StringDicId, kvp.Key );
						}
					}
				}
			}

			foreach ( String CurrentDBName in DBNames ) {
				Console.WriteLine( "   Importing " + CurrentDBName + "..." );
				SQLiteConnection Connection = new SQLiteConnection( "Data Source=db/" + CurrentDBName );
				Connection.Open();
				SQLiteCommand Command = new SQLiteCommand( "SELECT english, PointerRef, StringID FROM Text", Connection );
				SQLiteDataReader Reader = Command.ExecuteReader();
				while ( Reader.Read() ) {
					String English = Reader.GetString( 0 );
					int PointerRef = Reader.GetInt32( 1 );

					if ( updateDatabaseWithInGameStringId ) {
						UpdateDatabaseWithInGameStringId( Connection, PointerRef, Entries[PointerRef + 1].inGameStringId, npcInGameIdMapDict );
					}
					if ( generateGracesEnglish ) {
						string englishOriginal = Entries[PointerRef + 1].StringEng;
						int gracesJapaneseId = Reader.GetInt32( 2 );
						UpdateGracesJapanese( transactionGracesEnglish, gracesJapaneseId, englishOriginal, 0 );
					}

					if ( Entries[PointerRef + 1].StringJpn != null
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
						if ( placeEnglishInJpnEntry ) {
							Entries[PointerRef + 1].StringJpn = English;
						} else {
							Entries[PointerRef + 1].StringEng = English;
						}
					}
				}
				Reader.Close();
				Reader.Dispose();
				Connection.Close();
				Connection.Dispose();
			}

			if ( generateGracesEnglish ) {
				transactionGracesEnglish.Commit();
				connectionGracesEnglish.Close();
			}

			return true;
		}

		private static void UpdateGracesJapanese( SQLiteTransaction ta, int id, string originalString, int debug ) {
			// CREATE TABLE Japanese(ID INT PRIMARY KEY, string TEXT, debug INT)
			long exists = (long)SqliteUtil.SelectScalar( ta, "SELECT COUNT(1) FROM Japanese WHERE ID = ?", new object[1] { id } );

			if ( exists > 0 ) {
				SqliteUtil.Update( ta, "UPDATE Japanese SET string = ?, debug = ? WHERE ID = ?", new object[3] { originalString, debug, id } );
			} else {
				SqliteUtil.Update( ta, "INSERT INTO Japanese (ID, string, debug) VALUES (?, ?, ?)", new object[3] { id, originalString, debug } );
			}
		}

		private static void UpdateDatabaseWithInGameStringId( SQLiteConnection conn, int pointerref, int ingameid, Dictionary<uint, string> npcInGameIdMapDict = null ) {
			if ( ingameid > -1 ) {
				string areaName = "";
				if ( npcInGameIdMapDict != null && npcInGameIdMapDict.ContainsKey( (uint)ingameid ) ) {
					areaName = "[" + npcInGameIdMapDict[(uint)ingameid] + "] ";
				}
				string inGameIdText = " [" + ingameid + " / 0x" + ingameid.ToString( "X6" ) + "]";

				string UPDATE = "UPDATE Text SET IdentifyString = \"" + areaName + "\" || IdentifyString || \"" + inGameIdText + "\" WHERE PointerRef = " + pointerref;
				Console.WriteLine( UPDATE );
				SQLiteCommand command = new SQLiteCommand( UPDATE, conn );
				command.ExecuteNonQuery();
				command.Dispose();
			}
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
