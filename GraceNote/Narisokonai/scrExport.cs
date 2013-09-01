using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Narisokonai;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.Narisokonai {
	class scrExport {
		public static int Export( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: db src" );
				return -1;
			}

			string InDatabase = args[0];
			string OutFile = args[1];

			scr s = new scr();
			List<GraceNoteDatabaseEntry> Entries = ReadDatabase( "Data Source=" + InDatabase );

			// init loop stuff
			s.Sections = new List<scrSection>();
			scrSection sec = new scrSection();
			sec.Elements = new List<scrElement>();
			sec.Location = (uint)Entries[0].PointerRef;
			sec.PointerIndex = Entries[0].IdentifyPointerRef;
			int SectionPointerIndex = sec.PointerIndex;

			for ( int i = 0; i < Entries.Count; ++i ) {
				GraceNoteDatabaseEntry e = Entries[i];

				if ( e.IdentifyPointerRef != SectionPointerIndex ) {
					// reached new section, write old to scr
					s.Sections.Add( sec );
					sec = new scrSection();
					sec.Elements = new List<scrElement>();
					sec.Location = (uint)e.PointerRef;
					sec.PointerIndex = e.IdentifyPointerRef;
					SectionPointerIndex = sec.PointerIndex;
				}

				scrElement screlem = new scrElement();

				switch ( e.IdentifyString ) {
					case "Code":
						screlem.Type = scrElement.scrElementType.Code;
						screlem.Code = CodeStringToArray( e.TextEN );
						break;
					case "Text":
						screlem.Type = scrElement.scrElementType.Text;
						screlem.Text = e.TextEN;
						break;
					default: throw new Exception( "scrExport: Unknown IdentifyString type!" );
				}

				sec.Elements.Add( screlem );
			}
			s.Sections.Add( sec );

			s.CreateFile( OutFile );

			return 0;
		}

		public static byte[] CodeStringToArray( string str ) {
			List<byte> bytes = new List<byte>();

			while ( true ) {
				int start = str.IndexOf( '<' );
				int end = str.IndexOf( '>' );

				if ( start == -1 ) {
					break;
				}

				string bStr = str.Substring( start + 1, ( end - start ) - 1 );
				byte b = byte.Parse( bStr, System.Globalization.NumberStyles.AllowHexSpecifier );
				bytes.Add( b );
				str = str.Substring( end + 1 );

			}

			return bytes.ToArray();
		}

		public static List<GraceNoteDatabaseEntry> ReadDatabase( string ConnectionString ) {
			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();

			List<GraceNoteDatabaseEntry> Entries = new List<GraceNoteDatabaseEntry>();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText = "SELECT english, PointerRef, IdentifyString, IdentifyPointerRef FROM Text ORDER BY PointerRef";
				SQLiteDataReader r = Command.ExecuteReader();
				while ( r.Read() ) {
					String SQLText;

					try {
						SQLText = r.GetString( 0 ).Replace( "''", "'" );
					} catch ( System.InvalidCastException ) {
						SQLText = "";
					}

					int PointerRef = r.GetInt32( 1 );


					GraceNoteDatabaseEntry e = new GraceNoteDatabaseEntry();
					e.PointerRef = PointerRef;
					e.TextEN = SQLText;
					e.IdentifyString = r.GetString( 2 );
					e.IdentifyPointerRef = r.GetInt32( 3 );
					Entries.Add( e );
				}

				Transaction.Rollback();
			}
			return Entries;
		}

	}
}
